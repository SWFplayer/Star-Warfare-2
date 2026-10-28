using System.Collections.Generic;
using UnityEngine;

public class Robot : Player
{
	public static GameUnitState RobotMoveIdleState = new RobotMoveIdleState();

	public static GameUnitState RobotMoveCatchState = new RobotCatchState();

	public static GameUnitState RobotIdleState = new RobotIdleState();

	public static GameUnitState RobotFireState = new RobotFireState();

	public static GameUnitState RobotDeadState = new RobotDeadState();

	public static GameUnitState RobotInvalidState = new RobotInvalidState();

	public static GameUnitState RobotMindState = new RobotMindState();

	public bool Destroyed;

	public bool Arrived;

	public bool hasHPBar;

	public int CurrentLocation;

	public List<float> NPCIdleDuration;

	private ProtectedItemHpScript mHpScript;

	public List<float> SpeedValues;

	public float ProtectRadius;

	public GameObject MissionObject;

	public int mCurrentConditionLayer;

	public ConditionState[] mConditionStates = new ConditionState[3];

	public Dictionary<GameUnitState, ConditionState> mStateSwitchDic;

	public Dictionary<string, ConditionState> mStateTag;

	public List<ConditionState> mAllConditionState;

	private ERobotType mRobotType;

	private Player mLeader;

	protected float mRunSpeed = 10f;

	private static UnityEngine.AI.NavMeshHit mOutHit = default(UnityEngine.AI.NavMeshHit);

	protected Timer mFindTargetTimer = new Timer();

	public float MoveStateStartTime;

	public float MoveIdleDuration = 1f;

	public float AttackIdleDuration;

	public float AttackDuration;

	public bool IsJustReturnedToGame;

	public Vector3 AimDirection;

	private readonly int divide = 8;

	private int[] indexList;

	public static Vector2 ROBOT_MOVE_SPEED = new Vector2(4f, 7f);

	public static Vector2 ROBOT_STOP_TIME = new Vector2(1f, 7f);

	public static Vector2 ROBOT_DISTANCE = new Vector2(2f, 10f);

	public static Vector2 ROBOT_DISTANCE_TO_PLAYER = new Vector2(10f, 20f);

	public static Vector2 ATTACK_IDLE_DURATION = new Vector2(0f, 2f);

	public static Vector2 ATTACK_DURATION = new Vector2(1.5f, 6f);

	public static float PLAYER_HAS_HIT_TARGET_MAX_TIME = 11f;

	public static float ROBOT_CHOOSE_TARGET_RADIUS_0 = 100f;

	public static float ROBOT_CHOOSE_TARGET_RADIUS_1 = 20f;

	public static float ROBOT_CHOOSE_TARGET_ANGLE_1 = 120f;

	public static float NPC_CHOOSE_TARGET_RADIUS = 5f;

	public static float NPC_CHOOSE_TARGET_ANGLE = 360f;

	public static float ROBOT_DISAPPEAR_AFTER_DEAD = 4f;

	public static string ROBOT_DISAPPEAR_VFX = "_Old/_Effect/RPG_EXP_87";

	public static float ROBOT_BACK_DURATION = 10f;

	public static string ROBOT_BACK_VFX = "_Old/_Effect/RPG_EXP_87";

	public static float ROBOT_SCALE = 1f;

	public static Vector2 RailGunChargeDuration = new Vector2(0.2f, 1.5f);

	public Mission OwnerMission { get; set; }

	public bool CanMove { get; set; }

	public List<GameObject> PathPoints { get; set; }

	public bool IsLeader { get; set; }

	public Robot(ERobotType type = ERobotType.XIAOHUOBAN)
	{
		base.GameUnitType = EGameUnitType.ROBOT;
		mRobotType = type;
	}

	public Player GetLeader()
	{
		return mLeader;
	}

	public ERobotType GetRobotType()
	{
		return mRobotType;
	}

	public void SetRobotType(ERobotType type)
	{
		mRobotType = type;
	}

