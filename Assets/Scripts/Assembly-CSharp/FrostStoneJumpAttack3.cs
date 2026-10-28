using UnityEngine;

public class FrostStoneJumpAttack3 : EnemyState
{
	public override void Start(Enemy enemy)
	{
		FrostStone frostStone = enemy as FrostStone;
		frostStone.PlayAnimation(AnimationString.ENEMY_JUMPING, WrapMode.ClampForever);
		frostStone.LookAtTargetHorizontal();
		GameObject gameObject = Pool.sAllocate("SW2_Effect/B3_Jump02_smoke", frostStone.GetPosition(), 2f);
		frostStone.PlaySound(AudioName.FROSTSTONE_JUMP02);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		enemy.MoveDelta(Vector3.down * Time.deltaTime);
	}

	public override void End(Enemy enemy)
	{
	}
}
