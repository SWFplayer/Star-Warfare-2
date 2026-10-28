using System.Collections.Generic;
using UnityEngine;

public class FrostStone : EnemyBoss
{
	public class Skill : BossSkill
	{
		public CreatureMeleeAttackSkill MeleeAttackSkill1;

		public CreatureMeleeAttackSkill MeleeAttackSkill2;

		public CreatureMeleeAttackSkill DashAttackSkill;

		public CreatureMeleeAttackSkill RandomDashAttackSkill;

		public CreatureMeleeAttackSkill JumpAttackSkill;

		public void Init(GameObject go)
		{
			Collider collider = go.transform.Find(BoneName.FROSTSTONE_HEAD).gameObject.GetComponent<Collider>();
			Collider collider2 = go.transform.Find(BoneName.FROSTSTONE_RIGHT_HAND).gameObject.GetComponent<Collider>();
			Collider collider3 = go.transform.Find(BoneName.FROSTSTONE_RIGHT_ARM).gameObject.GetComponent<Collider>();
			MeleeAttackSkill1 = new CreatureMeleeAttackSkill();
			MeleeAttackSkill1.CheckTime = FrostStoneParameter.MELEE_ATTACK_CHECK_TIME1;
			MeleeAttackSkill1.SetDamageInfo(FrostStoneParameter.GetInstance(), 1);
			MeleeAttackSkill1.Colliders = new Collider[1];
			MeleeAttackSkill1.Colliders[0] = collider;
			MeleeAttackSkill1.KnockDownSpeed = FrostStoneParameter.MELEE_ATTACK_KNOCK_SPEED1;
			MeleeAttackSkill2 = new CreatureMeleeAttackSkill();
			MeleeAttackSkill2.CheckTime = FrostStoneParameter.MELEE_ATTACK_CHECK_TIME2;
			MeleeAttackSkill2.SetDamageInfo(FrostStoneParameter.GetInstance(), 2);
			MeleeAttackSkill2.Colliders = new Collider[2];
			MeleeAttackSkill2.Colliders[0] = collider2;
			MeleeAttackSkill2.Colliders[1] = collider3;
			MeleeAttackSkill2.KnockDownSpeed = FrostStoneParameter.MELEE_ATTACK_KNOCK_SPEED2;
			DashAttackSkill = new CreatureMeleeAttackSkill();
			DashAttackSkill.CheckTime = new Vector2(0f, 1f);
			DashAttackSkill.SetDamageInfo(FrostStoneParameter.GetInstance(), 7);
			DashAttackSkill.HitRadius = FrostStoneParameter.DASH_ATTACK.DASH_HIT_RADIUS;
			DashAttackSkill.KnockDownSpeed = FrostStoneParameter.DASH_ATTACK.DASH_KNOCK_SPEED;
			RandomDashAttackSkill = new CreatureMeleeAttackSkill();
			RandomDashAttackSkill.CheckTime = new Vector2(0f, 1f);
			RandomDashAttackSkill.SetDamageInfo(FrostStoneParameter.GetInstance(), 7);
			RandomDashAttackSkill.HitRadius = FrostStoneParameter.DASH_ATTACK.DASH_HIT_RADIUS;
			RandomDashAttackSkill.KnockDownSpeed = FrostStoneParameter.DASH_ATTACK.DASH_KNOCK_SPEED;
			JumpAttackSkill = new CreatureMeleeAttackSkill();
			JumpAttackSkill.CheckTime = new Vector2(0f, 1f);
			JumpAttackSkill.SetDamageInfo(FrostStoneParameter.GetInstance(), 9);
			JumpAttackSkill.HitRadius = FrostStoneParameter.JUMP_ATTACK_HIT_RADIUS;
			JumpAttackSkill.Radius = 0f;
			JumpAttackSkill.KnockDownSpeed = FrostStoneParameter.JUMP_ATTACK_KNOCK_SPEED;
			MoveAudioName = AudioName.FROSTSTONE_RUN;
		}
	}

	public static EnemyState FROST_STONE_BORN_STATE = new FrostStoneBornState();

	public static EnemyState FROST_STONE_MELEE_ATTACK_STATE01 = new FrostStoneMeleeAttackState1();

	public static EnemyState FROST_STONE_MELEE_ATTACK_STATE02 = new FrostStoneMeleeAttackState2();

	public static EnemyState FROST_STONE_JUMP_ATTACK_STATE01 = new EnemyPlayAnimState
	{
		AnimName = AnimationString.ENEMY_JUMPSTART,
		IsLookAtTarget = true,
		Sound = AudioName.FROSTSTONE_JUMP01
	};

