using UnityEngine;

public class SlasherDashMoveState : EnemyState
{
	public override void Start(Enemy enemy)
	{
		Slasher slasher = (Slasher)enemy;
		enemy.PlayAnimation(AnimationString.BOSS_DASH02, WrapMode.Loop);
		enemy.MoveToward(enemy.GetTargetPosition(), SlasherParameter.DASH_SPEED[(int)slasher.GetMindState()]);
		enemy.PlaySoundLoop(AudioName.SLASHER_DASH02);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		Slasher slasher = (Slasher)enemy;
		slasher.CheckDashHit();
	}

	public override void End(Enemy enemy)
	{
		enemy.StopNavMesh();
		enemy.StopSound(AudioName.SLASHER_DASH02);
	}
}
