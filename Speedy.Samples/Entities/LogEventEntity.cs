#region References

using System;

#endregion

namespace Speedy.Samples.Entities
{
	public class LogEventEntity : Entity<string>, ILogEvent, IModifiableEntity
	{
		#region Properties

		/// <inheritdoc />
		public DateTime CreatedOn { get; set; }

		public override string Id { get; set; }

		public string Message { get; set; }

		/// <inheritdoc />
		public DateTime ModifiedOn { get; set; }

		#endregion

		#region Methods

		/// <inheritdoc />
		public override bool CanBeModified()
		{
			return false;
		}

		#endregion
	}
}