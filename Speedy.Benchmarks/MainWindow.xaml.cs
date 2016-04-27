﻿#region References

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speedy;
using Speedy.Samples;
using Speedy.Samples.Entities;
using Speedy.Samples.Sync;
using Speedy.Sync;

#endregion

namespace Speed.Benchmarks
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		#region Constructors

		public MainWindow()
		{
			InitializeComponent();
			ViewModel = new MainWindowModel();
			DataContext = ViewModel;

			Worker = new BackgroundWorker();
			Worker.WorkerReportsProgress = true;
			Worker.WorkerSupportsCancellation = true;
			Worker.DoWork += WorkerOnDoWork;
			Worker.RunWorkerCompleted += WorkerOnRunWorkerCompleted;
			Worker.ProgressChanged += WorkerOnProgressChanged;
		}

		#endregion

		#region Properties

		public MainWindowModel ViewModel { get; }

		public BackgroundWorker Worker { get; set; }

		#endregion

		#region Methods

		private void Clear()
		{
			if (!Dispatcher.CheckAccess())
			{
				Dispatcher.Invoke(Clear);
				return;
			}

			ViewModel.Output = string.Empty;
		}

		private static void CompareClients(IContosoSyncClient client, IContosoSyncClient server)
		{
			using (var clientDatabase = client.GetDatabase())
			using (var serverDatabase = server.GetDatabase())
			{
				var serverAddresses = serverDatabase.GetReadOnlyRepository<Address>()
					.OrderBy(x => x.Line1).ToList().Select(x => x.DeepClone(true)).ToList();
				var serverPeople = serverDatabase.GetReadOnlyRepository<Person>()
					.OrderBy(x => x.Name).ToList().Select(x => x.DeepClone(true)).ToList();
				var clientAddresses = clientDatabase.GetReadOnlyRepository<Address>()
					.OrderBy(x => x.Line1).ToList().Select(x => x.DeepClone(true)).ToList();
				var clientPeople = clientDatabase.GetReadOnlyRepository<Person>()
					.OrderBy(x => x.Name).ToList().Select(x => x.DeepClone(true)).ToList();

				Extensions.AreEqual(serverAddresses, clientAddresses, false, nameof(Address.Id), nameof(Address.LinkedAddressId));
				Extensions.AreEqual(serverPeople, clientPeople, false, nameof(Person.Id), nameof(Person.AddressId));
			}
		}

		private void MainWindowOnClosing(object sender, CancelEventArgs e)
		{
			Worker.CancelAsync();
		}

		private void SyncOnClick(object sender, RoutedEventArgs e)
		{
			var directory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Speedy");

			switch (ViewModel.SyncStatus)
			{
				case "Sync":
					if (directory.Exists)
					{
						directory.Delete(true);
						directory.Create();
					}
					
					ViewModel.Errors = string.Empty;
					ViewModel.SyncClients.Clear();
					ViewModel.SyncClients.Add(new SyncClientState(new ContosoSyncClient("Server", GetEntityFrameworkProvider())));
					ViewModel.SyncClients.Add(new SyncClientState(new ContosoSyncClient("Client 1", GetEntityFrameworkProvider("server=localhost;database=Speedy2;integrated security=true;"))));
					ViewModel.SyncClients.Add(new SyncClientState(new ContosoSyncClient("Client 2", new ContosoDatabaseProvider())));
					//ViewModel.SyncClients.Add(new SyncClientState(new ContosoSyncClient("Client 3", new ContosoDatabaseProvider(directory.FullName))));
					Worker.RunWorkerAsync(ViewModel.SyncClients);
					break;

				default:
					Worker.CancelAsync();
					ViewModel.SyncClients.Clear();
					break;
			}

			ViewModel.SyncStatus = ViewModel.SyncStatus == "Sync" ? "Stop" : "Sync";
		}

		private static IContosoDatabaseProvider GetEntityFrameworkProvider(string connectionString = null)
		{
			using (var database = new EntityFrameworkContosoDatabase(connectionString ?? "name=DefaultConnection"))
			{
				database.ClearDatabase();
				return new EntityFrameworkContosoDatabaseProvider(database.Database.Connection.ConnectionString);
			}
		}

		private static bool UpdateClient(IContosoSyncClient client, bool forceAdd = false)
		{
			var number = Extensions.Random.Next(1, 101);
			var result = false;

			if (number % 1 == 0 || forceAdd) // 25%
			{
				using (var clientDatabase = client.GetDatabase())
				{
					Address address = null;
					Person person = null;

					if (number > 50 && clientDatabase.Addresses.Any())
					{
						address = clientDatabase.Addresses.GetRandomItem();
						person = new Person
						{
							Name = Extensions.LoremIpsumWord() + " " + Extensions.LoremIpsumWord(),
							AddressId = address.Id
						};

						clientDatabase.People.Add(person);
					}
					else
					{
						address = new Address
						{
							City = Extensions.LoremIpsumWord(),
							Line1 = Extensions.Random.Next(0, 999) + " " + Extensions.LoremIpsumWord(),
							Line2 = string.Empty,
							Postal = Extensions.Random.Next(0, 999999).ToString("000000"),
							State = Extensions.LoremIpsumWord().Substring(0, 2)
						};

						clientDatabase.Addresses.Add(address);
					}

					result = true;
					clientDatabase.SaveChanges();

					if (person != null)
					{
						Debug.WriteLine("+P: " + person.ToJson(true));
					}
					else
					{
						Debug.WriteLine("+A: " + address.ToJson(true));
					}
				}
			}

			if (number % 10 == 0) // 10%
			{
				using (var clientDatabase = client.GetDatabase())
				{
					// Delete Person or Address?
					if (number % 4 == 0)
					{
						var person = clientDatabase.People.GetRandomItem();
						if (person != null)
						{
							Debug.WriteLine("-P: " + person.ToJson(true));
							clientDatabase.People.Remove(person);
						}
					}
					else
					{
						var address = clientDatabase.Addresses.Where(x => !x.People.Any()).GetRandomItem();
						if (address != null)
						{
							Debug.WriteLine("-A: " + address.ToJson(true));
							clientDatabase.Addresses.Remove(address);
						}
					}

					clientDatabase.SaveChanges();
				}
			}

			return result;
		}

		private void WorkerOnDoWork(object sender, DoWorkEventArgs args)
		{
			var worker = (BackgroundWorker) sender;
			var timeout = DateTime.UtcNow;
			var collection = (ObservableCollection<SyncClientState>) args.Argument;
			var options = new SyncOptions();
			WriteError("Worker Started");

			while (!worker.CancellationPending)
			{
				if (timeout > DateTime.UtcNow)
				{
					Thread.Sleep(100);
					continue;
				}

				var server = collection[0];
				var client = collection.GetRandomItem(server);

				Clear();

				try
				{
					WriteLine("Updating " + client.Client.Name);
					var forceAdd = UpdateClient(client.Client);

					WriteLine("Updating " + server.Client.Name);
					UpdateClient(server.Client, forceAdd);
				}
				catch (Exception ex)
				{
					// Write message but ignore them for now...
					WriteError(ex.Message);
				}

				options.LastSyncedOn = client.LastSyncedOn;

				var engine = new SyncEngine(client.Client, server.Client, options);

				UpdateDisplay(client, engine);
				Debug.WriteLine("A: " + client.AddressCount + " P: " + client.PeopleCount + " " + client.Client.Name);

				UpdateDisplay(server, engine);
				Debug.WriteLine("A: " + server.AddressCount + " P: " + server.PeopleCount + " " + server.Client.Name);

				engine.SyncStatusChanged += (o, a) => worker.ReportProgress((int) a.Percent, a);
				engine.Run();
				
				foreach (var item in engine.SyncIssues)
				{
					WriteError(item.Id + " - " + item.IssueType + " : " + item.TypeName);
				}

				try
				{
					UpdateDisplay(client, engine);
					Debug.WriteLine("A: " + client.AddressCount + " P: " + client.PeopleCount + " " + client.Client.Name);

					UpdateDisplay(server, engine);
					Debug.WriteLine("A: " + server.AddressCount + " P: " + server.PeopleCount + " " + server.Client.Name);

					CheckClient(client.Client);
					CheckClient(server.Client);
					CompareClients(client.Client, server.Client);
				}
				catch (Exception ex)
				{
					WriteError("StartTime: " + engine.StartTime.TimeOfDay);
					WriteError("LastSyncedOn: " + options.LastSyncedOn.TimeOfDay);
					WriteError(ex?.Message ?? "null?");
					//worker.CancelAsync();
					client.LastSyncedOn = DateTime.MinValue;
					using (var database = client.Client.GetDatabase())
					{
						database.SyncTombstones.Remove(x => x.Id > 0);
						database.SaveChanges();
					}
					using (var database = server.Client.GetDatabase())
					{
						database.SyncTombstones.Remove(x => x.Id > 0);
						database.SaveChanges();
					}
				}

				timeout = DateTime.UtcNow.AddMilliseconds(250);
			}

			WriteError("Worker Ending");
		}

		private void CheckClient(IContosoSyncClient client)
		{
			using (var database = client.GetDatabase())
			{
				Assert.IsFalse(database.Addresses.Join(database.SyncTombstones, x => x.SyncId, x => x.SyncId, (a, t) => new { a, t }).Any());
				Assert.IsFalse(database.People.Join(database.SyncTombstones, x => x.SyncId, x => x.SyncId, (p, t) => new { p, t }).Any());
			}
		}

		private void UpdateDisplay(SyncClientState state, SyncEngine engine)
		{
			if (!Dispatcher.CheckAccess())
			{
				Dispatcher.Invoke(() => UpdateDisplay(state, engine));
				return;
			}

			using (var database = state.Client.GetDatabase())
			{
				state.AddressCount = database.Addresses.Count();
				state.PeopleCount = database.People.Count();
				state.PreviousSyncedOn = state.LastSyncedOn;
				if (engine.StartTime != DateTime.MinValue)
				{
					state.LastSyncedOn = engine.StartTime;
				}
				ViewModel.Progress = 0;
			}
		}

		private void WorkerOnProgressChanged(object sender, ProgressChangedEventArgs args)
		{
			var status = (SyncEngineStatusArgs) args.UserState;
			var clientState = ViewModel.SyncClients.FirstOrDefault(x => x.Client.Name == status.Name);
			if (clientState == null)
			{
				return;
			}

			clientState.Status = status.Status;
			ViewModel.Progress = (int) status.Percent;
		}

		private void WorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs args)
		{
			ViewModel.SyncStatus = "Sync";
		}

		private void WriteError(string message)
		{
			if (!Dispatcher.CheckAccess())
			{
				Dispatcher.Invoke(() => WriteError(message));
				return;
			}

			ViewModel.Errors += message + Environment.NewLine;
		}

		private void WriteLine(string message)
		{
			if (!Dispatcher.CheckAccess())
			{
				Dispatcher.Invoke(() => WriteLine(message));
				return;
			}

			ViewModel.Output += message + Environment.NewLine;
		}

		#endregion
	}
}