using UnityEngine;

public class Demster : Enemy
{
	public const float DashSpeed = 6f;

	public const float DashDuration = 4.5f;

	public const float EliteDashDuration = 10f;

	public const float ROTATE_DECREASE_DAMAGE = 0.2f;

	private const float RotateChance = 0.65f;

	private const float mRangeDistanceSqr = 576f;

	public static EnemyState DEMSTER_ROTATE_ATTACK_STATE = new DemsterRotateAttackState();

	public static EnemyState DEMSTER_ROTATE_ATTACK_FINISH_STATE = new EnemyPlayAnimState
	{
		AnimName = "attack_sp2_03",
		IsLookAtTarget = true
	};

	public CreatureMeleeAttackSkill DashAttackSkill;

	public EnemyBoss.BossDefend mBossDefend = new EnemyBoss.BossDefend();

	public Demster()
	{
		base.GameUnitType = EGameUnitType.ENEMY;
		base.EnemyType = EnemyType.DEMSTER;
		IsNewState = true;
	}

	public override void Init(GameObject gObject)
	{
		base.Init(gObject);
		SetShoutTimer("_Old/_Audio/enemy/luntaichong_juansuo", 3f);
		attackRange = 4.5f;
		bloodColor = BloodColor.Red;
		ConditionStateManager.GetInstance().InitConditionState(this);
		MeleeAttackSkill = new CreatureMeleeAttackSkill();
		MeleeAttackSkill.CheckTime = new Vector2(0.4f, 0.5f);
		MeleeAttackSkill.Radius = 5f;
		MeleeAttackSkill.Damage = attackDamage;
		MeleeAttackSkill.Angle = 60f;
		MeleeAttackSkill.PlayerDamageType = mPlayerDamageType;
		DashAttackSkill = new CreatureMeleeAttackSkill();
		DashAttackSkill.CheckTime = new Vector2(0f, 1f);
		DashAttackSkill.HitRadius = 3f;
		DashAttackSkill.Damage = attackDamage;
		DashAttackSkill.KnockDownSpeed = 0.1f;
		DashAttackSkill.PlayerDamageType = mPlayerDamageType;
		bipObject = entityObject.transform.Find(BoneName.ENEMY_BIP01).gameObject;
		mBossDefend.Init(this);
		mBossDefend.defendObj.transform.parent = entityObject.transform;
		mBossDefend.defendObj.transform.localPosition = new Vector3(0f, 0.8f, 0.3f);
		mBossDefend.defendObj.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
		if (entityTransform.Find("E6_Wind") == null)
		{
			Object original = Resources.Load(EffectName.E6_Wind);
			GameObject gameObject = Object.Instantiate(original, entityTransform.position, entityTransform.rotation) as GameObject;
			gameObject.transform.parent = entityTransform;
			gameObject.name = "E6_Wind";
			gameObject.SetActive(false);
		}
	}

	public override void StartDead()
	{
		base.StartDead();
		int num = Random.Range(0, 100);
		if (num < 50)
		{
			PlayAnimation("death1", WrapMode.ClampForever);
		}
		else
		{
			PlayAnimation("death2", WrapMode.ClampForever);
		}
	}

	public override void DoDead()
	{
		if (criticalAttacked)
		{
			Destroy();
		}
		else if (AnimationPlayed("death1", 0.7f) || AnimationPlayed("death2", 0.7f))
		{
			SetState(Enemy.FADE_OUT_STATE);
		}
	}

	protected override void MakeDecisionInEnemyIdle()
	{
		if (!(GetAttackTimeDuration() > attackFrequency) || !(Time.time - StateStartTime > 1f))
		{
			return;
		}
		float horizontalSqrDistanceFromTarget = GetHorizontalSqrDistanceFromTarget();
		if (horizontalSqrDistanceFromTarget <= base.AttackRangeSqr)
		{
			SetState(Enemy.ATTACK_STATE);
		}
		else if (horizontalSqrDistanceFromTarget <= 576f)
		{
			if (Random.value < 0.65f)
			{
				SetState(DEMSTER_ROTATE_ATTACK_STATE);
			}
			else
			{
				SetState(Enemy.CATCHING_STATE);
			}
		}
		else
		{
			SetState(Enemy.CATCHING_STATE);
		}
		SendStateChangeRequest(mConditionState);
	}

	public override void OnHit(DamageProperty dp)
	{
		base.OnHit(dp);
		if (NeedShowDefent())
		{
			mBossDefend.ShowDefend();
		}
	}

	public override void OnHitResponse(int newHp, int killerID, bool criticalAttack, WeaponType weaponType)
	{
		base.OnHitResponse(newHp, killerID, criticalAttack, weaponType);
		if (NeedShowDefent())
		{
			mBossDefend.ShowDefend();
		}
	}

	public override bool NeedShowDefent()
	{
		return mConditionState.mState == DEMSTER_ROTATE_ATTACK_STATE;
	}

	public override void ChangeToHitState()
	{
		if (!NeedShowDefent())
		{
			base.ChangeToHitState();
		}
	}

	public override bool inShield()
	{
		return NeedShowDefent();
	}
}
