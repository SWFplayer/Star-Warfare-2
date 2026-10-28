using System;
using System.Collections.Generic;
using UnityEngine;

public class SwVoid : GameUnit
{
	public static bool isUse = true;

	public static SwVoidState FLOATING_STATE = new SwVoidFloatingState();

	public static SwVoidState OPEN_STATE = new SwVoidOpenState();

	public static SwVoidState SPECIAL_IDLE_STATE = new SwVoidSpIdleState();

	public static SwVoidState AUTO_PICKUP_STATE = new SwVoidAutoPickState();

	public static SwVoidState INACTIVE_STATE = new SwVoidInactiveState();

	public static SwVoidState SHOP_STATE = new SwVoidShopState();

	protected Player mPlayer;

	protected Transform mPlayerTransform;

	protected Transform mPlayerSwVoidTransform;

	protected new UnityEngine.AI.NavMeshAgent mNavMeshAgent;

	protected SwVoidState mState;

	protected GameUnit attackTarget;

	protected GameWorld gameWorld;

	public ESwVoidType mType;

	protected byte mLevel;

	protected float startIdleTime;

	protected float startPickupTime;

	public float endPickupTime;

	protected float DISTANCE_CLOSE = 10f;

	protected float DISTANCE_MIDDLE = 20f;

	protected float DISTANCE_FAR = 30f;

	public static Vector3 SW_VOID_DELTA_POSITION = new Vector3(2f, 0f, 2f);

	protected bool isDash;

	protected float farSpeed;

	protected float YDefaultOffset;

	protected float modifyY;

	protected float floatingSpeed = 1f;

	protected float maxFloatingY = 1f;

	protected float floatingRate = 1f;

	protected bool firstHalf = true;

	protected float timeToSpecialIdle;

	protected SwVoidSavedata swVoidSavedata;

	protected List<SwVoidAttribute> attributes;

	protected List<Skill> skillListForPlayer;

	protected bool hasAutoPickSkill;

	private Vector3 autoPickItemPosition;

	private ItemScript mTargetItem;

	protected bool isLocal;

	public float timeToOpen { get; set; }

	public virtual void Loop(float deltaTime)
	{
		mState.Update(this, Time.deltaTime);
	}

	protected void catchPlayer(float deltaTime)
	{
		if (mPlayerSwVoidTransform == null)
		{
			return;
		}
		Vector3 vector = mPlayerSwVoidTransform.position - GetTransform().position;
		vector.y = 0f;
		float magnitude = vector.magnitude;
		if (magnitude < DISTANCE_CLOSE)
		{
			StartNavMesh(mPlayerSwVoidTransform.position, Speed);
			timeToSpecialIdle -= deltaTime;
			if (timeToSpecialIdle < 0f)
			{
				SetState(SPECIAL_IDLE_STATE);
				timeToSpecialIdle = UnityEngine.Random.Range(5, 10);
			}
		}
		else if (magnitude < DISTANCE_MIDDLE)
		{
			StartNavMesh(mPlayerSwVoidTransform.position, Speed);
			PlayAnimation(AnimationString.Idle, WrapMode.Loop);
			isDash = false;
			timeToSpecialIdle -= deltaTime;
			if (timeToSpecialIdle < 0f)
			{
				SetState(SPECIAL_IDLE_STATE);
				timeToSpecialIdle = UnityEngine.Random.Range(5, 10);
			}
		}
		else if (magnitude < DISTANCE_FAR && !isDash)
		{
			StartNavMesh(mPlayerSwVoidTransform.position, Speed);
			PlayAnimation(AnimationString.SWVOID_RUN, WrapMode.Loop);
		}
		else
		{
			StartNavMesh(mPlayerSwVoidTransform.position, farSpeed);
			isDash = true;
			PlayAnimation(AnimationString.SWVOID_DASH, WrapMode.Loop);
		}
	}

