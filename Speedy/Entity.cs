﻿#region References

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Speedy.Sync;

#endregion

namespace Speedy
{
	/// <summary>
	/// Represents a Speedy entity.
	/// </summary>
	/// <typeparam name="T"> The type of the entity key. </typeparam>
	public abstract class Entity<T> : Entity
	{
		#region Properties

		/// <summary>
		/// Gets or sets the ID of the entity.
		/// </summary>
		public abstract T Id { get; set; }

		#endregion

		#region Methods

		/// <inheritdoc />
		public override bool IdIsSet()
		{
			return !Equals(Id, default(T));
		}

		/// <summary>
		/// Allows the entity to calculate the next key.
		/// </summary>
		/// <param name="currentKey"> The current version of the key. </param>
		/// <returns> The new key to be used in. </returns>
		public virtual T NewId(ref T currentKey)
		{
			switch (currentKey)
			{
				case sbyte sbKey:
					currentKey = (T) (object) (sbKey + 1);
					break;

				case byte bKey:
					currentKey = (T) (object) (bKey + 1);
					break;

				case short sKey:
					currentKey = (T) (object) (sKey + 1);
					break;

				case ushort usKey:
					currentKey = (T) (object) (usKey + 1);
					break;

				case int iKey:
					currentKey = (T) (object) (iKey + 1);
					break;

				case uint uiKey:
					currentKey = (T) (object) (uiKey + 1);
					break;

				case long lKey:
					currentKey = (T) (object) (lKey + 1);
					break;

				case ulong ulKey:
					currentKey = (T) (object) (ulKey + 1);
					break;
			}

			return currentKey;
		}

