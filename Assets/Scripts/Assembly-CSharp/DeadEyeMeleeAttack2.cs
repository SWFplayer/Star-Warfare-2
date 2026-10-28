using UnityEngine;

public class DeadEyeMeleeAttack2 : EnemyState
{
	public override void Start(Enemy enemy)
	{
		enemy.PlayAnimation(AnimationString.BOSS_ATTACK02, WrapMode.ClampForever);
		DeadEye deadEye = (DeadEye)enemy;
		deadEye.ResetAttack(deadEye.mSkill.MeleeAttackSkill1);
		enemy.LookAtTargetHorizontal();
		deadEye.MeleeAttackHasUsedNum++;
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		DeadEye deadEye = (DeadEye)enemy;
		deadEye.CheckColliderHit(deadEye.mSkill.MeleeAttackSkill1);
	}
}
