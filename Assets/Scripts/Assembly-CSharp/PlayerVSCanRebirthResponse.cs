internal class PlayerVSCanRebirthResponse : Response
{
	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld != null)
		{
			LocalPlayer localPlayer = gameWorld.GetLocalPlayer();
			if (localPlayer != null)
			{
				localPlayer.SendRebirthRequest();
			}
		}
	}
}
