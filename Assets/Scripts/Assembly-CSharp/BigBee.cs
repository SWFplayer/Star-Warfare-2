using UnityEngine;

public class BigBee : Enemy
{
	public static EnemyState BIGBEE_3_SHOT_STATE = new BigBee3ShotState();

	public static int ShotAttackDamage;

	public static int ShotAreaDamage;

	public static float ShotExplodeRadius = 3f;

	public Vector3 mDownSpeed;

	public BigBee()
	{
		base.GameUnitType = EGameUnitType.ENEMY;
		base.EnemyType = EnemyType.BIGBEE;
		IsNewState = true;
	}

	public override void Init(GameObject gObject)
	{
		base.Init(gObject);
		SetShoutTimer("_Old/_Audio/enemy/feixingchong", 5f);
		entityTransform.GetComponent<Rigidbody>().useGravity = false;
		entityTransform.GetComponent<Rigidbody>().isKinematic = true;
		ShotAttackDamage = attackDamage;
		ShotAreaDamage = attackDamage;
		attackRange = 25f;
		ConditionStateManager.GetInstance().InitConditionState(this);
	}

	public override void CreateNavMeshAgent()
	{
		if (mNavMeshAgent == null)
		{
			entityObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
			mNavMeshAgent = entityObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
			mNavMeshAgent.speed = mRunSpeed;
			mNavMeshAgent.angularSpeed = 720f;
			mNavMeshAgent.acceleration = 100000f;
			mNavMeshAgent.walkableMask = 1 << NavMeshLayer.FLY_AREA;
			mNavMeshAgent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.LowQualityObstacleAvoidance;
			mNavMeshAgent.stoppingDistance = 0.5f;
			StopNavMesh();
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
			mRay = new Ray(mTargetPosition, Vector3.up);
			if (Physics.Raycast(mRay, out mRaycastHit, 100f, 1 << PhysicsLayer.FLY_AREA) && null != mNavMeshAgent)
			{
				mNavMeshAgent.enabled = true;
				mNavMeshAgent.Resume();
				mNavMeshAgent.SetDestination(mRaycastHit.point + new Vector3(0f, 0.2f, 0f));
				mNavMeshAgent.speed = mRunSpeed;
				SetCanTurn(false);
			}
		}
	}

	public override void StartEnemyIdleFromDoor()
	{
		entityObject.layer = PhysicsLayer.ENEMY;
		GetFlyArea();
		entityTransform.position = new Vector3(entityTransform.position.x, mFloorHeight + 0.2f, entityTransform.position.z);
		CreateNavMeshAgent();
		if (base.IsMasterPlayer && mHasMission)
		{
			AssignMission();
		}
	}

	public override void StartGraveBorn()
	{
		entityTransform.Translate(Vector3.down * 2f);
		entityObject.layer = PhysicsLayer.DEFAULT;
		GetFlyArea();
	}

	public override void DoGraveBorn()
	{
		PlayAnimation(AnimationString.ENEMY_RUN, WrapMode.Loop);
		entityTransform.Translate(Vector3.up * Time.deltaTime * 10f);
		if (entityTransform.position.y >= mFloorHeight + 0.2f)
		{
			Vector3 position = entityTransform.position;
			position.y = mFloorHeight + 0.2f;
			entityTransform.position = position;
			SetState(Enemy.IDLE_STATE);
			StartEnemyIdleFromGrave();
		}
	}

	protected void GetFlyArea()
	{
		mFloorHeight = Global.FLOORHEIGHT;
		mRay = new Ray(entityTransform.position + Vector3.down * 3f, Vector3.up);
		if (Physics.Raycast(mRay, out mRaycastHit, 100f, 1 << PhysicsLayer.FLY_AREA))
		{
			mFloorHeight = mRaycastHit.point.y;
		}
		else
		{
			Debug.LogError("No Floor Up.");
		}
	}

