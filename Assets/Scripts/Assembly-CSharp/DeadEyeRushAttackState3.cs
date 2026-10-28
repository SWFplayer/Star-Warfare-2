using UnityEngine;

public class DeadEyeRushAttackState3 : EnemyState
{
	public override void Start(Enemy enemy)
	{
		enemy.PlayAnimation(AnimationString.BOSS_DASH03, WrapMode.ClampForever);
		DeadEye deadEye = (DeadEye)enemy;
		float speed = deadEye.DashAttackParameter.DASH_SPEED1[(int)deadEye.GetMindState()];
		enemy.MoveToward(((DeadEye)enemy).DashTarget, speed);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		enemy.CheckHit2(((DeadEye)enemy).mSkill.DashAttackSkill);
		if (enemy.IsAnimationTriggered(0.22f))
		{
			enemy.PlaySound(AudioName.DEADEYE_LEAP_ATTACK);
		}
	}

	public override void End(Enemy enemy)
	{
		enemy.StopNavMesh();
	}
}
