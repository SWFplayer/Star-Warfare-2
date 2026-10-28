using System.Collections.Generic;
using UnityEngine;

public class ProtectedItem : GameUnit
{
	public static ProtectedItemIdleState IDLE_STATE = new ProtectedItemIdleState();

	public static ProtectedItemRunState RUN_STATE = new ProtectedItemRunState();

	public static ProtectedItemDeadState DEAD_STATE = new ProtectedItemDeadState();

	public Mission OwnerMission;

	public EProtectionItemType ItemType;

	private ProtectedItemState mState;

	private new UnityEngine.AI.NavMeshAgent mNavMeshAgent;

	private int mCurrentPathPointIndex;

	private Timer mRunChecker = new Timer();

	private ProtectedItemHpScript mHpScript;

	private bool mIsLeader;

	private float IdleStartTime;

	public Vector3 SpawnPosition { get; set; }

	public Quaternion spawnRotation { get; set; }

	public string PrefabName { get; set; }

	public bool CanMove { get; set; }

	public List<float> SpeedValues { get; set; }

	public List<float> WaitTimes { get; set; }

	public List<GameObject> PathPoints { get; set; }

	public bool Arrived { get; set; }

	public bool Destroyed { get; set; }

	public bool IsLeader
	{
		get
		{
			return mIsLeader;
		}
		set
		{
			mIsLeader = value;
		}
	}

	public void Init()
	{
		GameObject original = Resources.Load(PrefabName) as GameObject;
		entityObject = Object.Instantiate(original, SpawnPosition, spawnRotation) as GameObject;
		entityObject.name = base.Name;
		entityTransform = entityObject.transform;
		collider = entityObject.transform.GetComponent<Collider>();
		Arrived = false;
		Destroyed = false;
		mIsLeader = false;
		mHpScript = entityObject.GetComponent<ProtectedItemHpScript>();
		Hp = MaxHp;
		mNavMeshAgent = entityObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
		mRunChecker.SetTimer(1f, false);
		StartIdle();
	}

	public void Update()
	{
		mState.Update(this, Time.deltaTime);
	}

	public void SetState(ProtectedItemState newState)
	{
		mState = newState;
	}

	public override bool InPlayingState()
	{
		return mState != DEAD_STATE && entityObject != null;
	}

	public override void OnHit(DamageProperty dp)
	{
		int damage = dp.damage;
		if (InPlayingState())
		{
			Hp -= damage;
			Hp = Mathf.Clamp(Hp, 0, MaxHp);
			MissionManager.GetInstance().ProtectedItemHpPercentage = Mathf.Clamp(100f * (float)Hp / (float)MaxHp, 0f, MissionManager.GetInstance().ProtectedItemHpPercentage);
			mHpScript.SetHpFillAmount((float)Hp / (float)MaxHp);
			if (Hp <= 0)
			{
				StartDead();
			}
		}
	}

	public void EndCurrentState()
	{
		if (mState == IDLE_STATE)
		{
			EndIdle();
		}
		else if (mState == RUN_STATE)
		{
			EndRun();
		}
		else if (mState == DEAD_STATE)
		{
			EndDead();
		}
	}

	public void StartIdle()
	{
		SetState(IDLE_STATE);
		IdleStartTime = Time.time;
	}

	public void DoIdle()
	{
		if (!Arrived && CanMove && Time.time - IdleStartTime >= WaitTimes[mCurrentPathPointIndex])
		{
			mCurrentPathPointIndex++;
			if (mCurrentPathPointIndex > PathPoints.Count)
			{
				Arrived = true;
			}
			else
			{
				StartRun();
			}
		}
	}

	private void EndIdle()
	{
	}

	public void StartRun()
	{
		SetState(RUN_STATE);
		if (null != mNavMeshAgent)
		{
			mNavMeshAgent.enabled = true;
			mNavMeshAgent.SetDestination(PathPoints[mCurrentPathPointIndex - 1].transform.position);
			mNavMeshAgent.speed = SpeedValues[mCurrentPathPointIndex - 1];
		}
	}

	public void DoRun()
	{
		if (mRunChecker.Ready())
		{
			mRunChecker.Do();
			if ((entityTransform.position - PathPoints[mCurrentPathPointIndex - 1].transform.position).sqrMagnitude < 1f)
			{
				if (mIsLeader)
				{
					OwnerMission.TriggerAction();
				}
				StartIdle();
			}
		}
		if (null != mNavMeshAgent)
		{
			entityTransform.LookAt(entityTransform.position + new Vector3(entityTransform.forward.x, mNavMeshAgent.velocity.normalized.y, entityTransform.forward.z));
		}
	}

	private void EndRun()
	{
		if (null != mNavMeshAgent && mNavMeshAgent.enabled)
		{
			mNavMeshAgent.Stop(true);
			mNavMeshAgent.enabled = false;
		}
	}

	public void StartDead()
	{
		EndCurrentState();
		SetState(DEAD_STATE);
		Destroyed = true;
	}

	public void DoDead()
	{
		EndDead();
	}

	private void EndDead()
	{
		Hide();
	}

	public override float GetHitColliderRadius()
	{
		if (ItemType == EProtectionItemType.TANK)
		{
			return 4f;
		}
		return 0f;
	}

	public float GetCurrentIndexPercent()
	{
		return (float)(mCurrentPathPointIndex - 1) / (float)PathPoints.Count;
	}
}
