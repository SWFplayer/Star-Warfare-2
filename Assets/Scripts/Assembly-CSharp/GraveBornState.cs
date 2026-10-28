public class GraveBornState : EnemyState
{
	public override void Start(Enemy enemy)
	{
		enemy.StartGraveBorn();
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		enemy.DoGraveBorn();
	}

	public override void End(Enemy enemy)
	{
		enemy.EndGraveBorn();
	}
}