	public override void StartDead()
	{
		base.StartDead();
		if (Random.value < 50f)
		{
			PlayAnimation("Death01", WrapMode.ClampForever);
		}
		else
		{
			PlayAnimation("Death02", WrapMode.ClampForever);
		}
		mRay = new Ray(GetPosition() + Vector3.up, Vector3.down);
		if (Physics.Raycast(mRay, out mRaycastHit, 100f, 1 << PhysicsLayer.FLOOR))
		{
			mFloorHeight = mRaycastHit.point.y;
		}
		else
		{
			mFloorHeight = -99999f;
		}
		mDownSpeed = Vector3.down * 5f;
	}

	public override void DoDead()
	{
		base.DoDead();
		if (entityObject.activeSelf && GetPosition().y > mFloorHeight)
		{
			mDownSpeed += Physics.gravity * Time.deltaTime;
			entityTransform.Translate(mDownSpeed * Time.deltaTime);
		}
	}

	public override void CheckHit()
	{
		if (base.IsMasterPlayer && IsAnimationPlayed(4f / 15f) && !attacked)
		{
			attacked = true;
			SpawnBullet();
		}
	}

	protected override void MakeDecisionInEnemyIdle()
	{
		if (!(GetAttackTimeDuration() > attackFrequency))
		{
			return;
		}
		Vector3 vector = GetPosition() - mTargetPosition;
		vector.y = 0f;
		float sqrMagnitude = vector.sqrMagnitude;
		if (sqrMagnitude <= base.AttackRangeSqr)
		{
			float num = ((!base.IsElite) ? BigBeeParameter.BigBee3ShotChance : BigBeeParameter.BigBeeElite3ShotChance);
			if (Random.value <= num)
			{
				SetState(BIGBEE_3_SHOT_STATE);
			}
			else
			{
				SetState(Enemy.ATTACK_STATE);
			}
		}
		else
		{
			SetState(Enemy.CATCHING_STATE);
		}
		SendStateChangeRequest(mConditionState);
	}

	protected override void MakeDecisionInCatching()
	{
		Vector3 vector = GetPosition() - mTargetPosition;
		vector.y = 0f;
		float sqrMagnitude = vector.sqrMagnitude;
		if (!(sqrMagnitude < base.AttackRangeSqr) || !canHitTarget())
		{
			return;
		}
		if (GetAttackTimeDuration() > attackFrequency)
		{
			SetState(Enemy.ATTACK_STATE);
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && GameApp.GetInstance().GetGameMode().IsCoopMode())
			{
				EnemyStateRequest request = new EnemyStateRequest(base.EnemyID, GetID(EEnemyStateID.ATTACK), entityTransform.position);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
		else
		{
			SetState(Enemy.IDLE_STATE);
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && GameApp.GetInstance().GetGameMode().IsCoopMode())
			{
				EnemyStateRequest request2 = new EnemyStateRequest(base.EnemyID, GetID(EEnemyStateID.IDLE), entityTransform.position);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
			}
		}
	}

	public void SpawnBullet()
	{
		if (base.IsMasterPlayer)
		{
			Vector3 vector = new Vector3(entityTransform.position.x, entityTransform.position.y + 0.8f, entityTransform.position.z);
			Vector3 vector2 = new Vector3(mTargetPosition.x, mTargetPosition.y + 1.2f, mTargetPosition.z) - vector;
			float num = 12f;
			Vector3 vector3 = vector2.normalized * num;
			GameObject original = Resources.Load(EffectName.E4_Ball) as GameObject;
			GameObject gameObject = Object.Instantiate(original, vector, Quaternion.LookRotation(-vector3)) as GameObject;
			EnemyShotScript component = gameObject.GetComponent<EnemyShotScript>();
			component.speed = vector3;
			component.attackDamage = attackDamage;
			component.areaDamage = ShotAreaDamage;
			component.trType = TrajectoryType.Straight;
			component.damageType = EnemyDamageType.Sputtering;
			component.explodeRadius = ShotExplodeRadius;
			component.enemyType = mEnemyType;
			component.attacker = this;
			component.PlayerDamageType = mPlayerDamageType;
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				EnemyShotRequest request = new EnemyShotRequest(6, vector, vector3, base.EnemyID);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
	}
}
