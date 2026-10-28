public class WinState : PlayerState
{
	public override void NextState(Player player, float deltaTime)
	{
		if (player.DoWin() && player.IsLocal())
		{
			if (null != MissionDetailsHUD.m_Instance)
			{
				MissionDetailsHUD.m_Instance.ClearMessage();
			}
			GameApp.GetInstance().GetGameWorld().SetGameState(GameState.GameOverUIWin);
			if (!AudioManager.GetInstance().IsPlayingMusic(AudioName.BGM_WIN))
			{
				AudioManager.GetInstance().PlayMusic(AudioName.BGM_WIN);
			}
		}
	}
}
