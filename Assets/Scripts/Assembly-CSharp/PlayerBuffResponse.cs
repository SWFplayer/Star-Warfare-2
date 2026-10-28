internal class PlayerBuffResponse : Response
{
	private int mChannelId;

	private int mSkillId;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		mChannelId = bytesBuffer.ReadInt();
		mSkillId = bytesBuffer.ReadInt();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null || (gameWorld.State != GameState.Playing && GameApp.GetInstance().GetGameMode().IsVSMode()))
		{
			return;
		}
		Player localPlayer = gameWorld.GetLocalPlayer();
		if (localPlayer != null && mChannelId == localPlayer.GetChannelID())
		{
			localPlayer.TriggerSkillFromOtherPlayer(mSkillId);
			return;
		}
		RemotePlayer remotePlayerByUserID = GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(mChannelId);
		if (remotePlayerByUserID != null)
		{
			remotePlayerByUserID.TriggerSkillFromOtherPlayer(mSkillId);
		}
	}
}
