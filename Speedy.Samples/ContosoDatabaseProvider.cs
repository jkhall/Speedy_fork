﻿#region References

using System;

#endregion

namespace Speedy.Samples
{
	public class ContosoDatabaseProvider : IContosoDatabaseProvider
	{
		#region Fields

		private readonly string _directory;
		private readonly Lazy<ContosoDatabase> _memoryDatabase;

		#endregion

		#region Constructors

		public ContosoDatabaseProvider(string directory = null, DatabaseOptions options = null)
		{
			_directory = directory;
			Options = options ?? new DatabaseOptions();

			_memoryDatabase = new Lazy<ContosoDatabase>(() => new ContosoDatabase(null, options), true);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the options for this database.
		/// </summary>
		public DatabaseOptions Options { get; }

		#endregion

		#region Methods

		public IContosoDatabase GetDatabase()
		{
			return string.IsNullOrEmpty(_directory)
				? _memoryDatabase.Value
				: new ContosoDatabase(_directory, Options);
		}

		/// <summary>
		/// Gets an instance of the syncable database.
		/// </summary>
		/// <returns> The syncable database instance. </returns>
		ISyncableDatabase ISyncableDatabaseProvider.GetDatabase()
		{
			return GetDatabase();
		}

		/// <summary>
		/// Gets an instance of the database.
		/// </summary>
		/// <returns> The database instance. </returns>
		IDatabase IDatabaseProvider.GetDatabase()
		{
			return GetDatabase();
		}

		#endregion
	}
}