	public override void Init()
	{
		InitDivide();
		InitSkills();
		base.Init();
		if (mRobotType == ERobotType.XIAOHUOBAN)
		{
			entityObject.name = "_小伙伴";
			entityObject.layer = PhysicsLayer.ROBOT;
			entityObject.tag = TagName.PLAYER_ROOT;
		}
		else if (mRobotType == ERobotType.PROTECTED_NPC)
		{
			entityObject.name = "Protected_NPC";
			entityObject.layer = PhysicsLayer.ROBOT;
			entityObject.tag = TagName.PLAYER_ROOT;
			AddHPBar();
		}
		else if (mRobotType == ERobotType.DEFENDER)
		{
			entityObject.name = "DEFENDER";
			entityObject.layer = PhysicsLayer.ROBOT;
			entityObject.tag = TagName.PLAYER_ROOT;
			AddHPBar();
		}
		int num = 1;
		string name = entityObject.name;
		while (GameApp.GetInstance().GetGameWorld().GetRobotByName(name) != null)
		{
			name = entityObject.name + num;
			num++;
		}
		entityObject.name = name;
		entityObject.transform.localScale = new Vector3(ROBOT_SCALE, ROBOT_SCALE, ROBOT_SCALE);
		Transform transform = entityTransform.Find(BoneName.Bag).Find("Bag");
		if (transform != null)
		{
			transform.localPosition = Vector3.zero;
		}
		MaxHp += (int)playerSkill.GetSkill(SkillsType.HP_BOOTH);
		Hp = MaxHp;
		ConditionStateManager.GetInstance().InitConditionState(this);
		mLeader = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		Object.Destroy(entityObject.GetComponent<CharacterController>());
		if (entityObject.GetComponent<CapsuleCollider>() == null)
		{
			CapsuleCollider capsuleCollider = entityObject.AddComponent<CapsuleCollider>();
			capsuleCollider.height = 2f;
			capsuleCollider.center = Vector3.up;
		}
		mFindTargetTimer.SetTimer(1f, false);
		UpdateWeaponDamage();
	}

	public override Collider GetCollider()
	{
		return entityObject.GetComponent<Collider>();
	}

	public void SetState(ConditionState newState, int layer)
	{
		if (mConditionStates[layer] != null)
		{
			mConditionStates[layer].mState.End(this);
		}
		mConditionStates[layer] = newState;
		if (mConditionStates[layer] != null)
		{
			mConditionStates[layer].mState.Start(this);
		}
		if (layer == 1)
		{
			StateStartTime = Time.time;
		}
	}

	public void SetState(ConditionState newState)
	{
		SetState(newState, mCurrentConditionLayer);
	}

	public new void SetState(GameUnitState newState)
	{
		SetState(newState, mCurrentConditionLayer);
	}

	public void SetState(string stateTag)
	{
		if (mStateTag.ContainsKey(stateTag))
		{
			SetState(mStateTag[stateTag], mCurrentConditionLayer);
		}
		else
		{
			Debug.LogError("No StateTag: " + stateTag);
		}
	}

	public void SetState(GameUnitState state, int layer)
	{
		if (mStateSwitchDic.ContainsKey(state))
		{
			SetState(mStateSwitchDic[state], layer);
		}
	}

