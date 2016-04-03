﻿#region References

using System;
using System.Linq.Expressions;
using Speedy.Exceptions;

#endregion

namespace Speedy.Configuration
{
	/// <summary>
	/// The interface for the property configuration.
	/// </summary>
	public interface IPropertyConfiguration
	{
		#region Methods

		/// <summary>
		/// Checks to see if this configuration is for the provided entity.
		/// </summary>
		/// <param name="entity"> The entity to test against. </param>
		/// <returns> True if this configuration is for the entity and false if otherwise. </returns>
		bool IsMappingFor(object entity);

		/// <summary>
		/// Validates the entity using this configuration.
		/// </summary>
		/// <param name="entity"> The entity to validate. </param>
		void Validate(object entity);

		#endregion
	}

	/// <summary>
	/// The configuration for an entity property.
	/// </summary>
	/// <typeparam name="T"> The entity this configuration is for. </typeparam>
	public class PropertyConfiguration<T> : IPropertyConfiguration where T : Entity
	{
		#region Fields

		private readonly Type _entityType;
		private bool? _isNullable;
		private int _maxLength;
		private int _minLength;
		private readonly Expression<Func<T, object>> _property;
		private readonly Func<T, object> _propertyFunction;

		#endregion

		#region Constructors

		/// <summary>
		/// Instantiates an instance of the property configuration.
		/// </summary>
		/// <param name="property"> The property expression this configuration is for. </param>
		public PropertyConfiguration(Expression<Func<T, object>> property)
		{
			_entityType = typeof (T);
			_property = property;
			_propertyFunction = _property.Compile();
			_isNullable = null;
			_maxLength = -1;
			_minLength = -1;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Sets the maximum length of the member.
		/// </summary>
		/// <returns> The configuration after updated. </returns>
		public PropertyConfiguration<T> HasMaximumLength(int length)
		{
			_maxLength = length;
			return this;
		}

		/// <summary>
		/// Sets the minimum length of the member.
		/// </summary>
		/// <returns> The configuration after updated. </returns>
		public PropertyConfiguration<T> HasMinimumLength(int length)
		{
			_minLength = length;
			return this;
		}

		/// <summary>
		/// Checks to see if this configuration is for the provided entity.
		/// </summary>
		/// <param name="entity"> The entity to test against. </param>
		/// <returns> True if this configuration is for the entity and false if otherwise. </returns>
		public bool IsMappingFor(object entity)
		{
			return entity is T;
		}

		/// <summary>
		/// Marks the property as an optional member.
		/// </summary>
		/// <returns> The configuration after updated. </returns>
		public PropertyConfiguration<T> IsOptional()
		{
			_isNullable = true;
			return this;
		}

		/// <summary>
		/// Marks the property as a required member.
		/// </summary>
		/// <returns> The configuration after updated. </returns>
		public PropertyConfiguration<T> IsRequired()
		{
			_isNullable = false;
			return this;
		}

		/// <summary>
		/// Validates the entity using this configuration.
		/// </summary>
		/// <param name="entity"> The entity to validate. </param>
		public void Validate(T entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			var property = _propertyFunction.Invoke(entity);
			var dValue = _property as dynamic;
			var memberName = dValue.Body.Member.Name;

			if (_isNullable.HasValue && _isNullable.Value == false && property == null)
			{
				throw new ValidationException($"{_entityType.Name}: The {memberName} field is required.");
			}

			var stringEntity = property as string;
			if (stringEntity != null && _maxLength > 0 && stringEntity.Length > _maxLength)
			{
				throw new ValidationException($"{_entityType.Name}: The {memberName} field is too long.");
			}

			if (stringEntity != null && _minLength > 0 && stringEntity.Length < _minLength)
			{
				throw new ValidationException($"{_entityType.Name}: The {memberName} field is too short.");
			}
		}

		/// <summary>
		/// Validates the entity using this configuration.
		/// </summary>
		/// <param name="entity"> The entity to validate. </param>
		public void Validate(object entity)
		{
			Validate(entity as T);
		}

		#endregion
	}
}