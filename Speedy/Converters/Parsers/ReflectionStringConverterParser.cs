﻿#region References

using System;
using System.Collections.Concurrent;
using System.Reflection;
using Speedy.Extensions;

#endregion

namespace Speedy.Converters.Parsers
{
	internal class ReflectionStringConverterParser : IStringConverterParser
	{
		#region Fields

		private static readonly ConcurrentDictionary<Type, MethodInfo> _methods;

		#endregion

		#region Constructors

		static ReflectionStringConverterParser()
		{
			_methods = new ConcurrentDictionary<Type, MethodInfo>();
		}

		#endregion

		#region Methods

		/// <inheritdoc />
		public bool SupportsType(Type targetType)
		{
			var method = GetTryParseMethod(targetType);

			return method != null;
		}

		/// <inheritdoc />
		public bool TryParse(Type targetType, string value, out object result)
		{
			try
			{
				var method = GetTryParseMethod(targetType);
				result = targetType.GetDefault();

				if (method == null)
				{
					return false;
				}

				var parameters = new object[] { value, null };
				var success = (bool) method.Invoke(null, parameters);

				if (!success)
				{
					return false;
				}

				result = parameters[1];
				return true;
			}
			catch (Exception)
			{
				result = targetType.GetDefault();
				return false;
			}
		}

		private static MethodInfo GetTryParseMethod(Type targetType)
		{
			var method = _methods.GetOrAdd(targetType,
				x => x.GetTypeInfo().GetCachedMethod("TryParse", typeof(string), targetType.MakeByRefType())
			);

			return method;
		}

		#endregion
	}
}