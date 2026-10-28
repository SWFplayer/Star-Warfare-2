public class EnemyFadeOut : EnemyState
{
	public override void Start(Enemy enemy)
	{
		enemy.StartFadeOut();
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		enemy.DoFadeOut();
	}

	public override void End(Enemy enemy)
	{
		enemy.EndFadeOut();
	}
}
