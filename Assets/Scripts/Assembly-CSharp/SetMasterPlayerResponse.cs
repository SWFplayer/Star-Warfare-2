internal class SetMasterPlayerResponse : Response
{
	public int playerID;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		playerID = bytesBuffer.ReadInt();
	}

	public override void ProcessLogic()
	{
		if (playerID == Lobby.GetInstance().GetChannelID())
		{
			Lobby.GetInstance().IsMasterPlayer = true;
		}
		else
		{
			Lobby.GetInstance().IsMasterPlayer = false;
		}
		Lobby.GetInstance().MasterChannelID = playerID;
	}

	public override void ProcessRobotLogic(UIRobot robot)
	{
		if (playerID == robot.lobby.GetChannelID())
		{
			robot.lobby.IsMasterPlayer = true;
		}
		else
		{
			robot.lobby.IsMasterPlayer = false;
		}
	}
}
