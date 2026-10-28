using UnityEngine;

public class CDVoidCompareCondition : CDCompareCondition
{
	public ConditionDelegate mDelegate;

	public override bool IsTriggered(GameUnit gameUnit)
	{
		if (Time.time - GetLastTime(gameUnit) >= mCD && (mTimer == null || mTimer.Ready2()) && mDelegate(gameUnit))
		{
			SetLastTime(gameUnit, Time.time);
			return true;
		}
		return false;
	}
}
