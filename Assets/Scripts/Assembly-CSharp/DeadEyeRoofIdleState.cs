using UnityEngine;

public class DeadEyeRoofIdleState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public float time;
	}

	public override void Start(Enemy enemy)
	{
		DeadEye deadEye = (DeadEye)enemy;
		if (!enemy.StateData.ContainsKey(this))
		{
			enemy.StateData.Add(this, new MyData());
		}
		MyData myData = enemy.StateData[this] as MyData;
		enemy.PlayAnimation(AnimationString.DEAD_EYE_ROOF_IDLE, WrapMode.Loop);
		if (Random.value <= DeadEyeParameter.ROOF_IDLE_TIME_SHORT_CHANCE)
		{
			myData.time = 0.2f;
		}
		else if (deadEye.GetMindState() == EnemyBoss.MindState.NORMAL)
		{
			myData.time = DeadEyeParameter.ROOF_IDLE_TIME[0];
		}
		else if (deadEye.GetMindState() == EnemyBoss.MindState.RAGE1)
		{
			myData.time = DeadEyeParameter.ROOF_IDLE_TIME[1];
		}
		else
		{
			Debug.LogError("No time for roof idle");
		}
	}

	public override bool IsFinish(GameUnit gameUnit)
	{
		MyData myData = gameUnit.StateData[this] as MyData;
		return Time.time - gameUnit.StateStartTime >= myData.time;
	}
}
