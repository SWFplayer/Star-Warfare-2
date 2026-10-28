using System.Collections.Generic;
using UnityEngine;

internal class VSGameAutoBalanceResponse : Response
{
	protected int channelID;

	protected byte newSeatID;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		channelID = bytesBuffer.ReadInt();
		newSeatID = bytesBuffer.ReadByte();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null)
		{
			return;
		}
		Player localPlayer = gameWorld.GetLocalPlayer();
		int num = Lobby.GetInstance().GetChannelID();
		if (localPlayer == null)
		{
			return;
		}
		if (num == channelID)
		{
			if (newSeatID == localPlayer.GetSeatID())
			{
				return;
			}
			localPlayer.SetSeatID(newSeatID);
			if (localPlayer.Team != (TeamName)(localPlayer.GetSeatID() / 6))
			{
				UserStateHUD.GetInstance().InfoBox.SetChangeTeam(LocalizationManager.GetInstance().GetString("LOC_AUTOBALANCE"));
			}
			localPlayer.Team = (TeamName)(localPlayer.GetSeatID() / 6);
			localPlayer.CreatePlayerSign();
			List<RemotePlayer> remotePlayers = gameWorld.GetRemotePlayers();
			foreach (RemotePlayer item in remotePlayers)
			{
				if (item != null)
				{
					item.CreatePlayerSign();
				}
			}
			GameObject gameObject = GameObject.Find("GameUI");
			if (gameObject != null)
			{
				InGameUIScript component = gameObject.GetComponent<InGameUIScript>();
				if (component != null)
				{
					component.VSGameAutoBalance();
				}
			}
			if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.VS_KOTH)
			{
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
		}
		else
		{
			RemotePlayer remotePlayerByUserID = gameWorld.GetRemotePlayerByUserID(channelID);
			if (remotePlayerByUserID != null)
			{
				remotePlayerByUserID.SetSeatID(newSeatID);
				remotePlayerByUserID.Team = (TeamName)(remotePlayerByUserID.GetSeatID() / 6);
				remotePlayerByUserID.CreatePlayerSign();
			}
		}
	}
}