	public static EnemyState FROST_STONE_JUMP_ATTACK_STATE02 = new FrostStoneJumpAttack2();

	public static EnemyState FROST_STONE_JUMP_ATTACK_STATE03 = new FrostStoneJumpAttack3();

	public static EnemyState FROST_STONE_DASH_ATTACK_STATE = new FrostStoneDashAttackState();

	public static EnemyState FROST_STONE_RANDOM_DASH_ATTACK_STATE01 = new EnemyPlayAnimState
	{
		AnimName = "random",
		IsLookAtTarget = true,
		Sound = AudioName.FROSTSTONE_RANDOM,
		SoundTime = 0.2f
	};

	public static EnemyState FROST_STONE_RANDOM_DASH_ATTACK_STATE02 = new FrostStoneRandomDashState();

	public static EnemyState FROST_STONE_RANDOM_DASH_ATTACK_STATE03 = new EnemyPlayAnimState
	{
		AnimName = AnimationString.BOSS_DASH03,
		IsLookAtTarget = true
	};

	public static EnemyState FROST_STONE_SMALL_SNOW_BALL_ATTACK_STATE = new FrostStoneSmallSnowBallAttackState();

	public static EnemyState FROST_STONE_BIG_SNOW_BALL_ATTACK_STATE = new FrostStoneBigSnowBallAttack();

	public static EnemyState FROST_STONE_ICE_SPIKE_ATTACK_STATE = new FrostStoneIceSpikeState();

	public static EnemyState FROST_STONE_ARMOR_UP_STATE = new FrostStoneArmorUpState();

	public static EnemyState FROST_STONE_DASH_MOVE_STATE = new FrostStoneDashMoveState();

	public Skill mSkill = new Skill();

	public FrostStoneWallCheckScript mWallCheck;

	public bool HasIceArmor;

	public float mRemoveArmorTime;

	public float StartArmorHp;

	public float CurrentRandomDashHitNum;

	private float mDestructbleDistanceSqr;

	private float[] mAttackRadiusSqr = new float[2];

	private float[][] mMeleeAttackChances = new float[3][];

	private float[][] mMiddleAttackChances = new float[3][];

	private float[][] mRangeAttackChances = new float[3][];

	private List<GameObject> mArmorList = new List<GameObject>();

	public FrostStone()
	{
		base.GameUnitType = EGameUnitType.ENEMY;
		base.EnemyType = EnemyType.FROST_STONE;
		IsNewState = true;
	}

