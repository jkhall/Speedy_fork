﻿#region References

using System;
using Speedy.Protocols.Osc;

#endregion

namespace Speedy.Extensions
{
	/// <summary>
	/// Extensions for date time
	/// </summary>
	public static class DateTimeExtensions
	{
		#region Constants

		/// <summary>
		/// The amount of ticks in the Max Date / Time value.
		/// </summary>
		public const long MaxDateTimeTicks = 3155378975999999999L;

		/// <summary>
		/// The amount of ticks in the Min Date / Time value.
		/// </summary>
		public const long MinDateTimeTicks = 0L;

		#endregion

		#region Methods

		/// <summary>
		/// Returns the larger of two specified date times.
		/// </summary>
		/// <param name="left"> The left date and time. </param>
		/// <param name="right"> The right date and time. </param>
		/// <returns> The larger of the two. </returns>
		public static DateTime Max(this DateTime left, DateTime right)
		{
			return left.Ticks >= right.Ticks ? left : right;
		}
		
		/// <summary>
		/// Returns the smaller of two specified date times.
		/// </summary>
		/// <param name="left"> The left date and time. </param>
		/// <param name="right"> The right date and time. </param>
		/// <returns> The smaller of the two. </returns>
		public static DateTime Min(this DateTime left, DateTime right)
		{
			return left.Ticks <= right.Ticks ? left : right;
		}

		/// <summary>
		/// Convert a DateTime to an OscTimeTag.
		/// </summary>
		/// <param name="time"> The time to be converted. </param>
		/// <returns> The DateTime in OscTimeTag format. </returns>
		public static OscTimeTag ToOscTimeTag(this DateTime time)
		{
			return new OscTimeTag(time);
		}

		#endregion
	}
}