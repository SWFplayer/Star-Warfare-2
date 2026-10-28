using UnityEngine;

public class ConditionIdleState : EnemyState
{
	public override void Start(Enemy enemy)
	{
		enemy.StartEnemyIdle();
		enemy.PlayAnimation(AnimationString.ENEMY_IDLE, WrapMode.Loop);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		enemy.LookAtTargetHorizontal();
		if (enemy.IsMasterPlayer)
		{
			enemy.CheckTargetInEnemyIdle();
		}
	}

	public override void End(Enemy enemy)
	{
		enemy.EndEnemyIdle();
	}
}
