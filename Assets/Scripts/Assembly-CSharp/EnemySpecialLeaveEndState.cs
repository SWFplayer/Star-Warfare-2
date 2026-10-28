public class EnemySpecialLeaveEndState : EnemyState
{
	public override void Start(Enemy enemy)
	{
		EnemySpecial enemySpecial = enemy as EnemySpecial;
		if (enemySpecial != null)
		{
			enemySpecial.StartEnemySpecialLeaveEnd();
		}
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		EnemySpecial enemySpecial = enemy as EnemySpecial;
		if (enemySpecial != null)
		{
			enemySpecial.DoEnemySpecialLeaveEnd();
		}
	}

	public override void End(Enemy enemy)
	{
		EnemySpecial enemySpecial = enemy as EnemySpecial;
		if (enemySpecial != null)
		{
			enemySpecial.EndEnemySpecialLeaveEnd();
		}
	}
}
