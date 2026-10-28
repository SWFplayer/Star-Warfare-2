public class CerberliskThreeAttackPre : EnemyState
{
	public override void Start(Enemy enemy)
	{
		enemy.PlayAnimation("3hitpre");
		enemy.LookAtTargetHorizontal();
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		if (enemy.IsAnimationTriggered(0.38f) || enemy.IsAnimationTriggered(0.63f))
		{
			enemy.PlaySound(AudioName.CERBERLISK_3HIT_PRE);
		}
	}

	public override void End(Enemy enemy)
	{
	}
}
