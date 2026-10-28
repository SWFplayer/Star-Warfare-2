public class PlayerDeadState : PlayerState
{
	public override void NextState(Player player, float deltaTime)
	{
		if (player.IsLocal())
		{
			if (!player.DeadAnimationCompleted())
			{
				return;
			}
			if (GameApp.GetInstance().GetGameMode().IsVSMode())
			{
				LocalPlayer localPlayer = player as LocalPlayer;
				if (localPlayer != null)
				{
					localPlayer.SendWaitVSRebirthRequest();
				}
				player.GetObject().SetActive(false);
			}
			else if (player.StartWaitRebirth())
			{
				player.SetState(Player.WAIT_REBIRTH_STATE);
			}
			else
			{
				player.OnMissionFail();
			}
		}
		else if (player.DeadAnimationCompleted() && GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			player.GetObject().SetActive(false);
		}
	}
}
