public class MissionInfinite : Mission
{
	public override void LevelStart()
	{
		base.LevelStart();
		mProcess = EMissionProcess.NORMAL;
	}

	public override EMissionProcess CheckIfComplete()
	{
		return mProcess;
	}

	public override string GetProgressText()
	{
		return base.SummaryText + " [e4ce00](" + GetKilledCount() + ")[-]";
	}

	private int GetKilledCount()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld != null)
		{
			if (!GameApp.GetInstance().GetGameMode().IsSingle())
			{
				return gameWorld.TotalEnemyKilled;
			}
			Player localPlayer = gameWorld.GetLocalPlayer();
			if (localPlayer != null)
			{
				return localPlayer.Kills;
			}
		}
		return 0;
	}
}
