using UnityEngine;

public class Tank : Enemy
{
	public static EnemyState TANK_DASH_STATE = new TankDashState();

	public static EnemyState TANK_SHOCK_ATTACK_STATE = new ShockAttackState();

	public float DashSpeed;

	protected bool rushCollidesWall;

	private float mLastGotHitStateTime;

	public CreatureMeleeAttackSkill ShockAttackSkill = new CreatureMeleeAttackSkill();

	public CreatureMeleeAttackSkill DashAttackSkill = new CreatureMeleeAttackSkill();

	private float attackRange1Sqr;

	private float[] mAttackChances = new float[2];

	private float mLastDashTime;

	public Tank()
	{
		base.GameUnitType = EGameUnitType.ENEMY;
		base.EnemyType = EnemyType.TANK;
		IsNewState = true;
	}

	public override void Init(GameObject gObject)
	{
		base.Init(gObject);
		attackRange = TankParameter.Distance1;
		attackRange1Sqr = TankParameter.Distance2 * TankParameter.Distance2;
		DashSpeed = TankParameter.DashSpeed;
		SetShoutTimer("_Old/_Audio/enemy/daxingjiachong", 5f);
		mLastGotHitStateTime = Time.time;
		mAttackChances[0] = TankParameter.MeleeAttackChance;
		mAttackChances[1] = TankParameter.ShockAttackChance;
		MeleeAttackSkill = new CreatureMeleeAttackSkill();
		MeleeAttackSkill.CheckTime = new Vector2(0.4f, 0.5f);
		MeleeAttackSkill.Radius = 6f;
		MeleeAttackSkill.Damage = mAttackDamages[0];
		MeleeAttackSkill.Angle = 60f;
		MeleeAttackSkill.KnockDownSpeed = TankParameter.MeleeAttackKnockDownSpeed;
		MeleeAttackSkill.PlayerDamageType = mPlayerDamageType;
		ShockAttackSkill.CheckTime = new Vector2(0.3f, 0.4f);
		ShockAttackSkill.HitRadius = TankParameter.ShockAttackRadius;
		ShockAttackSkill.Damage = mAttackDamages[1];
		ShockAttackSkill.KnockDownSpeed = TankParameter.ShockAttackKnockDownSpeed;
		ShockAttackSkill.PlayerDamageType = mPlayerDamageType;
		DashAttackSkill.CheckTime = new Vector2(0f, 1f);
		DashAttackSkill.HitRadius = TankParameter.DashAttackRadius;
		DashAttackSkill.Damage = mAttackDamages[2];
		DashAttackSkill.KnockDownSpeed = TankParameter.DashAttackKnockDownSpeed;
		DashAttackSkill.PlayerDamageType = mPlayerDamageType;
		ConditionStateManager.GetInstance().InitConditionState(this);
	}

	public override void StartDead()
	{
		base.StartDead();
		if (Random.value < 0.5f)
		{
			PlayAnimation("death1", WrapMode.ClampForever);
		}
		else
		{
			PlayAnimation("death2", WrapMode.ClampForever);
		}
	}

	public void RushCollidesWall()
	{
		rushCollidesWall = true;
	}

	public override void ChangeToHitState()
	{
		if (mState != TANK_DASH_STATE && Time.time - mLastGotHitStateTime > TankParameter.GotHitStateInterval)
		{
			mLastGotHitStateTime = Time.time;
			SetState(Enemy.GOTHIT_STATE);
		}
	}

	public override void SetCriticalAttack(bool criticalAttack)
	{
		base.SetCriticalAttack(false);
	}

	protected override void MakeDecisionInEnemyIdle()
	{
		if (!(GetIdleTimeDuration() > attackFrequency))
		{
			return;
		}
		float sqrMagnitude = (mTargetPosition - entityTransform.position).sqrMagnitude;
		if (sqrMagnitude > attackRange1Sqr && Time.time - mLastDashTime > TankParameter.DashInterval)
		{
			SetState(TANK_DASH_STATE);
			mLastDashTime = Time.time;
		}
		else if (sqrMagnitude >= base.AttackRangeSqr)
		{
			SetState(Enemy.CATCHING_STATE);
		}
		else
		{
			switch (GameWorld.GetRandomIndex(mAttackChances))
			{
			case 0:
				SetState(Enemy.ATTACK_STATE);
				break;
			case 1:
				SetState(TANK_SHOCK_ATTACK_STATE);
				break;
			}
		}
		SendStateChangeRequest(mConditionState);
	}

	protected override void MakeDecisionInCatching()
	{
		if (base.SqrDistanceFromPlayer < base.AttackRange * base.AttackRange && canHitTarget())
		{
			if (GetAttackTimeDuration() > attackFrequency)
			{
				switch (GameWorld.GetRandomIndex(mAttackChances))
				{
				case 0:
					SetState(Enemy.ATTACK_STATE);
					break;
				case 1:
					SetState(TANK_SHOCK_ATTACK_STATE);
					break;
				}
			}
			else
			{
				SetState(Enemy.IDLE_STATE);
			}
			SendStateChangeRequest(mConditionState);
		}
		if (Time.time - StateStartTime > TankParameter.CatchToDashTime)
		{
			SetState(TANK_DASH_STATE);
			mLastDashTime = Time.time;
			SendStateChangeRequest(mConditionState);
		}
	}

	public override void StartEnemyIdleFromDoor()
	{
		base.StartEnemyIdleFromDoor();
		entityObject.layer = PhysicsLayer.BOSS;
	}

	protected override void StartEnemyIdleFromGrave()
	{
		base.StartEnemyIdleFromGrave();
		entityObject.layer = PhysicsLayer.BOSS;
	}

	public override float GetHitColliderRadius()
	{
		return 2f;
	}
}
