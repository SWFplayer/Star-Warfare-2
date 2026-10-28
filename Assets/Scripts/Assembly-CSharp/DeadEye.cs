using UnityEngine;

public class DeadEye : EnemyBoss
{
	public class Skill : BossSkill
	{
		public CreatureMeleeAttackSkill MeleeAttackSkill;

		public CreatureMeleeAttackSkill MeleeAttackSkill1;

		public CreatureMeleeAttackSkill LeapAttackSkill;

		public CreatureMeleeAttackSkill DashAttackSkill;

		public CreatureMeleeAttackSkill DownAttackSkill;

		public void Init(GameObject go)
		{
			Collider collider = go.transform.Find(BoneName.DEADEYE_LEFT_ARM).gameObject.GetComponent<Collider>();
			Collider collider2 = go.transform.Find(BoneName.DEADEYE_RIGHT_ARM).gameObject.GetComponent<Collider>();
			MeleeAttackSkill = new CreatureMeleeAttackSkill();
			MeleeAttackSkill.CheckTime = DeadEyeParameter.MELEE_ATTACK_CHECK_TIME1;
			MeleeAttackSkill.SetDamageInfo(DeadEyeParameter.GetInstance(), 1);
			MeleeAttackSkill.Colliders = new Collider[1];
			MeleeAttackSkill.Colliders[0] = collider;
			MeleeAttackSkill.KnockDownSpeed = DeadEyeParameter.MELEE_ATTACK_KNOCK_SPEED1;
			MeleeAttackSkill1 = new CreatureMeleeAttackSkill();
			MeleeAttackSkill1.CheckTime = DeadEyeParameter.MELEE_ATTACK_CHECK_TIME2;
			MeleeAttackSkill1.SetDamageInfo(DeadEyeParameter.GetInstance(), 2);
			MeleeAttackSkill1.Colliders = new Collider[2];
			MeleeAttackSkill1.Colliders[0] = collider;
			MeleeAttackSkill1.Colliders[1] = collider2;
			MeleeAttackSkill1.Interval = DeadEyeParameter.MELEE_ATTACK_2_INTERVAL;
			MeleeAttackSkill1.KnockDownSpeed = DeadEyeParameter.MELEE_ATTACK_KNOCK_SPEED2;
			MeleeAttackSkill1.KnockDownCheckTime = DeadEyeParameter.MELEE_ATTACK_KNOCK_CHECK_TIME2;
			LeapAttackSkill = new CreatureMeleeAttackSkill();
			LeapAttackSkill.CheckTime = DeadEyeParameter.LEAP_ATTACK_CHECK_TIME;
			LeapAttackSkill.SetDamageInfo(DeadEyeParameter.GetInstance(), 8);
			LeapAttackSkill.HitRadius = DeadEyeParameter.LEAP_CHECK_RADIUS;
			LeapAttackSkill.Radius = DeadEyeParameter.LEAP_SECTOR_RADIUS;
			LeapAttackSkill.Angle = DeadEyeParameter.LEAP_SECTOR_ANGLE;
			LeapAttackSkill.KnockDownSpeed = DeadEyeParameter.LEAP_KNOCK_SPEED;
			DashAttackSkill = new CreatureMeleeAttackSkill();
			DashAttackSkill.CheckTime = new Vector2(0f, 1f);
			DashAttackSkill.SetDamageInfo(DeadEyeParameter.GetInstance(), 7);
			DashAttackSkill.HitRadius = DeadEyeParameter.DASH_ATTACK.DASH_HIT_RADIUS;
			DashAttackSkill.Radius = DeadEyeParameter.DASH_ATTACK.DASH_SECTOR_RADIUS;
			DashAttackSkill.Angle = DeadEyeParameter.DASH_ATTACK.DASH_SECTOR_ANGLE;
			DashAttackSkill.KnockDownSpeed = DeadEyeParameter.DASH_ATTACK.DASH_KNOCK_SPEED;
			DownAttackSkill = new CreatureMeleeAttackSkill();
			DownAttackSkill.CheckTime = new Vector2(0f, 1f);
			DownAttackSkill.SetDamageInfo(DeadEyeParameter.GetInstance(), 13);
			DownAttackSkill.HitRadius = DeadEyeParameter.DEAD_EYE_DOWN_ATTACK_HIT_RADIUS;
			DownAttackSkill.Radius = 0f;
			DownAttackSkill.KnockDownSpeed = DeadEyeParameter.DEAD_EYE_DOWN_ATTACK_KNOCK_DOWN_SPEED;
			MoveAudioName = AudioName.DEADEYE_RUN;
		}
	}

