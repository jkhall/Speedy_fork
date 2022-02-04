﻿#region References

using Speedy.Sync;

#endregion

namespace Speedy.Data.SyncApi
{
	/// <summary>
	/// Represents the public setting model.
	/// </summary>
	public class Setting : SyncModel<long>
	{
		#region Constructors

		public Setting()
		{
			ResetChangeTracking();
		}

		#endregion

		#region Properties

		public override long Id { get; set; }

		public string Name { get; set; }

		public string Value { get; set; }

		#endregion
	}
}