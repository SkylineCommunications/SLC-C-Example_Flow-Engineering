namespace QAction_2
{
	using System;
	using System.Diagnostics;
	using System.Threading;
	using System.Threading.Tasks;

	public static class WaitHelper
	{
		public static void WaitUntil(Func<bool> condition, TimeSpan timeout, int intervalMs = 100)
		{
			var stopwatch = Stopwatch.StartNew();

			while (!condition())
			{
				if (stopwatch.Elapsed > timeout)
				{
					throw new TimeoutException("Timeout occurred while waiting for condition to be true.");
				}

				Thread.Sleep(intervalMs);
			}
		}

		public static async Task WaitUntilAsync(Func<bool> condition, TimeSpan timeout, int intervalMs = 100)
		{
			var stopwatch = Stopwatch.StartNew();

			while (!condition())
			{
				if (stopwatch.Elapsed > timeout)
				{
					throw new TimeoutException("Timeout occurred while waiting for condition to be true.");
				}

				await Task.Delay(intervalMs);
			}
		}
	}
}
