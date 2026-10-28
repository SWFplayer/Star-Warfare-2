using System;
using System.Globalization;

public class TimeUtil
{
	public static DateTime ConvertServerTicksToLocalTime(long serverTicks)
	{
		long ticks = new DateTime(1970, 1, 1, 8, 0, 0, new CultureInfo("en-US", false).Calendar).Ticks;
		return new DateTime(serverTicks * 10000 + ticks);
	}
}