	public static EnemyState DEAD_EYE_BORN_STATE = new DeadEyeBornState();

	public static EnemyState DEAD_EYE_MELEE_ATTACK01_STATE = new DeadEyeMeleeAttack1();

	public static EnemyState DEAD_EYE_MELEE_ATTACK02_STATE = new DeadEyeMeleeAttack3State();

	public static EnemyState DEAD_EYE_X_RAY_STATE = new DeadEyeXLaserState();

	public static EnemyState DEAD_EYE_4_RAY_STATE = new DeadEye4LaserState();

	public static EnemyState DEAD_EYE_SHOULDER_RAY_STATE = new DeadEyeShoulderLaserState();

	public static EnemyState DEAD_EYE_LEAP_ATTACK_STATE = new DeadEyeLeapAttackState();

	public static EnemyState DEAD_EYE_DASH01_ATTACK_STATE = new DeadEyeRushAttackState1();

	public static EnemyState DEAD_EYE_DASH02_ATTACK_STATE = new DeadEyeRushAttackState2();

	public static EnemyState DEAD_EYE_DASH03_ATTACK_STATE = new DeadEyeRushAttackState3();

	public static EnemyState DEAD_EYE_ROCK_ATTACK_STATE = new DeadEyeRockAttackState();

	public static EnemyState DEAD_EYE_DASH_UP_STATE = new DeadEyeDashUpState();

	public static EnemyState DEAD_EYE_UP_STATE = new DeadEyeUpState();

	public static EnemyState DEAD_EYE_ROOF_ROCK_ATTACK_STATE = new DeadEyeRoofRockAttackState();

	public static EnemyState DEAD_EYE_ROOF_IDLE_STATE = new DeadEyeRoofIdleState();

	public static EnemyState DEAD_EYE_ROOF_X_RAY_STATE = new DeadEyeRoofXLaserState();

	public static EnemyState DEAD_EYE_ROOF_4_RAY_STATE = new DeadEyeRoof4LaserState();

	public static EnemyState DEAD_EYE_ROOF_CONTINUE_RAY_STATE = new DeadEyeRoofContinueLaserState();

	public static EnemyState DEAD_EYE_ROOF_NET_RAY_STATE = new DeadEyeRoofNetLaserState();

	public static EnemyState DEAD_EYE_ROOF_CATCH_STATE = new DeadEyeRoofCatchState();

	public static EnemyState DEAD_EYE_ROOF_MOVE_BACK_STATE = new DeadEyeRoofMoveBackState();

	public static EnemyState DEAD_EYE_ROOF_DOWN_STATE = new DeadEyeDownState();

	public static EnemyState DEAD_EYE_ROOF_FALL_STATE = new DeadEyeRoofFallState();

	public static EnemyState DEAD_EYE_FALL_RECOVER_STATE = new EnemyPlayAnimState
	{
		AnimName = AnimationString.DEAD_EYE_ROOF_FALL_RECOVER
	};

	public static EnemyState DEAD_EYE_ROOF_DEAD_STATE = new DeadEyeRoofDeadState();

	public static EnemyState DEAD_EYE_DASH_MOVE_STATE = new DeadEyeDashMoveState();

	public Skill mSkill = new Skill();

	public Vector3 DashTarget;

	public float GroundRoofStartTime;

	private float[][] mMeleeAttackChances = new float[3][];

	private float[][] mMiddleAttackChances = new float[3][];

	private float[][] mRangeAttackChances = new float[3][];

	private float[][] mRoofAttackChances = new float[3][];

	private float[][] mCoopRangeAttackChances = new float[3][];

	private float[] mAttackRadius = new float[2];

	private GameObject[] mInitsLasers = new GameObject[2];

	public float ShadowLocalY;

	public float ShadowScale;

	public DeadEye()
	{
		base.GameUnitType = EGameUnitType.ENEMY;
		base.EnemyType = EnemyType.DEAD_EYE;
		IsNewState = true;
	}

