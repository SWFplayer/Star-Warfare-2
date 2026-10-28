public class GotHitState : EnemyState
{
	public override void Start(Enemy enemy)
	{
		enemy.StartGotHit();
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		enemy.DoGotHit();
	}

	public override void End(Enemy enemy)
	{
		enemy.EndGotHit();
	}
}
