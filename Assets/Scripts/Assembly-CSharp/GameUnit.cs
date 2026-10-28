using System.Collections.Generic;
using UnityEngine;

public class GameUnit : GameEntity
{
	public class CountTimer
	{
		public Timer Timer;

		public int Count;

		public CountTimer(float interval)
		{
			Timer = new Timer(interval, true);
			Count = 0;
		}
	}

	public class SecondAnimation
	{
		public static Vector3 Down = new Vector3(0f, -0.1f, 0f);

		private GameUnit mOwner;

		public bool mEnable;

		public GameObject mSecondAnimation;

		public Vector3 mLastLocalPosition;

		public Vector3 mLastLocalRotation;

		public Vector3 mDeltaPosition;

		private string mCurrentAnim;

		public void Init(GameUnit owner, GameObject animationGameObject)
		{
			mOwner = owner;
			mSecondAnimation = animationGameObject;
		}

		public void Update()
		{
			if (mEnable)
			{
				mDeltaPosition = mSecondAnimation.transform.localPosition - mLastLocalPosition;
				mLastLocalPosition = mSecondAnimation.transform.localPosition;
				Vector3 delta = mOwner.GetTransform().TransformDirection(mDeltaPosition + Down);
				mOwner.MoveDelta(delta);
				if (mSecondAnimation.GetComponent<Animation>()[mCurrentAnim].normalizedTime >= 1f)
				{
					mEnable = false;
					Stop();
				}
			}
		}

		public void PlayAnimation(string anim)
		{
			if (mSecondAnimation != null)
			{
				mCurrentAnim = anim;
				mEnable = true;
				mSecondAnimation.GetComponent<Animation>().Play(anim);
				mSecondAnimation.GetComponent<Animation>()[anim].wrapMode = WrapMode.ClampForever;
				mSecondAnimation.GetComponent<Animation>()[anim].time = 0f;
				mSecondAnimation.GetComponent<Animation>().Sample();
				mLastLocalPosition = mSecondAnimation.transform.localPosition;
			}
		}

		public void Stop()
		{
			mEnable = false;
			mSecondAnimation.GetComponent<Animation>().Stop();
		}
	}

	protected Rigidbody rigidbody;

	protected Collider collider;

	protected CharacterController cc;

	protected Vector3 characterControllerCenter;

	protected string mDisplayName;

	protected float BasicSpeed;

	protected int BasicMaxHp;

	protected bool isSpeedDownEffectAdded;

	public float StateStartTime;

	public bool IsStateRotateFinish;

	public bool NeedUpdateTargetPosition = true;

	protected GameUnit mTarget;

	protected Vector3 mTargetPosition;

	protected UnityEngine.AI.NavMeshAgent mNavMeshAgent;

	protected float mLastUpdateNavMeshTime;

	protected bool mCanTurn;

	public Dictionary<BaseCompareCondition, BaseCompareData> CompareData = new Dictionary<BaseCompareCondition, BaseCompareData>();

	public Dictionary<GameUnitState, GameUnitStateData> StateData = new Dictionary<GameUnitState, GameUnitStateData>();

	protected Dictionary<GameUnit, CountTimer> mCheckHitList = new Dictionary<GameUnit, CountTimer>();

	public float LastAnimTime;

	public float CurrentAnimTime;

	private string mCurrentAnimationName;

	public GameObject Shadow;

	public float ShadowHeight = 0.2f;

	public SecondAnimation mSecondAnim;

	public EGameUnitType GameUnitType { get; set; }

	public virtual float Speed { get; set; }

	public virtual int Hp { get; set; }

	public virtual int MaxHp { get; set; }

	public virtual int ItemShield { get; set; }

	public virtual int MaxItemShield { get; set; }

	public string CurrentAnimationName
	{
		get
		{
			return mCurrentAnimationName;
		}
		set
		{
			if (mCurrentAnimationName != value)
			{
				CurrentAnimTime = -1f;
				LastAnimTime = -1f;
			}
			mCurrentAnimationName = value;
		}
	}

	public bool IsMasterPlayer
	{
		get
		{
			return Lobby.GetInstance().IsMasterPlayer || GameApp.GetInstance().GetGameMode().IsSingle();
		}
	}

	public virtual bool InPlayingState()
	{
		return false;
	}

	public void SetDisplayName(string name)
	{
		mDisplayName = name;
	}

	public string GetDisplayName()
	{
		return mDisplayName;
	}

	public virtual Collider GetCollider()
	{
		return collider;
	}

	public CharacterController GetCharacterController()
	{
		return cc;
	}

	public Vector3 GetCharacterControllerCenter()
	{
		return characterControllerCenter;
	}

	public virtual float GetHitColliderRadius()
	{
		return 0f;
	}

