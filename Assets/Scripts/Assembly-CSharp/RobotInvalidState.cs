using UnityEngine;

public class RobotInvalidState : GameUnitState
{
	public class MyData : GameUnitStateData
	{
		public bool IsDead;

		public float EnterTime;

		public float HitTime;

		public bool IsHit;
	}

	public override void Start(GameUnit gameUnit)
	{
		if (!gameUnit.StateData.ContainsKey(this))
		{
			gameUnit.StateData.Add(this, new MyData());
		}
		MyData myData = gameUnit.StateData[this] as MyData;
		myData.IsDead = gameUnit.Hp == 0;
		myData.IsHit = false;
		myData.EnterTime = Time.time;
	}

	public override void Update(GameUnit gameUnit)
	{
		MyData myData = gameUnit.StateData[this] as MyData;
		Robot robot = gameUnit as Robot;
		if (!myData.IsHit)
		{
			if (robot.GetLeader().LastHitTargetTime > myData.EnterTime + 1f)
			{
				myData.IsHit = true;
				myData.HitTime = Time.time;
			}
		}
		else if (Time.time - myData.HitTime > Robot.ROBOT_BACK_DURATION)
		{
			robot.Reset();
			if (myData.IsDead)
			{
				robot.IsJustReturnedToGame = true;
				robot.Hp = robot.MaxHp;
			}
			robot.GetTransform().position = robot.GetLeader().GetPosition() + robot.GetLeader().GetTransform().TransformDirection(new Vector3(2f, 0f, 2f));
			robot.GetObject().SetActive(true);
			robot.SetState(Robot.RobotMoveIdleState, 0);
			robot.SetState(Robot.RobotIdleState, 1);
		}
	}
}
