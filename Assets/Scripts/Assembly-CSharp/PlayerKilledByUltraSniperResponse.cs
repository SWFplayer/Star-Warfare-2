internal class PlayerKilledByUltraSniperResponse : Response
{
	protected int killerID;

	protected int killedID;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		killedID = bytesBuffer.ReadInt();
		killerID = bytesBuffer.ReadInt();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null || gameWorld.State != GameState.Playing)
		{
			return;
		}
		LocalPlayer localPlayer = gameWorld.GetLocalPlayer();
		if (localPlayer != null)
		{
			Player player = null;
			Player player2 = null;
			int channelID = Lobby.GetInstance().GetChannelID();
			player = ((channelID != killerID) ? ((Player)GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(killerID)) : ((Player)GameApp.GetInstance().GetGameWorld().GetLocalPlayer()));
			player2 = ((channelID != killedID) ? ((Player)GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(killedID)) : ((Player)GameApp.GetInstance().GetGameWorld().GetLocalPlayer()));
			if (player != null && player2 != null)
			{
				player2.UpdateHitBySniper(player);
			}
		}
	}
}