	public override void Init(GameObject gObject)
	{
		base.Init(gObject);
		attackRange = FrostStoneParameter.MELEE_ATTACK_RADIUS;
		mAttackRadiusSqr[0] = FrostStoneParameter.MELEE_ATTACK_RADIUS * FrostStoneParameter.MELEE_ATTACK_RADIUS;
		mAttackRadiusSqr[1] = FrostStoneParameter.RANGE_ATTACK_RADIUS * FrostStoneParameter.RANGE_ATTACK_RADIUS;
		RageSoundName = AudioName.FROSTSTONE_RAGE;
		bipObject = entityTransform.Find("Bip01").gameObject;
		mHpPercetageToRage = FrostStoneParameter.HP_PERCENTAGE_TO_RAGE;
		mHpPercentagePerHit = FrostStoneParameter.HP_PERCENTAGE_PER_HIT;
		mTurnSpeed = FrostStoneParameter.TURN_SPEED;
		mRunSpeed = FrostStoneParameter.RUN_SPEED[0];
		CreateShadow(1f);
		ConditionStateManager.GetInstance().InitConditionState(this);
		for (int i = 0; i < 3; i++)
		{
			mMeleeAttackChances[i] = new float[2]
			{
				FrostStoneParameter.MELEE_ATTACK_CHANCE1[i],
				FrostStoneParameter.MELEE_ATTACK_CHANCE2[i]
			};
			mMiddleAttackChances[i] = new float[6]
			{
				FrostStoneParameter.MIDDLE_ATTACK_CHANCE1[i],
				FrostStoneParameter.MIDDLE_ATTACK_CHANCE2[i],
				FrostStoneParameter.MIDDLE_ATTACK_CHANCE3[i],
				FrostStoneParameter.MIDDLE_ATTACK_CHANCE4[i],
				FrostStoneParameter.MIDDLE_ATTACK_CHANCE5[i],
				FrostStoneParameter.MIDDLE_ATTACK_CHANCE6[i]
			};
			mRangeAttackChances[i] = new float[2]
			{
				FrostStoneParameter.RANGE_ATTACK_CHANCE1[i],
				FrostStoneParameter.RANGE_ATTACK_CHANCE2[i]
			};
		}
		mBossTouch.Init(this, mLocalPlayer, FrostStoneParameter.GetInstance(), 0, FrostStoneParameter.TOUCH_RADIUS, FrostStoneParameter.TOUCH_KNOCK_SPEED);
		mBossDefend.Init(this);
		mSkill.Init(entityObject);
		CreateNavMeshAgent();
		mNavMeshAgent.radius = 4f;
		StepBack = FrostStoneParameter.STEP_BACK;
		DashAttackParameter = FrostStoneParameter.DASH_ATTACK;
		mWallCheck = entityObject.GetComponentInChildren<FrostStoneWallCheckScript>();
		mDestructbleDistanceSqr = FrostStoneParameter.DESTRUCTBLE_DISTANCE * FrostStoneParameter.DESTRUCTBLE_DISTANCE;
		GameObject gameObject = Pool.sAllocate("SW2_Effect/B3_Armorup_exist01");
		mArmorList.Add(gameObject);
		gameObject.transform.parent = entityObject.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1");
		gameObject = Pool.sAllocate("SW2_Effect/B3_Armorup_exist");
		mArmorList.Add(gameObject);
		gameObject.transform.parent = entityObject.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 Head");
		gameObject = Pool.sAllocate("SW2_Effect/B3_Armorup_exist_bingjia_L");
		mArmorList.Add(gameObject);
		gameObject.transform.parent = entityObject.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 L Clavicle/Bip01 L UpperArm/Bip01 L Forearm/Bip01 L Hand");
		gameObject = Pool.sAllocate("SW2_Effect/B3_Armorup_exist_bingjia_R");
		mArmorList.Add(gameObject);
		gameObject.transform.parent = entityObject.transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand");
		foreach (GameObject mArmor in mArmorList)
		{
			mArmor.transform.localPosition = Vector3.zero;
			mArmor.transform.localRotation = Quaternion.identity;
			mArmor.SetActive(false);
		}
	}

