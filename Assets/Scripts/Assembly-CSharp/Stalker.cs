using UnityEngine;

public class Stalker : Enemy
{
	public static EnemyState JUMP_ATTACK_STATE = new StalkerJumpAttackState();

	public static EnemyState JUMP_CATCH_STATE = new StalkerJumpCatchState();

	private float[] mAttackChances = new float[3];

	public CreatureMeleeAttackSkill JumpAttackSkill;

	public Stalker()
	{
		base.GameUnitType = EGameUnitType.ENEMY;
		base.EnemyType = EnemyType.STALKER;
		IsNewState = true;
	}

	public override void Init(GameObject gObject)
	{
		base.Init(gObject);
		attackRange = 3f;
		SetShoutTimer("_Old/_Audio/enemy/xeiweichong", 5f);
		ConditionStateManager.GetInstance().InitConditionState(this);
		mAttackChances[0] = StalkerParameter.ATTACK_CHANCE1;
		mAttackChances[1] = StalkerParameter.ATTACK_CHANCE2;
		mAttackChances[2] = StalkerParameter.ATTACK_CHANCE3;
		MeleeAttackSkill = new CreatureMeleeAttackSkill();
		MeleeAttackSkill.CheckTime = new Vector2(0.4f, 0.5f);
		MeleeAttackSkill.Radius = 3f;
		MeleeAttackSkill.Damage = mAttackDamages[0];
		MeleeAttackSkill.Angle = 60f;
		MeleeAttackSkill.PlayerDamageType = mPlayerDamageType;
		JumpAttackSkill = new CreatureMeleeAttackSkill();
		JumpAttackSkill.CheckTime = new Vector2(0.6f, 0.7f);
		JumpAttackSkill.Radius = StalkerParameter.JUMP_ATTACK_RADIUS;
		JumpAttackSkill.Damage = mAttackDamages[1];
		JumpAttackSkill.Angle = 120f;
		JumpAttackSkill.PlayerDamageType = mPlayerDamageType;
	}

	public override void StartDead()
	{
		base.StartDead();
		if (Random.value < 50f)
		{
			PlayAnimation(AnimationString.ENEMY_DEAD, WrapMode.ClampForever);
		}
		else
		{
			PlayAnimation(AnimationString.ENEMY_DEAD01, WrapMode.ClampForever);
		}
	}

	public override void DoDead()
	{
		if (criticalAttacked)
		{
			Destroy();
		}
		else if (AnimationPlayed(AnimationString.ENEMY_DEAD01, 0.7f) || AnimationPlayed(AnimationString.ENEMY_DEAD, 0.7f))
		{
			SetState(Enemy.FADE_OUT_STATE);
		}
	}

	public override bool HandleMessage(EConditionMessage message)
	{
		if (base.HandleMessage(message))
		{
			return true;
		}
		if (message == EConditionMessage.ETryAttack)
		{
			switch (GameWorld.GetRandomIndex(mAttackChances))
			{
			case 0:
				SetState("ContinueCatch");
				SendStateChangeRequest(mConditionState);
				return true;
			case 1:
				SetState(JUMP_ATTACK_STATE);
				SendStateChangeRequest(mConditionState);
				return true;
			case 2:
				SetState(JUMP_CATCH_STATE);
				SendStateChangeRequest(mConditionState);
				return true;
			}
		}
		return false;
	}
}