	public override bool HandleMessage(EConditionMessage message)
	{
		if (mRobotType == ERobotType.XIAOHUOBAN)
		{
			switch (message)
			{
			case EConditionMessage.EMoveToNextPoint:
				mRunSpeed = Random.Range(ROBOT_MOVE_SPEED.x, ROBOT_MOVE_SPEED.y);
				mTargetPosition = ChooseNextPoint();
				SetState(RobotMoveCatchState);
				return true;
			case EConditionMessage.EDecideMoveOrIdle:
			{
				if (mLeader == null || !mLeader.InPlayingState())
				{
					SetState(RobotMoveIdleState);
				}
				float num = Vector3.Magnitude(mLeader.GetPosition() - GetPosition());
				if (num < ROBOT_DISTANCE_TO_PLAYER.x)
				{
					SetState(RobotMoveIdleState);
				}
				else if (num < ROBOT_DISTANCE_TO_PLAYER.y)
				{
					mRunSpeed = ROBOT_MOVE_SPEED.x;
					mTargetPosition = ChooseNextPoint();
					SetState(RobotMoveCatchState);
				}
				else
				{
					mRunSpeed = ROBOT_MOVE_SPEED.y;
					mTargetPosition = ChooseNextPoint();
					SetState(RobotMoveCatchState);
				}
				return true;
			}
			case EConditionMessage.ETryAttack:
				if (Time.time - mLeader.LastHitTargetTime < PLAYER_HAS_HIT_TARGET_MAX_TIME)
				{
					if (mLeader.LastHitTarget != null && mLeader.LastHitTarget.InPlayingState())
					{
						if (GameWorld.FindEnemyTarget(mLeader, true, ROBOT_CHOOSE_TARGET_RADIUS_1, ROBOT_CHOOSE_TARGET_ANGLE_1) != null)
						{
							mTarget = GameWorld.ResultTarget;
							SetState(RobotFireState);
						}
						else
						{
							SetState(RobotIdleState);
						}
					}
					else if (GameWorld.FindEnemyTarget(mLeader, false, ROBOT_CHOOSE_TARGET_RADIUS_0) != null)
					{
						mTarget = GameWorld.ResultTarget;
						SetState(RobotFireState);
					}
					else
					{
						SetState(RobotIdleState);
					}
				}
				else
				{
					SetState(RobotIdleState);
				}
				return true;
			default:
				HandleMindMessage(message);
				return false;
			}
		}
		if (mRobotType == ERobotType.PROTECTED_NPC)
		{
			switch (message)
			{
			case EConditionMessage.EDefenseAttack:
				if (mLeader.LastHitTarget != null && mLeader.LastHitTarget.InPlayingState())
				{
					if (GameWorld.FindEnemyTarget(mLeader, true, ROBOT_CHOOSE_TARGET_RADIUS_1, ROBOT_CHOOSE_TARGET_ANGLE_1, true) != null)
					{
						mTarget = GameWorld.ResultTarget;
						SetState("NPCFire");
					}
					else
					{
						SetState("NPCIdle");
					}
				}
				else if (GameWorld.FindEnemyTarget(mLeader, true, ROBOT_CHOOSE_TARGET_RADIUS_1, ROBOT_CHOOSE_TARGET_ANGLE_1, true) != null)
				{
					mTarget = GameWorld.ResultTarget;
					SetState("NPCFire");
				}
				else
				{
					SetState("NPCIdle");
				}
				return true;
			case EConditionMessage.ENPCMoveNext:
				OwnerMission.TriggerAction();
				if (CurrentLocation <= PathPoints.Count)
				{
					mRunSpeed = SpeedValues[CurrentLocation - 1];
					Debug.Log("Go to " + PathPoints[CurrentLocation - 1].name);
					mTargetPosition = PathPoints[CurrentLocation - 1].transform.position;
				}
				else
				{
					Arrived = true;
				}
				SetState("NPCMoveCatch");
				return true;
			case EConditionMessage.EFindNextTarget:
				if (mLeader.LastHitTarget != null && mLeader.LastHitTarget.InPlayingState())
				{
					if (GameWorld.FindEnemyTarget(mLeader, true, ROBOT_CHOOSE_TARGET_RADIUS_1, ROBOT_CHOOSE_TARGET_ANGLE_1, true) != null)
					{
						mTarget = GameWorld.ResultTarget;
					}
					else
					{
						SetState("NPCIdle");
					}
				}
				else if (GameWorld.FindEnemyTarget(mLeader, true, ROBOT_CHOOSE_TARGET_RADIUS_1, ROBOT_CHOOSE_TARGET_ANGLE_1, true) != null)
				{
					mTarget = GameWorld.ResultTarget;
				}
				else
				{
					SetState("NPCIdle");
				}
				return true;
			default:
				NPCHandleMindMessage(message);
				return false;
			}
		}
		if (mRobotType == ERobotType.DEFENDER)
		{
			switch (message)
			{
			case EConditionMessage.EMoveToNextPoint:
				mRunSpeed = ROBOT_MOVE_SPEED.y;
				mTargetPosition = ChooseNextPoint();
				SetState(RobotMoveCatchState);
				return true;
			case EConditionMessage.EDecideMoveOrIdle:
			{
				if (mLeader == null || !mLeader.InPlayingState())
				{
					SetState(RobotMoveIdleState);
				}
				float num2 = Vector3.Magnitude(mLeader.GetPosition() - GetPosition());
				if (num2 < ROBOT_DISTANCE_TO_PLAYER.x)
				{
					SetState(RobotMoveIdleState);
				}
				else if (num2 < ROBOT_DISTANCE_TO_PLAYER.y)
				{
					mRunSpeed = ROBOT_MOVE_SPEED.x;
					mTargetPosition = ChooseNextPoint();
					SetState(RobotMoveCatchState);
				}
				else
				{
					mRunSpeed = ROBOT_MOVE_SPEED.y;
					mTargetPosition = ChooseNextPoint();
					SetState(RobotMoveCatchState);
				}
				return true;
			}
			case EConditionMessage.ETryAttack:
				if (Time.time - mLeader.LastHitTargetTime < PLAYER_HAS_HIT_TARGET_MAX_TIME)
				{
					if (mLeader.LastHitTarget != null && mLeader.LastHitTarget.InPlayingState())
					{
						if (GameWorld.FindEnemyTarget(MissionObject, ProtectRadius, NPC_CHOOSE_TARGET_ANGLE) != null)
						{
							mTarget = GameWorld.ResultTarget;
							SetState(RobotFireState);
						}
						else
						{
							SetState(RobotIdleState);
						}
					}
					else if (GameWorld.FindEnemyTarget(MissionObject, ProtectRadius, NPC_CHOOSE_TARGET_ANGLE) != null)
					{
						mTarget = GameWorld.ResultTarget;
						SetState(RobotFireState);
					}
					else
					{
						SetState(RobotIdleState);
					}
				}
				else
				{
					SetState(RobotIdleState);
				}
				return true;
			case EConditionMessage.EFindNextTarget:
				if (Time.time - mLeader.LastHitTargetTime < PLAYER_HAS_HIT_TARGET_MAX_TIME)
				{
					if (mLeader.LastHitTarget != null && mLeader.LastHitTarget.InPlayingState())
					{
						if (GameWorld.FindEnemyTarget(MissionObject, ProtectRadius, NPC_CHOOSE_TARGET_ANGLE) != null)
						{
							mTarget = GameWorld.ResultTarget;
						}
						else
						{
							SetState(RobotIdleState);
						}
					}
					else if (GameWorld.FindEnemyTarget(MissionObject, ProtectRadius, NPC_CHOOSE_TARGET_ANGLE) != null)
					{
						mTarget = GameWorld.ResultTarget;
					}
					else
					{
						SetState(RobotIdleState);
					}
				}
				else
				{
					SetState(RobotIdleState);
				}
				return true;
			default:
				NPCHandleMindMessage(message);
				return false;
			}
		}
		return false;
	}

