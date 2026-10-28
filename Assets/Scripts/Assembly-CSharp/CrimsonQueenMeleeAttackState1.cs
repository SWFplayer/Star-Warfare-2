using UnityEngine;

public class CrimsonQueenMeleeAttackState1 : EnemyState
{
	public override void Start(Enemy enemy)
	{
		CrimsonQueen crimsonQueen = enemy as CrimsonQueen;
		crimsonQueen.PlayAnimation(AnimationString.BOSS_ATTACK01, WrapMode.ClampForever);
		crimsonQueen.ResetAttack(crimsonQueen.mSkill.MeleeAttackSkill1);
		crimsonQueen.LookAtTargetHorizontal();
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		CrimsonQueen crimsonQueen = enemy as CrimsonQueen;
		crimsonQueen.CheckColliderHit(crimsonQueen.mSkill.MeleeAttackSkill1);
		if (crimsonQueen.CurrentAnimTime >= CrimsonQueenParameter.MELEE_ATTACK_MOVE_FORWARD_TIME1.x && crimsonQueen.CurrentAnimTime <= CrimsonQueenParameter.MELEE_ATTACK_MOVE_FORWARD_TIME1.y)
		{
			crimsonQueen.SimpleMove(crimsonQueen.GetTransform().forward * CrimsonQueenParameter.MELEE_ATTACK_MOVE_FORWARD_SPEED1);
		}
		if (crimsonQueen.IsAnimationTriggered(0.37f))
		{
			GameObject gameObject = Pool.sAllocate("SW2_Effect/B5_attack01", enemy.GetPosition(), enemy.GetTransform().forward);
			gameObject.transform.parent = enemy.GetTransform();
			enemy.PlaySound(AudioName.CRIMSONQUEEN_ATTACK01);
		}
	}
}
