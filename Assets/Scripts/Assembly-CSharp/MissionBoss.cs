using System;
using System.Collections.Generic;
using UnityEngine;

public class MissionBoss : Mission
{
	private EMissionBossType mBossType;

	private List<EnemyBoss> mBossList = new List<EnemyBoss>();

	public override void LoadConfig(int i)
	{
		base.LoadConfig(i);
		GameDataTable dataTable = GetDataTable();
		mBossType = EMissionBossType.NONE;
		for (int j = 0; j < dataTable.Count; j++)
		{
			int num = dataTable.GetInt(j, 0);
			if (num == base.ID)
			{
				string text = dataTable.GetString(j, 1);
				mBossType = EMissionBossType.NONE;
				if (Enum.IsDefined(typeof(EMissionBossType), text))
				{
					mBossType = (EMissionBossType)(int)Enum.Parse(typeof(EMissionBossType), text, true);
				}
				else
				{
					Debug.LogError("Invalid Boss Type: " + text);
				}
				break;
			}
		}
	}

	public override void LevelStart()
	{
		base.LevelStart();
		mBossList.Clear();
		GameApp.GetInstance().GetGameWorld().DisableNavMeshObstacle();
		mProcess = EMissionProcess.NORMAL;
		GameApp.GetInstance().GetGameWorld().BossTotalHp = 0;
		GameApp.GetInstance().GetGameWorld().BossTotalDamage = 0;
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && GameApp.GetInstance().GetUserState().Boss1Type != EnemyType.NA)
		{
			BossCoop bossCoop = new BossCoop();
			bossCoop.Init();
			EnemyBoss enemyBoss = GameWorld.SpawnBoss(GameApp.GetInstance().GetUserState().Boss1Type, 99, bossCoop.mBoss1Position);
			EnemyBoss enemyBoss2 = GameWorld.SpawnBoss(GameApp.GetInstance().GetUserState().Boss2Type, 100, bossCoop.mBoss2Position);
			bossCoop.SetBoss(enemyBoss, enemyBoss2);
			GameApp.GetInstance().GetGameWorld().SetBoosCoop(bossCoop);
			mBossList.Add(enemyBoss);
			mBossList.Add(enemyBoss2);
			GameApp.GetInstance().GetGameWorld().BossTotalHp = enemyBoss.MaxHp + enemyBoss2.MaxHp;
			return;
		}
		GameObject[] array = GameObject.FindGameObjectsWithTag(TagName.BOSS_SPAWN_POINT);
		List<string> list = new List<string>();
		EnemyBoss enemyBoss3 = null;
		switch (mBossType)
		{
		case EMissionBossType.SLASHER:
			enemyBoss3 = new Slasher();
			list.Add("Enemy/" + Global.QUALITY_LEVEL_1 + "/Boss/Slasher");
			break;
		case EMissionBossType.DEADEYE:
			enemyBoss3 = new DeadEye();
			list.Add("Enemy/" + Global.QUALITY_LEVEL_1 + "/Boss/DeadEye");
			break;
		case EMissionBossType.FROSTSTONE:
			enemyBoss3 = new FrostStone();
			list.Add("Enemy/" + Global.QUALITY_LEVEL_1 + "/Boss/FrostStone");
			break;
		case EMissionBossType.CERBERLISK:
			enemyBoss3 = new Cerberlisk();
			list.Add("Enemy/" + Global.QUALITY_LEVEL_1 + "/Boss/Cerberlisk");
			break;
		case EMissionBossType.CRIMSONQUEEN:
			enemyBoss3 = new CrimsonQueen();
			list.Add("Enemy/" + Global.QUALITY_LEVEL_1 + "/Boss/CrimsonQueen");
			break;
		}
		mBossList.Add(enemyBoss3);
		GameApp.GetInstance().GetUserState().CoopBossType = enemyBoss3.EnemyType;
		for (int i = 0; i < mBossList.Count; i++)
		{
			if (array.Length > i)
			{
				GameObject gameObject = Resources.Load(list[i]) as GameObject;
				if (null != gameObject)
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, array[i].transform.position, array[i].transform.rotation) as GameObject;
					mBossList[i].Init(gameObject2);
					mBossList[i].EnemyID = (short)(Global.ENEMY_SPAWN_ID_OFFSET[5] + i);
					gameObject2.name = "E_" + mBossList[i].EnemyID;
					GameApp.GetInstance().GetGameWorld().AddEnemy(gameObject2.name, mBossList[i]);
					GameApp.GetInstance().GetGameWorld().BossTotalHp += mBossList[i].MaxHp;
					if (enemyBoss3.EnemyType == EnemyType.SLASHER)
					{
						mBossList[i].SetState(EnemyBoss.BOSS_INIT_STATE);
					}
					else if (enemyBoss3.EnemyType == EnemyType.DEAD_EYE)
					{
						mBossList[i].SetState(DeadEye.DEAD_EYE_BORN_STATE);
					}
					else if (enemyBoss3.EnemyType == EnemyType.FROST_STONE)
					{
						mBossList[i].SetState(FrostStone.FROST_STONE_BORN_STATE);
					}
					else if (enemyBoss3.EnemyType == EnemyType.CERBERLISK)
					{
						mBossList[i].SetState(Cerberlisk.CERBERLISK_INIT_STATE);
					}
					else if (enemyBoss3.EnemyType == EnemyType.CRIMSON_QUEEN)
					{
						mBossList[i].SetState(CrimsonQueen.CRIMSON_QUEEN_BORN_STATE);
					}
					if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
					{
						EnemySpawnRequest request = new EnemySpawnRequest(mBossList[i].EnemyType, EnemySpawnType.BOSS, array[i].transform.position, 0, mBossList[i].EnemyID, false, false);
						GameApp.GetInstance().GetNetworkManager().SendRequest(request);
					}
				}
				else
				{
					Debug.LogError("Failed to load resource: " + list[i]);
				}
			}
			else
			{
				Debug.LogError("Missing boss spawn points");
			}
		}
	}

	public override EMissionProcess CheckIfComplete()
	{
		if (mProcess == EMissionProcess.NORMAL && GameApp.GetInstance().GetGameWorld().GetLocalPlayer() != null)
		{
			bool flag = true;
			foreach (EnemyBoss mBoss in mBossList)
			{
				if (!mBoss.IsDead)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				mProcess = EMissionProcess.SUCCESS;
				GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
				LocalPlayer localPlayer = gameWorld.GetLocalPlayer();
				if (mBossType == EMissionBossType.SLASHER && !localPlayer.mGetSlasherWaveHited)
				{
					GameApp.GetInstance().GetUserState().Achievement.CheckAchievement_DodgeSlaserWave();
				}
				if (mBossType == EMissionBossType.FROSTSTONE && localPlayer.IceSpikeDestroy >= Global.ACHIEVEMENT_SPIKES)
				{
					GameApp.GetInstance().GetUserState().Achievement.CheckAchievement_50Spikes();
				}
				if (mBossType == EMissionBossType.CERBERLISK && !localPlayer.mGetFireDamage)
				{
					GameApp.GetInstance().GetUserState().Achievement.CheckAchievement_NoFireDamage();
				}
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
				{
					int num = gameWorld.OnlineBossGold;
					if (gameWorld.PlayerCountForBoss > 1 && gameWorld.BossTotalHp > 0)
					{
						num = (int)((float)num * (0.5f + 0.5f * (float)gameWorld.PlayerCountForBoss * (float)gameWorld.BossTotalDamage / (float)gameWorld.BossTotalHp));
					}
					localPlayer.AddBossCash(num);
					localPlayer.SetBossMithril(gameWorld.OnlineBossMithril);
				}
			}
		}
		return mProcess;
	}

	public override float GetProgressPercentage()
	{
		if (mBossList.Count > 0)
		{
			return (float)mBossList[0].Hp / (float)mBossList[0].MaxHp;
		}
		return 0f;
	}

	public float GetBoss2ProgressPercentage()
	{
		if (mBossList.Count > 1)
		{
			return (float)mBossList[1].Hp / (float)mBossList[1].MaxHp;
		}
		return 0f;
	}

	public List<EnemyBoss> GetBossList()
	{
		return mBossList;
	}
}