	private bool NPCHandleMindMessage(EConditionMessage message)
	{
		if (message == EConditionMessage.ERandomChangeGun)
		{
			ChangeWeaponMessageData changeWeaponMessageData = Condition.MessageData as ChangeWeaponMessageData;
			RandomChangeWeapon(changeWeaponMessageData.Chances);
		}
		return false;
	}

	private bool HandleMindMessage(EConditionMessage message)
	{
		switch (message)
		{
		case EConditionMessage.EDialog:
		{
			DialogMessageData dialogMessageData = Condition.MessageData as DialogMessageData;
			UserStateUI.GetInstance().FightDialogManager.PushMissionDialog(dialogMessageData.StartString + Random.Range(1, dialogMessageData.Num + 1));
			break;
		}
		case EConditionMessage.EBackToBattle:
		{
			IsJustReturnedToGame = false;
			GameObject original2 = Resources.Load(ROBOT_BACK_VFX) as GameObject;
			Object.Instantiate(original2, GetPosition(), Quaternion.identity);
			break;
		}
		case EConditionMessage.EUseAttackSkill:
			Debug.Log("AOE:让火焰净化一切!");
			break;
		case EConditionMessage.EGoneTeleport:
		{
			GameObject original = Resources.Load(ROBOT_DISAPPEAR_VFX) as GameObject;
			Object.Instantiate(original, GetPosition(), Quaternion.identity);
			GetTransform().position += new Vector3(0f, 100000f, 0f);
			GetObject().SetActive(false);
			SetState(RobotInvalidState, 1);
			break;
		}
		case EConditionMessage.ERandomChangeGun:
		{
			ChangeWeaponMessageData changeWeaponMessageData = Condition.MessageData as ChangeWeaponMessageData;
			RandomChangeWeapon(changeWeaponMessageData.Chances);
			break;
		}
		}
		return false;
	}

	private void InitDivide()
	{
		indexList = new int[divide];
		float num = 360 / divide;
		for (int i = 0; i < divide; i++)
		{
			indexList[i] = i;
		}
	}

	public Vector3 ChooseNextPoint()
	{
		for (int i = 0; i < divide; i++)
		{
			indexList[i] = i;
		}
		float num = 360 / divide;
		Vector3 vector = Vector3.zero;
		for (int j = 0; j < divide; j++)
		{
			int num2 = indexList[j];
			int num3 = Random.Range(0, divide);
			indexList[j] = indexList[num3];
			indexList[num3] = num2;
		}
		for (int k = 0; k < divide; k++)
		{
			vector = Quaternion.Euler(0f, (float)indexList[k] * num, 0f) * (mLeader.GetTransform().forward * Random.Range(ROBOT_DISTANCE.x, ROBOT_DISTANCE.y)) + mLeader.GetPosition();
			vector.y = mLeader.GetPosition().y;
			if (mNavMeshAgent.enabled && !mNavMeshAgent.Raycast(vector, out mOutHit))
			{
				break;
			}
		}
		return vector;
	}

