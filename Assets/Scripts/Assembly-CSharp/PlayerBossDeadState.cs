public class PlayerBossDeadState : PlayerBossState
{
	public override void Start(GameUnit gameUnit)
	{
		Player player = (Player)gameUnit;
	}

	public override void Update(GameUnit gameUnit)
	{
		Player player = (Player)gameUnit;
		if (player.IsLocal() && !player.DeadAnimationCompleted())
		{
		}
	}

	public override void End(GameUnit gameUnit)
	{
	}
}
