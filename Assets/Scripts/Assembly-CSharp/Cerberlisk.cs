using UnityEngine;

public class Cerberlisk : EnemyBoss
{
	public class Skill : BossSkill
	{
		public CreatureMeleeAttackSkill MeleeAttackSkill1;

		public CreatureMeleeAttackSkill MeleeAttackSkill2;

		public CreatureMeleeAttackSkill ThreeHitAttack1;

		public CreatureMeleeAttackSkill ThreeHitAttack2;

		public CreatureMeleeAttackSkill ThreeHitAttack3;

		public CreatureMeleeAttackSkill BreathInAttack;

		public CreatureMeleeAttackSkill DashAttackSkill;

		public CreatureMeleeAttackSkill MouthAttackSkill1;

		public CreatureMeleeAttackSkill MouthAttackSkill2;

		public Transform FirePosition;

		public Transform LeftHeadTransform;

		public GameObject PreFireVfx;

		public GameObject PreWindVfx;

		public GameObject BreatheInVfx;

		public GameObject BreatheOutVfx;

		public void Init(GameObject go)
		{
			Transform transform = go.transform.Find(BoneName.CERBERLISK_RIGHT_HEAD);
			FirePosition = go.transform.Find(BoneName.CERBERLISK_FIRE_POSITION);
			LeftHeadTransform = go.transform.Find(BoneName.CERBERLISK_LEFT_HEAD);
			PreFireVfx = Pool.sAllocate("SW2_Effect/B4_prefire");
			PreFireVfx.transform.parent = transform;
			PreFireVfx.transform.localPosition = Vector3.zero;
			PreFireVfx.transform.localRotation = Quaternion.identity;
			PreFireVfx.SetActive(false);
			PreWindVfx = Pool.sAllocate("SW2_Effect/B4_prewind");
			PreWindVfx.transform.parent = LeftHeadTransform;
			PreWindVfx.transform.localPosition = Vector3.zero;
			PreWindVfx.transform.localRotation = Quaternion.identity;
			PreWindVfx.SetActive(false);
			BreatheInVfx = Pool.sAllocate("SW2_Effect/B4_suck");
			BreatheInVfx.transform.parent = LeftHeadTransform;
			BreatheInVfx.transform.localPosition = Vector3.zero;
			BreatheInVfx.transform.localRotation = Quaternion.identity;
			BreatheInVfx.SetActive(false);
			BreatheOutVfx = Pool.sAllocate("SW2_Effect/B4_push");
			BreatheOutVfx.transform.parent = LeftHeadTransform;
			BreatheOutVfx.transform.localPosition = Vector3.zero;
			BreatheOutVfx.transform.localRotation = Quaternion.identity;
			BreatheOutVfx.SetActive(false);
			Collider collider = go.transform.Find(BoneName.CERBERLISK_LEFT_HEAD).gameObject.GetComponent<Collider>();
			Collider collider2 = transform.gameObject.GetComponent<Collider>();
			Collider collider3 = go.transform.Find(BoneName.CERBERLISK_LEFT_HAND).gameObject.GetComponent<Collider>();
			Collider collider4 = go.transform.Find(BoneName.CERBERLISK_RIGHT_HAND).gameObject.GetComponent<Collider>();
			MeleeAttackSkill1 = new CreatureMeleeAttackSkill();
			MeleeAttackSkill1.CheckTime = CerberliskParameter.MELEE_ATTACK_CHECK_TIME1;
			MeleeAttackSkill1.SetDamageInfo(CerberliskParameter.GetInstance(), 1);
			MeleeAttackSkill1.Colliders = new Collider[1];
			MeleeAttackSkill1.Colliders[0] = collider4;
			MeleeAttackSkill1.KnockDownSpeed = CerberliskParameter.MELEE_ATTACK_KNOCK_SPEED1;
			MeleeAttackSkill2 = new CreatureMeleeAttackSkill();
			MeleeAttackSkill2.CheckTime = CerberliskParameter.MELEE_ATTACK_CHECK_TIME2;
			MeleeAttackSkill2.SetDamageInfo(CerberliskParameter.GetInstance(), 2);
			MeleeAttackSkill2.Colliders = new Collider[2];
			MeleeAttackSkill2.Colliders[0] = collider;
			MeleeAttackSkill2.Colliders[1] = collider2;
			MeleeAttackSkill2.KnockDownSpeed = CerberliskParameter.MELEE_ATTACK_KNOCK_SPEED2;
			ThreeHitAttack1 = new CreatureMeleeAttackSkill();
			ThreeHitAttack1.CheckTime = CerberliskParameter.THREE_ATTACK_CHECK_TIME1;
			ThreeHitAttack1.SetDamageInfo(CerberliskParameter.GetInstance(), 3);
			ThreeHitAttack1.Radius = CerberliskParameter.THREE_ATTACK_RADIUS1;
			ThreeHitAttack1.Angle = CerberliskParameter.THREE_ATTACK_ANGLE1;
			ThreeHitAttack1.KnockDownSpeed = CerberliskParameter.THREE_ATTACK_KNOCK_SPEED1;
			ThreeHitAttack2 = new CreatureMeleeAttackSkill();
			ThreeHitAttack2.CheckTime = CerberliskParameter.THREE_ATTACK_CHECK_TIME2;
			ThreeHitAttack2.SetDamageInfo(CerberliskParameter.GetInstance(), 3);
			ThreeHitAttack2.Radius = CerberliskParameter.THREE_ATTACK_RADIUS2;
			ThreeHitAttack2.Angle = CerberliskParameter.THREE_ATTACK_ANGLE2;
			ThreeHitAttack2.KnockDownSpeed = CerberliskParameter.THREE_ATTACK_KNOCK_SPEED2;
			ThreeHitAttack3 = new CreatureMeleeAttackSkill();
			ThreeHitAttack3.CheckTime = CerberliskParameter.THREE_ATTACK_CHECK_TIME3;
			ThreeHitAttack3.SetDamageInfo(CerberliskParameter.GetInstance(), 3);
			ThreeHitAttack3.Radius = CerberliskParameter.BREATHE_IN_ATTACK_RADIUS;
			ThreeHitAttack3.Angle = CerberliskParameter.BREATHE_IN_ATTACK_ANGLE;
			ThreeHitAttack3.KnockDownSpeed = CerberliskParameter.THREE_ATTACK_KNOCK_SPEED3;
			BreathInAttack = new CreatureMeleeAttackSkill();
			BreathInAttack.CheckTime = CerberliskParameter.BREATHE_IN_ATTACK_TIME;
			BreathInAttack.SetDamageInfo(CerberliskParameter.GetInstance(), 6);
			BreathInAttack.Radius = CerberliskParameter.THREE_ATTACK_RADIUS1;
			BreathInAttack.Angle = CerberliskParameter.THREE_ATTACK_ANGLE1;
			BreathInAttack.KnockDownSpeed = CerberliskParameter.BREATHE_IN_ATTACK_KNOCK_SPEED;
			DashAttackSkill = new CreatureMeleeAttackSkill();
			DashAttackSkill.CheckTime = new Vector2(0f, 1f);
			DashAttackSkill.SetDamageInfo(CerberliskParameter.GetInstance(), 7);
			DashAttackSkill.HitRadius = CerberliskParameter.DASH_ATTACK.DASH_HIT_RADIUS;
			DashAttackSkill.KnockDownSpeed = CerberliskParameter.DASH_ATTACK.DASH_KNOCK_SPEED;
			MouthAttackSkill1 = new CreatureMeleeAttackSkill();
			MouthAttackSkill1.CheckTime = CerberliskParameter.BREATHE_IN_TIME;
			MouthAttackSkill1.SetDamageInfo(CerberliskParameter.GetInstance(), 10);
			MouthAttackSkill1.Colliders = new Collider[1];
			MouthAttackSkill1.Colliders[0] = collider;
			MouthAttackSkill1.KnockDownSpeed = CerberliskParameter.MOUTH_KNOCK_SPEED;
			MouthAttackSkill2 = new CreatureMeleeAttackSkill();
			MouthAttackSkill2.CheckTime = CerberliskParameter.BREATHE_FIRE_BALL_TIME;
			MouthAttackSkill2.SetDamageInfo(CerberliskParameter.GetInstance(), 10);
			MouthAttackSkill2.Colliders = new Collider[1];
			MouthAttackSkill2.Colliders[0] = collider;
			MouthAttackSkill2.KnockDownSpeed = CerberliskParameter.MOUTH_KNOCK_SPEED;
			MoveAudioName = AudioName.CERBERLISK_RUN;
		}
	}

