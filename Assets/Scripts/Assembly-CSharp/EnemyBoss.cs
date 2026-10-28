using UnityEngine;

public abstract class EnemyBoss : Enemy
{
	public class BossTouch
	{
		public Enemy mBoss;

		private Player mLocalPlayer;

		public int mTouchDamage;

		public float mTouchRadiusSqr;

		public float mTouchKnockSpeed;

		public Timer mTouchTimer = new Timer();

		private PlayerDamageType mPlayerDamageType;

		public void Init(Enemy boss, Player player, BossParameter param, int damageIndex, float radius, float knockSpeed)
		{
			mTouchTimer.SetTimer(2f, false);
			mBoss = boss;
			mLocalPlayer = player;
			mTouchDamage = param.GetDamage(damageIndex);
			mTouchRadiusSqr = radius * radius;
			mTouchKnockSpeed = knockSpeed;
			mPlayerDamageType = param.GetDamageType(damageIndex);
		}

		public void Update()
		{
			if (!mBoss.IsDead && mTouchTimer.Ready() && (mLocalPlayer.GetTransform().position - mBoss.GetPosition()).sqrMagnitude < mTouchRadiusSqr)
			{
				mTouchTimer.Do();
				DamageProperty dp = new DamageProperty(mBoss, mTouchDamage, mPlayerDamageType);
				mLocalPlayer.OnHit(dp);
				mBoss.CheckKnocked(mTouchKnockSpeed);
			}
		}
	}

	public class BossDefend
	{
		private Enemy mBoss;

		private float mLastDefendTime;

		public GameObject defendObj;

		public GameObject bornDefentObj;

		public Timer showDefendTimer = new Timer(0.2f);

		public void Init(Enemy boss)
		{
			mBoss = boss;
			GameObject original = Resources.Load(EffectName.Boss_Shield) as GameObject;
			defendObj = Object.Instantiate(original, mBoss.bipObject.transform.position + Vector3.down * 2f, Quaternion.identity) as GameObject;
			defendObj.transform.parent = mBoss.bipObject.transform;
			defendObj.SetActive(false);
			GameObject original2 = Resources.Load(EffectName.Boss_Born_Shield) as GameObject;
			bornDefentObj = Object.Instantiate(original2) as GameObject;
			bornDefentObj.SetActive(false);
			showDefendTimer.SetTimer(0.2f, false);
		}

		public void ShowDefend()
		{
			if (showDefendTimer.Ready())
			{
				defendObj.SetActive(false);
				defendObj.SetActive(true);
				showDefendTimer.Do();
			}
		}

		public void SetDefendTimeNow()
		{
			mLastDefendTime = Time.time;
		}

		public float GetDefendDuration()
		{
			return Time.time - mLastDefendTime;
		}
	}

	public enum MindState
	{
		NORMAL = 0,
		RAGE1 = 1,
		RAGE2 = 2,
		Count = 3
	}

	public enum ECoopBossMode
	{
		None = 0,
		LongIdle = 1,
		RangeIdle = 2,
		Normal = 3
	}

	public static EnemyState BOSS_INIT_STATE = new BossInitState();

	public static EnemyState BOSS_RAGE_STATE = new BossRageState();

	public static EnemyState BOSS_STEP_BACK_STATE = new BossStepBackState();

	protected Collider mBodyCollider;

	protected Collider mFlyBodyCollider;

	protected GameObject[] mTrails;

	protected GameObject mBipObject;

	protected string mWalkAudioName = string.Empty;

	protected MindState mMindState;

	protected float[] mMaxCatchingTimeArray = new float[2];

	protected int mCurrentHitTime;

	protected float mGroupAttackDistance;

	protected int mAttackCount;

	protected float mHpPercetageToRage = 0.4f;

	protected float mHpPercentagePerHit = 0.15f;

	protected bool mIsDead;

	public int MeleeAttackHasUsedNum;

	protected int mMeleeAttackHasHitPlayer;

	public sStepBack StepBack;

	public sDashAttack DashAttackParameter;

	protected ENextAction mNextAction;

	protected BossTouch mBossTouch = new BossTouch();

	protected bool mIsInDefent;

	public BossDefend mBossDefend = new BossDefend();

	public string RageSoundName;

	public bool IsInBossCoop;

	private ECoopBossMode mCoopBossMode;

	public int Index
	{
		get
		{
			switch (mEnemyType)
			{
			case EnemyType.SLASHER:
				return 0;
			case EnemyType.DEAD_EYE:
				return 1;
			case EnemyType.FROST_STONE:
				return 2;
			default:
				Debug.LogError("no index.");
				return -1;
			}
		}
	}

