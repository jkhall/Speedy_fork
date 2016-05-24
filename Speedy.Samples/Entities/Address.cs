﻿#region References

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Speedy.Sync;

#endregion

namespace Speedy.Samples.Entities
{
	[Serializable]
	public class Address : SyncEntity
	{
		#region Constructors

		[SuppressMessage("ReSharper", "VirtualMemberCallInContructor")]
		public Address()
		{
			People = new Collection<Person>();
			IgnoreProperties.AddRange(nameof(LinkedAddress), nameof(LinkedAddressId), nameof(People));
		}

		#endregion

		#region Properties

		public string City { get; set; }
		public string Line1 { get; set; }
		public string Line2 { get; set; }
		public virtual Address LinkedAddress { get; set; }
		public int? LinkedAddressId { get; set; }
		public Guid? LinkedAddressSyncId { get; set; }
		public virtual ICollection<Person> People { get; set; }
		public string Postal { get; set; }
		public string State { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Updates the sync ids of relationships.
		/// </summary>
		public override void UpdateLocalSyncIds()
		{
			this.UpdateIf(() => (LinkedAddress != null) && (LinkedAddress.SyncId != LinkedAddressSyncId), () => LinkedAddressSyncId = LinkedAddress.SyncId);
		}

		#endregion
	}
}