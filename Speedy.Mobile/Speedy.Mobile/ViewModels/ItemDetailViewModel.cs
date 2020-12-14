﻿#region References

using Speedy.Data.Client;

#endregion

namespace Speedy.Mobile.ViewModels
{
	public class ItemDetailViewModel : BaseViewModel
	{
		#region Constructors

		public ItemDetailViewModel(ClientLogEvent item = null)
		{
			Title = item?.Message;
			Item = item;
		}

		#endregion

		#region Properties

		public ClientLogEvent Item { get; set; }

		#endregion
	}
}