	public float MaxCatchingTime
	{
		get
		{
			return mMaxCatchingTimeArray[(int)mMindState];
		}
	}

	public float GroupAttackDistance
	{
		get
		{
			return mGroupAttackDistance;
		}
	}

	public override bool IsDead
	{
		get
		{
			return mIsDead;
		}
	}

	public void IncreaseAttackCount()
	{
		mAttackCount++;
	}

	public int GetAttackCount()
	{
		return mAttackCount;
	}

	public void ResetAttackCount()
	{
		mAttackCount = 0;
	}

	public override bool IsBoss()
	{
		return true;
	}

	public override void Init(GameObject gObject)
	{
		base.Init(gObject);
		entityObject.layer = PhysicsLayer.BOSS;
		loadParameters();
		mMindState = MindState.NORMAL;
		InitBossLevelTime();
		mDeltaPosition = Vector3.zero;
		mCurrentHitTime = 0;
		mDeadTimer.SetTimer(3f, false);
		mIsDead = false;
		if (!IsNewState)
		{
			SetState(Enemy.IDLE_STATE);
		}
		if (entityObject.transform.Find("SecondAnimation") != null)
		{
			mSecondAnim = new SecondAnimation();
			GameObject gameObject = entityObject.transform.Find("SecondAnimation").gameObject;
			mSecondAnim.Init(this, gameObject);
		}
		if (GameApp.GetInstance().GetGameMode().IsSingle() && GlobalData.GetInstance().Partner != null)
		{
			hp = (maxHp *= 2);
		}
	}

	public virtual void InitBossLevelTime()
	{
		mLastCatchingTime = Time.time;
	}

	public bool NeedRage()
	{
		if (mMindState == MindState.RAGE1)
		{
			return false;
		}
		if ((float)Hp < (float)MaxHp * mHpPercetageToRage)
		{
			mMindState = MindState.RAGE1;
			return true;
		}
		return false;
	}

	public MindState GetMindState()
	{
		return mMindState;
	}

	public int GetMindStateInt()
	{
		return (int)mMindState;
	}

	public void SetMindState(MindState state)
	{
		mMindState = state;
	}

	protected new virtual void loadParameters()
	{
	}

