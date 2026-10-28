using UnityEngine;

public class RobotMoveIdleState : GameUnitState
{
	public override void Start(GameUnit gameUnit)
	{
		Robot robot = (Robot)gameUnit;
		robot.MoveStateStartTime = Time.time;
		robot.MoveIdleDuration = Random.Range(Robot.ROBOT_STOP_TIME.x, Robot.ROBOT_STOP_TIME.y);
	}

	public override void Update(GameUnit gameUnit)
	{
		Robot robot = (Robot)gameUnit;
		robot.inputController.inputInfo.moveDirection = Vector3.zero;
	}

	public override void End(GameUnit gameUnit)
	{
	}
}
