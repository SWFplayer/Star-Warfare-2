public class EnemyAttackState : EnemyState
{
	public override void Start(Enemy enemy)
	{
		enemy.StartEnemyAttack();
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		enemy.DoEnemyAttack();
	}

	public override void End(Enemy enemy)
	{
		enemy.EndEnemyAttack();
	}
}
