using UnityEngine;

public class Tacorlion : Enemy
{
	public static int ShotAttackDamage;

	public static EnemyState TACORLION_RANGE_ATTACK_STATE = new TacorlionRangeAttackState();

	public float mMeleeAttackDistanceSqr;

	public float mRangeAttackDistanceSqr;

	public int RangeAttackNum;

	public Tacorlion()
	{
		base.GameUnitType = EGameUnitType.ENEMY;
		base.EnemyType = EnemyType.SCORPION;
		IsNewState = true;
	}

	public override void Init(GameObject gObject)
	{
		base.Init(gObject);
		SetShoutTimer("_Old/_Audio/enemy/xeiweichong", 5f);
		ConditionStateManager.GetInstance().InitConditionState(this);
		MeleeAttackSkill = new CreatureMeleeAttackSkill();
		MeleeAttackSkill.CheckTime = new Vector2(0.4f, 0.5f);
		MeleeAttackSkill.Radius = 3f;
		MeleeAttackSkill.Damage = attackDamage;
		MeleeAttackSkill.Angle = 60f;
		MeleeAttackSkill.PlayerDamageType = mPlayerDamageType;
		mMeleeAttackDistanceSqr = TacorlionParameter.MeleeAttackDistance * TacorlionParameter.MeleeAttackDistance;
		mRangeAttackDistanceSqr = TacorlionParameter.RangeAttackDistance * TacorlionParameter.RangeAttackDistance;
		attackRange = TacorlionParameter.MeleeAttackDistance;
		ShotAttackDamage = attackDamage;
	}

	public override bool HandleMessage(EConditionMessage message)
	{
		if (base.HandleMessage(message))
		{
			return true;
		}
		switch (message)
		{
		case EConditionMessage.EIncreaseNum:
			RangeAttackNum++;
			break;
		case EConditionMessage.EResetNum:
			RangeAttackNum = 0;
			break;
		}
		return false;
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

	public override void DoEnemyIdle()
	{
		PlayAnimation(AnimationString.ENEMY_IDLE, WrapMode.Loop);
		LookAtTargetHorizontal();
		if (!base.IsMasterPlayer || !CheckTargetInEnemyIdle() || !(GetAttackTimeDuration() > attackFrequency))
		{
			return;
		}
		float num = Vector3.SqrMagnitude(GetPosition() - mTargetPosition);
		if (RangeAttackNum > TacorlionParameter.MAX_RANGE_TIME && Random.value <= TacorlionParameter.MeleeAttackChance)
		{
			if (num <= mMeleeAttackDistanceSqr)
			{
				SetState(Enemy.ATTACK_STATE);
			}
			else
			{
				SetState("MeleeAttackCatch");
			}
		}
		else if (num <= mRangeAttackDistanceSqr)
		{
			SetState(TACORLION_RANGE_ATTACK_STATE);
		}
		else
		{
			SetState("RangeAttackCatch");
		}
		SendStateChangeRequest(mConditionState);
	}
}