	protected override void ChooseTargetPlayer(bool force)
	{
		GameUnit randomTarget = GetRandomTarget();
		if (randomTarget != null)
		{
			ChangeTarget(randomTarget);
			if (GameApp.GetInstance().GetGameMode().IsCoopMode())
			{
				EnemyChangeTargetRequest request = new EnemyChangeTargetRequest(base.EnemyID, (randomTarget as Player).GetChannelID(), entityTransform.position);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
	}

	public override void PlaySound(string name)
	{
		AudioManager.GetInstance().PlaySoundAt(name, entityObject, entityTransform.position, AudioRolloffMode.Linear, 200f);
	}

	public override void PlaySoundSingle(string name)
	{
		AudioManager.GetInstance().PlaySoundSingleAt(name, entityObject, entityTransform.position, AudioRolloffMode.Linear, 200f);
	}

	public override void DoShoutAudio()
	{
	}

	public virtual void EnableTrailEffect(bool bEnable)
	{
		if (bEnable)
		{
			GameObject[] array = mTrails;
			foreach (GameObject gameObject in array)
			{
				gameObject.transform.GetChild(0).GetComponent<ParticleEmitter>().emit = true;
			}
		}
		else
		{
			GameObject[] array2 = mTrails;
			foreach (GameObject gameObject2 in array2)
			{
				gameObject2.transform.GetChild(0).GetComponent<ParticleEmitter>().emit = false;
			}
		}
	}

	protected virtual void StopSoundOnHit()
	{
	}

	public override void OnHit(DamageProperty dp)
	{
		if (!InPlayingState())
		{
			return;
		}
		if (NeedShowDefent())
		{
			mBossDefend.ShowDefend();
			if (mBossDefend.GetDefendDuration() > EnemyBossParameter.MAX_DEFEND_TIME)
			{
				mIsInDefent = false;
			}
		}
		if (dp.wType != WeaponType.NoGun && dp.isLocal)
		{
			mLocalPlayer.HitEnemy();
		}
		mLastDamageProperty = dp;
		hp -= dp.damage;
		Vector3 hitPoint = Vector3.zero;
		if (GetHitPoint(out hitPoint))
		{
			GameObject original = Resources.Load("SW2_Effect/dragon_blood") as GameObject;
			Object.Instantiate(original, hitPoint, Quaternion.identity);
		}
		if ((float)(maxHp - hp) > (float)((mCurrentHitTime + 1) * maxHp) * mHpPercentagePerHit && hp > 0)
		{
			mCurrentHitTime++;
			ChangeToHitState();
			GameObject original2 = Resources.Load("_Old/_Effect/Dragon/dragon_dead_blood") as GameObject;
			Object.Instantiate(original2, bipObject.transform.position, Quaternion.identity);
		}
	}

	public override void OnHitResponse(int newHp, int killerID, bool criticalAttack, WeaponType weaponType)
	{
		if (!InPlayingState())
		{
			return;
		}
		Hp = newHp;
		if (NeedShowDefent())
		{
			mBossDefend.ShowDefend();
			if (mBossDefend.GetDefendDuration() > EnemyBossParameter.MAX_DEFEND_TIME)
			{
				mIsInDefent = false;
			}
		}
		if (Hp > 0)
		{
			Vector3 hitPoint = Vector3.zero;
			if (killerID == Lobby.GetInstance().GetChannelID())
			{
				if (GetHitPoint(out hitPoint))
				{
					GameObject original = Resources.Load("SW2_Effect/dragon_blood") as GameObject;
					Object.Instantiate(original, hitPoint, Quaternion.identity);
				}
			}
			else
			{
				GameObject original2 = Resources.Load("SW2_Effect/dragon_blood") as GameObject;
				Object.Instantiate(original2, bipObject.transform.position, Quaternion.identity);
			}
			if ((float)(maxHp - hp) > (float)((mCurrentHitTime + 1) * maxHp) * mHpPercentagePerHit)
			{
				mCurrentHitTime++;
				ChangeToHitState();
				GameObject original3 = Resources.Load("_Old/_Effect/Dragon/dragon_dead_blood") as GameObject;
				Object.Instantiate(original3, bipObject.transform.position, Quaternion.identity);
			}
		}
		else if (Hp == 0)
		{
			if (killerID == Lobby.GetInstance().GetChannelID())
			{
				Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
				localPlayer.RecoveryWhenMakeKills();
			}
			AddCashAndExpToPlayer();
			GameApp.GetInstance().GetGameWorld().TotalEnemyKilled++;
			ChangeToDeadState();
		}
	}

	public override void DoLogic(float deltaTime)
	{
		if (mSecondAnim != null)
		{
			mSecondAnim.Update();
		}
		base.DoLogic(deltaTime);
		mBossTouch.Update();
	}

	public override bool CheckTargetInCatching()
	{
		if (mTarget == null)
		{
			SetState(Enemy.IDLE_STATE);
			return false;
		}
		return true;
	}

	public override void DoCatching()
	{
		PlayAnimation(base.RunAnimationName, WrapMode.Loop);
		UpdateNavMeshInCatching(false);
		if (base.IsMasterPlayer)
		{
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && GameApp.GetInstance().GetGameMode().IsCoopMode())
			{
				if (GetUpdatePosTimeDuration() > mUpdatePosInterval)
				{
					SetUpdatePosTimeNow();
					EnemyStateRequest request = new EnemyStateRequest(base.EnemyID, GetID(EEnemyStateID.CATCHING), GetTransform().position, mTargetUserId);
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
		else
		{
			CheckTargetInCatching();
		}
	}

	public override void DoDead()
	{
		if (!mIsDead && mDeadTimer.Ready())
		{
			mIsDead = true;
		}
	}

	public override void StartEnemyIdle()
	{
		SetIdleTimeNow();
	}

	public override void DoEnemyIdle()
	{
		PlayAnimation(AnimationString.ENEMY_IDLE, WrapMode.Loop);
		if (base.IsMasterPlayer && !IsNewState)
		{
			MakeDecisionInEnemyIdle();
		}
	}

	public override void DoGotHit()
	{
		PlayAnimation(AnimationString.ENEMY_GOTHIT, WrapMode.ClampForever);
		if (AnimationPlayed(AnimationString.ENEMY_GOTHIT, 1f))
		{
			if (NeedRage())
			{
				SetState(BOSS_RAGE_STATE);
			}
			else
			{
				SetState(Enemy.IDLE_STATE);
			}
		}
	}

	public virtual void StartBossInit()
	{
	}

	public virtual void DoBossInit()
	{
	}

	public virtual void EndBossInit()
	{
	}

	public virtual void StartBossRage()
	{
	}

	public virtual void EndBossRage()
	{
	}

	public override bool HandleMessage(EConditionMessage message)
	{
		if (base.HandleMessage(message))
		{
			return true;
		}
		switch (message)
		{
		case EConditionMessage.EMakeDecisionInIdle:
			MakeDecisionInEnemyIdle();
			return true;
		case EConditionMessage.EChangeTargetPositionInCoop:
			mTargetPosition = mGameWorld.GetBossCoop().GetAttackPosition(this);
			break;
		case EConditionMessage.ELookAtTargetPostion:
			LookAtTargetHorizontal();
			break;
		case EConditionMessage.ENeedUpdateTargetPosition:
			NeedUpdateTargetPosition = true;
			break;
		case EConditionMessage.EMakeDecisionInSkillIdle:
			if (mNextAction == ENextAction.CoopDead)
			{
				SetInDefend();
				SendStateChangeRequest(mConditionState);
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
				{
					BossCoopRequest bossCoopRequest = new BossCoopRequest(6, 4);
					bossCoopRequest.Data.AddInt(base.EnemyID);
					GameApp.GetInstance().GetNetworkManager().SendRequest(bossCoopRequest);
				}
				return true;
			}
			break;
		}
		return false;
	}

	public override void ChangeToHitState()
	{
		if (IsStateMatch("CoopDash") || IsStateMatch("CoopDash2") || IsStateMatch("CoopSkillIdle"))
		{
			mGameWorld.GetBossCoop().OnGotHitInDash(this);
		}
		base.ChangeToHitState();
	}

	public override bool NeedShowDefent()
	{
		return mIsInDefent;
	}

	public void SetInDefend()
	{
		mNextAction = ENextAction.None;
		SetState(BOSS_RAGE_STATE);
		mIsInDefent = true;
		mBossDefend.SetDefendTimeNow();
	}

	public void SetInDefendRepetition()
	{
		mNextAction = ENextAction.None;
		mIsInDefent = true;
		mBossDefend.SetDefendTimeNow();
	}

	public void SetNextAction(ENextAction action)
	{
		mNextAction = action;
	}

	public override float GetHitColliderRadius()
	{
		return cc.radius;
	}

	public virtual void CoopBorn()
	{
		SetState("CoopBorn");
		mCoopBossMode = ECoopBossMode.LongIdle;
		SetInvincible(true);
		mBossDefend.bornDefentObj.SetActive(true);
	}

	public virtual void StartCoopRange()
	{
		SetState("CoopRangeIdle");
	}

	public virtual void StartCoopRage()
	{
		SetState(BOSS_RAGE_STATE);
	}

	public virtual void JoinBattle()
	{
	}

	public virtual void CoopMoveToPosition(Vector3 position)
	{
		mTargetPosition = position;
		NeedUpdateTargetPosition = false;
		SetState("CoopDash");
	}

	public virtual void CoopCastSkill()
	{
	}

	public static int GetBossIndex(EnemyType type)
	{
		switch (type)
		{
		case EnemyType.SLASHER:
		case EnemyType.DEAD_EYE:
		case EnemyType.FROST_STONE:
		case EnemyType.CERBERLISK:
			return (int)(type - 9);
		default:
			return -1;
		}
	}

	public override void StartDead()
	{
		SwVoidDataManager.GetInstance().DropBossMaterial(GetPosition());
	}

	public override bool UnitCanBeHitByRay(Vector3 position, float distance)
	{
		Vector3 vector = GetPosition() + GetTransform().up * 1.8f;
		Ray ray = new Ray(position, vector - position);
		int layerMask = (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.BOSS);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, distance, layerMask) && hitInfo.collider.gameObject.layer == PhysicsLayer.BOSS)
		{
			return true;
		}
		return false;
	}

	protected override void CreateShadow(float size)
	{
		base.CreateShadow(size);
		if (!GameApp.GetInstance().GetUserState().IsLowQuality())
		{
			Shadow.SetActive(false);
		}
	}

	public void SetShadow(bool enable)
	{
		if (GameApp.GetInstance().GetUserState().IsLowQuality())
		{
			Shadow.SetActive(enable);
		}
	}

	public override DamageProperty CreateDamageProperty(int damage)
	{
		return new DamageProperty(this, mBossParameter.GetDamage(damage), mBossParameter.GetDamageType(damage));
	}

	public override Vector3 GetColliderCenterPosition()
	{
		return GetPosition() + GetTransform().up * 2f;
	}
}
