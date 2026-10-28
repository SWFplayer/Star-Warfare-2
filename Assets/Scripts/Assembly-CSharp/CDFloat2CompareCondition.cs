using UnityEngine;

public class CDFloat2CompareCondition : CDCompareCondition
{
	public Float2ConditionDelegate mDelegate;

	public float mValue1;

	public float mValue2;

	public override bool IsTriggered(GameUnit gameUnit)
	{
		if (Time.time - GetLastTime(gameUnit) >= mCD && (mTimer == null || mTimer.Ready2()) && mDelegate(gameUnit, mValue1, mValue2))
		{
			SetLastTime(gameUnit, Time.time);
			return true;
		}
		return false;
	}
}