	public static EnemyState CERBERLISK_INIT_STATE = new CerberliskBigFireBallState
	{
		IsInitState = true
	};

	public static EnemyState CERBERLISK_MELEE_ATTACK_STATEO1 = new CerberliskMeleeAttack1();

	public static EnemyState CERBERLISK_MELEE_ATTACK_STATEO2 = new CerberliskMeleeAttack2();

	public static EnemyState CERBERLISK_THREE_ATTACK_PRE = new CerberliskThreeAttackPre();

	public static EnemyState CERBERLISK_THREE_ATTACK1 = new Cerberlisk3HitAttack1();

	public static EnemyState CERBERLISK_THREE_ATTACK2 = new Cerberlisk3HitAttack2();

	public static EnemyState CERBERLISK_THREE_ATTACK3 = new Cerberlisk3HitAttack3();

	public static EnemyState CERBERLISK_THREE_FIRE_BALL = new Cerberlisk3FireBallState();

	public static EnemyState CERBERLISK_BIG_FIRE_BALL = new CerberliskBigFireBallState();

	public static EnemyState CERBERLISK_BREATHE_IN_STATE = new CerberliskBreatheInState();

	public static EnemyState CERBERLISK_DASH_ATTACK = new CerberliskDashAttackState();

	public static EnemyState CERBERLISK_PUSH_STATE = new CerberliskPushState();

