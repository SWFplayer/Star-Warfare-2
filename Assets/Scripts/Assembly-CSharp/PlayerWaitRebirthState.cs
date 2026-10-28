public class PlayerWaitRebirthState : PlayerState
{
	public override void NextState(Player player, float deltaTime)
	{
		if (player.IsLocal() && player.CheckLose() && !GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			player.OnMissionFail();
		}
	}
}
