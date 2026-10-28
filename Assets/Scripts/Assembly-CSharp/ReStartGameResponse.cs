using System.Collections.Generic;
using UnityEngine;

internal class ReStartGameResponse : Response
{
	public override void ReadData(byte[] data)
	{
	}

	public override void ProcessLogic()
	{
		Lobby.GetInstance().GetVSClock().Restart();
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null)
		{
			return;
		}
		Player localPlayer = gameWorld.GetLocalPlayer();
		if (localPlayer == null)
		{
			return;
		}
		localPlayer.SetChargeWeaponStoreChargeValue(0);
		localPlayer.VSStatistics.ClearAll();
		gameWorld.BattleInfo.ClearAll();
		localPlayer.GetUserState().Achievement.ResetPVPState();
		if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.VS_KOTH)
		{
			gameWorld.VsBattleLogic.ClearAll();
			if (KOHHUD.mInstance != null)
			{
				KOHHUD.mInstance.Reset();
			}
		}
		else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.VS_TDM)
		{
			if (TDMHUD.mInstance != null)
			{
				TDMHUD.mInstance.Reset();
			}
		}
		else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Domination)
		{
			gameWorld.VsBattleLogic.ClearAll();
			if (DominationHUD.mInstance != null)
			{
				DominationHUD.mInstance.Reset();
			}
		}
		else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.VS_CTF_TDM)
		{
			if (CTFHUD.mInstance != null)
			{
				CTFHUD.mInstance.Reset();
			}
		}
		else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.VS_FFA)
		{
			if (FFAHUD.mInstance != null)
			{
				FFAHUD.mInstance.Reset();
			}
		}
		else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.GIANT_BETRAYAL && MutantHUD.mInstance != null)
		{
			MutantHUD.mInstance.Reset();
		}
		List<RemotePlayer> remotePlayers = gameWorld.GetRemotePlayers();
		foreach (RemotePlayer item in remotePlayers)
		{
			if (item != null)
			{
				item.VSStatistics.ClearAll();
				item.SetChargeWeaponStoreChargeValue(0);
			}
		}
		byte seatID = localPlayer.GetSeatID();
		if (GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			string text = ObjectNamePrefix.PLAYER_SPAWN_POINT + localPlayer.GetSeatID();
			if (GameApp.GetInstance().GetGameMode().IsTeamMode())
			{
				text = ObjectNamePrefix.TEAM_SPAWN_POINT + localPlayer.GetSeatID();
			}
			GameObject[] array = GameObject.FindGameObjectsWithTag(TagName.RESPAWN);
			for (byte b = 0; b < array.Length; b++)
			{
				if (text == array[b].name)
				{
					seatID = b;
					break;
				}
			}
		}
		if (PVPResultPageUI.mInstance != null)
		{
			PVPResultPageUI.mInstance.RestartGame();
		}
		if (!GameApp.GetInstance().GetGameMode().IsUsePvpCustomizeMode())
		{
			return;
		}
		localPlayer.SetState(Player.VS_INIT_LOADOUT_STATE);
		foreach (RemotePlayer item2 in remotePlayers)
		{
			if (item2 != null)
			{
				item2.SetState(Player.VS_INIT_LOADOUT_STATE);
			}
		}
	}
}
