﻿#region References

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Speedy.Exceptions;
using Speedy.Extensions;
using Speedy.Logging;

#endregion

namespace Speedy.Sync
{
	/// <summary>
	/// Represents a sync manager for syncing two clients.
	/// </summary>
	public abstract class SyncManager<T> : Bindable where T : Enum
	{
		#region Fields

		private CancellationTokenSource _cancellationToken;
		private AverageTimer _syncTimer;
		private T _syncType;
		private readonly Stopwatch _watch;

		#endregion

		#region Constructors

		/// <summary>
		/// Instantiates a sync manager for syncing two clients.
		/// </summary>
		/// <param name="dispatcher"> The dispatcher to update with. </param>
		protected SyncManager(IDispatcher dispatcher) : base(dispatcher)
		{
			_syncType = default;
			_watch = new Stopwatch();

			ProcessTimeout = TimeSpan.FromMilliseconds(60000);
			SessionId = Guid.NewGuid();
			ShowProgressThreshold = TimeSpan.FromMilliseconds(1000);
			SyncIssues = new List<SyncIssue>();
			SyncState = new SyncEngineState(Dispatcher);
			SyncOptions = new ConcurrentDictionary<T, SyncOptions>();
			SyncTimers = new ConcurrentDictionary<T, AverageTimer>();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets an optional incoming converter to convert incoming sync data. The converter is applied to the local sync client.
		/// </summary>
		public SyncClientIncomingConverter IncomingConverter { get; protected set; }

		/// <summary>
		/// Gets a value indicating the running sync is requesting to be cancelled.
		/// </summary>
		public bool IsCancellationPending => _cancellationToken?.Token.IsCancellationRequested ?? false;

		/// <summary>
		/// Gets a value indicating the running status of the sync manager.
		/// </summary>
		public bool IsRunning => _cancellationToken != null || Monitor.IsEntered(_watch) || _watch.IsRunning;

		/// <summary>
		/// Gets a value indicating if the last sync was successful.
		/// </summary>
		public bool IsSyncSuccessful { get; protected set; }

		/// <summary>
		/// Gets an optional outgoing converter to convert incoming sync data. The converter is applied to the local sync client.
		/// </summary>
		public SyncClientOutgoingConverter OutgoingConverter { get; protected set; }

		/// <summary>
		/// The timeout to be used when synchronously syncing.
		/// </summary>
		public TimeSpan ProcessTimeout { get; set; }

		/// <summary>
		/// The session ID of the sync manager.
		/// </summary>
		public Guid SessionId { get; set;}

		/// <summary>
		/// Gets a flag to indicate progress should be shown. Will only be true if sync takes longer than the <seealso cref="ShowProgressThreshold" />.
		/// </summary>
		public bool ShowProgress => _watch.IsRunning && _watch.Elapsed >= ShowProgressThreshold;

		/// <summary>
		/// Gets the value to determine when to trigger <seealso cref="ShowProgress" />. Defaults to one second.
		/// </summary>
		public TimeSpan ShowProgressThreshold { get; set; }

		/// <summary>
		/// Gets the list of issues that occurred during the last sync.
		/// </summary>
		public IList<SyncIssue> SyncIssues { get; }

		/// <summary>
		/// The configure sync options for the sync manager.
		/// </summary>
		/// <seealso cref="GetOrAddSyncOptions" />
		protected ConcurrentDictionary<T, SyncOptions> SyncOptions { get; }

		/// <summary>
		/// The configure sync timers for the sync manager.
		/// </summary>
		protected ConcurrentDictionary<T, AverageTimer> SyncTimers { get; }

		/// <summary>
		/// Gets the current sync state.
		/// </summary>
		public SyncEngineState SyncState { get; }

		/// <summary>
		/// The version of the sync system. Update this version any time the sync system changed dramatically
		/// </summary>
		public abstract Version SyncSystemVersion { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Cancels the current running sync.
		/// </summary>
		/// <param name="timeout"> The timeout to wait for the sync to cancel. </param>
		public void CancelSync(TimeSpan? timeout = null)
		{
			OnLogEvent($"Cancelling running Sync {_syncType}...", EventLevel.Verbose);

			_cancellationToken?.Cancel();
			
			OnPropertyChanged(nameof(IsCancellationPending));

			var watch = Stopwatch.StartNew();

			while (IsRunning)
			{
				if (timeout != null && watch.Elapsed.TotalMilliseconds >= timeout.Value.TotalMilliseconds)
				{
					OnLogEvent($"Timed out waiting for the Sync {_syncType} to complete after being cancelled.", EventLevel.Verbose);
					return;
				}

				Thread.Sleep(25);
			}

			OnLogEvent($"The Sync {_syncType} has been cancelled", EventLevel.Verbose);
		}

		/// <summary>
		/// Reset the sync dates on all sync options
		/// </summary>
		/// <param name="lastSyncedOnClient"> The last time when synced on the client. </param>
		/// <param name="lastSyncedOnServer"> The last time when synced on the server. </param>
		public void ResetSyncDates(DateTime lastSyncedOnClient, DateTime lastSyncedOnServer)
		{
			foreach (var collectionOptions in SyncOptions.Values)
			{
				collectionOptions.LastSyncedOnClient = lastSyncedOnClient;
				collectionOptions.LastSyncedOnServer = lastSyncedOnServer;
			}
		}

		/// <summary>
		/// Wait for the sync to complete.
		/// </summary>
		/// <param name="timeout"> An optional max amount of time to wait. ProcessTimeout will be used it no timeout provided. </param>
		/// <returns> True if the sync completed otherwise false if timed out waiting. </returns>
		public bool WaitForSyncToComplete(TimeSpan? timeout = null)
		{
			if (!IsRunning)
			{
				return true;
			}

			var watch = Stopwatch.StartNew();
			timeout ??= ProcessTimeout;

			while (IsRunning)
			{
				if (watch.Elapsed >= timeout)
				{
					return false;
				}

				Thread.Sleep(10);
			}

			return true;
		}
		
		/// <summary>
		/// Wait for the sync to start.
		/// </summary>
		/// <param name="timeout"> An optional max amount of time to wait. ProcessTimeout will be used it no timeout provided. </param>
		/// <returns> True if the sync was started otherwise false if timed out waiting. </returns>
		public bool WaitForSyncToStart(TimeSpan? timeout = null)
		{
			if (IsRunning)
			{
				return true;
			}

			var watch = Stopwatch.StartNew();
			timeout ??= ProcessTimeout;

			while (!IsRunning)
			{
				if (watch.Elapsed >= timeout)
				{
					return false;
				}

				Thread.Sleep(10);
			}

			return true;
		}

		/// <summary>
		/// Create the sync option for the provided sync type.
		/// </summary>
		/// <param name="syncType"> The type to create sync options for. </param>
		/// <returns> The sync options for the type. </returns>
		protected virtual SyncOptions CreateSyncOptions(T syncType)
		{
			return new SyncOptions
			{
				Id = syncType.ToString(),
				LastSyncedOnClient = DateTime.MinValue,
				LastSyncedOnServer = DateTime.MinValue,
				// note: everything below is a request, the sync clients (web sync controller)
				// has the options to override. Ex: you may request 300 items then the sync
				// client may reduce it to only 100 items.
				PermanentDeletions = false,
				ItemsPerSyncRequest = 300,
				IncludeIssueDetails = false
			};
		}

		/// <summary>
		/// Gets the default sync options for a sync manager.
		/// </summary>
		/// <param name="syncType"> The type of sync these options are for. </param>
		/// <param name="update"> Optional update action to change provided defaults. </param>
		/// <returns> The default set of options. </returns>
		/// <remarks>
		/// This should only be use in the sync manager constructor.
		/// </remarks>
		protected SyncOptions GetOrAddSyncOptions(T syncType, Action<SyncOptions> update = null)
		{
			return SyncOptions.GetOrAdd(syncType, key =>
			{
				if (SyncOptions.ContainsKey(syncType))
				{
					return SyncOptions[syncType];
				}

				var options = CreateSyncOptions(key);

				options.Values.AddOrUpdate(Sync.SyncOptions.SyncKey, ((int) (object) syncType).ToString());
				options.Values.AddOrUpdate(Sync.SyncOptions.SyncVersionKey, SyncSystemVersion.ToString(4));

				// optional update to modify sync options
				update?.Invoke(options);

				SyncOptions.GetOrAdd(syncType, options);

				return options;
			});
		}

		/// <summary>
		/// Gets the sync client to be used in the sync engine client input.
		/// </summary>
		/// <returns> The sync client. </returns>
		protected abstract ISyncClient GetSyncClientForClient();

		/// <summary>
		/// Gets the sync client to be used in the sync engine client input.
		/// </summary>
		/// <returns> The sync client. </returns>
		protected abstract ISyncClient GetSyncClientForServer();

		/// <summary>
		/// Gets the sync options by the provide sync type.
		/// </summary>
		/// <param name="syncType"> The sync type to get options for. </param>
		/// <returns> The sync options for the type </returns>
		protected SyncOptions GetSyncOptions(T syncType)
		{
			return SyncOptions.ContainsKey(syncType) ? SyncOptions[syncType] : null;
		}

		/// <summary>
		/// Write a message to the log.
		/// </summary>
		/// <param name="message"> The message to be written. </param>
		/// <param name="level"> The level of this message. </param>
		protected virtual void OnLogEvent(string message, EventLevel level)
		{
			Logger.Instance.Write(SessionId, message, level);
		}

		/// <summary>
		/// Indicate the sync is complete.
		/// </summary>
		/// <param name="options"> The options of the completed sync. </param>
		protected virtual void OnSyncCompleted(SyncOptions options)
		{
			SyncCompleted?.Invoke(this, options);
		}

		/// <summary>
		/// Indicate the sync is being updated.
		/// </summary>
		/// <param name="state"> The state of the sync. </param>
		protected virtual void OnSyncUpdated(SyncEngineState state)
		{
			SyncUpdated?.Invoke(this, state);
		}

		/// <summary>
		/// Processes a sync request.
		/// </summary>
		/// <param name="syncType"> The type of the sync to process. </param>
		/// <param name="updateOptions"> The action to possibly update options when the sync starts. </param>
		/// <param name="waitFor"> Optional timeout to wait for the active sync to complete. </param>
		/// <param name="postAction"> 
		/// An optional action to run after sync is completed but before notification goes out. If the sync cannot
		/// start then the options will be null as they were never read or set.
		/// </param>
		/// <returns> The task for the process. </returns>
		protected Task ProcessAsync(T syncType, Action<SyncOptions> updateOptions, TimeSpan? waitFor = null, Action<SyncOptions> postAction = null)
		{
			if (IsRunning)
			{
				if (waitFor == null)
				{
					OnLogEvent($"Sync {_syncType} is already running so Sync {syncType} not started.", EventLevel.Verbose);
					postAction?.Invoke(null);
					return Task.CompletedTask;
				}

				// See if we are going to force current sync to stop
				OnLogEvent($"Waiting for Sync {_syncType} to complete...", EventLevel.Verbose);
			}

			// Lock the sync before we start, wait until 
			var enteredLock = WaitForLock(waitFor ?? TimeSpan.Zero);
			if (!enteredLock)
			{
				OnLogEvent($"Failed to Sync {syncType} because current Sync {_syncType} never completed while waiting.", EventLevel.Verbose);
				postAction?.Invoke(null);
				return Task.CompletedTask;
			}

			try
			{
				// Start the sync before we start the task
				StartSync(syncType);

				// Start the sync in a background thread.
				return Task.Run(() => RunSync(syncType, updateOptions, postAction), _cancellationToken.Token);
			}
			finally
			{
				if (Monitor.IsEntered(_watch))
				{
					// Release the monitor
					Monitor.Exit(_watch);
				}
			}
		}

		private bool WaitForLock(TimeSpan timeout)
		{ 
			var watch = Stopwatch.StartNew();

			// Wait for an existing sync
			while (IsRunning && _watch.Elapsed < timeout)
			{
				Thread.Sleep(10);
			}

			// See if we have timed out, if so just return false
			if (IsRunning)
			{
				// The sync is still running so return false
				return false;
			}

			// Calculate new timeout due to previous processing time
			timeout -= watch.Elapsed;
			
			// Ensure timeout does not go negative
			if (timeout.Ticks < 0)
			{
				timeout = TimeSpan.Zero;
			}

			var enteredLock = false;
			Monitor.TryEnter(_watch, timeout, ref enteredLock);
			return enteredLock;
		}

		/// <summary>
		/// Wait on a task to be completed.
		/// </summary>
		/// <param name="task"> The task to wait for. </param>
		/// <param name="timeout">
		/// A TimeSpan that represents the number of milliseconds to wait, or
		/// a TimeSpan that represents -1 milliseconds to wait indefinitely.
		/// </param>
		protected void WaitOnTask(Task task, TimeSpan? timeout)
		{
			Task.WaitAll(new[] { task }, timeout ?? ProcessTimeout);
		}

		/// <summary>
		/// Run the sync. This should only be called by ProcessAsync.
		/// </summary>
		/// <param name="syncType"> The type of the sync to process. </param>
		/// <param name="updateOptions"> The action to possibly update options when the sync starts. </param>
		/// <param name="postAction"> 
		/// An optional action to run after sync is completed but before notification goes out. If the sync cannot
		/// start then the options will be null as they were never read or set.
		/// </param>
		private void RunSync(T syncType, Action<SyncOptions> updateOptions, Action<SyncOptions> postAction = null)
		{
			SyncOptions options = null;

			try
			{
				options = GetSyncOptions(syncType);
				updateOptions?.Invoke(options);

				OnLogEvent($"Syncing {syncType} for {options.LastSyncedOnClient}, {options.LastSyncedOnServer}", EventLevel.Verbose);

				var client = GetSyncClientForClient();
				var server = GetSyncClientForServer();
				var engine = new SyncEngine(client, server, options, _cancellationToken);

				engine.SyncStateChanged += async (sender, state) =>
				{
					SyncState.UpdateWith(state);
					OnSyncUpdated(state);

					if (Dispatcher != null)
					{
						await Dispatcher.RunAsync(() =>
						{
							OnPropertyChanged(nameof(IsCancellationPending));
							OnPropertyChanged(nameof(IsRunning));
							OnPropertyChanged(nameof(ShowProgress));
						});
					}
				};

				engine.Run();

				SyncIssues.AddRange(engine.SyncIssues);
				IsSyncSuccessful = !_cancellationToken.IsCancellationRequested && !SyncIssues.Any();
			}
			catch (WebClientException ex)
			{
				switch (ex.Code)
				{
					case HttpStatusCode.Unauthorized:
						SyncState.Message = "Unauthorized: please update your credentials in settings or contact support.";
						break;

					default:
						SyncState.Message = ex.Message;
						break;
				}

				IsSyncSuccessful = false;
				OnSyncUpdated(SyncState);
			}
			catch (Exception ex)
			{
				IsSyncSuccessful = false;
				SyncState.Message = ex.Message;
				OnSyncUpdated(SyncState);
			}
			finally
			{
				try
				{
					postAction?.Invoke(options);
				}
				catch (Exception ex)
				{
					OnLogEvent(ex.Message, EventLevel.Error);
				}

				try
				{
					OnSyncCompleted(options);
				}
				catch (Exception ex)
				{
					OnLogEvent(ex.Message, EventLevel.Error);
				}

				StopSync(syncType);
			}
		}


		private void StartSync(T syncType)
		{
			// See if we have a timer for this sync type
			if (SyncTimers.TryGetValue(syncType, out _syncTimer))
			{
				_syncTimer.Start();
			}

			// Start with sync failed so only successful sync will work
			IsSyncSuccessful = false;
			SyncIssues.Clear();

			OnLogEvent($"Sync {syncType} started", EventLevel.Verbose);

			_syncType = syncType;
			_cancellationToken = new CancellationTokenSource();
			_watch.Restart();

			OnPropertyChanged(nameof(IsCancellationPending));
			OnPropertyChanged(nameof(IsRunning));
			OnPropertyChanged(nameof(ShowProgress));
		}

		private void StopSync(T syncType)
		{
			if (_syncTimer != null)
			{
				_syncTimer?.Stop();
				
				OnLogEvent($"Sync {syncType} stopped. {_syncTimer.Average:mm\\:ss\\.fff}", EventLevel.Verbose);

				_syncTimer = null;
			}
			else
			{
				OnLogEvent($"Sync {syncType} stopped", EventLevel.Verbose);
			}

			_watch.Stop();
			_cancellationToken?.Dispose();
			_cancellationToken = null;

			OnPropertyChanged(nameof(IsCancellationPending));
			OnPropertyChanged(nameof(IsRunning));
			OnPropertyChanged(nameof(ShowProgress));
		}

		#endregion

		#region Events

		/// <summary>
		/// Indicates the sync is completed.
		/// </summary>
		public event EventHandler<SyncOptions> SyncCompleted;

		/// <summary>
		/// Indicates the sync is being updated.
		/// </summary>
		public event EventHandler<SyncEngineState> SyncUpdated;

		#endregion
	}
}