public class IdleState : PlayerState
{
	public override void Start(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		if (player.GetWeapon() != null)
		{
			player.GetWeapon().StartIdleState();
		}
	}

	public override void NextState(Player player, float deltaTime)
	{
		player.GetWeapon().UpdateIdleState(deltaTime);
	}
}
