using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : GameUnit
{
	public static PlayerState IDLE_STATE = new IdleState();

	public static PlayerState ATTACK_STATE = new AttackState();

	public static PlayerState DEAD_STATE = new PlayerDeadState();

	public static PlayerState KNOCKED_STATE = new PlayerKnockedState();

	public static PlayerState WIN_STATE = new WinState();

	public static PlayerState LOSE_STATE = new PlayerLoseState();

	public static PlayerState WAIT_REBIRTH_STATE = new PlayerWaitRebirthState();

	public static PlayerState WAIT_VS_REBIRTH_STATE = new PlayerWaitVSRebirthState();

	public static PlayerState VEHICLE_IDLE_STATE = new VehicleIdleState();

	public static PlayerState VEHICLE_ATTACK_STATE = new VehicleAttackState();

	public static PlayerState FREEZE_STATE = new PlayerFreezeState();

	public static PlayerState GET_ON_CARRIER_STATE = new PlayerGetOnCarrierState();

	public static PlayerState GET_OFF_CARRIER_STATE = new PlayerGetOffCarrierState();

	public static PlayerState VS_INIT_LOADOUT_STATE = new PlayerVSInitLoadoutState();

	public static PlayerState SWITCH_TO_BOSS_STATE = new SwitchToBossState();

	public static GameUnitState BOSS_IDLE_STATE = new PlayerBossIdleState();

	public static GameUnitState BOSS_JUMP_STATE = new PlayerBossJumpState();

	public static GameUnitState BOSS_DEAD_STATE = new PlayerBossDeadState();

	public static GameUnitState BOSS_CHOOSE_SKILL_STATE = new PlayerBossChooseSkillState();

	public static GameUnitState DEADEYE_MELEE_ATTACK_STATE = new PlayerDeadEyeMeleeAttackState();

	public static GameUnitState DEADEYE_X_LASER_STATE = new PlayerDeadEyeXLaserState();

	public static GameUnitState DEADEYE_4_LASER_STATE = new PlayerDeadEye4LaserState();

	public static GameUnitState DEADEYE_SHOULDER_LASER_STATE = new PlayerDeadEyeShoulderLaserState();

	public static GameUnitState DEADEYE_RUSH_ATTACK_STATE = new PlayerDeadEyeRushAttackState();

	public static GameUnitState SLASHER_WEAPON_ATTACK_STATE01 = new PlayerSlasherWeaponAttackState01();

	public static GameUnitState SLASHER_WEAPON_ATTACK_STATE02 = new PlayerSlasherWeaponAttackState02();

	public static GameUnitState SLASHER_WEAPON_RANGE_STATE = new PlayerSlasherWeaponRangeState();

	public static GameUnitState SLASHER_WEAPON_DASH_STATE = new PlayerSlasherWeaponDashState();

	public static GameUnitState SLASHER_WEAPON_WAVE_STATE = new PlayerSlasherWeaponWaveState();

	public static GameUnitState SLASHER_WEAPON_BLADE_STATE = new PlayerSlasherWeaponBladeState();

	public static GameUnitState FROSTSTONE_MELEE_ATTACK_STATE1 = new PlayerFrostStoneMeleeAttack1();

	public static GameUnitState FROSTSTONE_MELEE_ATTACK_STATE2 = new PlayerFrostStoneMeleeAttack2();

	public static GameUnitState FROSTSTONE_SMALL_SNOW_BALL_STATE = new PlayerFrostStoneSmallSnowBallState();

	public static GameUnitState FROSTSTONE_BIG_SNOW_BALL_STATE = new PlayerFrostStoneBigSnowBallState();

	public static GameUnitState FROSTSTONE_JUMP_ATTACK_STATE = new PlayerFrostStoneJumpAttack();

	public static GameUnitState FROSTSTONE_DASH_ATTACK_STATE = new PlayerFrostStoneDashState();

	public static GameUnitState CERBERLISK_MELEE_ATTACK_STATE1 = new PlayerCerberliskMeleeAttack1();

	public static GameUnitState CERBERLISK_MELEE_ATTACK_STATE2 = new PlayerCerberliskMeleeAttack2();

	public static GameUnitState CERBERLISK_THREE_HIT_ATTACK_STATE = new PlayerCerberlisk3HitAttackState();

	public static GameUnitState CERBERLISK_THREE_FIRE_BALL_STATE = new PlayerCerberlisk3FireBallState();

	public static GameUnitState CERBERLISK_DASH_ATTACK_STATE = new PlayerCerberliskDashState();

	public static GameUnitState CERBERLISK_BIG_FIRE_BALL_STATE = new PlayerCerberliskBigFireBallState();

	public static GameUnitState CERBERLISK_BREATHE_IN_FIRE_BALL_STATE = new PlayerCerberliskBreathInFireBallState();

	protected GameUnitState mBossState;

	protected int channelID;

	protected byte seatID;

	protected int accountID;

	protected bool mIsRegisted;

	protected List<GameObject> mSpawnPointList = new List<GameObject>();

	protected Weapon weapon;

	protected List<Weapon> weaponList;

	protected UserState userState;

	protected CameraVibrateController mCameraVibrateController;

	protected float speed = 6f;

	public byte bagNum;

	protected int currentBagIndex;

	public bool isPowerful = true;

	protected GameObject defentObj;

	protected FadeInAlphaAnimationScript[] defentScript = new FadeInAlphaAnimationScript[3];

	protected DefentOneByOneScript[] defentOnebyOneScript = new DefentOneByOneScript[3];

	protected Timer showDefentTimer = new Timer();

	protected PlayerSkill playerSkill;

	protected float speedBooth;

	protected GameObject mShieldObj;

	protected Transform mSpine;

	protected Transform mHead;

	protected Transform mWeapon;

	protected Transform mWeaponL;

	protected AimArm[] mArms;

	protected IK1JointAnalytic ikSolver = new IK1JointAnalytic();

	protected GameObject mHeadObj;

	protected GameObject mHandObj;

	protected GameObject mFootObj;

	protected GameObject mBodyObj;

	protected GameObject mBagObj;

	protected int[] pickupNum = new int[4];

	private string _monsterCash;

	private string _bonusCash;

	private string _pickupCash;

	private string _bossCash;

	private string _bossMithril;

	protected Vector3 mLastPosition;

	protected float mTotalMoveDistance;

	protected int mUsedEnergy;

	protected int attackCount;

	protected int hitEnemyCount;

	protected int exp;

	protected int combo;

	protected Timer comboTimer = new Timer();

	protected float teleportStartFadeTime = -1f;

	protected bool bStartFade;

	protected int pickupEnegy;

	protected int maxCombo;

	protected float knockedSpeed;

	protected GameObject chuangsongObj;

	protected bool isSpecialWinIdle = true;

	protected bool winSpecialFinish = true;

	protected int lastAssistItemCount = -1;

	protected Timer rebirthTimer = new Timer();

	protected Timer deadAnimationTimer = new Timer();

	protected Timer winAnimationTimer = new Timer();

	protected Timer vsRebirthTimer = new Timer();

	protected Timer switchToBossTimer = new Timer();

	protected UnityEngine.AI.NavMeshObstacle mNavMeshObstacle;

	protected Timer hitEnemyTimer = new Timer(0.05f, true);

	protected bool needToShowRespawnEffect;

	private int kills;

	private string KillsByAllPlayersStr;

	public bool PortalEndPlayed;

	protected Timer lastBloodEffectTimer = new Timer();

	protected int runningPhase = -1;

	protected int SpecialSkillAbsorbedDamage;

	protected AlphaAnimationScript bagAlphaAnimationScript;

	protected UVOffsetScript bagUVOffsetScript;

	protected bool mIsInvincible;

	public GameUnit LastHitTarget;

	public float LastHitTargetTime = float.MinValue;

	public BattleItemManager mBattleItemManager;

	public BattlePickupManager mBattlePickupManager;

	public bool mGetSlasherWaveHited;

	public int IceSpikeDestroy;

	public bool mGetFireDamage;

	protected bool mHasLeavedGame;

	protected bool _isCapturingFlag;

	protected float moveDistanceWithFlag;

	protected Transform flagSignTrans;

	protected Timer flagSignTimer;

	protected float mSuperShieldEndTime;

	protected float mSuperShieldDamageBooth;

	protected float mSuperShieldBoothRate;

	protected int mNextRebirthIndex = -1;

	protected byte mChargeWeaponStoreValue;

	public bool IsBoss;

	public EnemyType mBossType = EnemyType.NA;

	public int BossIndex;

	public DeadEye.Skill DeadEyeSkill = new DeadEye.Skill();

	public Slasher.Skill SlasherSkill = new Slasher.Skill();

	public FrostStone.Skill FrostStoneSkill = new FrostStone.Skill();

	public Cerberlisk.Skill CerberliskSkill = new Cerberlisk.Skill();

	public BossSkill CurrentBossSkill;

	public int BossSkillScore;

	public Vector2 JumpTime;

	public float JumpHorizontalSpeed;

	public sDashAttack DashAttackParameter;

	protected Timer mBossTouchTimer = new Timer(0.5f);

	protected float mBossTouchRadiusSqr = 16f;

	protected PlayerSkill bossPlayerSkill;

	protected bool isInBossWinState;

	public SocialPlayer SocialPlayer;

	public EPlayerVehicleState mVehicleState;

	public Vehicle mVehicle;

	public bool IsJustRespawn;

	private float mRespawnTime;

	public Timer TakeVehiclePickupTimer = new Timer(3f);

	public SwVoid mSwVoid;

	public SwVoidSavedata mSwVoidSavedata;

	public PlayerState State { get; set; }

	public InputController inputController { get; set; }

	protected int monsterCash
	{
		get
		{
			return AntiCracking.DecryptBufferStr(_monsterCash, "shuliang");
		}
		set
		{
			_monsterCash = AntiCracking.CryptBufferStr(value, "shuliang");
		}
	}

	protected int bonusCash
	{
		get
		{
			return AntiCracking.DecryptBufferStr(_bonusCash, "shuliang");
		}
		set
		{
			_bonusCash = AntiCracking.CryptBufferStr(value, "shuliang");
		}
	}

	protected int pickupCash
	{
		get
		{
			return AntiCracking.DecryptBufferStr(_pickupCash, "shuliang");
		}
		set
		{
			_pickupCash = AntiCracking.CryptBufferStr(value, "shuliang");
		}
	}

	protected int bossCash
	{
		get
		{
			return AntiCracking.DecryptBufferStr(_bossCash, "shuliang");
		}
		set
		{
			_bossCash = AntiCracking.CryptBufferStr(value, "shuliang");
		}
	}

	protected int bossMithril
	{
		get
		{
			return AntiCracking.DecryptBufferStr(_bossMithril, "shuliang");
		}
		set
		{
			_bossMithril = AntiCracking.CryptBufferStr(value, "shuliang");
		}
	}

	public bool NeverGotHit { get; set; }

	public int Kills
	{
		get
		{
			return kills;
		}
		set
		{
			kills = value;
		}
	}

	public int KillsInArea { get; set; }

	public bool JustMakeAShoot { get; set; }

	public bool StayInEnemiesArea { get; set; }

	public bool InCutscene { get; set; }

	public float AngleV { get; set; }

	public float TargetAngleV { get; set; }

	public bool SendingRebirthRequest { get; set; }

	public bool SendingWaitVSRebirthRequest { get; set; }

	public TeamName Team { get; set; }

	public PlayerVSStatistics VSStatistics { get; set; }

	public PlayerPVEStatistics PVEStatistics { get; set; }

	public bool IsFromDead { get; set; }

	public bool isCapturingFlag
	{
		get
		{
			return _isCapturingFlag;
		}
		set
		{
			if (!value)
			{
				if (GetUserState().Achievement != null)
				{
					GetUserState().Achievement.CheckAchievement_250WithFlag(moveDistanceWithFlag);
				}
				moveDistanceWithFlag = 0f;
			}
			_isCapturingFlag = value;
		}
	}

	public override float Speed
	{
		get
		{
			float skill = playerSkill.GetSkill(SkillsType.SPEED_BOOTH);
			bool flag = playerSkill.HasSkill(SkillsType.SHOOT_PUNISH_REDUCE);
			float num = 1f;
			if (isCapturingFlag)
			{
				num = 0.6f;
			}
			float num2 = ((!flag) ? 0.7f : 1f);
			if (State == ATTACK_STATE)
			{
				return Mathf.Max(num2 * num, (speed + weapon.GetSpeedDrag() + skill) * num2 * num);
			}
			return Mathf.Max(1f * num, (speed + weapon.GetSpeedDrag() + skill) * num);
		}
		set
		{
			speed = value;
		}
	}

	public InputController InputController
	{
		get
		{
			return inputController;
		}
	}

	public CameraVibrateController CameraVibrateController
	{
		get
		{
			return mCameraVibrateController;
		}
	}

	public bool CanJump()
	{
		return mBossState == BOSS_IDLE_STATE;
	}

	public GameObject GetHandObj()
	{
		return mHandObj;
	}

	public GameObject GetHeadObj()
	{
		return mHeadObj;
	}

	public GameObject GetBodyObj()
	{
		return mBodyObj;
	}

	public GameObject GetFootObj()
	{
		return mFootObj;
	}

	public GameObject GetBagObj()
	{
		return mBagObj;
	}

	public void SetBagObj(GameObject go)
	{
		mBagObj = go;
	}

	public Timer GetLastBloodEffectTimer()
	{
		return lastBloodEffectTimer;
	}

	public void ComboClear()
	{
		combo = 0;
	}

	public void MakeCombo()
	{
		combo++;
		comboTimer.Do();
		if (combo > maxCombo)
		{
			maxCombo = combo;
			if (IsLocal())
			{
				GetUserState().Achievement.SetPVEMaxCombo(maxCombo);
			}
		}
	}

	public float GetTotalMoveDistance()
	{
		return mTotalMoveDistance;
	}

	public void UseEnergy(int cost)
	{
		mUsedEnergy += cost;
	}

	public int GetUsedEnergy()
	{
		return mUsedEnergy;
	}

	public int GetAttackCount()
	{
		return attackCount;
	}

	public void HitEnemy()
	{
		if (hitEnemyCount + 1 <= attackCount && hitEnemyTimer.Ready())
		{
			hitEnemyTimer.Do();
			hitEnemyCount++;
			if (userState.Achievement != null)
			{
				userState.Achievement.AddHitEnemyCount();
			}
		}
	}

	public int GetHitEnemyCount()
	{
		return hitEnemyCount;
	}

	public string GetName()
	{
		return base.Name;
	}

	public void SetName(string name)
	{
		base.Name = name;
		GetUserState().SetOnlineNickname(name);
	}

	public int GetCombo()
	{
		return combo;
	}

	public int GetMaxCombo()
	{
		return maxCombo;
	}

	public int GetPickupEnegy()
	{
		return pickupEnegy;
	}

	public int GetMonsterCash()
	{
		return monsterCash;
	}

	public void SetMonsterCash(int cash)
	{
		monsterCash = cash;
	}

	public void AddMonsterCash(int cash)
	{
		monsterCash += cash;
	}

	public void AddBossCash(int cash)
	{
		bossCash += cash;
	}

	public void SetBossCash(int cash)
	{
		bossCash = cash;
	}

	public int GetBossCash()
	{
		return bossCash;
	}

	public void SetBossMithril(int mithril)
	{
		bossMithril = mithril;
	}

	public int GetBossMithril()
	{
		return bossMithril;
	}

	public void SetPickupEnegy(int enegy)
	{
		pickupEnegy = enegy;
	}

	public int GetBonusCash()
	{
		return bonusCash;
	}

	public void SetBonusCash(int cash)
	{
		bonusCash = cash;
	}

	public void AddBounsCash(int cash)
	{
		bonusCash += cash;
	}

	public int GetPickupCash()
	{
		return pickupCash;
	}

	public void SetPickupCash(int cash)
	{
		pickupCash = cash;
	}

	public void AddPickupCash(int cash)
	{
		pickupCash += cash;
	}

	public void IncreasePickNum(LootType type)
	{
		pickupNum[(int)type]++;
	}

	public int GetPickNum(LootType type)
	{
		return pickupNum[(int)type];
	}

	public List<Weapon> GetWeaponList()
	{
		return weaponList;
	}

	public Weapon GetWeapon(int id)
	{
		for (int i = 0; i < weaponList.Count; i++)
		{
			if (weaponList[i].GunID == id)
			{
				return weaponList[i];
			}
		}
		return null;
	}

	public string GetWeaponAnimationSuffix()
	{
		return weapon.GetAnimationSuffix();
	}

	public string GetWeaponAnimationSuffixAlter()
	{
		return weapon.GetAnimationSuffixAlter();
	}

	public PlayerSkill GetSkills()
	{
		return playerSkill;
	}

	public virtual bool IsLocal()
	{
		return false;
	}

	public void SetChannelID(int channelID)
	{
		this.channelID = channelID;
	}

	public int GetChannelID()
	{
		return channelID;
	}

	public void SetAccountID(int accountID)
	{
		this.accountID = accountID;
	}

	public int GetAccountID()
	{
		return accountID;
	}

	public void SetSeatID(byte seatID)
	{
		this.seatID = seatID;
	}

	public byte GetSeatID()
	{
		return seatID;
	}

	public void SetIsRegisted(bool isRegisted)
	{
		mIsRegisted = isRegisted;
		if (!IsLocal())
		{
			GetUserState().SetOnlineRegisted(isRegisted);
		}
	}

	public bool GetIsRegisted()
	{
		return mIsRegisted;
	}

	public virtual void SetState(PlayerState state)
	{
		if (State != null)
		{
			State.End(this);
		}
		State = state;
		mBossState = null;
		StateStartTime = Time.time;
		if (State != null)
		{
			State.Start(this);
		}
	}

	public virtual void SetState(GameUnitState state)
	{
		if (mBossState != null)
		{
			mBossState.End(this);
		}
		mBossState = state;
		if (mBossState != null)
		{
			mBossState.Start(this);
		}
		StateStartTime = Time.time;
	}

	public Weapon GetWeapon()
	{
		return weapon;
	}

	public override Collider GetCollider()
	{
		return cc;
	}

	public override bool InPlayingState()
	{
		if (State == DEAD_STATE || State == WAIT_REBIRTH_STATE || State == WAIT_VS_REBIRTH_STATE || State == WIN_STATE || State == LOSE_STATE || State == SWITCH_TO_BOSS_STATE || State == VS_INIT_LOADOUT_STATE || mBossState == BOSS_DEAD_STATE || entityObject == null)
		{
			return false;
		}
		return true;
	}

	public bool IsSameTeam(Player player)
	{
		if (GameApp.GetInstance().GetGameMode().IsTeamMode())
		{
			return Team == player.Team;
		}
		return false;
	}

	public bool IsFriendForce(Player player)
	{
		if (GameApp.GetInstance().GetGameMode().IsTeamMode())
		{
			return Team == player.Team;
		}
		if (GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			return this != player;
		}
		return false;
	}

	public UserState GetUserState()
	{
		return userState;
	}

	protected void FindSpawnPoints()
	{
		LevelConfig levelConfig = (GameApp.GetInstance().GetGameMode().IsCoopMode() ? GameApp.GetInstance().GetUserState().GetLevelConfig(GameApp.GetInstance().GetUserState().GetSelectedNetIndex(), 0) : ((!GameApp.GetInstance().GetGameMode().IsVSMode()) ? userState.GetCurrentLevelConfig() : GameApp.GetInstance().GetUserState().GetVSLevelConfig(0, GameApp.GetInstance().GetUserState().GetSelectedNetIndex())));
		string text = "Player_Spawn_Points_" + levelConfig.spawnPointId;
		bool flag = false;
		GameObject[] array = GameObject.FindGameObjectsWithTag(TagName.PLAYER_SPAWN_POINT);
		mSpawnPointList = new List<GameObject>();
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (text == gameObject.name)
			{
				flag = true;
				for (int j = 0; j < gameObject.transform.childCount; j++)
				{
					mSpawnPointList.Add(gameObject.transform.GetChild(j).gameObject);
				}
				break;
			}
		}
		if (!flag)
		{
			GameObject gameObject2 = GameObject.Find("__MissionTest");
			if (gameObject2 == null)
			{
				Debug.LogError("[NO PLAYER SPAWN POINT FOUND] SpawnPointID = " + levelConfig.spawnPointId);
				return;
			}
			MissionIllegalLoad component = gameObject2.GetComponent<MissionIllegalLoad>();
			GameObject item = ((!component.isTeamMode) ? GameObject.Find(ObjectNamePrefix.PLAYER_SPAWN_POINT_FULL_PATH + component.SpawnPoint) : GameObject.Find(ObjectNamePrefix.TEAM_SPAWN_POINT_FULL_NAME + component.SpawnPoint));
			mSpawnPointList.Add(item);
		}
	}

	public virtual void Init()
	{
		VSStatistics = new PlayerVSStatistics(this);
		PVEStatistics = new PlayerPVEStatistics();
		if (base.GameUnitType != EGameUnitType.ROBOT && !GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			FindSpawnPoints();
			if (IsLocal())
			{
				mTotalMoveDistance = 0f;
			}
		}
		InitPlayerAvatar();
		CreateDefent();
		CreateShield();
		monsterCash = 0;
		bonusCash = 0;
		pickupCash = 0;
		for (int i = 0; i < pickupNum.Length; i++)
		{
			pickupNum[i] = 0;
		}
		exp = 0;
		bossCash = 0;
		bossMithril = 0;
		MaxHp = 0;
		Hp = MaxHp;
		MaxItemShield = 0;
		ItemShield = MaxItemShield;
		inputController = new InputController();
		mCameraVibrateController = new CameraVibrateController();
		if (IsLocal() && GameApp.GetInstance().GetGameMode().IsUsePvpCustomizeMode())
		{
			SetState(VS_INIT_LOADOUT_STATE);
		}
		else if (base.GameUnitType != EGameUnitType.ROBOT)
		{
			State = IDLE_STATE;
		}
		showDefentTimer.SetTimer(0.8f, false);
		mBattleItemManager = new BattleItemManager();
		mBattleItemManager.Init(this);
		mBattlePickupManager = new BattlePickupManager();
		mBattlePickupManager.Init(playerSkill);
		comboTimer.SetTimer(3f, false);
		GameObject original = Resources.Load("_Old/_Effect/Level/chuangsong") as GameObject;
		chuangsongObj = UnityEngine.Object.Instantiate(original, entityTransform.position, Quaternion.identity) as GameObject;
		chuangsongObj.transform.position = entityTransform.position + Vector3.up;
		chuangsongObj.transform.parent = entityTransform;
		chuangsongObj.SetActiveRecursively(false);
		NeverGotHit = true;
		JustMakeAShoot = false;
		StayInEnemiesArea = false;
		InCutscene = false;
		SendingRebirthRequest = false;
		SendingWaitVSRebirthRequest = false;
		lastBloodEffectTimer.SetTimer(0.3f, false);
		mIsInvincible = false;
		mHasLeavedGame = false;
		isCapturingFlag = false;
		Kills = 0;
		KillsInArea = 0;
	}

	public virtual void InitInBossStage()
	{
		FindSpawnPoints();
		if (animation != null)
		{
			animation.Stop();
		}
		SetState(IDLE_STATE);
		foreach (Weapon weapon in weaponList)
		{
			if (weapon != null)
			{
				weapon.InitInBossStage();
			}
		}
		spawnMySwVoid();
		if (GraphicsConfig.IsBossLevel() && !GameApp.GetInstance().GetUserState().IsLowQuality())
		{
			if (Shadow == null && entityTransform.Find("Shadow") != null)
			{
				Shadow = entityTransform.Find("Shadow").gameObject;
			}
			if (Shadow != null)
			{
				Shadow.SetActive(false);
			}
		}
	}

	public void CreateWeaponDamageBooth()
	{
		foreach (Weapon weapon in weaponList)
		{
			weapon.DamageBoothByArmor();
		}
	}

	protected void spawnMySwVoid()
	{
		if (userState.SwVoidIdInUsing != 0)
		{
			if (userState.swVoidList.ContainsKey(userState.SwVoidIdInUsing))
			{
				mSwVoidSavedata = userState.swVoidList[userState.SwVoidIdInUsing];
			}
		}
		else
		{
			mSwVoidSavedata = null;
		}
		if (mSwVoidSavedata != null)
		{
			GameApp.GetInstance().GetGameWorld().SpawnSwVoid(this, mSwVoidSavedata);
		}
		else if (mSwVoid != null)
		{
			mSwVoid.Destroy();
		}
	}

	public void InitSkillButton(List<StateSkill> stateSkillList, int startIndex)
	{
		playerSkill.InitSkillButton(stateSkillList, startIndex);
	}

	public void TrySendPlayerSkillCheck(bool isOnCheck, byte triggerType, int skillId)
	{
		playerSkill.TrySendPlayerSkillCheck(isOnCheck, triggerType, skillId);
	}

	public void InitSkills()
	{
		playerSkill = userState.playerSkill;
		playerSkill.SetPlayer(this);
	}

	public void CreateUserState(byte[] weaponIDs, byte[] armorIDs)
	{
		if (userState == null)
		{
			userState = new UserState();
		}
		byte[] ownedWeapons = new byte[33];
		byte[] ownedHeads = new byte[Global.TOTAL_ARMOR_HEAD_NUM];
		byte[] ownedBodies = new byte[Global.TOTAL_ARMOR_BODY_NUM];
		byte[] ownedArms = new byte[Global.TOTAL_ARMOR_ARM_NUM];
		byte[] ownedFeet = new byte[Global.TOTAL_ARMOR_FOOT_NUM];
		byte[] ownedBag = new byte[Global.TOTAL_ARMOR_BAG_NUM];
		userState.InitArmors(ownedHeads, ownedBodies, ownedArms, ownedFeet, ownedBag);
		userState.InitWeapons(ownedWeapons);
		userState.SetBattleWeaponID(weaponIDs);
	}

	public void CreateUserState(OnlineCustomizeData coopCustomize)
	{
		if (userState == null)
		{
			userState = new UserState();
		}
		byte[] ownedWeapons = new byte[33];
		byte[] ownedHeads = new byte[Global.TOTAL_ARMOR_HEAD_NUM];
		byte[] ownedBodies = new byte[Global.TOTAL_ARMOR_BODY_NUM];
		byte[] ownedArms = new byte[Global.TOTAL_ARMOR_ARM_NUM];
		byte[] ownedFeet = new byte[Global.TOTAL_ARMOR_FOOT_NUM];
		byte[] ownedBag = new byte[Global.TOTAL_ARMOR_BAG_NUM];
		userState.InitArmors(ownedHeads, ownedBodies, ownedArms, ownedFeet, ownedBag);
		userState.InitWeapons(ownedWeapons);
		userState.SetOnlineCustomizeUserStateInfo(coopCustomize);
		userState.SetCoopCustomize(coopCustomize);
	}

	public void CreateUserState(OnlineCustomizeData[] pvpCustomizes, byte currentID)
	{
		if (userState == null)
		{
			userState = new UserState();
		}
		byte[] ownedWeapons = new byte[33];
		byte[] ownedHeads = new byte[Global.TOTAL_ARMOR_HEAD_NUM];
		byte[] ownedBodies = new byte[Global.TOTAL_ARMOR_BODY_NUM];
		byte[] ownedArms = new byte[Global.TOTAL_ARMOR_ARM_NUM];
		byte[] ownedFeet = new byte[Global.TOTAL_ARMOR_FOOT_NUM];
		byte[] ownedBag = new byte[Global.TOTAL_ARMOR_BAG_NUM];
		userState.InitArmors(ownedHeads, ownedBodies, ownedArms, ownedFeet, ownedBag);
		userState.InitWeapons(ownedWeapons);
		OnlineCustomizeData[] array = new OnlineCustomizeData[pvpCustomizes.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new OnlineCustomizeData(pvpCustomizes[i]);
			userState.SetOnlineCustomizeUserStateInfo(pvpCustomizes[i]);
		}
		userState.GetOnlinePlayer().SetOnlineCustomizeArray(array);
		userState.LoadCurrentCustomize(false, currentID);
	}

	public virtual void Loop(float deltaTime)
	{
		if (comboTimer.Ready())
		{
			ComboClear();
			comboTimer.Do();
		}
		if (base.CurrentAnimationName != null)
		{
			LastAnimTime = CurrentAnimTime;
			CurrentAnimTime = animation[base.CurrentAnimationName].normalizedTime;
			if (animation[base.CurrentAnimationName].wrapMode == WrapMode.Loop)
			{
				CurrentAnimTime %= 1f;
			}
		}
		if (!IsBoss)
		{
			if (mVehicle != null)
			{
				mVehicle.Loop();
			}
			if (mSwVoid != null && GameApp.GetInstance().GetGameWorld() != null && GameApp.GetInstance().GetGameWorld().State == GameState.Playing)
			{
				mSwVoid.Loop(deltaTime);
			}
		}
		playerSkill.Loop();
		if (IsJustRespawn && Time.time - mRespawnTime > 0.1f)
		{
			IsJustRespawn = false;
		}
		if (mBattleItemManager != null)
		{
			mBattleItemManager.Loop();
		}
		if (mBattlePickupManager != null)
		{
			mBattlePickupManager.Loop();
		}
		if (IsBoss && mSecondAnim != null)
		{
			mSecondAnim.Update();
		}
		if (needToShowRespawnEffect && LoadingUIScript.mInstance != null && !LoadingUIScript.mInstance.IsLoading())
		{
			GameObject gameObject = EffectPool.GetInstance().GetEffectPool(EffectPoolType.RESPAWN_2).CreateObject(GetPosition(), GetTransform().forward, Quaternion.identity);
			gameObject.transform.parent = GetTransform();
			needToShowRespawnEffect = false;
		}
		if (flagSignTrans != null && flagSignTimer != null && flagSignTimer.Ready())
		{
			if (mVehicleState == EPlayerVehicleState.NONE || mVehicleState == EPlayerVehicleState.CANNON)
			{
				flagSignTrans.localPosition = new Vector3(0f, 2.06f, 0f);
			}
			else if (mVehicleState == EPlayerVehicleState.CARRIER)
			{
				flagSignTrans.localPosition = new Vector3(0f, 3.46f, 0f);
			}
		}
		if (GetWeapon() != null && !IsBoss)
		{
			GetWeapon().DoLogic();
		}
	}

	public void CreateDeadBlood()
	{
		GameObject gameObject = null;
		GameObject gameObject2 = null;
		int num = UnityEngine.Random.Range(0, 100);
		FloorInfo ground = GetGround();
		gameObject = ((num >= 50) ? (Resources.Load(EffectName.DEAD_BLOOD_RED2) as GameObject) : (Resources.Load(EffectName.DEAD_BLOOD_RED) as GameObject));
		int num2 = num % 3 + 1;
		gameObject2 = Resources.Load("_Old/_Effect/Blood_Ground" + num2) as GameObject;
		UnityEngine.Object.Instantiate(gameObject, entityTransform.position + Vector3.up * 1.2f, Quaternion.identity);
		GameObject gameObject3 = UnityEngine.Object.Instantiate(gameObject2, new Vector3(entityTransform.position.x, ground.height + 0.1f, entityTransform.position.z), Quaternion.Euler(270f, 0f, 0f)) as GameObject;
		Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, ground.normal);
		gameObject3.transform.rotation = quaternion * gameObject3.transform.rotation;
	}

	public void CreatePlayerSign()
	{
		Transform transform = entityTransform.Find("PlayerSign");
		if (transform != null)
		{
			UnityEngine.Object.Destroy(transform.gameObject);
		}
		GameObject original = Resources.Load(Global.AVATAR_PATH + "/PlayerSign") as GameObject;
		GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
		gameObject.name = "PlayerSign";
		gameObject.transform.parent = entityTransform;
		if (IsLocal())
		{
			if (IsBoss)
			{
				gameObject.transform.localPosition = new Vector3(-0.1f, 8f, 0f);
				gameObject.transform.localScale = Vector3.one * 0.015f;
			}
			else
			{
				gameObject.transform.localPosition = new Vector3(-0.1f, 2.12f, 0f);
				gameObject.transform.localScale = Vector3.one * 0.007f;
			}
		}
		else if (IsBoss)
		{
			gameObject.transform.localPosition = Vector3.zero + Vector3.up * 8f;
			gameObject.transform.localScale = Vector3.one * (1f / 32f);
		}
		else
		{
			gameObject.transform.localPosition = Vector3.zero + Vector3.up * 2.6f;
			gameObject.transform.localScale = Vector3.one * (1f / 32f);
		}
		UILabel[] componentsInChildren = gameObject.GetComponentsInChildren<UILabel>(true);
		UILabel uILabel = componentsInChildren[0];
		uILabel.text = base.Name;
		if (GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			uILabel.color = UIConstant.COLOR_BLUE_TEAM;
		}
		else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.VS_FFA)
		{
			uILabel.color = UIConstant.FFA_SEAT_COLOR[seatID];
		}
		else if (Team == TeamName.Blue)
		{
			uILabel.color = UIConstant.COLOR_BLUE_TEAM;
		}
		else
		{
			uILabel.color = UIConstant.COLOR_RED_TEAM;
		}
		if (GameApp.GetInstance().GetGameMode().IsVSMode() && !IsLocal() && GameApp.GetInstance().GetGameWorld().GetLocalPlayer() != null && !IsSameTeam(GameApp.GetInstance().GetGameWorld().GetLocalPlayer()))
		{
			Debug.Log(string.Concat(channelID, "  is Team-", Team, "   LocalPlayer is Team", GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.Team));
				gameObject.SetActive(false);
			}
		}

		public void ShowPlayerSignObject()
		{
			Transform transform = entityTransform.Find("PlayerSign");
			if (transform != null)
			{
				transform.gameObject.SetActive(true);
			}
		}

		public void HidePlayerSignObject()
		{
			Transform transform = entityTransform.Find("PlayerSign");
			if (transform != null)
			{
				transform.gameObject.SetActive(false);
			}
		}

		public void CreatePlayerFlag()
		{
			flagSignTrans = entityTransform.Find("teamsign");
			if (flagSignTrans != null)
			{
				UnityEngine.Object.Destroy(flagSignTrans.gameObject);
			}
			GameObject gameObject = null;
			bool flag = false;
			GameObject gameObject2 = null;
			Vector3 vector = entityTransform.position + Vector3.up * 2.06f;
			if (GetChannelID() == GameApp.GetInstance().GetGameWorld().FlagInPlayerID)
			{
				gameObject = Resources.Load("Pickup/NewFlagOnPlayer") as GameObject;
				vector = entityTransform.position + Vector3.up * 2.06f;
				flag = true;
				gameObject2 = UnityEngine.Object.Instantiate(gameObject) as GameObject;
				gameObject2.name = "teamsign";
				gameObject2.transform.position = vector;
				gameObject2.transform.parent = entityTransform;
				Renderer renderer = gameObject2.GetComponent<Renderer>();
				Color white = Color.white;
				if (GameApp.GetInstance().GetGameMode().IsTeamMode())
				{
					white = UIConstant.COLOR_TEAM_PLAYER_ICONS[(int)Team];
				}
				else
				{
					white = UIConstant.COLOR_PLAYER_ICONS[seatID];
					white = new Color(white.r * 0.5f, white.g * 0.5f, white.b * 0.5f, white.a);
				}
				if (renderer != null)
				{
					renderer.material.SetColor("_TintColor", white);
				}
				flagSignTrans = gameObject2.transform;
				flagSignTimer = new Timer(1f, false);
			}
		}

		public void PlayTeleportAnimation()
		{
			chuangsongObj.GetComponent<Animation>().Stop();
			chuangsongObj.SetActiveRecursively(true);
			chuangsongObj.GetComponent<Animation>()["Take 001"].wrapMode = WrapMode.ClampForever;
			chuangsongObj.GetComponent<Animation>().Play("Take 001");
		}

		public bool TeleportAnimationEnds()
		{
			if (GameApp.GetInstance().GetGameWorld().State == GameState.SwitchBossLevel)
			{
				if (chuangsongObj.GetComponent<Animation>()["Take 001"].time >= chuangsongObj.GetComponent<Animation>()["Take 001"].clip.length * 0.5f && teleportStartFadeTime < 0f)
				{
					ActivatePlayer(false);
					EnableTeleportEffect(true);
					teleportStartFadeTime = Time.time;
				}
				if (!bStartFade && teleportStartFadeTime > 0f && Time.time - teleportStartFadeTime > 0.5f)
				{
					FadeAnimationScript.GetInstance().StartFade(new Color(1f, 1f, 1f, 0f), Color.white, 1f);
					bStartFade = true;
				}
				if (teleportStartFadeTime > 0f && Time.time - teleportStartFadeTime > 1.5f)
				{
					return true;
				}
			}
			return false;
		}

		public void EnableTeleportEffect(bool bEnable)
		{
			if (!bEnable)
			{
				chuangsongObj.SetActiveRecursively(false);
			}
			else
			{
				chuangsongObj.SetActiveRecursively(true);
			}
		}

		public void ActivatePlayer(bool bEnable)
		{
			if (!bEnable)
			{
				entityTransform.Find("body").GetComponent<Renderer>().enabled = false;
				entityTransform.Find("head").GetComponent<Renderer>().enabled = false;
				entityTransform.Find("hand").GetComponent<Renderer>().enabled = false;
				entityTransform.Find("foot").GetComponent<Renderer>().enabled = false;
				Transform transform = entityTransform.Find(BoneName.Bag + "/Bag");
				if (transform.GetComponent<Renderer>() != null)
				{
					transform.GetComponent<Renderer>().enabled = false;
				}
				else
				{
					int childCount = transform.GetChildCount();
					for (int i = 0; i < childCount; i++)
					{
						transform.GetChild(i).gameObject.SetActiveRecursively(false);
					}
				}
				weapon.EnableGunObject(false);
				return;
			}
			entityTransform.Find("body").GetComponent<Renderer>().enabled = true;
			entityTransform.Find("head").GetComponent<Renderer>().enabled = true;
			entityTransform.Find("hand").GetComponent<Renderer>().enabled = true;
			entityTransform.Find("foot").GetComponent<Renderer>().enabled = true;
			Transform transform2 = entityTransform.Find(BoneName.Bag + "/Bag");
			if (transform2.GetComponent<Renderer>() != null)
			{
				transform2.GetComponent<Renderer>().enabled = true;
			}
			else
			{
				int childCount2 = transform2.GetChildCount();
				for (int j = 0; j < childCount2; j++)
				{
					transform2.GetChild(j).gameObject.SetActiveRecursively(true);
				}
			}
			weapon.EnableGunObject(true);
		}

		public void PlayBagAlphaAnimation()
		{
			if (bagAlphaAnimationScript != null)
			{
				bagAlphaAnimationScript.StartAnimation();
			}
		}

		public void PlayBagUVAnimation(bool bPlay)
		{
			if (bagUVOffsetScript != null)
			{
				if (bPlay)
				{
					bagUVOffsetScript.enabled = true;
					return;
				}
				bagUVOffsetScript.enabled = false;
				bagUVOffsetScript.GetComponent<Renderer>().material.SetTextureOffset("_tex2", new Vector2(0.66f, 0.66f));
			}
		}

		protected void CreateShield()
		{
			GameObject original = Resources.Load("SW2_Effect/OutSourcing/prefabs/SW3_huzao01") as GameObject;
			mShieldObj = UnityEngine.Object.Instantiate(original, entityTransform.position, entityTransform.rotation) as GameObject;
			mShieldObj.transform.parent = entityTransform;
			HideShield();
		}

		protected void ShowShield()
		{
			mShieldObj.SetActive(true);
		}

		protected void HideShield()
		{
			mShieldObj.SetActive(false);
		}

		public void CreateDefent()
		{
			GameObject original = Resources.Load("_Old/_Effect/defent") as GameObject;
			defentObj = UnityEngine.Object.Instantiate(original, entityTransform.position + Vector3.up * 0.5f + Vector3.forward * 0.6f, entityTransform.rotation) as GameObject;
			defentObj.transform.parent = entityTransform;
			for (int i = 0; i < 3; i++)
			{
				defentScript[i] = defentObj.transform.GetChild(i).GetComponent<FadeInAlphaAnimationScript>();
				defentOnebyOneScript[i] = defentObj.transform.GetChild(i).GetComponent<DefentOneByOneScript>();
			}
			showDefentTimer.SetTimer(0.8f, false);
		}

		public void ShowDefent()
		{
			if (showDefentTimer.Ready())
			{
				int num = UnityEngine.Random.Range(-80, 80);
				defentObj.transform.rotation = entityTransform.rotation;
				defentObj.transform.rotation = Quaternion.AngleAxis(num, Vector3.up) * entityTransform.rotation;
				Vector3 normalized = defentObj.transform.TransformDirection(Vector3.forward).normalized;
				defentObj.transform.position = entityTransform.position + normalized * 0.6f + Vector3.up * 1.4f;
				for (int i = 0; i < 3; i++)
				{
					defentOnebyOneScript[i].appearTime = Time.time + (float)i * 0.05f;
					defentScript[i].FadeIn();
				}
				showDefentTimer.Do();
				AudioManager.GetInstance().PlaySound(AudioName.PLAYER_SHILED_ACTIVE);
			}
		}

		public void RecoveryWhenMakeKills()
		{
			float skill = GetSkills().GetSkill(SkillsType.HP_RECOVERY_WHEN_MAKE_KILL);
			RecoverHP(skill);
		}

		public new virtual bool IsPlayingAnimation(string name)
		{
			return entityObject.GetComponent<Animation>().IsPlaying(name);
		}

		public virtual bool IsPlayingAnimationFinished(string name)
		{
			return IsPlayingAnimation(name) && AnimationPlayed(name, 1f);
		}

		public virtual bool IsPlayingAnimationNotFinished(string name)
		{
			return IsPlayingAnimation(name) && !AnimationPlayed(name, 1f);
		}

		public new virtual void PlayAnimation(string name, WrapMode mode, float blendTime)
		{
			if (!entityObject.GetComponent<Animation>().IsPlaying(name) || mode != WrapMode.ClampForever || AnimationPlayed(name, 1f))
			{
				if (entityObject.GetComponent<Animation>()[name] == null && (name.Contains("UNCC_BUDDY01") || name.Contains("UNCC_BUDDY02")))
				{
					name = mVehicle.GetNamePrefex() + "_" + AnimationString.Idle;
				}
				entityObject.GetComponent<Animation>()[name].wrapMode = mode;
				entityObject.GetComponent<Animation>().CrossFade(name, blendTime);
				base.CurrentAnimationName = name;
				if (IsBoss && mode != WrapMode.Loop)
				{
					entityObject.GetComponent<Animation>()[name].time = 0f;
				}
			}
		}

		public new virtual void PlayAnimation(string name, WrapMode mode)
		{
			PlayAnimation(name, mode, 0.2f);
		}

		public new virtual void PlayAnimationWithoutBlend(string name, WrapMode mode)
		{
			if (!entityObject.GetComponent<Animation>().IsPlaying(name) || mode != WrapMode.ClampForever || AnimationPlayed(name, 1f))
			{
				if (entityObject.GetComponent<Animation>()[name] == null && (name.Contains("UNCC_BUDDY01") || name.Contains("UNCC_BUDDY02")))
				{
					name = mVehicle.GetNamePrefex() + "_" + AnimationString.Idle;
				}
				entityObject.GetComponent<Animation>()[name].wrapMode = mode;
				entityObject.GetComponent<Animation>().Play(name);
				base.CurrentAnimationName = name;
				if (IsBoss && mode != WrapMode.Loop)
				{
					entityObject.GetComponent<Animation>()[name].time = 0f;
				}
			}
		}

		public virtual void PlayAnimationAllLayers(string name, WrapMode mode, float blendLength)
		{
			if (!entityObject.GetComponent<Animation>().IsPlaying(name) || mode != WrapMode.ClampForever)
			{
				if (entityObject.GetComponent<Animation>()[name] == null && (name.Contains("UNCC_BUDDY01") || name.Contains("UNCC_BUDDY02")))
				{
					name = mVehicle.GetNamePrefex() + "_" + AnimationString.Idle;
				}
				entityObject.GetComponent<Animation>()[name].wrapMode = mode;
				entityObject.GetComponent<Animation>().CrossFade(name, blendLength, PlayMode.StopAll);
				base.CurrentAnimationName = name;
				if (IsBoss && mode != WrapMode.Loop)
				{
					entityObject.GetComponent<Animation>()[name].time = 0f;
				}
			}
		}

		public new virtual void PlayAnimationAllLayers(string name, WrapMode mode)
		{
			PlayAnimationAllLayers(name, mode, 0.1f);
		}

		public new virtual void StopAnimation(string name)
		{
			entityObject.GetComponent<Animation>().Stop(name);
		}

		public float GetLegacyAnimationTime(string name)
		{
			if (entityObject.GetComponent<Animation>()[name] == null)
			{
				return 0f;
			}
			return entityObject.GetComponent<Animation>()[name].normalizedTime;
		}

		public new bool AnimationPlayed(string name, float percent)
		{
			if (entityObject.GetComponent<Animation>()[name] == null)
			{
				return false;
			}
			if (entityObject.GetComponent<Animation>()[name].speed >= 0f)
			{
				if (entityObject.GetComponent<Animation>()[name].time >= entityObject.GetComponent<Animation>()[name].clip.length * percent)
				{
					return true;
				}
				return false;
			}
			if (entityObject.GetComponent<Animation>()[name].time <= entityObject.GetComponent<Animation>()[name].clip.length * (1f - percent))
			{
				return true;
			}
			return false;
		}

		public new bool AnimationPlayedLoop(string name, float percent)
		{
			int num = (int)(entityObject.GetComponent<Animation>()[name].time / entityObject.GetComponent<Animation>()[name].clip.length);
			float num2 = entityObject.GetComponent<Animation>()[name].time - entityObject.GetComponent<Animation>()[name].clip.length * (float)num;
			if (entityObject.GetComponent<Animation>()[name] == null)
			{
				return false;
			}
			if (entityObject.GetComponent<Animation>()[name].speed >= 0f)
			{
				if (num2 >= entityObject.GetComponent<Animation>()[name].clip.length * percent)
				{
					return true;
				}
				return false;
			}
			if (num2 <= entityObject.GetComponent<Animation>()[name].clip.length * (1f - percent))
			{
				return true;
			}
			return false;
		}

		public void AdjustAnimationForVehicle(float deltaTime)
		{
			if (mVehicle != null)
			{
				mVehicle.AdjustAnimationInLateUpdate(deltaTime);
				float num = AngleV * ((float)Math.PI / 180f);
				Matrix4x4 identity = Matrix4x4.identity;
				identity[0, 0] = Mathf.Cos(num / 2f);
				identity[0, 1] = 0f - Mathf.Sin(num / 2f);
				identity[1, 0] = Mathf.Sin(num / 2f);
				identity[1, 1] = Mathf.Cos(num / 2f);
				if (null != mHead)
				{
					Matrix4x4 matrix4x = Util.RelativeMatrix(mHead, entityTransform);
					Matrix4x4 matrix4x2 = matrix4x * identity;
					Matrix4x4 m = entityTransform.localToWorldMatrix * matrix4x2;
					mHead.rotation = Util.QuaternionFromMatrix(m);
				}
				if (null != mArms[0].mShoulder && null != mArms[0].mElbow && null != mArms[0].mShoulder)
				{
					ikSolver.Solve(new Transform[3]
					{
						mArms[0].mShoulder,
						mArms[0].mElbow,
						mArms[0].mHand
					}, mVehicle.LeftHand.position);
					mArms[0].mHand.rotation = mVehicle.LeftHand.rotation;
				}
				if (null != mArms[1].mShoulder && null != mArms[1].mElbow && null != mArms[1].mShoulder)
				{
					ikSolver.Solve(new Transform[3]
					{
						mArms[1].mShoulder,
						mArms[1].mElbow,
						mArms[1].mHand
					}, mVehicle.RightHand.position);
					mArms[1].mHand.rotation = mVehicle.RightHand.rotation;
				}
			}
		}

		public void AdjustAnimationInLateUpdate(float deltaTime)
		{
			if (mVehicleState != EPlayerVehicleState.NONE)
			{
				AdjustAnimationForVehicle(deltaTime);
				return;
			}
			Weapon weapon = GetWeapon();
			if (weapon == null || weapon.RotatePivot == null || ((weapon.GetWeaponType() == WeaponType.DoublePistol || weapon.GetWeaponType() == WeaponType.ChargeDoublePistol) && weapon.RotatePivotL == null))
			{
				return;
			}
			EAimType aimType = GetAimType();
			if (aimType == EAimType.NONE)
			{
				return;
			}
			float num = AngleV * ((float)Math.PI / 180f);
			Matrix4x4 identity = Matrix4x4.identity;
			identity[0, 0] = Mathf.Cos(num / 2f);
			identity[0, 1] = 0f - Mathf.Sin(num / 2f);
			identity[1, 0] = Mathf.Sin(num / 2f);
			identity[1, 1] = Mathf.Cos(num / 2f);
			if (null != mHead)
			{
				Matrix4x4 matrix4x = Util.RelativeMatrix(mHead, entityTransform);
				Matrix4x4 matrix4x2 = matrix4x * identity;
				Matrix4x4 m = entityTransform.localToWorldMatrix * matrix4x2;
				mHead.rotation = Util.QuaternionFromMatrix(m);
			}
			Vector3 worldPosition = Vector3.zero;
			if (base.GameUnitType == EGameUnitType.LOCAL_PLAYER && aimType == EAimType.LOOK_AT)
			{
				Camera main = Camera.main;
				Transform transform = main.transform;
				ThirdPersonStandardCameraScript component = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
				Vector3 vector = main.ScreenToWorldPoint(new Vector3(component.ReticlePosition.x, (float)Screen.height - component.ReticlePosition.y, 0.1f));
				worldPosition = transform.position + (vector - transform.position).normalized * 80f;
			}
			Transform rotatePivot = weapon.RotatePivot;
			if (inputController.inputInfo.IsMoving())
			{
				rotatePivot.localPosition = new Vector3(0f, 0f, Mathf.Lerp(rotatePivot.localPosition.z, 0f - weapon.Adjuster.mOffsetRun, deltaTime * 2f));
			}
			else
			{
				rotatePivot.localPosition = new Vector3(0f, 0f, Mathf.Lerp(rotatePivot.localPosition.z, 0f - weapon.Adjuster.mOffsetStand, deltaTime * 2f));
			}
			Matrix4x4 matrix4x3 = Util.RelativeMatrix(mWeapon, rotatePivot);
			Quaternion rotation = rotatePivot.rotation;
			if ((base.GameUnitType == EGameUnitType.LOCAL_PLAYER && aimType == EAimType.ANGLE) || (base.GameUnitType == EGameUnitType.REMOTE_PLAYER && aimType != EAimType.NONE) || (base.GameUnitType == EGameUnitType.ROBOT && aimType != EAimType.NONE))
			{
				if (inputController.inputInfo.IsMoving())
				{
					rotatePivot.Rotate(new Vector3(0f, weapon.Adjuster.mAngleHRun, 0f), Space.World);
				}
				else
				{
					rotatePivot.Rotate(new Vector3(0f, weapon.Adjuster.mAngleHStand, 0f), Space.World);
				}
				rotatePivot.Rotate(new Vector3(0f - (AngleV * weapon.Adjuster.mAngleVSclae + weapon.Adjuster.mAngleVOffset), 0f, 0f), Space.Self);
			}
			else if (aimType == EAimType.LOOK_AT)
			{
				rotatePivot.LookAt(worldPosition);
			}
			Matrix4x4 matrix4x4 = Util.CreateMatrix(rotatePivot.right, rotatePivot.up, rotatePivot.forward, rotatePivot.position);
			rotatePivot.rotation = rotation;
			Matrix4x4 matrix4x5 = matrix4x4 * matrix4x3;
			if (weapon.GetWeaponType() != WeaponType.DoublePistol && weapon.GetWeaponType() != WeaponType.ChargeDoublePistol)
			{
				for (int i = 0; i < mArms.Length; i++)
				{
					if (null != mArms[i].mShoulder && null != mArms[i].mElbow && null != mArms[i].mShoulder)
					{
						mArms[i].mHandRelativeToWeapon = Util.RelativeMatrix(mArms[i].mHand, mWeapon);
						Vector3 target = (matrix4x5 * mArms[i].mHandRelativeToWeapon).MultiplyPoint3x4(Vector3.zero);
						ikSolver.Solve(new Transform[3]
						{
							mArms[i].mShoulder,
							mArms[i].mElbow,
							mArms[i].mHand
						}, target);
						mArms[i].mHand.rotation = Util.QuaternionFromMatrix(matrix4x5 * mArms[i].mHandRelativeToWeapon);
					}
				}
				return;
			}
			Transform rotatePivotL = weapon.RotatePivotL;
			if (inputController.inputInfo.IsMoving())
			{
				rotatePivotL.localPosition = new Vector3(0f, 0f, Mathf.Lerp(rotatePivotL.localPosition.z, 0f - weapon.Adjuster.mOffsetRun, deltaTime * 2f));
			}
			else
			{
				rotatePivotL.localPosition = new Vector3(0f, 0f, Mathf.Lerp(rotatePivotL.localPosition.z, 0f - weapon.Adjuster.mOffsetStand, deltaTime * 2f));
			}
			Matrix4x4 matrix4x6 = Util.RelativeMatrix(mWeaponL, rotatePivotL);
			Quaternion rotation2 = rotatePivotL.rotation;
			if ((base.GameUnitType == EGameUnitType.LOCAL_PLAYER && aimType == EAimType.ANGLE) || (base.GameUnitType == EGameUnitType.REMOTE_PLAYER && aimType != EAimType.NONE) || (base.GameUnitType == EGameUnitType.ROBOT && aimType != EAimType.NONE))
			{
				if (inputController.inputInfo.IsMoving())
				{
					rotatePivotL.Rotate(new Vector3(0f, weapon.Adjuster.mAngleHRun, 0f), Space.World);
				}
				else
				{
					rotatePivotL.Rotate(new Vector3(0f, weapon.Adjuster.mAngleHStand, 0f), Space.World);
				}
				rotatePivotL.Rotate(new Vector3(0f - (AngleV * weapon.Adjuster.mAngleVSclae + weapon.Adjuster.mAngleVOffset), 0f, 0f), Space.Self);
			}
			else if (aimType == EAimType.LOOK_AT)
			{
				rotatePivotL.LookAt(worldPosition);
			}
			Matrix4x4 matrix4x7 = Util.CreateMatrix(rotatePivotL.right, rotatePivotL.up, rotatePivotL.forward, rotatePivotL.position);
			rotatePivotL.rotation = rotation2;
			Matrix4x4 matrix4x8 = matrix4x7 * matrix4x6;
			if (mArms.Length > 1)
			{
				if (null != mArms[0].mShoulder && null != mArms[0].mElbow && null != mArms[0].mShoulder)
				{
					mArms[0].mHandRelativeToWeapon = Util.RelativeMatrix(mArms[0].mHand, mWeaponL);
					Vector3 target2 = (matrix4x8 * mArms[0].mHandRelativeToWeapon).MultiplyPoint3x4(Vector3.zero);
					ikSolver.Solve(new Transform[3]
					{
						mArms[0].mShoulder,
						mArms[0].mElbow,
						mArms[0].mHand
					}, target2);
					mArms[0].mHand.rotation = Util.QuaternionFromMatrix(matrix4x8 * mArms[0].mHandRelativeToWeapon);
				}
				if (null != mArms[1].mShoulder && null != mArms[1].mElbow && null != mArms[1].mShoulder)
				{
					mArms[1].mHandRelativeToWeapon = Util.RelativeMatrix(mArms[1].mHand, mWeapon);
					Vector3 target3 = (matrix4x5 * mArms[1].mHandRelativeToWeapon).MultiplyPoint3x4(Vector3.zero);
					ikSolver.Solve(new Transform[3]
					{
						mArms[1].mShoulder,
						mArms[1].mElbow,
						mArms[1].mHand
					}, target3);
					mArms[1].mHand.rotation = Util.QuaternionFromMatrix(matrix4x5 * mArms[1].mHandRelativeToWeapon);
				}
			}
		}

		public void AddMixingTransformAnimation(string weaponSuffix)
		{
			if (entityObject.GetComponent<Animation>()[AnimationString.Stand + "_" + weaponSuffix + "_" + AnimationString.RunAttack] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Stand + "_" + weaponSuffix + "_" + AnimationString.RunAttack].AddMixingTransform(entityTransform.Find(BoneName.UpperBody));
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Fly + "_" + weaponSuffix + "_" + AnimationString.Idle] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Fly + "_" + weaponSuffix + "_" + AnimationString.Idle].AddMixingTransform(entityTransform.Find(BoneName.UpperBody));
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Fly + "_" + weaponSuffix + "_" + AnimationString.Run] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Fly + "_" + weaponSuffix + "_" + AnimationString.Run].AddMixingTransform(entityTransform.Find(BoneName.UpperBody));
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Fly + "_" + weaponSuffix + "_" + AnimationString.Attack] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Fly + "_" + weaponSuffix + "_" + AnimationString.Attack].AddMixingTransform(entityTransform.Find(BoneName.UpperBody));
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Fly + "_" + weaponSuffix + "_" + AnimationString.RunAttack] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Fly + "_" + weaponSuffix + "_" + AnimationString.RunAttack].AddMixingTransform(entityTransform.Find(BoneName.UpperBody));
			}
		}

		private void AddMixingTransformAnimationRailGun()
		{
			if (entityObject.GetComponent<Animation>()[AnimationString.Stand + "_RailGun_" + AnimationString.RunAttackUp] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Stand + "_RailGun_" + AnimationString.RunAttackUp].AddMixingTransform(entityTransform.Find(BoneName.UpperBody));
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Stand + "_RailGun_" + AnimationString.RunAttackIdle] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Stand + "_RailGun_" + AnimationString.RunAttackIdle].AddMixingTransform(entityTransform.Find(BoneName.UpperBody));
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Fly + "_RailGun_" + AnimationString.Idle] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Fly + "_RailGun_" + AnimationString.Idle].AddMixingTransform(entityTransform.Find(BoneName.UpperBody));
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Fly + "_RailGun_" + AnimationString.Run] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Fly + "_RailGun_" + AnimationString.Run].AddMixingTransform(entityTransform.Find(BoneName.UpperBody));
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Fly + "_RailGun_" + AnimationString.Attack] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Fly + "_RailGun_" + AnimationString.Attack].AddMixingTransform(entityTransform.Find(BoneName.UpperBody));
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Fly + "_RailGun_" + AnimationString.RunAttack] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Fly + "_RailGun_" + AnimationString.RunAttack].AddMixingTransform(entityTransform.Find(BoneName.UpperBody));
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Fly + "_RailGun_" + AnimationString.RunAttackIdle] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Fly + "_RailGun_" + AnimationString.RunAttackIdle].AddMixingTransform(entityTransform.Find(BoneName.UpperBody));
			}
		}

		private void AddMixingTransformAnimationJiatelin()
		{
			if (entityObject.GetComponent<Animation>()[AnimationString.Stand + "_jiatelin_" + AnimationString.RunAttackUp] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Stand + "_jiatelin_" + AnimationString.RunAttackUp].AddMixingTransform(entityTransform.Find(BoneName.UpperBody));
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Stand + "_jiatelin_" + AnimationString.RunAttackIdle] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Stand + "_jiatelin_" + AnimationString.RunAttackIdle].AddMixingTransform(entityTransform.Find(BoneName.UpperBody));
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Fly + "_jiatelin_" + AnimationString.Idle] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Fly + "_jiatelin_" + AnimationString.Idle].AddMixingTransform(entityTransform.Find(BoneName.UpperBody));
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Fly + "_jiatelin_" + AnimationString.Run] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Fly + "_jiatelin_" + AnimationString.Run].AddMixingTransform(entityTransform.Find(BoneName.UpperBody));
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Fly + "_jiatelin_" + AnimationString.Attack] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Fly + "_jiatelin_" + AnimationString.Attack].AddMixingTransform(entityTransform.Find(BoneName.UpperBody));
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Fly + "_jiatelin_" + AnimationString.RunAttack] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Fly + "_jiatelin_" + AnimationString.RunAttack].AddMixingTransform(entityTransform.Find(BoneName.UpperBody));
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Fly + "_jiatelin_" + AnimationString.RunAttackIdle] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Fly + "_jiatelin_" + AnimationString.RunAttackIdle].AddMixingTransform(entityTransform.Find(BoneName.UpperBody));
			}
		}

		private void AddMixingTransformAnimationLaserSword()
		{
			string text = "_LaserSword_";
			if (entityObject.GetComponent<Animation>()[AnimationString.Stand + text + AnimationString.RunAttack] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Stand + text + AnimationString.RunAttack].AddMixingTransform(entityTransform.Find(BoneName.UpperBody));
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Fly + text + AnimationString.Idle] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Fly + text + AnimationString.Idle].AddMixingTransform(entityTransform.Find(BoneName.UpperBody));
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Fly + text + AnimationString.Run] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Fly + text + AnimationString.Run].AddMixingTransform(entityTransform.Find(BoneName.UpperBody));
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Fly + text + AnimationString.RunAttack] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Fly + text + AnimationString.RunAttack].AddMixingTransform(entityTransform.Find(BoneName.UpperBody));
			}
		}

		private void RemoveUpperBodyAnimation(string animName)
		{
			entityObject.GetComponent<Animation>()[animName].AddMixingTransform(entityTransform.Find("Bip01"), false);
			entityObject.GetComponent<Animation>()[animName].AddMixingTransform(entityTransform.Find("Bip01/Bip01 Pelvis"), false);
			entityObject.GetComponent<Animation>()[animName].AddMixingTransform(entityTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine"), false);
			entityObject.GetComponent<Animation>()[animName].AddMixingTransform(entityTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 L Thigh"));
			entityObject.GetComponent<Animation>()[animName].AddMixingTransform(entityTransform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 R Thigh"));
		}

		public void SetLowerBodyAnimation(string weaponSuffix, int layer)
		{
			if (entityObject.GetComponent<Animation>()[AnimationString.Stand + "_" + weaponSuffix + "_" + AnimationString.Run] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Stand + "_" + weaponSuffix + "_" + AnimationString.Run].layer = layer;
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Stand + "_" + weaponSuffix + "_" + AnimationString.Idle] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Stand + "_" + weaponSuffix + "_" + AnimationString.Idle].layer = layer;
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Stand + "_" + weaponSuffix + "_" + AnimationString.Attack] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Stand + "_" + weaponSuffix + "_" + AnimationString.Attack].layer = layer;
			}
		}

		public void SetLowerBodyAnimationRailGun(int layer)
		{
			if (entityObject.GetComponent<Animation>()[AnimationString.Stand + "_RailGun_" + AnimationString.Run] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Stand + "_RailGun_" + AnimationString.Run].layer = layer;
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Stand + "_RailGun_" + AnimationString.Idle] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Stand + "_RailGun_" + AnimationString.Idle].layer = layer;
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Stand + "_RailGun_" + AnimationString.Attack] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Stand + "_RailGun_" + AnimationString.Attack].layer = layer;
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Stand + "_RailGun_" + AnimationString.RunAttackDown] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Stand + "_RailGun_" + AnimationString.RunAttackDown].layer = layer;
				RemoveUpperBodyAnimation(AnimationString.Stand + "_RailGun_" + AnimationString.RunAttackDown);
			}
		}

		public void SetLowerBodyAnimationJiatelin(int layer)
		{
			if (entityObject.GetComponent<Animation>()[AnimationString.Stand + "_jiatelin_" + AnimationString.Run] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Stand + "_jiatelin_" + AnimationString.Run].layer = layer;
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Stand + "_jiatelin_" + AnimationString.Idle] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Stand + "_jiatelin_" + AnimationString.Idle].layer = layer;
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Stand + "_jiatelin_" + AnimationString.Attack] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Stand + "_jiatelin_" + AnimationString.Attack].layer = layer;
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Stand + "_jiatelin_" + AnimationString.RunAttackDown] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Stand + "_jiatelin_" + AnimationString.RunAttackDown].layer = layer;
				RemoveUpperBodyAnimation(AnimationString.Stand + "_jiatelin_" + AnimationString.RunAttackDown);
			}
		}

		public void SetLowerBodyAnimationLaserSword(int layer)
		{
			string text = "_LaserSword_";
			if (entityObject.GetComponent<Animation>()[AnimationString.Stand + text + AnimationString.Run] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Stand + text + AnimationString.Run].layer = layer;
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Stand + text + AnimationString.Idle] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Stand + text + AnimationString.Idle].layer = layer;
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Stand + text + AnimationString.Attack] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Stand + text + AnimationString.Attack].layer = layer;
			}
			if (entityObject.GetComponent<Animation>()[AnimationString.Stand + text + AnimationString.Attack + "01"] != null)
			{
				entityObject.GetComponent<Animation>()[AnimationString.Stand + text + AnimationString.Attack + "01"].layer = layer;
			}
		}

		public void SetGameObject(GameObject obj)
		{
			if (entityObject != null)
			{
				UnityEngine.Object.DestroyImmediate(entityObject);
			}
			entityObject = obj;
			entityTransform = obj.transform;
			cc = entityObject.GetComponent<Collider>() as CharacterController;
			animationObject = entityObject;
			entityObject.GetComponent<Animation>()[AnimationString.FlyIdle].layer = -1;
			RemoveUpperBodyAnimation(AnimationString.FlyIdle);
			entityObject.GetComponent<Animation>()[AnimationString.FlyForward].layer = -1;
			RemoveUpperBodyAnimation(AnimationString.FlyForward);
			entityObject.GetComponent<Animation>()[AnimationString.FlyBack].layer = -1;
			RemoveUpperBodyAnimation(AnimationString.FlyBack);
			entityObject.GetComponent<Animation>()[AnimationString.FlyLeft].layer = -1;
			RemoveUpperBodyAnimation(AnimationString.FlyLeft);
			entityObject.GetComponent<Animation>()[AnimationString.FlyRight].layer = -1;
			RemoveUpperBodyAnimation(AnimationString.FlyRight);
			SetLowerBodyAnimation("assault", -1);
			SetLowerBodyAnimation("RPG", -1);
			SetLowerBodyAnimation("shotgun", -1);
			SetLowerBodyAnimation("DoubleP", -1);
			SetLowerBodyAnimationRailGun(-1);
			SetLowerBodyAnimationJiatelin(-1);
			SetLowerBodyAnimation("LaserBow", -1);
			SetLowerBodyAnimationLaserSword(-1);
			SetLowerBodyAnimation("Sniper", -1);
			SetLowerBodyAnimation("fist", -1);
			AddMixingTransformAnimation("assault");
			AddMixingTransformAnimation("RPG");
			AddMixingTransformAnimation("shotgun");
			AddMixingTransformAnimation("DoubleP");
			AddMixingTransformAnimationRailGun();
			AddMixingTransformAnimationJiatelin();
			AddMixingTransformAnimation("LaserBow");
			AddMixingTransformAnimationLaserSword();
			AddMixingTransformAnimation("Sniper");
			AddMixingTransformAnimation("fist");
		}

		public void StopWalkSound()
		{
			AudioManager.GetInstance().StopSound(AudioName.PLAYER_FOOTSTEP, entityObject);
		}

		public void DropAtSpawnPosition()
		{
			if (IsLocal())
			{
				if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.BOSS_FINAL)
				{
					InitPlayerAvatar(false);
				}
				else
				{
					InitPlayerAvatar();
					mBattlePickupManager.ReinitEffect();
					CreateDefent();
					CreateShield();
				}
			}
			GetTransform().position = mSpawnPointList[seatID].transform.position;
			GetTransform().rotation = mSpawnPointList[seatID].transform.rotation;
			if (IsLocal())
			{
				ThirdPersonStandardCameraScript component = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
				if (null != component)
				{
					component.AngelH = mSpawnPointList[seatID].transform.rotation.eulerAngles.y;
					component.TempAngelH = component.AngelH;
				}
				mTotalMoveDistance = 0f;
				mLastPosition = GetTransform().position;
			}
			else
			{
				(this as RemotePlayer).GetInterpolation().SetTransform(entityTransform);
			}
			if (IsLocal() && (GameApp.GetInstance().GetGameMode().IsSingle() || GameApp.GetInstance().GetGameMode().IsCoopMode()))
			{
				needToShowRespawnEffect = true;
			}
			else
			{
				GameObject gameObject = EffectPool.GetInstance().GetEffectPool(EffectPoolType.RESPAWN_2).CreateObject(GetPosition(), GetTransform().forward, Quaternion.identity);
				gameObject.transform.parent = GetTransform();
			}
			OnSpawned();
			if (mSwVoid != null && null != mSwVoid.GetTransform())
			{
				mSwVoid.GetTransform().position = mSwVoid.GetSafePositionAroundLocalPlayer(mSpawnPointList[seatID].transform);
			}
		}

		public void DropAtSpawnPositionVS()
		{
			GameObject gameObject = GameObject.Find(ObjectNamePrefix.PLAYER_SPAWN_POINT_FULL_PATH + seatID);
			DropAtSpawnPositionVS(gameObject.transform.position, gameObject.transform.eulerAngles.y);
		}

		public void DropAtSpawnPositionVS(Vector3 position, float rotationY)
		{
			OnSpawned();
			Vector3 eulerAngles = GetTransform().eulerAngles;
			eulerAngles.y = rotationY;
			GetTransform().position = position;
			GetTransform().eulerAngles = eulerAngles;
			if (mSwVoid != null && null != mSwVoid.GetTransform())
			{
				mSwVoid.GetTransform().position = mSwVoid.GetSafePositionAroundLocalPlayer(GetTransform());
			}
			if (IsLocal())
			{
				ThirdPersonStandardCameraScript component = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
				if (null != component)
				{
					component.AngelH = rotationY;
					component.TempAngelH = component.AngelH;
				}
			}
			if (IsLocal() && (GameApp.GetInstance().GetGameMode().IsSingle() || GameApp.GetInstance().GetGameMode().IsCoopMode()))
			{
				needToShowRespawnEffect = true;
			}
			else
			{
				GameObject gameObject = EffectPool.GetInstance().GetEffectPool(EffectPoolType.RESPAWN_2).CreateObject(GetPosition(), GetTransform().forward, Quaternion.identity);
				gameObject.transform.parent = GetTransform();
			}
			if (!IsBoss)
			{
				mBattlePickupManager.ReinitEffect();
			}
			if (GameApp.GetInstance().GetGameMode().IsBossVSPlayer() && !IsBoss)
			{
				Team = TeamName.Blue;
			}
		}

		public void ReSpawnPVE()
		{
			mRespawnTime = Time.time;
			IsJustRespawn = true;
			entityObject.GetComponent<Animation>().Stop();
			if (IsLocal())
			{
				Transform transform = Camera.main.transform.Find("Screen_DeadBlood");
				if (transform != null)
				{
					transform.GetComponent<Renderer>().enabled = false;
				}
				GameApp.GetInstance().GetGameWorld().SetGameState(GameState.Playing);
			}
			SetState(IDLE_STATE);
			Hp = MaxHp;
			SendingRebirthRequest = false;
			SendingWaitVSRebirthRequest = false;
			if (IsLocal())
			{
			}
			if (IsLocal() && (GameApp.GetInstance().GetGameMode().IsSingle() || GameApp.GetInstance().GetGameMode().IsCoopMode()))
			{
				needToShowRespawnEffect = true;
			}
			else
			{
				GameObject gameObject = EffectPool.GetInstance().GetEffectPool(EffectPoolType.RESPAWN_2).CreateObject(GetPosition(), GetTransform().forward, Quaternion.identity);
				gameObject.transform.parent = GetTransform();
			}
			OnSpawned();
		}

		public void ReSpawnAtPointVS(int index)
		{
			if (IsBoss)
			{
				return;
			}
			InitPlayerAvatar();
			CreateDefent();
			CreateShield();
			mRespawnTime = Time.time;
			IsJustRespawn = true;
			entityObject.GetComponent<Animation>().Stop();
			GameObject[] array = GameObject.FindGameObjectsWithTag(TagName.RESPAWN);
			GetTransform().position = array[index].transform.position;
			GetTransform().rotation = array[index].transform.rotation;
			SetState(IDLE_STATE);
			OnSpawned();
			if (mSwVoid != null && null != mSwVoid.GetTransform())
			{
				mSwVoid.GetTransform().position = mSwVoid.GetSafePositionAroundLocalPlayer(array[index].transform);
			}
			Hp = MaxHp;
			SendingRebirthRequest = false;
			SendingWaitVSRebirthRequest = false;
			if (IsLocal())
			{
				ThirdPersonStandardCameraScript component = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
				if (null != component)
				{
					component.AngelH = array[index].transform.rotation.eulerAngles.y;
					component.TempAngelH = component.AngelH;
					component.ChangeTarget(GetTransform());
				}
			}
			if (IsLocal() && (GameApp.GetInstance().GetGameMode().IsSingle() || GameApp.GetInstance().GetGameMode().IsCoopMode()))
			{
				needToShowRespawnEffect = true;
			}
			else
			{
				GameObject gameObject = EffectPool.GetInstance().GetEffectPool(EffectPoolType.RESPAWN_2).CreateObject(GetPosition(), GetTransform().forward, Quaternion.identity);
				gameObject.transform.parent = GetTransform();
			}
			mBattlePickupManager.ReinitEffect();
			if (IsLocal())
			{
				Transform transform = Camera.main.transform.Find("Screen_DeadBlood");
				if (transform != null)
				{
					transform.GetComponent<Renderer>().enabled = false;
				}
				GameApp.GetInstance().GetGameWorld().SetGameState(GameState.Playing);
			}
			else
			{
				entityObject.name = channelID.ToString();
				entityObject.layer = PhysicsLayer.REMOTE_PLAYER;
				(this as RemotePlayer).GetInterpolation().SetTransform(entityTransform);
				(this as RemotePlayer).GetInterpolation().Clear();
			}
			if (GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
			{
				Team = TeamName.Blue;
			}
		}

		public bool DoWaitVSRebirth()
		{
			return vsRebirthTimer.Ready();
		}

		public void OnWaitVSRebirth(int maxWaitTime)
		{
			vsRebirthTimer = new Timer();
			vsRebirthTimer.SetTimer(maxWaitTime, false);
			GameObject gameObject = GameObject.Find("GameUI");
			if (gameObject != null)
			{
				InGameUIScript component = gameObject.GetComponent<InGameUIScript>();
				if (component != null)
				{
					component.WaitVSRebirthStart();
				}
			}
		}

		public int GetVSRebirthRemainingTime()
		{
			return Mathf.Max(0, (int)(vsRebirthTimer.GetInterval() - vsRebirthTimer.GetTimeSpan()));
		}

		public virtual bool StartWaitRebirth()
		{
			return true;
		}

		public Timer GetRebirthTimer()
		{
			return rebirthTimer;
		}

		public void Attack()
		{
			JustMakeAShoot = true;
			attackCount++;
			if (userState.Achievement != null)
			{
				userState.Achievement.AddAttackCount();
			}
			weapon.Attack(Time.deltaTime);
			playerSkill.PlayerAttack(weapon);
		}

		public virtual void ChangeWeaponInBag(int bagIndex)
		{
		}

		protected void ChangeWeapon(Weapon w)
		{
			if (weapon != null)
			{
				weapon.GunOff();
				ModifyFirstCoolDownWhenChangeWeapon(weapon, w);
			}
			weapon = w;
			weapon.GunOn();
			if (base.GameUnitType == EGameUnitType.LOCAL_PLAYER && HUDBase.instance != null)
			{
				HUDBase.instance.m_Aim.ChangeWeapon(weapon.AimSpriteString);
			}
		}

		public void GetKnocked()
		{
			string name = AnimationString.Attacked;
			if (knockedSpeed < 0f)
			{
				name = AnimationString.AttackedBack;
				if (Mathf.Abs(knockedSpeed) < 0.05f)
				{
					name = AnimationString.AttackedBackLight;
				}
			}
			PlayAnimation(name, WrapMode.ClampForever);
			if (GameApp.GetInstance().GetGameMode().IsPlayerVSPlayer())
			{
				float percent = Mathf.Abs(knockedSpeed) / entityObject.GetComponent<Animation>()[name].length;
				float num = ((!(knockedSpeed > 0f)) ? (0f - Global.PVP_KNOCKED_SPEED) : Global.PVP_KNOCKED_SPEED);
				if (AnimationPlayed(name, percent))
				{
					SetState(IDLE_STATE);
					if (!(GetSkills().GetSkill(SkillsType.FLY) > 0f) || 1 == 0)
					{
						PlayAnimationAllLayers(AnimationString.Stand + "_" + GetWeaponAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
					}
				}
				else
				{
					cc.Move(entityTransform.forward * num + ((!IsLocal()) ? Vector3.zero : Vector3.down));
				}
			}
			else if (AnimationPlayed(name, 0.5f))
			{
				cc.Move(entityTransform.forward * knockedSpeed * 0.3f + ((!IsLocal()) ? Vector3.zero : Vector3.down));
				if (AnimationPlayed(name, 1f) || inputController.inputInfo.IsMoving())
				{
					SetState(IDLE_STATE);
					if (!(GetSkills().GetSkill(SkillsType.FLY) > 0f) || 1 == 0)
					{
						PlayAnimationAllLayers(AnimationString.Stand + "_" + GetWeaponAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
					}
				}
			}
			else
			{
				cc.Move(entityTransform.forward * knockedSpeed + ((!IsLocal()) ? Vector3.zero : Vector3.down));
			}
		}

		public void UnderAttackSetHP(int hp)
		{
			if (InPlayingState())
			{
				if (IsLocal())
				{
					Transform transform = Camera.main.transform.Find("Screen_Blood");
					if (transform != null)
					{
						transform.localScale = new Vector3(2.4f * (float)Screen.width / (float)Screen.height, 2.4f, 1f);
						ScreenBloodScript component = transform.GetComponent<ScreenBloodScript>();
						component.NewBlood(Hp - hp);
					}
				}
				else if (Hp == hp && Hp != 0 && GameApp.GetInstance().GetGameMode().IsCoopMode())
				{
					if (!IsBoss)
					{
						ShowDefent();
					}
					return;
				}
				bool flag = (float)Hp >= (float)MaxHp * 0.3f;
				Hp = hp;
				if (hp == 0)
				{
					OnDead();
					if (IsBoss)
					{
						SetState(BOSS_DEAD_STATE);
					}
					else
					{
						SetState(DEAD_STATE);
					}
				}
				else
				{
					AudioManager.GetInstance().PlaySoundAt(AudioName.PLAYER_HIT_LIGHT, entityTransform.position);
					if (IsLocal() && flag && (float)Hp < (float)MaxHp * 0.3f && ItemTween.GetInstance() != null)
					{
						ItemTween.GetInstance().AutoPopupWhenLowHp();
					}
				}
			}
			NeverGotHit = false;
		}

		public virtual void UnderAttack(int damage)
		{
			if (InPlayingState())
			{
				bool flag = (float)Hp >= (float)MaxHp * 0.3f;
				Hp -= damage;
				if (PlayerSkillBorrowedTime.isActive)
				{
					Hp = Mathf.Clamp(Hp, 1, MaxHp);
				}
				else
				{
					Hp = Mathf.Clamp(Hp, 0, MaxHp);
				}
				if (Hp <= 0)
				{
					OnDead();
					SetState(DEAD_STATE);
				}
				else
				{
					AudioManager.GetInstance().PlaySoundAt(AudioName.PLAYER_HIT_LIGHT, entityTransform.position);
					if (flag && (float)Hp < (float)MaxHp * 0.3f && ItemTween.GetInstance() != null)
					{
						ItemTween.GetInstance().AutoPopupWhenLowHp();
					}
				}
				Transform transform = Camera.main.transform.Find("Screen_Blood");
				if (transform != null)
				{
					transform.localScale = new Vector3(2.4f * (float)Screen.width / (float)Screen.height, 2.4f, 1f);
					ScreenBloodScript component = transform.GetComponent<ScreenBloodScript>();
					component.NewBlood(damage);
				}
			}
			NeverGotHit = false;
		}

		public void GetHit(int damage)
		{
			Hp -= damage;
			Hp = Mathf.Clamp(Hp, 0, MaxHp);
		}

		public void Block()
		{
			if (!(defentObj != null))
			{
				return;
			}
			float num = UnityEngine.Random.Range(0f, 1f);
			if (num < playerSkill.GetSkill(SkillsType.BLOCK_AT_A_RATE))
			{
				ShowDefent();
				int num2 = UnityEngine.Random.Range(0, 100);
				if (num2 < 50)
				{
					AudioManager.GetInstance().PlaySoundSingleAt("_Old/_Audio/force_shield01", entityTransform.position);
				}
				else
				{
					AudioManager.GetInstance().PlaySoundSingleAt("_Old/_Audio/force_shield02", entityTransform.position);
				}
			}
		}

		public bool DoSwitchToBoss()
		{
			return switchToBossTimer.Ready();
		}

		public bool DoWin()
		{
			if (!IsFromDead)
			{
				if (GetSkills().GetSkill(SkillsType.FLY) > 0f)
				{
					PlayAnimation(AnimationString.FlyWin01, WrapMode.Loop);
				}
				else if (!winSpecialFinish)
				{
					PlayAnimation(AnimationString.WinSpecial, WrapMode.ClampForever);
					if (AnimationPlayed(AnimationString.WinSpecial, 1f))
					{
						winSpecialFinish = true;
					}
				}
				else if (isSpecialWinIdle)
				{
					PlayAnimation(AnimationString.WinIdleSpecial, WrapMode.Loop);
				}
				else
				{
					PlayAnimationWithoutBlend(AnimationString.Stand + "_" + GetWeaponAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
				}
			}
			return winAnimationTimer.Ready();
		}

		public bool DoVSLose()
		{
			if (!IsFromDead && AnimationPlayed(AnimationString.Lose, 1f))
			{
				PlayAnimation(AnimationString.LoseIdle, WrapMode.Loop);
			}
			return winAnimationTimer.Ready();
		}

		public virtual bool CheckLose()
		{
			return false;
		}

		public bool DeadAnimationCompleted()
		{
			return deadAnimationTimer.Ready();
		}

		public virtual bool WinAnimationCompleted()
		{
			return false;
		}

		public void UploadStatistaic()
		{
			GameMode gameMode = GameApp.GetInstance().GetGameMode();
			if (!gameMode.IsMultiPlayer())
			{
				return;
			}
			if (gameMode.IsCoopMode())
			{
				if (GetBossCash() > 0)
				{
					Lobby.GetInstance().FirstBossChallenge = false;
				}
				PlayerUploadStatisticsRequest request = new PlayerUploadStatisticsRequest((int)((float)GetMonsterCash() * (1f + GetSkills().GetSkill(SkillsType.MONEY_BOOTH))), (int)((float)GetPickupCash() * (1f + GetSkills().GetSkill(SkillsType.MONEY_BOOTH))), GetPickupEnegy(), (int)((float)GetBonusCash() * (1f + GetSkills().GetSkill(SkillsType.MONEY_BOOTH))), (int)((float)GetBossCash() * (1f + GetSkills().GetSkill(SkillsType.MONEY_BOOTH))), GetBossMithril());
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			else if (gameMode.IsVSMode())
			{
				short k = (short)VSStatistics.Kills;
				if (gameMode.ModePlay == Mode.GIANT_BETRAYAL)
				{
					k = (short)VSStatistics.PlayerBossKills;
				}
				VSUploadStatisticsRequest request2 = new VSUploadStatisticsRequest(k, (short)VSStatistics.Death, (short)VSStatistics.Assist, (short)VSStatistics.GoodAssist, (short)VSStatistics.Capture, (short)VSStatistics.Assassinate, (short)VSStatistics.ControlPoint, (short)VSStatistics.ControlPointAssist, (short)VSStatistics.StopHim);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
			}
		}

		public override void OnHit(DamageProperty dp)
		{
			if (mIsInvincible || InCutscene || Time.time - mRespawnTime < 2f || IsInVehicleOnOffState())
			{
				return;
			}
			int num = CalculateDamage(dp);
			if (defentObj != null)
			{
				float num2 = UnityEngine.Random.Range(0f, 1f);
				if (num2 < playerSkill.GetSkill(SkillsType.BLOCK_AT_A_RATE))
				{
					ShowDefent();
					if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
					{
						PlayerOnHitRequest request = new PlayerOnHitRequest(0, false, 0);
						GameApp.GetInstance().GetNetworkManager().SendRequest(request);
					}
					return;
				}
			}
			if (IsInAnyShields(dp) || (mVehicle != null && dp.attackerGameUnit != null && mVehicle.AbsorbDamage(num, dp.attackerGameUnit.GetPosition())))
			{
				return;
			}
			if (ItemShield > 0)
			{
				num = HitShield(num);
			}
			if (num <= 0)
			{
				return;
			}
			playerSkill.playerTryOpenSuperShield();
			if (isInSuperShield())
			{
				convertDamageToBooth(num);
				return;
			}
			if (PlayerSkillBorrowedTime.isActive)
			{
				AbsorbDamage(num);
			}
			if (base.GameUnitType == EGameUnitType.LOCAL_PLAYER)
			{
				if (InPlayingState())
				{
					if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
					{
						PlayerOnHitRequest playerOnHitRequest = new PlayerOnHitRequest(num, false, 0);
						if (playerSkill.HasSkill(SkillsType.DAMAGE_RETURN) && dp.attackerGameUnit != null && dp.attackerGameUnit.InPlayingState() && dp.attackerGameUnit.GameUnitType == EGameUnitType.ENEMY)
						{
							playerOnHitRequest.SetEnemyId((Enemy)dp.attackerGameUnit);
						}
						GameApp.GetInstance().GetNetworkManager().SendRequest(playerOnHitRequest);
					}
					else
					{
						if (playerSkill.HasSkill(SkillsType.DAMAGE_RETURN) && dp.attackerGameUnit != null && dp.attackerGameUnit.InPlayingState())
						{
							dp.attackerGameUnit.Hp -= (int)((float)num * playerSkill.GetSkill(SkillsType.DAMAGE_RETURN));
							dp.attackerGameUnit.Hp = Mathf.Max(0, dp.attackerGameUnit.Hp);
						}
						UnderAttack(num);
					}
				}
			}
			else if (base.GameUnitType == EGameUnitType.ROBOT)
			{
				UnderAttack(num);
			}
			playerSkill.PlayerBeHit();
			if (dp.attackerGameUnit != null && dp.attackerGameUnit.GameUnitType == EGameUnitType.ENEMY && ((Enemy)dp.attackerGameUnit).EnemyType == EnemyType.CERBERLISK && (dp.EnemySkillType == 4 || dp.EnemySkillType == 5 || dp.EnemySkillType == 8))
			{
				mGetFireDamage = true;
			}
		}

		public bool CanBeKnocked()
		{
			if (IsBoss)
			{
				return false;
			}
			return mVehicleState == EPlayerVehicleState.NONE && State != FREEZE_STATE;
		}

		public int CalculateDamage(DamageProperty dp)
		{
			int damage = dp.damage;
			dp.debuff += playerSkill.GetSkill(SkillsType.ALL_RES);
			dp.debuff += playerSkill.GetSkill((SkillsType)(59 + dp.damageType - 1));
			float num = 1f - playerSkill.GetSkill(SkillsType.FINAL_RES);
			num -= playerSkill.GetSkill((SkillsType)(92 + dp.damageType - 1));
			num = Mathf.Max(0.05f, num);
			damage = (int)((float)damage * (1f + dp.buff) / (1f + dp.debuff) * num);
			if (playerSkill.HasSkill(SkillsType.BE_HIT_DAMAGE_LIMITED))
			{
				damage = Mathf.Min(damage, (int)((float)MaxHp * playerSkill.GetSkill(SkillsType.BE_HIT_DAMAGE_LIMITED)));
			}
			if (damage <= 0)
			{
				return 0;
			}
			return damage;
		}

		public void OnKnocked(float speed)
		{
			if (InPlayingState() && CanBeKnocked())
			{
				knockedSpeed = speed;
				GetWeapon().StopFire();
				GetWeapon().PlayIdleAnim();
				GetWeapon().AutoDestructEffect();
				GetWeapon().StopSound();
				SetState(KNOCKED_STATE);
				AudioManager.GetInstance().PlaySoundAt(AudioName.PLAYER_HIT_HEAVY, entityTransform.position);
			}
		}

		public virtual void OnDead()
		{
			GetOffVehicleImmediately(false);
			if (IsBoss)
			{
				PlayAnimation(AnimationString.ENEMY_DEAD, WrapMode.ClampForever);
			}
			else if (mVehicle != null && mVehicleState != EPlayerVehicleState.CANNON)
			{
				PlayAnimation(mVehicle.GetNamePrefex() + "_" + AnimationString.FallOffCarrier, WrapMode.ClampForever);
			}
			else if (GetSkills().GetSkill(SkillsType.FLY) > 0f)
			{
				PlayAnimation(AnimationString.FlyDead01, WrapMode.ClampForever);
			}
			else if (UnityEngine.Random.value > 0.5f)
			{
				PlayAnimation(AnimationString.Dead01, WrapMode.ClampForever);
			}
			else
			{
				PlayAnimation(AnimationString.Dead02, WrapMode.ClampForever);
			}
			if (mVehicle != null)
			{
				mVehicleState = EPlayerVehicleState.NONE;
				mVehicle = null;
			}
			if (!IsBoss)
			{
				GetWeapon().StopFire();
				GetWeapon().PlayIdleAnim();
				GetWeapon().StopSound();
			}
			StopWalkSound();
			playerSkill.PlayerDead();
			playerSkill.PlayerDeadClear();
			if (IsLocal())
			{
				UploadStatistaic();
			}
			else
			{
				RemotePlayer remotePlayer = this as RemotePlayer;
				if (remotePlayer != null)
				{
					remotePlayer.GetInterpolation().Clear();
				}
			}
			AudioManager.GetInstance().PlaySoundAt(GetDeadSoundName(), entityTransform.position);
			if (GameApp.GetInstance().GetGameMode().IsVSMode())
			{
				CreateDeadBlood();
			}
			if (IsLocal() && GameApp.GetInstance().GetGameMode().IsCatchTheFlagMode() && GameApp.GetInstance().GetGameWorld().FlagInPlayerID == channelID)
			{
				DropTheFlagRequest request = new DropTheFlagRequest(GetChannelID(), GetTransform().position + Vector3.up * 0f, false);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			deadAnimationTimer.SetTimer(4f, false);
			if (weapon != null)
			{
				weapon.ClearEffect();
			}
			foreach (Weapon weapon in GetWeaponList())
			{
				if (weapon.GetWeaponType() == WeaponType.ChargeDoublePistol)
				{
					SetChargeWeaponStoreChargeValue((weapon as ChargeDoublePistol).GetCharge());
				}
			}
			Debug.Log(GetWeaponList().Count);
		}

		public void OnSwitchToBoss()
		{
			GetOffVehicleImmediately(true);
			GetWeapon().StopFire();
			GetWeapon().StopSound();
			GetWeapon().PlayIdleAnim();
			switchToBossTimer.SetTimer(2f, false);
			EffectPool.GetInstance().GetEffectPool(EffectPoolType.RESPAWN_1).CreateObject(GetPosition(), GetTransform().forward, Quaternion.identity);
			if (IsLocal() && (State == DEAD_STATE || State == WAIT_REBIRTH_STATE))
			{
				Transform transform = Camera.main.transform.Find("Screen_DeadBlood");
				if (transform != null)
				{
					transform.GetComponent<Renderer>().enabled = false;
				}
			}
			SetState(SWITCH_TO_BOSS_STATE);
			if (GetSkills().GetSkill(SkillsType.FLY) > 0f)
			{
				PlayAnimationAllLayers(AnimationString.FlyIdle, WrapMode.Loop);
				PlayAnimation(AnimationString.Fly + "_" + GetWeaponAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
				AudioManager.GetInstance().StopSound("float_move");
				AudioManager.GetInstance().PlaySoundSingleAt("_Old/_Audio/float_idle", GetTransform().position);
			}
			else
			{
				PlayAnimationAllLayers(AnimationString.Stand + "_" + GetWeaponAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
				AudioManager.GetInstance().StopSound(AudioName.PLAYER_FOOTSTEP, GetObject());
			}
		}

		public void OnWin()
		{
			GetOffVehicleImmediately(true);
			if (IsLocal())
			{
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && GameApp.GetInstance().GetGameMode().ModePlay == Mode.BOSS_FINAL)
				{
					DailyBonusManager.GetInstance().CheckBossQuest(GameApp.GetInstance().GetUserState().CoopBossType, (byte)GameApp.GetInstance().GetUserState().GetSelectedDifficultMode());
				}
				UploadStatistaic();
				GetUserState().Achievement.AddVSWinNum(GameApp.GetInstance().GetGameMode().ModePlay, true);
				GetUserState().Achievement.ReportToLeaderBoard();
			}
			int num = UnityEngine.Random.Range(0, 100);
			if (num < 50)
			{
				winSpecialFinish = false;
				isSpecialWinIdle = false;
			}
			else
			{
				winSpecialFinish = true;
				isSpecialWinIdle = true;
			}
			GetWeapon().StopFire();
			GetWeapon().StopSound();
			GetWeapon().PlayIdleAnim();
			winAnimationTimer.SetTimer(4f, false);
			bool flag = GetSkills().GetSkill(SkillsType.FLY) > 0f;
			if (IsLocal())
			{
				AudioManager.GetInstance().StopAllMusic();
				AudioManager.GetInstance().PlaySound(AudioName.PLAYER_WIN);
				if (flag)
				{
					AudioManager.GetInstance().StopSound("float_move");
				}
				else
				{
					AudioManager.GetInstance().StopSound(AudioName.PLAYER_FOOTSTEP, GetObject());
				}
			}
			if (State == DEAD_STATE || State == WAIT_REBIRTH_STATE || State == WAIT_VS_REBIRTH_STATE)
			{
				IsFromDead = true;
				if (flag)
				{
					PlayAnimationAllLayers(AnimationString.FlyIdle, WrapMode.Loop);
					PlayAnimation(AnimationString.Fly + "_" + GetWeaponAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
					AudioManager.GetInstance().PlaySoundSingleAt("_Old/_Audio/float_idle", GetTransform().position);
				}
				else
				{
					PlayAnimationAllLayers(AnimationString.Stand + "_" + GetWeaponAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
				}
				if (IsLocal())
				{
					Transform transform = Camera.main.transform.Find("Screen_DeadBlood");
					if (transform != null)
					{
						transform.GetComponent<Renderer>().enabled = false;
					}
				}
			}
			else
			{
				IsFromDead = false;
			}
			if (!IsBoss)
			{
				SetState(WIN_STATE);
			}
		}

		public void OnLose()
		{
			GetOffVehicleImmediately(true);
			if (IsLocal())
			{
				UploadStatistaic();
				GetUserState().Achievement.AddVSWinNum(GameApp.GetInstance().GetGameMode().ModePlay, false);
				GetUserState().Achievement.ReportToLeaderBoard();
			}
			GetWeapon().StopFire();
			GetWeapon().StopSound();
			GetWeapon().PlayIdleAnim();
			winAnimationTimer.SetTimer(4f, false);
			if (IsLocal())
			{
				AudioManager.GetInstance().StopAllMusic();
				AudioManager.GetInstance().PlaySound(AudioName.PLAYER_LOSE);
			}
			if (State == DEAD_STATE || State == WAIT_REBIRTH_STATE || State == WAIT_VS_REBIRTH_STATE)
			{
				IsFromDead = true;
			}
			else
			{
				IsFromDead = false;
				if (!IsBoss)
				{
					PlayAnimationAllLayers(AnimationString.Lose, WrapMode.ClampForever);
				}
			}
			if (!IsBoss)
			{
				SetState(LOSE_STATE);
			}
		}

		public void OnMissionFail()
		{
			GetOffVehicleImmediately(true);
			if (IsLocal())
			{
				UploadStatistaic();
				GetUserState().Achievement.ReportToLeaderBoard();
			}
			if (State == DEAD_STATE || State == WAIT_REBIRTH_STATE || State == WAIT_VS_REBIRTH_STATE)
			{
				IsFromDead = true;
			}
			else
			{
				IsFromDead = false;
				if (!IsBoss)
				{
					PlayAnimationAllLayers(AnimationString.Lose, WrapMode.ClampForever);
				}
			}
			SetState(LOSE_STATE);
			GetWeapon().StopFire();
			GetWeapon().PlayIdleAnim();
			GetWeapon().StopSound();
			winAnimationTimer.SetTimer(4f, false);
			winAnimationTimer.Do();
			AudioManager.GetInstance().StopAllMusic();
			AudioManager.GetInstance().PlaySound(AudioName.PLAYER_LOSE);
			MissionManager.GetInstance().OnMainMissionComplete(false);
			if (MissionDetailsHUD.m_Instance != null)
			{
				MissionDetailsHUD.m_Instance.OnFail();
			}
		}

		public void OnQuitOnline()
		{
			mHasLeavedGame = true;
			GetOffVehicleImmediately(true);
			if (!IsBoss)
			{
				SetState(LOSE_STATE);
				PlayAnimationAllLayers(AnimationString.Lose, WrapMode.ClampForever);
			}
		}

		public virtual void OnPickUp(LootType lootType, float param1, float param2)
		{
		}

		public void OnPickUpBySwVoid(ItemScript item)
		{
			if (item != null)
			{
				OnPickUp(item.itemType, item.Param1, item.Param2);
			}
			item.Destroy();
			if (GameApp.GetInstance().GetGameMode().IsSingle())
			{
				OnPickUp(item.itemType, item.Param1, item.Param2);
				item.Destroy();
				if (item.GetSpawner() != null)
				{
					item.GetSpawner().ItemPickUp();
				}
			}
			else if (IsLocal())
			{
				PickUpItemRequest request = new PickUpItemRequest(item.sequenceID);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}

		public FloorInfo GetGround()
		{
			FloorInfo floorInfo = new FloorInfo();
			floorInfo.height = Global.FLOORHEIGHT;
			Vector3 up = Vector3.up;
			Ray ray = new Ray(entityTransform.position + new Vector3(0f, 0.5f, 0f), Vector3.down);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 10f, 1 << PhysicsLayer.FLOOR))
			{
				floorInfo.height = hitInfo.point.y;
				floorInfo.normal = hitInfo.normal;
			}
			if (floorInfo.normal.y < 0f)
			{
				floorInfo.normal = -floorInfo.normal;
			}
			return floorInfo;
		}

		public bool isAttacking()
		{
			return State == ATTACK_STATE;
		}

		public void ToggleInvincible()
		{
			mIsInvincible = !mIsInvincible;
		}

		public bool IsInvincible()
		{
			return mIsInvincible;
		}

		public void ChangerAllShader(string shaderName)
		{
		}

		private void ChangeShader(string shaderName, GameObject changeObject)
		{
			Material material = changeObject.GetComponent<Renderer>().material;
			material.shader = Shader.Find(shaderName);
		}

		public void RecalculateMaxHp()
		{
			if (GameApp.GetInstance().GetGameMode().IsSingle())
			{
				MaxHp = (int)playerSkill.GetSkill(SkillsType.HP_BOOTH);
				MaxHp += (int)((float)MaxHp * playerSkill.GetSkill(SkillsType.HP_UP_RATE));
				Hp = MaxHp;
			}
		}

		public void RecalculateMaxHp(bool CurrentHPBooth, float rate)
		{
			if (GameApp.GetInstance().GetGameMode().IsSingle())
			{
				MaxHp = (int)playerSkill.GetSkill(SkillsType.HP_BOOTH);
				if (CurrentHPBooth)
				{
					Hp += (int)((float)MaxHp * rate);
				}
				MaxHp += (int)((float)MaxHp * playerSkill.GetSkill(SkillsType.HP_UP_RATE));
				Hp = Mathf.Clamp(Hp, 0, MaxHp);
			}
		}

		private void ModifyFirstCoolDownWhenChangeWeapon(Weapon oldWeapon, Weapon newWeapon)
		{
			float num = oldWeapon.GetLastAttackTime();
			if (!(newWeapon.AttackFrequency > oldWeapon.AttackFrequency) && num < Time.time)
			{
				num = num + oldWeapon.AttackFrequency - newWeapon.AttackFrequency;
			}
			if (GetSkills().GetSkill(SkillsType.CHANGE_GUN_SPEED_UP) > 0f && num + newWeapon.AttackFrequency > Time.time)
			{
				num -= (num + newWeapon.AttackFrequency - Time.time) * GetSkills().GetSkill(SkillsType.CHANGE_GUN_SPEED_UP);
			}
			newWeapon.SetLastAttackTime(num);
		}

		public override void CheckHit2(CreatureMeleeAttackSkill skill)
		{
			if (IsLocal())
			{
				base.CheckHit2(skill);
			}
		}

		public override void CheckColliderHit(CreatureMeleeAttackSkill skill)
		{
			if (IsLocal())
			{
				base.CheckColliderHit(skill);
			}
		}

		public virtual void RefreshAvatar(EnemyType type)
		{
			SetState(IDLE_STATE);
			isInBossWinState = false;
			string prefab = string.Empty;
			switch (type)
			{
			case EnemyType.DEAD_EYE:
				prefab = "Enemy/" + Global.QUALITY_LEVEL_1 + "/Boss/DeadEye";
				break;
			case EnemyType.SLASHER:
				prefab = "Enemy/" + Global.QUALITY_LEVEL_1 + "/Boss/Slasher";
				break;
			case EnemyType.FROST_STONE:
				prefab = "Enemy/" + Global.QUALITY_LEVEL_1 + "/Boss/FrostStone";
				break;
			case EnemyType.CERBERLISK:
				prefab = "Enemy/" + Global.QUALITY_LEVEL_1 + "/Boss/Cerberlisk";
				break;
			}
			GameObject gameObject = Pool.sAllocate(prefab);
			gameObject.transform.position = GetPosition();
			gameObject.transform.parent = null;
			gameObject.transform.position = entityObject.transform.position;
			gameObject.transform.rotation = entityObject.transform.rotation;
			GameObject obj = entityObject;
			SetObject(gameObject);
			Vector3 center = cc.center;
			center.y = cc.radius;
			cc.center = center;
			UnityEngine.Object.Destroy(obj);
			IsBoss = true;
			mBossType = type;
			BossIndex = EnemyBoss.GetBossIndex(mBossType);
			animation = entityObject.GetComponent<Animation>();
			bossPlayerSkill = new PlayerSkill();
			bossPlayerSkill.Clear();
			bossPlayerSkill.CreateSkills();
			bossPlayerSkill.SetPlayer(this);
			if (GameApp.GetInstance().GetGameWorld() != null)
			{
				GiantBetrayalModeLogic giantBetrayalModeLogic = GameApp.GetInstance().GetGameWorld().VsBattleLogic as GiantBetrayalModeLogic;
				giantBetrayalModeLogic.bossPlayerActiveBufferList = new Dictionary<int, SkillGroup>();
			}
			playerSkill = bossPlayerSkill;
			SetState(BOSS_IDLE_STATE);
			InitBoss(type);
			CreateShadow(1f);
			Team = TeamName.Red;
			CreatePlayerSign();
			if (weapon != null)
			{
				weapon.ClearEffect();
			}
		}

		public void InitBoss(EnemyType type)
		{
			switch (type)
			{
			case EnemyType.DEAD_EYE:
				DeadEyeSkill.Init(entityObject);
				DashAttackParameter = DeadEyeParameter.DASH_ATTACK;
				CurrentBossSkill = DeadEyeSkill;
				break;
			case EnemyType.SLASHER:
				SlasherSkill.Init(entityObject, false);
				CurrentBossSkill = SlasherSkill;
				break;
			case EnemyType.FROST_STONE:
				FrostStoneSkill.Init(entityObject);
				DashAttackParameter = FrostStoneParameter.DASH_ATTACK;
				CurrentBossSkill = FrostStoneSkill;
				break;
			case EnemyType.CERBERLISK:
				CerberliskSkill.Init(entityObject);
				DashAttackParameter = CerberliskParameter.DASH_ATTACK;
				CurrentBossSkill = CerberliskSkill;
				break;
			}
			JumpTime = PlayerBossParameter.BossJumpTime[BossIndex];
			JumpHorizontalSpeed = PlayerBossParameter.BossJumpHorizontalSpeed[BossIndex];
			if (entityObject.transform.Find("SecondAnimation") != null)
			{
				mSecondAnim = new SecondAnimation();
				GameObject gameObject = entityObject.transform.Find("SecondAnimation").gameObject;
				mSecondAnim.Init(this, gameObject);
			}
		}

		public float GetCurrentDps()
		{
			Weapon weapon = null;
			weapon = ((this.weapon == null) ? weaponList[0] : this.weapon);
			if (weapon == null)
			{
				return 0f;
			}
			return weapon.Damage / weapon.AttackFrequency;
		}

		public void RecoverHP(float recoveryAmount)
		{
			if (recoveryAmount >= 0f)
			{
				recoveryAmount *= 1f + playerSkill.GetSkill(SkillsType.RECOVERY_BOOTH);
			}
			else if (ItemShield > 0)
			{
				recoveryAmount = -HitShield(-(int)recoveryAmount);
			}
			Hp += (int)recoveryAmount;
			Hp = Mathf.Clamp(Hp, 1, MaxHp);
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && !IsLocal())
			{
			}
		}

		public void AddShield(float recoveryAmount, float percentAmount)
		{
			MaxItemShield += (int)(recoveryAmount + percentAmount * (float)MaxHp);
			ItemShield += (int)(recoveryAmount + percentAmount * (float)MaxHp);
			MaxItemShield = Mathf.Clamp(MaxItemShield, 0, MaxHp * 2);
			ItemShield = Mathf.Clamp(ItemShield, 0, MaxHp * 2);
			Debug.Log("Add shield : " + ItemShield + "/" + MaxItemShield);
			ShowShield();
		}

		public int HitShield(int Damage)
		{
			if (ItemShield <= Damage)
			{
				int result = Damage - ItemShield;
				RemoveShiled();
				return result;
			}
			ItemShield -= Damage;
			Debug.Log("Hit shield : " + ItemShield + "/" + MaxItemShield);
			return 0;
		}

		public void RemoveShiled()
		{
			MaxItemShield = 0;
			ItemShield = 0;
			Debug.Log("Remove shield");
			HideShield();
		}

		public void SetShield(int itemShield, int maxItemShield)
		{
			ItemShield = itemShield;
			MaxItemShield = maxItemShield;
			Debug.Log("Set shield : " + ItemShield + "/" + MaxItemShield);
			if (itemShield > 0)
			{
				ShowShield();
			}
			else
			{
				HideShield();
			}
		}

		public void RecoverRateHp(float recoveryRate)
		{
			RecoverHP(recoveryRate * (float)MaxHp);
		}

		public void ResetAllWeaponAttackFrenquency()
		{
			foreach (Weapon weapon in weaponList)
			{
				weapon.AttackFrequency = weapon.AttackFrenquencyInit / (1f + weapon.GetAccBuff());
			}
			this.weapon.SetWeaponShootSpeed();
		}

		public virtual void ChangeToIdleState()
		{
			SetState(IDLE_STATE);
		}

		public void InitPlayerAvatar(bool needNewAvatar = true)
		{
			GameObject gameObject = GetObject();
			if (needNewAvatar || gameObject == null)
			{
				gameObject = AvatarBuilder.GetInstance().ReBuildPlayerAvatar(userState, this);
				SetGameObject(gameObject);
			}
			if (IsLocal() && !GameApp.GetInstance().GetGameMode().IsVSMode() && base.GameUnitType != EGameUnitType.ROBOT)
			{
				gameObject.transform.position = mSpawnPointList[seatID].transform.position;
			}
			if (IsLocal())
			{
				mLastPosition = gameObject.transform.position;
			}
			gameObject.name = "Player";
			gameObject.layer = PhysicsLayer.PLAYER;
			gameObject.tag = TagName.PLAYER_ROOT;
			weaponList = userState.GetBattleWeapons();
			if (needNewAvatar)
			{
				foreach (Weapon weapon in weaponList)
				{
					weapon.Init(this);
				}
				if (base.GameUnitType == EGameUnitType.ROBOT)
				{
					ChangeWeapon(weaponList[0]);
				}
				else if (IsLocal())
				{
					ChangeWeaponInBag(userState.GetWeaponBagIndex(weaponList[0]));
				}
				else
				{
					ChangeWeapon(weaponList[0]);
				}
			}
			Transform transform = (Transform)gameObject.GetComponent("Transform");
			mSpine = transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1");
			mHead = transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head");
			mWeapon = transform.Find(BoneName.Weapon);
			mWeaponL = transform.Find(BoneName.WeaponL);
			mHeadObj = transform.Find("head").gameObject;
			mHandObj = transform.Find("hand").gameObject;
			mBodyObj = transform.Find("body").gameObject;
			mFootObj = transform.Find("foot").gameObject;
			mArms = new AimArm[2]
			{
				new AimArm(transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 L Clavicle/Bip01 L UpperArm"), transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 L Clavicle/Bip01 L UpperArm/Bip01 L Forearm"), transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 L Clavicle/Bip01 L UpperArm/Bip01 L Forearm/Bip01 L Hand")),
				new AimArm(transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm"), transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm"), transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand"))
			};
			Transform transform2 = entityTransform.Find(BoneName.Bag).Find("Bag");
			FlyBagAnimationScript componentInChildren = transform2.GetComponentInChildren<FlyBagAnimationScript>();
			if (componentInChildren != null)
			{
				componentInChildren.player = this;
			}
			HealBagAnimationScript componentInChildren2 = transform2.GetComponentInChildren<HealBagAnimationScript>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.player = this;
				componentInChildren2.Init();
			}
			bagUVOffsetScript = transform2.GetComponent<UVOffsetScript>();
			animation = entityObject.GetComponent<Animation>();
			base.CurrentAnimationName = null;
		}

		public void EnableKillStreak(int killStreakId)
		{
			playerSkill.EnableKillStreak(killStreakId);
		}

		public void UseItem(int itemId, bool isCopy)
		{
			if (IsLocal())
			{
				Item item = userState.GetItems()[itemId];
				if (item != null)
				{
					item.OnUsed(isCopy);
					if (!isCopy)
					{
						GetSkills().PlayerUseAItem();
					}
					GetUserState().Achievement.AddItemUsed(itemId);
				}
			}
			mBattleItemManager.OnItemUsed(itemId);
		}

		public void ReuseItem(int itemId)
		{
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				PlayerUseItemRequest request = new PlayerUseItemRequest(itemId, true);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			else
			{
				UseItem(itemId, true);
			}
		}

		public void TriggerSkillFromOtherPlayer(int skillId)
		{
			playerSkill.TriggerSkillFromOtherPlayer(skillId);
		}

		public void TriggerOffSkillFromOtherPlayer(int skillId)
		{
			playerSkill.TriggerOffSkillFromOtherPlayer(skillId);
		}

		public override bool UnitCanBeHitByRay(Vector3 position, float distance)
		{
			Vector3 vector = GetPosition() + new Vector3(0f, 1f, 0f);
			Ray ray = new Ray(position, vector - position);
			int layerMask = ((!IsLocal()) ? ((1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.REMOTE_PLAYER) | (1 << PhysicsLayer.BOSS)) : ((1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.PLAYER) | (1 << PhysicsLayer.BOSS)));
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, distance, layerMask))
			{
				if (hitInfo.collider.gameObject.layer == PhysicsLayer.PLAYER || hitInfo.collider.gameObject.layer == PhysicsLayer.REMOTE_PLAYER)
				{
					return true;
				}
				if (hitInfo.collider.gameObject.layer == PhysicsLayer.BOSS && IsBoss)
				{
					return true;
				}
			}
			return false;
		}

		public string GetDeadSoundName()
		{
			if (IsBoss)
			{
				switch (mBossType)
				{
				case EnemyType.DEAD_EYE:
					return AudioName.DEADEYE_DEAD;
				case EnemyType.SLASHER:
					return AudioName.SLASHER_DEAD;
				case EnemyType.FROST_STONE:
					return AudioName.FROSTSTONE_DEAD;
				case EnemyType.CERBERLISK:
					return AudioName.CERBERLISK_DEAD;
				default:
					return null;
				}
			}
			return AudioName.PLAYER_DEAD;
		}

		public bool IsInChooseBossSkillState()
		{
			if (IsBoss && mBossState != null && mBossState == BOSS_CHOOSE_SKILL_STATE)
			{
				return true;
			}
			return false;
		}

		public UnityEngine.AI.NavMeshObstacle GetNavMeshObstacle()
		{
			return mNavMeshObstacle;
		}

		public void SetNavMeshObstacle(UnityEngine.AI.NavMeshObstacle instance)
		{
			mNavMeshObstacle = instance;
		}

		public virtual bool IsRobot()
		{
			return false;
		}

		public void PlayerInvisible()
		{
			GetHeadObj().GetComponent<Renderer>().enabled = false;
			GetHandObj().GetComponent<Renderer>().enabled = false;
			GetFootObj().GetComponent<Renderer>().enabled = false;
			GetBagObj().SetActive(false);
			GetBodyObj().GetComponent<Renderer>().enabled = false;
			GetWeapon().EnableGunObject(false);
		}

		public void PlayerAppear()
		{
			GetHeadObj().GetComponent<Renderer>().enabled = true;
			GetHandObj().GetComponent<Renderer>().enabled = true;
			GetFootObj().GetComponent<Renderer>().enabled = true;
			GetBagObj().SetActive(true);
			GetBodyObj().GetComponent<Renderer>().enabled = true;
			GetWeapon().EnableGunObject(true);
		}

		protected virtual EAimType GetAimType()
		{
			EAimType result = EAimType.NONE;
			if (State == IDLE_STATE)
			{
				result = weapon.GetIdleAimType();
			}
			else if (State == ATTACK_STATE)
			{
				result = weapon.GetAttackAimType();
			}
			return result;
		}

		public void GetOffVehicleImmediately(bool GunOn)
		{
			if (mVehicle != null)
			{
				mVehicle.DeadOff();
				if (GunOn)
				{
					GetWeapon().GunOn();
				}
				mVehicle = null;
				mVehicleState = EPlayerVehicleState.NONE;
			}
		}

		public bool IsInVehicleOnOffState()
		{
			return State == GET_ON_CARRIER_STATE || State == FREEZE_STATE || State == GET_OFF_CARRIER_STATE;
		}

		public bool IsInVehicleState()
		{
			return State == GET_ON_CARRIER_STATE || State == FREEZE_STATE || State == GET_OFF_CARRIER_STATE || State == VEHICLE_IDLE_STATE || State == VEHICLE_ATTACK_STATE;
		}

		private void OnSpawned()
		{
			if (State != VS_INIT_LOADOUT_STATE)
			{
				spawnMySwVoid();
			}
		}

		public bool CanSpawnVehicle()
		{
			return Hp > 0 && State != DEAD_STATE && State != WIN_STATE && State != LOSE_STATE && State != WAIT_REBIRTH_STATE && State != WAIT_VS_REBIRTH_STATE && State != VS_INIT_LOADOUT_STATE;
		}

		public bool IsInAnyShields(DamageProperty dp)
		{
			if (GameApp.GetInstance().GetGameWorld() == null)
			{
				return false;
			}
			int damage = CalculateDamage(dp);
			float num = float.MaxValue;
			ShieldScript shieldScript = null;
			foreach (ShieldScript currentShield in GameApp.GetInstance().GetGameWorld().GetCurrentShields())
			{
				if (currentShield != null && Vector3.Distance(GetPosition(), currentShield.transform.position) < num && currentShield.shieldStarted)
				{
					shieldScript = currentShield;
				}
			}
			if (shieldScript == null)
			{
				return false;
			}
			if (Vector3.Distance(GetPosition(), shieldScript.transform.position) < shieldScript.shieldRadius - 0.5f && (shieldScript.ownedPlayer == this || shieldScript.ownedPlayer.IsFriendForce(this)))
			{
				if (dp.attackerGameUnit.GameUnitType == EGameUnitType.ENEMY)
				{
					shieldScript.AbsorbDamage(damage);
				}
				else if (dp.attackerGameUnit.GameUnitType == EGameUnitType.LOCAL_PLAYER)
				{
					shieldScript.PlayerHitShield(dp);
				}
				else if (dp.attackerGameUnit.GameUnitType == EGameUnitType.REMOTE_PLAYER)
				{
					shieldScript.PlayerHitShield(dp);
				}
				return true;
			}
			return false;
		}

		protected override void CreateShadow(float size)
		{
			base.CreateShadow(size);
			if (GraphicsConfig.IsBossLevel() && !GameApp.GetInstance().GetUserState().IsLowQuality())
			{
				Shadow.SetActive(false);
			}
		}

		public void SetInBossWinState(bool isInWin)
		{
			isInBossWinState = isInWin;
		}

		public int GetKillsByAllPlayers()
		{
			return AntiCracking.DecryptBufferStr(KillsByAllPlayersStr, "kill_monster");
		}

		public void SetKillsByAllPlayers(int _kills)
		{
			KillsByAllPlayersStr = AntiCracking.CryptBufferStr(Mathf.Clamp(_kills, 0, 10000), "kill_monster");
		}

		public void AddKillsByAllPlayers(int _kills)
		{
			SetKillsByAllPlayers(Mathf.Clamp(GetKillsByAllPlayers() + _kills, 0, 10000));
		}

		public bool hasBeenDestroyed()
		{
			if (entityObject != null && entityObject.transform.childCount == 0)
			{
				return true;
			}
			return false;
		}

		public GameUnitState GetPlayerBossState()
		{
			return mBossState;
		}

		public void HitTargetShowCrossHit(bool isSwVoidHit = false)
		{
			if (StateAim.mInstance != null)
			{
				StateAim.mInstance.HitTarget(isSwVoidHit);
			}
		}

		public void AbsorbDamage(int damage)
		{
			SpecialSkillAbsorbedDamage += damage;
			SpecialSkillAbsorbedDamage = Mathf.Clamp(SpecialSkillAbsorbedDamage, 0, MaxHp * 2);
		}

		public void ResetAbsorbDamage()
		{
			SpecialSkillAbsorbedDamage = 0;
		}

		public int GetAbsorbDamage()
		{
			return SpecialSkillAbsorbedDamage;
		}

		public void OpenSuperShield(float superShieldTime, float superShieldRate)
		{
			mSuperShieldEndTime = superShieldTime + Time.time;
			mSuperShieldBoothRate = superShieldRate;
			mSuperShieldDamageBooth = 0f;
		}

		public bool isInSuperShield()
		{
			return Time.time < mSuperShieldEndTime;
		}

		public void convertDamageToBooth(int damage)
		{
			mSuperShieldDamageBooth += (float)damage * 1f / (float)MaxHp * mSuperShieldBoothRate;
		}

		public float GetSuperShieldBooth()
		{
			return mSuperShieldDamageBooth;
		}

		public void SetNextRebirthIndex(int value)
		{
			mNextRebirthIndex = value;
		}

		public void ClearSuperShieldDamageBooth()
		{
			mSuperShieldDamageBooth = 0f;
		}

		public void SetChargeWeaponStoreChargeValue(byte _value)
		{
			mChargeWeaponStoreValue = _value;
		}

		public byte GetChargeWeaponStoreChargeValue()
		{
			return mChargeWeaponStoreValue;
		}
	}
