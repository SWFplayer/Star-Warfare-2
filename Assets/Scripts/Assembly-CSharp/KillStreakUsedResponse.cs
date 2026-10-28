internal class KillStreakUsedResponse : Response
{
	private int mChannelId;

	private int mKillStreakId;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		mChannelId = bytesBuffer.ReadInt();
		mKillStreakId = bytesBuffer.ReadInt();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null || gameWorld.State != GameState.Playing)
		{
			return;
		}
		if (GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.GetChannelID() == mChannelId)
		{
			PvpSkill killStreakById = GameApp.GetInstance().GetUserState().GetPvpSkillManager()
				.GetKillStreakById(mKillStreakId);
			if (killStreakById != null)
			{
				UserStateHUD.InfoBoxHUD infoBox = UserStateHUD.GetInstance().InfoBox;
				if (infoBox != null)
				{
					infoBox.SetBroadcasting(LocalizationManager.GetInstance().GetString(killStreakById.GetMessage(), GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
						.GetName()));
				}
			}
			return;
		}
		RemotePlayer remotePlayerByUserID = GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(mChannelId);
		if (remotePlayerByUserID == null)
		{
			return;
		}
		PvpSkill killStreakById2 = GameApp.GetInstance().GetUserState().GetPvpSkillManager()
			.GetKillStreakById(mKillStreakId);
		if (killStreakById2 != null)
		{
			UserStateHUD.InfoBoxHUD infoBox2 = UserStateHUD.GetInstance().InfoBox;
			if (infoBox2 != null)
			{
				infoBox2.SetBroadcasting(LocalizationManager.GetInstance().GetString(killStreakById2.GetMessage(), remotePlayerByUserID.GetName()));
			}
		}
	}
}
