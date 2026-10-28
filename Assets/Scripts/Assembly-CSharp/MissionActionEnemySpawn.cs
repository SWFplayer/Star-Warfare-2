using System;
using System.Collections.Generic;
using UnityEngine;

public class MissionActionEnemySpawn : MissionAction
{
	private SpawnConfig mSpawnConfig;

	public override void LoadConfig()
	{
		Dictionary<int, Dictionary<byte, SpawnConfig>> dictionary = new Dictionary<int, Dictionary<byte, SpawnConfig>>();
		GameDataTable dataTable = GetDataTable();
		mSpawnConfig = new SpawnConfig();
		mSpawnConfig.Rounds = new List<Round>();
		for (int i = 0; i < dataTable.Count; i++)
		{
			int num = dataTable.GetInt(i, 0);
			if (num != base.ID)
			{
				continue;
			}
			Round round = new Round();
			round.intermission = dataTable.GetFloat(i, 1);
			round.EnemyInfos = new List<EnemySpawnInfo>();
			SpawnFromType spawnFromType = SpawnFromType.GROUND;
			string text = dataTable.GetString(i, 8);
			if (Enum.IsDefined(typeof(SpawnFromType), text))
			{
				spawnFromType = (SpawnFromType)(int)Enum.Parse(typeof(SpawnFromType), text, true);
			}
			else
			{
				Debug.LogError("[INVALID SPAWN FROM TYPE]: ID = " + base.ID + ", row = " + i + ", value = " + text);
			}
			int eliteProbability = dataTable.GetInt(i, 9);
			for (int j = 0; j < 3; j++)
			{
				EnemySpawnInfo enemySpawnInfo = new EnemySpawnInfo();
				string text2 = dataTable.GetString(i, 2 + j * 2);
				if (Enum.IsDefined(typeof(EnemyType), text2))
				{
					enemySpawnInfo.EType = (EnemyType)(int)Enum.Parse(typeof(EnemyType), text2, true);
				}
				else
				{
					Debug.LogError("[INVALID ENEMY TYPE]: ID = " + base.ID + ", row = " + i + ", col = " + j + ", value = " + text2);
				}
				enemySpawnInfo.Count = dataTable.GetInt(i, 3 + j * 2);
				enemySpawnInfo.From = spawnFromType;
				enemySpawnInfo.EliteProbability = eliteProbability;
				round.EnemyInfos.Add(enemySpawnInfo);
			}
			mSpawnConfig.Rounds.Add(round);
		}
	}

	public override void StartAction()
	{
		EnemySpawnScript.Instance.SpawnEventEnemy(mSpawnConfig);
	}
}
