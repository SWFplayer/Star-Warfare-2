using UnityEngine;

public class Slasher : EnemyBoss
{
	public class Skill : BossSkill
	{
		public CreatureMeleeAttackSkill WeaponMeleeAttackSkill1;

		public CreatureMeleeAttackSkill WeaponMeleeAttackSkill2;

		public CreatureMeleeAttackSkill WeaponDashSkill;

		public GameObject Stick;

		public Animation mEyeAnimation;

		public void Init(GameObject go, bool isEnemy)
		{
			if (!isEnemy)
			{
				GameObject original = Resources.Load("Level/SlasherStick") as GameObject;
				Stick = Object.Instantiate(original) as GameObject;
				SlasherStickScript component = Stick.GetComponent<SlasherStickScript>();
				Transform parent = go.transform.Find(BoneName.SLASHER_WEAPON);
				Stick.gameObject.transform.parent = parent;
				Stick.gameObject.transform.localPosition = Vector3.zero;
				Stick.gameObject.transform.localRotation = Quaternion.identity;
				Stick.gameObject.tag = "untagged";
				Stick.gameObject.layer = PhysicsLayer.DEFAULT;
				mEyeAnimation = go.transform.Find(BoneName.SLASHER_EYE).gameObject.GetComponent<Animation>();
				Collider collider = Stick.GetComponent<Collider>();
			}
			WeaponMeleeAttackSkill1 = new CreatureMeleeAttackSkill();
			WeaponMeleeAttackSkill1.CheckTime = new Vector2(0.46f, 0.7f);
			WeaponMeleeAttackSkill1.SetDamageInfo(SlasherParameter.GetInstance(), 3);
			WeaponMeleeAttackSkill1.Radius = SlasherParameter.WEAPON_ATTACK01_RADIUS;
			WeaponMeleeAttackSkill1.Angle = SlasherParameter.WEAPON_ATTACK01_ANGLE;
			WeaponMeleeAttackSkill1.KnockDownSpeed = SlasherParameter.MELEE_ATTACK01_KNOCK_SPEED;
			WeaponMeleeAttackSkill2 = new CreatureMeleeAttackSkill();
			WeaponMeleeAttackSkill2.CheckTime = new Vector2(0.24f, 0.7f);
			WeaponMeleeAttackSkill2.SetDamageInfo(SlasherParameter.GetInstance(), 4);
			WeaponMeleeAttackSkill2.Radius = SlasherParameter.WEAPON_ATTACK02_RADIUS;
			WeaponMeleeAttackSkill2.Angle = SlasherParameter.WEAPON_ATTACK02_ANGLE;
			WeaponMeleeAttackSkill2.KnockDownSpeed = SlasherParameter.WEAPON_ATTACK02_KNOCK_SPEED;
			WeaponDashSkill = new CreatureMeleeAttackSkill();
			WeaponDashSkill.SetDamageInfo(SlasherParameter.GetInstance(), 6);
			WeaponDashSkill.HitRadius = SlasherParameter.WEAPON_DASH_CHECK_RADIUS;
			WeaponDashSkill.Radius = SlasherParameter.WEAPON_DASH_SECTOR_RADIUS;
			WeaponDashSkill.Angle = SlasherParameter.WEAPON_DASH_SECTOR_ANGLE;
			WeaponDashSkill.KnockDownSpeed = SlasherParameter.WEAPON_DASH_KNOCK_SPEED;
			MoveAudioName = AudioName.SLASHER_RUN;
		}

		public void PlayEyeEffect()
		{
			mEyeAnimation[AnimationString.SLASHER_EYE_EFFECT].wrapMode = WrapMode.ClampForever;
			mEyeAnimation[AnimationString.SLASHER_EYE_EFFECT].time = 0f;
			mEyeAnimation.CrossFade(AnimationString.SLASHER_EYE_EFFECT, 0f, PlayMode.StopAll);
		}
	}

	public static EnemyState SLASHER_ATTACK01_STATE = new SlasherAttack01State();

	public static EnemyState SLASHER_ATTACK02_STATE = new SlasherAttack02State();

	public static EnemyState SLASHER_ATTACK03_STATE = new SlasherAttack03State();

	public static EnemyState SLASHER_DASH01_STATE = new SlasherDash01State();

	public static EnemyState SLASHER_DASH02_STATE = new SlasherDash02State();

	public static EnemyState SLASHER_DASH03_STATE = new SlasherDash03State();

	public static EnemyState SLASHER_RANGE01_STATE = new SlasherRange01State();

	public static EnemyState SLASHER_RANGE02_STATE = new SlasherRange02State();

	public static EnemyState SLASHER_WEAPON_GET_DASH01_STATE = new SlasherWeaponGetDash01State();

	public static EnemyState SLASHER_WEAPON_GET_DASH02_STATE = new SlasherWeaponGetDash02State();

	public static EnemyState SLASHER_WEAPON_GET_DASH03_STATE = new SlasherWeaponGetDash03State();

	public static EnemyState SLASHER_WEAPON_GET_STATE = new SlasherWeaponGetState();

	public static EnemyState SLASHER_WEAPON_IDLE_STATE = new SlasherWeaponIdleState();

	public static EnemyState SLASHER_WEAPON_RUN_STATE = new SlasherWeaponRunState();

	public static EnemyState SLASHER_WEAPON_ATTACK01_STATE = new SlasherWeaponAttack01State();

	public static EnemyState SLASHER_WEAPON_ATTACK02_STATE = new SlasherWeaponAttack02State();

	public static EnemyState SLASHER_WEAPON_DASH01_STATE = new SlasherWeaponDash01State();

	public static EnemyState SLASHER_WEAPON_DASH02_STATE = new SlasherWeaponDash02State();

	public static EnemyState SLASHER_WEAPON_DASH03_STATE = new SlasherWeaponDash03State();

	public static EnemyState SLASHER_WEAPON_RANGE01_STATE = new SlasherWeaponRange01State();

	public static EnemyState SLASHER_WEAPON_ATTACKED_STATE = new SlasherWeaponAttackedState();

	public static EnemyState SLASHER_WEAPON_DROP_STATE = new SlasherWeaponDropState();

	public static EnemyState SLASHER_WEAPON_WAVE_STATE = new SlasherWeaponWaveState();

	public static EnemyState SLASHER_WEAPON_BLADE_STATE = new SlasherWeaponBladeState();

	public static EnemyState SLASHER_STEPBACK_STATE = new SlasherStepbackState();

	public static EnemyState SLASHER_WEAPON_STEPBACK_STATE = new SlasherWeaponStepbackState();

	public static EnemyState SLASHER_DASH_MOVE_STATE1 = new EnemyPlayAnimState
	{
		AnimName = AnimationString.BOSS_DASH01
	};

	public static EnemyState SLASHER_DASH_MOVE_STATE2 = new SlasherDashMoveState();

	private Transform mWeaponTransform;

	private Transform mMouthTransform;

	protected Collider mLeftArmCollider;

	protected Collider mRightArmCollider;

	protected Collider mWeaponCollider;

	protected Animation mEyeAnimation;

	private bool mHasWeapon;

	private Vector3 mDashTarget;

	private SlasherStickScript mTargetStick;

	private float mStartWeaponOnTime;

	private float mStartWeaponOffTime;

	private float mLeapDeceleration;

	private float mLeapMinSpeed;

	private Timer mRainTimer = new Timer();

	private bool mIsTargetLock;

	private bool mIsLeap;

	private bool mIsWaveEffect;

	private bool mIsTraceEffect;

	private int mMeleeAttackCount;

	private int mMaxMeleeAttackCount;

	private string mIdleAnimationName;

	private Skill mSkill = new Skill();

	public int BladeWindNumber { get; set; }

	public Slasher()
	{
		base.GameUnitType = EGameUnitType.ENEMY;
		base.EnemyType = EnemyType.SLASHER;
		IsNewState = true;
	}

	public override void Init(GameObject gObject)
	{
		base.Init(gObject);
		RageSoundName = AudioName.SLASHER_RAGE;
		mMeleeAttackCount = 0;
		mMaxMeleeAttackCount = Random.Range(SlasherParameter.MELEE_ATTACK_MIN_COUNT, SlasherParameter.MELLE_ATTACK_MAX_COUNT + 1);
		mHasWeapon = false;
		bipObject = entityTransform.Find(BoneName.ENEMY_BIP01).gameObject;
		mWeaponTransform = entityTransform.Find(BoneName.SLASHER_WEAPON);
		mMouthTransform = entityTransform.Find(BoneName.SLASHER_MOUTH);
		mLeftArmCollider = entityTransform.Find(BoneName.SLASHER_LEFT_ARM).gameObject.GetComponent<Collider>();
		mRightArmCollider = entityTransform.Find(BoneName.SLASHER_RIGHT_ARM).gameObject.GetComponent<Collider>();
		mEyeAnimation = entityTransform.Find(BoneName.SLASHER_EYE).gameObject.GetComponent<Animation>();
		mStartWeaponOnTime = Time.time;
		mStartWeaponOffTime = Time.time;
		mRainTimer.SetTimer(SlasherParameter.RAIN_INTERVAL, false);
		ConditionStateManager.GetInstance().InitConditionState(this);
		mBossTouch.Init(this, mLocalPlayer, SlasherParameter.GetInstance(), 0, SlasherParameter.TOUCH_RADIUS, SlasherParameter.TOUCH_KNOCK_SPEED);
		mBossDefend.Init(this);
		mSkill.Init(entityObject, true);
		mBossParameter = SlasherParameter.GetInstance();
	}