	public virtual void init(GameObject gObject, Player player)
	{
		entityObject = gObject;
		entityTransform = entityObject.transform;
		LoadConfig();
		if (entityObject.GetComponent<CharacterController>() != null)
		{
			cc = entityObject.GetComponent<CharacterController>();
			characterControllerCenter = cc.center;
		}
		mPlayer = player;
		mPlayerTransform = mPlayer.GetTransform();
		entityTransform.position = GetSafePositionAroundLocalPlayer(mPlayerTransform);
		player.mSwVoid = this;
		if (player.IsLocal())
		{
			isLocal = true;
		}
		else
		{
			isLocal = false;
		}
		CreateNavMeshAgent();
		InitAnimation();
		gameWorld = GameApp.GetInstance().GetGameWorld();
		SetPlayerFollowTransform();
		mNavMeshAgent.baseOffset = YDefaultOffset;
		timeToSpecialIdle = UnityEngine.Random.Range(5, 10);
		mLevel = swVoidSavedata.GetLevel();
		SetState(INACTIVE_STATE);
		if ((swVoidSavedata.isPassiveSkillActive(0) && swVoidSavedata.GetPassiveSkillId(0) == SwVoidParameter.STEEL_ARMOR_SKILL) || (swVoidSavedata.isPassiveSkillActive(1) && swVoidSavedata.GetPassiveSkillId(1) == SwVoidParameter.STEEL_ARMOR_SKILL))
		{
			UnityEngine.Object original = Resources.Load(EffectName.SKILL_1);
			GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
			gameObject.transform.parent = entityTransform.Find("VoidEntity");
			gameObject.transform.localPosition = Vector3.zero;
		}
		else if ((swVoidSavedata.isPassiveSkillActive(0) && swVoidSavedata.GetPassiveSkillId(0) == SwVoidParameter.AUTO_PICKUP_SKILL) || (swVoidSavedata.isPassiveSkillActive(1) && swVoidSavedata.GetPassiveSkillId(1) == SwVoidParameter.AUTO_PICKUP_SKILL))
		{
			hasAutoPickSkill = true;
		}
	}

	public void InitForUI(GameObject gObject)
	{
		entityObject = gObject;
		entityTransform = entityObject.transform;
		InitAnimation();
		SetState(SHOP_STATE);
	}

	public new void CreateNavMeshAgent()
	{
		if (mNavMeshAgent == null)
		{
			entityObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
			mNavMeshAgent = entityObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
			mNavMeshAgent.speed = Speed;
			mNavMeshAgent.angularSpeed = 720f;
			mNavMeshAgent.acceleration = 100000f;
			mNavMeshAgent.baseOffset = -0.1f;
			mNavMeshAgent.walkableMask = (1 << NavMeshLayer.NORMAL_GROUND) | (1 << NavMeshLayer.JUMP);
			mNavMeshAgent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.LowQualityObstacleAvoidance;
			mNavMeshAgent.stoppingDistance = 0.5f;
			mNavMeshAgent.enabled = true;
		}
	}

	public new void StopNavMesh()
	{
		if (null != mNavMeshAgent && mNavMeshAgent.enabled)
		{
			mNavMeshAgent.Stop(true);
			mNavMeshAgent.enabled = false;
		}
	}

	public void StartNavMesh(Vector3 destination, float newSpeed)
	{
		if (null != mNavMeshAgent)
		{
			mNavMeshAgent.enabled = true;
			mNavMeshAgent.Resume();
			mNavMeshAgent.SetDestination(destination);
			mNavMeshAgent.speed = newSpeed;
		}
	}

	public virtual void UpdateNavMeshInCatching()
	{
	}

	public virtual void Floating()
	{
		bool flag = false;
		if (isDash || mState != FLOATING_STATE)
		{
			flag = true;
		}
		modifyY += floatingSpeed * Time.deltaTime;
		if (modifyY > 1f && firstHalf)
		{
			firstHalf = false;
			if (flag)
			{
				floatingRate = 0.5f;
			}
			else
			{
				floatingRate = 1f;
			}
		}
		if (modifyY > 2f)
		{
			modifyY -= 2f;
			firstHalf = true;
			if (flag)
			{
				floatingRate = 0.5f;
			}
			else
			{
				floatingRate = 1f;
			}
		}
		mNavMeshAgent.baseOffset = maxFloatingY * floatingRate * Mathf.Sin(modifyY * (float)Math.PI) + YDefaultOffset;
	}

	public virtual void SetState(SwVoidState newState)
	{
		if (mState != null)
		{
			mState.End(this);
		}
		mState = newState;
		if (mState != null)
		{
			mState.Start(this);
		}
	}

	public SwVoidState GetState()
	{
		return mState;
	}

	public virtual void DoIdleFloating(float deltaTime)
	{
		catchPlayer(deltaTime);
		Floating();
		Vector3 vector = new Vector3(mPlayer.GetPosition().x, entityTransform.position.y, mPlayer.GetPosition().z);
		if (isLocal && Time.time - endPickupTime > SwVoidParameter.AUTO_PICKUP_INTERVAL && Vector3.Distance(entityTransform.position, vector) < SwVoidParameter.DISTANCE_CLOSE)
		{
			TryPickupItem();
		}
		if (Time.time - startIdleTime >= 5f && Vector3.Distance(vector, entityTransform.position) >= DISTANCE_FAR)
		{
			StopNavMesh();
			entityTransform.position = GetSafePositionAroundLocalPlayer(mPlayerTransform);
			SetState(OPEN_STATE);
		}
	}

