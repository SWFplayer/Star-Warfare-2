using UnityEngine;

public class FrostStoneMeleeAttackState1 : EnemyState
{
	public override void Start(Enemy enemy)
	{
		FrostStone frostStone = enemy as FrostStone;
		frostStone.PlayAnimation(AnimationString.BOSS_ATTACK03, WrapMode.ClampForever);
		frostStone.PlaySecondAnimation("attack03point");
		frostStone.ResetAttack(frostStone.mSkill.MeleeAttackSkill1);
		frostStone.LookAtTargetHorizontal();
		frostStone.MeleeAttackHasUsedNum++;
		frostStone.PlaySound(AudioName.FROSTSTONE_ATTACK01);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		FrostStone frostStone = (FrostStone)enemy;
		frostStone.CheckColliderHit(frostStone.mSkill.MeleeAttackSkill1);
		if (frostStone.IsAnimationTriggered(0.4f))
		{
			frostStone.SetCanTurn(false);
		}
	}

	public override void End(Enemy enemy)
	{
		FrostStone frostStone = (FrostStone)enemy;
		frostStone.StopSecondAnimation();
	}
}
