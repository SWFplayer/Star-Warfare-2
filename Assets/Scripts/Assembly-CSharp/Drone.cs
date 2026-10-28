using UnityEngine;

public class Drone : Enemy
{
	protected Collider handCollider;

	protected Vector3 targetPosition;

	protected Vector3[] p = new Vector3[4];

	protected void RandomRunAnimation()
	{
		int num = Random.Range(0, 100);
		if (num < 40 || base.IsElite)
		{
			runAnimationName = AnimationString.ENEMY_RUN;
		}
		else if (num < 60)
		{
			runAnimationName = AnimationString.ENEMY_RUN01;
		}
		else
		{
			runAnimationName = AnimationString.ENEMY_RUN02;
		}
	}

	public override void StartDead()
	{
		base.StartDead();
		int num = Random.Range(0, 100);
		if (num < 50)
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

	public override void Init(GameObject gObject)
	{
		base.Init(gObject);
		SetShoutTimer(AudioName.ENEMY_AMBIENT1_DIGGER, 5f);
		attackRange = 3f;
		RandomRunAnimation();
		MeleeAttackSkill = new CreatureMeleeAttackSkill();
		MeleeAttackSkill.CheckTime = new Vector2(0.4f, 0.5f);
		MeleeAttackSkill.Radius = 4f;
		MeleeAttackSkill.Damage = attackDamage;
		MeleeAttackSkill.Angle = 60f;
		MeleeAttackSkill.PlayerDamageType = mPlayerDamageType;
	}
}
