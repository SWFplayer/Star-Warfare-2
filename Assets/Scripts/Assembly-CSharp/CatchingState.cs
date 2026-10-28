public class CatchingState : EnemyState
{
	public override void Start(Enemy enemy)
	{
		enemy.StartCatching();
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		enemy.DoCatching();
	}

	public override void End(Enemy enemy)
	{
		enemy.EndCatching();
	}
}
