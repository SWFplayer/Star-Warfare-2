public class PlayerVSInitLoadoutState : PlayerState
{
	public override void Start(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		player.GetObject().SetActive(false);
	}

	public override void NextState(Player player, float deltaTime)
	{
	}

	public override void End(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		player.GetObject().SetActive(true);
	}
}
