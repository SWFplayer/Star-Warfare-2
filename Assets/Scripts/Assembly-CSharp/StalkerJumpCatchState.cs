using UnityEngine;

public class StalkerJumpCatchState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public Vector3[] Positions = new Vector3[2];

		public int InternalState;
	}

	public override void Start(Enemy enemy)
	{
		enemy.SetCatchingTimeNow();
		enemy.SetUpdatePosTimeNow();
		if (!enemy.StateData.ContainsKey(this))
		{
			enemy.StateData.Add(this, new MyData());
		}
		MyData myData = enemy.StateData[this] as MyData;
		myData.InternalState = 0;
		Vector3 vector = (enemy.GetPosition() + enemy.GetTargetPosition()) / 2f;
		float num = Vector3.Magnitude(enemy.GetPosition() - enemy.GetTargetPosition()) / 2f;
		myData.Positions[0] = vector + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)) * num;
		myData.Positions[1] = vector + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)) * num;
		enemy.MoveToward(myData.Positions[0], -1f);
		enemy.PlayAnimation(AnimationString.STALKER_JUMP_CATCH, WrapMode.ClampForever);
		enemy.LookAtTargetHorizontal();
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		MyData myData = enemy.StateData[this] as MyData;
		enemy.LookAtTargetHorizontal();
		if (myData.InternalState == 0 && enemy.IsAnimationFinish())
		{
			myData.InternalState++;
		}
	}

	public override bool IsFinish(GameUnit gameUnit)
	{
		MyData myData = gameUnit.StateData[this] as MyData;
		return myData.InternalState >= 1;
	}
}
