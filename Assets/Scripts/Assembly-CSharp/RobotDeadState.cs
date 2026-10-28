using UnityEngine;

public class RobotDeadState : GameUnitState
{
	public override void Start(GameUnit gameUnit)
	{
		Debug.Log("Dead");
		Robot robot = (Robot)gameUnit;
		robot.PlayAnimationAllLayers(AnimationString.Dead01, WrapMode.ClampForever);
		AudioManager.GetInstance().PlaySoundAt(AudioName.PLAYER_DEAD, robot.GetPosition());
		robot.StopNavMesh();
	}

	public override void Update(GameUnit gameUnit)
	{
		Robot robot = (Robot)gameUnit;
		if (robot.GetRobotType() == ERobotType.XIAOHUOBAN && Time.time - robot.StateStartTime > Robot.ROBOT_DISAPPEAR_AFTER_DEAD && robot.GetObject().activeSelf)
		{
			robot.HandleMessage(EConditionMessage.EGoneTeleport);
		}
	}

	public override void End(GameUnit gameUnit)
	{
	}
}
