public class UIRobotWaitingLeaveRoomState : RobotState
{
	public override void NextState(UIRobot robot)
	{
		if (GetInStateTime() > 2f)
		{
			robot.SetState(robot.IdleState);
		}
	}
}