	protected override void loadParameters()
	{
		mTurnSpeed = SlasherParameter.TURN_SPEED;
		mRunSpeed = SlasherParameter.CATCHING_SPEED;
		mHpPercentagePerHit = SlasherParameter.HP_PERCENTAGE_PER_HIT;
		mHpPercetageToRage = SlasherParameter.HP_PERCENTAGE_TO_RAGE;
	}

	private EEnemyStateID ChooseNearAttack()
	{
		EEnemyStateID eEnemyStateID = EEnemyStateID.NO_STATE;
		float nearestDistanceToTargetPlayer = GetNearestDistanceToTargetPlayer();
		if (nearestDistanceToTargetPlayer < SlasherParameter.GROUP_ATTACK_DISTANCE)
		{
			SetState(SLASHER_ATTACK02_STATE);
			return EEnemyStateID.SLASHER_ATTACK02;
		}
		int num = Random.Range(0, 100);
		if ((float)num < SlasherParameter.PROBABILITY_SINGLE_ATTACK[(int)mMindState])
		{
			SetState(SLASHER_ATTACK01_STATE);
			return EEnemyStateID.SLASHER_ATTACK01;
		}
		SetState(SLASHER_ATTACK02_STATE);
		return EEnemyStateID.SLASHER_ATTACK02;
	}

	private EEnemyStateID ChooseMiddleAttack()
	{
		EEnemyStateID eEnemyStateID = EEnemyStateID.NO_STATE;
		int num = Random.Range(0, 100);
		if ((float)num < SlasherParameter.PROBABILITY_CATCHING[(int)mMindState])
		{
			SetState(Enemy.CATCHING_STATE);
			return EEnemyStateID.CATCHING;
		}
		if ((float)num < SlasherParameter.PROBABILITY_RAIN[(int)mMindState])
		{
			SetState(SLASHER_RANGE01_STATE);
			return EEnemyStateID.SLASHER_RANGE01;
		}
		if ((float)num < SlasherParameter.PROBABILITY_LEAP[(int)mMindState])
		{
			SetState(SLASHER_RANGE02_STATE);
			return EEnemyStateID.SLASHER_RANGE02;
		}
		SetState(SLASHER_ATTACK03_STATE);
		return EEnemyStateID.SLASHER_ATTACK03;
	}

	private EEnemyStateID ChooseFarAttack()
	{
		EEnemyStateID eEnemyStateID = EEnemyStateID.NO_STATE;
		SetState(SLASHER_DASH01_STATE);
		return EEnemyStateID.SLASHER_DASH01;
	}

	private EEnemyStateID ChooseNearWeaponAttack()
	{
		EEnemyStateID eEnemyStateID = EEnemyStateID.NO_STATE;
		float nearestDistanceToTargetPlayer = GetNearestDistanceToTargetPlayer();
		if (nearestDistanceToTargetPlayer < SlasherParameter.WEAPON_GROUP_ATTACK_DISTANCE)
		{
			SetState(SLASHER_WEAPON_ATTACK02_STATE);
			return EEnemyStateID.SLASHER_WEAPON_ATTACK02;
		}
		int num = Random.Range(0, 100);
		if ((float)num < SlasherParameter.WEAPON_PROBABILITY_SINGLE_ATTACK[(int)mMindState])
		{
			SetState(SLASHER_WEAPON_ATTACK01_STATE);
			return EEnemyStateID.SLASHER_WEAPON_ATTACK01;
		}
		SetState(SLASHER_WEAPON_ATTACK02_STATE);
		return EEnemyStateID.SLASHER_WEAPON_ATTACK02;
	}

