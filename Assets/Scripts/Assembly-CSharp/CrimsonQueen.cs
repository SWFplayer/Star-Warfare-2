using System.Collections.Generic;
using UnityEngine;

public class CrimsonQueen : EnemyBoss
{
	public class Skill : BossSkill
	{
		public CreatureMeleeAttackSkill MeleeAttackSkill1;

		public CreatureMeleeAttackSkill MeleeAttackSkill2;

		public CreatureMeleeAttackSkill DashAttackSkill;

		public void Init(GameObject go)
		{
			Collider collider = go.transform.Find(BoneName.CRIMSON_QUEEN_TAIL).gameObject.GetComponent<Collider>();
			Collider collider2 = go.transform.Find(BoneName.CRIMSON_QUEEN_LEFT_HAND).gameObject.GetComponent<Collider>();
			Collider collider3 = go.transform.Find(BoneName.CRIMSON_QUEEN_RIGHT_HAND).gameObject.GetComponent<Collider>();
			MeleeAttackSkill1 = new CreatureMeleeAttackSkill();
			MeleeAttackSkill1.CheckTime = CrimsonQueenParameter.MELEE_ATTACK_CHECK_TIME1;
			MeleeAttackSkill1.SetDamageInfo(CrimsonQueenParameter.GetInstance(), 1);
			MeleeAttackSkill1.Colliders = new Collider[2];
			MeleeAttackSkill1.Colliders[0] = collider2;
			MeleeAttackSkill1.Colliders[1] = collider3;
			MeleeAttackSkill1.KnockDownSpeed = CrimsonQueenParameter.MELEE_ATTACK_KNOCK_SPEED1;
			MeleeAttackSkill2 = new CreatureMeleeAttackSkill();
			MeleeAttackSkill2.CheckTime = CrimsonQueenParameter.MELEE_ATTACK_CHECK_TIME2;
			MeleeAttackSkill2.SetDamageInfo(CrimsonQueenParameter.GetInstance(), 2);
			MeleeAttackSkill2.Colliders = new Collider[1];
			MeleeAttackSkill2.Colliders[0] = collider;
			MeleeAttackSkill2.KnockDownSpeed = CrimsonQueenParameter.MELEE_ATTACK_KNOCK_SPEED2;
			DashAttackSkill = new CreatureMeleeAttackSkill();
			DashAttackSkill.CheckTime = new Vector2(0f, 1f);
			DashAttackSkill.SetDamageInfo(CrimsonQueenParameter.GetInstance(), 7);
			DashAttackSkill.HitRadius = CrimsonQueenParameter.DASH_ATTACK.DASH_HIT_RADIUS;
			DashAttackSkill.KnockDownSpeed = CrimsonQueenParameter.DASH_ATTACK.DASH_KNOCK_SPEED;
		}
	}

	public static EnemyState CRIMSON_QUEEN_BORN_STATE = new CrimsonQueenBornState();

	public static EnemyState CRIMSON_QUEEN_MELEE_ATTACK_STATE01 = new CrimsonQueenMeleeAttackState1();

	public static EnemyState CRIMSON_QUEEN_MELEE_ATTACK_STATE02 = new CrimsonQueenMeleeAttackState2();

	public static EnemyState CRIMSON_QUEEN_TAIL_LASER_STATE = new CrimsonQueenTailLaserState();

	public static EnemyState CRIMSON_QUEEN_FUNNEL_LASER_STATE = new CrimsonQueenFunnelLaserState();

	public static EnemyState CRIMSON_QUEEN_TRACKING_RPG_STATE = new CrimsonQueenTrackingRPGState();

	public static EnemyState CRIMSON_QUEEN_EXPLOSION_SPAWN_STATE = new CrimsonQueenExplosionSpawnState();

	public static EnemyState CRIMSON_QUEEN_DASH_ATTACK_STATE = new CrimsonQueenDashAttackState();

	public static CrimsonQueen Instance;

	public Skill mSkill = new Skill();

	private float[] mAttackRadiusSqr = new float[2];

	private float[][] mMeleeAttackChances = new float[3][];

	private float[][] mMiddleAttackChances = new float[3][];

	private float[][] mRangeAttackChances = new float[3][];

