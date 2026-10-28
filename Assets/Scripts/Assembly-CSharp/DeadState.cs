public class DeadState : EnemyState
{
	public override void Start(Enemy enemy)
	{
		enemy.StartDead();
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		enemy.DoDead();
	}

	public override void End(Enemy enemy)
	{
		enemy.EndDead();
	}
}
