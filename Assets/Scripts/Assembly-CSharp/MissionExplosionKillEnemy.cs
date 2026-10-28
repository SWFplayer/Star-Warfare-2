public class MissionExplosionKillEnemy : Mission
{
	private int mTargetAmount;

	public override void LoadConfig(int i)
	{
		base.LoadConfig(i);
		GameDataTable dataTable = GetDataTable();
		mTargetAmount = 0;
		for (int j = 0; j < dataTable.Count; j++)
		{
			int num = dataTable.GetInt(j, 0);
			if (num == base.ID)
			{
				mTargetAmount = dataTable.GetInt(j, 1);
			}
		}
	}

	public override void LevelStart()
	{
		base.LevelStart();
		mProcess = EMissionProcess.NORMAL;
		MissionManager.GetInstance().ExplosionKillEnemyAmount = 0;
	}

	public override EMissionProcess CheckIfComplete()
	{
		if (mProcess == EMissionProcess.NORMAL && MissionManager.GetInstance().ExplosionKillEnemyAmount >= mTargetAmount)
		{
			mProcess = EMissionProcess.SUCCESS;
		}
		return mProcess;
	}

	public override float GetProgressPercentage()
	{
		return (float)MissionManager.GetInstance().ExplosionKillEnemyAmount / (float)mTargetAmount;
	}
}
