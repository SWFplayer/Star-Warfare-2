using System;
using UnityEngine;

public class MissionActionBoss : MissionAction
{
	private static int mCounter;

	private EnemyType mBossType = EnemyType.NA;

	private int mTransformId;

	private EnemySpecialPositionScript mPositionScript;

	public EnemyType BossType
	{
		get
		{
			return mBossType;
		}
	}

	public override void LoadConfig()
	{
		GameDataTable dataTable = GetDataTable();
		for (int i = 0; i < dataTable.Count; i++)
		{
			int num = dataTable.GetInt(i, 0);
			if (num == base.ID)
			{
				string text = dataTable.GetString(i, 1);
				if (Enum.IsDefined(typeof(EnemyType), text))
				{
					mBossType = (EnemyType)(int)Enum.Parse(typeof(EnemyType), text, true);
				}
				else
				{
					Debug.LogError("[INVALID BOSS TYPE]: lootTypeString = " + text);
				}
				mTransformId = dataTable.GetInt(i, 2);
				break;
			}
		}
	}

	public override void LevelStart()
	{
		mCounter = 0;
		if (mTransformId == 0)
		{
			return;
		}
		string text = MissionNamePrefix.ACTION_BOSS + mTransformId;
		bool flag = false;
		GameObject[] array = GameObject.FindGameObjectsWithTag(TagName.MISSION_ACTION_BOSS);
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (text == gameObject.name)
			{
				flag = true;
				mPositionScript = gameObject.GetComponent<EnemySpecialPositionScript>();
				if (null == mPositionScript)
				{
					Debug.LogError("[MISSING EnemySpecialPositionScript] ActionID = " + base.ID + ", mTransformId = " + mTransformId);
				}
				break;
			}
		}
		if (!flag)
		{
			Debug.LogError("[NO TRANSFORM FOUND MISSION_ACTION_BOSS] ActionID = " + base.ID + ", mTransformId = " + mTransformId);
		}
	}

	public override void StartAction()
	{
		Enemy enemy = GameApp.GetInstance().GetGameWorld().SpawnEnemy(mBossType, (short)(Global.ENEMY_SPAWN_ID_OFFSET[4] + mCounter), mPositionScript.BornStart.position, false, false, 1, GameApp.GetInstance().GetGameWorld().DifficultyLevel);
		EnemySpecial enemySpecial = enemy as EnemySpecial;
		if (enemySpecial != null)
		{
			enemySpecial.PositionScript = mPositionScript;
			enemySpecial.SetState(EnemySpecial.ENEMY_SPECIAL_BORN_START_STATE);
		}
		MissionManager.GetInstance().GetCurrentStageMission(0).mIsBossAppeared = true;
		mCounter++;
	}
}
