using UnityEngine;

public class BossRageState : EnemyState
{
	public override void Start(Enemy enemy)
	{
		EnemyBoss enemyBoss = enemy as EnemyBoss;
		if (enemyBoss != null)
		{
			enemyBoss.PlaySound(enemyBoss.RageSoundName);
		}
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		EnemyBoss enemyBoss = enemy as EnemyBoss;
		if (enemyBoss != null)
		{
			enemyBoss.PlayAnimation(AnimationString.ENEMY_RAGE, WrapMode.ClampForever);
		}
	}

	public override void End(Enemy enemy)
	{
		EnemyBoss enemyBoss = enemy as EnemyBoss;
		if (enemyBoss != null)
		{
			enemyBoss.StopSound(enemyBoss.RageSoundName);
		}
	}
}