	public Transform LaserPoint;

	public Transform[] TrackingRPGPositions;

	private Transform[] mFunnelPositions;

	public float LastRPGTime;

	private List<Funnel> mFunnelList = new List<Funnel>();

	private List<Funnel> mAssistFunnelList = new List<Funnel>();

	private List<Funnel> mRPGList = new List<Funnel>();

	private Timer mFunnelAttackTimer = new Timer(CrimsonQueenParameter.ASSIST_FUNNEL_ATTACK_INTERVAL);

	private Vector3[] mOffsetList = new Vector3[6]
	{
		new Vector3(-1f, 1f, 0f),
		new Vector3(1f, 1f, 0f),
		new Vector3(-1f, 0f, 0f),
		new Vector3(1f, 0f, 0f),
		new Vector3(-1f, -1f, 0f),
		new Vector3(1f, -1f, 0f)
	};

	private Vector3 mDeadPosition;

	public CrimsonQueen()
	{
		base.GameUnitType = EGameUnitType.ENEMY;
		base.EnemyType = EnemyType.CRIMSON_QUEEN;
		IsNewState = true;
		Instance = this;
	}

	public override void Init(GameObject gObject)
	{
		base.Init(gObject);
		RageSoundName = AudioName.CRIMSONQUEEN_RAGE;
		bipObject = entityTransform.Find("Bip01").gameObject;
		attackRange = CrimsonQueenParameter.MELEE_ATTACK_RADIUS;
		mAttackRadiusSqr[0] = CrimsonQueenParameter.MELEE_ATTACK_RADIUS * CrimsonQueenParameter.MELEE_ATTACK_RADIUS;
		mAttackRadiusSqr[1] = CrimsonQueenParameter.RANGE_ATTACK_RADIUS * CrimsonQueenParameter.RANGE_ATTACK_RADIUS;
		mHpPercetageToRage = CrimsonQueenParameter.HP_PERCENTAGE_TO_RAGE;
		mHpPercentagePerHit = CrimsonQueenParameter.HP_PERCENTAGE_PER_HIT;
		mTurnSpeed = CrimsonQueenParameter.TURN_SPEED;
		mRunSpeed = CrimsonQueenParameter.RUN_SPEED[0];
		CreateShadow(1f);
		ConditionStateManager.GetInstance().InitConditionState(this);
		for (int i = 0; i < 3; i++)
		{
			mMeleeAttackChances[i] = new float[2]
			{
				CrimsonQueenParameter.MELEE_ATTACK_CHANCE1[i],
				CrimsonQueenParameter.MELEE_ATTACK_CHANCE2[i]
			};
			mMiddleAttackChances[i] = new float[5]
			{
				CrimsonQueenParameter.MIDDLE_ATTACK_CHANCE1[i],
				CrimsonQueenParameter.MIDDLE_ATTACK_CHANCE2[i],
				CrimsonQueenParameter.MIDDLE_ATTACK_CHANCE3[i],
				CrimsonQueenParameter.MIDDLE_ATTACK_CHANCE4[i],
				CrimsonQueenParameter.MIDDLE_ATTACK_CHANCE5[i]
			};
			mRangeAttackChances[i] = new float[2]
			{
				CrimsonQueenParameter.RANGE_ATTACK_CHANCE1[i],
				CrimsonQueenParameter.RANGE_ATTACK_CHANCE2[i]
			};
		}
		mBossTouch.Init(this, mLocalPlayer, CrimsonQueenParameter.GetInstance(), 0, CrimsonQueenParameter.TOUCH_RADIUS, CrimsonQueenParameter.TOUCH_KNOCK_SPEED);
		mBossDefend.Init(this);
		mSkill.Init(entityObject);
		CreateNavMeshAgent();
		mNavMeshAgent.radius = 2f;
		StepBack = CrimsonQueenParameter.STEP_BACK;
		DashAttackParameter = CrimsonQueenParameter.DASH_ATTACK;
		LaserPoint = entityTransform.Find(BoneName.CRIMSON_QUEEN_TAIL);
		GameObject original = Resources.Load("Enemy/L1/Boss/Weapon/bee") as GameObject;
		for (int j = 0; j < 10; j++)
		{
			GameObject gameObject = Object.Instantiate(original) as GameObject;
			gameObject.SetActive(false);
			Funnel funnel = new Funnel();
			funnel.SetObject(gameObject);
			mFunnelList.Add(funnel);
		}
		Pool.sGetInstance().PreAllocate("Enemy/L1/Boss/Weapon/TrackingRPG", 10);
		CrimsonQueenTrackingRPGPosition component = entityObject.GetComponent<CrimsonQueenTrackingRPGPosition>();
		TrackingRPGPositions = component.Positions;
		mFunnelPositions = component.FunnelPosition;
		for (int k = 0; k < mFunnelPositions.Length; k++)
		{
			GameObject gameObject2 = Object.Instantiate(original) as GameObject;
			gameObject2.SetActive(false);
			Funnel funnel2 = new Funnel();
			funnel2.SetObject(gameObject2);
			mAssistFunnelList.Add(funnel2);
		}
		Pool.sGetInstance().PreAllocate("Enemy/L1/Boss/Weapon/CqBomb");
		AudioManager.GetInstance().PlaySoundSingleLoop(AudioName.CRIMSONQUEEN_RUN, entityObject, entityTransform);
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
		PlaySound(AudioName.CRIMSONQUEEN_DEAD);
		Ray ray = new Ray(GetPosition() + Vector3.up, Vector3.down);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, CrimsonQueenParameter.EXPLOSION_SPAWN_MOVE_UP_HEIGHT + 3f, 1 << PhysicsLayer.FLOOR))
		{
			mDeadPosition = hitInfo.point + new Vector3(0f, 0.1f, 0f);
		}
		else
		{
			mDeadPosition = GetPosition();
			if (mTarget != null && mTarget.GetTransform() != null)
			{
				mDeadPosition.y = mTarget.GetPosition().y;
			}
			else
			{
				mDeadPosition.y -= CrimsonQueenParameter.EXPLOSION_SPAWN_MOVE_UP_HEIGHT;
			}
		}
		StopSound(AudioName.CRIMSONQUEEN_RUN);
	}

	public override void DoDead()
	{
		base.DoDead();
		entityObject.transform.position = Vector3.MoveTowards(entityObject.transform.position, mDeadPosition, 10f * Time.deltaTime);
	}

	protected override void MakeDecisionInEnemyIdle()
	{
		if (mTarget == null)
		{
			ChooseTargetPlayer(true);
		}
		if (mTarget == null || !(Time.time - StateStartTime > CrimsonQueenParameter.IDLE_TIME[(int)mMindState]))
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
		else if (base.SqrDistanceFromPlayer < mAttackRadiusSqr[0])
		{
			StartMeleeAttack();
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
			StartMeleeAttack();
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

	private void StartMeleeAttack()
	{
		switch (GameWorld.GetRandomIndex(mMeleeAttackChances[(int)mMindState]))
		{
		case 0:
			SetState(CRIMSON_QUEEN_MELEE_ATTACK_STATE01);
			break;
		case 1:
			SetState(CRIMSON_QUEEN_MELEE_ATTACK_STATE02);
			break;
		default:
			Debug.LogError("dd");
			break;
		}
		SendStateChangeRequest(mConditionState);
	}

	private void StartMiddleAttack()
	{
		int randomIndex = GameWorld.GetRandomIndex(mMiddleAttackChances[(int)mMindState]);
		bool flag = true;
		switch (randomIndex)
		{
		case 0:
			SetState(Enemy.CATCHING_STATE);
			break;
		case 1:
			SetState(CRIMSON_QUEEN_TAIL_LASER_STATE);
			break;
		case 2:
			SetState(CRIMSON_QUEEN_FUNNEL_LASER_STATE);
			break;
		case 3:
			if (Time.time - LastRPGTime > CrimsonQueenParameter.TRACKING_RPG_CD)
			{
				SetState(CRIMSON_QUEEN_TRACKING_RPG_STATE);
			}
			else
			{
				flag = false;
			}
			break;
		case 4:
			SetState(CRIMSON_QUEEN_EXPLOSION_SPAWN_STATE);
			break;
		default:
			Debug.LogError("dddd");
			break;
		}
		if (flag)
		{
			SendStateChangeRequest(mConditionState);
		}
	}

	private void StartRangeAttack()
	{
		switch (GameWorld.GetRandomIndex(mRangeAttackChances[(int)mMindState]))
		{
		case 0:
			SetState(CRIMSON_QUEEN_DASH_ATTACK_STATE);
			break;
		case 1:
			SetState(CRIMSON_QUEEN_TAIL_LASER_STATE);
			break;
		default:
			Debug.LogError("dddd");
			break;
		}
		SendStateChangeRequest(mConditionState);
	}

	public override void DoLogic(float deltaTime)
	{
		base.DoLogic(deltaTime);
		if (hp <= 0)
		{
			return;
		}
		foreach (Funnel mFunnel in mFunnelList)
		{
			mFunnel.Update();
		}
		foreach (Funnel mAssistFunnel in mAssistFunnelList)
		{
			mAssistFunnel.Update();
		}
		if (!base.IsMasterPlayer)
		{
			return;
		}
		if (mFunnelAttackTimer.Ready2())
		{
			FunnelAttack();
		}
		foreach (Funnel mAssistFunnel2 in mAssistFunnelList)
		{
			if (mAssistFunnel2.State == Funnel.EState.Dead && Time.time - mAssistFunnel2.DeathTime > CrimsonQueenParameter.ASSIST_FUNNEL_RESPAWN_TIME)
			{
				mAssistFunnel2.Respawn();
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
				{
					FunnelRequest request = new FunnelRequest(mAssistFunnel2.ID, EFunnelActionType.ERespawn);
					GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				}
			}
		}
	}

	public List<Funnel> GetFunnels(int num)
	{
		int num2 = 0;
		List<Funnel> list = new List<Funnel>();
		foreach (Funnel mFunnel in mFunnelList)
		{
			if (!mFunnel.Enable)
			{
				list.Add(mFunnel);
				num2++;
			}
			if (num2 >= num)
			{
				break;
			}
		}
		return list;
	}

	public void SpawnRPG(int num)
	{
		if (mTarget != null)
		{
			int num2 = mRPGList.Count + 10;
			for (int i = 0; i < num; i++)
			{
				GameObject gameObject = Pool.sGetInstance().Allocate("Enemy/L1/Boss/Weapon/TrackingRPG");
				CrimsonQueenTrackingRPG component = gameObject.GetComponent<CrimsonQueenTrackingRPG>();
				component.DoNotDestroy = true;
				component.SetTransform(TrackingRPGPositions[i]);
				component.damageType = EnemyDamageType.Explosion;
				component.attackDamage = CrimsonQueenParameter.GetInstance().GetDamage(5);
				component.areaDamage = CrimsonQueenParameter.GetInstance().GetDamage(5);
				component.explodeRadius = CrimsonQueenParameter.TRACKING_RPG_RADIUS;
				component.enemyType = mEnemyType;
				component.attacker = this;
				component.mTrackingPlayer = mTarget as Player;
				component.mSpeed = CrimsonQueenParameter.TRACKING_RPG_MOVE_SPEED;
				component.mOffset = mOffsetList[i];
				component.CheckFloor = false;
				Funnel funnel = new Funnel();
				funnel.SetObject(gameObject);
				funnel.ID = (byte)(mRPGList.Count + 10);
				funnel.InitTrackingRPG();
				mRPGList.Add(funnel);
			}
			if (base.IsMasterPlayer && GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				FunnelRequest funnelRequest = new FunnelRequest(0, EFunnelActionType.ESpawnTrackingRPG);
				funnelRequest.Data.AddByte((byte)num);
				funnelRequest.Data.AddByte((byte)num2);
				funnelRequest.Data.AddInt((GameApp.GetInstance().GetUserState().GetSelectedDifficultMode() != EDifficultMode.HEROIC) ? CrimsonQueenParameter.TRACKING_RPG_HP : CrimsonQueenParameter.TRACKING_RPG_HP_HERO);
				GameApp.GetInstance().GetNetworkManager().SendRequest(funnelRequest);
			}
		}
	}

	public void ClearTrackingRPGList()
	{
		mRPGList.Clear();
		Funnel.mTrackingRPGNum = 0;
	}

	public void SpawnAssistFunnel()
	{
		byte b = 0;
		Transform[] array = mFunnelPositions;
		foreach (Transform followTransform in array)
		{
			foreach (Funnel mAssistFunnel in mAssistFunnelList)
			{
				if (!mAssistFunnel.Enable)
				{
					mAssistFunnel.ID = b++;
					mAssistFunnel.InitAssistFunnel(followTransform, this);
					mAssistFunnel.GetObject().name = "AssistFunnel_" + b;
					break;
				}
			}
		}
		if (base.IsMasterPlayer && GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			FunnelRequest funnelRequest = new FunnelRequest(0, EFunnelActionType.ESpawn);
			funnelRequest.Data.AddByte((byte)mFunnelPositions.Length);
			funnelRequest.Data.AddInt((GameApp.GetInstance().GetUserState().GetSelectedDifficultMode() != EDifficultMode.HEROIC) ? CrimsonQueenParameter.ASSIST_FUNNEL_HP : CrimsonQueenParameter.ASSIST_FUNNEL_HP_HERO);
			GameApp.GetInstance().GetNetworkManager().SendRequest(funnelRequest);
		}
	}

	public Funnel GetFunnel(byte id)
	{
		foreach (Funnel mAssistFunnel in mAssistFunnelList)
		{
			if (mAssistFunnel.ID == id)
			{
				return mAssistFunnel;
			}
		}
		foreach (Funnel mRPG in mRPGList)
		{
			if (mRPG.ID == id)
			{
				return mRPG;
			}
		}
		Debug.LogError("Could not find Funnel with ID: " + id);
		return null;
	}

	public Funnel GetFunnel(string name)
	{
		foreach (Funnel mAssistFunnel in mAssistFunnelList)
		{
			if (mAssistFunnel.GetObject().name == name)
			{
				return mAssistFunnel;
			}
		}
		foreach (Funnel mRPG in mRPGList)
		{
			if (mRPG.GetObject().name == name)
			{
				return mRPG;
			}
		}
		Debug.LogError("Could not find Funnel with name: " + name);
		return null;
	}

	public List<Funnel> GetRpgList()
	{
		return mRPGList;
	}

	public void FunnelAttack()
	{
		if (mState == CRIMSON_QUEEN_EXPLOSION_SPAWN_STATE)
		{
			return;
		}
		bool flag = false;
		foreach (Funnel mAssistFunnel in mAssistFunnelList)
		{
			if (mAssistFunnel.Enable)
			{
				mAssistFunnel.Attack();
				flag = true;
			}
		}
		if (flag)
		{
			PlaySound(AudioName.DEADEYE_LASER_SHORT);
		}
		if (base.IsMasterPlayer && GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			FunnelRequest request = new FunnelRequest(0, EFunnelActionType.EFire);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
	}

	public override bool HandleMessage(EConditionMessage message)
	{
		if (base.HandleMessage(message))
		{
			return true;
		}
		if (message == EConditionMessage.ESpawnAssistFunnel)
		{
			SpawnAssistFunnel();
			return false;
		}
		return false;
	}

	public override void ChangeToDeadState()
	{
		base.ChangeToDeadState();
		foreach (Funnel mFunnel in mFunnelList)
		{
			if (mFunnel != null)
			{
				mFunnel.SetActive(false);
			}
		}
		foreach (Funnel mAssistFunnel in mAssistFunnelList)
		{
			if (mAssistFunnel != null)
			{
				mAssistFunnel.SetActive(false);
			}
		}
	}

	public override void StartGotHit()
	{
		base.StartGotHit();
		PlaySound(AudioName.CRIMSONQUEEN_ATTACKED);
	}

	public override void StartCatching()
	{
		base.StartCatching();
	}

	public override void EndCatching()
	{
		base.EndCatching();
	}

	public override void StartEnemyIdle()
	{
		base.StartEnemyIdle();
	}

	public override void EndEnemyIdle()
	{
		base.EndEnemyIdle();
	}

	public override Vector3 GetColliderCenterPosition()
	{
		return GetPosition() + Vector3.up * 6f;
	}
}
