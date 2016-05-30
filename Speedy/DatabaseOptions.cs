#region References

using System;

#endregion

namespace Speedy
{
	/// <summary>
	/// Represents options for a Speedy database.
	/// </summary>
	public class DatabaseOptions
	{
		#region Constructors

		/// <summary>
		/// Instantiates an instance of the database options class.
		/// </summary>
		public DatabaseOptions()
		{
			DetectSyncableRepositories = true;
			MaintainDates = true;
			MaintainSyncId = true;
			SyncOrder = new string[0];
			SyncTombstoneTimeout = TimeSpan.FromDays(7);
			Timeout = TimeSpan.FromSeconds(30);
			UnmaintainEntities = new Type[0];
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the flag to automatically detect syncable repositories.
		/// </summary>
		public bool DetectSyncableRepositories { get; set; }

		/// <summary>
		/// Gets or sets the flag to manage the CreatedOn and optional ModifiedOn properties.
		/// </summary>
		public bool MaintainDates { get; set; }

		/// <summary>
		/// Gets or sets the flag to manage the sync ID for sync entities.
		/// </summary>
		public bool MaintainSyncId { get; set; }

		/// <summary>
		/// Gets or sets the sync order of the syncable repositories.
		/// </summary>
		public string[] SyncOrder { get; set; }

		/// <summary>
		/// Gets or sets the referenced id to use when tombstoning entities.
		/// </summary>
		public string SyncTombstoneReferenceId { get; set; }

		/// <summary>
		/// Gets or sets the timespan before an entity tombstone will expire.
		/// </summary>
		public TimeSpan SyncTombstoneTimeout { get; set; }

		/// <summary>
		/// Gets or sets the timeout for blocking calls.
		/// </summary>
		public TimeSpan Timeout { get; set; }

		/// <summary>
		/// Gets or sets the list of entities to ignore during maintenance updates.
		/// </summary>
		public Type[] UnmaintainEntities { get; set; }

		#endregion
	}
}