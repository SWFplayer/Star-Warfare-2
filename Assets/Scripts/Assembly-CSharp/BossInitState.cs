public class BossInitState : EnemyState
{
	public override void Start(Enemy enemy)
	{
		EnemyBoss enemyBoss = enemy as EnemyBoss;
		if (enemyBoss != null)
		{
			enemyBoss.StartBossInit();
		}
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		EnemyBoss enemyBoss = enemy as EnemyBoss;
		if (enemyBoss != null)
		{
			enemyBoss.DoBossInit();
		}
	}

	public override void End(Enemy enemy)
	{
		EnemyBoss enemyBoss = enemy as EnemyBoss;
		if (enemyBoss != null)
		{
			enemyBoss.EndBossInit();
		}
	}
}
