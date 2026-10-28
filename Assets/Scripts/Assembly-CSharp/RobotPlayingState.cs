using UnityEngine;

public class RobotPlayingState : RobotState
{
	public override void NextState(UIRobot robot)
	{
		if (robot.sendingTimer.Ready())
		{
			robot.SendInput();
			SendTransformStateRequest request = new SendTransformStateRequest(robot.o.transform.position, robot.o.transform.rotation.ToEuler(), robot.timeMgr.NetworkTime);
			robot.network.SendRequest(request);
			robot.SpawnEnemy();
			robot.sendingTimer.Do();
		}
		robot.Update();
		robot.Move();
		robot.timeMgr.RobotLoop(robot.network);
		robot.UpdateEnemy();
		if (GetInStateTime() > robot.PlayingDuration)
		{
			LeaveRoomRequest request2 = new LeaveRoomRequest();
			robot.network.SendRequest(request2);
			robot.SetState(robot.WaitingLeaveState);
			robot.lobby.IsMasterPlayer = false;
			EnterStateTime = Time.time;
			robot.ClearAllEnemies();
			Debug.Log("Robot " + robot.userName + " left a game.");
		}
	}
}
