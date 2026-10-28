using UnityEngine;

internal class JoinRoomResponse : Response
{
	protected short roomID;

	protected byte mapID;

	protected byte gameMode;

	protected byte winCondition;

	protected short winValue;

	protected byte autoBalance;

	protected bool isWaiting;

	protected byte seatID;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		roomID = bytesBuffer.ReadShort();
		if (roomID >= 0)
		{
			mapID = bytesBuffer.ReadByte();
			gameMode = bytesBuffer.ReadByte();
			winCondition = bytesBuffer.ReadByte();
			winValue = bytesBuffer.ReadShort();
			autoBalance = bytesBuffer.ReadByte();
			isWaiting = bytesBuffer.ReadBool();
			seatID = bytesBuffer.ReadByte();
		}
	}

	public override void ProcessLogic()
	{
		Debug.Log("JoinRoomResponse, roomID = " + roomID);
		if (roomID >= 0)
		{
			Lobby.GetInstance().IsMasterPlayer = false;
			Lobby.GetInstance().SetCurrentRoomID(roomID);
			Lobby.GetInstance().SetCurrentRoomMapID(mapID);
			GameApp.GetInstance().GetGameMode().ModePlay = (Mode)gameMode;
			Lobby.GetInstance().IsScoreWinCondition = winCondition;
			Lobby.GetInstance().WinValue = winValue;
			Lobby.GetInstance().AutoBalance = autoBalance;
			Lobby.GetInstance().CurrentSeatID = seatID;
			Lobby.GetInstance().GetVSClock().SetTotalGameSeconds(winValue * 60);
			if (GameApp.GetInstance().GetGameMode().IsCoopMode((Mode)gameMode))
			{
				if (isWaiting)
				{
					if (VSRoomUIScript.mInstance != null)
					{
						GetRoomDataRequest request = new GetRoomDataRequest(Lobby.GetInstance().GetCurrentRoomID());
						GameApp.GetInstance().GetNetworkManager().SendRequest(request);
						VSRoomUIScript.mInstance.ChangePage(RoomPage.WaitParnterPage);
					}
				}
				else if (LobbyScript.mInstance != null)
				{
					LobbyScript.mInstance.StartGame();
				}
			}
			else if (LobbyScript.mInstance != null)
			{
				LobbyScript.mInstance.StartGame();
			}
			return;
		}
		Lobby.GetInstance().SetCurrentRoomID(-1);
		if (LobbyScript.mInstance != null)
		{
			if (roomID == -1)
			{
				UIMsgBox.instance.ShowMessage(LobbyScript.mInstance, LocalizationManager.GetInstance().GetString("TIPS_UPDATE11_5"), 1, 13);
			}
			else if (roomID == -2)
			{
				UIMsgBox.instance.ShowMessage(LobbyScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_LOBBY_FRIENDSJOINBOSS"), 1, 13);
			}
		}
	}

	public override void ProcessRobotLogic(UIRobot robot)
	{
		if (roomID != -1)
		{
			robot.SetState(robot.InRoomState);
			robot.lobby.SetCurrentRoomID(roomID);
			Debug.Log("Robot " + robot.userName + " Joined Room: " + roomID);
		}
		else if (robot.State == robot.IdleState)
		{
			robot.sendingJoingRequest = false;
		}
		else
		{
			Debug.LogError("..." + robot.State);
		}
	}
}
