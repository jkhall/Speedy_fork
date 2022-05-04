﻿#region References

using System;
using System.Collections.Generic;
using System.Reflection;
using Speedy.Protocols.Osc;

#endregion

namespace Speedy.Validation
{
	/// <summary>
	/// Validation for an object member.
	/// </summary>
	public class MemberValidator<T> : MemberValidator
	{
		#region Constructors

		/// <summary>
		/// Creates an instance of a member validator.
		/// </summary>
		internal MemberValidator(MemberInfo info) : base(info)
		{
		}

		#endregion

		#region Methods

		/// <summary>
		/// Validate an object within a range.
		/// </summary>
		/// <param name="minimum"> The minimum value. </param>
		/// <param name="maximum"> The maximum value. </param>
		/// <param name="excludeRangeValues"> The option to exclude the minimum and maximum values </param>
		/// <returns> True if the value is and within the provided range. </returns>
		public MemberValidator<T> HasMinMaxRange(T minimum, T maximum, bool excludeRangeValues = false)
		{
			return AddMinMaxRange(minimum, maximum, $"{Info.Name} is not within the provided range values.", excludeRangeValues);
		}

		/// <summary>
		/// Validate an object within a range.
		/// </summary>
		/// <param name="minimum"> The minimum value. </param>
		/// <param name="maximum"> The maximum value. </param>
		/// <param name="message"> The message for failed validation. </param>
		/// <param name="excludeRangeValues"> The option to exclude the minimum and maximum values </param>
		/// <returns> True if the value is and within the provided range. </returns>
		public MemberValidator<T> HasMinMaxRange(T minimum, T maximum, string message, bool excludeRangeValues = false)
		{
			return AddMinMaxRange(minimum, maximum, message, excludeRangeValues);
		}

		/// <summary>
		/// Validate an object within a range.
		/// </summary>
		/// <param name="minimum"> The minimum value. </param>
		/// <param name="maximum"> The maximum value. </param>
		/// <param name="excludeRangeValues"> The option to exclude the minimum and maximum values </param>
		/// <returns> True if the value is and within the provided range. </returns>
		public MemberValidator<T> HasMinMaxRange(int minimum, int maximum, bool excludeRangeValues = false)
		{
			return AddMinMaxRange(minimum, maximum, $"{Info.Name} is not within the provided range values.", excludeRangeValues);
		}

		/// <summary>
		/// Validate an object within a range.
		/// </summary>
		/// <param name="minimum"> The minimum value. </param>
		/// <param name="maximum"> The maximum value. </param>
		/// <param name="message"> The message for failed validation. </param>
		/// <param name="excludeRangeValues"> The option to exclude the minimum and maximum values </param>
		/// <returns> True if the value is and within the provided range. </returns>
		public MemberValidator<T> HasMinMaxRange(int minimum, int maximum, string message, bool excludeRangeValues = false)
		{
			return AddMinMaxRange(minimum, maximum, message, excludeRangeValues);
		}

		/// <summary>
		/// Validates that a member has a valid value.
		/// </summary>
		/// <returns> True if the value is and within the provided range. </returns>
		public MemberValidator<T> HasValidValue()
		{
			return HasValidValue($"{Info.Name} does not contain a valid value.");
		}

		/// <summary>
		/// Validates that a member has a valid value.
		/// </summary>
		/// <param name="message"> The message for failed validation. </param>
		/// <returns> True if the value is and within the provided range. </returns>
		public MemberValidator<T> HasValidValue(string message)
		{
			var underlyingType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
			var validation = new Validation<T>(this, message, x => Enum.IsDefined(underlyingType, x));
			Validations.Add(validation);
			return this;
		}

		/// <summary>
		/// Validate a member to ensure it is not null.
		/// </summary>
		public MemberValidator<T> IsNotNull()
		{
			return IsNotNull($"{Info.Name} is null.");
		}

		/// <summary>
		/// Validate a member to ensure it is not null.
		/// </summary>
		/// <param name="message"> The message for failed validation. </param>
		public MemberValidator<T> IsNotNull(string message)
		{
			var validation = new Validation<T>(this, message, x => x != null);
			Validations.Add(validation);
			return this;
		}