	public static EnemyState CERBERLISK_BREATHE_FIRE_BALL_STATE = new CerberliskBreatheInFireBallState();

	public static EnemyState CERBERLISK_CYCLONE_STATE = new CerberliskCycloneState();

	public Skill mSkill = new Skill();

	private float[] mAttackRadiusSqr = new float[2];

	private float[][] mMeleeAttackChances = new float[3][];

	private float[][] mMiddleAttackChances = new float[3][];

	private float[][] mRangeAttackChances = new float[3][];

	public Cerberlisk()
	{
		base.GameUnitType = EGameUnitType.ENEMY;
		base.EnemyType = EnemyType.CERBERLISK;
		IsNewState = true;
	}

	public override void Init(GameObject gameObject)
	{
		base.Init(gameObject);
		attackRange = CerberliskParameter.MELEE_ATTACK_RADIUS;
		mAttackRadiusSqr[0] = CerberliskParameter.MELEE_ATTACK_RADIUS * CerberliskParameter.MELEE_ATTACK_RADIUS;
		mAttackRadiusSqr[1] = CerberliskParameter.RANGE_ATTACK_RADIUS * CerberliskParameter.RANGE_ATTACK_RADIUS;
		RageSoundName = AudioName.CERBERLISK_RAGE;
		bipObject = entityTransform.Find("Bip01").gameObject;
		mHpPercetageToRage = CerberliskParameter.HP_PERCENTAGE_TO_RAGE;
		mHpPercentagePerHit = CerberliskParameter.HP_PERCENTAGE_PER_HIT;
		mTurnSpeed = CerberliskParameter.TURN_SPEED;
		mRunSpeed = CerberliskParameter.RUN_SPEED[0];
		CreateShadow(1f);
		ConditionStateManager.GetInstance().InitConditionState(this);
		for (int i = 0; i < 3; i++)
		{
			mMeleeAttackChances[i] = new float[2]
			{
				CerberliskParameter.MELEE_ATTACK_CHANCE1[i],
				CerberliskParameter.MELEE_ATTACK_CHANCE2[i]
			};
			mMiddleAttackChances[i] = new float[5]
			{
				CerberliskParameter.MIDDLE_ATTACK_CHANCE1[i],
				CerberliskParameter.MIDDLE_ATTACK_CHANCE2[i],
				CerberliskParameter.MIDDLE_ATTACK_CHANCE3[i],
				CerberliskParameter.MIDDLE_ATTACK_CHANCE4[i],
				CerberliskParameter.MIDDLE_ATTACK_CHANCE5[i]
			};
			mRangeAttackChances[i] = new float[2]
			{
				CerberliskParameter.RANGE_ATTACK_CHANCE1[i],
				CerberliskParameter.RANGE_ATTACK_CHANCE2[i]
			};
		}
		mBossTouch.Init(this, mLocalPlayer, CerberliskParameter.GetInstance(), 0, CerberliskParameter.TOUCH_RADIUS, CerberliskParameter.TOUCH_KNOCK_SPEED);
		mBossDefend.Init(this);
		mSkill.Init(entityObject);
		CreateNavMeshAgent();
		mNavMeshAgent.radius = 4f;
		StepBack = CerberliskParameter.STEP_BACK;
		DashAttackParameter = CerberliskParameter.DASH_ATTACK;
	}

