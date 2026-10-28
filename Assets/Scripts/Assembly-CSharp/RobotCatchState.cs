using UnityEngine;

public class RobotCatchState : GameUnitState
{
	public override void Start(GameUnit gameUnit)
	{
		Robot robot = (Robot)gameUnit;
		robot.MoveStateStartTime = Time.time;
		robot.UpdateNavMeshInCatching(true);
	}

	public override void Update(GameUnit gameUnit)
	{
		Robot robot = (Robot)gameUnit;
		robot.inputController.inputInfo.moveDirection = Vector3.Normalize(robot.GetTargetPosition() - robot.GetPosition());
	}

	public override void End(GameUnit gameUnit)
	{
		gameUnit.StopNavMesh();
	}
}