	public override void Loop(float deltaTime)
	{
		base.Loop(deltaTime);
		if (GameApp.GetInstance().GetGameWorld() != null && GameApp.GetInstance().GetGameWorld().State == GameState.Playing)
		{
			if (InPlayingState())
			{
				mCurrentConditionLayer = 0;
				mConditionStates[0].Update(this);
				mCurrentConditionLayer = 2;
				mConditionStates[2].Update(this);
			}
			mCurrentConditionLayer = 1;
			mConditionStates[1].Update(this);
		}
	}

	public override void UpdateNavMeshInCatching(bool force)
	{
		if (force || Time.time - mLastUpdateNavMeshTime > 3f / mRunSpeed)
		{
			mLastUpdateNavMeshTime = Time.time;
			if (null != mNavMeshAgent)
			{
				mNavMeshAgent.enabled = true;
				mNavMeshAgent.Resume();
				mNavMeshAgent.SetDestination(mTargetPosition);
				mNavMeshAgent.speed = mRunSpeed;
				SetCanTurn(false);
			}
		}
	}

	public override bool InPlayingState()
	{
		return mConditionStates[1].mState != RobotDeadState && entityObject != null && entityObject.activeSelf;
	}

	public override bool IsLocal()
	{
		return true;
	}

	public override void UnderAttack(int damage)
	{
		if (InPlayingState())
		{
			Hp -= damage;
			Hp = Mathf.Clamp(Hp, 0, MaxHp);
			if (Hp <= 0)
			{
				OnDead();
			}
			else
			{
				AudioManager.GetInstance().PlaySoundAt(AudioName.PLAYER_HIT_LIGHT, entityTransform.position);
			}
		}
		if (hasHPBar)
		{
			mHpScript.SetHpFillAmount((float)Hp / (float)MaxHp);
			MissionManager.GetInstance().ProtectedItemHpPercentage = Mathf.Clamp(100f * (float)Hp / (float)MaxHp, 0f, MissionManager.GetInstance().ProtectedItemHpPercentage);
		}
		base.NeverGotHit = false;
	}

	public override void OnDead()
	{
		if (mVehicle != null)
		{
			mVehicle.GetOff();
		}
		GetWeapon().StopFire();
		GetWeapon().PlayIdleAnim();
		GetWeapon().StopSound();
		StopWalkSound();
		if (mVehicleState != EPlayerVehicleState.NONE && mVehicle != null)
		{
			mVehicle.StopAllSound();
		}
		SetState(RobotDeadState);
		CreateDeadBlood();
		Destroyed = true;
	}

	public void Reset()
	{
		CompareData.Clear();
	}

	public void AddHPBar()
	{
		hasHPBar = true;
		GameObject original = Resources.Load("Mission/Protected/BillBoard") as GameObject;
		GameObject gameObject = Object.Instantiate(original) as GameObject;
		gameObject.transform.position += GetPosition();
		gameObject.transform.parent = entityTransform;
		mHpScript = gameObject.GetComponent<ProtectedItemHpScript>();
	}

	public override void ChangeToIdleState()
	{
	}

	public void RandomChangeWeapon(float[] chances)
	{
		float[] array = new float[weaponList.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = chances[i];
		}
		int randomIndex = GameWorld.GetRandomIndex(array);
		if (weaponList[randomIndex] != weapon)
		{
			ChangeWeapon(weaponList[randomIndex]);
		}
	}

	public override bool IsRobot()
	{
		return true;
	}

	public void UpdateWeaponDamage()
	{
		float currentDps = mLeader.GetCurrentDps();
		foreach (Weapon weapon in weaponList)
		{
			if (currentDps >= weapon.Dps * 1.25f)
			{
				weapon.Dps = currentDps * 0.8f;
			}
			else if (currentDps <= weapon.Dps * 0.8f)
			{
				weapon.Dps = currentDps * 1.25f;
			}
			else
			{
				weapon.Dps = (currentDps + weapon.Dps) * 0.5f;
			}
		}
	}

	public void SetWeaponDamage(float scale)
	{
		foreach (Weapon weapon in weaponList)
		{
			weapon.Dps *= scale;
		}
	}

	protected override EAimType GetAimType()
	{
		EAimType result = EAimType.NONE;
		if (mConditionStates[1].mState == RobotIdleState)
		{
			result = weapon.GetIdleAimType();
		}
		else if (mConditionStates[1].mState == RobotFireState)
		{
			result = weapon.GetAttackAimType();
		}
		return result;
	}

	public float GetCurrentIndexPercent()
	{
		return (float)(CurrentLocation - 1) / (float)PathPoints.Count;
	}
}