	protected override void MakeDecisionInEnemyIdle()
	{
		if (mTarget == null)
		{
			ChooseTargetPlayer(true);
		}
		if (mTarget == null || !(Time.time - StateStartTime > CerberliskParameter.IDLE_TIME[(int)mMindState]))
		{
			return;
		}
		GameUnit randomTarget = GetRandomTarget();
		ChangeTarget(randomTarget);
		if (GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			EnemyChangeTargetRequest request = new EnemyChangeTargetRequest(base.EnemyID, (randomTarget as Player).GetChannelID(), entityTransform.position);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
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
		}
		else if ((!IsHardMode() && mMeleeAttackHasHitPlayer >= StepBack.TRIGGER_STEP_BACK_MELEE_ATTACK_HIT_NUM) || MeleeAttackHasUsedNum >= StepBack.TRIGGER_STEP_BACK_MELEE_ATTACK_NUM)
		{
			mMeleeAttackHasHitPlayer = 0;
			MeleeAttackHasUsedNum = 0;
			SetState(EnemyBoss.BOSS_STEP_BACK_STATE);
			SendStateChangeRequest(mConditionState);
		}
		else if (base.SqrDistanceFromPlayer < mAttackRadiusSqr[0])
		{
			switch (GameWorld.GetRandomIndex(mMeleeAttackChances[(int)mMindState]))
			{
			case 0:
				SetState(CERBERLISK_MELEE_ATTACK_STATEO1);
				break;
			case 1:
				SetState(CERBERLISK_MELEE_ATTACK_STATEO2);
				break;
			default:
				Debug.LogError("dd");
				break;
			}
			SendStateChangeRequest(mConditionState);
		}
		else if (base.SqrDistanceFromPlayer < mAttackRadiusSqr[1])
		{
			StartMiddleAttack();
		}
		else
		{
			StartRangeAttack();
		}
	}

	protected override void MakeDecisionInCatching()
	{
		if (base.SqrDistanceFromPlayer < mAttackRadiusSqr[0])
		{
			switch (GameWorld.GetRandomIndex(mMeleeAttackChances[(int)mMindState]))
			{
			case 0:
				SetState(CERBERLISK_MELEE_ATTACK_STATEO1);
				break;
			case 1:
				SetState(CERBERLISK_MELEE_ATTACK_STATEO2);
				break;
			default:
				Debug.LogError("dd");
				break;
			}
			SendStateChangeRequest(mConditionState);
		}
		else if (Time.time - StateStartTime > SlasherParameter.MAX_CATCHING_TIME)
		{
			if (base.SqrDistanceFromPlayer < mAttackRadiusSqr[1])
			{
				StartMiddleAttack();
			}
			else
			{
				StartRangeAttack();
			}
		}
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
		PlaySound(AudioName.CERBERLISK_DEAD);
	}

	public override void StartCatching()
	{
		base.StartCatching();
		PlaySoundLoop(AudioName.CERBERLISK_RUN);
	}

	public override void EndCatching()
	{
		base.EndCatching();
		StopSound(AudioName.CERBERLISK_RUN);
	}

	public override void StartGotHit()
	{
		base.StartGotHit();
		PlaySound(AudioName.CERBERLISK_ATTACKED);
	}

	private void StartMiddleAttack()
	{
		switch (GameWorld.GetRandomIndex(mMiddleAttackChances[(int)mMindState]))
		{
		case 0:
			SetState(Enemy.CATCHING_STATE);
			break;
		case 1:
			SetState(CERBERLISK_THREE_ATTACK_PRE);
			break;
		case 2:
			if (mMindState == MindState.NORMAL)
			{
				SetState(CERBERLISK_THREE_FIRE_BALL);
			}
			else
			{
				SetState(CERBERLISK_BREATHE_FIRE_BALL_STATE);
			}
			break;
		case 3:
			SetState(CERBERLISK_BIG_FIRE_BALL);
			break;
		case 4:
			if (mMindState == MindState.NORMAL)
			{
				SetState(CERBERLISK_BREATHE_IN_STATE);
			}
			else
			{
				SetState(CERBERLISK_CYCLONE_STATE);
			}
			break;
		default:
			Debug.LogError("dddd");
			break;
		}
		SendStateChangeRequest(mConditionState);
	}

	private void StartRangeAttack()
	{
		switch (GameWorld.GetRandomIndex(mRangeAttackChances[(int)mMindState]))
		{
		case 0:
			SetState(CERBERLISK_DASH_ATTACK);
			break;
		case 1:
			SetState(CERBERLISK_PUSH_STATE);
			break;
		default:
			Debug.LogError("dddd");
			break;
		}
		SendStateChangeRequest(mConditionState);
	}

	public float GetBreathInSpeed()
	{
		float num = CerberliskParameter.BREATHE_IN_SPEED;
		if (IsHardMode())
		{
			num += 2f;
		}
		return num;
	}

	public float GetPushSpeed()
	{
		float num = CerberliskParameter.PUSH_SPEED;
		if (IsHardMode())
		{
			num += 2f;
		}
		return num;
	}
}
