public class EnemyIdleState : EnemyState
{
	public override void Start(Enemy enemy)
	{
		enemy.StartEnemyIdle();
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		enemy.DoEnemyIdle();
	}

	public override void End(Enemy enemy)
	{
		enemy.EndEnemyIdle();
	}
}