	public override void Init(GameObject gObject)
	{
		base.Init(gObject);
		attackRange = DeadEyeParameter.MELEE_ATTACK_RADIUS;
		RageSoundName = AudioName.DEADEYE_RAGE;
		bipObject = entityTransform.Find("Bone01").gameObject;
		mHpPercetageToRage = DeadEyeParameter.HP_PERCENTAGE_TO_RAGE;
		mHpPercentagePerHit = DeadEyeParameter.HP_PERCENTAGE_PER_HIT;
		mTurnSpeed = DeadEyeParameter.TURN_SPEED;
		mRunSpeed = DeadEyeParameter.RUN_SPEED[0];
		CreateShadow(1f);
		ConditionStateManager.GetInstance().InitConditionState(this);
		for (int i = 0; i < 3; i++)
		{
			mMeleeAttackChances[i] = new float[2]
			{
				DeadEyeParameter.MELEE_ATTACK_CHANCE1[i],
				DeadEyeParameter.MELEE_ATTACK_CHANCE2[i]
			};
			mMiddleAttackChances[i] = new float[5]
			{
				DeadEyeParameter.MIDDLE_ATTACK_CHANCE1[i],
				DeadEyeParameter.MIDDLE_ATTACK_CHANCE2[i],
				DeadEyeParameter.MIDDLE_ATTACK_CHANCE3[i],
				DeadEyeParameter.MIDDLE_ATTACK_CHANCE4[i],
				DeadEyeParameter.MIDDLE_ATTACK_CHANCE5[i]
			};
			mRangeAttackChances[i] = new float[2]
			{
				DeadEyeParameter.RANGE_ATTACK_CHANCE1[i],
				DeadEyeParameter.RANGE_ATTACK_CHANCE2[i]
			};
			mRoofAttackChances[i] = new float[5]
			{
				DeadEyeParameter.ROOF_ATTACK_CHANCE1[i],
				DeadEyeParameter.ROOF_ATTACK_CHANCE2[i],
				DeadEyeParameter.ROOF_ATTACK_CHANCE3[i],
				DeadEyeParameter.ROOF_ATTACK_CHANCE4[i],
				DeadEyeParameter.ROOF_ATTACK_CHANCE5[i]
			};
			mCoopRangeAttackChances[i] = new float[3]
			{
				DeadEyeParameter.MIDDLE_ATTACK_CHANCE2[i],
				DeadEyeParameter.MIDDLE_ATTACK_CHANCE3[i],
				DeadEyeParameter.MIDDLE_ATTACK_CHANCE4[i]
			};
		}
		mAttackRadius[0] = DeadEyeParameter.MELEE_ATTACK_RADIUS;
		mAttackRadius[1] = DeadEyeParameter.RANGE_ATTACK_RADIUS;
		mSkill.Init(entityObject);
		mBossTouch.Init(this, mLocalPlayer, DeadEyeParameter.GetInstance(), 0, DeadEyeParameter.TOUCH_RADIUS, DeadEyeParameter.TOUCH_KNOCK_SPEED);
		mBossDefend.Init(this);
		StepBack = DeadEyeParameter.STEP_BACK;
		DashAttackParameter = DeadEyeParameter.DASH_ATTACK;
		ShadowLocalY = Shadow.transform.localPosition.y;
		ShadowScale = Shadow.transform.localScale.x;
	}