	public virtual void StartIdle()
	{
		PlayAnimation(AnimationString.SWVOID_IDLE, WrapMode.Loop);
		startIdleTime = Time.time;
	}

	public virtual void DoOpen()
	{
		if (AnimationPlayed(AnimationString.SWVOID_OPEN, 1f) && mPlayer.InPlayingState())
		{
			SetState(FLOATING_STATE);
		}
	}

	public virtual void StartOpen()
	{
		PlayAnimation(AnimationString.SWVOID_OPEN, WrapMode.ClampForever);
	}

	public virtual void DoSpecialIdle()
	{
		if (AnimationPlayed(AnimationString.SWVOID_IDLE_SP, 1f))
		{
			SetState(FLOATING_STATE);
		}
	}

	public void StartAutoPickup()
	{
		startPickupTime = Time.time;
		StartNavMesh(autoPickItemPosition, Speed);
	}

	public void DoAutoPickup()
	{
		Vector3 vector = autoPickItemPosition;
		vector.y = 0f;
		Vector3 position = entityTransform.position;
		position.y = 0f;
		autoPickItemPosition.y = entityTransform.position.y;
		StartNavMesh(autoPickItemPosition, Speed);
		if ((vector - position).magnitude < 1f)
		{
			mPlayer.OnPickUpBySwVoid(mTargetItem);
			SetState(FLOATING_STATE);
		}
		else if (mTargetItem == null || Time.time - startPickupTime > SwVoidParameter.AUTO_PICKUP_MAX_TIME)
		{
			StopNavMesh();
			SetState(FLOATING_STATE);
		}
	}

	public void StartInactive()
	{
		timeToOpen = Time.time + 5f;
		entityObject.SetActive(false);
	}

	public void DoInactive()
	{
		if (timeToOpen < Time.time)
		{
			if (animation == null)
			{
				SetState(FLOATING_STATE);
			}
			else
			{
				SetState(OPEN_STATE);
			}
		}
	}

	public void EndInactive()
	{
		entityObject.SetActive(true);
	}

	public void ChangeStateFromResponse(ESwVoidStateID state, Vector3 pos, int targetId, short enemyId)
	{
		switch (state)
		{
		case ESwVoidStateID.SOLDIER_ATTACK:
			if (mType == ESwVoidType.SOLDIER)
			{
				if (targetId != 0)
				{
					attackTarget = gameWorld.GetPlayerByUserID(targetId);
				}
				else
				{
					attackTarget = gameWorld.GetEnemyByID("E_" + enemyId);
				}
				((SwVoidSoldier)this).isFirstRoundAttack = true;
				SetState(SwVoidSoldier.ATTACK_STATE);
			}
			break;
		case ESwVoidStateID.SOLDIER_CHANGE_TARGET:
			if (mType == ESwVoidType.SOLDIER)
			{
				if (targetId != 0)
				{
					attackTarget = gameWorld.GetPlayerByUserID(targetId);
				}
				else
				{
					attackTarget = gameWorld.GetEnemyByID("E_" + enemyId);
				}
				((SwVoidSoldier)this).isFirstRoundAttack = false;
				SetState(SwVoidSoldier.ATTACK_STATE);
			}
			break;
		case ESwVoidStateID.DEMOMAN_EXPLODE:
			if (mType == ESwVoidType.DEMOMAN)
			{
				((SwVoidDemoman)this).GetExplodeResponse(pos);
			}
			break;
		case ESwVoidStateID.SPY_INVISIBLE:
			if (mType == ESwVoidType.SPY)
			{
				((SwVoidSpy)this).SetState(SwVoidSpy.INVISIBLE_STATE);
			}
			break;
		case ESwVoidStateID.SCOUT_DASH:
			if (mType == ESwVoidType.SCOUT)
			{
				((SwVoidScout)this).chargeTargetPosition = pos;
				((SwVoidScout)this).SetState(SwVoidScout.DASH_STATE);
			}
			break;
		case ESwVoidStateID.AUTO_PICKUP:
			mTargetItem = gameWorld.GetMoneyBySequenceID(enemyId);
			if (mTargetItem != null)
			{
				autoPickItemPosition = mTargetItem.transform.position;
				autoPickItemPosition.y += mNavMeshAgent.baseOffset;
				SetState(AUTO_PICKUP_STATE);
			}
			break;
		case ESwVoidStateID.HEAVY_SHIELD:
			if (mType == ESwVoidType.HEAVY)
			{
				SwVoidHeavy swVoidHeavy = this as SwVoidHeavy;
				swVoidHeavy.shieldPosition = pos;
				swVoidHeavy.SetState(SwVoidHeavy.SHIELD_STATE);
			}
			break;
		case ESwVoidStateID.MEDIC_HEAL:
			break;
		}
	}

