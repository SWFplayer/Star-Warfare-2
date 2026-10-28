using UnityEngine;

public class DeadEyeMeleeAttack1 : EnemyState
{
	public override void Start(Enemy enemy)
	{
		DeadEye deadEye = (DeadEye)enemy;
		deadEye.PlayAnimation(AnimationString.BOSS_ATTACK01, WrapMode.ClampForever);
		deadEye.ResetAttack(deadEye.mSkill.MeleeAttackSkill);
		enemy.LookAtTargetHorizontal();
		deadEye.MeleeAttackHasUsedNum++;
		GameObject gameObject = Pool.sAllocate("SW2_Effect/B2_Attack01_Swing", enemy.GetPosition(), enemy.GetTransform().forward);
		gameObject.transform.parent = enemy.GetTransform();
		gameObject = Pool.sAllocate("SW2_Effect/B2_Attack01_smoke", deadEye.GetPosition() + deadEye.GetTransform().TransformDirection(-0.7f, 0f, 9f));
		gameObject.transform.parent = enemy.GetTransform();
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		DeadEye deadEye = (DeadEye)enemy;
		deadEye.CheckColliderHit(deadEye.mSkill.MeleeAttackSkill);
		if (deadEye.IsAnimationTriggered(0.25f))
		{
			deadEye.PlaySound(AudioName.DEADEYE_ATTACK01);
		}
	}
}
