internal class PlayerWaitVSRebirthResponse : Response
{
	private int maxWaitTime;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		maxWaitTime = bytesBuffer.ReadInt();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld != null)
		{
			Player localPlayer = gameWorld.GetLocalPlayer();
			if (localPlayer != null)
			{
				localPlayer.OnWaitVSRebirth(maxWaitTime);
				localPlayer.SetState(Player.WAIT_VS_REBIRTH_STATE);
			}
		}
	}
}
