using UnityEngine;

public class FrostStoneMeleeAttackState2 : EnemyState
{
	public override void Start(Enemy enemy)
	{
		FrostStone frostStone = enemy as FrostStone;
		frostStone.PlayAnimation(AnimationString.BOSS_ATTACK02, WrapMode.ClampForever);
		frostStone.ResetAttack(frostStone.mSkill.MeleeAttackSkill2);
		frostStone.MeleeAttackHasUsedNum++;
		frostStone.LookAtTargetHorizontal();
		frostStone.PlaySound(AudioName.FROSTSTONE_ATTACK02);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		FrostStone frostStone = (FrostStone)enemy;
		frostStone.CheckColliderHit(frostStone.mSkill.MeleeAttackSkill2);
	}

	public override void End(Enemy enemy)
	{
	}
}
