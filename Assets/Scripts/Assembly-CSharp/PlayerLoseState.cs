using UnityEngine;

public class PlayerLoseState : PlayerState
{
	public override void Start(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		Weapon weapon = player.GetWeapon();
		if (weapon != null)
		{
			weapon.StopFire();
		}
	}

	public override void NextState(Player player, float deltaTime)
	{
		if (player.DoVSLose() && player.IsLocal())
		{
			if (GameApp.GetInstance().GetGameMode().IsSingle() && !player.IsFromDead && !player.PortalEndPlayed)
			{
				EffectPool.GetInstance().GetEffectPool(EffectPoolType.RESPAWN_1).CreateObject(player.GetPosition(), player.GetTransform().forward, Quaternion.identity);
				EffectPool.GetInstance().GetEffectPool(EffectPoolType.PORTAL_OFF).CreateObject(player.GetPosition(), player.GetTransform().forward, Quaternion.identity);
				player.PlayerInvisible();
				player.PortalEndPlayed = true;
			}
			if (MissionDetailsHUD.m_Instance != null)
			{
				MissionDetailsHUD.m_Instance.ClearMessage();
			}
			GameApp.GetInstance().GetGameWorld().SetGameState(GameState.GameOverUILose);
			if (!AudioManager.GetInstance().IsPlayingMusic(AudioName.BGM_LOSE))
			{
				AudioManager.GetInstance().PlayMusic(AudioName.BGM_LOSE);
			}
		}
	}
}
