public class PerKillCompareCondition : CDCompareCondition
{
	public Int2ConditionDelegate mDelegate;

	public int mValue1;

	public override bool IsTriggered(GameUnit gameUnit)
	{
		if (!gameUnit.CompareData.ContainsKey(this))
		{
			gameUnit.CompareData.Add(this, new PerKillCompareDate());
		}
		int times = ((PerKillCompareDate)gameUnit.CompareData[this]).Times;
		if ((mTimer == null || mTimer.Ready2()) && mDelegate(gameUnit, mValue1, times))
		{
			((PerKillCompareDate)gameUnit.CompareData[this]).Times++;
			return true;
		}
		return false;
	}
}
