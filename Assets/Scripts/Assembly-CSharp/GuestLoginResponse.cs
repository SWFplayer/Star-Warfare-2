using UnityEngine;

public class GuestLoginResponse : Response
{
	private int channelID;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		channelID = bytesBuffer.ReadInt();
	}

	public override void ProcessLogic()
	{
		TimeManager.GetInstance().LastSynTime = Time.time;
		Lobby.GetInstance().SetChannelID(channelID);
		if (MainMenuScript.mInstance != null)
		{
			MainMenuScript.mInstance.GotoMultiMenu();
			MainMenuScript.mInstance.CorrectLogin();
		}
		else if (MainMenu2DScript.mInstance != null)
		{
			MainMenu2DScript.mInstance.GotoMultiMenu();
			MainMenu2DScript.mInstance.CorrectLogin();
		}
		else if (!(LobbyScript.mInstance != null))
		{
		}
	}

	public override void ProcessRobotLogic(UIRobot robot)
	{
	}
}
