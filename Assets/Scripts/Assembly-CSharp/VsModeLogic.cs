public abstract class VsModeLogic
{
	protected float[] Scores;

	public bool isTimeRun;

	public virtual void Init()
	{
	}

	public virtual void Update()
	{
	}

	public virtual void SetPointController(byte pointID, TeamName teamName, bool isNeutral = false)
	{
	}

	public virtual void SetScore(int blueScore, int redScore)
	{
		Scores[0] = blueScore;
		Scores[1] = redScore;
	}

	public virtual void ClearAll()
	{
	}

	public virtual void Restart()
	{
		isTimeRun = false;
		Scores[0] = 0f;
		Scores[1] = 0f;
	}

	public virtual void SendBattleInfoToNewPlayer(int newPlayerId)
	{
	}

	public virtual void ChangeMainPointController(byte id, TeamName team, bool neutral, float balance)
	{
	}

	public virtual void SetInfo(float blueScore, float redScore)
	{
	}

	public int GetScore(byte teamID)
	{
		return (int)Scores[teamID];
	}

	public float GetScoreInFloat(byte teamID)
	{
		return Scores[teamID];
	}

	public void StopTime()
	{
		isTimeRun = false;
	}

	public virtual byte GetPointCountForecast(TeamName team, int pointId, TeamName newTeam)
	{
		return 0;
	}
}
