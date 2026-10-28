using UnityEngine;

public class CrimsonQueenMeleeAttackState2 : EnemyState
{
	public override void Start(Enemy enemy)
	{
		CrimsonQueen crimsonQueen = enemy as CrimsonQueen;
		crimsonQueen.PlayAnimation(AnimationString.BOSS_ATTACK02, WrapMode.ClampForever);
		crimsonQueen.ResetAttack(crimsonQueen.mSkill.MeleeAttackSkill2);
		crimsonQueen.LookAtTargetHorizontal();
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		CrimsonQueen crimsonQueen = enemy as CrimsonQueen;
		crimsonQueen.CheckColliderHit(crimsonQueen.mSkill.MeleeAttackSkill2);
		if (crimsonQueen.CurrentAnimTime >= CrimsonQueenParameter.MELEE_ATTACK_MOVE_FORWARD_TIME2.x && crimsonQueen.CurrentAnimTime <= CrimsonQueenParameter.MELEE_ATTACK_MOVE_FORWARD_TIME2.y)
		{
			crimsonQueen.SimpleMove(crimsonQueen.GetTransform().forward * CrimsonQueenParameter.MELEE_ATTACK_MOVE_FORWARD_SPEED2);
		}
		if (crimsonQueen.IsAnimationTriggered(0.37f))
		{
			GameObject gameObject = Pool.sAllocate("SW2_Effect/B5_attack02", enemy.GetPosition(), enemy.GetTransform().forward);
			gameObject.transform.parent = enemy.GetTransform();
			enemy.PlaySound(AudioName.CRIMSONQUEEN_ATTACK02);
		}
	}
}
