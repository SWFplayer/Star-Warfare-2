using UnityEngine;

public class CDFloatCompareCondition : CDCompareCondition
{
	public FloatConditionDelegate mDelegate;

	public float mValue;

	public override bool IsTriggered(GameUnit gameUnit)
	{
		if (Time.time - GetLastTime(gameUnit) >= mCD && (mTimer == null || mTimer.Ready2()) && mDelegate(gameUnit, mValue))
		{
			SetLastTime(gameUnit, Time.time);
			return true;
		}
		return false;
	}
}
