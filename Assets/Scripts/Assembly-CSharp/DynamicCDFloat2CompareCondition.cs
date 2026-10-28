using UnityEngine;

public class DynamicCDFloat2CompareCondition : CDCompareCondition
{
	public class MyData : CDCompareDate
	{
		public float DynamicCD;
	}

	public float mValue1;

	public float mValue2;

	public override bool IsTriggered(GameUnit gameUnit)
	{
		if (!gameUnit.CompareData.ContainsKey(this))
		{
			gameUnit.CompareData.Add(this, new MyData());
			MyData myData = gameUnit.CompareData[this] as MyData;
			myData.LastTriggerTime = Time.time;
			myData.DynamicCD = Random.Range(mValue1, mValue2);
		}
		MyData myData2 = gameUnit.CompareData[this] as MyData;
		if (Time.time - myData2.LastTriggerTime >= myData2.DynamicCD)
		{
			myData2.LastTriggerTime = Time.time;
			myData2.DynamicCD = Random.Range(mValue1, mValue2);
			return true;
		}
		return false;
	}
}
