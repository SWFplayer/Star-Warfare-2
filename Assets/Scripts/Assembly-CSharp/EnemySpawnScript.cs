using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnScript : MonoBehaviour
{
	public static EnemySpawnScript Instance;

	protected Player player;

	protected int waveNum;

	protected SpawnParam mSpawnParam = new SpawnParam();

	protected GameWorld gameWorld;

	protected List<GameObject> mDoorList;

	protected List<GameObject> mGraveList;

	protected float lastUpdateTime = -1000f;

	protected int currentSpawnIndex;

	protected int currentDoorIndex;

	protected int doorCount;

	protected bool levelClear;

	private SpawnConfig spawnConfigInfo;

	protected EnemyType bossType;

	private int playerIndex;

	private int mProtectItemIndex;

	public bool survival;

	protected List<EnemySpawnItem> spawnItems;

	protected List<EnemySpawnItem> mergeSpawnItems;

	private List<EnemySpawnItem> mEventSpawnItems = new List<EnemySpawnItem>();

	private List<EnemySpawnItem> mEventMergeSpawnItems = new List<EnemySpawnItem>();

	private int mEventTotalCount;

	private int mEventMergeTotalCount;

	private Ray mRay = default(Ray);

	private RaycastHit mRaycastHit = default(RaycastHit);

	public SpawnParam SpawnParameters
	{
		get
		{
			return mSpawnParam;
		}
		set
		{
			if (mSpawnParam == null)
			{
				mSpawnParam = new SpawnParam();
			}
			if (value.MaxEnemyCount >= 0)
			{
				mSpawnParam.MaxEnemyCount = value.MaxEnemyCount;
			}
			if (value.MaxMergeEnemyCount >= 0)
			{
				mSpawnParam.MaxMergeEnemyCount = value.MaxMergeEnemyCount;
			}
			if (value.SpawnInterval >= 0f)
			{
				mSpawnParam.SpawnInterval = value.SpawnInterval;
			}
			if (value.MoveSpawnDistance >= 0f)
			{
				mSpawnParam.MoveSpawnDistance = value.MoveSpawnDistance;
			}
			if (value.StopSpawnDistance >= 0f)
			{
				mSpawnParam.StopSpawnDistance = value.StopSpawnDistance;
			}
			if (value.ActionSpawnRadiusNear1 >= 0f)
			{
				mSpawnParam.ActionSpawnRadiusNear1 = value.ActionSpawnRadiusNear1;
			}
			if (value.ActionSpawnRadiusNear2 >= 0f)
			{
				mSpawnParam.ActionSpawnRadiusNear2 = value.ActionSpawnRadiusNear2;
			}
			if (value.EnemyAcceleration >= 0f)
			{
				mSpawnParam.EnemyAcceleration = value.EnemyAcceleration;
			}
			if (value.MaxEnemySpeed >= 0f)
			{
				mSpawnParam.MaxEnemySpeed = value.MaxEnemySpeed;
			}
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	public static bool CanMerge(EnemyType type)
	{
		return false;
	}

	private Player GetNextPlayer()
	{
		List<RemotePlayer> remotePlayers = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
		int count = remotePlayers.Count;
		Player result = gameWorld.GetLocalPlayer();
		if (playerIndex < count)
		{
			result = remotePlayers[playerIndex];
		}
		if (playerIndex > count)
		{
			playerIndex = 0;
		}
		else
		{
			playerIndex++;
		}
		return result;
	}

	private GameUnit GetNextProtectItem()
	{
		List<ProtectedItem> protectedItemList = GameApp.GetInstance().GetGameWorld().GetProtectedItemList();
		if (mProtectItemIndex >= protectedItemList.Count)
		{
			mProtectItemIndex = 0;
		}
		if (mProtectItemIndex < protectedItemList.Count)
		{
			return protectedItemList[mProtectItemIndex++];
		}
		Debug.LogError("No Protected Item to spawn enemy.");
		return null;
	}

	private IEnumerator Start()
	{
		if (Instance != null && Instance != this)
		{
			Debug.LogError("Duplicate EnemySpawnScript!!");
			yield return new WaitForSeconds(36000000f);
		}
		while (!InGameUIScript.bInited)
		{
			yield return 0;
		}
		if (GameApp.GetInstance().GetGameMode().IsCoopMode() && Lobby.GetInstance().IsMasterPlayer)
		{
			float waitDuration = 0f;
			while (!GameWorld.sCanSpawnEnemy && waitDuration < 20f)
			{
				waitDuration += 0.5f;
				yield return new WaitForSeconds(0.5f);
			}
		}
		if (GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			yield break;
		}
		for (int i = 0; i < 10; i++)
		{
			yield return 0;
		}
		int stageId = GameApp.GetInstance().GetUserState().GetSelectedStageIndex();
		int subStageId = GameApp.GetInstance().GetUserState().GetSelectedSubStageIndex();
		int netStageId = GameApp.GetInstance().GetUserState().GetSelectedNetIndex();
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			stageId = netStageId;
			if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Survival)
			{
				subStageId = (byte)Global.SURVIVAL_SUB_STAGE_ID;
			}
			else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Boss)
			{
				subStageId = (byte)Global.BOSS_SUB_STAGE_ID;
			}
			else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.BOSS_FINAL)
			{
				subStageId = (byte)Global.BOSS_FINAL_SUB_STAGE_ID;
			}
		}
		if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Survival)
		{
			survival = true;
		}
		int syncDifficultyLevel = GameApp.GetInstance().GetGameWorld().DifficultyLevel;
		GameApp.GetInstance().GetGameWorld().DifficultyLevel = ((GameApp.GetInstance().GetUserState().GetSelectedDifficultMode() != EDifficultMode.HEROIC) ? (stageId * 3) : ((stageId + 4) * 3));
		if (!survival)
		{
			GameApp.GetInstance().GetGameWorld().DifficultyLevel += subStageId / 2;
		}
		int baseDifficultyLevel = GameApp.GetInstance().GetGameWorld().DifficultyLevel;
		if (GameApp.GetInstance().GetGameWorld().EnemyID != 0)
		{
			GameApp.GetInstance().GetGameWorld().DifficultyLevel = syncDifficultyLevel;
		}
		gameWorld = GameApp.GetInstance().GetGameWorld();
		LevelConfig levelConfig = GameApp.GetInstance().GetUserState().GetCurrentLevelConfig();
		string doorName = "EnemySpawnPoint_" + levelConfig.enemySpawnDoorId;
		bool doorFound = false;
		GameObject[] doorPoints = GameObject.FindGameObjectsWithTag(TagName.ENEMY_SPAWN_POINT);
		mDoorList = new List<GameObject>();
		GameObject[] array = doorPoints;
		foreach (GameObject p in array)
		{
			if (doorName == p.name)
			{
				doorFound = true;
				for (int k = 0; k < p.transform.childCount; k++)
				{
					mDoorList.Add(p.transform.GetChild(k).gameObject);
				}
				break;
			}
		}
		if (!doorFound)
		{
			Debug.LogError("[NO ENEMY SPAWN DOOR FOUND] EnemySpawnDoorId = " + levelConfig.enemySpawnDoorId);
		}
		doorCount = mDoorList.Count;
		string graveName = "Grave_" + levelConfig.enemySpawnGraveId;
		bool graveFound = false;
		GameObject[] gravePoints = GameObject.FindGameObjectsWithTag(TagName.GRAVE);
		mGraveList = new List<GameObject>();
		GameObject[] array2 = gravePoints;
		foreach (GameObject p2 in array2)
		{
			if (graveName == p2.name)
			{
				graveFound = true;
				for (int m = 0; m < p2.transform.childCount; m++)
				{
					mGraveList.Add(p2.transform.GetChild(m).gameObject);
				}
				break;
			}
		}
		if (!graveFound)
		{
			Debug.LogError("[NO ENEMY SPAWN GRAVE FOUND] EnemySpawnGraveId = " + levelConfig.enemySpawnGraveId);
		}
		player = gameWorld.GetLocalPlayer();
		spawnConfigInfo = new SpawnConfig();
		spawnConfigInfo.Rounds = new List<Round>();
		string sheetName = stageId + 1 + "-" + subStageId;
		GameDataTable dataTable = GameDataManager.GetInstance().GetGameDataTable((GameApp.GetInstance().GetUserState().GetSelectedDifficultMode() != EDifficultMode.HEROIC) ? DataFileName.ENEMY_SPAWN : DataFileName.ENEMY_SPAWN_HARD, sheetName);
		for (int n = 0; n < dataTable.Count; n++)
		{
			Round newRound = new Round
			{
				intermission = dataTable.GetFloat(n, 0),
				EnemyInfos = new List<EnemySpawnInfo>()
			};
			for (int num = 0; num < 3; num++)
			{
				EnemySpawnInfo info = new EnemySpawnInfo();
				string enemyType = dataTable.GetString(n, 1 + num * 2);
				if (Enum.IsDefined(typeof(EnemyType), enemyType))
				{
					info.EType = (EnemyType)(int)Enum.Parse(typeof(EnemyType), enemyType, true);
				}
				else if (enemyType == "ZERGLING")
				{
					info.EType = EnemyType.STALKER;
				}
				else
				{
					Debug.LogError("[INVALID ENEMY TYPE]: row = " + n + ", col = " + num + ", value = " + enemyType);
				}
				string fromType = dataTable.GetString(n, 7);
				if (Enum.IsDefined(typeof(SpawnFromType), fromType))
				{
					info.From = (SpawnFromType)(int)Enum.Parse(typeof(SpawnFromType), fromType, true);
				}
				else
				{
					Debug.LogError("[INVALID SPAWN FROM TYPE]: row = " + n + ", col = " + num + ", value = " + fromType);
				}
				info.Count = dataTable.GetInt(n, 2 + num * 2);
				if (info.EType == EnemyType.NA && info.Count > 0)
				{
					info.Count = 0;
				}
				newRound.EnemyInfos.Add(info);
			}
			spawnConfigInfo.Rounds.Add(newRound);
		}
		if (!survival)
		{
			GameApp.GetInstance().GetGameWorld().TotalWaves = spawnConfigInfo.Rounds.Count;
		}
		else
		{
			GameApp.GetInstance().GetGameWorld().TotalEnemyCount = 999999;
		}
		spawnItems = new List<EnemySpawnItem>();
		mergeSpawnItems = new List<EnemySpawnItem>();
		int totalRound = 0;
		int roundShift = ((GameApp.GetInstance().GetGameMode().IsSingle() && survival) ? UnityEngine.Random.Range(0, spawnConfigInfo.Rounds.Count) : 0);
		for (int r = 0; r < spawnConfigInfo.Rounds.Count; r++)
		{
			Round round = spawnConfigInfo.Rounds[(r + roundShift) % spawnConfigInfo.Rounds.Count];
			for (int num2 = 0; num2 < round.EnemyInfos.Count; num2++)
			{
				EnemySpawnInfo enemyInfo = round.EnemyInfos[num2];
				for (int num3 = 0; num3 < enemyInfo.Count; num3++)
				{
					EnemySpawnItem esItem = new EnemySpawnItem
					{
						Mission = SpawnMissionType.NORMAL,
						EType = enemyInfo.EType,
						From = enemyInfo.From,
						EliteProbability = enemyInfo.EliteProbability,
						Round = r,
						intermission = 0f
					};
					if (num2 == 0 && num3 == 0)
					{
						esItem.intermission = round.intermission;
					}
					if (CanMerge(enemyInfo.EType))
					{
						esItem.CanMerge = true;
						mergeSpawnItems.Add(esItem);
					}
					else
					{
						esItem.CanMerge = false;
						spawnItems.Add(esItem);
					}
				}
			}
			totalRound++;
		}
		GameDataTable spawnParamTable = GameDataManager.GetInstance().GetGameDataTable((GameApp.GetInstance().GetUserState().GetSelectedDifficultMode() != EDifficultMode.HEROIC) ? DataFileName.ENEMY_SPAWN : DataFileName.ENEMY_SPAWN_HARD, DataSheetName.ENEMY_SPAWN_PARAM);
		for (int num4 = 0; num4 < spawnParamTable.Count; num4++)
		{
			if (sheetName.Equals(spawnParamTable.GetString(num4, 0)))
			{
				mSpawnParam.MaxEnemyCount = spawnParamTable.GetInt(num4, 1);
				mSpawnParam.MaxMergeEnemyCount = spawnParamTable.GetInt(num4, 2);
				mSpawnParam.SpawnInterval = spawnParamTable.GetFloat(num4, 3);
				mSpawnParam.MoveSpawnDistance = spawnParamTable.GetFloat(num4, 4);
				mSpawnParam.StopSpawnDistance = spawnParamTable.GetFloat(num4, 5);
				mSpawnParam.ActionSpawnRadiusNear1 = spawnParamTable.GetFloat(num4, 6);
				mSpawnParam.ActionSpawnRadiusNear2 = spawnParamTable.GetFloat(num4, 7);
				mSpawnParam.EnemyAcceleration = spawnParamTable.GetFloat(num4, 8);
				mSpawnParam.MaxEnemySpeed = spawnParamTable.GetFloat(num4, 9);
				break;
			}
		}
		int repeatIndex = 0;
		int mergeRepeatIndex = 0;
		int enemyLeft = 0;
		int mergeEnemyLeft = 0;
		int difficultyUpTimes = 0;
		int totalCount = spawnItems.Count;
		int totalMergeCount = mergeSpawnItems.Count;
		GameApp.GetInstance().GetGameWorld().TotalEnemyCount = totalCount;
		if (GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			yield return new WaitForSeconds(5f);
		}
		while (true && totalCount > 0)
		{
			if (Lobby.GetInstance().IsMasterPlayer || GameApp.GetInstance().GetGameMode().IsSingle())
			{
				while (!GameApp.GetInstance().GetGameWorld().CanSpawnEnemy())
				{
					yield return new WaitForSeconds(1f);
				}
				int enemyID = GameApp.GetInstance().GetGameWorld().EnemyID;
				int mergeEnemyID = GameApp.GetInstance().GetGameWorld().MergeEnemyID;
				int eventEnemyID = GameApp.GetInstance().GetGameWorld().EventEnemyID;
				int eventMergeEnemyID = GameApp.GetInstance().GetGameWorld().EventMergeEnemyID;
				int indexID = enemyID;
				int mergeIndexID = mergeEnemyID;
				difficultyUpTimes = (indexID + mergeEnemyID) / Global.SURVIVAL_DIFFICULTY_UP_ENEMY_NUM;
				if (survival)
				{
					indexID = enemyID % totalCount;
					repeatIndex = enemyID / totalCount;
					if (totalMergeCount > 0)
					{
						mergeIndexID = mergeEnemyID % totalMergeCount;
						mergeRepeatIndex = mergeEnemyID / totalMergeCount;
					}
					else
					{
						mergeIndexID = 0;
						mergeRepeatIndex = 0;
					}
				}
				else
				{
					indexID = enemyID % totalCount;
					mergeIndexID = ((totalMergeCount > 0) ? (mergeEnemyID % totalMergeCount) : 0);
				}
				if (indexID < totalCount)
				{
					while (MissionManager.GetInstance().MainMissionFinished)
					{
						yield return new WaitForSeconds(1f);
					}
					bool canSpawn = true;
					bool canSpawnNormalEnemy = false;
					bool canSpawnMergeEnemy = false;
					enemyLeft = GameApp.GetInstance().GetGameWorld().GetNormalEnemiesCount();
					mergeEnemyLeft = GameApp.GetInstance().GetGameWorld().GetMergeEnemiesCount();
					if (enemyLeft >= mSpawnParam.MaxEnemyCount && mergeEnemyLeft >= mSpawnParam.MaxMergeEnemyCount)
					{
						canSpawn = false;
					}
					else
					{
						if (mergeEnemyLeft < mSpawnParam.MaxMergeEnemyCount)
						{
							canSpawnMergeEnemy = true;
							int currentRound = spawnItems[indexID].Round;
							if (indexID > 0)
							{
								currentRound = spawnItems[indexID - 1].Round;
							}
							if (mergeSpawnItems.Count <= 0)
							{
								canSpawn = false;
								canSpawnMergeEnemy = false;
							}
							else if (mergeSpawnItems[mergeIndexID].Round > currentRound && enemyLeft >= mSpawnParam.MaxEnemyCount)
							{
								canSpawn = false;
								canSpawnMergeEnemy = false;
							}
						}
						if (enemyLeft < mSpawnParam.MaxEnemyCount)
						{
							canSpawn = true;
							canSpawnNormalEnemy = true;
						}
					}
					while (!canSpawn)
					{
						yield return new WaitForSeconds(mSpawnParam.SpawnInterval);
						canSpawn = true;
						canSpawnNormalEnemy = false;
						canSpawnMergeEnemy = false;
						enemyLeft = GameApp.GetInstance().GetGameWorld().GetNormalEnemiesCount();
						mergeEnemyLeft = GameApp.GetInstance().GetGameWorld().GetMergeEnemiesCount();
						if (enemyLeft >= mSpawnParam.MaxEnemyCount && mergeEnemyLeft >= mSpawnParam.MaxMergeEnemyCount)
						{
							canSpawn = false;
							continue;
						}
						if (mergeEnemyLeft < mSpawnParam.MaxMergeEnemyCount)
						{
							canSpawnMergeEnemy = true;
							int currentRound2 = spawnItems[indexID].Round;
							if (indexID > 0)
							{
								currentRound2 = spawnItems[indexID - 1].Round;
							}
							if (mergeSpawnItems.Count <= 0)
							{
								canSpawn = false;
								canSpawnMergeEnemy = false;
							}
							else if (mergeSpawnItems[mergeIndexID].Round > currentRound2 && enemyLeft >= mSpawnParam.MaxEnemyCount)
							{
								canSpawn = false;
								canSpawnMergeEnemy = false;
							}
						}
						if (enemyLeft < mSpawnParam.MaxEnemyCount)
						{
							canSpawn = true;
							canSpawnNormalEnemy = true;
						}
					}
					GameObject enemyPrefab = null;
					Transform grave = null;
					Vector3 predictPos = Vector3.zero;
					EnemySpawnItem[] spawnItemArray = new EnemySpawnItem[2];
					if (canSpawnNormalEnemy)
					{
						spawnItemArray[0] = spawnItems[indexID];
					}
					if (canSpawnMergeEnemy)
					{
						spawnItemArray[1] = mergeSpawnItems[mergeIndexID];
					}
					if (eventEnemyID < mEventTotalCount)
					{
						spawnItemArray[0] = mEventSpawnItems[eventEnemyID];
					}
					if (eventMergeEnemyID < mEventMergeTotalCount)
					{
						spawnItemArray[1] = mEventMergeSpawnItems[eventMergeEnemyID];
					}
					EnemySpawnItem[] array3 = spawnItemArray;
					foreach (EnemySpawnItem spawnItem in array3)
					{
						if (spawnItem == null)
						{
							continue;
						}
						if (survival)
						{
							GameApp.GetInstance().GetGameWorld().DifficultyLevel = baseDifficultyLevel + difficultyUpTimes;
						}
						EnemyType enemyType2 = spawnItem.EType;
						if (spawnItem.intermission > 0f)
						{
							yield return new WaitForSeconds(spawnItem.intermission);
						}
						else
						{
							yield return new WaitForSeconds(mSpawnParam.SpawnInterval);
						}
						SpawnFromType from = spawnItem.From;
						switch (from)
						{
						case SpawnFromType.GROUND:
						{
							Player nextPlayer = GetNextPlayer();
							predictPos = CalculatePredictPosition(nextPlayer, spawnItem.EType);
							if (predictPos == Vector3.zero)
							{
								grave = CalculateGravePosition(nextPlayer.GetTransform());
							}
							break;
						}
						case SpawnFromType.NEAR1:
						case SpawnFromType.NEAR2:
						{
							Player nextPlayer2 = GetNextPlayer();
							switch (from)
							{
							case SpawnFromType.NEAR1:
								predictPos = CalculateEventSpawnPosition(nextPlayer2.GetPosition(), mSpawnParam.ActionSpawnRadiusNear1, spawnItem.EType);
								break;
							case SpawnFromType.NEAR2:
								predictPos = CalculateEventSpawnPosition(nextPlayer2.GetPosition(), mSpawnParam.ActionSpawnRadiusNear2, spawnItem.EType);
								break;
							}
							if (predictPos == Vector3.zero)
							{
								grave = CalculateGravePosition(nextPlayer2.GetTransform());
							}
							break;
						}
						case SpawnFromType.NEAR_PROTECT:
						{
							GameUnit nextItem = GetNextProtectItem();
							predictPos = CalculateEventSpawnPosition(nextItem.GetPosition(), 6f, spawnItem.EType);
							if (predictPos == Vector3.zero)
							{
								grave = CalculateGravePosition(nextItem.GetTransform());
							}
							break;
						}
						}
						Vector3 spawnPosition = Vector3.zero;
						switch (from)
						{
						case SpawnFromType.POINT:
							spawnPosition = mDoorList[currentDoorIndex].transform.position;
							currentDoorIndex++;
							if (currentDoorIndex == doorCount)
							{
								currentDoorIndex = 0;
							}
							break;
						case SpawnFromType.GROUND:
						case SpawnFromType.NEAR1:
						case SpawnFromType.NEAR2:
						case SpawnFromType.NEAR_PROTECT:
							if (predictPos == Vector3.zero)
							{
								float rndX = UnityEngine.Random.Range((0f - grave.localScale.x) / 2f, grave.localScale.x / 2f);
								float rndZ = UnityEngine.Random.Range((0f - grave.localScale.z) / 2f, grave.localScale.z / 2f);
								spawnPosition = grave.position + new Vector3(rndX, 0f, rndZ);
							}
							else
							{
								spawnPosition = predictPos;
							}
							break;
						}
						Enemy enemy = null;
						bool newPlayer = false;
						if (stageId == 0 && subStageId > 0 && subStageId < 5)
						{
							newPlayer = true;
						}
						bool bElite = false;
						if (!newPlayer)
						{
							bElite = EliteSpawn(enemyType2, spawnItem.EliteProbability, 0, 0);
						}
						EnemySpawnType spawnType = EnemySpawnType.NORMAL;
						short spawnId = (short)(enemyID + Global.ENEMY_SPAWN_ID_OFFSET[(int)spawnType]);
						if (spawnItem.Mission == SpawnMissionType.NORMAL)
						{
							if (spawnItem.CanMerge)
							{
								spawnType = EnemySpawnType.MERGE;
								spawnId = (short)mergeEnemyID;
							}
							else
							{
								spawnType = EnemySpawnType.NORMAL;
								spawnId = (short)enemyID;
							}
						}
						else if (spawnItem.CanMerge)
						{
							spawnType = EnemySpawnType.EVENT_MERGE;
							spawnId = (short)eventMergeEnemyID;
						}
						else
						{
							spawnType = EnemySpawnType.EVENT_NORMAL;
							spawnId = (short)eventEnemyID;
						}
						spawnId = (short)(spawnId + Global.ENEMY_SPAWN_ID_OFFSET[(int)spawnType]);
						if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
						{
							if (Lobby.GetInstance().IsMasterPlayer && gameWorld.State == GameState.Playing)
							{
								EnemySpawnRequest esRequest = new EnemySpawnRequest(enemyType2, spawnType, spawnPosition, GameApp.GetInstance().GetGameWorld().DifficultyLevel, spawnId, bElite, from != SpawnFromType.POINT);
								GameApp.GetInstance().GetNetworkManager().SendRequest(esRequest);
							}
						}
						else if (GameApp.GetInstance().GetGameMode().IsSingle())
						{
							GameApp.GetInstance().GetGameWorld().SpawnEnemy(enemyType2, spawnId, spawnPosition, bElite, from != SpawnFromType.POINT, 1, GameApp.GetInstance().GetGameWorld().DifficultyLevel);
						}
						if (spawnItem.Mission == SpawnMissionType.NORMAL)
						{
							if (spawnItem.CanMerge)
							{
								GameApp.GetInstance().GetGameWorld().MergeEnemyID = GameApp.GetInstance().GetGameWorld().MergeEnemyID + 1;
							}
							else
							{
								GameApp.GetInstance().GetGameWorld().EnemyID = GameApp.GetInstance().GetGameWorld().EnemyID + 1;
							}
						}
						else if (spawnItem.CanMerge)
						{
							GameApp.GetInstance().GetGameWorld().EventMergeEnemyID = GameApp.GetInstance().GetGameWorld().EventMergeEnemyID + 1;
						}
						else
						{
							GameApp.GetInstance().GetGameWorld().EventEnemyID = GameApp.GetInstance().GetGameWorld().EventEnemyID + 1;
						}
					}
				}
				else if (GameApp.GetInstance().GetGameWorld().GetEnemies()
					.Count == 0)
				{
					break;
				}
			}
			yield return 0;
		}
		yield return new WaitForSeconds(1f);
		enemyLeft = GameApp.GetInstance().GetGameWorld().GetEnemies()
			.Count;
	}

	private bool EliteSpawn(EnemyType eType, int eliteProbability, int spawnNum, int index)
	{
		bool result = false;
		int num = UnityEngine.Random.Range(0, 100);
		if (num < eliteProbability)
		{
			result = true;
		}
		if (eType == EnemyType.BANELING || eType == EnemyType.TANK)
		{
			result = false;
		}
		return result;
	}

	private Vector3 CalculatePredictPosition(Player player, EnemyType type)
	{
		bool success = false;
		Vector3 pos = Vector3.zero;
		Vector3 position = player.GetTransform().position;
		if (player.inputController.inputInfo.IsMoving())
		{
			Vector3 normalized = new Vector3(player.inputController.inputInfo.moveDirection.x, 0f, player.inputController.inputInfo.moveDirection.z).normalized;
			position += normalized * mSpawnParam.MoveSpawnDistance;
		}
		else
		{
			position += player.GetTransform().forward * mSpawnParam.StopSpawnDistance;
		}
		Vector3 vector = new Vector3(UnityEngine.Random.Range(-2f, 2f), 0f, UnityEngine.Random.Range(-2f, 2f));
		position += vector;
		mRay = new Ray(position + Vector3.up * 20f, Vector3.down);
		int layerMask = 1 << PhysicsLayer.FLOOR;
		if (Physics.Raycast(mRay, out mRaycastHit, 40f, layerMask))
		{
			Vector3 vector2 = mRaycastHit.point + new Vector3(0f, 0.2f, 0f);
			mRay = new Ray(player.GetTransform().position + new Vector3(0f, 0.5f, 0f), vector2 - player.GetTransform().position);
			layerMask = (1 << PhysicsLayer.WALL_AGAINST_PALYER) | (1 << PhysicsLayer.FLOOR) | PhysicsLayer.DESTRUCTIBLE;
			float magnitude = (vector2 - player.GetTransform().position).magnitude;
			if (!Physics.Raycast(mRay, out mRaycastHit, magnitude + 1f, layerMask))
			{
				pos = vector2;
				success = true;
			}
		}
		CorrectSpawnPosition(type, ref success, ref pos);
		if (!success)
		{
			position -= 2f * vector;
			mRay = new Ray(position + Vector3.up * 20f, Vector3.down);
			layerMask = 1 << PhysicsLayer.FLOOR;
			if (Physics.Raycast(mRay, out mRaycastHit, 40f, layerMask))
			{
				Vector3 vector3 = mRaycastHit.point + new Vector3(0f, 0.2f, 0f);
				mRay = new Ray(player.GetTransform().position + new Vector3(0f, 0.5f, 0f), vector3 - player.GetTransform().position);
				layerMask = (1 << PhysicsLayer.WALL_AGAINST_PALYER) | (1 << PhysicsLayer.FLOOR) | PhysicsLayer.DESTRUCTIBLE;
				float magnitude2 = (vector3 - player.GetTransform().position).magnitude;
				if (!Physics.Raycast(mRay, out mRaycastHit, magnitude2 + 1f, layerMask))
				{
					pos = vector3;
					success = true;
				}
			}
			CorrectSpawnPosition(type, ref success, ref pos);
		}
		return pos;
	}

	private Vector3 CalculateEventSpawnPosition(Vector3 playerPosition, float dis, EnemyType type)
	{
		Vector3 pos = Vector3.zero;
		float num = UnityEngine.Random.Range(0f, 360f);
		for (int i = 0; i < 4; i++)
		{
			float f = (num + 90f * (float)i) * ((float)Math.PI / 180f);
			Vector3 vector = playerPosition + dis * new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
			mRay = new Ray(vector + Vector3.up * 20f, Vector3.down);
			int layerMask = 1 << PhysicsLayer.FLOOR;
			bool success = false;
			if (Physics.Raycast(mRay, out mRaycastHit, 40f, layerMask))
			{
				Vector3 vector2 = mRaycastHit.point + new Vector3(0f, 0.2f, 0f);
				mRay = new Ray(playerPosition + new Vector3(0f, 0.5f, 0f), vector2 - playerPosition);
				layerMask = (1 << PhysicsLayer.WALL_AGAINST_PALYER) | (1 << PhysicsLayer.TRANSPARENT_WALL) | (1 << PhysicsLayer.DESTRUCTIBLE);
				float magnitude = (vector2 - playerPosition).magnitude;
				if (!Physics.Raycast(mRay, out mRaycastHit, magnitude + 1f, layerMask))
				{
					pos = vector2;
					success = true;
				}
			}
			CorrectSpawnPosition(type, ref success, ref pos);
			if (success)
			{
				break;
			}
		}
		return pos;
	}

	private Transform CalculateGravePosition(Transform playerTrans)
	{
		GameObject gameObject = null;
		float num = 99999f;
		foreach (GameObject mGrave in mGraveList)
		{
			float sqrMagnitude = (playerTrans.position - mGrave.transform.position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				gameObject = mGrave;
				num = sqrMagnitude;
			}
		}
		return gameObject.transform;
	}

	public void ClearEventEnemy()
	{
		if (MissionManager.GetInstance().GetCurrentStageMission(0).Type != MissionType.KILL_SPECIAL_ENEMY)
		{
			mEventSpawnItems = new List<EnemySpawnItem>();
			mEventMergeSpawnItems = new List<EnemySpawnItem>();
			mEventTotalCount = 0;
			mEventMergeTotalCount = 0;
			GameApp.GetInstance().GetGameWorld().EventEnemyID = 0;
			GameApp.GetInstance().GetGameWorld().EventMergeEnemyID = 0;
		}
	}

	public void SpawnEventEnemy(SpawnConfig config)
	{
		ClearEventEnemy();
		for (int i = 0; i < config.Rounds.Count; i++)
		{
			Round round = config.Rounds[i];
			for (int j = 0; j < round.EnemyInfos.Count; j++)
			{
				EnemySpawnInfo enemySpawnInfo = round.EnemyInfos[j];
				for (int k = 0; k < enemySpawnInfo.Count; k++)
				{
					EnemySpawnItem enemySpawnItem = new EnemySpawnItem();
					enemySpawnItem.Mission = SpawnMissionType.EVENT;
					enemySpawnItem.EType = enemySpawnInfo.EType;
					enemySpawnItem.From = enemySpawnInfo.From;
					enemySpawnItem.EliteProbability = enemySpawnInfo.EliteProbability;
					enemySpawnItem.Round = i;
					enemySpawnItem.intermission = 0f;
					if (j == 0 && k == 0)
					{
						enemySpawnItem.intermission = round.intermission;
					}
					if (CanMerge(enemySpawnInfo.EType))
					{
						enemySpawnItem.CanMerge = true;
						mEventMergeSpawnItems.Add(enemySpawnItem);
						mEventMergeTotalCount++;
					}
					else
					{
						enemySpawnItem.CanMerge = false;
						mEventSpawnItems.Add(enemySpawnItem);
						mEventTotalCount++;
					}
				}
			}
		}
	}

	private void Update()
	{
	}

	private void OnDrawGizmos()
	{
	}

	private void CorrectSpawnPosition(EnemyType type, ref bool success, ref Vector3 pos)
	{
		if (type == EnemyType.BIGBEE && success)
		{
			mRay = new Ray(pos + Vector3.down * 20f, Vector3.up);
			int layerMask = 1 << PhysicsLayer.FLY_AREA;
			if (!Physics.Raycast(mRay, out mRaycastHit, 40f, layerMask))
			{
				success = false;
				pos = Vector3.zero;
			}
		}
		else if (type == EnemyType.BANELING && success)
		{
			Vector3 forward = player.GetTransform().forward;
			Vector3 rhs = pos - player.GetPosition();
			float num = Vector3.Dot(forward.normalized, rhs);
			if (num < 0f)
			{
				pos = Vector3.zero;
				success = false;
			}
		}
	}
}
