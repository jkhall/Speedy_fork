#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Speedy.Sync;

#endregion

namespace Speedy
{
	/// <summary>
	/// The interfaces for a Speedy syncable database.
	/// </summary>
	public interface ISyncableDatabase : IDatabase
	{
		#region Properties

		/// <summary>
		/// Gets the sync tombstone repository.
		/// </summary>
		IRepository<SyncTombstone> SyncTombstones { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Gets a list of syncable repositories.
		/// </summary>
		/// <returns> The list of syncable repositories. </returns>
		IEnumerable<ISyncableRepository> GetSyncableRepositories();

		/// <summary>
		/// Gets a syncable repository of the requested entity.
		/// </summary>
		/// <typeparam name="T"> The type of the entity to get a repository for. </typeparam>
		/// <returns> The repository of entities requested. </returns>
		ISyncableRepository<T> GetSyncableRepository<T>() where T : SyncEntity, new();

		/// <summary>
		/// Gets a syncable repository of the requested entity.
		/// </summary>
		/// <returns> The repository of entities requested. </returns>
		ISyncableRepository GetSyncableRepository(Type type);

		/// <summary>
		/// Gets a list of sync tombstones that matches the filter.
		/// </summary>
		/// <param name="filter"> The filter to use. </param>
		/// <returns> The list of sync tombstones. </returns>
		IQueryable<SyncTombstone> GetSyncTombstones(Expression<Func<SyncTombstone, bool>> filter);

		/// <summary>
		/// Removes sync tombstones that represent match the filter.
		/// </summary>
		/// <param name="filter"> The filter to use. </param>
		void RemoveSyncTombstones(Expression<Func<SyncTombstone, bool>> filter);

		#endregion
	}
}