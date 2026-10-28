public class EnemySpecialBornStartState : EnemyState
{
	public override void Start(Enemy enemy)
	{
		EnemySpecial enemySpecial = enemy as EnemySpecial;
		if (enemySpecial != null)
		{
			enemySpecial.StartEnemySpecialBornStart();
		}
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		EnemySpecial enemySpecial = enemy as EnemySpecial;
		if (enemySpecial != null)
		{
			enemySpecial.DoEnemySpecialBornStart();
		}
	}

	public override void End(Enemy enemy)
	{
		EnemySpecial enemySpecial = enemy as EnemySpecial;
		if (enemySpecial != null)
		{
			enemySpecial.EndEnemySpecialBornStart();
		}
	}
}
