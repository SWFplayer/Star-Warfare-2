using UnityEngine;

public class RobotMasterInRoomState : RobotState
{
	public override void Start(UIRobot robot)
	{
		robot.joinedPlayer = 0;
		robot.Friends.Clear();
		robot.sendingStartGameRequest = false;
	}

	public override void NextState(UIRobot robot)
	{
		if (GetInStateTime() > 4f)
		{
			if (!robot.sendingStartGameRequest && robot.joinedPlayer == robot.maxPlayer)
			{
				robot.sendingStartGameRequest = true;
				StartGameRequest request = new StartGameRequest();
				robot.network.SendRequest(request);
				Debug.Log("Robot " + robot.userName + " start a game, maxPlayer: " + robot.maxPlayer);
			}
			EnterStateTime = Time.time;
		}
	}
}
