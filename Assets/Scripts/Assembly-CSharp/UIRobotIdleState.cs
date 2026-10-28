using UnityEngine;

public class UIRobotIdleState : RobotState
{
	public float mDuration;

	private Timer mTimer = new Timer();

	private Timer mLoginTimer = new Timer();

	public override void Start(UIRobot robot)
	{
		mDuration = Random.Range(1f, 2f);
		mTimer.SetTimer(mDuration, true);
		mLoginTimer.SetTimer(60f, false);
		robot.sendingCreatingRoomRequest = false;
		robot.sendingJoingRequest = false;
	}

	public override void NextState(UIRobot robot)
	{
		if (GetInStateTime() > 120f)
		{
			robot.IsDead = true;
		}
		if (mLoginTimer.Ready2())
		{
			RoleLoginRequest request = new RoleLoginRequest(robot.userName, robot.id, "2014-07-25 14:42:58", 99, false);
			robot.network.SendRequest(request);
			Debug.Log("重新log in. " + robot.id + "  " + robot.sendingCreatingRoomRequest);
		}
		if (!(GetInStateTime() > 1f) || !mTimer.Ready2())
		{
			return;
		}
		if (robot.id % 3 == 0)
		{
			if (!robot.sendingCreatingRoomRequest)
			{
				robot.sendingCreatingRoomRequest = true;
				CreateRoomRequest request2 = new CreateRoomRequest(robot.userName + "'s room", -1, 3, false, 2, 1, 1, 500, 0, 99, 0);
				robot.network.SendRequest(request2);
				Debug.LogError(robot.userName + " createRoom");
			}
		}
		else if (!robot.sendingJoingRequest)
		{
			RobotRoom robotRoom = robot.uiScript.FindIdleRoom();
			if (robotRoom != null)
			{
				byte rankID = 1;
				JoinRoomRequest request3 = new JoinRoomRequest(robotRoom.roomID, rankID, 500);
				robot.network.SendRequest(request3);
				robotRoom.PlayerNum++;
				robot.sendingJoingRequest = true;
			}
		}
	}
}
