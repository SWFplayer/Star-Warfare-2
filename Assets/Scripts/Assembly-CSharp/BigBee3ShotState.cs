using UnityEngine;

public class BigBee3ShotState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;
	}

	public override void Start(Enemy enemy)
	{
		if (!enemy.StateData.ContainsKey(this))
		{
			enemy.StateData.Add(this, new MyData());
		}
		MyData myData = enemy.StateData[this] as MyData;
		myData.InnerState = 0;
		enemy.PlayAnimation("attack_sp", WrapMode.ClampForever);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		MyData myData = enemy.StateData[this] as MyData;
		BigBee bigBee = enemy as BigBee;
		if (myData.InnerState < BigBeeParameter.BigBee3ShotTime.Length && bigBee.CurrentAnimTime >= BigBeeParameter.BigBee3ShotTime[myData.InnerState])
		{
			myData.InnerState++;
			bigBee.SpawnBullet();
		}
	}

	public override void End(Enemy enemy)
	{
	}
}