		/// <summary>
		/// Configure this member as optional.
		/// </summary>
		public MemberValidator<T> IsOptional()
		{
			return IsOptional($"{Info.Name} is required but was not provided.");
		}

		/// <summary>
		/// Configure this member as optional.
		/// </summary>
		/// <param name="message"> The message for failed validation. </param>
		public MemberValidator<T> IsOptional(string message)
		{
			return IsRequired(false, message);
		}

		/// <summary>
		/// Configure this member as required.
		/// </summary>
		public MemberValidator<T> IsRequired()
		{
			return IsRequired($"{Info.Name} is required but was not provided.");
		}

		/// <summary>
		/// Configure this member as required.
		/// </summary>
		/// <param name="message"> The message for failed validation. </param>
		public MemberValidator<T> IsRequired(string message)
		{
			return IsRequired(true, message);
		}

		/// <summary>
		/// Validate an object is equal to or greater than provided minimum value.
		/// </summary>
		/// <param name="minimum"> The inclusive minimum value. </param>
		/// <returns> True if the value is equal to or greater than the provided value. </returns>
		public MemberValidator<T> NoLessThan(T minimum)
		{
			return NoLessThan(minimum, $"{Info.Name} is less than the provided minimum value.");
		}

		/// <summary>
		/// Validate an object is equal to or greater than provided minimum value.
		/// </summary>
		/// <param name="minimum"> The inclusive minimum value. </param>
		/// <param name="message"> The message for failed validation. </param>
		/// <returns> True if the value is equal to or greater than the provided value. </returns>
		public MemberValidator<T> NoLessThan(T minimum, string message)
		{
			return AddNoLessThan(minimum, message);
		}

		/// <summary>
		/// Validate an object is equal to or less than provided minimum value.
		/// </summary>
		/// <param name="maximum"> The inclusive maximum value. </param>
		/// <returns> True if the value is equal to or less than the provided value. </returns>
		public MemberValidator<T> NoMoreThan(T maximum)
		{
			return NoMoreThan(maximum, $"{Info.Name} is greater than the provided maximum value.");
		}

		/// <summary>
		/// Validate an object is equal to or less than provided minimum value.
		/// </summary>
		/// <param name="maximum"> The inclusive maximum value. </param>
		/// <param name="message"> The message for failed validation. </param>
		/// <returns> True if the value is equal to or less than the provided value. </returns>
		public MemberValidator<T> NoMoreThan(T maximum, string message)
		{
			return AddNoMoreThan(maximum, message);
		}

		/// <inheritdoc />
		public override bool TryValidate(object value)
		{
			for (var i = 0; i < Validations.Count; i++)
			{
				if (i >= Validations.Count)
				{
					return true;
				}

				var validation = Validations[i];

				if (!validation.TryValidate(value))
				{
					return false;
				}
			}

			return true;
		}

		private MemberValidator<T1> AddMinMaxRange<T1>(object minimum, object maximum, Func<T1, T1, Func<T1, bool>> getValidate, string message)
		{
			if (minimum is not T1 tMinimum)
			{
				throw new InvalidCastException("Minimum value is the incorrect type.");
			}

			if (maximum is not T1 tMaximum)
			{
				throw new InvalidCastException("Maximum value is the incorrect type.");
			}

			return AddValidation(getValidate(tMinimum, tMaximum), message);
		}

		private MemberValidator<T1> AddMinMaxRange<T1, T2>(object minimum, object maximum, Func<T2, T2, Func<T1, bool>> getValidate, string message)
		{
			if (minimum is not T2 tMinimum)
			{
				throw new InvalidCastException("Minimum value is the incorrect type.");
			}

			if (maximum is not T2 tMaximum)
			{
				throw new InvalidCastException("Maximum value is the incorrect type.");
			}

			return AddValidation(getValidate(tMinimum, tMaximum), message);
		}

