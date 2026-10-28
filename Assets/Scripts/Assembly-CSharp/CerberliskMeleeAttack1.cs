using UnityEngine;

public class CerberliskMeleeAttack1 : EnemyState
{
	public override void Start(Enemy enemy)
	{
		Cerberlisk cerberlisk = enemy as Cerberlisk;
		cerberlisk.PlayAnimation(AnimationString.BOSS_ATTACK01, WrapMode.ClampForever);
		cerberlisk.ResetAttack(cerberlisk.mSkill.MeleeAttackSkill1);
		cerberlisk.LookAtTargetHorizontal();
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		Cerberlisk cerberlisk = enemy as Cerberlisk;
		cerberlisk.CheckColliderHit(cerberlisk.mSkill.MeleeAttackSkill1);
		if (cerberlisk.IsAnimationTriggered(0.2f))
		{
			cerberlisk.PlaySound(AudioName.CERBERLISK_ATTACK01);
		}
	}

	public override void End(Enemy enemy)
	{
	}
}
