public class EnemySpecialAttack1State : EnemyState
{
	public override void Start(Enemy enemy)
	{
		EnemySpecial enemySpecial = enemy as EnemySpecial;
		if (enemySpecial != null)
		{
			enemySpecial.StartEnemySpecialAttack1();
		}
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		EnemySpecial enemySpecial = enemy as EnemySpecial;
		if (enemySpecial != null)
		{
			enemySpecial.DoEnemySpecialAttack1();
		}
	}

	public override void End(Enemy enemy)
	{
		EnemySpecial enemySpecial = enemy as EnemySpecial;
		if (enemySpecial != null)
		{
			enemySpecial.EndEnemySpecialAttack1();
		}
	}
}
