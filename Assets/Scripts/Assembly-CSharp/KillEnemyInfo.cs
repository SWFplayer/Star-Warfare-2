public class KillEnemyInfo
{
	public EnemyType mEnemyType;

	public int mKillCount;

	public int mKillEliteCount;

	public KillEnemyInfo(EnemyType type)
	{
		mEnemyType = type;
		Reset();
	}

	public void AddKill(bool isElite)
	{
		mKillCount++;
		if (isElite)
		{
			mKillEliteCount++;
		}
	}

	public void Reset()
	{
		mKillCount = 0;
		mKillEliteCount = 0;
	}
}
