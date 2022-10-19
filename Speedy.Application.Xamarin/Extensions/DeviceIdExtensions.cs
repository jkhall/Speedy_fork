﻿#region References

#if MONOANDROID
using Android.Provider;
#elif XAMARIN_IOS
using UIKit;
#elif WINDOWS_UWP || NETCOREAPP
using System.Runtime.InteropServices.WindowsRuntime;
using Speedy.Application.Internal;
using Windows.System.Profile;
#endif

#endregion

namespace Speedy.Application.Xamarin.Extensions;

/// <summary>
/// Extension methods for <see cref="DeviceId" />.
/// </summary>
public static class DeviceIdExtensions
{
	#region Methods

	public static DeviceId AddVendorId(this DeviceId builder)
	{
		#if MONOANDROID
		var context = Android.App.Application.Context;
		var id = Settings.Secure.GetString(context.ContentResolver, Settings.Secure.AndroidId);
		builder.AddComponent("VendorId", new DeviceIdComponent(id));
		#elif XAMARIN_IOS
		builder.AddComponent("VendorId", new DeviceIdComponent(UIDevice.CurrentDevice.IdentifierForVendor.AsString()));
		#elif WINDOWS_UWP || NETCOREAPP
		var systemId = SystemIdentification.GetSystemIdForPublisher();
		if (systemId == null)
		{
			builder.AddComponent("VendorId", new DeviceIdComponent(string.Empty));
		}
		else
		{
			var systemIdBytes = systemId.Id.ToArray();
			var encoder = new Base32ByteArrayEncoder(Base32ByteArrayEncoder.CrockfordAlphabet);
			var id = encoder.Encode(systemIdBytes);
			builder.AddComponent("VendorId", new DeviceIdComponent(id));
		}
		#endif

		return builder;
	}

	#endregion
}