	private EEnemyStateID ChooseMiddleWeaponAttack()
	{
		EEnemyStateID eEnemyStateID = EEnemyStateID.NO_STATE;
		int num = Random.Range(0, 100);
		if ((float)num < SlasherParameter.WEAPON_PROBABILITY_CATCHING[(int)mMindState])
		{
			SetState(SLASHER_WEAPON_RUN_STATE);
			return EEnemyStateID.SLASHER_WEAPON_RUN;
		}
		if ((float)num < SlasherParameter.WEAPON_PROBABILITY_RAIN[(int)mMindState])
		{
			SetState(SLASHER_WEAPON_RANGE01_STATE);
			return EEnemyStateID.SLASHER_WEAPON_RANGE01;
		}
		if ((float)num < SlasherParameter.WEAPON_PROBABILITY_WAVE[(int)mMindState])
		{
			SetState(SLASHER_WEAPON_WAVE_STATE);
			return EEnemyStateID.SLASHER_WEAPON_WAVE;
		}
		BladeWindNumber = ((Random.Range(0, 100) >= 50) ? SlasherParameter.WEAPON_BLADE_WIND_NUMBER2 : SlasherParameter.WEAPON_BLADE_WIND_NUMBER1);
		if (GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			SlasherBladeWindRequest request = new SlasherBladeWindRequest(base.EnemyID, (byte)BladeWindNumber);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
		SetState(SLASHER_WEAPON_BLADE_STATE);
		return EEnemyStateID.SLASHER_WEAPON_BLADE;
	}

	private EEnemyStateID ChooseFarWeaponAttack()
	{
		EEnemyStateID eEnemyStateID = EEnemyStateID.NO_STATE;
		SetState(SLASHER_WEAPON_DASH01_STATE);
		return EEnemyStateID.SLASHER_WEAPON_DASH01;
	}

	private void CheckWeaponOn()
	{
	}

	private void CheckWeaponOff()
	{
	}

	private void PlayEyeEffect()
	{
		mEyeAnimation[AnimationString.SLASHER_EYE_EFFECT].wrapMode = WrapMode.ClampForever;
		mEyeAnimation[AnimationString.SLASHER_EYE_EFFECT].time = 0f;
		mEyeAnimation.CrossFade(AnimationString.SLASHER_EYE_EFFECT, 0f, PlayMode.StopAll);
	}

	public override void StartBossInit()
	{
		CreateNavMeshAgent();
		mIsShoot = false;
		mIsWaveEffect = false;
		PlayEyeEffect();
		PlaySound(AudioName.SLASHER_ATTACK03);
	}

	public override void DoBossInit()
	{
		DoSlasherAttack03();
		SetCanTurn(false);
	}

	public override void EndBossInit()
	{
		base.EndBossInit();
		StopSound(AudioName.SLASHER_ATTACK03);
	}

	public override void StartDead()
	{
		base.StartDead();
		mDeadTimer.Do();
		entityObject.layer = PhysicsLayer.DEFAULT;
		GameObject original = Resources.Load("_Old/_Effect/Dragon/dragon_dead_blood") as GameObject;
		Object.Instantiate(original, bipObject.transform.position, Quaternion.identity);
		PlayAnimation(AnimationString.ENEMY_DEAD, WrapMode.ClampForever);
		if (GameApp.GetInstance().GetGameMode().IsSingle())
		{
			AddCashAndExpToPlayer();
		}
		PlaySound(AudioName.SLASHER_DEAD);
	}

	public void FindWeaponByID(byte weaponID)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag(TagName.SLASHER_STICK);
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			SlasherStickScript component = gameObject.GetComponent<SlasherStickScript>();
			if (null != component && component.ID == weaponID)
			{
				mTargetStick = component;
				break;
			}
		}
	}

	private SlasherStickScript FindWeapon()
	{
		SlasherStickScript slasherStickScript = null;
		float num = 0f;
		GameObject[] array = GameObject.FindGameObjectsWithTag(TagName.SLASHER_STICK);
		if (array.Length > 0)
		{
			slasherStickScript = array[0].GetComponent<SlasherStickScript>();
			Vector3 vector = slasherStickScript.SlasherStart.position - entityTransform.position;
			vector.y = 0f;
			num = Vector3.Angle(vector, slasherStickScript.SlasherStart.forward);
			for (int i = 1; i < array.Length; i++)
			{
				SlasherStickScript component = array[1].GetComponent<SlasherStickScript>();
				vector = component.SlasherStart.position - entityTransform.position;
				vector.y = 0f;
				float num2 = Vector3.Angle(vector, component.SlasherStart.forward);
				if (num2 < num)
				{
					slasherStickScript = component;
					num = num2;
				}
			}
		}
		if (slasherStickScript != null && num > 60f)
		{
			slasherStickScript = null;
		}
		return slasherStickScript;
	}

	protected override void MakeDecisionInEnemyIdle()
	{
		if (mTarget == null)
		{
			ChooseTargetPlayer(true);
		}
		if (mTarget == null)
		{
			return;
		}
		EEnemyStateID eEnemyStateID = EEnemyStateID.NO_STATE;
		GameUnit gameUnit = GetCurrentTarget();
		if (GetIdleTimeDuration() > mMaxIdleTime)
		{
			mTargetStick = null;
			if (!IsInBossCoop && (float)Hp < (float)MaxHp * mHpPercetageToRage && Time.time - mStartWeaponOffTime > SlasherParameter.MAX_WEAPON_OFF_TIME)
			{
				mTargetStick = FindWeapon();
			}
			if (mNextAction == ENextAction.CoopDead)
			{
				SetInDefend();
				eEnemyStateID = EEnemyStateID.BOSS_RAGE;
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
				{
					BossCoopRequest bossCoopRequest = new BossCoopRequest(6, 4);
					bossCoopRequest.Data.AddInt(base.EnemyID);
					GameApp.GetInstance().GetNetworkManager().SendRequest(bossCoopRequest);
				}
			}
			else if (mTargetStick != null)
			{
				if (GameApp.GetInstance().GetGameMode().IsCoopMode())
				{
					SlasherStickRequest request = new SlasherStickRequest(base.EnemyID, mTargetStick.ID);
					GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				}
				SetState(SLASHER_WEAPON_GET_DASH01_STATE);
				eEnemyStateID = EEnemyStateID.SLASHER_WEAPON_GET_DASH01;
			}
			else if (mMeleeAttackCount >= mMaxMeleeAttackCount)
			{
				gameUnit = GetRandomTarget();
				SetState(SLASHER_STEPBACK_STATE);
				eEnemyStateID = EEnemyStateID.SLASHER_STEPBACK;
			}
			else
			{
				float averagehorizontalDistance = GetAveragehorizontalDistance();
				if (averagehorizontalDistance < SlasherParameter.MELEE_ATTACK_DISTANCE)
				{
					gameUnit = GetNearestTarget();
					ChangeTarget(gameUnit);
					eEnemyStateID = ChooseNearAttack();
				}
				else if (averagehorizontalDistance < SlasherParameter.DASH_ATTACK_DISTANCE)
				{
					gameUnit = GetRandomTarget();
					eEnemyStateID = ChooseMiddleAttack();
				}
				else
				{
					gameUnit = GetFarthestTarget();
					eEnemyStateID = ChooseFarAttack();
				}
			}
		}
		if (eEnemyStateID == EEnemyStateID.NO_STATE)
		{
			return;
		}
		ChangeTarget(gameUnit);
		if (GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			if (gameUnit != null)
			{
				EnemyChangeTargetRequest request2 = new EnemyChangeTargetRequest(base.EnemyID, (gameUnit as Player).GetChannelID(), entityTransform.position);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
			}
			EnemyStateRequest request3 = new EnemyStateRequest(base.EnemyID, GetID(eEnemyStateID), entityTransform.position);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request3);
		}
	}

	public override void StartEnemyIdle()
	{
		base.StartEnemyIdle();
		mMaxIdleTime = Random.Range(SlasherParameter.MIN_IDLE_TIME[(int)mMindState], SlasherParameter.MAX_IDLE_TIME[(int)mMindState]);
		if (Random.Range(0, 100) < 50)
		{
			mIdleAnimationName = AnimationString.ENEMY_IDLE;
		}
		else
		{
			mIdleAnimationName = AnimationString.SLASHER_IDLE02;
		}
	}

	public override void DoEnemyIdle()
	{
		PlayAnimation(mIdleAnimationName, WrapMode.Loop);
	}

	protected override void MakeDecisionInCatching()
	{
		EEnemyStateID eEnemyStateID = EEnemyStateID.NO_STATE;
		GameUnit currentTarget = GetCurrentTarget();
		if (base.SqrDistanceFromPlayer < SlasherParameter.MELEE_ATTACK_DISTANCE * SlasherParameter.MELEE_ATTACK_DISTANCE)
		{
			eEnemyStateID = ChooseNearAttack();
		}
		else if (GetCatchingTimeDuration() > SlasherParameter.MAX_CATCHING_TIME)
		{
			eEnemyStateID = ((!(base.SqrDistanceFromPlayer < SlasherParameter.DASH_ATTACK_DISTANCE * SlasherParameter.DASH_ATTACK_DISTANCE)) ? ChooseFarAttack() : ChooseMiddleAttack());
		}
		if (eEnemyStateID != EEnemyStateID.NO_STATE && GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			EnemyStateRequest request = new EnemyStateRequest(base.EnemyID, GetID(eEnemyStateID), entityTransform.position);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
	}

	public override void StartCatching()
	{
		base.StartCatching();
		PlaySoundLoop(AudioName.SLASHER_RUN);
	}

	public override void EndCatching()
	{
		base.EndCatching();
		StopSound(AudioName.SLASHER_RUN);
	}

	public override void StartGotHit()
	{
		base.StartGotHit();
		PlaySound(AudioName.SLASHER_ATTACKED);
	}

	public override void DoGotHit()
	{
		string name = AnimationString.ENEMY_GOTHIT;
		if (mHasWeapon)
		{
			name = AnimationString.SLASHER_WEAPON_ATTACKED;
		}
		PlayAnimation(name, WrapMode.ClampForever);
		if (AnimationPlayed(name, 1f))
		{
			if (NeedRage())
			{
				SetState(EnemyBoss.BOSS_RAGE_STATE);
			}
			else if (mHasWeapon)
			{
				SetState(SLASHER_WEAPON_IDLE_STATE);
			}
			else
			{
				SetState(Enemy.IDLE_STATE);
			}
		}
	}

	public override void EndGotHit()
	{
		base.EndGotHit();
		StopSound(AudioName.SLASHER_ATTACKED);
	}

	public void StartSlasherAttack01()
	{
		CheckWeaponOff();
		mIsShoot = false;
		mIsTraceEffect = false;
		LookAtTargetHorizontal();
		mMeleeAttackCount++;
		mMaxMeleeAttackCount = Random.Range(SlasherParameter.MELEE_ATTACK_MIN_COUNT, SlasherParameter.MELLE_ATTACK_MAX_COUNT + 1);
		PlaySound(AudioName.SLASHER_ATTACK01);
	}

	public void DoSlasherAttack01()
	{
		PlayAnimation(AnimationString.BOSS_ATTACK01, WrapMode.ClampForever);
		float num = animation[AnimationString.BOSS_ATTACK01].time / animation[AnimationString.BOSS_ATTACK01].clip.length;
		if (!mIsTraceEffect && num > 0.35f)
		{
			mIsTraceEffect = true;
			GameObject original = Resources.Load(EffectName.SLASHER_TRACE) as GameObject;
			GameObject gameObject = Object.Instantiate(original) as GameObject;
			gameObject.transform.parent = entityTransform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
		}
		if (!mIsShoot && num > 0.35f && num < 0.55f && mRightArmCollider.bounds.Intersects(mLocalPlayer.GetCollider().bounds))
		{
			mLocalPlayer.OnHit(CreateDamageProperty(1));
			mIsShoot = true;
			CheckKnocked(SlasherParameter.MELEE_ATTACK01_KNOCK_SPEED);
		}
		if (num > 1f)
		{
			SetState(Enemy.IDLE_STATE);
		}
	}

	public void EndSlasherAttack01()
	{
		StopSound(AudioName.SLASHER_ATTACK01);
	}

	public void StartSlasherAttack02()
	{
		CheckWeaponOff();
		mIsShoot = false;
		LookAtTargetHorizontal();
		mMeleeAttackCount++;
		mMaxMeleeAttackCount = Random.Range(SlasherParameter.MELEE_ATTACK_MIN_COUNT, SlasherParameter.MELLE_ATTACK_MAX_COUNT + 1);
		PlaySound(AudioName.SLASHER_ATTACK01);
	}

	public void DoSlasherAttack02()
	{
		PlayAnimation(AnimationString.BOSS_ATTACK02, WrapMode.ClampForever);
		float num = animation[AnimationString.BOSS_ATTACK02].time / animation[AnimationString.BOSS_ATTACK02].clip.length;
		if (!mIsShoot && num > 0.48f && num < 0.6f && (mLeftArmCollider.bounds.Intersects(mLocalPlayer.GetCollider().bounds) || mRightArmCollider.bounds.Intersects(mLocalPlayer.GetCollider().bounds)))
		{
			mLocalPlayer.OnHit(CreateDamageProperty(2));
			mIsShoot = true;
			CheckKnocked(SlasherParameter.MELEE_ATTACK02_KNOCK_SPEED);
		}
		if (AnimationPlayed(AnimationString.BOSS_ATTACK02, 1f))
		{
			SetState(Enemy.IDLE_STATE);
		}
	}

	public void EndSlasherAttack02()
	{
		StopSound(AudioName.SLASHER_ATTACK01);
	}

	public void StartSlasherAttack03()
	{
		CheckWeaponOff();
		LookAtTargetHorizontal();
		mIsShoot = false;
		mIsWaveEffect = false;
		PlayEyeEffect();
		PlaySound(AudioName.SLASHER_ATTACK03);
	}

	public void DoSlasherAttack03()
	{
		PlayAnimation(AnimationString.BOSS_ATTACK03, WrapMode.ClampForever);
		float num = animation[AnimationString.BOSS_ATTACK03].time / animation[AnimationString.BOSS_ATTACK03].clip.length;
		if (!mIsWaveEffect)
		{
			LookAtTargetHorizontal();
		}
		if (!mIsWaveEffect && num > 0.36f)
		{
			mIsWaveEffect = true;
			GameObject original = Resources.Load(EffectName.SLASHER_WAVE_ROUND) as GameObject;
			Object.Instantiate(original, entityTransform.position + entityTransform.forward * SlasherParameter.WAVE_OFFSET, Quaternion.identity);
			GameObject original2 = Resources.Load(EffectName.SLASHER_WAVE_DIRECT) as GameObject;
			GameObject gameObject = Object.Instantiate(original2, entityTransform.position + entityTransform.forward * SlasherParameter.WAVE_OFFSET, Quaternion.LookRotation(entityTransform.forward)) as GameObject;
			SlasherWaveRootScript component = gameObject.GetComponent<SlasherWaveRootScript>();
			if (null != component)
			{
				component.SetDamageInfo(SlasherParameter.GetInstance(), 10);
				component.KnockSpeed = SlasherParameter.WAVE_LIGHT_KNOCK_SPEED;
				component.Attacker = this;
			}
		}
		if (!mIsShoot && num > 0.5f && num < 0.8f && (entityTransform.position + entityTransform.forward * SlasherParameter.WAVE_OFFSET - mLocalPlayer.GetTransform().position).sqrMagnitude < SlasherParameter.WAVE_RADIUS * SlasherParameter.WAVE_RADIUS)
		{
			mIsShoot = true;
			mLocalPlayer.OnHit(CreateDamageProperty(9));
			CheckKnocked(SlasherParameter.WAVE_KNOCK_SPEED);
		}
		if (num > 1f)
		{
			SetState(Enemy.IDLE_STATE);
		}
	}

	public void EndSlasherAttack03()
	{
		StopSound(AudioName.SLASHER_ATTACK03);
	}

	public void StartSlasherDash01()
	{
		CheckWeaponOff();
		LookAtTargetHorizontal();
	}

	public void DoSlasherDash01()
	{
		PlayAnimation(AnimationString.BOSS_DASH01, WrapMode.ClampForever);
		if (AnimationPlayed(AnimationString.BOSS_DASH01, 1f))
		{
			SetState(SLASHER_DASH02_STATE);
		}
	}

	public void EndSlasherDash01()
	{
	}

	private void SetNavMeshAgentForDash(float speed)
	{
		mDashTarget = mTargetPosition;
		Vector3 vector = mTargetPosition - entityTransform.position;
		vector.y = 0f;
		Vector3 normalized = vector.normalized;
		float magnitude = vector.magnitude;
		mRay = new Ray(entityTransform.position + new Vector3(0f, 0.5f, 0f), normalized);
		int layerMask = 1 << PhysicsLayer.WALL_AGAINST_PALYER;
		magnitude = ((!Physics.Raycast(mRay, out mRaycastHit, magnitude + SlasherParameter.DASH_OVER_DISTANCE + SlasherParameter.MINIMUM_DISTANCE_BEFORE_WALL, layerMask)) ? (magnitude + SlasherParameter.DASH_OVER_DISTANCE) : ((entityTransform.position + new Vector3(0f, 0.5f, 0f) - mRaycastHit.point).magnitude - SlasherParameter.MINIMUM_DISTANCE_BEFORE_WALL));
		mDashTarget = entityTransform.position + normalized * magnitude;
		if (null != mNavMeshAgent)
		{
			mNavMeshAgent.enabled = true;
			mNavMeshAgent.Resume();
			mNavMeshAgent.SetDestination(mDashTarget);
			mNavMeshAgent.speed = speed;
			SetCanTurn(false);
		}
	}

	public void CheckDashHit()
	{
		if (!mIsShoot)
		{
			float sqrMagnitude = (mLocalPlayer.GetTransform().position - entityTransform.position).sqrMagnitude;
			if (sqrMagnitude < SlasherParameter.DASH_CHECK_RADIUS * SlasherParameter.DASH_CHECK_RADIUS)
			{
				mIsShoot = true;
			}
			else if (sqrMagnitude < SlasherParameter.DASH_SECTOR_RADIUS * SlasherParameter.DASH_SECTOR_RADIUS && GetLocalPlayerHorizontalAbsAngle() < SlasherParameter.DASH_SECTOR_ANGLE)
			{
				mIsShoot = true;
			}
			if (mIsShoot)
			{
				mLocalPlayer.OnHit(CreateDamageProperty(5));
				CheckKnocked(SlasherParameter.DASH_KNOCK_SPEED);
			}
		}
	}

	public void StartSlasherDash02()
	{
		UpdateNavMeshInDash(true, SlasherParameter.DASH_SPEED[(int)mMindState]);
		mIsShoot = false;
		mIsTargetLock = false;
		PlaySoundLoop(AudioName.SLASHER_DASH02);
	}

	public void DoSlasherDash02()
	{
		PlayAnimation(AnimationString.BOSS_DASH02, WrapMode.Loop);
		CheckDashHit();
		if (!mIsTargetLock)
		{
			if ((mLocalPlayer.GetTransform().position - entityTransform.position).sqrMagnitude < SlasherParameter.DASH_LOCK_DISTANCE * SlasherParameter.DASH_LOCK_DISTANCE)
			{
				mIsTargetLock = true;
				SetNavMeshAgentForDash(SlasherParameter.DASH_SPEED[(int)mMindState]);
			}
			else
			{
				UpdateNavMeshInDash(false, SlasherParameter.DASH_SPEED[(int)mMindState]);
			}
		}
		else if ((entityTransform.position - mDashTarget).sqrMagnitude < SlasherParameter.DASH_STOP_DISTANCE * SlasherParameter.DASH_STOP_DISTANCE)
		{
			SetState(SLASHER_DASH03_STATE);
		}
	}

	public void EndSlasherDash02()
	{
		StopNavMesh();
		StopSound(AudioName.SLASHER_DASH02);
	}

	public void StartSlasherDash03()
	{
		if (null != mNavMeshAgent)
		{
			mNavMeshAgent.enabled = true;
			mNavMeshAgent.Resume();
			mNavMeshAgent.SetDestination(mDashTarget);
			mNavMeshAgent.speed = SlasherParameter.DASH_SPEED[(int)mMindState];
			SetCanTurn(false);
		}
	}

	public void DoSlasherDash03()
	{
		PlayAnimation(AnimationString.BOSS_DASH03, WrapMode.ClampForever);
		CheckDashHit();
		if (AnimationPlayed(AnimationString.BOSS_DASH03, 1f))
		{
			SetState(Enemy.IDLE_STATE);
		}
	}

	public void EndSlasherDash03()
	{
		StopNavMesh();
	}

	private void ShotThornRain(float angleH, float angleV)
	{
		GameObject original = Resources.Load(EffectName.SLASHER_ARROW) as GameObject;
		GameObject gameObject = Object.Instantiate(original, mMouthTransform.position, Quaternion.identity) as GameObject;
		Vector3 vector = mTargetPosition - mMouthTransform.position;
		vector.y = 0f;
		Vector3 normalized = vector.normalized;
		float magnitude = vector.magnitude;
		float num = magnitude / SlasherParameter.RAIN_SPEED;
		float num2 = mMouthTransform.position.y - mFloorHeight;
		float num3 = num2 / num + 0.5f * Physics.gravity.y * num;
		Vector3 vector2 = num3 * Vector3.down;
		Vector3 vector3 = vector2 + SlasherParameter.RAIN_SPEED * normalized;
		gameObject.transform.LookAt(mMouthTransform.position + vector3 * 10f);
		gameObject.transform.RotateAround(mMouthTransform.position, gameObject.transform.up, angleH);
		gameObject.transform.RotateAround(mMouthTransform.position, gameObject.transform.right, 0f - angleV);
		SlasherThornScript component = gameObject.GetComponent<SlasherThornScript>();
		component.Speed = gameObject.transform.forward * vector3.magnitude;
		component.SetDamageInfo(SlasherParameter.GetInstance(), 7);
		component.ExplodeRadius = SlasherParameter.RAIN_EXPLODE_RADIUS;
		component.ExplosionTime = SlasherParameter.RAIN_EXPLODE_TIME;
		component.SetSlasher(this);
	}

	public void StartSlasherRange01()
	{
		CheckWeaponOff();
		LookAtTargetHorizontal();
		GetGround();
		PlaySound(AudioName.SLASHER_RANGE01);
	}

	public void DoSlasherRange01()
	{
		LookAtTargetHorizontal();
		PlayAnimation(AnimationString.SLASHER_RANGE01, WrapMode.ClampForever);
		float num = animation[AnimationString.SLASHER_RANGE01].time / animation[AnimationString.SLASHER_RANGE01].clip.length;
		if (mRainTimer.Ready() && num > 0.45f && num < 0.66f)
		{
			mRainTimer.Do();
			float angleH = (float)Random.Range(-100, 100) * SlasherParameter.RAIN_HORIZONTAL_RANGE / 100f;
			float angleV = (float)Random.Range(-100, 100) * (SlasherParameter.RAIN_VERTICAL_UP_RANGE + SlasherParameter.RAIN_VERTICAL_DOWN_RANGE) / 100f - SlasherParameter.RAIN_VERTICAL_DOWN_RANGE;
			ShotThornRain(angleH, angleV);
		}
		if (num > 1f)
		{
			SetState(Enemy.IDLE_STATE);
		}
	}

	public void EndSlasherRange01()
	{
		StopSound(AudioName.SLASHER_RANGE01);
	}

	public void StartSlasherRange02()
	{
		CheckWeaponOff();
		mIsShoot = false;
		mIsLeap = false;
		PlayEyeEffect();
		PlaySound(AudioName.SLASHER_RANGE02);
	}

	public void DoSlasherRange02()
	{
		PlayAnimation(AnimationString.SLASHER_RANGE02, WrapMode.ClampForever);
		float num = animation[AnimationString.SLASHER_RANGE02].time / animation[AnimationString.SLASHER_RANGE02].clip.length;
		if (num > 0.1f)
		{
			if (num < 0.3f)
			{
				LookAtTargetHorizontal();
			}
			else
			{
				if (null != mNavMeshAgent)
				{
					if (!mIsLeap)
					{
						mIsLeap = true;
						Vector3 vector = mTargetPosition;
						Vector3 vector2 = mTargetPosition - entityTransform.position;
						vector2.y = 0f;
						Vector3 normalized = vector2.normalized;
						float magnitude = vector2.magnitude;
						mRay = new Ray(entityTransform.position + new Vector3(0f, 0.5f, 0f), normalized);
						int layerMask = 1 << PhysicsLayer.WALL_AGAINST_PALYER;
						magnitude = ((!Physics.Raycast(mRay, out mRaycastHit, magnitude + SlasherParameter.LEAP_OVER_DISTANCE + SlasherParameter.MINIMUM_DISTANCE_BEFORE_WALL, layerMask)) ? (magnitude + SlasherParameter.LEAP_OVER_DISTANCE) : ((entityTransform.position + new Vector3(0f, 0.5f, 0f) - mRaycastHit.point).magnitude - SlasherParameter.MINIMUM_DISTANCE_BEFORE_WALL));
						vector = entityTransform.position + normalized * magnitude;
						mNavMeshAgent.enabled = true;
						mNavMeshAgent.Resume();
						mNavMeshAgent.SetDestination(vector);
						mNavMeshAgent.speed = SlasherParameter.LEAP_MAXIMUM_SPEED * magnitude;
						SetCanTurn(false);
						mLeapMinSpeed = SlasherParameter.LEAP_MINIMUM_SPEED * magnitude;
						mLeapDeceleration = (SlasherParameter.LEAP_MAXIMUM_SPEED - SlasherParameter.LEAP_MINIMUM_SPEED) / SlasherParameter.LEAP_TIME * magnitude;
					}
					else if (mNavMeshAgent.speed > mLeapMinSpeed)
					{
						mNavMeshAgent.speed = Mathf.Max(mLeapMinSpeed, mNavMeshAgent.speed - mLeapDeceleration * Time.deltaTime);
					}
				}
				if (!mIsShoot)
				{
					float sqrMagnitude = (mLocalPlayer.GetTransform().position - entityTransform.position).sqrMagnitude;
					if (sqrMagnitude < SlasherParameter.LEAP_CHECK_RADIUS * SlasherParameter.LEAP_CHECK_RADIUS)
					{
						mIsShoot = true;
					}
					else if (sqrMagnitude < SlasherParameter.LEAP_SECTOR_RADIUS * SlasherParameter.LEAP_SECTOR_RADIUS && GetLocalPlayerHorizontalAbsAngle() < SlasherParameter.LEAP_SECTOR_ANGLE)
					{
						mIsShoot = true;
					}
					if (mIsShoot)
					{
						mLocalPlayer.OnHit(CreateDamageProperty(8));
						CheckKnocked(SlasherParameter.LEAP_KNOCK_SPEED);
					}
				}
			}
		}
		if (num > 1f)
		{
			SetState(Enemy.IDLE_STATE);
		}
	}

	public void EndSlasherRange02()
	{
		StopNavMesh();
		StopSound(AudioName.SLASHER_RANGE02);
	}

	public void StartSlasherWeaponGetDash01()
	{
		CheckWeaponOff();
		LookAtPointHorizontal(mTargetStick.SlasherStart.position);
		mMeleeAttackCount = 0;
	}

	public void DoSlasherWeaponGetDash01()
	{
		PlayAnimation(AnimationString.BOSS_DASH01, WrapMode.ClampForever);
		if (AnimationPlayed(AnimationString.BOSS_DASH01, 1f))
		{
			SetState(SLASHER_WEAPON_GET_DASH02_STATE);
		}
	}

	public void EndSlasherWeaponGetDash01()
	{
	}

	public void StartSlasherWeaponGetDash02()
	{
		if (null != mNavMeshAgent)
		{
			mNavMeshAgent.enabled = true;
			mNavMeshAgent.Resume();
			mNavMeshAgent.SetDestination(mTargetStick.SlasherStart.position);
			mNavMeshAgent.speed = SlasherParameter.GET_WEAPON_DASH_SPEED;
			SetCanTurn(false);
		}
		mIsShoot = false;
		PlaySoundLoop(AudioName.SLASHER_DASH02);
	}

	public void DoSlasherWeaponGetDash02()
	{
		PlayAnimation(AnimationString.BOSS_DASH02, WrapMode.Loop);
		if (!mIsShoot && (mLocalPlayer.GetTransform().position - entityTransform.position).sqrMagnitude < SlasherParameter.DASH_CHECK_RADIUS * SlasherParameter.DASH_CHECK_RADIUS)
		{
			mIsShoot = true;
			mLocalPlayer.OnHit(CreateDamageProperty(5));
			CheckKnocked(SlasherParameter.DASH_KNOCK_SPEED);
		}
		if ((entityTransform.position - mTargetStick.SlasherStart.position).sqrMagnitude < 1f)
		{
			SetState(SLASHER_WEAPON_GET_DASH03_STATE);
		}
	}

	public void EndSlasherWeaponGetDash02()
	{
		StopNavMesh();
		StopSound(AudioName.SLASHER_DASH02);
	}

	public void StartSlasherWeaponGetDash03()
	{
		if (null != mNavMeshAgent)
		{
			mNavMeshAgent.enabled = true;
			mNavMeshAgent.Resume();
			mNavMeshAgent.SetDestination(mTargetStick.SlasherEnd.position);
			mNavMeshAgent.speed = SlasherParameter.GET_WEAPON_DASH_SPEED;
			SetCanTurn(false);
		}
		mIsShoot = false;
		PlaySoundLoop(AudioName.SLASHER_DASH02);
	}

	public void DoSlasherWeaponGetDash03()
	{
		PlayAnimation(AnimationString.BOSS_DASH02, WrapMode.Loop);
		if (!mIsShoot && (mLocalPlayer.GetTransform().position - entityTransform.position).sqrMagnitude < SlasherParameter.DASH_CHECK_RADIUS * SlasherParameter.DASH_CHECK_RADIUS)
		{
			mIsShoot = true;
			mLocalPlayer.OnHit(CreateDamageProperty(5));
			CheckKnocked(SlasherParameter.DASH_KNOCK_SPEED);
		}
		if ((entityTransform.position - mTargetStick.SlasherEnd.position).sqrMagnitude < 1f)
		{
			SetState(SLASHER_WEAPON_GET_STATE);
		}
	}

	public void EndSlasherWeaponGetDash03()
	{
		StopNavMesh();
		StopSound(AudioName.SLASHER_DASH02);
	}

	public void StartSlasherWeaponGet()
	{
		entityTransform.position = mTargetStick.SlasherEnd.position;
		LookAtPointHorizontalAtOnce(entityTransform.position + mTargetStick.SlasherEnd.forward);
		mIsShoot = false;
		PlaySound(AudioName.SLASHER_WEAPON_GET);
	}

	public void DoSlasherWeaponGet()
	{
		PlayAnimation(AnimationString.SLASHER_WEAPON_GET, WrapMode.ClampForever);
		if (!mIsShoot && AnimationPlayed(AnimationString.SLASHER_WEAPON_GET, 0.13f))
		{
			mIsShoot = true;
			mTargetStick.gameObject.transform.parent = mWeaponTransform;
			mTargetStick.gameObject.transform.localPosition = Vector3.zero;
			mTargetStick.gameObject.transform.localRotation = Quaternion.identity;
			mTargetStick.gameObject.tag = "Untagged";
			mTargetStick.gameObject.layer = PhysicsLayer.DEFAULT;
			mWeaponCollider = mTargetStick.gameObject.GetComponent<Collider>();
			mHasWeapon = true;
			mStartWeaponOnTime = Time.time;
		}
		if (AnimationPlayed(AnimationString.SLASHER_WEAPON_GET, 1f))
		{
			SetState(SLASHER_WEAPON_IDLE_STATE);
		}
	}

	public void EndSlasherWeaponGet()
	{
		StopSound(AudioName.SLASHER_WEAPON_GET);
	}

	private void MakeDecisionInWeaponIdle()
	{
		if (mTarget == null)
		{
			ChooseTargetPlayer(true);
		}
		if (mTarget == null)
		{
			return;
		}
		EEnemyStateID eEnemyStateID = EEnemyStateID.NO_STATE;
		GameUnit gameUnit = GetCurrentTarget();
		if (GetIdleTimeDuration() > mMaxIdleTime)
		{
			if (Time.time - mStartWeaponOnTime > SlasherParameter.MAX_WEAPON_ON_TIME)
			{
				gameUnit = GetFarthestTarget();
				SetState(SLASHER_WEAPON_DROP_STATE);
				eEnemyStateID = EEnemyStateID.SLASHER_WEAPON_DROP;
			}
			else if (mMeleeAttackCount >= mMaxMeleeAttackCount)
			{
				gameUnit = GetRandomTarget();
				SetState(SLASHER_WEAPON_STEPBACK_STATE);
				eEnemyStateID = EEnemyStateID.SLASHER_WEAPON_STEPBACK;
			}
			else
			{
				float averagehorizontalDistance = GetAveragehorizontalDistance();
				if (averagehorizontalDistance < SlasherParameter.WEAPON_MELEE_ATTACK_DISTANCE)
				{
					gameUnit = GetNearestTarget();
					ChangeTarget(gameUnit);
					eEnemyStateID = ChooseNearWeaponAttack();
				}
				else if (averagehorizontalDistance < SlasherParameter.DASH_ATTACK_DISTANCE)
				{
					gameUnit = GetRandomTarget();
					eEnemyStateID = ChooseMiddleWeaponAttack();
				}
				else
				{
					gameUnit = GetFarthestTarget();
					eEnemyStateID = ChooseFarWeaponAttack();
				}
			}
		}
		if (eEnemyStateID == EEnemyStateID.NO_STATE)
		{
			return;
		}
		ChangeTarget(gameUnit);
		if (GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			if (gameUnit != null)
			{
				EnemyChangeTargetRequest request = new EnemyChangeTargetRequest(base.EnemyID, (gameUnit as Player).GetChannelID(), entityTransform.position);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			EnemyStateRequest request2 = new EnemyStateRequest(base.EnemyID, GetID(eEnemyStateID), entityTransform.position);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
		}
	}

	public void StartSlasherWeaponIdle()
	{
		SetIdleTimeNow();
		mMaxIdleTime = Random.Range(SlasherParameter.MIN_WEAPON_IDLE_TIME[(int)mMindState], SlasherParameter.MAX_WEAPON_IDLE_TIME[(int)mMindState]);
	}

	public void DoSlasherWeaponIdle()
	{
		PlayAnimation(AnimationString.SLASHER_WEAPON_IDLE, WrapMode.Loop);
		if (base.IsMasterPlayer)
		{
			MakeDecisionInWeaponIdle();
		}
	}

	public void EndSlasherWeaponIdle()
	{
	}

	private void MakeDecisionInWeaponRun()
	{
		EEnemyStateID eEnemyStateID = EEnemyStateID.NO_STATE;
		GameUnit currentTarget = GetCurrentTarget();
		if (base.SqrDistanceFromPlayer < SlasherParameter.WEAPON_MELEE_ATTACK_DISTANCE * SlasherParameter.WEAPON_MELEE_ATTACK_DISTANCE)
		{
			eEnemyStateID = ChooseNearWeaponAttack();
		}
		else if (GetCatchingTimeDuration() > SlasherParameter.MAX_CATCHING_TIME)
		{
			eEnemyStateID = ((!(base.SqrDistanceFromPlayer < SlasherParameter.DASH_ATTACK_DISTANCE * SlasherParameter.DASH_ATTACK_DISTANCE)) ? ChooseFarWeaponAttack() : ChooseMiddleWeaponAttack());
		}
		if (eEnemyStateID != EEnemyStateID.NO_STATE && GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			EnemyStateRequest request = new EnemyStateRequest(base.EnemyID, GetID(eEnemyStateID), entityTransform.position);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
	}

	private bool CheckTargetInWeaponRun()
	{
		if (mTarget == null)
		{
			SetState(SLASHER_WEAPON_IDLE_STATE);
			return false;
		}
		return true;
	}

	public void StartSlasherWeaponRun()
	{
		CheckWeaponOn();
		SetCatchingTimeNow();
		SetUpdatePosTimeNow();
		UpdateNavMeshInCatching(true);
		PlaySoundLoop(AudioName.SLASHER_RUN);
	}

	public void DoSlasherWeaponRun()
	{
		PlayAnimation(AnimationString.SLASHER_WEAPON_RUN, WrapMode.Loop);
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
				else if (CheckTargetInWeaponRun())
				{
					MakeDecisionInWeaponRun();
				}
			}
			else if (CheckTargetInWeaponRun())
			{
				MakeDecisionInWeaponRun();
			}
		}
		else
		{
			CheckTargetInWeaponRun();
		}
	}

	public void EndSlasherWeaponRun()
	{
		StopNavMesh();
		StopSound(AudioName.SLASHER_RUN);
	}

	public void StartSlasherWeaponAttack01()
	{
		CheckWeaponOn();
		mIsShoot = false;
		LookAtTargetHorizontal();
		mMaxMeleeAttackCount = Random.Range(SlasherParameter.MELEE_ATTACK_MIN_COUNT, SlasherParameter.MELLE_ATTACK_MAX_COUNT + 1);
		mMeleeAttackCount++;
		PlaySound(AudioName.SLASHER_WEAPON_ATTACK01);
		ResetAttack(mSkill.WeaponMeleeAttackSkill1);
	}

	public void DoSlasherWeaponAttack01()
	{
		PlayAnimation(AnimationString.SLASHER_WEAPON_ATTACK01, WrapMode.ClampForever);
		CheckHit2(mSkill.WeaponMeleeAttackSkill1);
		if (CurrentAnimTime > 1f)
		{
			SetState(SLASHER_WEAPON_IDLE_STATE);
		}
	}

	public void EndSlasherWeaponAttack01()
	{
		StopSound(AudioName.SLASHER_WEAPON_ATTACK01);
	}

	public void StartSlasherWeaponAttack02()
	{
		CheckWeaponOn();
		mIsShoot = false;
		LookAtTargetHorizontal();
		mMaxMeleeAttackCount = Random.Range(SlasherParameter.MELEE_ATTACK_MIN_COUNT, SlasherParameter.MELLE_ATTACK_MAX_COUNT + 1);
		mMeleeAttackCount++;
		PlaySound(AudioName.SLASHER_WEAPON_ATTACK02);
		ResetAttack(mSkill.WeaponMeleeAttackSkill2);
	}

	public void DoSlasherWeaponAttack02()
	{
		PlayAnimation(AnimationString.SLASHER_WEAPON_ATTACK02, WrapMode.ClampForever);
		CheckHit2(mSkill.WeaponMeleeAttackSkill2);
		if (CurrentAnimTime > 1f)
		{
			SetState(SLASHER_WEAPON_IDLE_STATE);
		}
	}

	public void EndSlasherWeaponAttack02()
	{
		StopSound(AudioName.SLASHER_WEAPON_ATTACK02);
	}

	public void StartSlasherWeaponDash01()
	{
		CheckWeaponOn();
		LookAtTargetHorizontal();
		mDashTarget = mTargetPosition;
		PlayEyeEffect();
	}

	public void DoSlasherWeaponDash01()
	{
		PlayAnimation(AnimationString.SLASHER_WEAPON_DASH01, WrapMode.ClampForever);
		if (AnimationPlayed(AnimationString.SLASHER_WEAPON_DASH01, 1f))
		{
			SetState(SLASHER_WEAPON_DASH02_STATE);
		}
	}

	public void EndSlasherWeaponDash01()
	{
	}

	private void CheckWeaponDashHit()
	{
		if (!mIsShoot)
		{
			float sqrMagnitude = (mLocalPlayer.GetTransform().position - entityTransform.position).sqrMagnitude;
			if (sqrMagnitude < SlasherParameter.WEAPON_DASH_CHECK_RADIUS * SlasherParameter.WEAPON_DASH_CHECK_RADIUS)
			{
				mIsShoot = true;
			}
			else if (sqrMagnitude < SlasherParameter.WEAPON_DASH_SECTOR_RADIUS * SlasherParameter.WEAPON_DASH_SECTOR_RADIUS && GetLocalPlayerHorizontalAbsAngle() < SlasherParameter.WEAPON_DASH_SECTOR_ANGLE)
			{
				mIsShoot = true;
			}
			if (mIsShoot)
			{
				mLocalPlayer.OnHit(CreateDamageProperty(6));
				CheckKnocked(SlasherParameter.WEAPON_DASH_KNOCK_SPEED);
			}
		}
	}

	public void StartSlasherWeaponDash02()
	{
		UpdateNavMeshInDash(true, SlasherParameter.WEAPON_DASH_SPEED);
		mIsShoot = false;
		mIsTargetLock = false;
		PlaySoundLoop(AudioName.SLASHER_WEAPON_DASH02);
	}

	public void DoSlasherWeaponDash02()
	{
		PlayAnimation(AnimationString.SLASHER_WEAPON_DASH02, WrapMode.Loop);
		CheckWeaponDashHit();
		if (!mIsTargetLock)
		{
			if ((mLocalPlayer.GetTransform().position - entityTransform.position).sqrMagnitude < SlasherParameter.DASH_LOCK_DISTANCE * SlasherParameter.DASH_LOCK_DISTANCE)
			{
				mIsTargetLock = true;
				SetNavMeshAgentForDash(SlasherParameter.WEAPON_DASH_SPEED);
			}
			else
			{
				UpdateNavMeshInDash(false, SlasherParameter.WEAPON_DASH_SPEED);
			}
		}
		else if ((entityTransform.position - mDashTarget).sqrMagnitude < SlasherParameter.DASH_STOP_DISTANCE * SlasherParameter.DASH_STOP_DISTANCE)
		{
			SetState(SLASHER_WEAPON_DASH03_STATE);
		}
	}

	public void EndSlasherWeaponDash02()
	{
		StopNavMesh();
		StopSound(AudioName.SLASHER_WEAPON_DASH02);
	}

	public void StartSlasherWeaponDash03()
	{
		if (null != mNavMeshAgent)
		{
			mNavMeshAgent.enabled = true;
			mNavMeshAgent.Resume();
			mNavMeshAgent.SetDestination(mDashTarget);
			mNavMeshAgent.speed = SlasherParameter.WEAPON_DASH_SPEED;
			SetCanTurn(false);
		}
	}

	public void DoSlasherWeaponDash03()
	{
		PlayAnimation(AnimationString.SLASHER_WEAPON_DASH03, WrapMode.ClampForever);
		CheckWeaponDashHit();
		if (AnimationPlayed(AnimationString.SLASHER_WEAPON_DASH03, 1f))
		{
			SetState(SLASHER_WEAPON_IDLE_STATE);
		}
	}

	public void EndSlasherWeaponDash03()
	{
		StopNavMesh();
	}

	public void StartSlasherWeaponRange01()
	{
		CheckWeaponOn();
		LookAtTargetHorizontal();
		GetGround();
		PlaySound(AudioName.SLASHER_RANGE01);
	}

	public void DoSlasherWeaponRange01()
	{
		LookAtTargetHorizontal();
		PlayAnimation(AnimationString.SLASHER_WEAPON_RANGE01, WrapMode.ClampForever);
		float num = animation[AnimationString.SLASHER_WEAPON_RANGE01].time / animation[AnimationString.SLASHER_WEAPON_RANGE01].clip.length;
		if (mRainTimer.Ready() && num > 0.45f && num < 0.66f)
		{
			mRainTimer.Do();
			float angleH = (float)Random.Range(-100, 100) * SlasherParameter.RAIN_HORIZONTAL_RANGE / 100f;
			float angleV = (float)Random.Range(-100, 100) * SlasherParameter.RAIN_VERTICAL_UP_RANGE / 100f;
			ShotThornRain(angleH, angleV);
		}
		if (num > 1f)
		{
			SetState(SLASHER_WEAPON_IDLE_STATE);
		}
	}

	public void EndSlasherWeaponRange01()
	{
		StopSound(AudioName.SLASHER_RANGE01);
	}

	public void StartSlasherWeaponAttacked()
	{
	}

	public void DoSlasherWeaponAttacked()
	{
	}

	public void EndSlasherWeaponAttacked()
	{
	}

	public void StartSlasherWeaponDrop()
	{
		CheckWeaponOn();
		LookAtTargetHorizontal();
		mIsShoot = false;
		mMeleeAttackCount = 0;
		PlaySound(AudioName.SLASHER_WEAPON_DROP);
	}

	public void DoSlasherWeaponDrop()
	{
		LookAtTargetHorizontal();
		PlayAnimation(AnimationString.SLASHER_WEAPON_DROP, WrapMode.ClampForever);
		if (!mIsShoot && AnimationPlayed(AnimationString.SLASHER_WEAPON_DROP, 0.51f))
		{
			mIsShoot = true;
			GameObject original = Resources.Load(EffectName.SLASHER_STICK_DROP) as GameObject;
			GameObject gameObject = Object.Instantiate(original, mTargetStick.gameObject.transform.position - 2.3f * mTargetStick.gameObject.transform.up, mTargetStick.gameObject.transform.rotation) as GameObject;
			SlasherStickDropScript component = gameObject.GetComponent<SlasherStickDropScript>();
			if (null != component)
			{
				Vector3 normalized = (mTargetPosition + Vector3.up - gameObject.transform.position).normalized;
				component.SetDamageInfo(SlasherParameter.GetInstance(), 13);
				component.Attacker = this;
				component.KnockSpeed = SlasherParameter.WEAPON_DROP_KNOCK_SPEED;
				component.MoveSpeed = normalized * SlasherParameter.WEAPON_DROP_MOVE_SPEED;
				component.RotateSpeed = SlasherParameter.WEAPON_DROP_ROTATE_SPEED;
				component.Obstacle = mTargetStick.Obstacle;
			}
			mTargetStick.gameObject.transform.parent = gameObject.transform;
			AudioManager.GetInstance().PlaySoundLoop(AudioName.SLASHER_WEAPON_SPIN, gameObject, gameObject.transform);
			mWeaponCollider = null;
			mHasWeapon = false;
			mStartWeaponOffTime = Time.time;
		}
		if (AnimationPlayed(AnimationString.SLASHER_WEAPON_DROP, 1f))
		{
			SetState(SLASHER_DASH01_STATE);
		}
	}

	public void EndSlasherWeaponDrop()
	{
		StopSound(AudioName.SLASHER_WEAPON_DROP);
	}

	public void StartSlasherWeaponWave()
	{
		CheckWeaponOn();
		LookAtTargetHorizontal();
		mIsWaveEffect = false;
		PlayEyeEffect();
		PlaySound(AudioName.SLASHER_ATTACK03);
	}

	public void DoSlasherWeaponWave()
	{
		PlayAnimation(AnimationString.SLASHER_WEAPON_ATTACK01, WrapMode.ClampForever);
		float num = animation[AnimationString.SLASHER_WEAPON_ATTACK01].time / animation[AnimationString.SLASHER_WEAPON_ATTACK01].clip.length;
		if (!mIsWaveEffect)
		{
			LookAtTargetHorizontal();
		}
		if (!mIsWaveEffect && num > 0.42f)
		{
			mIsWaveEffect = true;
			GameObject original = Resources.Load(EffectName.SLASHER_WAVE_DIRECT) as GameObject;
			GameObject gameObject = Object.Instantiate(original, entityTransform.position + entityTransform.forward * SlasherParameter.WAVE_OFFSET, Quaternion.LookRotation(entityTransform.forward)) as GameObject;
			SlasherWaveRootScript component = gameObject.GetComponent<SlasherWaveRootScript>();
			if (null != component)
			{
				component.SetDamageInfo(SlasherParameter.GetInstance(), 10);
				component.KnockSpeed = SlasherParameter.WAVE_LIGHT_KNOCK_SPEED;
				component.Attacker = this;
			}
		}
		if (num > 1f)
		{
			SetState(SLASHER_WEAPON_IDLE_STATE);
		}
	}

	public void EndSlasherWeaponWave()
	{
		StopSound(AudioName.SLASHER_ATTACK03);
	}

	private void ShotBlade()
	{
		GameObject original = Resources.Load(EffectName.SLASHER_SWORD_WIND) as GameObject;
		GameObject[] array = new GameObject[BladeWindNumber];
		for (int i = 0; i < BladeWindNumber; i++)
		{
			array[i] = Object.Instantiate(original, entityTransform.position + entityTransform.forward * 3f + Vector3.up * 0.5f, Quaternion.identity) as GameObject;
			array[i].transform.LookAt(new Vector3(mTargetPosition.x, array[i].transform.position.y, mTargetPosition.z));
			array[i].transform.RotateAround(array[i].transform.position, Vector3.up, ((float)BladeWindNumber / 2f - 0.5f - (float)i) * SlasherParameter.WEAPON_BLADE_ANGLE);
			SlasherBladeScript component = array[i].GetComponent<SlasherBladeScript>();
			component.SpeedValue = SlasherParameter.WEAPON_BLADE_SPEED;
			component.Speed = array[i].transform.forward * SlasherParameter.WEAPON_BLADE_SPEED;
			component.SetDamageInfo(SlasherParameter.GetInstance(), 12);
			component.Attacker = this;
		}
	}

	public void StartSlasherWeaponBlade()
	{
		CheckWeaponOn();
		LookAtTargetHorizontal();
		mIsShoot = false;
		PlaySound(AudioName.SLASHER_WEAPON_ATTACK02);
	}

	public void DoSlasherWeaponBlade()
	{
		PlayAnimation(AnimationString.SLASHER_WEAPON_ATTACK02, WrapMode.ClampForever);
		float num = animation[AnimationString.SLASHER_WEAPON_ATTACK02].time / animation[AnimationString.SLASHER_WEAPON_ATTACK02].clip.length;
		if (!mIsShoot)
		{
			LookAtTargetHorizontal();
		}
		if (!mIsShoot && num > 0.5f)
		{
			mIsShoot = true;
			ShotBlade();
		}
		if (num > 1f)
		{
			SetState(SLASHER_WEAPON_IDLE_STATE);
		}
	}

	public void EndSlasherWeaponBlade()
	{
		StopSound(AudioName.SLASHER_WEAPON_ATTACK02);
	}

	private void SetNavMeshAgentForStepBack()
	{
		Vector3 vector = entityTransform.position - entityTransform.forward * SlasherParameter.BACK_DISTANCE;
		Vector3 vector2 = -entityTransform.forward;
		float num = SlasherParameter.BACK_DISTANCE;
		mRay = new Ray(entityTransform.position + new Vector3(0f, 0.5f, 0f), vector2);
		int layerMask = 1 << PhysicsLayer.WALL_AGAINST_PALYER;
		if (Physics.Raycast(mRay, out mRaycastHit, num + SlasherParameter.MINIMUM_DISTANCE_BEFORE_WALL, layerMask))
		{
			num = (entityTransform.position + new Vector3(0f, 0.5f, 0f) - mRaycastHit.point).magnitude - SlasherParameter.MINIMUM_DISTANCE_BEFORE_WALL;
		}
		num = Mathf.Max(0f, num);
		vector = entityTransform.position + vector2 * num;
		mNavMeshAgent.enabled = true;
		mNavMeshAgent.Resume();
		mNavMeshAgent.SetDestination(vector);
		mNavMeshAgent.speed = SlasherParameter.BACK_MAXIMUM_SPEED * num;
		mLeapMinSpeed = SlasherParameter.BACK_MINIMUM_SPEED * num;
		mLeapDeceleration = (SlasherParameter.BACK_MAXIMUM_SPEED - SlasherParameter.BACK_MINIMUM_SPEED) / SlasherParameter.BACK_TIME * num;
	}

	public void StartSlasherStepback()
	{
		LookAtTargetHorizontal();
		CheckWeaponOff();
		mIsShoot = false;
		mIsLeap = false;
		mMeleeAttackCount = 0;
		PlaySound(AudioName.SLASHER_STEPBACK);
	}

	public void DoSlasherStepback()
	{
		LookAtTargetHorizontal();
		PlayAnimation(AnimationString.BOSS_STEP_BACK, WrapMode.ClampForever);
		float num = animation[AnimationString.BOSS_STEP_BACK].time / animation[AnimationString.BOSS_STEP_BACK].clip.length;
		if (num > 0.37f)
		{
			if (null != mNavMeshAgent)
			{
				if (!mIsLeap)
				{
					mIsLeap = true;
					SetNavMeshAgentForStepBack();
				}
				else if (mNavMeshAgent.speed > mLeapMinSpeed)
				{
					mNavMeshAgent.speed = Mathf.Max(mLeapMinSpeed, mNavMeshAgent.speed - mLeapDeceleration * Time.deltaTime);
				}
			}
			if (!mIsShoot)
			{
				float sqrMagnitude = (mLocalPlayer.GetTransform().position - entityTransform.position).sqrMagnitude;
				if (sqrMagnitude < SlasherParameter.BACK_CHECK_RADIUS * SlasherParameter.BACK_CHECK_RADIUS)
				{
					mIsShoot = true;
					mLocalPlayer.OnHit(CreateDamageProperty(14));
					CheckKnocked(SlasherParameter.BACK_KNOCK_SPEED);
				}
			}
		}
		if (num > 1f)
		{
			SetState(SLASHER_ATTACK03_STATE);
		}
	}

	public void EndSlasherStepback()
	{
		StopNavMesh();
		StopSound(AudioName.SLASHER_STEPBACK);
	}

	public void StartSlasherWeaponStepback()
	{
		LookAtTargetHorizontal();
		CheckWeaponOff();
		mIsShoot = false;
		mIsLeap = false;
		mMeleeAttackCount = 0;
		PlaySound(AudioName.SLASHER_STEPBACK);
	}

	public void DoSlasherWeaponStepback()
	{
		LookAtTargetHorizontal();
		PlayAnimation(AnimationString.SLASHER_WEAPON_STEPBACK, WrapMode.ClampForever);
		float num = animation[AnimationString.SLASHER_WEAPON_STEPBACK].time / animation[AnimationString.SLASHER_WEAPON_STEPBACK].clip.length;
		if (num > 0.37f)
		{
			if (null != mNavMeshAgent)
			{
				if (!mIsLeap)
				{
					mIsLeap = true;
					SetNavMeshAgentForStepBack();
				}
				else if (mNavMeshAgent.speed > mLeapMinSpeed)
				{
					mNavMeshAgent.speed = Mathf.Max(mLeapMinSpeed, mNavMeshAgent.speed - mLeapDeceleration * Time.deltaTime);
				}
			}
			if (!mIsShoot)
			{
				float sqrMagnitude = (mLocalPlayer.GetTransform().position - entityTransform.position).sqrMagnitude;
				if (sqrMagnitude < SlasherParameter.WEAPON_BACK_CHECK_RADIUS * SlasherParameter.WEAPON_BACK_CHECK_RADIUS)
				{
					mIsShoot = true;
					mLocalPlayer.OnHit(CreateDamageProperty(15));
					CheckKnocked(SlasherParameter.WEAPON_BACK_KNOCK_SPEED);
				}
			}
		}
		if (num > 1f)
		{
			SetState(SLASHER_WEAPON_WAVE_STATE);
		}
	}

	public void EndSlasherWeaponStepback()
	{
		StopNavMesh();
		StopSound(AudioName.SLASHER_STEPBACK);
	}

	public override bool HandleMessage(EConditionMessage message)
	{
		if (base.HandleMessage(message))
		{
			return true;
		}
		switch (message)
		{
		case EConditionMessage.ERageToIdle:
			if (mHasWeapon)
			{
				SetState(SLASHER_WEAPON_IDLE_STATE);
			}
			else
			{
				mStartWeaponOffTime = Time.time;
				SetState(Enemy.IDLE_STATE);
			}
			return true;
		default:
			return false;
		}
	}

	public override void CoopCastSkill()
	{
		int num = Random.Range(1, 3);
		SetState("CoopSkill" + num);
	}

	public override void ChangeToHitState()
	{
		if (mState != SLASHER_WEAPON_GET_STATE && mState != SLASHER_WEAPON_GET_DASH01_STATE && mState != SLASHER_WEAPON_GET_DASH02_STATE && mState != SLASHER_WEAPON_GET_DASH03_STATE)
		{
			base.ChangeToHitState();
		}
	}
}
