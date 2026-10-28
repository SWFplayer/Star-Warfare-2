public class EnemySpecialTauntState : EnemyState
{
	public override void Start(Enemy enemy)
	{
		EnemySpecial enemySpecial = enemy as EnemySpecial;
		if (enemySpecial != null)
		{
			enemySpecial.StartEnemySpecialTaunt();
		}
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		EnemySpecial enemySpecial = enemy as EnemySpecial;
		if (enemySpecial != null)
		{
			enemySpecial.DoEnemySpecialTaunt();
		}
	}

	public override void End(Enemy enemy)
	{
		EnemySpecial enemySpecial = enemy as EnemySpecial;
		if (enemySpecial != null)
		{
			enemySpecial.EndEnemySpecialTaunt();
		}
	}
}
