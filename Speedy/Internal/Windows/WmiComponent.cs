﻿#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.Versioning;
using Speedy.Runtime;

#endregion

namespace Speedy.Internal.Windows;

#if (!NETSTANDARD)
[SupportedOSPlatform("windows")]
#endif
internal class WmiComponent : IDeviceIdComponent
{
	#region Constructors

	public WmiComponent(string wmiClass, params string[] wmiProperty)
	{
		WmiClass = wmiClass;
		WmiProperty = wmiProperty;
	}

	#endregion

	#region Properties

	public string WmiClass { get; }

	public string[] WmiProperty { get; }

	#endregion

	#region Methods

	/// <inheritdoc />
	public virtual string GetValue()
	{
		return GetIdentifier(WmiClass, WmiProperty);
	}

	protected string GetIdentifier(string wmiClass, params string[] wmiProperty)
	{
		var response = new List<string>();
		using (var mos = new ManagementObjectSearcher("SELECT " + string.Join(",", wmiProperty) + " FROM " + wmiClass))
		{
			foreach (var o in mos.Get())
			{
				using (o)
				{
					var mo = (ManagementObject) o;
					var value = string.Join(",", wmiProperty.Select(x =>
					{
						try
						{
							return mo[x]?.ToString() ?? string.Empty;
						}
						catch (Exception)
						{
							return string.Empty;
						}
					}));

					response.Add(value);
				}
			}
		}

		return string.Join(",", response.OrderBy(x => x));
	}

	#endregion
}