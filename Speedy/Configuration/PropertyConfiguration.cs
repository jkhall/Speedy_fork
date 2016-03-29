﻿#region References

using System;
using System.Linq.Expressions;
using Speedy.Exceptions;

#endregion

namespace Speedy.Configuration
{
	public interface IPropertyConfiguration
	{
		#region Methods

		bool IsMappingFor(object entity);

		void Validate(object entity);

		#endregion
	}

	public class PropertyConfiguration<T> : IPropertyConfiguration where T : Entity
	{
		#region Fields

		private bool? _isNullable;
		private int _maxLength;
		private int _minLength;
		private readonly Expression<Func<T, object>> _property;

		#endregion

		#region Constructors

		public PropertyConfiguration(Expression<Func<T, object>> property)
		{
			_property = property;
			_isNullable = null;
			_maxLength = -1;
			_minLength = -1;
		}

		#endregion

		#region Methods

		public bool IsMappingFor(object entity)
		{
			return entity is T;
		}

		public PropertyConfiguration<T> HasMaxLength(int length)
		{
			_maxLength = length;
			return this;
		}

		public PropertyConfiguration<T> HasMinLength(int length)
		{
			_minLength = length;
			return this;
		}

		public PropertyConfiguration<T> IsOptional()
		{
			_isNullable = true;
			return this;
		}

		public PropertyConfiguration<T> IsRequired()
		{
			_isNullable = false;
			return this;
		}

		public void Validate(T entity)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity));
			}

			var entityType = typeof (T);
			var function = _property.Compile();
			var property = function.Invoke(entity);
			var dValue = _property as dynamic;
			var memberName = dValue.Body.Member.Name;

			if (_isNullable.HasValue && _isNullable.Value == false && property == null)
			{
				throw new ValidationException($"{entityType.Name}: The {memberName} field is required.");
			}

			var stringEntity = property as string;
			if (stringEntity != null && _maxLength > 0 && stringEntity.Length > _maxLength)
			{
				throw new ValidationException($"{entityType.Name}: The {memberName} field is too long.");
			}

			if (stringEntity != null && _minLength > 0 && stringEntity.Length < _minLength)
			{
				throw new ValidationException($"{entityType.Name}: The {memberName} field is too short.");
			}
		}

		public void Validate(object entity)
		{
			Validate(entity as T);
		}

		#endregion
	}
}