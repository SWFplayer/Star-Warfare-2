using UnityEngine;

internal class CreateRoomResponse : Response
{
	public bool success;

	public short roomID;

	public byte gameMode;

	public byte winCondition;

	public short winValue;

	public byte diffMode;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		success = bytesBuffer.ReadBool();
		if (success)
		{
			roomID = bytesBuffer.ReadShort();
			gameMode = bytesBuffer.ReadByte();
			winCondition = bytesBuffer.ReadByte();
			winValue = bytesBuffer.ReadShort();
			diffMode = bytesBuffer.ReadByte();
		}
	}

	public override void ProcessLogic()
	{
		if (!success || roomID == -1)
		{
			if (VSRoomUIScript.mInstance != null)
			{
				VSRoomUIScript.mInstance.OnCreateRoomFail();
			}
			return;
		}
		Lobby.GetInstance().SetCurrentRoomID(roomID);
		GameApp.GetInstance().GetGameMode().ModePlay = (Mode)gameMode;
		Lobby.GetInstance().IsScoreWinCondition = winCondition;
		Lobby.GetInstance().WinValue = winValue;
		Lobby.GetInstance().IsMasterPlayer = true;
		Lobby.GetInstance().DifficultMode = (EDifficultMode)diffMode;
		Lobby.GetInstance().MasterChannelID = Lobby.GetInstance().GetChannelID();
		Debug.Log("set current room ID as:" + roomID);
		if (GameApp.GetInstance().GetGameMode().IsCoopMode((Mode)gameMode) && VSRoomUIScript.mInstance != null)
		{
			GetRoomDataRequest request = new GetRoomDataRequest(Lobby.GetInstance().GetCurrentRoomID());
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			VSRoomUIScript.mInstance.ChangePage(RoomPage.WaitParnterPage);
		}
	}

	public override void ProcessRobotLogic(UIRobot robot)
	{
		if (roomID == -1)
		{
			if (robot.State == robot.IdleState)
			{
				robot.sendingCreatingRoomRequest = false;
			}
			else
			{
				Debug.LogError("..." + robot.State);
			}
			return;
		}
		robot.lobby.SetCurrentRoomID(roomID);
		robot.lobby.IsMasterPlayer = true;
		robot.sendingCreatingRoomRequest = false;
		robot.SetState(robot.MasterInRoomState);
		RobotRoom robotRoom = new RobotRoom();
		robotRoom.roomID = roomID;
		robotRoom.rrState = RobotRoomState.Idle;
		robot.uiScript.AddRoom(robotRoom);
	}
}
