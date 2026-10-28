public class MutantInfoResponse : Response
{
	protected byte infoType;

	protected float timeInfo;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		infoType = bytesBuffer.ReadByte();
		timeInfo = bytesBuffer.ReadFloat();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null || GameApp.GetInstance().GetGameMode().ModePlay != Mode.GIANT_BETRAYAL)
		{
			return;
		}
		VsModeLogic vsBattleLogic = gameWorld.VsBattleLogic;
		if (vsBattleLogic == null)
		{
			return;
		}
		GiantBetrayalModeLogic giantBetrayalModeLogic = vsBattleLogic as GiantBetrayalModeLogic;
		if (giantBetrayalModeLogic != null)
		{
			if (infoType == 0)
			{
				giantBetrayalModeLogic.SetPlayerBossWinTime(timeInfo);
			}
			else if (infoType == 1)
			{
				giantBetrayalModeLogic.SetMatchRemainingTime(timeInfo);
			}
		}
	}
}
