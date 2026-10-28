using UnityEngine;

public abstract class CDCompareCondition : BaseCompareCondition
{
	public float mCD;

	protected Timer mTimer;

	public bool StartNow = true;

	public void SetTimerInterval(float interval)
	{
		mTimer = new Timer(interval);
	}

	protected float GetLastTime(GameUnit gameUnit)
	{
		if (!gameUnit.CompareData.ContainsKey(this))
		{
			gameUnit.CompareData.Add(this, new CDCompareDate());
			if (!StartNow)
			{
				((CDCompareDate)gameUnit.CompareData[this]).LastTriggerTime = Time.time;
			}
		}
		return ((CDCompareDate)gameUnit.CompareData[this]).LastTriggerTime;
	}

	protected void SetLastTime(GameUnit gameUnit, float time)
	{
		((CDCompareDate)gameUnit.CompareData[this]).LastTriggerTime = time;
	}
}
