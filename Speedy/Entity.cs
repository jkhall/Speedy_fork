﻿namespace Speedy
{
	/// <summary>
	/// Represents a Speedy entity.
	/// </summary>
	/// <typeparam name="T"> The type of the entity key. </typeparam>
	public abstract class Entity<T> : IEntity
	{
		#region Properties

		/// <summary>
		/// Gets or sets the ID of the entity.
		/// </summary>
		public abstract T Id { get; set; }

		#endregion

		#region Methods

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

		/// <inheritdoc />
		public virtual bool IdIsSet()
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
			return default;
		}

		/// <inheritdoc />
		public virtual bool TrySetId(string id)
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
		/// <returns>
		/// The real entity unwrapped from the Entity Framework proxy.
		/// </returns>
		public T1 Unwrap<T1>()
		{
			return (T1) Unwrap();
		}
		
		/// <summary>
		/// Unwrap the entity from the proxy.
		/// </summary>
		/// <returns>
		/// The real entity unwrapped from the Entity Framework proxy.
		/// </returns>
		public object Unwrap()
		{
			var json = this.ToJson(ignoreVirtuals: true);
			var type = this.GetRealType();
			return json.FromJson(type);
		}

		#endregion
	}

	/// <summary>
	/// Represents a Speedy entity.
	/// </summary>
	public interface IEntity
	{
		#region Methods

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
		/// Determine if the ID is set on the entity.
		/// </summary>
		/// <returns> True if the ID is set or false if otherwise. </returns>
		bool IdIsSet();

		/// <summary>
		/// Try to set the ID from a serialized version.
		/// </summary>
		/// <returns> True if the ID is successfully set or false if otherwise. </returns>
		bool TrySetId(string id);

		#endregion
	}
}