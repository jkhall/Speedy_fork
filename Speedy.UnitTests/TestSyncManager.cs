﻿#region References

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Speedy.Net;
using Speedy.Sync;

#endregion

namespace Speedy.UnitTests
{
	public class TestSyncManager : SyncManager<TestSyncType>
	{
		#region Constructors

		public TestSyncManager() : this(new DefaultDispatcher())
		{
		}

		public TestSyncManager(IDispatcher dispatcher) : base(dispatcher)
		{
			// Setup our sync options
			GetOrAddSyncOptions(TestSyncType.All, options => { });
			GetOrAddSyncOptions(TestSyncType.Accounts, options => { });
			GetOrAddSyncOptions(TestSyncType.Addresses, options => { });

			// Setup tracking of certain syncs
			GetOrAddSyncTimer(TestSyncType.All);
			GetOrAddSyncTimer(TestSyncType.Accounts);
		}

		#endregion

		#region Methods

		public SyncResults<TestSyncType> Sync(TimeSpan? testDelay = null, TimeSpan? waitFor = null, Action<SyncResults<TestSyncType>> postAction = null)
		{
			return WaitOnTask(SyncAsync(testDelay, waitFor, postAction), waitFor ?? ProcessTimeout);
		}

		public SyncResults<TestSyncType> SyncAccounts(TimeSpan? testDelay = null, TimeSpan? waitFor = null, Action<SyncResults<TestSyncType>> postAction = null)
		{
			return WaitOnTask(SyncAccountsAsync(testDelay, waitFor, postAction), waitFor ?? ProcessTimeout);
		}

		public Task<SyncResults<TestSyncType>> SyncAccountsAsync(TimeSpan? testDelay = null, TimeSpan? waitFor = null, Action<SyncResults<TestSyncType>> postAction = null)
		{
			return ProcessAsync(TestSyncType.Accounts, options => DoTestDelay(testDelay), waitFor, postAction);
		}

		public SyncResults<TestSyncType> SyncAddresses(TimeSpan? testDelay = null, TimeSpan? waitFor = null, Action<SyncResults<TestSyncType>> postAction = null)
		{
			return WaitOnTask(SyncAddressesAsync(testDelay, waitFor, postAction), waitFor ?? ProcessTimeout);
		}

		public Task<SyncResults<TestSyncType>> SyncAddressesAsync(TimeSpan? testDelay = null, TimeSpan? waitFor = null, Action<SyncResults<TestSyncType>> postAction = null)
		{
			return ProcessAsync(TestSyncType.Addresses, options => DoTestDelay(testDelay), waitFor, postAction);
		}

		public Task<SyncResults<TestSyncType>> SyncAsync(TimeSpan? testDelay = null, TimeSpan? waitFor = null, Action<SyncResults<TestSyncType>> postAction = null)
		{
			return ProcessAsync(TestSyncType.All, options => DoTestDelay(testDelay), waitFor, postAction);
		}

		protected override ISyncClient GetSyncClientForClient()
		{
			var corrections = new List<SyncObject>();
			var client = new Mock<ISyncClient>();
			var statistics = new SyncStatistics();

			client.Setup(x => x.Statistics).Returns(() => statistics);

			client.Setup(x => x.BeginSync(It.IsAny<Guid>(), It.IsAny<SyncOptions>()))
				.Returns<Guid, SyncOptions>((i, o) => new SyncSession { Id = i, StartedOn = TimeService.UtcNow });

			client.Setup(x => x.GetChanges(It.IsAny<Guid>(), It.IsAny<SyncRequest>()))
				.Returns<Guid, SyncRequest>((id, x) => new ServiceResult<SyncObject>());

			client.Setup(x => x.ApplyChanges(It.IsAny<Guid>(), It.IsAny<ServiceRequest<SyncObject>>()))
				.Returns<Guid, ServiceRequest<SyncObject>>((id, x) => new ServiceResult<SyncIssue>());

			client.Setup(x => x.ApplyCorrections(It.IsAny<Guid>(), It.IsAny<ServiceRequest<SyncObject>>()))
				.Returns<Guid, ServiceRequest<SyncObject>>((id, x) =>
				{
					corrections.AddRange(x.Collection);
					return new ServiceResult<SyncIssue>();
				});

			return client.Object;
		}

		protected override ISyncClient GetSyncClientForServer()
		{
			var corrections = new List<SyncObject>();
			var client = new Mock<ISyncClient>();
			var statistics = new SyncStatistics();

			client.Setup(x => x.Statistics).Returns(() => statistics);

			client.Setup(x => x.BeginSync(It.IsAny<Guid>(), It.IsAny<SyncOptions>()))
				.Returns<Guid, SyncOptions>((i, o) => new SyncSession { Id = i, StartedOn = TimeService.UtcNow });

			client.Setup(x => x.GetChanges(It.IsAny<Guid>(), It.IsAny<SyncRequest>()))
				.Returns<Guid, SyncRequest>((id, x) => new ServiceResult<SyncObject>());

			client.Setup(x => x.ApplyChanges(It.IsAny<Guid>(), It.IsAny<ServiceRequest<SyncObject>>()))
				.Returns<Guid, ServiceRequest<SyncObject>>((id, x) => new ServiceResult<SyncIssue>());

			client.Setup(x => x.ApplyCorrections(It.IsAny<Guid>(), It.IsAny<ServiceRequest<SyncObject>>()))
				.Returns<Guid, ServiceRequest<SyncObject>>((id, x) =>
				{
					corrections.AddRange(x.Collection);
					return new ServiceResult<SyncIssue>();
				});

			return client.Object;
		}

		private void DoTestDelay(TimeSpan? testDelay)
		{
			if (testDelay == null)
			{
				return;
			}

			var watch = Stopwatch.StartNew();

			while ((watch.Elapsed <= testDelay.Value) && !IsCancellationPending)
			{
				// Delay while getting options
				Thread.Sleep(10);
			}
		}

		#endregion
	}
}