﻿#region References

using System;
using Speedy.Extensions;

#endregion

namespace Speedy.Converters.Parsers
{
	internal class StringConverterParser : IStringConverterParser
	{
		#region Methods

		/// <inheritdoc />
		public bool SupportsType(Type targetType)
		{
			return targetType == typeof(string);
		}

		/// <inheritdoc />
		public bool TryParse(Type targetType, string value, out object result)
		{
			if (!SupportsType(targetType))
			{
				result = targetType.GetDefault();
				return false;
			}

			result = value;
			return true;
		}

		#endregion
	}
}