	protected void TryPickupItem()
	{
		if (!hasAutoPickSkill || gameWorld.GetItemList() == null)
		{
			return;
		}
		float num = SwVoidParameter.AUTO_PICKUP_DISTANCE;
		foreach (ItemScript item in gameWorld.GetItemList())
		{
			if (item.itemType == LootType.MONEY)
			{
				Vector3 position = item.transform.position;
				float magnitude = (entityTransform.transform.position - position).magnitude;
				if (magnitude < num)
				{
					autoPickItemPosition = position;
					autoPickItemPosition.y = mNavMeshAgent.baseOffset + autoPickItemPosition.y;
					mTargetItem = item;
					num = magnitude;
				}
			}
		}
		if (num < SwVoidParameter.AUTO_PICKUP_DISTANCE)
		{
			SetState(AUTO_PICKUP_STATE);
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				ESwVoidStateID state = ESwVoidStateID.AUTO_PICKUP;
				SwVoidStateRequest request = new SwVoidStateRequest(state, mTargetItem.sequenceID, 0, Vector3.zero);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
	}

	protected void SetPlayerFollowTransform()
	{
		mPlayerSwVoidTransform = mPlayerTransform.Find("SwVoidPosition");
		if (mPlayerSwVoidTransform == null)
		{
			mPlayerSwVoidTransform = GetTransform().Find("SwVoidPosition");
			mPlayerSwVoidTransform.parent = mPlayer.GetTransform();
		}
		mPlayerSwVoidTransform.position = mPlayer.GetTransform().position + SW_VOID_DELTA_POSITION.x * mPlayerTransform.forward + SW_VOID_DELTA_POSITION.z * mPlayerTransform.right + SW_VOID_DELTA_POSITION.y * mPlayerTransform.up;
		GetTransform().transform.position = mPlayerSwVoidTransform.position;
	}

	protected void LoadConfig()
	{
		DISTANCE_CLOSE = SwVoidParameter.DISTANCE_CLOSE;
		DISTANCE_MIDDLE = SwVoidParameter.DISTANCE_MIDDLE;
		DISTANCE_FAR = SwVoidParameter.DISTANCE_FAR;
		SW_VOID_DELTA_POSITION = SwVoidParameter.DELTA_POSITION_OF_PLAYER;
		floatingSpeed = SwVoidParameter.FLOATING_SPEED;
		maxFloatingY = SwVoidParameter.FLOATING_RANGE;
		Speed = SwVoidParameter.SPEED;
		farSpeed = SwVoidParameter.FAR_SPEED;
		YDefaultOffset = SwVoidParameter.Y_DEFAULT_OFFSET;
	}

	public virtual void InitSkillButton(StateSkill stateSkill)
	{
	}

	public virtual void Destroy()
	{
		mPlayer.mSwVoid = null;
		UnityEngine.Object.Destroy(entityObject);
	}

	public virtual void DestroyForUI()
	{
		UnityEngine.Object.Destroy(entityObject);
	}

	public Player GetPlayer()
	{
		return mPlayer;
	}

	public string GetIconName()
	{
		return SwVoidDataManager.GetSwVoidParameterString(mType, mLevel, SwVoidDataManager.ESwVoidActiveSkillColumn.ICON_NAME);
	}

	public bool IsHighLevel()
	{
		return mLevel >= SwVoidParameter.HIGH_LEVEL;
	}

	public Vector3 GetSafePositionAroundLocalPlayer(Transform tempPlayerTransform)
	{
		if (mPlayer == null)
		{
			return Vector3.zero;
		}
		Vector3[] array = new Vector3[2]
		{
			SW_VOID_DELTA_POSITION,
			-SW_VOID_DELTA_POSITION
		};
		Vector3[] array2 = array;
		RaycastHit hitInfo;
		Ray ray;
		foreach (Vector3 vector in array2)
		{
			Vector3 vector2 = tempPlayerTransform.position + tempPlayerTransform.TransformDirection(vector);
			ray = new Ray(vector2 + Vector3.up * 2f, Vector3.down);
			if (Physics.Raycast(ray, out hitInfo, 4f, 1 << PhysicsLayer.FLOOR))
			{
				vector2.y = hitInfo.point.y + 0.1f;
				return vector2;
			}
			Debug.LogWarning("SwVoid GetSafePositionAroundLocalPlayer No Floor down: " + vector);
		}
		Debug.LogWarning("SwVoid GetSafePositionAroundLocalPlayer No Floor down in four direction, use mPlayer position.");
		ray = new Ray(mPlayer.GetPosition() + Vector3.up * 2f, Vector3.down);
		if (Physics.Raycast(ray, out hitInfo, 4f, 1 << PhysicsLayer.FLOOR))
		{
			Debug.LogWarning(string.Concat("pos = ", hitInfo.point, new Vector3(0f, 0.1f, 0f)));
			return hitInfo.point + new Vector3(0f, 0.1f, 0f);
		}
		Debug.LogWarning("SwVoid GetSafePositionAroundLocalPlayer mPlayer position invalid, use default point.");
		GameObject gameObject = GameObject.FindGameObjectWithTag(TagName.SWVOID_DEFAULT_SPAWN_POINT);
		if (gameObject != null)
		{
			ray = new Ray(gameObject.transform.position + Vector3.up * 2f, Vector3.down);
			if (Physics.Raycast(ray, out hitInfo, 4f, 1 << PhysicsLayer.FLOOR))
			{
				Debug.LogWarning(string.Concat("pos = ", hitInfo.point, new Vector3(0f, 0.2f, 0f)));
				return hitInfo.point + new Vector3(0f, 0.2f, 0f);
			}
		}
		Debug.LogWarning("SwVoid GetSafePositionAroundLocalPlayer FAIL to find default point, use local player position.");
		Debug.LogWarning(string.Concat("pos = ", GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.GetPosition(), new Vector3(0f, 0.1f, 0f)));
		return GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.GetPosition() + new Vector3(0f, 0.1f, 0f);
	}

	public virtual void ActiveSkill()
	{
	}

	public virtual bool IsCoolDown()
	{
		return mState != OPEN_STATE && mState != INACTIVE_STATE;
	}

	public virtual Timer GetTimer(int id)
	{
		return null;
	}

	public void InitAnimation()
	{
		if ((bool)entityObject.transform.Find("VoidEntity"))
		{
			animation = entityObject.transform.Find("VoidEntity").GetComponent<Animation>();
		}
	}

	public new virtual void PlayAnimation(string name, WrapMode mode, float blendTime)
	{
		if (animation[name] == null)
		{
			Debug.LogError(name + " animation is null!");
		}
		else if (!animation.IsPlaying(name) || mode != WrapMode.ClampForever || AnimationPlayed(name, 1f))
		{
			animation[name].wrapMode = mode;
			animation.CrossFade(name, blendTime);
		}
	}

	public new virtual void PlayAnimation(string name, WrapMode mode)
	{
		PlayAnimation(name, mode, 0.2f);
	}

	public new virtual bool IsPlayingAnimation(string name)
	{
		return animation.IsPlaying(name);
	}

	public new virtual void PlayAnimationWithoutBlend(string name, WrapMode mode)
	{
		if (!animation.IsPlaying(name) || mode != WrapMode.ClampForever || AnimationPlayed(name, 1f))
		{
			animation[name].wrapMode = mode;
			animation.Play(name);
		}
	}

	public new bool AnimationPlayed(string name, float percent)
	{
		if (animation[name] == null)
		{
			return false;
		}
		if (animation[name].speed >= 0f)
		{
			if (animation[name].time >= animation[name].clip.length * percent)
			{
				return true;
			}
			return false;
		}
		if (animation[name].time <= animation[name].clip.length * (1f - percent))
		{
			return true;
		}
		return false;
	}

	public virtual string GetCenter()
	{
		return string.Empty;
	}

	public bool IsReadyForActiveSkill()
	{
		return mState == AUTO_PICKUP_STATE || mState == FLOATING_STATE || mState == SPECIAL_IDLE_STATE || mState == SwVoidMedic.AUTO_HEAL_STATE;
	}

	public void SetSavedata(SwVoidSavedata swvoidSavedata)
	{
		swVoidSavedata = swvoidSavedata;
	}

	public virtual void HighLevelInit()
	{
	}
}
