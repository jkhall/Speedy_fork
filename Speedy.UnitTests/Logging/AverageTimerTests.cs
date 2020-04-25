﻿#region References

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speedy.Logging;
using System;

#endregion

namespace Speedy.UnitTests.Logging
{
	[TestClass]
	public class AverageTimerTests : BaseTests
	{
		[TestMethod]
		public void CancelShouldResetTimer()
		{
			var currentTime = new DateTime(2020, 04, 23, 07, 56, 00);
			var timer = new AverageTimer(4);
			
			TimeService.UtcNowProvider = () => currentTime;

			timer.Start();
			currentTime = currentTime.AddMilliseconds(123);

			Assert.IsTrue(timer.IsRunning);
			Assert.AreEqual(123, timer.Elapsed.Milliseconds);
			Assert.AreEqual(0, timer.Average.Ticks);

			timer.Cancel();

			Assert.IsFalse(timer.IsRunning);
			Assert.AreEqual(0, timer.Elapsed.Milliseconds);
			Assert.AreEqual(0, timer.Average.Ticks);
		}
		
		[TestMethod]
		public void ShouldAverageWithLimit()
		{
			var currentTime = new DateTime(2020, 04, 23, 07, 56, 00);
			var timer = new AverageTimer(4);
			var values = new [] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			
			TimeService.UtcNowProvider = () => currentTime;

			for (int i = 0; i < values.Length; i++)
			{
				int value = values[i];

				timer.Start();
				currentTime = currentTime.AddTicks(value);
				timer.Stop();

				// Just bump up to ensure average is not borked by time moving
				currentTime = currentTime.AddTicks(50 + i);
			}

			// 6 + 7 + 8 + 9 = 30 / 4 = 7
			Assert.IsFalse(timer.IsRunning);
			Assert.AreEqual(9, timer.Elapsed.Ticks);
			Assert.AreEqual(7, timer.Average.Ticks);
			Assert.AreEqual(4, timer.Samples);
		}
		
		[TestMethod]
		public void ShouldAverageOverTime()
		{
			var dateTime = new DateTime(2020, 04, 23, 07, 56, 12);
			var timer = new AverageTimer(10);
			
			Assert.IsFalse(timer.IsRunning);
			TimeService.UtcNowProvider = () => dateTime;

			timer.Start();

			Assert.IsTrue(timer.IsRunning);
			TimeService.UtcNowProvider = () => dateTime.AddTicks(10);
			
			timer.Stop();

			Assert.IsFalse(timer.IsRunning);
			Assert.AreEqual(10, timer.Elapsed.Ticks);
			Assert.AreEqual(10, timer.Average.Ticks);
			Assert.AreEqual(1, timer.Samples);

			// Just bump up to ensure average is borked by time moving
			TimeService.UtcNowProvider = () => dateTime.AddTicks(100);

			timer.Start();

			Assert.IsTrue(timer.IsRunning);
			TimeService.UtcNowProvider = () => dateTime.AddTicks(120);
			
			timer.Stop();

			// 10 + 20 = 30 / 2 = 15
			Assert.IsFalse(timer.IsRunning);
			Assert.AreEqual(20, timer.Elapsed.Ticks);
			Assert.AreEqual(15, timer.Average.Ticks);
			Assert.AreEqual(2, timer.Samples);

			// Just bump up to ensure average is borked by time moving
			TimeService.UtcNowProvider = () => dateTime.AddTicks(131);
			
			timer.Start();

			Assert.IsTrue(timer.IsRunning);
			TimeService.UtcNowProvider = () => dateTime.AddTicks(140);
			
			timer.Stop();

			// 10 + 20 + 9 = 39 / 3 = 13
			Assert.IsFalse(timer.IsRunning);
			Assert.AreEqual(9, timer.Elapsed.Ticks);
			Assert.AreEqual(13, timer.Average.Ticks);
			Assert.AreEqual(3, timer.Samples);
		}
	}
}