		/// <inheritdoc />
		public override bool TrySetId(string id)
		{
			try
			{
				Id = id.FromJson<T>();
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Unwrap the entity from the proxy as a specific type.
		/// </summary>
		/// <param name="update"> An optional update method. </param>
		/// <returns> The real entity unwrapped from the Entity Framework proxy. </returns>
		public T1 Unwrap<T1>(Action<T1> update = null)
		{
			return this.Unwrap<Entity<T>, T1>(update);
		}

		/// <summary>
		/// Gets the default exclusions for update. Warning: this is called during constructor, overrides need to be
		/// sure to only return static values as to not cause issues.
		/// </summary>
		/// <returns> The values to exclude during update. </returns>
		protected override HashSet<string> GetDefaultExclusionsForSyncUpdate()
		{
			return new HashSet<string> { nameof(Id) };
		}

		#endregion
	}

	/// <summary>
	/// Represents a Speedy entity.
	/// </summary>
	public abstract class Entity : IEntity
	{
		#region Fields

		/// <summary>
		/// Represents the base type for a sync entity interface, just a quick lookup value.
		/// </summary>
		protected static readonly Type SyncEntityInterfaceType;

		/// <summary>
		/// Properties to ignore when tracking changes.
		/// </summary>
		private readonly HashSet<string> _excludedPropertiesForChangeTracking;

		/// <summary>
		/// Properties to ignore when updating.
		/// </summary>
		private readonly HashSet<string> _excludedPropertiesForSyncUpdate;

		/// <summary>
		/// All hash sets for types, this is for optimization
		/// </summary>
		private static readonly ConcurrentDictionary<Type, HashSet<string>> _exclusionCacheForChangeTracking;

		/// <summary>
		/// All hash sets for types, this is for optimization
		/// </summary>
		private static readonly ConcurrentDictionary<Type, HashSet<string>> _exclusionCacheForIncomingSync;

		/// <summary>
		/// All hash sets for types, this is for optimization
		/// </summary>
		private static readonly ConcurrentDictionary<Type, HashSet<string>> _exclusionCacheForOutgoingSync;

		/// <summary>
		/// All hash sets for types, this is for optimization
		/// </summary>
		private static readonly ConcurrentDictionary<Type, HashSet<string>> _exclusionCacheForSyncUpdate;

		/// <summary>
		/// Represents if the entity has had changes or not.
		/// </summary>
		private bool _hasChanges;

		/// <summary>
		/// Cached version of the "real" type, meaning not EF proxy but rather root type
		/// </summary>
		private Type _realType;

		#endregion

		#region Constructors

		/// <summary>
		/// Instantiates an entity
		/// </summary>
		protected Entity()
		{
			var type = GetType();
			_excludedPropertiesForChangeTracking = _exclusionCacheForChangeTracking.GetOrAdd(type, x => new HashSet<string>(GetDefaultExclusionsForChangeTracking()));
			_excludedPropertiesForSyncUpdate = _exclusionCacheForSyncUpdate.GetOrAdd(type, x => new HashSet<string>(GetDefaultExclusionsForSyncUpdate()));
		}

		/// <summary>
		/// Instantiates an entity
		/// </summary>
		static Entity()
		{
			_exclusionCacheForChangeTracking = new ConcurrentDictionary<Type, HashSet<string>>();
			_exclusionCacheForIncomingSync = new ConcurrentDictionary<Type, HashSet<string>>();
			_exclusionCacheForOutgoingSync = new ConcurrentDictionary<Type, HashSet<string>>();
			_exclusionCacheForSyncUpdate = new ConcurrentDictionary<Type, HashSet<string>>();

			SyncEntityInterfaceType = typeof(ISyncEntity);
		}

		#endregion

		#region Methods

		/// <inheritdoc />
		public virtual bool CanBeModified()
		{
			return true;
		}

		/// <inheritdoc />
		public virtual void EntityAdded()
		{
		}

		/// <inheritdoc />
		public virtual void EntityDeleted()
		{
		}

		/// <inheritdoc />
		public virtual void EntityModified()
		{
		}

		/// <summary>
		/// Exclude properties from changing tracking.
		/// </summary>
		/// <param name="propertyNames"> The member names to exclude </param>
		public void ExcludePropertiesForChangeTracking(params string[] propertyNames)
		{
			foreach (var propertyName in propertyNames)
			{
				if (_excludedPropertiesForChangeTracking.Contains(propertyName))
				{
					continue;
				}

				_excludedPropertiesForChangeTracking.Add(propertyName);
			}
		}

		/// <inheritdoc />
		public void ExcludePropertiesForSyncUpdate(params string[] propertyNames)
		{
			foreach (var propertyName in propertyNames)
			{
				if (_excludedPropertiesForSyncUpdate.Contains(propertyName))
				{
					continue;
				}

				_excludedPropertiesForSyncUpdate.Add(propertyName);
			}
		}

		/// <inheritdoc />
		public HashSet<string> GetExcludedPropertiesForChangeTracking()
		{
			return new HashSet<string>(_excludedPropertiesForChangeTracking);
		}

		/// <inheritdoc />
		public HashSet<string> GetExcludedPropertiesForSyncUpdate()
		{
			return new HashSet<string>(_excludedPropertiesForSyncUpdate);
		}

		/// <summary>
		/// Determines if the object has changes.
		/// </summary>
		public virtual bool HasChanges()
		{
			return _hasChanges;
		}

		/// <inheritdoc />
		public abstract bool IdIsSet();

		/// <inheritdoc />
		public bool IsPropertyExcludedForChangeTracking(string propertyName)
		{
			return _excludedPropertiesForChangeTracking.Contains(propertyName);
		}

		/// <inheritdoc />
		public bool IsPropertyExcludedForSyncUpdate(string propertyName)
		{
			return _excludedPropertiesForSyncUpdate.Contains(propertyName);
		}

		/// <summary>
		/// Notify that a property has changed
		/// </summary>
		/// <param name="propertyName"> The name of the property that changed. </param>
		public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (!_excludedPropertiesForChangeTracking.Contains(propertyName))
			{
				_hasChanges = true;
			}

			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Reset the change tracking flag.
		/// </summary>
		public void ResetChangeTracking()
		{
			_hasChanges = false;
		}

		/// <inheritdoc />
		public void ResetExcludedPropertiesForSyncUpdate(bool setToDefault = true)
		{
			_excludedPropertiesForSyncUpdate.Clear();

			if (setToDefault)
			{
				_excludedPropertiesForSyncUpdate.AddRange(GetDefaultExclusionsForSyncUpdate());
			}
		}

		/// <inheritdoc />
		public void ResetPropertyChangeTrackingExclusions(bool setToDefault = true)
		{
			_excludedPropertiesForChangeTracking.Clear();

			if (setToDefault)
			{
				_excludedPropertiesForChangeTracking.AddRange(GetDefaultExclusionsForChangeTracking());
			}
		}

		/// <inheritdoc />
		public abstract bool TrySetId(string id);

		/// <summary>
		/// Unwrap the entity from the proxy.
		/// </summary>
		/// <returns>
		/// The real entity unwrapped from the Entity Framework proxy.
		/// </returns>
		public virtual object Unwrap()
		{
			var type = _realType ??= this.GetRealType();
			return this.Unwrap(type);
		}

		/// <summary>
		/// Used by the ISyncEntity class to set the exclusion. Do not use this method directly,
		/// you should be implementing the SyncEntity.GetDefaultExclusionsForIncomingSync method instead.
		/// </summary>
		/// <param name="type"> The type of the sync entity. </param>
		/// <param name="values"> The values to be excluded. </param>
		/// <returns> The excluded values. </returns>
		protected HashSet<string> AddExclusionsForIncomingSync(Type type, Func<Type, HashSet<string>> values)
		{
			return _exclusionCacheForIncomingSync.GetOrAdd(type, values);
		}

		/// <summary>
		/// Used by the ISyncEntity class to set the exclusion. Do not use this method directly,
		/// you should be implementing the SyncEntity.GetDefaultExclusionsForOutgoingSync method instead.
		/// </summary>
		/// <param name="type"> The type of the sync entity. </param>
		/// <param name="values"> The values to be excluded. </param>
		/// <returns> The excluded values. </returns>
		protected HashSet<string> AddExclusionsForOutgoingSync(Type type, Func<Type, HashSet<string>> values)
		{
			return _exclusionCacheForOutgoingSync.GetOrAdd(type, values);
		}

		/// <summary>
		/// Gets the default exclusions for change tracking. Warning: this is called during constructor, overrides need to be
		/// sure to only return static values as to not cause issues.
		/// </summary>
		/// <returns> The values to exclude during change tracking. </returns>
		protected virtual HashSet<string> GetDefaultExclusionsForChangeTracking()
		{
			return new HashSet<string>();
		}

		/// <summary>
		/// Gets the default exclusions for update. Warning: this is called during constructor, overrides need to be
		/// sure to only return static values as to not cause issues.
		/// </summary>
		/// <returns> The values to exclude during update. </returns>
		protected abstract HashSet<string> GetDefaultExclusionsForSyncUpdate();

		#endregion

		#region Events

		/// <inheritdoc />
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}

	/// <summary>
	/// Represents a Speedy entity.
	/// </summary>
	public interface IEntity : INotifyPropertyChanged
	{
		#region Methods

		/// <summary>
		/// Checks to see if an entity can be modified.
		/// </summary>
		bool CanBeModified();

		/// <summary>
		/// Update an entity that has been added.
		/// </summary>
		void EntityAdded();

		/// <summary>
		/// Update an entity that has been deleted.
		/// </summary>
		void EntityDeleted();

		/// <summary>
		/// Update an entity that has been modified.
		/// </summary>
		void EntityModified();

		/// <summary>
		/// Add a property to exclude from change tracking.
		/// </summary>
		/// <param name="propertyNames"> The names of the property to exclude. </param>
		void ExcludePropertiesForChangeTracking(params string[] propertyNames);

		/// <summary>
		/// Add a property to exclude during update.
		/// </summary>
		/// <param name="propertyNames"> The names of the property to exclude. </param>
		void ExcludePropertiesForSyncUpdate(params string[] propertyNames);

		/// <summary>
		/// Get the properties excluded from change tracking.
		/// </summary>
		/// <returns> The names of the property to exclude. </returns>
		HashSet<string> GetExcludedPropertiesForChangeTracking();

		/// <summary>
		/// Get the properties excluded during update.
		/// </summary>
		/// <returns> The names of the property to exclude. </returns>
		HashSet<string> GetExcludedPropertiesForSyncUpdate();

		/// <summary>
		/// Determine if the ID is set on the entity.
		/// </summary>
		/// <returns> True if the ID is set or false if otherwise. </returns>
		bool IdIsSet();

		/// <summary>
		/// Checks a property has been excluded for change tracking.
		/// </summary>
		/// <param name="propertyName"> The property name to be tested. </param>
		/// <returns> True if the property is excluded or false if otherwise. </returns>
		bool IsPropertyExcludedForChangeTracking(string propertyName);

		/// <summary>
		/// Checks a property has been excluded from updating.
		/// </summary>
		/// <param name="propertyName"> The property name to be tested. </param>
		/// <returns> True if the property can be written during an update or false if otherwise. </returns>
		bool IsPropertyExcludedForSyncUpdate(string propertyName);

		/// <summary>
		/// Resets exclusion back to default values or just clears if setToDefault is false.
		/// </summary>
		/// <param name="setToDefault"> Set to default excluded values. Defaults to true. </param>
		void ResetExcludedPropertiesForSyncUpdate(bool setToDefault = true);

		/// <summary>
		/// Resets exclusion back to default values or just clears if setToDefault is false.
		/// </summary>
		/// <param name="setToDefault"> Set to default excluded values. Defaults to true. </param>
		void ResetPropertyChangeTrackingExclusions(bool setToDefault = true);

		/// <summary>
		/// Try to set the ID from a serialized version.
		/// </summary>
		/// <returns> True if the ID is successfully set or false if otherwise. </returns>
		bool TrySetId(string id);

		#endregion
	}

	/// <summary>
	/// Represents a Speedy entity that track the date and time it was created.
	/// </summary>
	public interface ICreatedEntity : IEntity
	{
		#region Properties

		/// <summary>
		/// Gets or sets the date and time the entity was created.
		/// </summary>
		DateTime CreatedOn { get; set; }

		#endregion
	}

	/// <summary>
	/// Represents a Speedy entity that track the date and time it was last modified.
	/// </summary>
	public interface IModifiableEntity : ICreatedEntity
	{
		#region Properties

		/// <summary>
		/// Gets or sets the date and time the entity was modified.
		/// </summary>
		DateTime ModifiedOn { get; set; }

		#endregion
	}
}