	protected override void MakeDecisionInEnemyIdle()
	{
		if (mTarget == null)
		{
			ChooseTargetPlayer(true);
		}
		if (mTarget == null || !(Time.time - StateStartTime > DeadEyeParameter.IDLE_TIME[(int)mMindState]))
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
			return;
		}
		if (!IsInBossCoop && (float)hp < (float)maxHp * DeadEyeParameter.CHANGE_TO_ROOF_HP && Time.time - GroundRoofStartTime > DeadEyeParameter.CHANGE_TO_ROOF_INTERVAL)
		{
			SetState("DashUp");
			SendStateChangeRequest(mConditionState);
			return;
		}
		if ((!IsHardMode() && mMeleeAttackHasHitPlayer >= StepBack.TRIGGER_STEP_BACK_MELEE_ATTACK_HIT_NUM) || MeleeAttackHasUsedNum >= StepBack.TRIGGER_STEP_BACK_MELEE_ATTACK_NUM)
		{
			mMeleeAttackHasHitPlayer = 0;
			MeleeAttackHasUsedNum = 0;
			SetState(EnemyBoss.BOSS_STEP_BACK_STATE);
			SendStateChangeRequest(mConditionState);
			return;
		}
		float sqrMagnitude = (mTargetPosition - entityTransform.position).sqrMagnitude;
		if (sqrMagnitude < mAttackRadius[0] * mAttackRadius[0])
		{
			switch (GetAttackIndex(0))
			{
			case 0:
				SetState(DEAD_EYE_MELEE_ATTACK01_STATE);
				break;
			case 1:
				SetState(DEAD_EYE_MELEE_ATTACK02_STATE);
				break;
			default:
				Debug.LogError("dd");
				break;
			}
			SendStateChangeRequest(mConditionState);
		}
		else if (sqrMagnitude < mAttackRadius[1] * mAttackRadius[1])
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
		if (base.SqrDistanceFromPlayer < mAttackRadius[0] * mAttackRadius[0])
		{
			switch (GetAttackIndex(0))
			{
			case 0:
				SetState(DEAD_EYE_MELEE_ATTACK01_STATE);
				break;
			case 1:
				SetState(DEAD_EYE_MELEE_ATTACK02_STATE);
				break;
			default:
				Debug.LogError("dd");
				break;
			}
			SendStateChangeRequest(mConditionState);
		}
		else if (Time.time - StateStartTime > SlasherParameter.MAX_CATCHING_TIME)
		{
			if (base.SqrDistanceFromPlayer < mAttackRadius[1] * mAttackRadius[1])
			{
				StartMiddleAttack();
			}
			else
			{
				StartRangeAttack();
			}
		}
	}

	public override bool HandleMessage(EConditionMessage message)
	{
		if (base.HandleMessage(message))
		{
			return true;
		}
		switch (message)
		{
		case EConditionMessage.ETryAttack2:
			return TryRoofAttack(true);
		case EConditionMessage.ETryRangeAttack2:
			return TryRoofAttack(false);
		case EConditionMessage.ETryRangeAttack:
		{
			if (mTarget == null)
			{
				ChooseTargetPlayer(true);
			}
			if (mTarget == null)
			{
				return false;
			}
			GameUnit randomTarget2 = GetRandomTarget();
			ChangeTarget(randomTarget2);
			if (GameApp.GetInstance().GetGameMode().IsCoopMode())
			{
				EnemyChangeTargetRequest request2 = new EnemyChangeTargetRequest(base.EnemyID, (randomTarget2 as Player).GetChannelID(), entityTransform.position);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
			}
			switch (GameWorld.GetRandomIndex(mCoopRangeAttackChances[(int)mMindState]))
			{
			case 0:
				SetState(DEAD_EYE_X_RAY_STATE);
				break;
			case 1:
				SetState(DEAD_EYE_4_RAY_STATE);
				break;
			case 2:
				SetState(DEAD_EYE_SHOULDER_RAY_STATE);
				break;
			case 3:
				SetState(DEAD_EYE_ROCK_ATTACK_STATE);
				break;
			default:
				Debug.LogError("dd");
				break;
			}
			SendStateChangeRequest(mConditionState);
			return true;
		}
		case EConditionMessage.EMakeDecisionInCoopLongIdle:
			if (mTarget == null)
			{
				ChooseTargetPlayer(true);
			}
			if (mTarget == null)
			{
				return false;
			}
			if (Time.time - StateStartTime > DeadEyeParameter.IDLE_TIME[(int)mMindState] && mNextAction == ENextAction.CoopDead)
			{
				GameUnit randomTarget3 = GetRandomTarget();
				ChangeTarget(randomTarget3);
				if (GameApp.GetInstance().GetGameMode().IsCoopMode())
				{
					EnemyChangeTargetRequest request3 = new EnemyChangeTargetRequest(base.EnemyID, (randomTarget3 as Player).GetChannelID(), entityTransform.position);
					GameApp.GetInstance().GetNetworkManager().SendRequest(request3);
				}
				SetInDefend();
				SendStateChangeRequest(mConditionState);
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
				{
					BossCoopRequest bossCoopRequest2 = new BossCoopRequest(6, 4);
					bossCoopRequest2.Data.AddInt(base.EnemyID);
					GameApp.GetInstance().GetNetworkManager().SendRequest(bossCoopRequest2);
				}
				return true;
			}
			return false;
		case EConditionMessage.ETryCoopRangeAttack:
			if (mTarget == null)
			{
				ChooseTargetPlayer(true);
			}
			if (mTarget == null)
			{
				return false;
			}
			if (Time.time - StateStartTime > DeadEyeParameter.IDLE_TIME[(int)mMindState])
			{
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
				else
				{
					switch (GameWorld.GetRandomIndex(mCoopRangeAttackChances[(int)mMindState]))
					{
					case 0:
						SetState("CoopRange1");
						break;
					case 1:
						SetState("CoopRange2");
						break;
					case 2:
						SetState("CoopRange3");
						break;
					default:
						Debug.LogError("dd");
						break;
					}
					SendStateChangeRequest(mConditionState);
				}
				return true;
			}
			return false;
		default:
			return false;
		}
	}

	public override void OnHitTarget(GameUnit target, CreatureSkill skill, int Count)
	{
		base.OnHitTarget(target, skill, Count);
		if (target.GameUnitType == EGameUnitType.LOCAL_PLAYER && (skill == MeleeAttackSkill || skill == mSkill.MeleeAttackSkill1 || skill == mSkill.LeapAttackSkill || skill == mSkill.DashAttackSkill) && Count == 0)
		{
			mMeleeAttackHasHitPlayer++;
		}
	}

	public override void UpdateNavMeshInCatching(bool force)
	{
		if (!force && !(Time.time - mLastUpdateNavMeshTime > 3f / mRunSpeed))
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
				mNavMeshAgent.speed = DeadEyeParameter.RUN_SPEED[(int)mMindState];
				SetCanTurn(false);
			}
		}
	}

	public bool IsOnRoof()
	{
		return mConditionState.mState == DEAD_EYE_ROOF_ROCK_ATTACK_STATE || mConditionState.mState == DEAD_EYE_ROOF_IDLE_STATE || mConditionState.mState == DEAD_EYE_ROOF_X_RAY_STATE || mConditionState.mState == DEAD_EYE_ROOF_4_RAY_STATE || mConditionState.mState == DEAD_EYE_ROOF_CONTINUE_RAY_STATE || mConditionState.mState == DEAD_EYE_ROOF_NET_RAY_STATE || mConditionState.mState == DEAD_EYE_ROOF_CATCH_STATE || mConditionState.mState == DEAD_EYE_ROOF_DOWN_STATE || mConditionState.mState == DEAD_EYE_UP_STATE || mConditionState.mState == DEAD_EYE_ROOF_FALL_STATE || mConditionState.mState == DEAD_EYE_ROOF_MOVE_BACK_STATE;
	}

	public override void ChangeToHitState()
	{
		if (mConditionState.mState == DEAD_EYE_ROOF_DOWN_STATE || mConditionState.mState == DEAD_EYE_UP_STATE || mConditionState.mState == DEAD_EYE_ROOF_FALL_STATE)
		{
			return;
		}
		if (IsOnRoof())
		{
			if (Time.time - GroundRoofStartTime > DeadEyeParameter.ROOF_MIN_DURATION)
			{
				SetState("RoofFall");
			}
		}
		else
		{
			base.ChangeToHitState();
		}
	}

	public override void ChangeToDeadState()
	{
		if (IsOnRoof())
		{
			SetState("RoofDeadFall");
		}
		else
		{
			SetState(Enemy.DEAD_STATE);
		}
	}

	public override bool InPlayingState()
	{
		return base.InPlayingState() && mConditionState.mState != DEAD_EYE_ROOF_DEAD_STATE && mConditionState.mState != DEAD_EYE_ROOF_FALL_STATE;
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
		PlaySound(AudioName.DEADEYE_DEAD);
	}

	public override void DoLogic(float deltaTime)
	{
		base.DoLogic(deltaTime);
		entityTransform.eulerAngles = new Vector3(0f, entityTransform.eulerAngles.y, 0f);
	}

	private int GetAttackIndex(int type)
	{
		switch (type)
		{
		case 0:
			return GameWorld.GetRandomIndex(mMeleeAttackChances[(int)mMindState]);
		case 1:
			return GameWorld.GetRandomIndex(mMiddleAttackChances[(int)mMindState]);
		case 2:
			return GameWorld.GetRandomIndex(mRangeAttackChances[(int)mMindState]);
		case 3:
			return GameWorld.GetRandomIndex(mRoofAttackChances[(int)mMindState]);
		default:
			return -1;
		}
	}

	public bool TryRoofAttack(bool moveBack)
	{
		GameUnit randomTarget = GetRandomTarget();

        if (randomTarget == null)
        {
            return false;
        }

        ChangeTarget(randomTarget);
		if (GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			EnemyChangeTargetRequest request = new EnemyChangeTargetRequest(base.EnemyID, (randomTarget as Player).GetChannelID(), entityTransform.position);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
		if (Time.time - GroundRoofStartTime > DeadEyeParameter.CHANGE_TO_GROUND_INTERVAL)
		{
			SetState(DEAD_EYE_ROOF_CATCH_STATE);
			SendStateChangeRequest(mConditionState);
			return true;
		}
		int attackIndex = GetAttackIndex(3);
		if (moveBack && (attackIndex == 0 || attackIndex == 1 || attackIndex == 2 || attackIndex == 3) && Random.value <= DeadEyeParameter.ROOF_RUN_BACK_CHANCE && (mLastConditionState.mState == DEAD_EYE_ROOF_X_RAY_STATE || mLastConditionState.mState == DEAD_EYE_ROOF_4_RAY_STATE || mLastConditionState.mState == DEAD_EYE_ROOF_CONTINUE_RAY_STATE || mLastConditionState.mState == DEAD_EYE_ROOF_NET_RAY_STATE))
		{
			SetState(DEAD_EYE_ROOF_MOVE_BACK_STATE);
			SendStateChangeRequest(mConditionState);
			return true;
		}
		switch (attackIndex)
		{
		case 0:
			SetState(DEAD_EYE_ROOF_X_RAY_STATE);
			SendStateChangeRequest(mConditionState);
			return true;
		case 1:
			SetState(DEAD_EYE_ROOF_4_RAY_STATE);
			SendStateChangeRequest(mConditionState);
			return true;
		case 2:
			SetState(DEAD_EYE_ROOF_CONTINUE_RAY_STATE);
			SendStateChangeRequest(mConditionState);
			return true;
		case 3:
			SetState(DEAD_EYE_ROOF_NET_RAY_STATE);
			SendStateChangeRequest(mConditionState);
			return true;
		case 4:
			return false;
		default:
			return false;
		}
	}

	public override void CoopCastSkill()
	{
		int num = Random.Range(1, 4);
		SetState("CoopSkill" + num);
	}

	public override void StartCatching()
	{
		base.StartCatching();
		PlaySoundLoop(AudioName.DEADEYE_RUN);
	}

	public override void EndCatching()
	{
		base.EndCatching();
		StopSound(AudioName.DEADEYE_RUN);
	}

	public override void StartGotHit()
	{
		base.StartGotHit();
		PlaySound(AudioName.DEADEYE_ATTACKED);
	}

	private void StartMiddleAttack()
	{
		int attackIndex = GetAttackIndex(1);
		if ((attackIndex == 1 || attackIndex == 2 || attackIndex == 3) && Random.value <= DeadEyeParameter.CATCH_BETWEEN_LASER_ATTACK_CHANCE && (mLastConditionState.mState == DEAD_EYE_X_RAY_STATE || mLastConditionState.mState == DEAD_EYE_4_RAY_STATE || mLastConditionState.mState == DEAD_EYE_SHOULDER_RAY_STATE))
		{
			SetState("LaserCatch");
		}
		else
		{
			switch (attackIndex)
			{
			case 0:
				SetState("ContinueCatch");
				break;
			case 1:
				SetState(DEAD_EYE_X_RAY_STATE);
				break;
			case 2:
				SetState(DEAD_EYE_4_RAY_STATE);
				break;
			case 3:
				SetState(DEAD_EYE_SHOULDER_RAY_STATE);
				break;
			case 4:
				SetState(DEAD_EYE_ROCK_ATTACK_STATE);
				break;
			default:
				Debug.LogError("dd");
				break;
			}
		}
		SendStateChangeRequest(mConditionState);
	}

	private void StartRangeAttack()
	{
		switch (GetAttackIndex(2))
		{
		case 0:
			SetState(DEAD_EYE_DASH01_ATTACK_STATE);
			break;
		case 1:
			SetState(DEAD_EYE_LEAP_ATTACK_STATE);
			break;
		default:
			Debug.LogError("dd");
			break;
		}
		SendStateChangeRequest(mConditionState);
	}
}