	protected override void MakeDecisionInEnemyIdle()
	{
		if (mTarget == null)
		{
			ChooseTargetPlayer(true);
		}
		if (mTarget == null || !(Time.time - StateStartTime > FrostStoneParameter.IDLE_TIME[(int)mMindState]))
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
		else if (mMindState >= MindState.RAGE1 && !HasIceArmor && Time.time - mRemoveArmorTime > FrostStoneParameter.ICE_ARMOR_REFRESH_ARMOR_TIME)
		{
			SetState(FROST_STONE_ARMOR_UP_STATE);
			SendStateChangeRequest(mConditionState);
			Debug.Log("装冰甲");
		}
		else if (base.SqrDistanceFromPlayer < mAttackRadiusSqr[0])
		{
			switch (GameWorld.GetRandomIndex(mMeleeAttackChances[(int)mMindState]))
			{
			case 0:
				SetState(FROST_STONE_MELEE_ATTACK_STATE01);
				break;
			case 1:
				SetState(FROST_STONE_MELEE_ATTACK_STATE02);
				break;
			default:
				Debug.LogError("..");
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

	public override void OnHitTarget(GameUnit target, CreatureSkill skill, int Count)
	{
		base.OnHitTarget(target, skill, Count);
		if (target.GameUnitType == EGameUnitType.LOCAL_PLAYER)
		{
			if (skill == mSkill.MeleeAttackSkill1 || skill == mSkill.MeleeAttackSkill2 || skill == mSkill.DashAttackSkill)
			{
				mMeleeAttackHasHitPlayer++;
			}
			else if (skill == mSkill.RandomDashAttackSkill)
			{
				CurrentRandomDashHitNum++;
			}
		}
	}

	protected override void MakeDecisionInCatching()
	{
		if (base.SqrDistanceFromPlayer < mAttackRadiusSqr[0])
		{
			switch (GameWorld.GetRandomIndex(mMeleeAttackChances[(int)mMindState]))
			{
			case 0:
				SetState(FROST_STONE_MELEE_ATTACK_STATE01);
				break;
			case 1:
				SetState(FROST_STONE_MELEE_ATTACK_STATE02);
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
		PlaySound(AudioName.FROSTSTONE_DEAD);
		if (mArmorList[0].activeSelf)
		{
			SetArmor(false);
		}
	}

	public override void ChangeToHitState()
	{
		if (!HasIceArmor && mConditionState.mState != FROST_STONE_JUMP_ATTACK_STATE01 && mConditionState.mState != FROST_STONE_JUMP_ATTACK_STATE02 && mConditionState.mState != FROST_STONE_JUMP_ATTACK_STATE03)
		{
			base.ChangeToHitState();
		}
	}

	public override void DoLogic(float deltaTime)
	{
		base.DoLogic(deltaTime);
		if (HasIceArmor && base.IsMasterPlayer && StartArmorHp - (float)hp > FrostStoneParameter.ICE_ARMOR_DROP_HP * (float)maxHp && mConditionState.mState != FROST_STONE_JUMP_ATTACK_STATE01 && mConditionState.mState != FROST_STONE_JUMP_ATTACK_STATE02 && mConditionState.mState != FROST_STONE_JUMP_ATTACK_STATE03)
		{
			HasIceArmor = false;
			SetState(Enemy.GOTHIT_STATE);
			SendStateChangeRequest(mConditionState);
			Debug.Log("移除冰甲");
			mRemoveArmorTime = Time.time;
			SetArmor(false);
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				FrostStoneRemoveIceArmorRequest request = new FrostStoneRemoveIceArmorRequest(base.EnemyID);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
		Dictionary<string, DestructibleObjectScript> destructibleDic = GameApp.GetInstance().GetGameWorld().GetDestructibleDic();
		foreach (DestructibleObjectScript value in destructibleDic.Values)
		{
			if (value != null && Vector3.SqrMagnitude(GetPosition() - value.transform.position) < mDestructbleDistanceSqr)
			{
				value.OnHit(9999999, false);
				break;
			}
		}
	}

	public void SetArmor(bool show)
	{
		foreach (GameObject mArmor in mArmorList)
		{
			mArmor.SetActive(show);
		}
		if (!show)
		{
			GameObject gameObject = Pool.sAllocate("SW2_Effect/B3_Armorup_break01", mArmorList[0].transform.position, 5f);
			gameObject.transform.rotation = mArmorList[0].transform.rotation;
			gameObject = Pool.sAllocate("SW2_Effect/B3_Armorup_break", mArmorList[1].transform.position, 5f);
			gameObject.transform.rotation = mArmorList[1].transform.rotation;
			gameObject = Pool.sAllocate("SW2_Effect/B3_Armorup_break_L", mArmorList[2].transform.position, 5f);
			gameObject.transform.rotation = mArmorList[2].transform.rotation;
			gameObject = Pool.sAllocate("SW2_Effect/B3_Armorup_break_R", mArmorList[3].transform.position, 5f);
			gameObject.transform.rotation = mArmorList[3].transform.rotation;
		}
	}

	public override void StartCatching()
	{
		base.StartCatching();
		PlaySoundLoop(AudioName.FROSTSTONE_RUN);
	}

	public override void EndCatching()
	{
		base.EndCatching();
		StopSound(AudioName.FROSTSTONE_RUN);
	}

	public override void StartGotHit()
	{
		base.StartGotHit();
		PlaySound(AudioName.FROSTSTONE_ATTACKED);
	}

	public override void CoopCastSkill()
	{
		int num = Random.Range(1, 3);
		SetState("CoopSkill" + num);
	}

	public override void StartCoopRage()
	{
		SetState("CoopRageState");
	}

	private void StartMiddleAttack()
	{
		switch (GameWorld.GetRandomIndex(mMiddleAttackChances[(int)mMindState]))
		{
		case 0:
			SetState(Enemy.CATCHING_STATE);
			break;
		case 1:
			SetState(FROST_STONE_SMALL_SNOW_BALL_ATTACK_STATE);
			break;
		case 2:
			SetState(FROST_STONE_ICE_SPIKE_ATTACK_STATE);
			break;
		case 3:
			SetState(FROST_STONE_JUMP_ATTACK_STATE01);
			break;
		case 4:
			SetState(FROST_STONE_RANDOM_DASH_ATTACK_STATE01);
			break;
		case 5:
			SetState(FROST_STONE_BIG_SNOW_BALL_ATTACK_STATE);
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
			SetState(FROST_STONE_DASH_ATTACK_STATE);
			break;
		case 1:
			SetState(FROST_STONE_BIG_SNOW_BALL_ATTACK_STATE);
			break;
		default:
			Debug.LogError("dddd");
			break;
		}
		SendStateChangeRequest(mConditionState);
	}
}