	public GameUnit GetTarget()
	{
		return mTarget;
	}

	public Vector3 GetTargetPosition()
	{
		return mTargetPosition;
	}

	public void SetTargetPosition(Vector3 pos)
	{
		mTargetPosition = pos;
	}

	public override void SetObject(GameObject obj)
	{
		base.SetObject(obj);
		collider = entityObject.GetComponent<Collider>();
		if (entityObject.GetComponent<CharacterController>() != null)
		{
			cc = entityObject.GetComponent<CharacterController>();
			characterControllerCenter = cc.center;
		}
	}

	public virtual void PlaySound(string name)
	{
		AudioManager.GetInstance().PlaySoundAt(name, entityObject, entityTransform.position, AudioRolloffMode.Linear, 50f);
	}

	public virtual void PlaySoundSingle(string name)
	{
		AudioManager.GetInstance().PlaySoundSingleAt(name, entityObject, entityTransform.position, AudioRolloffMode.Linear, 50f);
	}

	public virtual void PlaySoundLoop(string name)
	{
		AudioManager.GetInstance().PlaySoundLoop(name, entityObject, entityTransform);
	}

	public virtual void PlaySoundLoopSingle(string name)
	{
		AudioManager.GetInstance().PlaySoundSingleLoop(name, entityObject);
	}

	public virtual void PlayLogarithmicSound(string name)
	{
		AudioManager.GetInstance().PlaySoundAt(name, entityObject, entityTransform.position, AudioRolloffMode.Logarithmic, 1000f);
	}

	public virtual void PlayLogarithmicSingleSound(string name)
	{
		AudioManager.GetInstance().PlaySoundSingleAt(name, entityObject, entityTransform.position, AudioRolloffMode.Logarithmic, 1000f);
	}

	public virtual void StopSound(string name)
	{
		AudioManager.GetInstance().StopSound(name, entityObject);
	}

	public virtual bool HandleMessage(EConditionMessage message)
	{
		if (message == EConditionMessage.EStopMove)
		{
			StopNavMesh();
		}
		return false;
	}

	public virtual void OnHit(DamageProperty dp)
	{
	}

	public virtual void OnHitTarget(GameUnit target, CreatureSkill skill, int Count)
	{
	}

	public bool IsAnimationFinish()
	{
		return AnimationPlayed(CurrentAnimationName, 1f);
	}

	public bool IsAnimationPlayed(float time)
	{
		return AnimationPlayed(CurrentAnimationName, time);
	}

	public bool IsAnimationTriggered(float time)
	{
		if (LastAnimTime < time && CurrentAnimTime >= time)
		{
			return true;
		}
		if (time <= CurrentAnimTime && CurrentAnimTime < LastAnimTime)
		{
			return true;
		}
		if (CurrentAnimTime < LastAnimTime && LastAnimTime <= time)
		{
			return true;
		}
		return false;
	}

	public virtual void CheckHit2(CreatureMeleeAttackSkill skill)
	{
		foreach (KeyValuePair<GameUnit, CountTimer> mCheckHit in mCheckHitList)
		{
			GameUnit key = mCheckHit.Key;
			if (key == null || !key.InPlayingState() || !mCheckHit.Value.Timer.Ready() || !(skill.CheckTime.x <= CurrentAnimTime) || !(CurrentAnimTime <= skill.CheckTime.y))
			{
				continue;
			}
			Vector3 direction = key.GetTransform().position - entityTransform.position;
			direction -= direction.normalized * key.GetHitColliderRadius();
			Vector3 vector = entityTransform.InverseTransformDirection(direction);
			float sqrMagnitude = vector.sqrMagnitude;
			if (sqrMagnitude <= skill.HitRadiusSqr || (sqrMagnitude <= skill.RadiusSqr && vector.z > 0f && Mathf.Abs(vector.z / vector.x) > Mathf.Tan(skill.AngleRad)))
			{
				if (skill.KnockDownSpeed > 0f && skill.KnockDownCheckTime.x <= CurrentAnimTime && CurrentAnimTime <= skill.KnockDownCheckTime.y)
				{
					CheckKnocked(skill.KnockDownSpeed, key);
				}
				OnHitTarget(key, skill, mCheckHit.Value.Count);
				mCheckHit.Value.Count++;
				mCheckHit.Value.Timer.Do();
			}
		}
	}

