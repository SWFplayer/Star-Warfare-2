using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : GameUnit
{
	public enum EParamName
	{
		ID = 0,
		HP = 1,
		SPEED = 2,
		CAN_ACCELERATE = 3,
		EXP = 4,
		MONEY = 5,
		PICKUPRATE = 6,
		PICKUPLOWER = 7,
		PICUPUPPER = 8,
		ATTACK1 = 9,
		ATTACK2 = 10,
		ATTACK3 = 11,
		ATTACK_FREQUENCY = 12,
		ELITE_HP_SCALE = 13,
		ELITE_SPEED_ADD = 14,
		ELITE_ATTACK_SCALE = 15,
		ELITE_MONEY_SCALE = 16,
		Icon = 17,
		Name = 18,
		DamageType = 19
	}

	protected const float SLOW_DOWN_EFFECT = 0.7f;

	public static EnemyState GRAVEBORN_STATE = new GraveBornState();

	public static EnemyState IDLE_STATE = new EnemyIdleState();

	public static EnemyState CATCHING_STATE = new CatchingState();

	public static EnemyState GOTHIT_STATE = new GotHitState();

	public static EnemyState ATTACK_STATE = new EnemyAttackState();

	public static EnemyState DEAD_STATE = new DeadState();

	public static EnemyState FADE_OUT_STATE = new EnemyFadeOut();

	public static EnemyState BLACK_HOLE_STATE = new BlackHoleState();

	public static EnemyState STUN_STATE = new StunState();

	public static EnemyState ROUTE_STATE = new RouteState();

	public static EnemyState CONDITON_CATCH_STATE = new ConditionCatchState();

	public static EnemyState CONDITON_IDLE_STATE = new ConditionIdleState();

	protected EnemyType mEnemyType;

	protected GameWorld mGameWorld;

	protected Vector3 dir;

	protected Timer deadRemoveBodyTimer;

	protected Player targetPlayer;

	protected int hp;

	protected int maxHp;

	protected float mRunSpeed;

	protected bool mCanAccelerate;

	protected float mAcceleration;

	protected Timer mAccelerateTimer = new Timer();

	protected float mMaxSpeed;

	protected float mMaxAnimationSpeed;

	protected EnemyState mState;

	protected DamageProperty mLastDamageProperty;

	protected float attackRange;

	public float attackFrequency;

	public int attackDamage;

	protected int[] mAttackDamages = new int[3];

	protected int rushDamage;

	protected int shotDamage;

	protected int touchDamage;

	protected float bombRange;

	protected int score;

	protected int lootCash;

	protected float pickupRate;

	protected int pickupLower;

	protected int pickupUpper;

	protected float lootCashScale = 1f;

	protected int exp;

	protected float idleStartTime;

	protected float lastAttackTime = -100f;

	protected string runAnimationName = AnimationString.ENEMY_RUN;

	protected GameObject targetObj;

	protected float onhitRate = 100f;

	protected bool criticalAttacked;

	protected bool attacked;

	protected Quaternion deadRotation;

	protected Vector3 deadPosition;

	protected Vector3[] path;

	protected int lastTargetCode = -1;

	protected Timer catchPlayerTargetChangeTimer = new Timer();

	protected BloodColor bloodColor;

	protected float lastReachPointTime;

	protected PlayerDamageType mPlayerDamageType;

	public Ray mRay;

	public RaycastHit mRaycastHit;

	public float lastStateTime;

	public bool IsNewState;

	public ConditionState mConditionState;

	public ConditionState mLastConditionState;

	public Dictionary<GameUnitState, ConditionState> mStateSwitchDic;

	public Dictionary<string, ConditionState> mStateTag;

	public List<ConditionState> mAllConditionState;

	public CreatureMeleeAttackSkill MeleeAttackSkill;

	protected Timer mShoutAudioTimer;

	protected string shoutAudioName;

	protected MonsterConfig monsterConfig;

	protected Vector3 smoothPos = Vector3.zero;

	protected float mFloorHeight = Global.FLOORHEIGHT;

	protected bool seePlayer;

	protected Timer recordTimer = new Timer();

	protected Quaternion enemyRotation;

	protected float lastMoveTime;

	protected Vector3 lastPos;

	protected Timer gotHitAnimationCDTimer = new Timer();

	protected Vector3 localScale = Vector3.zero;

	protected float wayPointSqrDistance;

	protected Timer lastBloodEffectTimer = new Timer();

	public GameObject bipObject;

	protected bool mIsSlowDown;

	protected Timer mSlowDownTimer = new Timer();

	protected float mLastIdleTime;

	protected float mLastUpdatePosTime;

	protected float mLastAttackTime;

	protected float mLastCatchingTime;

	protected float mLastGotHitTime;

	protected Timer mLastBloodEffectTimer = new Timer();

	public Timer mDeadTimer = new Timer();

	protected Timer mDeadRemoveBodyTimer = new Timer();

	protected Timer mOnHitTimer = new Timer();

	protected Timer mLookAtTimer = new Timer();

	protected Timer mRunAudioTimer = new Timer();

	protected Timer mWalkAudioTimer = new Timer();

	protected Timer mFindTargetTimer = new Timer();

	protected Timer mHitTargetTimer = new Timer();

	protected Player mLocalPlayer;

	protected int mTargetUserId;

	protected Vector3 mTargetToLookAt;

	protected float mUpdatePosInterval;

	protected float mMaxTurnRadian;

	protected float mTurnSpeed;

	protected bool mHasMission;

	protected Material mMaterial;

	protected float alpha = 1f;

	protected float mMaxIdleTime;

	protected float mMaxCatchingTime;

	protected bool mIsShoot;

	protected Vector3 mDeltaPosition;

	protected int mCurrentDeltaCount;

	protected int mMaxDeltaCount = 30;

	private bool mRemoteEnemyMove = true;

	private bool mIsInvincible;

	protected static List<GameUnit> mTargetList = new List<GameUnit>();

	protected static Dictionary<EnemyType, Timer> sShoutAudioTimer = new Dictionary<EnemyType, Timer>();

	public MissionEnemyRouteScript currentRouteScript;

	protected BossParameter mBossParameter;

	protected bool isPresentLooted;

	private List<GameObject> mFootParticles = new List<GameObject>();

	public short EnemyID { get; set; }

	public int PlayerCount { get; set; }

	public int DifficultLevel { get; set; }

	public bool IsElite { get; set; }

	public override int Hp
	{
		get
		{
			return hp;
		}
		set
		{
			hp = value;
		}
	}

	public override int MaxHp
	{
		get
		{
			return maxHp;
		}
		set
		{
			maxHp = value;
		}
	}

	public string RunAnimationName
	{
		get
		{
			return runAnimationName;
		}
	}

	public float AttackRange
	{
		get
		{
			return attackRange;
		}
	}

	public float AttackRangeSqr
	{
		get
		{
			return attackRange * attackRange;
		}
	}

	public EnemyType EnemyType
	{
		get
		{
			return mEnemyType;
		}
		set
		{
			mEnemyType = value;
		}
	}

	public float WayPointSqrDistance
	{
		get
		{
			return wayPointSqrDistance;
		}
	}

	public virtual bool IsDead
	{
		get
		{
			return mState == DEAD_STATE;
		}
	}

	public float SqrDistanceFromPlayer
	{
		get
		{
			if (mTarget == null)
			{
				return 99999f;
			}
			Vector3 vector = mTargetPosition - entityTransform.position;
			return (vector - vector.normalized * mTarget.GetHitColliderRadius()).sqrMagnitude;
		}
	}

	public Enemy()
	{
		base.GameUnitType = EGameUnitType.ENEMY;
	}

	public override bool InPlayingState()
	{
		return mState != GRAVEBORN_STATE && mState != DEAD_STATE && mState != FADE_OUT_STATE && entityObject != null && entityObject.activeSelf;
	}

	public Timer GetLastBloodEffectTimer()
	{
		return lastBloodEffectTimer;
	}

	public float GetIdleTimeDuration()
	{
		return Time.time - mLastIdleTime;
	}

	public void SetIdleTimeNow()
	{
		mLastIdleTime = Time.time;
	}

	public float GetCatchingTimeDuration()
	{
		return Time.time - mLastCatchingTime;
	}

	public void SetCatchingTimeNow()
	{
		mLastCatchingTime = Time.time;
	}

	public float GetUpdatePosTimeDuration()
	{
		return Time.time - mLastUpdatePosTime;
	}

	public void SetUpdatePosTimeNow()
	{
		mLastUpdatePosTime = Time.time;
	}

	public float GetAttackTimeDuration()
	{
		return Time.time - mLastAttackTime;
	}

	public void SetAttackTimeNow()
	{
		mLastAttackTime = Time.time;
	}

	public bool CouldMakeNextAttack()
	{
		if (Time.time - lastAttackTime >= attackFrequency)
		{
			return true;
		}
		return false;
	}

	public virtual bool CouldEnterAttackState()
	{
		if (SqrDistanceFromPlayer < AttackRange * AttackRange)
		{
			return true;
		}
		return false;
	}

	public virtual void PlayAnimation(string name)
	{
		animation.CrossFade(name, 0.1f);
		base.CurrentAnimationName = name;
	}

	public new virtual void PlayAnimation(string name, WrapMode mode)
	{
		if (!animation.IsPlaying(name) || mode != WrapMode.ClampForever)
		{
			animation[name].wrapMode = mode;
			animation.CrossFade(name, 0.1f);
			base.CurrentAnimationName = name;
		}
	}

	public new virtual void PlayAnimation(string name, WrapMode mode, float speed)
	{
		if (!animation.IsPlaying(name) || mode != WrapMode.ClampForever)
		{
			animation[name].wrapMode = mode;
			animation[name].speed = speed;
			animation.CrossFade(name, 0.1f);
			base.CurrentAnimationName = name;
		}
	}

	public new void StopAnimation()
	{
		animation.Stop();
	}

	public virtual bool IsBoss()
	{
		return false;
	}

	public PlayerDamageType GetPlayerDamageType()
	{
		return mPlayerDamageType;
	}

	public virtual void UpdatePosition(Vector3 position)
	{
		if (!Lobby.GetInstance().IsMasterPlayer && InPlayingState() && entityTransform != null)
		{
			mDeltaPosition = position - entityTransform.position;
			mCurrentDeltaCount = 0;
		}
	}

	public virtual void LoadConfig()
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.ENEMY_LIST, DataSheetName.ENEMY_OVERALL);
		monsterConfig = new MonsterConfig();
		monsterConfig.hp = gameDataTable.GetInt((int)EnemyType, 1);
		monsterConfig.walkSpeed = gameDataTable.GetFloat((int)EnemyType, 2);
		monsterConfig.canAccelerate = gameDataTable.GetBoolean((int)EnemyType, 3);
		monsterConfig.exp = gameDataTable.GetInt((int)EnemyType, 4);
		monsterConfig.lootCash = gameDataTable.GetInt((int)EnemyType, 5);
		monsterConfig.pickupRate = gameDataTable.GetFloat((int)EnemyType, 6);
		monsterConfig.pickupLower = gameDataTable.GetInt((int)EnemyType, 7);
		monsterConfig.pickupUpper = gameDataTable.GetInt((int)EnemyType, 8);
		monsterConfig.AttackDamages[0] = gameDataTable.GetInt((int)EnemyType, 9);
		monsterConfig.AttackDamages[1] = gameDataTable.GetInt((int)EnemyType, 10);
		monsterConfig.AttackDamages[2] = gameDataTable.GetInt((int)EnemyType, 11);
		monsterConfig.AttackFrequency = gameDataTable.GetFloat((int)EnemyType, 12);
		if (IsElite)
		{
			animation[AnimationString.ENEMY_RUN].speed += gameDataTable.GetFloat((int)EnemyType, 14) / monsterConfig.walkSpeed;
			monsterConfig.hp = (int)((float)monsterConfig.hp * gameDataTable.GetFloat((int)EnemyType, 13));
			monsterConfig.walkSpeed += gameDataTable.GetFloat((int)EnemyType, 14);
			monsterConfig.lootCash = (int)((float)monsterConfig.lootCash * gameDataTable.GetFloat((int)EnemyType, 16));
			for (int i = 0; i < monsterConfig.AttackDamages.Length; i++)
			{
				monsterConfig.AttackDamages[i] = (int)((float)monsterConfig.AttackDamages[i] * gameDataTable.GetFloat((int)EnemyType, 15));
			}
			lootCashScale = gameDataTable.GetFloat((int)EnemyType, 16);
		}
		float num = 1f;
		if (!IsBoss())
		{
			float num2 = 1f;
			num = 1f;
			if (DifficultLevel >= 22)
			{
				num2 = (float)DifficultLevel * 2f;
			}
			else
			{
				for (int j = 0; j < DifficultLevel; j++)
				{
					num = 1.1f + 0.01f * (float)j;
					if (num > 1.25f)
					{
						num = 1.25f;
					}
					num2 *= num;
				}
			}
			for (int k = 0; k < mAttackDamages.Length; k++)
			{
				float num3 = (float)monsterConfig.AttackDamages[k] * num2;
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && GameApp.GetInstance().GetGameMode().IsCoopMode())
				{
					if (PlayerCount == 2)
					{
						num3 *= 1.2f;
					}
					else if (PlayerCount == 3)
					{
						num3 *= 1.5f;
					}
				}
				mAttackDamages[k] = (int)num3;
			}
			attackDamage = mAttackDamages[0];
			if (EnemySpawnScript.Instance != null)
			{
				mAcceleration = EnemySpawnScript.Instance.SpawnParameters.EnemyAcceleration;
			}
			if (EnemyType != EnemyType.SPECIAL_SLASHER)
			{
				mPlayerDamageType = (PlayerDamageType)(int)Enum.Parse(typeof(PlayerDamageType), gameDataTable.GetString((int)EnemyType, 19));
			}
		}
		else
		{
			if (GameApp.GetInstance().GetUserState().GetSelectedDifficultMode() == EDifficultMode.HEROIC)
			{
				monsterConfig.hp = (int)((float)monsterConfig.hp * BossParameter.BOSS_HARD_MODE_HP_SCALE);
			}
			int num4 = ((GameApp.GetInstance().GetUserState().GetSelectedDifficultMode() == EDifficultMode.HEROIC) ? 1 : 0);
			int num5 = (int)(mEnemyType - 9);
			GameDataTable gameDataTable2 = GameDataManager.GetInstance().GetGameDataTable(DataFileName.ENEMY_LIST, DataSheetName.ONLINE_BOSS_BONUS);
			for (int l = 0; l < gameDataTable2.Count; l++)
			{
				if (gameDataTable2.GetInt(l, 1) == num5 && gameDataTable2.GetInt(l, 2) == -1 && gameDataTable2.GetInt(l, 3) == num4)
				{
					monsterConfig.lootCash = gameDataTable2.GetInt(l, 9);
					break;
				}
			}
		}
		hp = monsterConfig.hp;
		lootCash = monsterConfig.lootCash;
		pickupRate = monsterConfig.pickupRate;
		pickupLower = monsterConfig.pickupLower;
		pickupUpper = monsterConfig.pickupUpper;
		exp = monsterConfig.exp;
		mRunSpeed = monsterConfig.walkSpeed;
		mCanAccelerate = monsterConfig.canAccelerate;
		attackFrequency = monsterConfig.AttackFrequency;
		int num6 = hp;
		float num7 = hp;
		float num8 = lootCash;
		float num9 = exp;
		num = 1f;
		float num10 = 1f;
		if (!IsBoss())
		{
			if (DifficultLevel >= 22)
			{
				num10 = DifficultLevel;
			}
			else
			{
				for (int m = 0; m < DifficultLevel; m++)
				{
					num = 1.25f - (float)m * 0.01f;
					if (num < 1.1f)
					{
						num = 1.1f;
					}
					num10 *= num;
				}
			}
		}
		num7 *= num10;
		num8 *= Mathf.Pow(1.07f, DifficultLevel);
		num8 = Mathf.Min(num8, (float)lootCash * 5f);
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			if (IsBoss())
			{
				switch (mGameWorld.PlayerCountForBoss)
				{
				case 2:
					num7 *= 2f;
					break;
				case 3:
					num7 *= 3f;
					break;
				}
			}
			else if (PlayerCount == 2)
			{
				num7 *= 1.6f;
				num8 *= 1.25f;
			}
			else if (PlayerCount == 3)
			{
				num7 *= 2f;
				num8 *= 1.5f;
			}
		}
		hp = (int)num7;
		lootCash = (int)num8;
		exp = (int)num9;
		maxHp = hp;
	}

	protected virtual void loadParameters()
	{
	}

	public virtual void Init(GameObject gObject)
	{
		mGameWorld = GameApp.GetInstance().GetGameWorld();
		mLocalPlayer = mGameWorld.GetLocalPlayer();
		SetObject(gObject);
		entityObject.tag = TagName.ENEMY_ROOT;
		base.Name = entityObject.name;
		rigidbody = entityObject.GetComponent<Rigidbody>();
		if (null != rigidbody)
		{
			rigidbody.useGravity = false;
			rigidbody.isKinematic = true;
			rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		}
		mUpdatePosInterval = 1f;
		attackRange = 2.5f;
		mTurnSpeed = 0.1f;
		if (EnemySpawnScript.Instance != null)
		{
			mMaxSpeed = EnemySpawnScript.Instance.SpawnParameters.MaxEnemySpeed;
		}
		mMaxAnimationSpeed = 2f;
		LoadConfig();
		criticalAttacked = false;
		if (entityObject.GetComponent<Animation>() != null)
		{
			animation.wrapMode = WrapMode.Loop;
			animation.Play(AnimationString.ENEMY_IDLE);
		}
		idleStartTime = -2f;
		catchPlayerTargetChangeTimer.SetTimer(0.5f, false);
		shoutAudioName = AudioName.ENEMY_AMBIENT1_DIGGER;
		lastReachPointTime = Time.time;
		recordTimer.SetTimer(3f, false);
		lastMoveTime = Time.time;
		lastPos = entityTransform.position;
		gotHitAnimationCDTimer = new Timer();
		gotHitAnimationCDTimer.SetTimer(0.2f, true);
		mSlowDownTimer.SetTimer(3f, false);
		mLookAtTimer.SetTimer(0.3f, true);
		mFindTargetTimer.SetTimer(5f, true);
		mHitTargetTimer.SetTimer(0.5f, true);
		mAccelerateTimer.SetTimer(10f, false);
		wayPointSqrDistance = 0.1f;
		lastBloodEffectTimer.SetTimer(0.3f, false);
		mCanTurn = false;
		mHasMission = false;
		if (MissionManager.GetInstance().IsProtection)
		{
			int num = UnityEngine.Random.Range(0, 100);
			if (num < 60)
			{
				mHasMission = true;
			}
		}
		for (int i = 0; i < entityObject.transform.childCount; i++)
		{
			Transform child = entityObject.transform.GetChild(i);
			if (child.GetComponent<Renderer>() != null)
			{
				mMaterial = child.GetComponent<Renderer>().material;
			}
		}
		if (!IsBoss())
		{
			Renderer[] componentsInChildren = gObject.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.gameObject.layer = PhysicsLayer.ENEMY;
			}
		}
		if (!GameApp.GetInstance().GetUserState().IsLowQuality())
		{
			BindFootParticles();
		}
	}

	protected void BindFootParticles()
	{
		string[] array = new string[4]
		{
			BoneName.LeftFoot,
			BoneName.RightFoot,
			BoneName.LeftHand,
			BoneName.RightHand
		};
		mFootParticles.Clear();
		string[] array2 = array;
		foreach (string name in array2)
		{
			Transform transform = entityObject.transform.Find(name);
			if (!(transform != null))
			{
				continue;
			}
			string name2 = "NV_Ground";
			if (transform.Find(name2) != null)
			{
				Debug.LogError("Already binded!");
				mFootParticles.Add(transform.Find(name2).gameObject);
				continue;
			}
			GameObject gameObject = Pool.sGetInstance().Allocate(EffectName.MONSTER_FOOT_PARTICLE);
			if (gameObject != null)
			{
				gameObject.transform.position = Vector3.zero;
				gameObject.transform.parent = transform;
				gameObject.name = name2;
				mFootParticles.Add(gameObject);
			}
		}
		foreach (GameObject mFootParticle in mFootParticles)
		{
			mFootParticle.transform.localPosition = Vector3.zero;
			mFootParticle.transform.localRotation = Quaternion.identity;
			mFootParticle.transform.localScale = Vector3.one;
			mFootParticle.SetActive(false);
		}
	}

	protected void DisplayFootParticles()
	{
		foreach (GameObject mFootParticle in mFootParticles)
		{
			mFootParticle.SetActive(true);
		}
	}

	protected void HideFootParticles()
	{
		foreach (GameObject mFootParticle in mFootParticles)
		{
			mFootParticle.SetActive(false);
		}
	}

	protected virtual void Destroy()
	{
		mGameWorld.GetEnemies().Remove(base.Name);
		entityObject.SetActive(false);
		entityObject.transform.position = new Vector3(0f, 1000f, 0f);
		foreach (GameObject mFootParticle in mFootParticles)
		{
			Pool.sGetInstance().Deallocate(mFootParticle);
		}
	}

	public virtual void SetState(EnemyState newState, bool changeConditionState = true)
	{
		if (mState != null)
		{
			mState.End(this);
		}
		mState = newState;
		IsStateRotateFinish = false;
		if (IsNewState && changeConditionState)
		{
			ChangeToConditionState(mStateSwitchDic[newState]);
		}
		if (mState != null)
		{
			mState.Start(this);
		}
		StateStartTime = Time.time;
		if (IsNewState && changeConditionState)
		{
			SetNextConditionState(mStateSwitchDic[newState]);
		}
	}

	public void SetState(string stateTag)
	{
		if (mStateTag.ContainsKey(stateTag))
		{
			ChangeToConditionState(mStateTag[stateTag]);
			SetState((EnemyState)mStateTag[stateTag].mState, false);
			SetNextConditionState(mStateTag[stateTag]);
		}
		else
		{
			Debug.LogError("No StateTag: " + stateTag);
		}
	}

	public void ChangeToConditionState(ConditionState state)
	{
		mLastConditionState = mConditionState;
		mConditionState = state;
	}

	public void SetNextConditionState(ConditionState state)
	{
		if (mConditionState != null)
		{
			for (int i = 0; i < mConditionState.mStartMessageList.Count; i++)
			{
				HandleMessage(mConditionState.mStartMessageList[i]);
			}
		}
		if (mLastConditionState != null)
		{
			for (int j = 0; j < mLastConditionState.mEndMessageList.Count; j++)
			{
				HandleMessage(mLastConditionState.mEndMessageList[j]);
			}
		}
	}

	public bool IsStateMatch(string tag)
	{
		return mStateTag.ContainsKey(tag) && mStateTag[tag] == mConditionState;
	}

	public EnemyState GetState()
	{
		return mState;
	}

	public virtual void CheckHit()
	{
		if (MeleeAttackSkill != null)
		{
			CheckHit2(MeleeAttackSkill);
		}
		else
		{
			Debug.LogError("NO MeleeAttackSkill");
		}
	}

	public virtual bool AttackAnimationEnds()
	{
		if (Time.time - lastAttackTime > entityObject.GetComponent<Animation>()[AnimationString.ENEMY_ATTACK].length)
		{
			return true;
		}
		return false;
	}

	public virtual bool AttackAnimationEnds(string name)
	{
		if (Time.time - lastAttackTime > entityObject.GetComponent<Animation>()[name].length)
		{
			return true;
		}
		return false;
	}

	public virtual void ChangeToHitState()
	{
		SetState(GOTHIT_STATE);
	}

	public virtual void ChangeToDeadState()
	{
		SetState(DEAD_STATE);
	}

	public override void OnHit(DamageProperty dp)
	{
		if (!InPlayingState() || Hp <= 0)
		{
			return;
		}
		hp -= dp.damage;
		if (dp.wType != WeaponType.NoGun)
		{
			mLocalPlayer.GetUserState().Achievement.CheckMaxDamage(dp.damage);
		}
		mLastDamageProperty = dp;
		if (dp.wType != WeaponType.NoGun && dp.isLocal)
		{
			mLocalPlayer.HitEnemy();
		}
		if (hp <= 0)
		{
			mLocalPlayer.PVEStatistics.Add(mEnemyType, IsElite);
			mLocalPlayer.Kills++;
			isPresentLooted = mLocalPlayer.GetSkills().PlayerKillAmountOfEnemy(this);
			if (dp.wType == WeaponType.UltraSniper)
			{
				UpdateHitBySniper(mLocalPlayer);
			}
		}
		if (IsElite)
		{
			criticalAttacked = false;
		}
		else
		{
			criticalAttacked = dp.criticalAttack;
		}
		if (mState != ROUTE_STATE && mState != BLACK_HOLE_STATE && mState != STUN_STATE)
		{
			ChangeToHitState();
		}
	}

	public virtual void OnHitResponse(int newHp, int killerID, bool criticalAttack, WeaponType weaponType)
	{
		if (Hp <= 0)
		{
			return;
		}
		Hp = newHp;
		if (Hp > 0)
		{
			if (InPlayingState())
			{
				if (lastBloodEffectTimer.Ready())
				{
					mGameWorld.GetHitBloodPool().CreateObject(GetPosition(), Vector3.zero, Quaternion.identity);
					lastBloodEffectTimer.Do();
				}
				if (mState != ROUTE_STATE && mState != BLACK_HOLE_STATE && mState != STUN_STATE)
				{
					ChangeToHitState();
				}
			}
		}
		else
		{
			if (Hp != 0)
			{
				return;
			}
			if (killerID == Lobby.GetInstance().GetChannelID())
			{
				Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
				localPlayer.RecoveryWhenMakeKills();
				localPlayer.Kills++;
				isPresentLooted = localPlayer.GetSkills().PlayerKillAmountOfEnemy(this);
				if (weaponType == WeaponType.UltraSniper)
				{
					UpdateHitBySniper(localPlayer);
				}
			}
			else
			{
				Player remotePlayerByUserID = mGameWorld.GetRemotePlayerByUserID(killerID);
				if (remotePlayerByUserID != null && weaponType == WeaponType.UltraSniper)
				{
					UpdateHitBySniper(remotePlayerByUserID);
				}
			}
			if (killerID == Lobby.GetInstance().GetChannelID() || IsBoss())
			{
				AddCashAndExpToPlayer();
				mLocalPlayer.PVEStatistics.Add(mEnemyType, IsElite);
			}
			GameApp.GetInstance().GetGameWorld().TotalEnemyKilled++;
			SetCriticalAttack(criticalAttack);
			ChangeToDeadState();
		}
	}

	public virtual void SetCriticalAttack(bool criticalAttack)
	{
		if (IsElite)
		{
			criticalAttacked = false;
		}
		else
		{
			criticalAttacked = criticalAttack;
		}
	}

	public virtual void OnIdle()
	{
		SetState(CATCHING_STATE);
	}

	public virtual void OnAttack()
	{
	}

	public Player GetTargetPlayer()
	{
		return targetPlayer;
	}

	public void ChangeTargetPlayer(int targetID)
	{
		if (targetID != 0)
		{
			if (Lobby.GetInstance().GetChannelID() == targetID)
			{
				mTarget = mLocalPlayer;
			}
			else
			{
				mTarget = GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(targetID);
			}
			if (mTarget != null)
			{
				mTargetPosition = mTarget.GetTransform().position;
			}
		}
	}

	public virtual void FindNewTarget()
	{
	}

	public void AssignMission()
	{
		mTarget = mGameWorld.GetRandomProtectedItem();
		if (mTarget == null)
		{
			mHasMission = false;
			ChooseTargetPlayer(false);
		}
	}

	protected virtual void ChooseTargetPlayer(bool force)
	{
		if (mHasMission || (!force && !mFindTargetTimer.Ready()))
		{
			return;
		}
		mFindTargetTimer.Do();
		Player player = mLocalPlayer;
		float num = Vector3.Distance(mLocalPlayer.GetTransform().position, entityTransform.position);
		if (!mLocalPlayer.InPlayingState())
		{
			num = 99999f;
		}
		List<RemotePlayer> remotePlayers = mGameWorld.GetRemotePlayers();
		foreach (RemotePlayer item in remotePlayers)
		{
			if (item.InPlayingState())
			{
				float num2 = Vector3.Distance(item.GetTransform().position, entityTransform.position);
				if (num2 < num)
				{
					player = item;
					num = num2;
				}
			}
		}
		List<Robot> robotList = mGameWorld.GetRobotList();
		foreach (Robot item2 in robotList)
		{
			if (item2 != null && item2.InPlayingState())
			{
				float num3 = Vector3.Distance(item2.GetPosition(), entityTransform.position);
				if (num3 < num)
				{
					player = item2;
					num = num3;
				}
			}
		}
		if (player != null && player.InPlayingState())
		{
			mTarget = player;
			mTargetPosition = player.GetTransform().position;
			if (GameApp.GetInstance().GetGameMode().IsCoopMode())
			{
				EnemyChangeTargetRequest request = new EnemyChangeTargetRequest(EnemyID, player.GetChannelID(), entityTransform.position);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
		else
		{
			mTarget = null;
		}
	}

	public int GetRandomPlayer()
	{
		List<int> list = new List<int>();
		if (mLocalPlayer.InPlayingState())
		{
			list.Add(Lobby.GetInstance().GetChannelID());
		}
		List<RemotePlayer> remotePlayers = mGameWorld.GetRemotePlayers();
		foreach (RemotePlayer item in remotePlayers)
		{
			if (item.InPlayingState())
			{
				list.Add(item.GetChannelID());
			}
		}
		if (list.Count == 0)
		{
			return Lobby.GetInstance().GetChannelID();
		}
		int index = UnityEngine.Random.Range(0, list.Count);
		return list[index];
	}

	public int GetNearestPlayer()
	{
		int channelID = Lobby.GetInstance().GetChannelID();
		float num = Vector3.Distance(mLocalPlayer.GetTransform().position, entityTransform.position);
		if (!mLocalPlayer.InPlayingState())
		{
			num = 99999f;
		}
		List<RemotePlayer> remotePlayers = mGameWorld.GetRemotePlayers();
		foreach (RemotePlayer item in remotePlayers)
		{
			if (item.InPlayingState())
			{
				float num2 = Vector3.Distance(item.GetTransform().position, entityTransform.position);
				if (num2 < num)
				{
					channelID = item.GetChannelID();
					num = num2;
				}
			}
		}
		return channelID;
	}

	public int GetFarthestPlayer()
	{
		int channelID = Lobby.GetInstance().GetChannelID();
		float num = Vector3.Distance(mLocalPlayer.GetTransform().position, entityTransform.position);
		if (!mLocalPlayer.InPlayingState())
		{
			num = 0f;
		}
		List<RemotePlayer> remotePlayers = mGameWorld.GetRemotePlayers();
		foreach (RemotePlayer item in remotePlayers)
		{
			if (item.InPlayingState())
			{
				float num2 = Vector3.Distance(item.GetTransform().position, entityTransform.position);
				if (num2 > num)
				{
					channelID = item.GetChannelID();
					num = num2;
				}
			}
		}
		return channelID;
	}

	public GameUnit GetCurrentTarget()
	{
		return mTarget;
	}

	public virtual void LookAtPointHorizontal(Vector3 targetPoint)
	{
		Vector3 targetPoint2 = targetPoint;
		targetPoint2.y = entityTransform.position.y;
		LookAtPoint(targetPoint2);
	}

	public virtual void LookAtPoint(Vector3 targetPoint)
	{
		if (mLookAtTimer.Ready())
		{
			mLookAtTimer.Do();
			mTargetToLookAt = targetPoint;
			Vector3 vector = mTargetToLookAt - entityTransform.position;
			float f = Vector3.Angle(vector, entityTransform.forward);
			float num = Mathf.Abs(f) * ((float)Math.PI / 180f);
			mMaxTurnRadian = num * mTurnSpeed;
			SetCanTurn(true);
		}
	}

	public virtual void LookAtPointHorizontalAtOnce(Vector3 targetPoint)
	{
		Vector3 targetPoint2 = targetPoint;
		targetPoint2.y = entityTransform.position.y;
		LookAtPointAtOnce(targetPoint2);
	}

	public virtual void LookAtPointAtOnce(Vector3 targetPoint)
	{
		entityTransform.LookAt(targetPoint);
	}

	public void ChangeTarget(GameUnit target)
	{
		mTarget = target;
		if (mTarget != null && mTarget.InPlayingState() && mTarget.GetTransform() != null)
		{
			mTargetPosition = mTarget.GetTransform().position;
		}
	}

	public virtual void LookAtTargetHorizontal()
	{
		LookAtPointHorizontal(mTargetPosition);
	}

	public virtual void LookAtTarget()
	{
		LookAtPoint(mTargetPosition);
	}

	protected List<Player> GetPotentialPlayerList()
	{
		List<Player> list = new List<Player>();
		if (mLocalPlayer.InPlayingState())
		{
			list.Add(mLocalPlayer);
		}
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			List<RemotePlayer> remotePlayers = mGameWorld.GetRemotePlayers();
			foreach (RemotePlayer item in remotePlayers)
			{
				if (item.InPlayingState())
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	private List<GameUnit> GetPotentialTargetList()
	{
		List<GameUnit> list = new List<GameUnit>();
		if (mLocalPlayer.InPlayingState())
		{
			list.Add(mLocalPlayer);
		}
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			List<RemotePlayer> remotePlayers = mGameWorld.GetRemotePlayers();
			foreach (RemotePlayer item in remotePlayers)
			{
				if (item.InPlayingState())
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	public GameUnit GetRandomTarget()
	{
		List<GameUnit> potentialTargetList = GetPotentialTargetList();
		if (potentialTargetList.Count > 0)
		{
			int index = UnityEngine.Random.Range(0, potentialTargetList.Count);
			return potentialTargetList[index];
		}
		return null;
	}

	public GameUnit GetNearestTarget()
	{
		GameUnit result = null;
		float num = 99999f;
		List<GameUnit> potentialTargetList = GetPotentialTargetList();
		foreach (GameUnit item in potentialTargetList)
		{
			float num2 = Vector3.Distance(item.GetTransform().position, entityTransform.position);
			if (num2 < num)
			{
				result = item;
				num = num2;
			}
		}
		return result;
	}

	public GameUnit GetFarthestTarget()
	{
		GameUnit result = null;
		float num = 0f;
		List<GameUnit> potentialTargetList = GetPotentialTargetList();
		foreach (GameUnit item in potentialTargetList)
		{
			float num2 = Vector3.Distance(item.GetTransform().position, entityTransform.position);
			if (num2 > num)
			{
				result = item;
				num = num2;
			}
		}
		return result;
	}

	public virtual Vector3 GetAveragePosition()
	{
		if (GameApp.GetInstance().GetGameMode().IsSingle())
		{
			return mLocalPlayer.GetTransform().position;
		}
		int num = 0;
		Vector3 vector = Vector3.zero;
		List<Player> potentialPlayerList = GetPotentialPlayerList();
		foreach (Player item in potentialPlayerList)
		{
			if (item.InPlayingState())
			{
				num++;
				vector += item.GetTransform().position;
			}
		}
		if (num > 0)
		{
			mRay = new Ray(vector + 3f * Vector3.up, Vector3.down);
			int num2 = 1 << PhysicsLayer.FLOOR;
			vector = ((!Physics.Raycast(mRay, out mRaycastHit, 10f)) ? mTargetPosition : (mRaycastHit.point + new Vector3(0f, 0.1f, 0f)));
		}
		return (num != 0) ? vector : mLocalPlayer.GetTransform().position;
	}

	public virtual float GetAveragehorizontalDistance()
	{
		if (GameApp.GetInstance().GetGameMode().IsSingle())
		{
			return GetDistanceFromLocalPlayer();
		}
		int num = 0;
		float num2 = 0f;
		List<Player> potentialPlayerList = GetPotentialPlayerList();
		foreach (Player item in potentialPlayerList)
		{
			if (item.InPlayingState())
			{
				num2 += Vector3.Distance(new Vector3(item.GetTransform().position.x, 0f, item.GetTransform().position.z), new Vector3(entityTransform.position.x, 0f, entityTransform.position.z));
				num++;
			}
		}
		return (num != 0) ? (num2 / (float)num) : 0f;
	}

	public virtual float GetNearestDistanceToTargetPlayer()
	{
		if (GameApp.GetInstance().GetGameMode().IsSingle())
		{
			return 99999f;
		}
		float num = 99999f;
		List<Player> potentialPlayerList = GetPotentialPlayerList();
		foreach (Player item in potentialPlayerList)
		{
			if (item.InPlayingState() && item != mTarget)
			{
				float num2 = Vector3.Distance(item.GetTransform().position, mTargetPosition);
				if (num2 < num)
				{
					num = num2;
				}
			}
		}
		return num;
	}

	protected virtual bool RaycastTargetPlayer(out RaycastHit rayhit)
	{
		mRay = new Ray(entityTransform.position + new Vector3(0f, 0.5f, 0f), mTargetPosition + new Vector3(0f, 0.5f, 0f) - (entityTransform.position + new Vector3(0f, 0.5f, 0f)));
		return Physics.Raycast(mRay, out rayhit, 100f, (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.TRANSPARENT_WALL) | (1 << PhysicsLayer.PLAYER) | (1 << PhysicsLayer.REMOTE_PLAYER));
	}

	protected bool canHitTarget()
	{
		if (mHitTargetTimer.Ready())
		{
			if (SqrDistanceFromPlayer < (GetHitColliderRadius() + 1f) * (GetHitColliderRadius() + 1f))
			{
				return true;
			}
			mHitTargetTimer.Do();
			Vector3 direction = mTargetPosition - entityTransform.position;
			mRay = new Ray(entityTransform.position + new Vector3(0f, 0.5f, 0f), direction);
			int layerMask = (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.TRANSPARENT_WALL) | (1 << PhysicsLayer.PLAYER) | (1 << PhysicsLayer.REMOTE_PLAYER) | (1 << PhysicsLayer.PROTECTED_ITEM) | (1 << PhysicsLayer.ROBOT);
			if (Physics.Raycast(mRay, out mRaycastHit, GetDistanceFromTarget(), layerMask) && (mRaycastHit.collider.gameObject.layer == PhysicsLayer.PLAYER || mRaycastHit.collider.gameObject.layer == PhysicsLayer.REMOTE_PLAYER || mRaycastHit.collider.gameObject.layer == PhysicsLayer.PROTECTED_ITEM || mRaycastHit.collider.gameObject.layer == PhysicsLayer.ROBOT))
			{
				return true;
			}
		}
		return false;
	}

	protected virtual bool GetHitPoint(out Vector3 hitPoint)
	{
		hitPoint = Vector3.zero;
		Camera main = Camera.main;
		Transform transform = main.transform;
		ThirdPersonStandardCameraScript component = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
		Vector3 vector = main.ScreenToWorldPoint(new Vector3(component.ReticlePosition.x, (float)Screen.height - component.ReticlePosition.y, 0.1f));
		Ray ray = new Ray(transform.position, vector - transform.position);
		RaycastHit hitInfo = default(RaycastHit);
		if (Physics.Raycast(ray, out hitInfo, 100f, 1 << PhysicsLayer.ENEMY))
		{
			hitPoint = hitInfo.point;
			return true;
		}
		ray = new Ray(mLocalPlayer.GetTransform().position + Vector3.up * 0.5f, bipObject.transform.position + Vector3.up * UnityEngine.Random.Range(-1f, 1f) + Vector3.right * UnityEngine.Random.Range(-1f, 1f) + Vector3.forward * UnityEngine.Random.Range(-1f, 1f) - mLocalPlayer.GetTransform().position - Vector3.up * 0.5f);
		hitInfo = default(RaycastHit);
		if (Physics.Raycast(ray, out hitInfo, 100f, 1 << PhysicsLayer.ENEMY))
		{
			hitPoint = hitInfo.point;
			return true;
		}
		return false;
	}

	protected bool GetHitResponsePoint(out Vector3 hitPoint)
	{
		hitPoint = Vector3.zero;
		if (!mLocalPlayer.InPlayingState())
		{
			return false;
		}
		bool flag = true;
		if (mGameWorld.GetPlayingPlayerCount() > 1 && mLocalPlayer.State != Player.ATTACK_STATE)
		{
			flag = false;
		}
		if (flag)
		{
			return GetHitPoint(out hitPoint);
		}
		RemotePlayer remotePlayer = null;
		List<RemotePlayer> remotePlayers = mGameWorld.GetRemotePlayers();
		foreach (RemotePlayer item in remotePlayers)
		{
			if (item.InPlayingState() && item.State == Player.ATTACK_STATE)
			{
				remotePlayer = item;
				break;
			}
		}
		if (remotePlayer != null)
		{
			Ray ray = new Ray(remotePlayer.GetTransform().position + Vector3.up * 0.5f, bipObject.transform.position + Vector3.up * UnityEngine.Random.Range(-1f, 1f) + Vector3.right * UnityEngine.Random.Range(-1f, 1f) + Vector3.forward * UnityEngine.Random.Range(-1f, 1f) - remotePlayer.GetTransform().position - Vector3.up * 0.5f);
			RaycastHit hitInfo = default(RaycastHit);
			if (Physics.Raycast(ray, out hitInfo, 100f, 1 << PhysicsLayer.ENEMY))
			{
				hitPoint = hitInfo.point;
				return true;
			}
		}
		return false;
	}

	public int GetLoot()
	{
		float num = UnityEngine.Random.Range(pickupLower, pickupUpper);
		num *= lootCashScale;
		float a = Mathf.Pow(1.07f, DifficultLevel);
		a = Mathf.Min(a, 5f);
		return (int)(num * a);
	}

	public void AddCashAndExpToPlayer()
	{
		mLocalPlayer.MakeCombo();
		if (mLocalPlayer.StayInEnemiesArea)
		{
			mLocalPlayer.KillsInArea++;
		}
		if (GameApp.GetInstance().GetGameMode().IsSingle())
		{
			MissionManager.GetInstance().CheckWeaponKillEnemy(mLastDamageProperty);
			MissionManager.GetInstance().CheckSpecialEnemy(mEnemyType, IsElite);
			if (mLocalPlayer.GetSkills().GetSkill(SkillsType.HP_RECOVERY_WHEN_KILL_BUGS) > 0f)
			{
				mLocalPlayer.RecoverHP(mLocalPlayer.GetSkills().GetSkill(SkillsType.HP_RECOVERY_WHEN_KILL_BUGS));
			}
		}
		float num = 0f;
		if (mLocalPlayer.GetCombo() >= 10)
		{
			num = 0.6f;
		}
		else if (mLocalPlayer.GetCombo() >= 8)
		{
			num = 0.4f;
		}
		else if (mLocalPlayer.GetCombo() >= 5)
		{
			num = 0.2f;
		}
		else if (mLocalPlayer.GetCombo() >= 3)
		{
			num = 0.1f;
		}
		if (IsBoss())
		{
			if (GameApp.GetInstance().GetGameMode().IsSingle())
			{
				mLocalPlayer.AddBossCash(lootCash);
			}
		}
		else
		{
			mLocalPlayer.AddMonsterCash(lootCash);
			mLocalPlayer.AddBounsCash((int)((float)lootCash * num));
		}
		GameApp.GetInstance().GetUserState().Achievement.AddEnemyKills(this);
	}

	public virtual bool OnSpecialState(float deltaTime)
	{
		return false;
	}

	public virtual void DoMove(float deltaTime)
	{
		entityTransform.Translate(dir * mRunSpeed * deltaTime, Space.World);
		Vector3 ground = GetGround();
		Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, ground);
		entityTransform.rotation = quaternion * enemyRotation;
	}

	public float GetPlayerHorizontalAbsAngle()
	{
		Vector3 to = new Vector3(entityTransform.forward.x, 0f, entityTransform.forward.z);
		Vector3 vector = mLocalPlayer.GetTransform().position - entityTransform.position;
		vector.y = 0f;
		float f = Vector3.Angle(vector, to);
		return Mathf.Abs(f);
	}

	public float GetPlayerHorizontalAngle()
	{
		Vector3 vector = new Vector3(entityTransform.forward.x, 0f, entityTransform.forward.z);
		Vector3 vector2 = mLocalPlayer.GetTransform().position - entityTransform.position;
		vector2.y = 0f;
		float num = Vector3.Angle(vector2, vector);
		if (Vector3.Cross(vector, vector2).y < 0f)
		{
			num = 0f - num;
		}
		return num;
	}

	public float GetTargetHorizontalAngle()
	{
		Vector3 vector = new Vector3(entityTransform.forward.x, 0f, entityTransform.forward.z);
		Vector3 vector2 = mTargetPosition - entityTransform.position;
		vector2.y = 0f;
		float num = Vector3.Angle(vector2, vector);
		if (Vector3.Cross(vector, vector2).y < 0f)
		{
			num = 0f - num;
		}
		return num;
	}

	public float GetDistanceFromGameUnit(GameUnit gameUnit)
	{
		if (gameUnit == null || gameUnit.GetTransform() == null || entityTransform == null)
		{
			return 99999f;
		}
		return (entityTransform.position - gameUnit.GetTransform().position).magnitude;
	}

	public float GetDistanceFromLocalPlayer()
	{
		return GetDistanceFromGameUnit(mLocalPlayer);
	}

	public float GetDistanceFromTarget()
	{
		return (entityTransform.position - mTargetPosition).magnitude;
	}

	public float GetHorizontalDistanceFromTarget()
	{
		Vector3 vector = mTargetPosition - entityTransform.position;
		vector.y = 0f;
		return vector.magnitude;
	}

	public float GetHorizontalDistanceFromLocalPlayer()
	{
		return GetHorizontalDistanceFromGameUnit(mLocalPlayer);
	}

	public float GetHorizontalDistanceFromGameUnit(GameUnit gameUnit)
	{
		if (gameUnit == null || gameUnit.GetTransform() == null || entityTransform == null)
		{
			return 99999f;
		}
		Vector3 vector = gameUnit.GetTransform().position - entityTransform.position;
		vector.y = 0f;
		return vector.magnitude;
	}

	public float GetSqrDistanceFromGameUnit(GameUnit gameUnit)
	{
		if (gameUnit == null || gameUnit.GetTransform() == null || entityTransform == null)
		{
			return 99999f;
		}
		return (entityTransform.position - gameUnit.GetTransform().position).sqrMagnitude;
	}

	public float GetSqrDistanceFromLocalPlayer()
	{
		return GetSqrDistanceFromGameUnit(mLocalPlayer);
	}

	public float GetSqrDistanceFromTarget()
	{
		return (entityTransform.position - mTargetPosition).sqrMagnitude;
	}

	public float GetHorizontalSqrDistanceFromGameUnit(GameUnit gameUnit)
	{
		if (gameUnit == null || gameUnit.GetTransform() == null || entityTransform == null)
		{
			return 99999f;
		}
		Vector3 vector = gameUnit.GetTransform().position - entityTransform.position;
		vector.y = 0f;
		return vector.sqrMagnitude;
	}

	public float GetHorizontalSqrDistanceFromLocalPlayer()
	{
		return GetHorizontalSqrDistanceFromGameUnit(mLocalPlayer);
	}

	public float GetHorizontalSqrDistanceFromTarget()
	{
		Vector3 vector = mTargetPosition - entityTransform.position;
		vector.y = 0f;
		return vector.sqrMagnitude;
	}

	public float GetGameUnitHorizontalAngle(GameUnit gameUnit)
	{
		if (gameUnit == null || gameUnit.GetTransform() == null || entityTransform == null)
		{
			return 0f;
		}
		Vector3 vector = new Vector3(entityTransform.forward.x, 0f, entityTransform.forward.z);
		Vector3 vector2 = gameUnit.GetTransform().position - entityTransform.position;
		vector2.y = 0f;
		float num = Vector3.Angle(vector2, vector);
		if (Vector3.Cross(vector, vector2).y < 0f)
		{
			num = 0f - num;
		}
		return num;
	}

	public float GetLocalPlayerHorizontalAngle()
	{
		return GetGameUnitHorizontalAngle(mLocalPlayer);
	}

	public float GetGameUnitHorizontalAbsAngle(GameUnit gameUnit)
	{
		if (gameUnit == null || gameUnit.GetTransform() == null || entityTransform == null)
		{
			return 0f;
		}
		Vector3 to = new Vector3(entityTransform.forward.x, 0f, entityTransform.forward.z);
		Vector3 vector = gameUnit.GetTransform().position - entityTransform.position;
		vector.y = 0f;
		return Vector3.Angle(vector, to);
	}

	public float GetLocalPlayerHorizontalAbsAngle()
	{
		return GetGameUnitHorizontalAbsAngle(mLocalPlayer);
	}

	public float GetTargetHorizontalAbsAngle()
	{
		Vector3 to = new Vector3(entityTransform.forward.x, 0f, entityTransform.forward.z);
		Vector3 vector = mTargetPosition - entityTransform.position;
		vector.y = 0f;
		return Vector3.Angle(vector, to);
	}

	public float GetTargetVerticalAbsAngle(Vector3 pos)
	{
		Vector3 to = new Vector3(0f, entityTransform.forward.y, entityTransform.forward.z);
		return Vector3.Angle(pos, to);
	}

	public virtual void DoShoutAudio()
	{
		if (mShoutAudioTimer != null && mShoutAudioTimer.Ready() && InPlayingState())
		{
			PlaySound(shoutAudioName);
			mShoutAudioTimer.Do();
		}
	}

	public virtual void UpdatePosition(Vector3 position, EnemyStateConst nextState)
	{
		if (!base.IsMasterPlayer && InPlayingState() && entityTransform != null)
		{
			mDeltaPosition = position - entityTransform.position;
			mCurrentDeltaCount = 0;
		}
	}

	public virtual void DoLogic(float deltaTime)
	{
		if (base.CurrentAnimationName != null)
		{
			LastAnimTime = CurrentAnimTime;
			CurrentAnimTime = animation[base.CurrentAnimationName].normalizedTime;
			if (animation[base.CurrentAnimationName].wrapMode == WrapMode.Loop)
			{
				CurrentAnimTime %= 1f;
			}
		}
		if (mTarget != null)
		{
			if (mTarget.InPlayingState() && mTarget.GetTransform() != null)
			{
				if (NeedUpdateTargetPosition)
				{
					mTargetPosition = mTarget.GetTransform().position;
				}
			}
			else
			{
				mTarget = null;
			}
		}
		if (!base.IsMasterPlayer && mCurrentDeltaCount < mMaxDeltaCount && mRemoteEnemyMove)
		{
			entityTransform.position += mDeltaPosition / mMaxDeltaCount;
			mCurrentDeltaCount++;
		}
		if (CanTurn())
		{
			Vector3 target = mTargetToLookAt - entityTransform.position;
			Vector3 vector = Vector3.RotateTowards(entityTransform.forward, target, mMaxTurnRadian, 100f);
			vector.y = 0f;
			entityTransform.LookAt(entityTransform.position + vector);
		}
		if (mCanAccelerate && mAccelerateTimer.Ready())
		{
			mAccelerateTimer.Do();
			if (mRunSpeed < mMaxSpeed)
			{
				mRunSpeed += mAcceleration;
				if (null != mNavMeshAgent)
				{
					mNavMeshAgent.speed += mAcceleration;
				}
			}
			if (animation != null)
			{
				if (animation[AnimationString.ENEMY_RUN] != null && animation[AnimationString.ENEMY_RUN].speed < mMaxAnimationSpeed)
				{
					animation[AnimationString.ENEMY_RUN].speed += 0.2f;
				}
				if ((bool)animation[AnimationString.ENEMY_ATTACK] && animation[AnimationString.ENEMY_ATTACK].speed < mMaxAnimationSpeed)
				{
					animation[AnimationString.ENEMY_ATTACK].speed += 0.2f;
				}
			}
		}
		if (hp <= 0 && InPlayingState())
		{
			ChangeToDeadState();
		}
		if (IsNewState)
		{
			mConditionState.Update(this);
		}
		else if (mState != null)
		{
			mState.Update(this, deltaTime);
		}
		DoShoutAudio();
		if (mIsSlowDown && mSlowDownTimer.Ready())
		{
			mIsSlowDown = false;
			mRunSpeed /= 0.7f;
		}
	}

	public new bool AnimationPlayed(string name, float percent)
	{
		if (animation[name].time >= animation[name].clip.length * percent && animation.IsPlaying(name))
		{
			return true;
		}
		return false;
	}

	public bool isAllPlayerDead()
	{
		bool flag = true;
		if (GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.InPlayingState())
		{
			flag = false;
		}
		if (flag)
		{
			List<RemotePlayer> remotePlayers = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
			foreach (RemotePlayer item in remotePlayers)
			{
				if (item.InPlayingState())
				{
					flag = false;
					break;
				}
			}
		}
		return flag;
	}

	public virtual bool RemoveDeadBodyTimer()
	{
		if (deadRemoveBodyTimer.Ready())
		{
			mGameWorld.GetEnemies().Remove(base.Name);
			entityObject.SetActive(false);
			entityObject.transform.position = new Vector3(0f, 1000f, 0f);
		}
		return true;
	}

	public virtual Vector3 GetGround()
	{
		mFloorHeight = Global.FLOORHEIGHT;
		Vector3 vector = Vector3.up;
		mRay = new Ray(entityTransform.position + new Vector3(0f, 5f, 0f), Vector3.down);
		if (Physics.Raycast(mRay, out mRaycastHit, 100f, 1 << PhysicsLayer.FLOOR))
		{
			mFloorHeight = mRaycastHit.point.y;
			vector = mRaycastHit.normal;
		}
		if (vector.y < 0f)
		{
			vector = -vector;
		}
		return vector;
	}

	public float GetGroundHeight()
	{
		mFloorHeight = Global.FLOORHEIGHT;
		Vector3 up = Vector3.up;
		mRay = new Ray(entityTransform.position + new Vector3(0f, 5f, 0f), Vector3.down);
		if (Physics.Raycast(mRay, out mRaycastHit, 100f, 1 << PhysicsLayer.FLOOR))
		{
			mFloorHeight = mRaycastHit.point.y;
		}
		return mFloorHeight;
	}

	public virtual bool NeedShowDefent()
	{
		return false;
	}

	public float GetFloorHeight()
	{
		return mFloorHeight;
	}

	public override void CheckKnocked(float speed)
	{
		Vector3 vector = new Vector3(mLocalPlayer.GetTransform().forward.x, 0f, mLocalPlayer.GetTransform().forward.z);
		Vector3 to = entityTransform.position - mLocalPlayer.GetTransform().position;
		to.y = 0f;
		float f = Vector3.Angle(vector, to);
		if (Mathf.Abs(f) < 90f)
		{
			speed = 0f - speed;
		}
		mLocalPlayer.OnKnocked(speed);
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			PlayerOnKnockedRequest request = new PlayerOnKnockedRequest(speed, mLocalPlayer.GetChannelID());
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
	}

	public override void CheckKnocked(float speed, GameUnit target)
	{
		if (target.GameUnitType == EGameUnitType.LOCAL_PLAYER)
		{
			Player player = target as Player;
			Vector3 vector = new Vector3(target.GetTransform().forward.x, 0f, target.GetTransform().forward.z);
			Vector3 to = entityTransform.position - target.GetTransform().position;
			to.y = 0f;
			float f = Vector3.Angle(vector, to);
			if (Mathf.Abs(f) < 90f)
			{
				speed = 0f - speed;
			}
			player.OnKnocked(speed);
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				PlayerOnKnockedRequest request = new PlayerOnKnockedRequest(speed, ((Player)target).GetChannelID());
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
	}

	public static GameObject GetRootObjectByCollider(Collider c)
	{
		GameObject gameObject = c.gameObject;
		while (gameObject.transform.parent != null && !(gameObject.tag == TagName.ENEMY_ROOT) && !(gameObject.tag == TagName.PLAYER_ROOT))
		{
			gameObject = gameObject.transform.parent.gameObject;
		}
		return gameObject;
	}

	public void SlowDownEffect()
	{
		mSlowDownTimer.Do();
		if (!mIsSlowDown)
		{
			mIsSlowDown = true;
			mRunSpeed *= 0.7f;
		}
	}

	public void HitEnemy(Player attacker, DamageProperty dp)
	{
		if (mIsInvincible)
		{
			return;
		}
		if (IsBoss())
		{
			dp.buff += attacker.GetSkills().GetSkill(SkillsType.BOSS_HUNTER);
		}
		else
		{
			dp.buff += attacker.GetSkills().GetSkill(SkillsType.MONSTER_HUNTER);
		}
		dp.buff += attacker.GetSkills().GetDamageBoothDependsOnState(this);
		float num = Mathf.Max(0f, 1f + attacker.GetSkills().GetSkill(SkillsType.FINAL_DAMAGE));
		dp.damage = (int)((float)dp.damage * (1f + dp.buff) / (1f + dp.debuff) * num);
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			if (dp.isLocal && GetState() != GRAVEBORN_STATE)
			{
				int num2 = dp.damage;
				if (NeedShowDefent())
				{
					num2 /= 10;
				}
				else if (EnemyType == EnemyType.FROST_STONE && ((FrostStone)this).HasIceArmor)
				{
					num2 = (int)((float)num2 * FrostStoneParameter.ICE_ARMOR_DECREASE_DAMAGE);
				}
				else if (EnemyType == EnemyType.DEMSTER && NeedShowDefent())
				{
					num2 = (int)((float)num2 * 0.2f);
				}
				if (IsBoss())
				{
					int num3 = Mathf.Min(hp, num2);
					mGameWorld.BossTotalDamage += num3;
				}
				if (dp.wType != WeaponType.NoGun)
				{
					mLocalPlayer.HitEnemy();
				}
				EnemyOnHitRequest request = new EnemyOnHitRequest(EnemyID, num2, (byte)dp.wType, dp.criticalAttack, dp.weaponid);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
		else
		{
			if (attacker.GetSkills().HasSkill(SkillsType.HEALTH_STEAL))
			{
				attacker.RecoverHP(attacker.GetSkills().GetSkill(SkillsType.HEALTH_STEAL) * (float)dp.damage);
			}
			if (NeedShowDefent())
			{
				dp.damage /= 10;
			}
			else if (EnemyType == EnemyType.FROST_STONE && ((FrostStone)this).HasIceArmor)
			{
				dp.damage = (int)((float)dp.damage * FrostStoneParameter.ICE_ARMOR_DECREASE_DAMAGE);
			}
			else if (EnemyType == EnemyType.DEMSTER && NeedShowDefent())
			{
				dp.damage = (int)((float)dp.damage * 0.2f);
			}
			OnHit(dp);
		}
		if (!IsBoss() && GetLastBloodEffectTimer().Ready())
		{
			mGameWorld.GetHitBloodPool().CreateObject(dp.hitpoint, Vector3.zero, Quaternion.identity);
			GetLastBloodEffectTimer().Do();
		}
		if (attacker == null || !attacker.InPlayingState())
		{
			return;
		}
		attacker.GetSkills().PlayerHitTheTargetByDamageType(dp);
		if (dp.wType != WeaponType.NoGun)
		{
			int hitTriggeredSkillToSelf = attacker.GetWeapon().GetHitTriggeredSkillToSelf();
			if (hitTriggeredSkillToSelf != 0)
			{
				attacker.TriggerSkillFromOtherPlayer(hitTriggeredSkillToSelf);
			}
		}
	}

	public override void OnHitTarget(GameUnit target, CreatureSkill skill, int Count)
	{
		DamageProperty dp = new DamageProperty(this, skill.Damage, skill.PlayerDamageType);
		target.OnHit(dp);
	}

	public virtual Vector3 GetColliderCenterPosition()
	{
		return entityTransform.position + Vector3.up * 0.8f;
	}

	public virtual bool CanTurn()
	{
		return mCanTurn && mState != DEAD_STATE;
	}

	public override void CreateNavMeshAgent()
	{
		base.CreateNavMeshAgent();
		mNavMeshAgent.speed = mRunSpeed;
	}

	protected List<GameUnit> GetTargetList()
	{
		mTargetList.Clear();
		mTargetList.Add(mLocalPlayer);
		if (base.IsMasterPlayer)
		{
			foreach (Robot robot in mGameWorld.GetRobotList())
			{
				if (robot.InPlayingState())
				{
					mTargetList.Add(robot);
				}
			}
			foreach (ProtectedItem protectedItem in mGameWorld.GetProtectedItemList())
			{
				if (protectedItem.InPlayingState())
				{
					mTargetList.Add(protectedItem);
				}
			}
		}
		return mTargetList;
	}

	public virtual void StartCatching()
	{
		SetCatchingTimeNow();
		SetUpdatePosTimeNow();
		UpdateNavMeshInCatching(true);
		DisplayFootParticles();
	}

	public override void UpdateNavMeshInCatching(bool force)
	{
		if (!force && !(Time.time - mLastUpdateNavMeshTime > 3f / mRunSpeed))
		{
			return;
		}
		mLastUpdateNavMeshTime = Time.time;
		if (mTarget == null)
		{
			return;
		}
		Vector3 destination = mTargetPosition;
		if (null != mNavMeshAgent)
		{
			mNavMeshAgent.enabled = true;
			mNavMeshAgent.Resume();
			mNavMeshAgent.SetDestination(destination);
			mNavMeshAgent.speed = mRunSpeed;
			if (force)
			{
				mNavMeshAgent.angularSpeed = 720f;
			}
			else
			{
				mNavMeshAgent.angularSpeed = 120f;
			}
			SetCanTurn(false);
		}
	}

	public virtual void DoCatching()
	{
		PlayAnimation(RunAnimationName, WrapMode.Loop);
		UpdateNavMeshInCatching(false);
		if (!base.IsMasterPlayer)
		{
			return;
		}
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			if (GetUpdatePosTimeDuration() > mUpdatePosInterval)
			{
				SetUpdatePosTimeNow();
				EnemyStateRequest request = new EnemyStateRequest(EnemyID, GetID(EEnemyStateID.CATCHING), GetTransform().position);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			else if (CheckTargetInCatching())
			{
				MakeDecisionInCatching();
			}
		}
		else if (CheckTargetInCatching())
		{
			MakeDecisionInCatching();
		}
	}

	public virtual bool CheckTargetInCatching()
	{
		if (mTarget == null)
		{
			ChooseTargetPlayer(true);
		}
		if (mTarget == null)
		{
			SetState(IDLE_STATE);
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && GameApp.GetInstance().GetGameMode().IsCoopMode())
			{
				EnemyStateRequest request = new EnemyStateRequest(EnemyID, GetID(EEnemyStateID.IDLE), entityTransform.position);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			return false;
		}
		return true;
	}

	protected virtual void MakeDecisionInCatching()
	{
		if (!(SqrDistanceFromPlayer < AttackRange * AttackRange) || !canHitTarget())
		{
			return;
		}
		if (GetAttackTimeDuration() > attackFrequency)
		{
			SetState(ATTACK_STATE);
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && GameApp.GetInstance().GetGameMode().IsCoopMode())
			{
				EnemyStateRequest request = new EnemyStateRequest(EnemyID, GetID(EEnemyStateID.ATTACK), entityTransform.position);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
		else
		{
			SetState(IDLE_STATE);
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && GameApp.GetInstance().GetGameMode().IsCoopMode())
			{
				EnemyStateRequest request2 = new EnemyStateRequest(EnemyID, GetID(EEnemyStateID.IDLE), entityTransform.position);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
			}
		}
	}

	public virtual void EndCatching()
	{
		StopNavMesh();
		HideFootParticles();
	}

	public void MoveToward(Vector3 position, float speed = -1f)
	{
		mLastUpdateNavMeshTime = Time.time;
		if (null != mNavMeshAgent)
		{
			mNavMeshAgent.enabled = true;
			mNavMeshAgent.Resume();
			mNavMeshAgent.SetDestination(position);
			if (speed < 0f)
			{
				mNavMeshAgent.speed = mRunSpeed;
			}
			else
			{
				mNavMeshAgent.speed = speed;
			}
			SetCanTurn(false);
		}
	}

	public void PlayRunAnimation()
	{
		PlayAnimation(RunAnimationName, WrapMode.Loop);
	}

	public void ChangeNavigationSpeed(float speed)
	{
		mNavMeshAgent.speed = speed;
	}

	protected virtual void PlayDeadSound()
	{
		int num = UnityEngine.Random.Range(0, 100);
		if (num < 50)
		{
			PlaySound("RPG_Audio/Enemy/enemy_dead1");
		}
		else
		{
			PlaySound("RPG_Audio/Enemy/enemy_dead2");
		}
	}

	public virtual void StartDead()
	{
		if (base.IsMasterPlayer && !IsBoss())
		{
			OnLoot();
		}
		if (IsBoss())
		{
			SwVoidDataManager.GetInstance().DropBossMaterial(GetPosition());
		}
		else
		{
			SwVoidDataManager.GetInstance().CheckDropMaterial(GetPosition());
		}
		entityObject.layer = PhysicsLayer.DEFAULT;
		deadRemoveBodyTimer = new Timer();
		if (EnemyType == EnemyType.BANELING)
		{
			criticalAttacked = true;
		}
		else if (EnemyType == EnemyType.MYTH370 || EnemyType == EnemyType.TANK)
		{
			criticalAttacked = false;
		}
		if (criticalAttacked)
		{
			deadRemoveBodyTimer.SetTimer(3f, true);
		}
		else
		{
			deadRemoveBodyTimer.SetTimer(3f, false);
		}
		GameObject original = null;
		GameObject original2 = null;
		Vector3 ground = GetGround();
		int num = UnityEngine.Random.Range(0, 100);
		if (bloodColor == BloodColor.Red)
		{
			original = ((num >= 50) ? (Resources.Load(EffectName.DEAD_BLOOD_RED2) as GameObject) : (Resources.Load(EffectName.DEAD_BLOOD_RED) as GameObject));
			int num2 = num % 3 + 1;
			original2 = Resources.Load("_Old/_Effect/Blood_Ground" + num2) as GameObject;
		}
		else if (bloodColor == BloodColor.Green)
		{
			original = Resources.Load(EffectName.DEAD_BLOOD_GREEN) as GameObject;
			original2 = Resources.Load("_Old/_Effect/Blood_Ground4") as GameObject;
		}
		UnityEngine.Object.Instantiate(original, entityTransform.position + Vector3.up * 1.2f, Quaternion.identity);
		GameObject gameObject = UnityEngine.Object.Instantiate(original2, new Vector3(entityTransform.position.x, mFloorHeight + 0.1f, entityTransform.position.z), Quaternion.Euler(270f, 0f, 0f)) as GameObject;
		if (!criticalAttacked)
		{
			gameObject.transform.localScale *= 0.3f;
			ScaleScript scaleScript = gameObject.AddComponent<ScaleScript>();
			scaleScript.enabled = true;
			scaleScript.scaleSpeed = 0.1f;
			scaleScript.enableMaxScale = true;
			scaleScript.maxScale = 0.3f;
		}
		Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, ground);
		gameObject.transform.rotation = quaternion * gameObject.transform.rotation;
		if (criticalAttacked)
		{
			num = UnityEngine.Random.Range(0, 360);
			GameObject gameObject2 = mGameWorld.GetDeadBodyPool(mEnemyType).CreateObject(new Vector3(entityTransform.position.x, mFloorHeight + 0.1f, entityTransform.position.z), Vector3.zero, Quaternion.Euler(0f, num, 0f));
			gameObject2.transform.rotation = quaternion * gameObject2.transform.rotation;
			num = UnityEngine.Random.Range(0, 100);
			if (num < 30)
			{
				gameObject2.GetComponent<Animation>().Play("ani");
			}
			else if (num < 65)
			{
				gameObject2.GetComponent<Animation>().Play("ani02");
			}
			else
			{
				gameObject2.GetComponent<Animation>().Play("ani03");
			}
		}
		num = UnityEngine.Random.Range(0, 100);
		if (num < 50)
		{
			AudioManager.GetInstance().PlaySound(AudioName.ENEMY_SMASH01, entityObject);
		}
		else
		{
			AudioManager.GetInstance().PlaySound(AudioName.ENEMY_SMASH02, entityObject);
		}
		if (GameApp.GetInstance().GetGameMode().IsSingle())
		{
			AddCashAndExpToPlayer();
			mLocalPlayer.RecoveryWhenMakeKills();
		}
		mLocalPlayer.AddKillsByAllPlayers(1);
		DestroyNavMeshAgent();
	}

	public virtual void DoDead()
	{
		if (criticalAttacked)
		{
			Destroy();
		}
		else if (deadRemoveBodyTimer != null && deadRemoveBodyTimer.Ready())
		{
			SetState(FADE_OUT_STATE);
		}
	}

	public virtual void EndDead()
	{
	}

	public virtual void StartEnemyAttack()
	{
		SetAttackTimeNow();
		LookAtTargetHorizontal();
		attacked = false;
		ResetCheckHitList(99f);
	}

	public virtual void DoEnemyAttack()
	{
		PlayAnimation(AnimationString.ENEMY_ATTACK, WrapMode.ClampForever);
		CheckHit();
		if (AnimationPlayed(AnimationString.ENEMY_ATTACK, 1f))
		{
			SetState(IDLE_STATE);
		}
	}

	public virtual void EndEnemyAttack()
	{
	}

	public virtual void StartEnemyIdle()
	{
		SetIdleTimeNow();
		if (base.IsMasterPlayer)
		{
			ChooseTargetPlayer(false);
		}
	}

	public virtual void StartEnemyIdleFromDoor()
	{
		entityObject.layer = PhysicsLayer.ENEMY;
		mAccelerateTimer.Do();
		GetGround();
		entityTransform.position = new Vector3(entityTransform.position.x, mFloorHeight + 0.2f, entityTransform.position.z);
		mGameWorld.CreateEnemyEffect(EEnemyEffectType.GRAVE_ROCK2, entityTransform.position, Quaternion.identity);
		CreateNavMeshAgent();
		if (base.IsMasterPlayer && mHasMission)
		{
			AssignMission();
		}
	}

	protected virtual void StartEnemyIdleFromGrave()
	{
		entityObject.layer = PhysicsLayer.ENEMY;
		mAccelerateTimer.Do();
		CreateNavMeshAgent();
		if (base.IsMasterPlayer && mHasMission)
		{
			AssignMission();
		}
	}

	public virtual void DoEnemyIdle()
	{
		PlayAnimation(AnimationString.ENEMY_IDLE, WrapMode.Loop);
		LookAtTargetHorizontal();
		if (base.IsMasterPlayer && CheckTargetInEnemyIdle())
		{
			MakeDecisionInEnemyIdle();
		}
	}

	public virtual bool CheckTargetInEnemyIdle()
	{
		if (mTarget == null || !mTarget.InPlayingState())
		{
			ChooseTargetPlayer(true);
		}
		if (mTarget == null)
		{
			return false;
		}
		return true;
	}

	protected virtual void MakeDecisionInEnemyIdle()
	{
		if (!(GetIdleTimeDuration() > 0.1f))
		{
			return;
		}
		if (SqrDistanceFromPlayer < AttackRange * AttackRange)
		{
			if (GetAttackTimeDuration() > attackFrequency)
			{
				if (canHitTarget())
				{
					SetState(ATTACK_STATE);
					if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && GameApp.GetInstance().GetGameMode().IsCoopMode())
					{
						EnemyStateRequest request = new EnemyStateRequest(EnemyID, GetID(EEnemyStateID.ATTACK), entityTransform.position);
						GameApp.GetInstance().GetNetworkManager().SendRequest(request);
					}
				}
				else
				{
					SetState(CATCHING_STATE);
					if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && GameApp.GetInstance().GetGameMode().IsCoopMode())
					{
						EnemyStateRequest request2 = new EnemyStateRequest(EnemyID, GetID(EEnemyStateID.CATCHING), entityTransform.position);
						GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
					}
				}
			}
			else
			{
				SetIdleTimeNow();
			}
		}
		else
		{
			SetState(CATCHING_STATE);
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && GameApp.GetInstance().GetGameMode().IsCoopMode())
			{
				EnemyStateRequest request3 = new EnemyStateRequest(EnemyID, GetID(EEnemyStateID.CATCHING), entityTransform.position);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request3);
			}
		}
	}

	public virtual void EndEnemyIdle()
	{
	}

	protected virtual void PlayGotHitSound()
	{
	}

	public virtual void StartGotHit()
	{
		StopNavMesh();
		PlayGotHitSound();
		mLastGotHitTime = Time.time;
	}

	public virtual void DoGotHit()
	{
		PlayAnimation(AnimationString.ENEMY_GOTHIT, WrapMode.ClampForever);
		if (AnimationPlayed(AnimationString.ENEMY_GOTHIT, 1f))
		{
			SetState(IDLE_STATE);
		}
	}

	public virtual void EndGotHit()
	{
	}

	public virtual void StartGraveBorn()
	{
		entityTransform.Translate(Vector3.down * 2f);
		entityObject.layer = PhysicsLayer.DEFAULT;
		GetGround();
	}

	public virtual void DoGraveBorn()
	{
		PlayAnimation(AnimationString.ENEMY_RUN, WrapMode.Loop);
		entityTransform.Translate(Vector3.up * Time.deltaTime * 2f);
		if (entityTransform.position.y >= mFloorHeight + 0.2f)
		{
			SetState(IDLE_STATE);
			StartEnemyIdleFromGrave();
		}
	}

	public virtual void EndGraveBorn()
	{
		if (entityTransform.position.y < mFloorHeight + 0.2f)
		{
			entityTransform.position = new Vector3(entityTransform.position.x, mFloorHeight + 0.2f, entityTransform.position.z);
		}
		entityObject.layer = PhysicsLayer.ENEMY;
	}

	public virtual void StartFadeOut()
	{
		if (mMaterial != null && mMaterial.HasProperty("_BumpMap"))
		{
			mMaterial.shader = Shader.Find("StarWarfare/Enemy/Alpha_Bumped_Specular");
		}
		else
		{
			Destroy();
		}
	}

	public virtual void DoFadeOut()
	{
		if (alpha < 0f)
		{
			Destroy();
			mMaterial.SetFloat("_AlphaAfterDead", 1f);
		}
		else
		{
			alpha -= Time.deltaTime / 2f;
			mMaterial.SetFloat("_AlphaAfterDead", alpha);
		}
	}

	public virtual void EndFadeOut()
	{
	}

	public virtual string GetMeshPath()
	{
		return BoneName.EnemyMesh;
	}

	public void SendStateChangeRequest()
	{
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			EnemyStateRequest request = new EnemyStateRequest(EnemyID, mConditionState.ID, entityTransform.position);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
	}

	public void SendStateChangeRequest(ConditionState state)
	{
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			EnemyStateRequest request = new EnemyStateRequest(EnemyID, state.ID, entityTransform.position);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
	}

	public override void ResetAttack(CreatureSkill skill)
	{
		SetAttackTimeNow();
		LookAtTargetHorizontal();
		attacked = false;
		ResetCheckHitList(skill.Interval);
	}

	private void ResetCheckHitList(float interval)
	{
		mCheckHitList.Clear();
		mCheckHitList.Add(mLocalPlayer, new CountTimer(interval));
		foreach (Robot robot in mGameWorld.GetRobotList())
		{
			if (robot.InPlayingState())
			{
				mCheckHitList.Add(robot, new CountTimer(interval));
			}
		}
		foreach (ProtectedItem protectedItem in mGameWorld.GetProtectedItemList())
		{
			if (protectedItem.InPlayingState())
			{
				mCheckHitList.Add(protectedItem, new CountTimer(interval));
			}
		}
	}

	public void UpdateNavMeshInDash(bool force, float speed)
	{
		if (!force && !(Time.time - mLastUpdateNavMeshTime > 3f / speed))
		{
			return;
		}
		mLastUpdateNavMeshTime = Time.time;
		if (mTarget != null)
		{
			Vector3 destination = mTargetPosition;
			if (null != mNavMeshAgent)
			{
				mNavMeshAgent.enabled = true;
				mNavMeshAgent.Resume();
				mNavMeshAgent.SetDestination(destination);
				mNavMeshAgent.speed = speed;
				mNavMeshAgent.angularSpeed = 720f;
				SetCanTurn(false);
			}
		}
	}

	public float GetUpdatePosInterval()
	{
		return mUpdatePosInterval;
	}

	public bool IsReachLookAtAngle()
	{
		float num = Vector3.Dot((mTargetToLookAt - entityTransform.position).normalized, entityTransform.forward);
		return num >= 0.95f;
	}

	public byte GetID(EEnemyStateID enemyStateId)
	{
		if (IsNewState)
		{
			EnemyState enemyState = mGameWorld.GetEnemyState(enemyStateId);
			return mStateSwitchDic[enemyState].ID;
		}
		return (byte)enemyStateId;
	}

	public static bool IsBoss(EnemyType enemyType)
	{
		if (enemyType == EnemyType.SLASHER || enemyType == EnemyType.DEAD_EYE || enemyType == EnemyType.FROST_STONE || enemyType == EnemyType.CERBERLISK || enemyType == EnemyType.SPECIAL_SLASHER || enemyType == EnemyType.CRIMSON_QUEEN)
		{
			return true;
		}
		return false;
	}

	public void SetRemoteEnemyMove(bool enable)
	{
		mRemoteEnemyMove = enable;
		if (enable)
		{
			mCurrentDeltaCount = 99;
		}
	}

	protected void SetShoutTimer(string audio, float interval)
	{
		shoutAudioName = audio;
		if (!sShoutAudioTimer.ContainsKey(EnemyType))
		{
			sShoutAudioTimer[EnemyType] = new Timer(interval);
		}
		mShoutAudioTimer = sShoutAudioTimer[EnemyType];
	}

	public virtual DamageProperty CreateDamageProperty(int damage)
	{
		return new DamageProperty(this, damage, mPlayerDamageType);
	}

	public byte GetConditionStateId(EnemyState state)
	{
		if (mConditionState.mState == state)
		{
			return mConditionState.ID;
		}
		Debug.LogError("\ufffd\ufffd\ufffdǵ\ufffdǰstate");
		return 0;
	}

	public void EnemyGotHitAnimation()
	{
		PlayGotHitSound();
		PlayAnimation(AnimationString.ENEMY_GOTHIT, WrapMode.Once);
	}

	public void SetInvincible(bool invincible)
	{
		mIsInvincible = invincible;
	}

	public void OnLoot()
	{
		float value = UnityEngine.Random.value;
		if (value < pickupRate)
		{
			GetGround();
			float floorHeight = GetFloorHeight();
			int loot = GetLoot();
			float a = Mathf.Pow(1.07f, DifficultLevel);
			a = Mathf.Min(a, 5f) * lootCashScale;
			int num = (int)((float)loot / ((float)Global.LOOT_CASH_STACK * a) + 1f);
			for (int i = 0; i < num; i++)
			{
				float f = ((!isPresentLooted) ? (num - 1) : num);
				float num2 = Mathf.Pow(f, 0.5f);
				float num3 = UnityEngine.Random.Range(0f - num2, num2);
				float num4 = UnityEngine.Random.Range(0f - num2, num2);
				Vector3 position = new Vector3(entityTransform.position.x + num3, floorHeight + 1f, entityTransform.position.z + num4);
				GameWorld.TryCreatePickUp(LootType.MONEY, position, (short)(loot / num), 0f);
			}
		}
	}

	public override bool UnitCanBeHitByRay(Vector3 position, float distance)
	{
		Vector3 vector = GetPosition() + new Vector3(0f, 1f, 0f);
		Ray ray = new Ray(position, vector - position);
		int layerMask = (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.ENEMY);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, distance, layerMask) && hitInfo.collider.gameObject.layer == PhysicsLayer.ENEMY)
		{
			return true;
		}
		return false;
	}

	public virtual bool inShield()
	{
		return false;
	}
}
