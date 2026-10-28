using UnityEngine;

public class CerberliskMeleeAttack2 : EnemyState
{
	public override void Start(Enemy enemy)
	{
		Cerberlisk cerberlisk = enemy as Cerberlisk;
		cerberlisk.PlayAnimation(AnimationString.BOSS_ATTACK02, WrapMode.ClampForever);
		cerberlisk.ResetAttack(cerberlisk.mSkill.MeleeAttackSkill2);
		cerberlisk.LookAtTargetHorizontal();
		cerberlisk.PlaySound(AudioName.CERBERLISK_ATTACK02);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		Cerberlisk cerberlisk = enemy as Cerberlisk;
		cerberlisk.CheckColliderHit(cerberlisk.mSkill.MeleeAttackSkill2);
	}

	public override void End(Enemy enemy)
	{
	}
}
