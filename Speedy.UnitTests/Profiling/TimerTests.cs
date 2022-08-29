﻿#region References

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speedy.Profiling;

#endregion

namespace Speedy.UnitTests.Profiling
{
	[TestClass]
	public class TimerTests : BaseTests
	{
		#region Methods

		[TestMethod]
		public void AddAverageTimerShouldWork()
		{
			TestHelper.CurrentTime = new DateTime(2020, 04, 23, 07, 56, 12);

			var timer = new Timer();
			var count = 0;

			timer.PropertyChanged += (sender, args) =>
			{
				if (args.PropertyName == nameof(Timer.Elapsed))
				{
					count++;
				}
			};

			Assert.IsFalse(timer.IsRunning);
			Assert.AreEqual(0, timer.Elapsed.Ticks);

			var averageTimer = new AverageTimer();
			Assert.AreEqual(0, averageTimer.Elapsed.TotalMilliseconds);
			averageTimer.Start();
			Assert.AreEqual(0, averageTimer.Elapsed.TotalMilliseconds);
			Assert.AreEqual(0, count);
			TestHelper.CurrentTime += TimeSpan.FromMilliseconds(123456);
			Assert.AreEqual(123456, averageTimer.Elapsed.TotalMilliseconds);
			Assert.AreEqual(0, count);
			averageTimer.Stop();

			Assert.AreEqual(123456, averageTimer.Elapsed.TotalMilliseconds);
			Assert.IsFalse(timer.IsRunning);
			Assert.AreEqual(0, count);

			timer.Add(averageTimer);

			Assert.IsFalse(timer.IsRunning);
			Assert.AreEqual(123456, timer.Elapsed.TotalMilliseconds);
			Assert.AreEqual("00:02:03.4560000", timer.Elapsed.ToString());
			Assert.AreEqual(1, count);
		}

		[TestMethod]
		public void AddTimeSpanShouldWork()
		{
			var timer = new Timer();
			var count = 0;

			timer.PropertyChanged += (sender, args) =>
			{
				if (args.PropertyName == nameof(Timer.Elapsed))
				{
					count++;
				}
			};

			Assert.IsFalse(timer.IsRunning);
			Assert.AreEqual(0, timer.Elapsed.Ticks);
			Assert.AreEqual(0, count);

			timer.Add(TimeSpan.FromMilliseconds(123456));

			Assert.IsFalse(timer.IsRunning);
			Assert.AreEqual(123456, timer.Elapsed.TotalMilliseconds);
			Assert.AreEqual("00:02:03.4560000", timer.Elapsed.ToString());
			Assert.AreEqual(1, count);
		}

		[TestMethod]
		public void ShouldResetToProvidedElapsed()
		{
			var timer = new Timer();
			Assert.IsFalse(timer.IsRunning);
			Assert.AreEqual(0, timer.Elapsed.Ticks);

			timer.Reset(TimeSpan.FromMilliseconds(1234));
			Assert.IsFalse(timer.IsRunning);
			Assert.AreEqual(1234, timer.Elapsed.TotalMilliseconds);

			timer.Reset();
			Assert.IsFalse(timer.IsRunning);
			Assert.AreEqual(0, timer.Elapsed.TotalMilliseconds);
		}

		[TestMethod]
		public void ShouldRestartWithProvidedStartTime()
		{
			TestHelper.CurrentTime = new DateTime(2020, 04, 23, 07, 56, 12);

			var timer = new Timer();
			Assert.IsFalse(timer.IsRunning);
			Assert.AreEqual(0, timer.Elapsed.Ticks);

			timer.Restart(new DateTime(2020, 04, 23, 07, 53, 46));

			Assert.IsTrue(timer.IsRunning);
			Assert.AreEqual(146000, timer.Elapsed.TotalMilliseconds);

			timer.Restart();
			Assert.IsTrue(timer.IsRunning);
			Assert.AreEqual(0, timer.Elapsed.TotalMilliseconds);

			timer.Restart();
			TestHelper.CurrentTime += TimeSpan.FromMilliseconds(123456);

			Assert.IsTrue(timer.IsRunning);
			Assert.AreEqual(123456, timer.Elapsed.TotalMilliseconds);
		}

		[TestMethod]
		public void ShouldTrackUsingTimeService()
		{
			TestHelper.CurrentTime = new DateTime(2020, 04, 23, 07, 56, 12);
			var timer = new Timer();

			Assert.IsFalse(timer.IsRunning);
			
			timer.Start();

			Assert.IsTrue(timer.IsRunning);
			TestHelper.CurrentTime += TimeSpan.FromTicks(1);

			timer.Stop();

			Assert.IsFalse(timer.IsRunning);
			Assert.AreEqual(1, timer.Elapsed.Ticks);
		}

		[TestMethod]
		public void StartWithDateTimeShouldStartTimerInPast()
		{
			TestHelper.CurrentTime = new DateTime(2020, 04, 23, 07, 56, 12);

			var timer = new Timer();
			Assert.IsFalse(timer.IsRunning);
			Assert.AreEqual(0, timer.Elapsed.Ticks);

			timer.Start(TestHelper.CurrentTime.AddMilliseconds(-12345));
			Assert.IsTrue(timer.IsRunning);
			Assert.AreEqual(12345, timer.Elapsed.TotalMilliseconds);

			timer.Stop();
			Assert.IsFalse(timer.IsRunning);
			Assert.AreEqual(12345, timer.Elapsed.TotalMilliseconds);
		}

		[TestMethod]
		public void StopWithDateTimeShouldStopTimerInPast()
		{
			TestHelper.CurrentTime = new DateTime(2020, 04, 23, 07, 56, 12);

			var timer = new Timer();
			Assert.IsFalse(timer.IsRunning);
			Assert.AreEqual(0, timer.Elapsed.Ticks);

			timer.Start();
			Assert.IsTrue(timer.IsRunning);
			Assert.AreEqual(0, timer.Elapsed.TotalMilliseconds);

			TestHelper.CurrentTime += TimeSpan.FromSeconds(12);

			timer.Stop(new DateTime(2020, 04, 23, 07, 56, 15));
			Assert.IsFalse(timer.IsRunning);
			Assert.AreEqual(3000, timer.Elapsed.TotalMilliseconds);
		}

		#endregion
	}
}