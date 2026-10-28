using System.Collections.Generic;

public class OperatingInfo : IOperating
{
	public int totalPlayTime;

	public int[,] clearStageTime = new int[Global.TOTAL_STAGE, Global.TOTAL_SUB_STAGE];

	public int soloDeathTimes;

	public List<LevelID> firstDeathLevelID = new List<LevelID>();

	public List<LevelID> recentlyDeathLevelID = new List<LevelID>();

	public int assualtShootCount;

	public int assualtHitCount;

	public List<int> recentlyNormalBattleGetPieceTime = new List<int>();

	public List<int> recentlyNormalBattleGetPieceCount = new List<int>();

	public List<int> recentlyInfinityBattleGetPieceTime = new List<int>();

	public List<int> recentlyInfinityBattleGetPieceCount = new List<int>();

	public OperatingInfo()
	{
		clearStageTime = (clearStageTime = new int[Global.TOTAL_STAGE, Global.TOTAL_SUB_STAGE]);
		soloDeathTimes = 0;
		firstDeathLevelID = new List<LevelID>();
		recentlyDeathLevelID = new List<LevelID>();
		assualtShootCount = 0;
		assualtHitCount = 0;
		recentlyNormalBattleGetPieceTime = new List<int>();
		recentlyNormalBattleGetPieceCount = new List<int>();
		recentlyInfinityBattleGetPieceTime = new List<int>();
		recentlyInfinityBattleGetPieceCount = new List<int>();
	}

	public void WriteToBuffer(BytesBuffer buffer)
	{
	}

	public void SetClearStageTime(LevelID levelID, int time)
	{
		clearStageTime[levelID.StageID, levelID.SubStageID] = time;
	}

	public void SetSoloDeathTimes(int deathTime)
	{
		soloDeathTimes = deathTime;
	}

	public void AddSoloDeathTimes(int deathTime)
	{
		soloDeathTimes += deathTime;
	}

	public void AddFirstDeathLevelID(LevelID levelID)
	{
		if (firstDeathLevelID.Count < 20)
		{
			firstDeathLevelID.Add(levelID);
		}
	}

	public void AddRecentlyDeathLevelID(LevelID levelID)
	{
		recentlyDeathLevelID.Insert(0, levelID);
		while (recentlyDeathLevelID.Count > 20)
		{
			recentlyDeathLevelID.RemoveAt(20);
		}
	}

	public void SetAssualtShootCount(int shoot)
	{
		assualtShootCount = shoot;
	}

	public void AddAssualtShootCount(int shoot)
	{
		assualtShootCount += shoot;
	}

	public void SetAssualtHitCount(int hit)
	{
		assualtHitCount = hit;
	}

	public void AddAssualtHitCount(int hit)
	{
		assualtHitCount += hit;
	}

	public float GetAssualtHitRate()
	{
		return (float)assualtHitCount / (float)assualtShootCount;
	}

	public void AddRecentlyNormalGetPieceTime(int time)
	{
		recentlyNormalBattleGetPieceTime.Insert(0, time);
		while (recentlyNormalBattleGetPieceTime.Count > 10)
		{
			recentlyNormalBattleGetPieceTime.RemoveAt(10);
		}
	}

	public void AddRecentlyNormalGetPieceCount(int count)
	{
		recentlyNormalBattleGetPieceCount.Insert(0, count);
		while (recentlyNormalBattleGetPieceCount.Count > 10)
		{
			recentlyNormalBattleGetPieceCount.RemoveAt(10);
		}
	}

	public void AddRecentlyInfinityGetPieceTime(int time)
	{
		recentlyInfinityBattleGetPieceTime.Insert(0, time);
		while (recentlyInfinityBattleGetPieceTime.Count > 10)
		{
			recentlyInfinityBattleGetPieceTime.RemoveAt(10);
		}
	}

	public void AddRecentlyInfinityGetPieceCount(int count)
	{
		recentlyInfinityBattleGetPieceCount.Insert(0, count);
		while (recentlyInfinityBattleGetPieceCount.Count > 10)
		{
			recentlyInfinityBattleGetPieceCount.RemoveAt(10);
		}
	}
}
