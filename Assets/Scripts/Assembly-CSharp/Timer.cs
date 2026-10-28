using UnityEngine;

public class Timer
{
	protected TimerInfo info = new TimerInfo();

	protected bool start;

	public string Name { get; set; }

	public Timer()
	{
	}

	public Timer(float interval)
	{
		SetTimer(interval, false);
	}

	public Timer(float interval, bool readyAtStart)
	{
		SetTimer(interval, readyAtStart);
	}

	public void SetTimer(float interval, bool readyAtStart)
	{
		if (readyAtStart)
		{
			info.lastDoTime = -9999f;
		}
		else
		{
			info.lastDoTime = Time.time;
		}
		info.interval = interval;
		start = true;
	}

	public void Do()
	{
		info.lastDoTime = Time.time;
		start = true;
	}

	public bool Ready()
	{
		if (start && Time.time - info.lastDoTime > info.interval)
		{
			return true;
		}
		return false;
	}

	public void Pause()
	{
		info.lastDoTime = Time.time;
		start = false;
	}

	public bool Ready2()
	{
		if (start)
		{
			bool flag = Time.time - info.lastDoTime > info.interval;
			if (flag)
			{
				Do();
			}
			return flag;
		}
		return false;
	}

	public float GetInterval()
	{
		return info.interval;
	}

	public float GetTimeSpan()
	{
		if (start)
		{
			return Mathf.Min(Time.time - info.lastDoTime, info.interval);
		}
		return 0f;
	}
}