	public virtual void CheckColliderHit(CreatureMeleeAttackSkill skill)
	{
		foreach (KeyValuePair<GameUnit, CountTimer> mCheckHit in mCheckHitList)
		{
			GameUnit key = mCheckHit.Key;
			if (key == null || !key.InPlayingState() || !mCheckHit.Value.Timer.Ready() || !(skill.CheckTime.x <= CurrentAnimTime) || !(CurrentAnimTime <= skill.CheckTime.y))
			{
				continue;
			}
			Collider[] colliders = skill.Colliders;
			foreach (Collider collider in colliders)
			{
				if (collider.bounds.Intersects(key.GetCollider().bounds))
				{
					if (skill.KnockDownSpeed > 0f && skill.KnockDownCheckTime.x <= CurrentAnimTime && CurrentAnimTime <= skill.KnockDownCheckTime.y)
					{
						CheckKnocked(skill.KnockDownSpeed, key);
					}
					OnHitTarget(key, skill, mCheckHit.Value.Count);
					mCheckHit.Value.Count++;
					mCheckHit.Value.Timer.Do();
					break;
				}
			}
		}
	}

	public virtual void CheckKnocked(float speed)
	{
	}

	public virtual void CheckKnocked(float speed, GameUnit target)
	{
	}

	public bool HasHitEnemy()
	{
		foreach (CountTimer value in mCheckHitList.Values)
		{
			if (value.Count >= 1)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsOnGround()
	{
		return cc.isGrounded;
	}

	protected virtual void CreateShadow(float size)
	{
		GameObject original = Resources.Load("_Old/_Effect/Shadow") as GameObject;
		Shadow = Object.Instantiate(original) as GameObject;
		Shadow.transform.localScale = new Vector3(size, size, size);
		Shadow.transform.parent = entityObject.transform;
		Shadow.transform.localPosition = new Vector3(0f, ShadowHeight, 0f);
	}

	public virtual void ResetAttack(CreatureSkill skill)
	{
	}

	public void Move(Vector3 motion)
	{
		cc.Move(motion * Speed * Time.deltaTime);
	}

	public void SimpleMove(Vector3 speed)
	{
		cc.Move(speed * Time.deltaTime);
	}

	public void SimpleMoveWithGravity(Vector3 speed)
	{
		cc.Move((speed + Vector3.down * 15f) * Time.deltaTime);
	}

	public void SimpleMoveWithGravity(Vector3 speed, float gravity)
	{
		cc.Move((speed + Vector3.down * gravity) * Time.deltaTime);
	}

	public void MoveDelta(Vector3 delta)
	{
		cc.Move(delta);
	}

	public virtual bool UnitCanBeHitByRay(Vector3 position, float distance)
	{
		return true;
	}

	public virtual void CreateNavMeshAgent()
	{
		mNavMeshAgent = entityObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
		if (mNavMeshAgent == null)
		{
			mNavMeshAgent = entityObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
			mNavMeshAgent.speed = 10f;
			mNavMeshAgent.angularSpeed = 720f;
			mNavMeshAgent.acceleration = 100000f;
			mNavMeshAgent.baseOffset = -0.1f;
			mNavMeshAgent.walkableMask = (1 << NavMeshLayer.NORMAL_GROUND) | (1 << NavMeshLayer.JUMP);
			mNavMeshAgent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.LowQualityObstacleAvoidance;
			mNavMeshAgent.stoppingDistance = 0.5f;
		}
		StopNavMesh();
	}

	public virtual void UpdateNavMeshInCatching(bool force)
	{
	}

	public virtual void StopNavMesh()
	{
		if (null != mNavMeshAgent && mNavMeshAgent.enabled)
		{
			mNavMeshAgent.Stop(true);
			mNavMeshAgent.enabled = false;
		}
	}

	public void DestroyNavMeshAgent()
	{
		if (mNavMeshAgent != null)
		{
			Object.Destroy(mNavMeshAgent);
			mNavMeshAgent = null;
		}
	}

	public void SetCanTurn(bool canTurn)
	{
		mCanTurn = canTurn;
		if (null != mNavMeshAgent)
		{
			mNavMeshAgent.updateRotation = !canTurn;
		}
	}

	public bool IsHardMode()
	{
		return GameApp.GetInstance().GetUserState().GetSelectedDifficultMode() == EDifficultMode.HEROIC;
	}

	public void UpdateHitBySniper(Player player)
	{
		List<Weapon> battleWeapons = player.GetUserState().GetBattleWeapons();
		UltraSniper ultraSniper = null;
		foreach (Weapon item in battleWeapons)
		{
			if (item.GetWeaponType() == WeaponType.UltraSniper)
			{
				ultraSniper = item as UltraSniper;
			}
		}
		if (ultraSniper != null && entityTransform != null)
		{
			ultraSniper.ExplodeAt(entityTransform.position + Vector3.up);
		}
	}

	public void PlaySecondAnimation(string anim)
	{
		mSecondAnim.PlayAnimation(anim);
	}

	public void StopSecondAnimation()
	{
		mSecondAnim.Stop();
	}

	public Vector3 GetDeltaPosition()
	{
		return mSecondAnim.mDeltaPosition;
	}
}
