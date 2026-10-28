public class MakeEnoughDamageToPlayerBossResponse : Response
{
	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
	}

	public override void ProcessLogic()
	{
		Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		if (localPlayer != null)
		{
			localPlayer.VSStatistics.MakeEnoughDamageToPlayerBoss();
			UserStateHUD.InfoBoxHUD infoBox = UserStateHUD.GetInstance().InfoBox;
			localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.TO_PLAYER_BOSS_ENOUGH_DAMAGE);
		}
	}
}
