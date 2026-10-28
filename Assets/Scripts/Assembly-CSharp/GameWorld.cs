using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWorld
{
	public static string NextLevel;

	public static ELevelType NextLevelType;

	protected LocalPlayer mLocalPlayer;

	protected List<RemotePlayer> remotePlayerList = new List<RemotePlayer>();

	protected List<Robot> mRobotList = new List<Robot>();

	protected Hashtable enemyList = new Hashtable();

	protected HashSet<int> lostEnemyList = new HashSet<int>();

	protected Dictionary<string, ProtectedItem> mProtectedItemDictionary = new Dictionary<string, ProtectedItem>();

	protected Dictionary<string, Robot> mProtectedNPCDictionary = new Dictionary<string, Robot>();

	protected Dictionary<string, DestructibleObjectScript> mDestructibleDictionary = new Dictionary<string, DestructibleObjectScript>();

	protected int enemyRespawns;

	protected Dictionary<EEnemyStateID, EnemyState> mEnemyStateDictionary = new Dictionary<EEnemyStateID, EnemyState>();

	private List<RadarPoint> mRadarPointList = new List<RadarPoint>();

	private Dictionary<string, IndicationEffect> mIndicationEffectDic = new Dictionary<string, IndicationEffect>();

	private Dictionary<string, RadarTargetEffect> mRadarEffectDic = new Dictionary<string, RadarTargetEffect>();

	public Dictionary<int, VFXInfo> VFXDictionary = new Dictionary<int, VFXInfo>();

	protected int mEnemyID;

	protected int mergeEnemyID;

	protected int eventEnemyID;

	protected int eventMergeEnemyID;

	protected int mDestructibleID;

	protected Timer switchBossLevelTimer = new Timer();

	protected EnemyType bossType;

	protected short bossID;

	protected float bossLeveStartFadeTime = -1f;

	protected ThirdPersonStandardCameraScript tpcs;

	protected GameState mGameState;

	protected ObjectPool[] enemyObjectPool = new ObjectPool[8];

	protected ObjectPool[] eliteEnemyObjectPool = new ObjectPool[6];

	protected Dictionary<EnemyType, ObjectPool> mSpecialEnemyPool = new Dictionary<EnemyType, ObjectPool>();

	protected ObjectPool[] enemyBodyPool = new ObjectPool[8];

	protected ObjectPool[] mVehiclePool;

	protected ObjectPool[] mBattlePickUpPool;

	protected ObjectPool[] mMoneyPickUpPool;

	protected ObjectPool[] mVehiclePickUpPool;

	protected ObjectPool[] mPickupSupportPool;

	protected ObjectPool hitBloodPool = new ObjectPool();

	protected ObjectPool[] mLootEffectPool = new ObjectPool[3];

	protected ObjectPool[] mEnemyEffectPool = new ObjectPool[5];

	protected Enemy[] bossArray;

	protected VSClock vsClock;

	protected VSClock flagClock;

	protected VSClock missionClock;

	protected List<ItemScript> itemInSceneList;

	protected VsModeLogic mVsModeLogic;

	private bool mCanSpawnEnemy = true;

	private bool mMainFirstComplete = true;

	public static GameUnit ResultTarget;

	private static List<GameUnit> mTempGameUnitList = new List<GameUnit>();

	private BossCoop mBossCoop;

	public Dictionary<int, TwinBlockInfo> twinBlockDic = new Dictionary<int, TwinBlockInfo>();

	public static bool sCanSpawnEnemy = false;

	private List<ShieldScript> currentActiveShields = new List<ShieldScript>();

	private Pool mPool;

	private float flagMissTime;

	protected GameObject mFlagObject;

	private string[] lootEffects = new string[3]
	{
		EffectName.CASH_LOOT,
		EffectName.ENERGY_LOOT,
		EffectName.HP_LOOT
	};

	public int DifficultyLevel { get; set; }

	public TeamSkill TeamSkills { get; set; }

	public TeamSkill OtherTeamSkill { get; set; }

	public int TotalEnemyCount { get; set; }

	public int TotalWaves { get; set; }

	public int TotalEnemyKilled { get; set; }

	public GameState State
	{
		get
		{
			return mGameState;
		}
	}

	public bool InBossBattle { get; set; }

	public bool StartSpawnEnemy { get; set; }

	public MithrilDropInfo MithrilDrops { get; set; }

	public int OnlineBossMithril { get; set; }

	public int OnlineBossGold { get; set; }

	public int PlayerCountForBoss { get; set; }

	public bool FirstChallenge { get; set; }

	public int BossTotalHp { get; set; }

	public int BossTotalDamage { get; set; }

	public VSBattleInformation BattleInfo { get; set; }

	public PVPRewardInfo PVPReward { get; set; }

	public VSClock FlagClock
	{
		get
		{
			return flagClock;
		}
	}

	public VSClock MissionClock
	{
		get
		{
			return missionClock;
		}
	}

	public bool Exit { get; set; }

	public int FlagInPlayerID { get; set; }

	public int LastFlagInPlayerID { get; set; }

	public GameObject FlagDirObj { get; set; }

	public SwVoidGetRewardResponseData VoidRewardData { get; set; }

	public VsModeLogic VsBattleLogic
	{
		get
		{
			return mVsModeLogic;
		}
	}

	public GameObject FlagObject
	{
		get
		{
			return mFlagObject;
		}
	}

	public int EnemyID
	{
		get
		{
			return mEnemyID;
		}
		set
		{
			mEnemyID = value;
		}
	}

	public int MergeEnemyID
	{
		get
		{
			return mergeEnemyID;
		}
		set
		{
			mergeEnemyID = value;
		}
	}

	public int EventEnemyID
	{
		get
		{
			return eventEnemyID;
		}
		set
		{
			eventEnemyID = value;
		}
	}

	public int EventMergeEnemyID
	{
		get
		{
			return eventMergeEnemyID;
		}
		set
		{
			eventMergeEnemyID = value;
		}
	}

	public bool CanSpawnEnemy()
	{
		return mCanSpawnEnemy;
	}

	public void ToggleSpawnEnemySwitch()
	{
		mCanSpawnEnemy = !mCanSpawnEnemy;
		if (mCanSpawnEnemy)
		{
			return;
		}
		foreach (Enemy value in enemyList.Values)
		{
			if (value != null)
			{
				DamageProperty damageProperty = new DamageProperty(null, value.MaxHp);
				damageProperty.damage = value.MaxHp;
				damageProperty.criticalAttack = true;
				damageProperty.isLocal = true;
				damageProperty.wType = WeaponType.NoGun;
				damageProperty.hitpoint = value.GetTransform().position + Vector3.up * 2f;
				value.HitEnemy(mLocalPlayer, damageProperty);
			}
		}
	}

	public Enemy GetBoss(int index)
	{
		if (index < bossArray.Length)
		{
			return bossArray[index];
		}
		return null;
	}

	public void DropTheFlag(Vector3 pos)
	{
		if (mFlagObject == null)
		{
			GameObject original = Resources.Load("Pickup/NewFlag") as GameObject;
			mFlagObject = UnityEngine.Object.Instantiate(original) as GameObject;
			mFlagObject.name = "CathTheFlag";
		}
		mFlagObject.transform.position = pos;
		flagMissTime = 0f;
	}

	public void HideTheFlag()
	{
		if (mFlagObject != null)
		{
			UnityEngine.Object.Destroy(mFlagObject);
		}
		flagMissTime = 0f;
	}

	public void CatchTheFlag()
	{
		if (mFlagObject != null)
		{
			UnityEngine.Object.Destroy(mFlagObject);
		}
		flagMissTime = 0f;
	}

	public void Init()
	{
		MemoryManager.UnloadUITexture();
		if (GameApp.GetInstance().GetGameMode().IsSingle())
		{
			GameApp.GetInstance().GetUserState().Boss1Type = EnemyType.NA;
			GameApp.GetInstance().GetUserState().Boss2Type = EnemyType.NA;
		}
		Exit = false;
		StartSpawnEnemy = false;
		UserState userState = GameApp.GetInstance().GetUserState();
		if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.BOSS_FINAL)
		{
			mLocalPlayer = GlobalData.GetInstance().mPlayer;
			mLocalPlayer.InitInBossStage();
		}
		else if (GameApp.GetInstance().GetGameMode().IsUsePvpCustomizeMode())
		{
			userState.LoadCurrentCustomize(false, 0);
			mLocalPlayer = new LocalPlayer();
			mLocalPlayer.Init();
		}
		else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.GIANT_BETRAYAL)
		{
			userState.LoadCurrentCustomize(true, 0);
			mLocalPlayer = new LocalPlayer();
			mLocalPlayer.Init();
		}
		else
		{
			mLocalPlayer = new LocalPlayer();
			mLocalPlayer.Init();
		}
		if (GameApp.GetInstance().GetGameMode().ModePlay != Mode.BOSS_FINAL)
		{
			userState.UseFlaskItems(!GameApp.GetInstance().GetGameMode().IsVSMode());
		}
		InitPool();
		mBossCoop = null;
		if (GameApp.GetInstance().GetGameMode().IsCatchTheFlagMode() && Lobby.GetInstance().IsMasterPlayer)
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag(TagName.FLAG_RESPAWN);
			int num = UnityEngine.Random.Range(0, array.Length);
			DropTheFlagRequest request = new DropTheFlagRequest(-1, array[num].transform.position + Vector3.up * 0f, true);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			flagMissTime = 0f;
		}
		tpcs = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
		tpcs.Init();
		mGameState = GameState.Playing;
		MissionManager.GetInstance().LevelInit();
		TotalEnemyKilled = 0;
		vsClock = Lobby.GetInstance().GetVSClock();
		vsClock.Restart();
		InitVsModeLogic();
		GameObject prefab = Resources.Load("_Old/_Effect/hitBlood") as GameObject;
		hitBloodPool.Init("effect/hitBlood", prefab, 12, 0.8f);
		InBossBattle = false;
		BattleInfo = new VSBattleInformation();
		PVPReward = new PVPRewardInfo();
		FlagInPlayerID = -1;
		LastFlagInPlayerID = -1;
		InitPickUpPool();
		InitEnemyPool();
		InitVehiclePool();
		InitEnemyState();
		InitSpecialEnemyPool();
		InitVFXDictionary();
		itemInSceneList = new List<ItemScript>();
		mMainFirstComplete = !userState.GetMissionStatus(userState.GetSelectedStageIndex(), userState.GetSelectedSubStageIndex(), 0, EDifficultMode.NORMAL);
		VoidRewardData = null;
		currentActiveShields = new List<ShieldScript>();
	}

	private void InitPool()
	{
		mPool = new Pool();
		if (!GameApp.GetInstance().GetUserState().IsLowQuality())
		{
			Pool.sGetInstance().PreAllocate(EffectName.MONSTER_FOOT_PARTICLE, 8);
		}
	}

	private void InitPickUpPool()
	{
		if (mMoneyPickUpPool != null && mBattlePickUpPool != null && mVehiclePickUpPool != null && mPickupSupportPool != null)
		{
			return;
		}
		GameObject root = new GameObject("_PickUp");
		mMoneyPickUpPool = new ObjectPool[1];
		mBattlePickUpPool = new ObjectPool[17];
		mVehiclePickUpPool = new ObjectPool[7];
		mPickupSupportPool = new ObjectPool[1];
		for (int i = 0; i < mMoneyPickUpPool.Length; i++)
		{
			mMoneyPickUpPool[i] = new ObjectPool();
			mMoneyPickUpPool[i].CanUseFirstOne = true;
		}
		GameObject prefab = Resources.Load("Pickup/money") as GameObject;
		mMoneyPickUpPool[0].Init("Money", prefab, 5, 1f);
		for (int j = 0; j < mMoneyPickUpPool.Length; j++)
		{
			mMoneyPickUpPool[j].SetRoot(root);
		}
		for (int k = 0; k < mPickupSupportPool.Length; k++)
		{
			mPickupSupportPool[k] = new ObjectPool();
			mPickupSupportPool[k].CanUseFirstOne = true;
			prefab = Resources.Load(EffectName.PICKUP_SUPPORT) as GameObject;
			mPickupSupportPool[k].Init("PickupSupport", prefab, 5, 1f);
			mPickupSupportPool[k].SetRoot(root);
		}
		for (int l = 0; l < mBattlePickUpPool.Length; l++)
		{
			mBattlePickUpPool[l] = new ObjectPool();
			mBattlePickUpPool[l].CanUseFirstOne = true;
			prefab = Resources.Load("Pickup/PickUp" + (l + 1)) as GameObject;
			if (prefab != null)
			{
				mBattlePickUpPool[l].Init("PickUp" + (l + 1), prefab, 1, 1f);
			}
			mBattlePickUpPool[l].SetRoot(root);
		}
		for (int m = 0; m < mVehiclePickUpPool.Length; m++)
		{
			mVehiclePickUpPool[m] = new ObjectPool();
			mVehiclePickUpPool[m].CanUseFirstOne = true;
			prefab = Resources.Load("Pickup/vehicle" + m) as GameObject;
			if (prefab != null)
			{
				mVehiclePickUpPool[m].Init("vehicle" + m, prefab, 1, 1f);
			}
			mVehiclePickUpPool[m].SetRoot(root);
		}
		for (int n = 0; n < mLootEffectPool.Length; n++)
		{
			mLootEffectPool[n] = new ObjectPool();
			prefab = Resources.Load(lootEffects[n]) as GameObject;
			mLootEffectPool[n].Init(lootEffects[n], prefab, 4, 2f);
			mLootEffectPool[n].SetRoot(root);
		}
	}

	public int GetLootEffectIndex(string prefab)
	{
		int result = -1;
		for (int i = 0; i < lootEffects.Length; i++)
		{
			if (lootEffects[i].Equals(prefab))
			{
				result = i;
				break;
			}
		}
		return result;
	}

	public void SpawnLootEffect(int index, Vector3 position, Transform parent)
	{
		GameObject gameObject = mLootEffectPool[index].CreateObject(position, Vector3.zero, Quaternion.identity);
		gameObject.transform.parent = parent;
	}

	public GameObject CreateEnemyEffect(EEnemyEffectType type, Vector3 position, Quaternion rotation)
	{
		if (mEnemyEffectPool[(int)type] != null)
		{
			return mEnemyEffectPool[(int)type].CreateObject(position, position, rotation);
		}
		return null;
	}

	private void InitEnemyPool()
	{
		GameObject gameObject = null;
		if (!GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			GameObject gameObject2 = GameObject.Find("_Enemy");
			if (gameObject2 == null)
			{
				gameObject2 = new GameObject("_Enemy");
			}
			for (int i = 0; i < 8; i++)
			{
				enemyObjectPool[i] = new ObjectPool();
				gameObject = Resources.Load("Enemy/" + Global.QUALITY_LEVEL_1 + "/enemy" + string.Format("{0:D2}", i + 1)) as GameObject;
				enemyObjectPool[i].Init("Enemy_" + (i + 1), gameObject, 16, 0.8f);
				enemyObjectPool[i].SetRoot(gameObject2);
				enemyBodyPool[i] = new ObjectPool();
				gameObject = Resources.Load("SW2_Effect/DeadBody/Deadbody " + i) as GameObject;
				enemyBodyPool[i].Init("DeadBody" + (i + 1), gameObject, 16, 3f);
				enemyBodyPool[i].SetRoot(gameObject2);
			}
			for (int j = 0; j < 6; j++)
			{
				eliteEnemyObjectPool[j] = new ObjectPool();
				gameObject = Resources.Load("Enemy/" + Global.QUALITY_LEVEL_1 + "/enemy" + string.Format("{0:D2}", j + 1) + "_elite") as GameObject;
				eliteEnemyObjectPool[j].Init("Enemy_" + (j + 1) + "_Elite", gameObject, 2, 0.8f);
				eliteEnemyObjectPool[j].SetRoot(gameObject2);
			}
			GameObject prefab = Resources.Load(EffectName.E_Yan) as GameObject;
			mEnemyEffectPool[0] = new ObjectPool();
			mEnemyEffectPool[0].Init(EEnemyEffectType.YAN.ToString(), prefab, 6, 2f);
			mEnemyEffectPool[0].SetRoot(gameObject2);
			prefab = Resources.Load(EffectName.E_GraveRock1) as GameObject;
			mEnemyEffectPool[1] = new ObjectPool();
			mEnemyEffectPool[1].Init(EEnemyEffectType.GRAVE_ROCK1.ToString(), prefab, 6, 2f);
			mEnemyEffectPool[1].SetRoot(gameObject2);
			prefab = Resources.Load(EffectName.E_GraveSmoke) as GameObject;
			mEnemyEffectPool[2] = new ObjectPool();
			mEnemyEffectPool[2].Init(EEnemyEffectType.GRAVE_SMOKE.ToString(), prefab, 6, 2f);
			mEnemyEffectPool[2].SetRoot(gameObject2);
			prefab = Resources.Load(EffectName.E_GraveGround) as GameObject;
			mEnemyEffectPool[3] = new ObjectPool();
			mEnemyEffectPool[3].Init(EEnemyEffectType.GRAVE_GROUND.ToString(), prefab, 6, 2f);
			mEnemyEffectPool[3].SetRoot(gameObject2);
			prefab = Resources.Load(EffectName.E_GraveRock2) as GameObject;
			mEnemyEffectPool[4] = new ObjectPool();
			mEnemyEffectPool[4].Init(EEnemyEffectType.GRAVE_ROCK2.ToString(), prefab, 6, 2f);
			mEnemyEffectPool[4].SetRoot(gameObject2);
		}
	}

	private void InitSpecialEnemyPool()
	{
		if (!GameApp.GetInstance().GetGameMode().IsSingle())
		{
			return;
		}
		mSpecialEnemyPool = new Dictionary<EnemyType, ObjectPool>();
		GameObject gameObject = GameObject.Find("_Enemy");
		if (gameObject == null)
		{
			gameObject = new GameObject("_Enemy");
		}
		byte selectedStageIndex = GameApp.GetInstance().GetUserState().GetSelectedStageIndex();
		byte selectedSubStageIndex = GameApp.GetInstance().GetUserState().GetSelectedSubStageIndex();
		EDifficultMode selectedDifficultMode = GameApp.GetInstance().GetUserState().GetSelectedDifficultMode();
		Mission mission = MissionManager.GetInstance().GetMission(selectedStageIndex, selectedSubStageIndex, 0, selectedDifficultMode);
		if (mission == null)
		{
			return;
		}
		List<EnemyType> bossTypeList = mission.GetBossTypeList();
		foreach (EnemyType item in bossTypeList)
		{
			ObjectPool objectPool = new ObjectPool();
			if (item == EnemyType.SPECIAL_SLASHER)
			{
				GameObject prefab = Resources.Load("Enemy/" + Global.QUALITY_LEVEL_1 + "/Special/SpecialSlasher") as GameObject;
				objectPool.Init(item.ToString(), prefab, 1, 1f);
				objectPool.SetRoot(gameObject);
			}
			mSpecialEnemyPool.Add(item, objectPool);
		}
	}

	public void ReleaseAllEnemies()
	{
		if (mLocalPlayer != null && mLocalPlayer.GetTransform() != null)
		{
			GameObject[] array = new GameObject[mLocalPlayer.GetTransform().childCount];
			for (int i = 0; i < mLocalPlayer.GetTransform().childCount; i++)
			{
				array[i] = mLocalPlayer.GetTransform().GetChild(i).gameObject;
			}
			GameObject[] array2 = array;
			foreach (GameObject obj in array2)
			{
				UnityEngine.Object.DestroyImmediate(obj);
			}
			EffectPool.GetInstance().GetEffectPool(EffectPoolType.RESPAWN_1).CreateObject(mLocalPlayer.GetPosition(), mLocalPlayer.GetTransform().forward, Quaternion.identity);
			EffectPool.GetInstance().GetEffectPool(EffectPoolType.PORTAL_OFF).CreateObject(mLocalPlayer.GetPosition(), mLocalPlayer.GetTransform().forward, Quaternion.identity);
		}
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && remotePlayerList != null)
		{
			foreach (RemotePlayer remotePlayer in remotePlayerList)
			{
				if (remotePlayer != null && remotePlayer.GetTransform() != null)
				{
					remotePlayer.GetOffVehicleImmediately(true);
					GameObject[] array3 = new GameObject[remotePlayer.GetTransform().childCount];
					for (int k = 0; k < remotePlayer.GetTransform().childCount; k++)
					{
						array3[k] = remotePlayer.GetTransform().GetChild(k).gameObject;
					}
					GameObject[] array4 = array3;
					foreach (GameObject obj2 in array4)
					{
						UnityEngine.Object.DestroyImmediate(obj2);
					}
				}
			}
		}
		if (mMoneyPickUpPool != null)
		{
			for (int m = 0; m < mMoneyPickUpPool.Length; m++)
			{
				if (mMoneyPickUpPool[m] != null)
				{
					mMoneyPickUpPool[m].DestructAll();
					mMoneyPickUpPool[m].ReleaseAll();
					mMoneyPickUpPool[m] = null;
				}
			}
			mMoneyPickUpPool = null;
		}
		if (mBattlePickUpPool != null)
		{
			for (int n = 0; n < mBattlePickUpPool.Length; n++)
			{
				if (mBattlePickUpPool[n] != null)
				{
					mBattlePickUpPool[n].DestructAll();
					mBattlePickUpPool[n].ReleaseAll();
					mBattlePickUpPool[n] = null;
				}
			}
			mBattlePickUpPool = null;
		}
		if (mVehiclePickUpPool != null)
		{
			for (int num = 0; num < mVehiclePickUpPool.Length; num++)
			{
				if (mVehiclePickUpPool[num] != null)
				{
					mVehiclePickUpPool[num].DestructAll();
					mVehiclePickUpPool[num].ReleaseAll();
					mVehiclePickUpPool[num] = null;
				}
			}
			mVehiclePickUpPool = null;
		}
		if (mPickupSupportPool != null)
		{
			for (int num2 = 0; num2 < mPickupSupportPool.Length; num2++)
			{
				if (mPickupSupportPool[num2] != null)
				{
					mPickupSupportPool[num2].DestructAll();
					mPickupSupportPool[num2].ReleaseAll();
					mPickupSupportPool[num2] = null;
				}
			}
			mPickupSupportPool = null;
		}
		for (int num3 = 0; num3 < mVehiclePool.Length; num3++)
		{
			if (mVehiclePool[num3] != null)
			{
				mVehiclePool[num3].DestructAll();
				mVehiclePool[num3].ReleaseAll();
				mVehiclePool[num3] = null;
			}
		}
		enemyList.Clear();
		for (int num4 = 0; num4 < 8; num4++)
		{
			if (enemyObjectPool[num4] != null)
			{
				enemyObjectPool[num4].DestructAll();
				enemyObjectPool[num4].ReleaseAll();
				enemyObjectPool[num4] = null;
			}
			if (enemyBodyPool[num4] != null)
			{
				enemyBodyPool[num4].DestructAll();
				enemyBodyPool[num4].ReleaseAll();
				enemyBodyPool[num4] = null;
			}
		}
		for (int num5 = 0; num5 < 6; num5++)
		{
			if (eliteEnemyObjectPool[num5] != null)
			{
				eliteEnemyObjectPool[num5].DestructAll();
				eliteEnemyObjectPool[num5].ReleaseAll();
				eliteEnemyObjectPool[num5] = null;
			}
		}
		if (mSpecialEnemyPool != null)
		{
			foreach (EnemyType key in mSpecialEnemyPool.Keys)
			{
				ObjectPool objectPool = mSpecialEnemyPool[key];
				if (objectPool != null)
				{
					objectPool.DestructAll();
					objectPool.ReleaseAll();
				}
			}
			mSpecialEnemyPool.Clear();
		}
		for (int num6 = 0; num6 < mLootEffectPool.Length; num6++)
		{
			if (mLootEffectPool[num6] != null)
			{
				mLootEffectPool[num6].DestructAll();
				mLootEffectPool[num6].ReleaseAll();
				mLootEffectPool[num6] = null;
			}
		}
		for (int num7 = 0; num7 < 5; num7++)
		{
			if (mEnemyEffectPool[num7] != null)
			{
				mEnemyEffectPool[num7].AutoDestruct();
				mEnemyEffectPool[num7].ReleaseAll();
				mEnemyEffectPool[num7] = null;
			}
		}
		EffectPool.GetInstance().GetEffectPool(EffectPoolType.MINE).DestructAll();
		EffectPool.GetInstance().GetEffectPool(EffectPoolType.MINE).ReleaseAll();
		Pool.sGetInstance().Deallocate(EffectName.MONSTER_FOOT_PARTICLE);
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	private void InitVehiclePool()
	{
		mVehiclePool = new ObjectPool[7];
		for (int i = 0; i < mVehiclePool.Length; i++)
		{
			mVehiclePool[i] = new ObjectPool();
			mVehiclePool[i].CanUseFirstOne = false;
		}
		GameObject gameObject = new GameObject("_Vehicle");
	}

	public void InitEnemyState()
	{
		mEnemyStateDictionary = new Dictionary<EEnemyStateID, EnemyState>();
		mEnemyStateDictionary.Add(EEnemyStateID.IDLE, Enemy.IDLE_STATE);
		mEnemyStateDictionary.Add(EEnemyStateID.ATTACK, Enemy.ATTACK_STATE);
		mEnemyStateDictionary.Add(EEnemyStateID.CATCHING, Enemy.CATCHING_STATE);
		mEnemyStateDictionary.Add(EEnemyStateID.ZERGLING_LOOK_AROUND, null);
		mEnemyStateDictionary.Add(EEnemyStateID.ZERGLING_JUMP, null);
		mEnemyStateDictionary.Add(EEnemyStateID.BLACKHOLE, Enemy.BLACK_HOLE_STATE);
		mEnemyStateDictionary.Add(EEnemyStateID.STUN, Enemy.STUN_STATE);
		mEnemyStateDictionary.Add(EEnemyStateID.BOSS_RAGE, EnemyBoss.BOSS_RAGE_STATE);
		mEnemyStateDictionary.Add(EEnemyStateID.SLASHER_ATTACK01, Slasher.SLASHER_ATTACK01_STATE);
		mEnemyStateDictionary.Add(EEnemyStateID.SLASHER_ATTACK02, Slasher.SLASHER_ATTACK02_STATE);
		mEnemyStateDictionary.Add(EEnemyStateID.SLASHER_ATTACK03, Slasher.SLASHER_ATTACK03_STATE);
		mEnemyStateDictionary.Add(EEnemyStateID.SLASHER_DASH01, Slasher.SLASHER_DASH01_STATE);
		mEnemyStateDictionary.Add(EEnemyStateID.SLASHER_RANGE01, Slasher.SLASHER_RANGE01_STATE);
		mEnemyStateDictionary.Add(EEnemyStateID.SLASHER_RANGE02, Slasher.SLASHER_RANGE02_STATE);
		mEnemyStateDictionary.Add(EEnemyStateID.SLASHER_WEAPON_GET_DASH01, Slasher.SLASHER_WEAPON_GET_DASH01_STATE);
		mEnemyStateDictionary.Add(EEnemyStateID.SLASHER_WEAPON_RUN, Slasher.SLASHER_WEAPON_RUN_STATE);
		mEnemyStateDictionary.Add(EEnemyStateID.SLASHER_WEAPON_ATTACK01, Slasher.SLASHER_WEAPON_ATTACK01_STATE);
		mEnemyStateDictionary.Add(EEnemyStateID.SLASHER_WEAPON_ATTACK02, Slasher.SLASHER_WEAPON_ATTACK02_STATE);
		mEnemyStateDictionary.Add(EEnemyStateID.SLASHER_WEAPON_DASH01, Slasher.SLASHER_WEAPON_DASH01_STATE);
		mEnemyStateDictionary.Add(EEnemyStateID.SLASHER_WEAPON_RANGE01, Slasher.SLASHER_WEAPON_RANGE01_STATE);
		mEnemyStateDictionary.Add(EEnemyStateID.SLASHER_WEAPON_DROP, Slasher.SLASHER_WEAPON_DROP_STATE);
		mEnemyStateDictionary.Add(EEnemyStateID.SLASHER_WEAPON_WAVE, Slasher.SLASHER_WEAPON_WAVE_STATE);
		mEnemyStateDictionary.Add(EEnemyStateID.SLASHER_WEAPON_BLADE, Slasher.SLASHER_WEAPON_BLADE_STATE);
		mEnemyStateDictionary.Add(EEnemyStateID.SLASHER_STEPBACK, Slasher.SLASHER_STEPBACK_STATE);
		mEnemyStateDictionary.Add(EEnemyStateID.SLASHER_WEAPON_STEPBACK, Slasher.SLASHER_WEAPON_STEPBACK_STATE);
		mEnemyStateDictionary.Add(EEnemyStateID.STALKER_JUMP_CATCH, Stalker.JUMP_CATCH_STATE);
		mEnemyStateDictionary.Add(EEnemyStateID.STALKER_JUMP_ATTACK, Stalker.JUMP_ATTACK_STATE);
	}

	public void StartMissionClock()
	{
		missionClock = new VSClock();
		missionClock.SetTotalGameSeconds(MissionManager.GetInstance().TimeLimitation);
		missionClock.Restart();
	}

	public ObjectPool GetDeadBodyPool(EnemyType eType)
	{
		return enemyBodyPool[(int)eType];
	}

	public ObjectPool GetHitBloodPool()
	{
		return hitBloodPool;
	}

	public ObjectPool GetEffectPool(EffectPoolType type)
	{
		return EffectPool.GetInstance().GetEffectPool(type);
	}

	public EnemyState GetEnemyState(EEnemyStateID stateID)
	{
		return mEnemyStateDictionary[stateID];
	}

	public EEnemyStateID GetEnemyStateID(EnemyState state)
	{
		foreach (KeyValuePair<EEnemyStateID, EnemyState> item in mEnemyStateDictionary)
		{
			if (item.Value == state)
			{
				return item.Key;
			}
		}
		Debug.LogWarning("NO EnemyStateId for: " + state);
		return EEnemyStateID.NO_STATE;
	}

	public void AddLostEnemy(int enemyID)
	{
		lostEnemyList.Add(enemyID);
	}

	public int GetLostEnemyCount()
	{
		return lostEnemyList.Count;
	}

	public void AddRespawnedEnemy()
	{
		enemyRespawns++;
	}

	public int GetRespawnedEnemy()
	{
		return enemyRespawns;
	}

	public ThirdPersonStandardCameraScript GetCamera()
	{
		return tpcs;
	}

	public int GetEnemyTypeID(EnemyType eType)
	{
		int result = 0;
		switch (eType)
		{
		case EnemyType.DRONE:
			result = 0;
			break;
		case EnemyType.STALKER:
			result = 1;
			break;
		case EnemyType.SCORPION:
			result = 2;
			break;
		case EnemyType.BIGBEE:
			result = 3;
			break;
		case EnemyType.MYTH370:
			result = 4;
			break;
		case EnemyType.DEMSTER:
			result = 5;
			break;
		case EnemyType.BANELING:
			result = 6;
			break;
		case EnemyType.TANK:
			result = 7;
			break;
		}
		return result;
	}

	public LocalPlayer GetLocalPlayer()
	{
		return mLocalPlayer;
	}

	public int GetPlayerCount()
	{
		return remotePlayerList.Count + 1;
	}

	public int GetInGamePlayerCount()
	{
		int num = 1;
		foreach (RemotePlayer remotePlayer in remotePlayerList)
		{
			if (remotePlayer != null && remotePlayer.State != Player.LOSE_STATE)
			{
				num++;
			}
		}
		return num;
	}

	public int GetPlayingPlayerCount()
	{
		int num = 0;
		if (mLocalPlayer.InPlayingState())
		{
			num++;
		}
		foreach (RemotePlayer remotePlayer in remotePlayerList)
		{
			if (remotePlayer.InPlayingState())
			{
				num++;
			}
		}
		return num;
	}

	public Enemy SpawnEnemy(EnemyType eType, short enemyID, Vector3 spawnPosition, bool bElite, bool bFromGrave, int playerCount, int difficultLevel)
	{
		if (State != GameState.Playing)
		{
			return null;
		}
		Enemy enemy = null;
		GameObject gameObject = null;
		switch (eType)
		{
		case EnemyType.DRONE:
			enemy = new Drone();
			break;
		case EnemyType.STALKER:
			enemy = new Stalker();
			break;
		case EnemyType.SCORPION:
			enemy = new Tacorlion();
			break;
		case EnemyType.BIGBEE:
			enemy = new BigBee();
			break;
		case EnemyType.MYTH370:
			enemy = new Myth370();
			break;
		case EnemyType.DEMSTER:
			enemy = new Demster();
			break;
		case EnemyType.BANELING:
			enemy = new Baneling();
			break;
		case EnemyType.SPECIAL_SLASHER:
			enemy = new SpecialSlasher();
			break;
		case EnemyType.TANK:
			enemy = new Tank();
			break;
		}
		if (enemy != null && State != GameState.SwitchBossLevel)
		{
			enemy.EnemyType = eType;
			if (GameObject.Find("E_" + enemyID) == null)
			{
				gameObject = ((eType == EnemyType.SPECIAL_SLASHER) ? mSpecialEnemyPool[eType].CreateObject(Vector3.zero, Vector3.zero, Quaternion.identity) : (bElite ? eliteEnemyObjectPool[GetEnemyTypeID(eType)].CreateObject(Vector3.zero, Vector3.zero, Quaternion.identity) : enemyObjectPool[GetEnemyTypeID(eType)].CreateObject(Vector3.zero, Vector3.zero, Quaternion.identity)));
				enemy.IsElite = bElite;
				enemy.PlayerCount = playerCount;
				enemy.DifficultLevel = difficultLevel;
				enemy.EnemyID = enemyID;
				enemy.Init(gameObject);
				gameObject.transform.position = spawnPosition;
				gameObject.name = "E_" + enemyID;
				enemy.Name = gameObject.name;
				enemyList.Add(gameObject.name, enemy);
				if (eType != EnemyType.SPECIAL_SLASHER)
				{
					if (bFromGrave)
					{
						CreateEnemyEffect(EEnemyEffectType.YAN, spawnPosition + Vector3.up, Quaternion.identity);
						CreateEnemyEffect(EEnemyEffectType.GRAVE_ROCK1, spawnPosition, Quaternion.identity);
						GameObject gameObject2 = CreateEnemyEffect(EEnemyEffectType.GRAVE_SMOKE, spawnPosition + Vector3.up, Quaternion.identity);
						CreateEnemyEffect(EEnemyEffectType.GRAVE_GROUND, GetPositionOnGround(spawnPosition), Quaternion.Euler(270f, 0f, 0f));
						if (gameObject2 != null)
						{
							gameObject2.transform.parent = enemy.GetTransform();
						}
						enemy.SetState(Enemy.GRAVEBORN_STATE);
					}
					else
					{
						enemy.SetState(Enemy.IDLE_STATE);
						enemy.StartEnemyIdleFromDoor();
					}
				}
			}
		}
		return enemy;
	}

	public static EnemyBoss SpawnBoss(EnemyType type, short ID, Vector3 position)
	{
		EnemyBoss enemyBoss = null;
		string path = string.Empty;
		switch (type)
		{
		case EnemyType.DEAD_EYE:
			path = "Enemy/" + Global.QUALITY_LEVEL_1 + "/Boss/DeadEye";
			enemyBoss = new DeadEye();
			break;
		case EnemyType.SLASHER:
			path = "Enemy/" + Global.QUALITY_LEVEL_1 + "/Boss/Slasher";
			enemyBoss = new Slasher();
			break;
		case EnemyType.FROST_STONE:
			path = "Enemy/" + Global.QUALITY_LEVEL_1 + "/Boss/FrostStone";
			enemyBoss = new FrostStone();
			break;
		case EnemyType.CERBERLISK:
			path = "Enemy/" + Global.QUALITY_LEVEL_1 + "/Boss/Cerberlisk";
			enemyBoss = new Cerberlisk();
			break;
		case EnemyType.CRIMSON_QUEEN:
			path = "Enemy/" + Global.QUALITY_LEVEL_1 + "/Boss/CrimsonQueen";
			enemyBoss = new CrimsonQueen();
			break;
		}
		enemyBoss.EnemyType = type;
		GameObject original = Resources.Load(path) as GameObject;
		GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
		gameObject.transform.position = position;
		gameObject.name = "E_" + ID;
		enemyBoss.EnemyID = ID;
		enemyBoss.Init(gameObject);
		GameApp.GetInstance().GetGameWorld().AddEnemy(gameObject.name, enemyBoss);
		return enemyBoss;
	}

	public void AddEnemy(string enemyName, Enemy enemy)
	{
		enemyList.Add(enemyName, enemy);
	}

	public void AddProtectedItem(ProtectedItem item)
	{
		if (!mProtectedItemDictionary.ContainsKey(item.Name))
		{
			mProtectedItemDictionary.Add(item.Name, item);
		}
	}

	public void AddProtectedNPC(Robot item)
	{
		if (!mProtectedNPCDictionary.ContainsKey(item.Name))
		{
			mProtectedNPCDictionary.Add(item.Name, item);
		}
	}

	public ProtectedItem GetRandomProtectedItem()
	{
		List<ProtectedItem> list = new List<ProtectedItem>();
		foreach (ProtectedItem value in mProtectedItemDictionary.Values)
		{
			list.Add(value);
		}
		if (list.Count > 0)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			return list[index];
		}
		return null;
	}

	public List<ProtectedItem> GetProtectedItemList()
	{
		List<ProtectedItem> list = new List<ProtectedItem>();
		foreach (ProtectedItem value in mProtectedItemDictionary.Values)
		{
			list.Add(value);
		}
		return list;
	}

	public ProtectedItem GetProtectedItemByName(string itemName)
	{
		if (mProtectedItemDictionary.ContainsKey(itemName))
		{
			return mProtectedItemDictionary[itemName];
		}
		return null;
	}

	public void AddRemotePlayer(RemotePlayer player)
	{
		remotePlayerList.Add(player);
		Debug.Log("AddRemotePlayer id = " + player.GetChannelID() + ", remotePlayerSize = " + remotePlayerList.Count);
	}

	public void AddRobot(Robot robot)
	{
		mRobotList.Add(robot);
	}

	public RemotePlayer GetRemotePlayerByUserID(int userID)
	{
		foreach (RemotePlayer remotePlayer in remotePlayerList)
		{
			if (remotePlayer.GetChannelID() == userID)
			{
				return remotePlayer;
			}
		}
		return null;
	}

	public void RemoveRemotePlayer(int userID)
	{
		List<RemotePlayer> list = new List<RemotePlayer>();
		foreach (RemotePlayer remotePlayer in remotePlayerList)
		{
			if (remotePlayer.GetChannelID() != userID)
			{
				continue;
			}
			if (GameApp.GetInstance().GetGameMode().IsCoopMode())
			{
				if (GameApp.GetInstance().GetGameMode().IsSurvivalMode())
				{
					remotePlayer.GetOffVehicleImmediately(false);
					UnityEngine.Object.Destroy(remotePlayer.GetObject());
					remotePlayer.GetTransform().position = new Vector3(0f, -10000f, 0f);
					list.Add(remotePlayer);
				}
				else
				{
					remotePlayer.OnQuitOnline();
					remotePlayer.GetObject().SetActive(false);
				}
			}
			else
			{
				remotePlayer.GetOffVehicleImmediately(false);
				UnityEngine.Object.Destroy(remotePlayer.GetObject());
				remotePlayer.GetTransform().position = new Vector3(0f, -10000f, 0f);
				list.Add(remotePlayer);
			}
			if (remotePlayer.mSwVoid != null)
			{
				remotePlayer.mSwVoid.Destroy();
			}
			if (remotePlayer.GetWeapon() != null)
			{
				remotePlayer.GetWeapon().ClearEffect();
			}
		}
		foreach (RemotePlayer item in list)
		{
			if (remotePlayerList.Contains(item))
			{
				remotePlayerList.Remove(item);
				Debug.Log("RemoveRemotePlayer id = " + item.GetChannelID() + ", remotePlayerSize = " + remotePlayerList.Count);
			}
		}
	}

	public Robot GetRobotByName(string name)
	{
		foreach (Robot mRobot in mRobotList)
		{
			if (mRobot.GetObject().name == name)
			{
				return mRobot;
			}
		}
		return null;
	}

	public List<RemotePlayer> GetRemotePlayers()
	{
		return remotePlayerList;
	}

	public void SetRemotePlayers(List<RemotePlayer> playerList)
	{
		remotePlayerList = playerList;
	}

	public Hashtable GetEnemies()
	{
		return enemyList;
	}

	public List<Robot> GetRobotList()
	{
		return mRobotList;
	}

	public int GetNormalEnemiesCount()
	{
		int num = 0;
		foreach (Enemy value in enemyList.Values)
		{
			if (!EnemySpawnScript.CanMerge(value.EnemyType))
			{
				num++;
			}
		}
		return num;
	}

	public int GetMergeEnemiesCount()
	{
		int num = 0;
		foreach (Enemy value in enemyList.Values)
		{
			if (EnemySpawnScript.CanMerge(value.EnemyType))
			{
				num++;
			}
		}
		return num;
	}

	public Enemy GetEnemyByID(string enemyID)
	{
		return (Enemy)enemyList[enemyID];
	}

	public int GetDestructibleCount()
	{
		return mDestructibleDictionary.Count;
	}

	public Dictionary<string, DestructibleObjectScript> GetDestructibleDic()
	{
		return mDestructibleDictionary;
	}

	public void AddDestructible(GameObject obj)
	{
		DestructibleObjectScript component = obj.GetComponent<DestructibleObjectScript>();
		if (null != component)
		{
			mDestructibleID++;
			component.Activate();
			mDestructibleDictionary.Add(obj.name, component);
		}
	}

	public void RemoveDestructible(string name)
	{
		if (mDestructibleDictionary.ContainsKey(name))
		{
			mDestructibleDictionary.Remove(name);
		}
	}

	public void ClearDestructibleDictionary()
	{
		mDestructibleDictionary = new Dictionary<string, DestructibleObjectScript>();
	}

	public DestructibleObjectScript GetDestructibleByName(GameObject go)
	{
		if (mDestructibleDictionary.ContainsKey(go.name))
		{
			return mDestructibleDictionary[go.name];
		}
		return go.GetComponent<DestructibleObjectScript>();
	}

	public void OnMainMissionFinished()
	{
		foreach (Enemy value in enemyList.Values)
		{
			if (value != null && value.Hp > 0)
			{
				DamageProperty damageProperty = new DamageProperty(null, value.Hp);
				damageProperty.isLocal = true;
				damageProperty.criticalAttack = true;
				damageProperty.wType = WeaponType.NoGun;
				value.OnHit(damageProperty);
			}
		}
	}

	public void Loop(float deltaTime)
	{
		if (vsClock != null)
		{
			vsClock.Update();
		}
		if (flagClock != null)
		{
			flagClock.Update();
		}
		if (missionClock != null)
		{
			missionClock.Update();
		}
		if (VsBattleLogic != null)
		{
			VsBattleLogic.Update();
		}
		MissionManager.GetInstance().Update();
		mPool.Update();
		if (mLocalPlayer != null)
		{
			mLocalPlayer.Loop(deltaTime);
		}
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			foreach (RemotePlayer remotePlayer in remotePlayerList)
			{
				remotePlayer.Loop(deltaTime);
			}
		}
		if (State == GameState.Playing)
		{
			object[] array = new object[enemyList.Count];
			enemyList.Keys.CopyTo(array, 0);
			for (int i = 0; i < array.Length; i++)
			{
				Enemy enemy = enemyList[array[i]] as Enemy;
				enemy.DoLogic(deltaTime);
			}
		}
		foreach (ProtectedItem value in mProtectedItemDictionary.Values)
		{
			if (value != null)
			{
				value.Update();
			}
		}
		if (bossLeveStartFadeTime > 0f)
		{
			if (Time.time - bossLeveStartFadeTime > 1f)
			{
				mLocalPlayer.ActivatePlayer(true);
			}
			if (Time.time - bossLeveStartFadeTime > 2f)
			{
				mLocalPlayer.EnableTeleportEffect(false);
				foreach (RemotePlayer remotePlayer2 in remotePlayerList)
				{
					if (remotePlayer2 != null)
					{
						remotePlayer2.EnableTeleportEffect(false);
					}
				}
				bossLeveStartFadeTime = -1f;
			}
		}
		foreach (Robot mRobot in mRobotList)
		{
			mRobot.Loop(deltaTime);
		}
		for (int j = 0; j < 8; j++)
		{
			if (enemyBodyPool[j] != null)
			{
				enemyBodyPool[j].AutoDestruct();
			}
		}
		hitBloodPool.AutoDestruct();
		for (int k = 0; k < mLootEffectPool.Length; k++)
		{
			if (mLootEffectPool[k] != null)
			{
				mLootEffectPool[k].AutoDestruct();
			}
		}
		for (int l = 0; l < 5; l++)
		{
			if (mEnemyEffectPool[l] != null)
			{
				mEnemyEffectPool[l].AutoDestruct();
			}
		}
		if (GameApp.GetInstance().GetGameMode().IsCatchTheFlagMode())
		{
			if (mLocalPlayer != null && FlagDirObj != null)
			{
				FlagDirObj.transform.localPosition = mLocalPlayer.GetTransform().position + Vector3.up * 2.6f;
				if (mFlagObject != null)
				{
					FlagDirObj.SetActive(true);
					FlagDirObj.transform.LookAt(mFlagObject.transform.position + Vector3.up);
				}
				else if (FlagInPlayerID == mLocalPlayer.GetChannelID())
				{
					FlagDirObj.SetActive(false);
				}
				else
				{
					RemotePlayer remotePlayerByUserID = GetRemotePlayerByUserID(FlagInPlayerID);
					if (remotePlayerByUserID != null)
					{
						FlagDirObj.SetActive(true);
						FlagDirObj.transform.LookAt(remotePlayerByUserID.GetTransform());
					}
					else
					{
						FlagDirObj.SetActive(false);
					}
				}
			}
			if (Lobby.GetInstance().IsMasterPlayer && FlagInPlayerID == -1 && mFlagObject == null)
			{
				if (flagMissTime == 0f)
				{
					flagMissTime = Time.time;
				}
				else if (Time.time - flagMissTime > 20f)
				{
					GameObject[] array2 = GameObject.FindGameObjectsWithTag(TagName.FLAG_RESPAWN);
					int num = UnityEngine.Random.Range(0, array2.Length);
					DropTheFlagRequest request = new DropTheFlagRequest(-1, array2[num].transform.position + Vector3.up * 0f, true);
					GameApp.GetInstance().GetNetworkManager().SendRequest(request);
					flagMissTime = 0f;
				}
			}
		}
		FogRender.GetInstance().Update();
		PostProcessRender.GetInstance().Update();
		if (mBossCoop != null)
		{
			mBossCoop.Update();
		}
		if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Domination && BattleInfo != null && BattleInfo.GetEnemyTeamScore() - BattleInfo.GetMyTeamScore() >= 500)
		{
			GameApp.GetInstance().GetUserState().Achievement.C3PWinBack = true;
		}
	}

	public void LateLoop(float deltaTime)
	{
		if (mLocalPlayer != null && !mLocalPlayer.hasBeenDestroyed())
		{
			mLocalPlayer.AdjustAnimationInLateUpdate(deltaTime);
		}
		foreach (RemotePlayer remotePlayer in remotePlayerList)
		{
			if (remotePlayer != null && !remotePlayer.hasBeenDestroyed())
			{
				if (remotePlayer.mVehicleState == EPlayerVehicleState.CANNON)
				{
					remotePlayer.GetInterpolation().Loop();
				}
				remotePlayer.AdjustAnimationInLateUpdate(deltaTime);
			}
		}
		foreach (Robot mRobot in mRobotList)
		{
			if (!mRobot.hasBeenDestroyed())
			{
				mRobot.AdjustAnimationInLateUpdate(deltaTime);
			}
		}
	}

	public void SpawnItem(LootType lootType, Vector3 pos, int amount, short sequenceID)
	{
		float num = Global.FLOORHEIGHT;
		Ray ray = new Ray(pos + new Vector3(0f, 0.5f, 0f), Vector3.down);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 10f, 1 << PhysicsLayer.FLOOR))
		{
			num = hitInfo.point.y;
		}
		GameObject original = Resources.Load("_Old/_Loot/Money") as GameObject;
		GameObject gameObject = null;
		Debug.Log("lootType = " + lootType);
		float num2 = num;
		if (lootType == LootType.MONEY)
		{
			gameObject = UnityEngine.Object.Instantiate(original, new Vector3(pos.x, num2 + 1f, pos.z), Quaternion.identity) as GameObject;
			gameObject.GetComponent<ItemScript>().itemType = lootType;
			gameObject.GetComponent<ItemScript>().LowPos = num2 + 1.2f;
			gameObject.GetComponent<ItemScript>().HighPos = num2 + 1.5f;
			gameObject.GetComponent<ItemScript>().Param1 = amount;
			gameObject.GetComponent<ItemScript>().sequenceID = sequenceID;
		}
		if (gameObject != null)
		{
			gameObject.name = "Loot_" + sequenceID;
			gameObject.tag = TagName.ITEM;
			GameObject original2 = Resources.Load("_Old/_Loot/Halo") as GameObject;
			GameObject gameObject2 = UnityEngine.Object.Instantiate(original2, gameObject.transform.position, Quaternion.Euler(0f, 0f, 0f)) as GameObject;
			gameObject2.AddComponent<LookAtCameraScript>();
			gameObject2.transform.localScale = Vector3.one * 0.4f;
			gameObject2.transform.parent = gameObject.transform;
		}
	}

	public void PlayerPickUpLoot(short sequeneceId, int playerId)
	{
		GameObject gameObject = null;
		string objectName = "Loot_" + sequeneceId;
		for (int i = 0; i < mMoneyPickUpPool.Length; i++)
		{
			gameObject = mMoneyPickUpPool[i].FindObjectByName(objectName);
			if (gameObject != null)
			{
				break;
			}
		}
		if (gameObject == null)
		{
			for (int j = 0; j < mBattlePickUpPool.Length; j++)
			{
				gameObject = mBattlePickUpPool[j].FindObjectByName(objectName);
				if (gameObject != null)
				{
					break;
				}
			}
		}
		if (gameObject == null)
		{
			for (int k = 0; k < mVehiclePickUpPool.Length; k++)
			{
				gameObject = mVehiclePickUpPool[k].FindObjectByName(objectName);
				if (gameObject != null)
				{
					break;
				}
			}
		}
		if (!(gameObject != null))
		{
			return;
		}
		ItemScript component = gameObject.GetComponent<ItemScript>();
		if (component != null)
		{
			if (playerId == GetLocalPlayer().GetChannelID())
			{
				component = gameObject.GetComponent<ItemScript>();
				GetLocalPlayer().OnPickUp(component.itemType, component.Param1, component.Param2);
			}
			else
			{
				RemotePlayer remotePlayerByUserID = GetRemotePlayerByUserID(playerId);
				if (remotePlayerByUserID != null)
				{
					remotePlayerByUserID.OnPickUp(component.itemType, component.Param1, component.Param2);
					switch (component.itemType)
					{
					case LootType.VEHICLE:
						if (remotePlayerByUserID.CanSpawnVehicle())
						{
							SpawnVehicle(remotePlayerByUserID, (EVehicleType)component.Param1);
						}
						break;
					case LootType.PICKUP:
					{
						remotePlayerByUserID.mBattlePickupManager.PickUp((byte)component.Param1);
						float skill = remotePlayerByUserID.GetSkills().GetSkill(SkillsType.SHARE_RATE);
						if (!(skill > 0f))
						{
							break;
						}
						List<RemotePlayer> remotePlayers = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
						foreach (RemotePlayer item in remotePlayers)
						{
							if (remotePlayerByUserID.IsFriendForce(item) && item.InPlayingState() && BattlePickup.GetPickupType((int)component.Param1) == BattlePickup.EPickupType.PICKUP)
							{
								item.mBattlePickupManager.SharePickUp((byte)component.Param1, skill);
							}
						}
						if (mLocalPlayer.IsFriendForce(remotePlayerByUserID) && mLocalPlayer.InPlayingState() && BattlePickup.GetPickupType((int)component.Param1) == BattlePickup.EPickupType.PICKUP)
						{
							mLocalPlayer.mBattlePickupManager.SharePickUp((byte)component.Param1, skill);
						}
						break;
					}
					}
				}
			}
			if (component.GetSpawner() != null)
			{
				component.GetSpawner().ItemPickUp();
			}
		}
		component.Destroy();
	}

	public void VSTimeStopResume()
	{
		if (vsClock != null && GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.VS_FFA)
			{
				if (remotePlayerList.Count == 0)
				{
					vsClock.StopTime();
				}
				else
				{
					vsClock.ResumeTime();
				}
			}
			else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.VS_TDM)
			{
				bool flag = false;
				foreach (RemotePlayer remotePlayer in remotePlayerList)
				{
					if (remotePlayer.Team != mLocalPlayer.Team)
					{
						flag = true;
					}
				}
				if (flag)
				{
					vsClock.ResumeTime();
				}
				else
				{
					vsClock.StopTime();
				}
			}
		}
		if (flagClock != null && GameApp.GetInstance().GetGameMode().IsCatchTheFlagMode())
		{
			if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.VS_CTF_FFA)
			{
				if (remotePlayerList.Count == 0)
				{
					flagClock.StopTime();
				}
				else if (FlagInPlayerID != -1)
				{
					flagClock.ResumeTime();
				}
			}
			else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.VS_CTF_TDM)
			{
				bool flag2 = false;
				foreach (RemotePlayer remotePlayer2 in remotePlayerList)
				{
					if (remotePlayer2.Team != mLocalPlayer.Team)
					{
						flag2 = true;
					}
				}
				if (flag2)
				{
					if (FlagInPlayerID != -1)
					{
						flagClock.ResumeTime();
					}
				}
				else
				{
					flagClock.StopTime();
				}
			}
		}
		if (GameApp.GetInstance().GetGameMode().ModePlay != Mode.VS_KOTH && GameApp.GetInstance().GetGameMode().ModePlay != Mode.Domination)
		{
			return;
		}
		bool isTimeRun = false;
		foreach (RemotePlayer remotePlayer3 in remotePlayerList)
		{
			if (remotePlayer3.Team != mLocalPlayer.Team)
			{
				isTimeRun = true;
			}
		}
		if (mVsModeLogic != null)
		{
			mVsModeLogic.isTimeRun = isTimeRun;
		}
	}

	public bool RemotePlayerExists(int channelID)
	{
		RemotePlayer remotePlayerByUserID = GetRemotePlayerByUserID(channelID);
		if (remotePlayerByUserID != null)
		{
			return true;
		}
		return false;
	}

	public void SubmitBattleScores()
	{
		if (!GameApp.GetInstance().GetGameMode().IsCoopMode() && GameApp.GetInstance().GetGameMode().IsVSMode() && GameApp.GetInstance().GetGameMode().ModePlay != Mode.VS_TDM && GameApp.GetInstance().GetGameMode().ModePlay != Mode.VS_FFA)
		{
		}
	}

	public Player GetPlayerByUserID(int playerID)
	{
		int channelID = Lobby.GetInstance().GetChannelID();
		if (playerID == channelID)
		{
			return mLocalPlayer;
		}
		return GetRemotePlayerByUserID(playerID);
	}

	public Player GetPlayerBySeatID(int seatID)
	{
		if (mLocalPlayer.GetSeatID() == seatID)
		{
			return mLocalPlayer;
		}
		foreach (RemotePlayer remotePlayer in remotePlayerList)
		{
			if (remotePlayer.GetSeatID() == seatID)
			{
				return remotePlayer;
			}
		}
		Debug.Log("GetPlayerBySeatID return null with: " + seatID);
		return null;
	}

	public Vehicle GenerateVirtualVehicle(EVehicleType vType, GameObject entityObject)
	{
		Vehicle vehicle = null;
		if (vType == EVehicleType.LONG_SWORD_CANNON)
		{
			vehicle = new VehicleLongSwordCannon();
		}
		if (vehicle != null)
		{
			vehicle.Init(entityObject);
		}
		return vehicle;
	}

	public Vehicle SpawnVehicle(EVehicleType vType)
	{
		Vehicle vehicle = null;
		switch (vType)
		{
		case EVehicleType.UNCC_TURRET:
			vehicle = new VehicleUnccTurret();
			break;
		case EVehicleType.RPG_TURRET:
			vehicle = new RPGTurret();
			break;
		case EVehicleType.ATV_MINI:
			vehicle = new VehicleATVMini();
			break;
		case EVehicleType.ATV_LASER:
			vehicle = new VehicleATVLaser();
			break;
		case EVehicleType.UNCC_TURRET_FOR_MISSION:
			vehicle = new VehicleUnccTurretForMission();
			break;
		case EVehicleType.LONG_SWORD:
			vehicle = new VehicleLongSword();
			break;
		case EVehicleType.LONG_SWORD_CANNON:
			vehicle = new VehicleLongSwordCannon();
			break;
		}
		if (!mVehiclePool[(int)vType].IsInitialized())
		{
			switch (vType)
			{
			case EVehicleType.UNCC_TURRET:
			{
				GameObject prefab = Resources.Load("Character/Vehicle/UNCC_Turret") as GameObject;
				mVehiclePool[0].Init(EVehicleType.UNCC_TURRET.ToString(), prefab, 1, 1f);
				break;
			}
			case EVehicleType.RPG_TURRET:
			{
				GameObject prefab = Resources.Load("Character/Vehicle/UNCC_Turret2") as GameObject;
				mVehiclePool[1].Init(EVehicleType.RPG_TURRET.ToString(), prefab, 1, 1f);
				break;
			}
			case EVehicleType.ATV_MINI:
			{
				GameObject prefab = Resources.Load("Character/Vehicle/ATV_mini") as GameObject;
				mVehiclePool[2].Init(EVehicleType.ATV_MINI.ToString(), prefab, 1, 1f);
				break;
			}
			case EVehicleType.ATV_LASER:
			{
				GameObject prefab = Resources.Load("Character/Vehicle/ATV_mini2") as GameObject;
				mVehiclePool[3].Init(EVehicleType.ATV_LASER.ToString(), prefab, 1, 1f);
				break;
			}
			case EVehicleType.UNCC_TURRET_FOR_MISSION:
			{
				GameObject prefab = Resources.Load("Character/Vehicle/UNCC_Turret") as GameObject;
				mVehiclePool[4].Init(EVehicleType.UNCC_TURRET_FOR_MISSION.ToString(), prefab, 1, 1f);
				break;
			}
			case EVehicleType.LONG_SWORD:
			{
				GameObject prefab = Resources.Load("Character/Vehicle/Long_Sword") as GameObject;
				mVehiclePool[5].Init(EVehicleType.LONG_SWORD.ToString(), prefab, 1, 1f);
				break;
			}
			case EVehicleType.LONG_SWORD_CANNON:
			{
				GameObject prefab = Resources.Load("Character/Vehicle/Long_Sword") as GameObject;
				mVehiclePool[6].Init(EVehicleType.LONG_SWORD_CANNON.ToString(), prefab, 1, 1f);
				break;
			}
			}
			GameObject root = GameObject.Find("_Vehicle");
			mVehiclePool[(int)vType].SetRoot(root);
		}
		GameObject gObject = mVehiclePool[(int)vType].CreateObject(Vector3.zero, Vector3.zero, Quaternion.identity);
		if (vehicle != null)
		{
			vehicle.Init(gObject);
		}
		return vehicle;
	}

	public void SpawnVehicle(Player player, EVehicleType type)
	{
		Vehicle vehicle = SpawnVehicle(type);
		if (vehicle != null)
		{
			vehicle.Open(player);
		}
	}

	public void TrySpawnSwVoid(Player player)
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		if (userState.swVoidList.Count > 0 && userState.SwVoidIdInUsing != 0)
		{
			SwVoidSavedata swVoidSavedata = userState.swVoidList[userState.SwVoidIdInUsing];
			if (GameApp.GetInstance().GetGameMode().IsSingle())
			{
				SpawnSwVoid(player, swVoidSavedata);
				return;
			}
			SwVoidSpawnRequest request = new SwVoidSpawnRequest(swVoidSavedata);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
	}

	public void SpawnLocalSwVoid()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		if (userState.swVoidList.Count > 0 && userState.SwVoidIdInUsing != 0)
		{
			SwVoidSavedata swVoidSavedata = userState.swVoidList[userState.SwVoidIdInUsing];
			SpawnSwVoid(GetLocalPlayer(), swVoidSavedata);
		}
	}

	public void SpawnSwVoid(Player player, SwVoidSavedata swVoidSavedata)
	{
		SwVoid swVoid;
		GameObject original;
		switch (swVoidSavedata.GetType())
		{
		case ESwVoidType.SOLDIER:
			swVoid = new SwVoidSoldier();
			original = (swVoidSavedata.IsHighLevel() ? (Resources.Load("Character/SwVoid/" + Global.QUALITY_LEVEL_1 + "/V1_Nirvana_02") as GameObject) : (Resources.Load("Character/SwVoid/" + Global.QUALITY_LEVEL_1 + "/V1_Nirvana_01") as GameObject));
			break;
		case ESwVoidType.MEDIC:
			swVoid = new SwVoidMedic();
			original = (swVoidSavedata.IsHighLevel() ? (Resources.Load("Character/SwVoid/" + Global.QUALITY_LEVEL_1 + "/V3_Hygeio_02") as GameObject) : (Resources.Load("Character/SwVoid/" + Global.QUALITY_LEVEL_1 + "/V3_Hygeio_01") as GameObject));
			break;
		case ESwVoidType.SPY:
			swVoid = new SwVoidSpy();
			original = Resources.Load("Character/SwVoid/" + Global.QUALITY_LEVEL_1 + "/V1_NirvanaSpy") as GameObject;
			break;
		case ESwVoidType.SCOUT:
			swVoid = new SwVoidScout();
			original = (swVoidSavedata.IsHighLevel() ? (Resources.Load("Character/SwVoid/" + Global.QUALITY_LEVEL_1 + "/V2_Twilight_02") as GameObject) : (Resources.Load("Character/SwVoid/" + Global.QUALITY_LEVEL_1 + "/V2_Twilight_01") as GameObject));
			break;
		case ESwVoidType.HEAVY:
			swVoid = new SwVoidHeavy();
			original = (swVoidSavedata.IsHighLevel() ? (Resources.Load("Character/SwVoid/" + Global.QUALITY_LEVEL_1 + "/V5_Catalyst_02") as GameObject) : (Resources.Load("Character/SwVoid/" + Global.QUALITY_LEVEL_1 + "/V5_Catalyst_01") as GameObject));
			break;
		case ESwVoidType.DEMOMAN:
			swVoid = new SwVoidDemoman();
			original = (swVoidSavedata.IsHighLevel() ? (Resources.Load("Character/SwVoid/" + Global.QUALITY_LEVEL_1 + "/V4_Collapsar_02") as GameObject) : (Resources.Load("Character/SwVoid/" + Global.QUALITY_LEVEL_1 + "/V4_Collapsar_01") as GameObject));
			break;
		default:
			swVoid = new SwVoidSoldier();
			original = Resources.Load("_Test/SwVoid0") as GameObject;
			break;
		}
		GameObject gObject = UnityEngine.Object.Instantiate(original, player.GetTransform().position, Quaternion.Euler(0f, 0f, 0f)) as GameObject;
		if (player.mSwVoid != null)
		{
			player.mSwVoid.Destroy();
		}
		swVoid.SetSavedata(swVoidSavedata);
		swVoid.init(gObject, player);
		if (null != HUDBase.instance && null != HUDBase.instance.m_SkillHUDManager)
		{
			HUDBase.instance.m_SkillHUDManager.RefreshSkillButtons();
		}
	}

	public GameObject CreatePickUp(LootType m_LootType, Vector3 position, float Param1, float Param2, short sequenceID, DynamicPickUpScript pickupPoint = null)
	{
		if (State != GameState.Playing)
		{
			return null;
		}
		if (m_LootType == LootType.NONE)
		{
			return null;
		}
		if (mMoneyPickUpPool == null || mVehiclePickUpPool == null || mBattlePickUpPool == null)
		{
			InitPickUpPool();
		}
		float num = Global.FLOORHEIGHT;
		Ray ray = new Ray(position + new Vector3(0f, 0.5f, 0f), Vector3.down);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 10f, 1 << PhysicsLayer.FLOOR))
		{
			num = hitInfo.point.y;
		}
		Vector3 position2 = new Vector3(position.x, num + 1f, position.z);
		Vector3 position3 = new Vector3(position.x, num, position.z);
		GameObject support = mPickupSupportPool[0].CreateObject(position3, Vector3.zero, Quaternion.identity);
		GameObject gameObject = null;
		switch (m_LootType)
		{
		case LootType.MONEY:
			gameObject = mMoneyPickUpPool[0].CreateObject(position2, Vector3.zero, Quaternion.identity);
			break;
		case LootType.VEHICLE:
			if ((int)Param1 < mVehiclePickUpPool.Length)
			{
				gameObject = mVehiclePickUpPool[(int)Param1].CreateObject(position2, Vector3.zero, Quaternion.identity);
			}
			else
			{
				Debug.LogError(string.Concat("Invalid Pickup Info: ", m_LootType, Param1));
			}
			break;
		case LootType.PICKUP:
			if ((int)Param1 - 1 < mBattlePickUpPool.Length)
			{
				gameObject = mBattlePickUpPool[(int)Param1 - 1].CreateObject(position2, Vector3.zero, Quaternion.identity);
			}
			else
			{
				Debug.LogError(string.Concat("Invalid Pickup Info: ", m_LootType, Param1));
			}
			break;
		case LootType.NONE:
			return gameObject;
		}
		if (null != gameObject)
		{
			gameObject.GetComponent<ItemScript>().itemType = m_LootType;
			gameObject.GetComponent<ItemScript>().Param1 = Param1;
			gameObject.GetComponent<ItemScript>().Param2 = Param2;
			gameObject.GetComponent<ItemScript>().SetSpawner(pickupPoint);
			gameObject.GetComponent<ItemScript>().sequenceID = sequenceID;
			gameObject.GetComponent<ItemScript>().LowPos = num + 1.2f;
			gameObject.GetComponent<ItemScript>().HighPos = num + 1.5f;
			gameObject.GetComponent<ItemScript>().support = support;
			gameObject.name = "Loot_" + sequenceID;
			gameObject.tag = TagName.ITEM;
			if (m_LootType == LootType.VEHICLE)
			{
			}
			GameApp.GetInstance().GetGameWorld().AddItem(gameObject.GetComponent<ItemScript>());
		}
		else
		{
			Debug.LogError("Prefab " + m_LootType.ToString() + Param1 + " missed!");
		}
		return gameObject;
	}

	public static GameObject TryCreatePickUp(LootType m_LootType, Vector3 position, float Param1, float Param2, DynamicPickUpScript pickupPoint = null)
	{
		if (GameApp.GetInstance().GetGameMode().IsSingle())
		{
			GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
			if (gameWorld != null)
			{
				return gameWorld.CreatePickUp(m_LootType, position, Param1, Param2, 0, pickupPoint);
			}
			return null;
		}
		ItemSpawnRequest request = new ItemSpawnRequest((byte)m_LootType, position, (short)Param1, (short)Param2);
		GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		return null;
	}

	public void AddRadarPoint(RadarPoint radarPoint)
	{
		if (radarPoint != null && !mRadarPointList.Contains(radarPoint))
		{
			mRadarPointList.Add(radarPoint);
		}
	}

	public void AddRadarPoint(string tag, string key, Transform transform)
	{
		RadarPoint radarPoint = new RadarPoint();
		radarPoint.Tag = tag;
		radarPoint.Key = key;
		radarPoint.Transform = transform;
		AddRadarPoint(radarPoint);
	}

	public void AddRadarPoint(string tag, string key, Transform transform, bool NeedArrow)
	{
		RadarPoint radarPoint = new RadarPoint();
		radarPoint.Tag = tag;
		radarPoint.Key = key;
		radarPoint.Transform = transform;
		radarPoint.NeedArrow = NeedArrow;
		AddRadarPoint(radarPoint);
	}

	public void RemoveRadarPoint(RadarPoint radarPoint)
	{
		if (radarPoint != null && mRadarPointList.Contains(radarPoint))
		{
			RemoveIndication(radarPoint.Key);
			mRadarPointList.Remove(radarPoint);
		}
	}

	public void RemoveRadarPointByKey(string key)
	{
		RadarPoint radarPoint = null;
		foreach (RadarPoint mRadarPoint in mRadarPointList)
		{
			if (key.Equals(mRadarPoint.Key))
			{
				radarPoint = mRadarPoint;
				break;
			}
		}
		if (radarPoint != null)
		{
			RemoveIndication(radarPoint.Key);
		}
		mRadarPointList.Remove(radarPoint);
	}

	public void RemoveRadarPointByTransform(Transform targetTransform)
	{
		if (!(targetTransform != null))
		{
			return;
		}
		RadarPoint radarPoint = null;
		foreach (RadarPoint mRadarPoint in mRadarPointList)
		{
			if (mRadarPoint.Transform.Equals(targetTransform))
			{
				radarPoint = mRadarPoint;
				break;
			}
		}
		if (radarPoint != null)
		{
			RemoveIndication(radarPoint.Key);
		}
		mRadarPointList.Remove(radarPoint);
	}

	private void RemoveIndication(string key)
	{
		if (mIndicationEffectDic.ContainsKey(key))
		{
			mIndicationEffectDic[key].SelfDestroy();
			mIndicationEffectDic.Remove(key);
		}
		if (mRadarEffectDic.ContainsKey(key))
		{
			if (mRadarEffectDic[key] != null)
			{
				mRadarEffectDic[key].SelfDestroy();
			}
			mRadarEffectDic.Remove(key);
		}
	}

	public void RemoveAllRadarPoints()
	{
		mRadarPointList.Clear();
	}

	public List<RadarPoint> GetRadarPointList()
	{
		return mRadarPointList;
	}

	public static GameUnit FindEnemyTarget(GameUnit gameUnit, bool isFront, float distance = 0f, float angle = 0f, bool isParallel = false)
	{
		GameUnit gameUnit2 = null;
		float num = 9999999f;
		float num2 = distance * distance;
		if (GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			foreach (RemotePlayer remotePlayer in GameApp.GetInstance().GetGameWorld().GetRemotePlayers())
			{
				if (remotePlayer.IsSameTeam(localPlayer))
				{
					continue;
				}
				float num3 = Vector3.SqrMagnitude(gameUnit.GetTransform().position - remotePlayer.GetTransform().position);
				if (!(num3 < num))
				{
					continue;
				}
				if (!isFront)
				{
					if ((distance < float.Epsilon || num3 < num2) && (!isParallel || Mathf.Abs(gameUnit.GetTransform().position.y - remotePlayer.GetTransform().position.y) < 0.5f))
					{
						num = num3;
						gameUnit2 = remotePlayer;
					}
				}
				else if (CheckEnemyInRange(gameUnit, remotePlayer.GetTransform().position, distance, angle, isParallel))
				{
					num = num3;
					gameUnit2 = remotePlayer;
				}
			}
		}
		else
		{
			Hashtable enemies = GameApp.GetInstance().GetGameWorld().GetEnemies();
			foreach (DictionaryEntry item in enemies)
			{
				Enemy enemy = item.Value as Enemy;
				float num4 = Vector3.SqrMagnitude(gameUnit.GetTransform().position - enemy.GetTransform().position);
				if (!(num4 < num))
				{
					continue;
				}
				if (!isFront)
				{
					if ((distance < float.Epsilon || num4 < num2) && (!isParallel || Mathf.Abs(gameUnit.GetTransform().position.y - enemy.GetTransform().position.y) < 0.5f) && enemy.UnitCanBeHitByRay(gameUnit.GetPosition() + Vector3.up, 100f))
					{
						num = num4;
						gameUnit2 = enemy;
					}
				}
				else if (CheckEnemyInRange(gameUnit, enemy.GetTransform().position, distance, angle, isParallel) && enemy.UnitCanBeHitByRay(gameUnit.GetPosition() + Vector3.up, 100f))
				{
					num = num4;
					gameUnit2 = enemy;
				}
			}
		}
		ResultTarget = gameUnit2;
		return gameUnit2;
	}

	public static GameUnit FindEnemyTarget(GameObject gameObject, float distance = 0f, float angle = 0f, bool isParallel = false)
	{
		GameUnit gameUnit = null;
		float num = 9999999f;
		float num2 = distance * distance;
		if (GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			foreach (RemotePlayer remotePlayer in GameApp.GetInstance().GetGameWorld().GetRemotePlayers())
			{
				if (!remotePlayer.IsSameTeam(localPlayer))
				{
					float num3 = Vector3.SqrMagnitude(gameObject.transform.position - remotePlayer.GetTransform().position);
					if (num3 < num)
					{
						num = num3;
						gameUnit = remotePlayer;
					}
				}
			}
		}
		else
		{
			Hashtable enemies = GameApp.GetInstance().GetGameWorld().GetEnemies();
			foreach (DictionaryEntry item in enemies)
			{
				Enemy enemy = item.Value as Enemy;
				float num4 = Vector3.SqrMagnitude(gameObject.transform.position - enemy.GetTransform().position);
				if (num4 < num && (distance < float.Epsilon || num4 < num2) && (!isParallel || Mathf.Abs(gameObject.transform.position.y - enemy.GetTransform().position.y) < 0.5f) && enemy.UnitCanBeHitByRay(gameObject.transform.position + Vector3.up, 100f))
				{
					num = num4;
					gameUnit = enemy;
				}
			}
		}
		ResultTarget = gameUnit;
		return gameUnit;
	}

	public static GameUnit[] FindEnemyTargetList(GameUnit gameUnit, bool isFront, float distance, float angle = 0f, bool isParallel = false)
	{
		mTempGameUnitList.Clear();
		Hashtable enemies = GameApp.GetInstance().GetGameWorld().GetEnemies();
		float num = distance * distance;
		foreach (DictionaryEntry item in enemies)
		{
			Enemy enemy = item.Value as Enemy;
			float num2 = Vector3.SqrMagnitude(gameUnit.GetTransform().position - enemy.GetTransform().position);
			if (num2 < distance)
			{
				if (!isFront)
				{
					mTempGameUnitList.Add(enemy);
				}
				else if (CheckEnemyInRange(gameUnit, enemy.GetTransform().position, distance, angle, isParallel))
				{
					mTempGameUnitList.Add(enemy);
				}
			}
		}
		if (mTempGameUnitList.Count > 0)
		{
			return mTempGameUnitList.ToArray();
		}
		return null;
	}

	public static bool CheckEnemyInRange(GameUnit gameUnit, Vector3 enemyPosition, float frontDistance, float angle, bool isParallel = false)
	{
		bool result = false;
		Vector3 to = enemyPosition - gameUnit.GetTransform().position;
		to.y = 0f;
		if (to.sqrMagnitude < frontDistance * frontDistance)
		{
			Vector3 vector = ((!(angle > 0f)) ? (-gameUnit.GetTransform().forward) : gameUnit.GetTransform().forward);
			vector.y = 0f;
			if (Vector3.Angle(vector, to) < Mathf.Abs(angle))
			{
				result = !isParallel || !(Mathf.Abs(gameUnit.GetTransform().position.y - enemyPosition.y) < 0.5f) || true;
			}
		}
		return result;
	}

	public void AddItem(ItemScript newItem)
	{
		if (itemInSceneList == null)
		{
			itemInSceneList = new List<ItemScript>();
		}
		itemInSceneList.Add(newItem);
	}

	public void RemoveItem(ItemScript item)
	{
		if (itemInSceneList == null)
		{
			itemInSceneList = new List<ItemScript>();
		}
		else if (itemInSceneList.Contains(item))
		{
			itemInSceneList.Remove(item);
		}
	}

	public List<ItemScript> GetItemList()
	{
		return itemInSceneList;
	}

	public Robot CreateRobot()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		OnlineCustomizeData coopCustomize = new OnlineCustomizeData(userState.GetCoopCustomize());
		Robot robot = new Robot();
		robot.CreateUserState(coopCustomize);
		robot.GetUserState().LoadCurrentCustomize(true, 0);
		robot.Init();
		robot.GetTransform().position = GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.GetPosition() + GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.GetTransform()
			.TransformDirection(new Vector3(2f, 0f, 2f));
		GameApp.GetInstance().GetGameWorld().AddRobot(robot);
		robot.CreateNavMeshAgent();
		return robot;
	}

	public Robot CreateRobot(OnlineFriend friend)
	{
		OnlineCustomizeData coopCustomize = new OnlineCustomizeData(friend.GetCustomize());
		Robot robot = new Robot();
		robot.CreateUserState(coopCustomize);
		robot.GetUserState().LoadCurrentCustomize(true, 0);
		robot.Init();
		robot.GetTransform().position = GetSafePositionAroundLocalPlayer();
		GameApp.GetInstance().GetGameWorld().AddRobot(robot);
		robot.CreateNavMeshAgent();
		return robot;
	}

	public Robot CreateRobot(ERobotType type, Vector3 SpawnPosition, Quaternion SpawnRotation)
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		OnlineCustomizeData onlineCustomizeData = new OnlineCustomizeData();
		if (GameApp.GetInstance().GetUserState().GetSelectedStageIndex() < 4)
		{
			switch (type)
			{
			case ERobotType.PROTECTED_NPC:
				onlineCustomizeData.mWeaponIdArray = new short[1] { 17 };
				break;
			case ERobotType.DEFENDER:
				onlineCustomizeData.mWeaponIdArray = new short[1] { 17 };
				break;
			}
			onlineCustomizeData.mAvatarIdArray = new short[5] { 6, 6, 6, 6, 3 };
		}
		else
		{
			switch (type)
			{
			case ERobotType.PROTECTED_NPC:
				onlineCustomizeData.mWeaponIdArray = new short[3] { 8, 9, 27 };
				break;
			case ERobotType.DEFENDER:
				onlineCustomizeData.mWeaponIdArray = new short[3] { 8, 9, 27 };
				break;
			}
			onlineCustomizeData.mAvatarIdArray = new short[5] { 9, 9, 9, 9, 3 };
		}
		Robot robot = new Robot(type);
		robot.CreateUserState(onlineCustomizeData);
		robot.GetUserState().LoadCurrentCustomize(true, 0);
		robot.Init();
		robot.GetTransform().position = SpawnPosition;
		robot.GetTransform().rotation = SpawnRotation;
		GameApp.GetInstance().GetGameWorld().AddRobot(robot);
		robot.CreateNavMeshAgent();
		return robot;
	}

	public Robot CreateRobot(ERobotType type)
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		OnlineCustomizeData onlineCustomizeData = new OnlineCustomizeData();
		if (GameApp.GetInstance().GetUserState().GetSelectedStageIndex() < 4)
		{
			switch (type)
			{
			case ERobotType.PROTECTED_NPC:
				onlineCustomizeData.mWeaponIdArray = new short[1] { 17 };
				break;
			case ERobotType.DEFENDER:
				onlineCustomizeData.mWeaponIdArray = new short[1] { 17 };
				break;
			}
			onlineCustomizeData.mAvatarIdArray = new short[5] { 6, 6, 6, 6, 3 };
		}
		else
		{
			switch (type)
			{
			case ERobotType.PROTECTED_NPC:
				onlineCustomizeData.mWeaponIdArray = new short[3] { 8, 9, 27 };
				break;
			case ERobotType.DEFENDER:
				onlineCustomizeData.mWeaponIdArray = new short[3] { 8, 9, 27 };
				break;
			}
			onlineCustomizeData.mAvatarIdArray = new short[5] { 9, 9, 9, 9, 3 };
		}
		Robot robot = new Robot(type);
		robot.CreateUserState(onlineCustomizeData);
		robot.GetUserState().LoadCurrentCustomize(true, 0);
		robot.Init();
		robot.GetTransform().position = GetSafePositionAroundLocalPlayer();
		GameApp.GetInstance().GetGameWorld().AddRobot(robot);
		robot.CreateNavMeshAgent();
		return robot;
	}

	public static int GetRandomIndex(float[] nums)
	{
		float num = 0f;
		foreach (float num2 in nums)
		{
			num += num2;
		}
		float num3 = UnityEngine.Random.Range(0f, num);
		for (int j = 0; j < nums.Length; j++)
		{
			if (num3 <= nums[j])
			{
				return j;
			}
			num3 -= nums[j];
		}
		Debug.Log("not included in every one.");
		return -1;
	}

	public BossCoop GetBossCoop()
	{
		return mBossCoop;
	}

	public void SetBoosCoop(BossCoop bossCoop)
	{
		mBossCoop = bossCoop;
	}

	public bool GetFirstComplete()
	{
		return mMainFirstComplete;
	}

	public void AddIndication(string key, IndicationEffect script)
	{
		mIndicationEffectDic.Add(key, script);
	}

	public void AddRadarEffect(string key, RadarTargetEffect script)
	{
		mRadarEffectDic.Add(key, script);
	}

	public static T GetComponent<T>(GameObject go) where T : Component
	{
		T component = go.GetComponent<T>();
		if (component == null)
		{
			return go.AddComponent<T>();
		}
		return component;
	}

	private void InitVsModeLogic()
	{
		if (GameApp.GetInstance().GetGameMode().IsCatchTheFlagMode())
		{
			flagClock = new VSClock();
			flagClock.SetTotalGameSeconds(40);
			flagClock.Restart();
			GameObject gameObject = Resources.Load("_Old/_UI/flagdir") as GameObject;
			if (gameObject != null)
			{
				FlagDirObj = UnityEngine.Object.Instantiate(gameObject) as GameObject;
				FlagDirObj.transform.localPosition = mLocalPlayer.GetTransform().position;
			}
			else
			{
				FlagDirObj = null;
			}
		}
		else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.VS_KOTH)
		{
			mVsModeLogic = new KOTHModeLogic();
			mVsModeLogic.Init();
		}
		else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Domination)
		{
			mVsModeLogic = new DominationModeLogic();
			mVsModeLogic.Init();
		}
		else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.GIANT_BETRAYAL)
		{
			mVsModeLogic = new GiantBetrayalModeLogic();
			mVsModeLogic.Init();
		}
	}

	public void InitVFXDictionary()
	{
		if (VFXDictionary.Count != 0)
		{
			return;
		}
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SKILL, DataSheetName.SKILL_VFX);
		for (int i = 0; i < gameDataTable.Count - 1; i++)
		{
			VFXInfo vFXInfo = new VFXInfo();
			vFXInfo.vfx1 = gameDataTable.GetString(i, 1);
			vFXInfo.vfx1Position = gameDataTable.GetInt(i, 2);
			vFXInfo.vfx1time = gameDataTable.GetFloat(i, 3);
			vFXInfo.vfx2 = gameDataTable.GetString(i, 4);
			if (vFXInfo.vfx2 != string.Empty)
			{
				vFXInfo.vfx2Position = gameDataTable.GetInt(i, 5);
				vFXInfo.vfx2time = gameDataTable.GetFloat(i, 6);
			}
			VFXDictionary.Add(gameDataTable.GetInt(i, 0), vFXInfo);
		}
	}

	public void PlayVFXOnPlayer(Player player, short skillID)
	{
		if (player.GetTransform() == null || (player.GetObject() != null && player.GetObject().transform.childCount == 0))
		{
			return;
		}
		UnityEngine.Object original = Resources.Load(VFXDictionary[skillID].vfx1);
		GameObject gameObject = null;
		if (VFXDictionary[skillID].vfx1Position == 0)
		{
			if (VFXDictionary[skillID].vfx1time != 0f)
			{
				gameObject = UnityEngine.Object.Instantiate(original, player.GetPosition(), Quaternion.identity) as GameObject;
				gameObject.transform.parent = player.GetTransform();
				gameObject.AddComponent<AutoDestroyScript>().life = VFXDictionary[skillID].vfx1time;
			}
			else if (player.GetTransform().Find(skillID.ToString() + '1') == null)
			{
				gameObject = UnityEngine.Object.Instantiate(original, player.GetPosition(), Quaternion.identity) as GameObject;
				gameObject.transform.parent = player.GetTransform();
				gameObject.name = skillID.ToString() + '1';
			}
		}
		else if (VFXDictionary[skillID].vfx1Position == 1)
		{
			Transform transform = player.GetTransform().Find(BoneName.Weapon);
			if (transform != null)
			{
				if (VFXDictionary[skillID].vfx1time != 0f)
				{
					gameObject = UnityEngine.Object.Instantiate(original, transform.transform.position, Quaternion.identity) as GameObject;
					gameObject.transform.parent = transform;
					gameObject.AddComponent<AutoDestroyScript>().life = VFXDictionary[skillID].vfx1time;
				}
				else if (player.GetTransform().Find(skillID.ToString() + '1') == null)
				{
					gameObject = UnityEngine.Object.Instantiate(original, transform.transform.position, Quaternion.identity) as GameObject;
					gameObject.transform.parent = transform;
					gameObject.name = skillID.ToString() + '1';
				}
			}
		}
		if (!(VFXDictionary[skillID].vfx2 != string.Empty))
		{
			return;
		}
		original = Resources.Load(VFXDictionary[skillID].vfx2);
		if (VFXDictionary[skillID].vfx2Position == 0)
		{
			if (VFXDictionary[skillID].vfx2time != 0f)
			{
				gameObject = UnityEngine.Object.Instantiate(original, player.GetPosition(), Quaternion.identity) as GameObject;
				gameObject.transform.parent = player.GetTransform();
				gameObject.AddComponent<AutoDestroyScript>().life = VFXDictionary[skillID].vfx2time;
			}
			else if (player.GetTransform().Find(skillID.ToString() + '2') == null)
			{
				gameObject = UnityEngine.Object.Instantiate(original, player.GetPosition(), Quaternion.identity) as GameObject;
				gameObject.transform.parent = player.GetTransform();
				gameObject.name = skillID.ToString() + '2';
			}
		}
		else
		{
			if (VFXDictionary[skillID].vfx2Position != 1)
			{
				return;
			}
			Transform transform2 = player.GetTransform().Find(BoneName.Weapon);
			if (transform2 != null)
			{
				if (VFXDictionary[skillID].vfx2time != 0f)
				{
					gameObject = UnityEngine.Object.Instantiate(original, transform2.transform.position, Quaternion.identity) as GameObject;
					gameObject.transform.parent = transform2;
					gameObject.AddComponent<AutoDestroyScript>().life = VFXDictionary[skillID].vfx2time;
				}
				else if (player.GetTransform().Find(skillID.ToString() + '2') == null)
				{
					gameObject = UnityEngine.Object.Instantiate(original, transform2.transform.position, Quaternion.identity) as GameObject;
					gameObject.transform.parent = transform2;
					gameObject.name = skillID.ToString() + '2';
				}
			}
		}
	}

	public void StopVFXOnPlayer(Player player, short skillID)
	{
		if (player.GetTransform() == null || (player.GetObject() != null && player.GetObject().transform.childCount == 0))
		{
			return;
		}
		GameObject gameObject = null;
		if (VFXDictionary[skillID].vfx1time == 0f)
		{
			if (VFXDictionary[skillID].vfx1Position == 0)
			{
				if (player.GetTransform().Find(skillID.ToString() + '1') != null)
				{
					gameObject = player.GetTransform().Find(skillID.ToString() + '1').gameObject;
					UnityEngine.Object.Destroy(gameObject);
				}
				else
				{
					Debug.Log("Can't find skill " + skillID + " on player " + player.GetChannelID());
				}
			}
			else if (VFXDictionary[skillID].vfx1Position == 1)
			{
				Transform transform = player.GetTransform().Find(BoneName.Weapon);
				if (transform != null && transform.Find(skillID.ToString() + '1') != null)
				{
					gameObject = transform.Find(skillID.ToString() + '1').gameObject;
					UnityEngine.Object.Destroy(gameObject);
				}
				else
				{
					Debug.Log("Can't find skill " + skillID + " on player " + player.GetChannelID());
				}
			}
		}
		if (!(VFXDictionary[skillID].vfx2 != string.Empty) || VFXDictionary[skillID].vfx2time != 0f)
		{
			return;
		}
		if (VFXDictionary[skillID].vfx2Position == 0)
		{
			if (player.GetTransform().Find(skillID.ToString() + '2') != null)
			{
				gameObject = player.GetTransform().Find(skillID.ToString() + '2').gameObject;
				UnityEngine.Object.Destroy(gameObject);
			}
			else
			{
				Debug.Log("Can't find skill " + skillID + " on player " + player.GetChannelID());
			}
		}
		else if (VFXDictionary[skillID].vfx2Position == 1)
		{
			Transform transform2 = player.GetTransform().Find(BoneName.Weapon);
			if (transform2 != null && transform2.Find(skillID.ToString() + '2') != null)
			{
				gameObject = transform2.Find(skillID.ToString() + '2').gameObject;
				UnityEngine.Object.Destroy(gameObject);
			}
			else
			{
				Debug.Log("Can't find skill " + skillID + " on player " + player.GetChannelID());
			}
		}
	}

	public static void RefreshHeadFrame(UISprite frame)
	{
		int currentTilteFrameID = GameApp.GetInstance().GetUserState().GetCurrentTilteFrameID();
		frame.spriteName = "head_" + string.Format("{0:00}", currentTilteFrameID);
		RefreshHeadFrameColor(frame);
	}

	public static void RefreshHeadFrameColor(UISprite frame)
	{
		int prestige = GameApp.GetInstance().GetUserState().GetOnlinePlayer()
			.GetPrestige();
		frame.color = Global.HeadFrameColor[prestige];
	}

	public static void RefreshHeadFrame(UISprite frame, int frameType, int prestige)
	{
		frame.spriteName = "head_" + string.Format("{0:00}", frameType);
		frame.color = Global.HeadFrameColor[prestige];
	}

	public static void RefreshHeadFrame(UISprite frame, Player player)
	{
		int prestige = player.GetUserState().GetOnlinePlayer().GetPrestige();
		frame.spriteName = "head_" + string.Format("{0:00}", player.GetUserState().GetCurrentTilteFrameID(), prestige);
		if (prestige >= 0 && prestige < Global.HeadFrameColor.Length)
		{
			frame.color = Global.HeadFrameColor[prestige];
		}
		else
		{
			Debug.Log("prestige wrong: " + prestige);
		}
	}

	public void DisableNavMeshObstacle()
	{
		mLocalPlayer.GetNavMeshObstacle().enabled = false;
		foreach (RemotePlayer remotePlayer in remotePlayerList)
		{
			remotePlayer.GetNavMeshObstacle().enabled = false;
		}
	}

	public ItemScript GetMoneyBySequenceID(int sequeneceId)
	{
		GameObject gameObject = null;
		string objectName = "Loot_" + sequeneceId;
		for (int i = 0; i < mMoneyPickUpPool.Length; i++)
		{
			gameObject = mMoneyPickUpPool[i].FindObjectByName(objectName);
			if (gameObject != null)
			{
				return gameObject.GetComponent<ItemScript>();
			}
		}
		return null;
	}

	private Vector3 GetSafePositionAroundLocalPlayer()
	{
		Vector3[] array = new Vector3[4]
		{
			new Vector3(2f, 0f, 2f),
			new Vector3(-2f, 0f, 2f),
			new Vector3(2f, 0f, -2f),
			new Vector3(-2f, 0f, -2f)
		};
		Vector3[] array2 = array;
		foreach (Vector3 vector in array2)
		{
			Vector3 vector2 = GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.GetPosition() + GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.GetTransform()
				.TransformDirection(vector);
			Ray ray = new Ray(vector2 + Vector3.up * 2f, Vector3.down);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 4f, 1 << PhysicsLayer.FLOOR))
			{
				vector2.y = hitInfo.point.y + 0.1f;
				return vector2;
			}
			Debug.LogWarning("GetSafePositionAroundLocalPlayer No Floor down: " + vector);
		}
		Debug.LogWarning("GetSafePositionAroundLocalPlayer No Floor down in four direction, use localplayer's position.");
		return GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.GetPosition();
	}

	public Vector3 GetPositionOnGround(Vector3 position)
	{
		Ray ray = new Ray(position + Vector3.up * 2f, Vector3.down);
		Vector3 result = position;
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 4f, 1 << PhysicsLayer.FLOOR))
		{
			result = hitInfo.point + new Vector3(0f, 0.1f, 0f);
		}
		else
		{
			Debug.LogError("没有找到地面。");
		}
		return result;
	}

	public void SetGameState(GameState newState)
	{
		mGameState = newState;
	}

	public static void LoadLevel(string name, ELevelType type)
	{
        
        
        NextLevel = name;
		NextLevelType = type;
		Application.LoadLevel("empty");
	}

	public List<ShieldScript> GetCurrentShields()
	{
		return currentActiveShields;
	}

	public void CreatePresentNearPlayer(Vector3 position)
	{
		Ray ray = new Ray(position + Vector3.up, Vector3.down);
		int layerMask = 1 << PhysicsLayer.FLOOR;
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 30f, layerMask))
		{
			int num = UnityEngine.Random.Range(15, 18);
			Debug.Log("Generate Present id " + num);
			TryCreatePickUp(LootType.PICKUP, hitInfo.point, num, 0f);
		}
		else
		{
			CreateARandomItemNearPlayer(BattlePickup.EPickupType.PRESENT);
		}
	}

	public void CreateARandomItemNearPlayer(BattlePickup.EPickupType mType)
	{
		Vector3 vector = default(Vector3);
		float num = 3f;
		Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		if (localPlayer != null && localPlayer.GetTransform() != null)
		{
			vector = localPlayer.GetTransform().position;
		}
		Vector3 vector2;
		Ray ray;
		int layerMask;
		RaycastHit hitInfo;
		do
		{
			vector2 = vector;
			float f = UnityEngine.Random.Range(0f, 360f) * ((float)Math.PI / 180f);
			vector2 = vector + num * new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
			ray = new Ray(vector, localPlayer.GetPosition() - vector);
			layerMask = (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.WALL_AGAINST_PALYER);
		}
		while (Physics.Raycast(ray, out hitInfo, num, layerMask));
		vector = vector2;
		int num2 = 0;
		switch (mType)
		{
		case BattlePickup.EPickupType.PICKUP:
			num2 = UnityEngine.Random.Range(1, 15);
			break;
		case BattlePickup.EPickupType.PRESENT:
			num2 = UnityEngine.Random.Range(15, 18);
			Debug.Log("Generate Present id " + num2);
			break;
		}
		localPlayer.GetUserState().Achievement.AddPickupIDList(num2);
		TryCreatePickUp(LootType.PICKUP, vector, num2, 0f);
	}
}