		/// <summary>
		/// Validate an object within a range.
		/// </summary>
		/// <param name="minimum"> The inclusive minimum value. </param>
		/// <param name="maximum"> The inclusive maximum value. </param>
		/// <param name="message"> The message for failed validation. </param>
		/// <param name="excludeRangeValues"> The option to exclude the minimum and maximum values </param>
		/// <returns> True if the value is and within the provided range. </returns>
		private MemberValidator<T> AddMinMaxRange(object minimum, object maximum, string message, bool excludeRangeValues)
		{
			if (typeof(T) == typeof(short))
			{
				return AddMinMaxRange<short>(minimum, maximum, (min, max) => excludeRangeValues ? x => (x > min) && (x < max) : x => (x >= min) && (x <= max), message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(ushort))
			{
				return AddMinMaxRange<ushort>(minimum, maximum, (min, max) => excludeRangeValues ? x => (x > min) && (x < max) : x => (x >= min) && (x <= max), message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(int))
			{
				return AddMinMaxRange<int>(minimum, maximum, (min, max) => excludeRangeValues ? x => (x > min) && (x < max) : x => (x >= min) && (x <= max), message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(uint))
			{
				return AddMinMaxRange<uint>(minimum, maximum, (min, max) => excludeRangeValues ? x => (x > min) && (x < max) : x => (x >= min) && (x <= max), message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(long))
			{
				return AddMinMaxRange<long>(minimum, maximum, (min, max) => excludeRangeValues ? x => (x > min) && (x < max) : x => (x >= min) && (x <= max), message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(ulong))
			{
				return AddMinMaxRange<ulong>(minimum, maximum, (min, max) => excludeRangeValues ? x => (x > min) && (x < max) : x => (x >= min) && (x <= max), message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(float))
			{
				return AddMinMaxRange<float>(minimum, maximum, (min, max) => excludeRangeValues ? x => (x > min) && (x < max) : x => (x >= min) && (x <= max), message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(double))
			{
				return AddMinMaxRange<double>(minimum, maximum, (min, max) => excludeRangeValues ? x => (x > min) && (x < max) : x => (x >= min) && (x <= max), message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(decimal))
			{
				return AddMinMaxRange<decimal>(minimum, maximum, (min, max) => excludeRangeValues ? x => (x > min) && (x < max) : x => (x >= min) && (x <= max), message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(byte))
			{
				return AddMinMaxRange<byte>(minimum, maximum, (min, max) => excludeRangeValues ? x => (x > min) && (x < max) : x => (x >= min) && (x <= max), message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(sbyte))
			{
				return AddMinMaxRange<sbyte>(minimum, maximum, (min, max) => excludeRangeValues ? x => (x > min) && (x < max) : x => (x >= min) && (x <= max), message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(char))
			{
				return AddMinMaxRange<char>(minimum, maximum, (min, max) => excludeRangeValues ? x => (x > min) && (x < max) : x => (x >= min) && (x <= max), message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(string))
			{
				return AddMinMaxRange<string, int>(minimum, maximum, (min, max) => excludeRangeValues
						? x => (x != null) && (x.Length > min) && (x.Length < max)
						: x => (x != null) && (x.Length >= min) && (x.Length <= max),
					message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(DateTime))
			{
				return AddMinMaxRange<DateTime>(minimum, maximum, (min, max) => excludeRangeValues ? x => (x > min) && (x < max) : x => (x >= min) && (x <= max), message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(DateTimeOffset))
			{
				return AddMinMaxRange<DateTimeOffset>(minimum, maximum, (min, max) => excludeRangeValues ? x => (x > min) && (x < max) : x => (x >= min) && (x <= max), message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(OscTimeTag))
			{
				return AddMinMaxRange<OscTimeTag>(minimum, maximum, (min, max) => excludeRangeValues ? x => (x > min) && (x < max) : x => (x >= min) && (x <= max), message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(TimeSpan))
			{
				return AddMinMaxRange<TimeSpan>(minimum, maximum, (min, max) => excludeRangeValues ? x => (x > min) && (x < max) : x => (x >= min) && (x <= max), message) as MemberValidator<T>;
			}

			throw new NotSupportedException($"The type is not supported for {nameof(AddMinMaxRange)}.");
		}

		private MemberValidator<T1> AddNoLessThan<T1>(object minimum, Func<T1, Func<T1, bool>> getValidate, string message)
		{
			return AddNoLessThan<T1, T1>(minimum, getValidate, message);
		}

		private MemberValidator<T1> AddNoLessThan<T1, T2>(object minimum, Func<T2, Func<T1, bool>> getValidate, string message)
		{
			if (minimum is not T2 tMinimum)
			{
				throw new InvalidCastException("Minimum value is the incorrect type.");
			}

			return AddValidation(getValidate(tMinimum), message);
		}

		/// <summary>
		/// Validate an object is equal to or greater than provided minimum value.
		/// </summary>
		/// <param name="minimum"> The inclusive minimum value. </param>
		/// <param name="message"> The message for failed validation. </param>
		/// <returns> True if the value is equal to or greater than the provided value. </returns>
		private MemberValidator<T> AddNoLessThan(object minimum, string message)
		{
			if (typeof(T) == typeof(short))
			{
				return AddNoLessThan<short>(minimum, min => x => x >= min, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(ushort))
			{
				return AddNoLessThan<ushort>(minimum, min => x => x >= min, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(int))
			{
				return AddNoLessThan<int>(minimum, min => x => x >= min, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(uint))
			{
				return AddNoLessThan<uint>(minimum, min => x => x >= min, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(long))
			{
				return AddNoLessThan<long>(minimum, min => x => x >= min, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(ulong))
			{
				return AddNoLessThan<ulong>(minimum, min => x => x >= min, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(float))
			{
				return AddNoLessThan<float>(minimum, min => x => x >= min, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(double))
			{
				return AddNoLessThan<double>(minimum, min => x => x >= min, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(decimal))
			{
				return AddNoLessThan<decimal>(minimum, min => x => x >= min, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(byte))
			{
				return AddNoLessThan<byte>(minimum, min => x => x >= min, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(sbyte))
			{
				return AddNoLessThan<sbyte>(minimum, min => x => x >= min, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(char))
			{
				return AddNoLessThan<char>(minimum, min => x => x >= min, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(string))
			{
				return AddNoLessThan<string, int>(minimum, min => x => x.Length >= min, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(DateTime))
			{
				return AddNoLessThan<DateTime>(minimum, min => x => x >= min, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(DateTimeOffset))
			{
				return AddNoLessThan<DateTimeOffset>(minimum, min => x => x >= min, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(OscTimeTag))
			{
				return AddNoLessThan<OscTimeTag>(minimum, min => x => x >= min, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(TimeSpan))
			{
				return AddNoLessThan<TimeSpan>(minimum, min => x => x >= min, message) as MemberValidator<T>;
			}

			throw new NotSupportedException($"The type is not supported for {nameof(AddNoLessThan)}.");
		}

		private MemberValidator<T1> AddNoMoreThan<T1>(object maximum, Func<T1, Func<T1, bool>> getValidate, string message)
		{
			return AddNoMoreThan<T1, T1>(maximum, getValidate, message);
		}

		private MemberValidator<T1> AddNoMoreThan<T1, T2>(object maximum, Func<T2, Func<T1, bool>> getValidate, string message)
		{
			if (maximum is not T2 tMaximum)
			{
				throw new InvalidCastException("Maximum value is the incorrect type.");
			}

			return AddValidation(getValidate(tMaximum), message);
		}

		/// <summary>
		/// Validate an object is equal to or less than provided minimum value.
		/// </summary>
		/// <param name="maximum"> The inclusive maximum value. </param>
		/// <param name="message"> The message for failed validation. </param>
		/// <returns> True if the value is equal to or less than the provided value. </returns>
		private MemberValidator<T> AddNoMoreThan(object maximum, string message)
		{
			if (typeof(T) == typeof(short))
			{
				return AddNoMoreThan<short>(maximum, max => x => x <= max, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(ushort))
			{
				return AddNoMoreThan<ushort>(maximum, max => x => x <= max, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(int))
			{
				return AddNoMoreThan<int>(maximum, max => x => x <= max, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(uint))
			{
				return AddNoMoreThan<uint>(maximum, max => x => x <= max, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(long))
			{
				return AddNoMoreThan<long>(maximum, max => x => x <= max, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(ulong))
			{
				return AddNoMoreThan<ulong>(maximum, max => x => x <= max, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(float))
			{
				return AddNoMoreThan<float>(maximum, max => x => x <= max, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(double))
			{
				return AddNoMoreThan<double>(maximum, max => x => x <= max, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(decimal))
			{
				return AddNoMoreThan<decimal>(maximum, max => x => x <= max, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(byte))
			{
				return AddNoMoreThan<byte>(maximum, max => x => x <= max, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(sbyte))
			{
				return AddNoMoreThan<sbyte>(maximum, max => x => x <= max, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(char))
			{
				return AddNoMoreThan<char>(maximum, max => x => x <= max, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(string))
			{
				return AddNoMoreThan<string, int>(maximum, max => x => x.Length <= max, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(DateTime))
			{
				return AddNoMoreThan<DateTime>(maximum, max => x => x <= max, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(DateTimeOffset))
			{
				return AddNoMoreThan<DateTimeOffset>(maximum, max => x => x <= max, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(OscTimeTag))
			{
				return AddNoMoreThan<OscTimeTag>(maximum, max => x => x <= max, message) as MemberValidator<T>;
			}
			if (typeof(T) == typeof(TimeSpan))
			{
				return AddNoMoreThan<TimeSpan>(maximum, max => x => x <= max, message) as MemberValidator<T>;
			}

			throw new NotSupportedException($"The type is not supported for {nameof(AddNoLessThan)}.");
		}

		private MemberValidator<T1> AddValidation<T1>(Func<T1, bool> validate, string message)
		{
			var validation = new Validation<T1>(this, message, validate);
			Validations.Add(validation);
			return this as MemberValidator<T1>;
		}

		/// <summary>
		/// Validate a member to determine if it is required.
		/// </summary>
		/// <param name="required"> The flag to determine if it is required. </param>
		/// <param name="message"> The message for failed validation. </param>
		private MemberValidator<T> IsRequired(bool required, string message)
		{
			MemberRequired = required;
			MemberRequiredMessage = message;
			return this;
		}

		#endregion
	}

	/// <summary>
	/// Validation for an object member.
	/// </summary>
	public abstract class MemberValidator
	{
		#region Constructors

		/// <summary>
		/// Creates an instance of a member validator.
		/// </summary>
		protected MemberValidator(MemberInfo info)
		{
			Info = info;
			Validations = new List<IValidation>();
		}

		#endregion

		#region Properties

		/// <summary>
		/// The name of the validator
		/// </summary>
		public string Name => Info.Name;

		/// <summary>
		/// The validations for the validator.
		/// </summary>
		public IList<IValidation> Validations { get; }

		/// <summary>
		/// The info for the member.
		/// </summary>
		protected internal MemberInfo Info { get; }

		/// <summary>
		/// Get the required status.
		/// </summary>
		internal bool MemberRequired { get; set; }

		/// <summary>
		/// Get the required validation.
		/// </summary>
		internal string MemberRequiredMessage { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Tries to validate the property.
		/// </summary>
		/// <returns> Returns true if the validations pass otherwise false. </returns>
		public abstract bool TryValidate(object value);

		/// <summary>
		/// Process the validations.
		/// </summary>
		/// <param name="propertyValue"> The property value. </param>
		/// <param name="failedValidation"> The list of failed validations. </param>
		internal void ProcessValidations(object propertyValue, ICollection<IValidation> failedValidation)
		{
			for (var i = 0; i < Validations.Count; i++)
			{
				if (i >= Validations.Count)
				{
					return;
				}

				var validation = Validations[i];

				if (!validation.TryValidate(propertyValue))
				{
					failedValidation.Add(validation);
				}
			}
		}

		#endregion
	}
}