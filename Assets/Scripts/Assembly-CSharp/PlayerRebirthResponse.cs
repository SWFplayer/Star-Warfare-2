using UnityEngine;

internal class PlayerRebirthResponse : Response
{
	protected int playerID;

	protected byte spawnPointIndex;

	protected byte pvpLoadoutID;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		playerID = bytesBuffer.ReadInt();
		spawnPointIndex = bytesBuffer.ReadByte();
		pvpLoadoutID = bytesBuffer.ReadByte();
	}

	public override void ProcessLogic()
	{
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
		int channelID = Lobby.GetInstance().GetChannelID();
		if (playerID == channelID)
		{
			if (GameApp.GetInstance().GetGameMode().IsVSMode())
			{
				if (GameApp.GetInstance().GetGameMode().IsUsePvpCustomizeMode())
				{
					localPlayer.GetUserState().LoadCurrentCustomize(false, pvpLoadoutID);
					localPlayer.InitSkills();
				}
				else
				{
					localPlayer.GetUserState().LoadCurrentCustomize(true, 0);
					localPlayer.InitSkills();
				}
				if (((LocalPlayer)localPlayer).DeadWithoutKillNum >= 3)
				{
					localPlayer.GetSkills().TripleDead();
				}
			}
			ThirdPersonStandardCameraScript component = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
			if (null != component)
			{
				component.ResetAngleV();
			}
			if (GameApp.GetInstance().GetGameMode().IsVSMode())
			{
				localPlayer.ReSpawnAtPointVS(spawnPointIndex);
				GameObject gameObject = GameObject.Find("GameUI");
				if (gameObject != null)
				{
					InGameUIScript component2 = gameObject.GetComponent<InGameUIScript>();
					if (component2 != null)
					{
						component2.OnVSRebirth();
					}
				}
				if (null != HUDBase.instance)
				{
					if (null != HUDBase.instance.m_SkillHUDManager)
					{
						HUDBase.instance.m_SkillHUDManager.RefreshSkillButtons();
					}
					if (null != HUDBase.instance.m_KillStreakHUDManager)
					{
						HUDBase.instance.m_KillStreakHUDManager.RefreshSkillButtons();
					}
					if (null != HUDBase.instance.m_StateItem)
					{
						HUDBase.instance.m_StateItem.Init();
					}
					if (null != HUDBase.instance.m_Weapon)
					{
						HUDBase.instance.m_Weapon.Refresh();
					}
				}
			}
			else
			{
				localPlayer.ReSpawnPVE();
			}
			localPlayer.GetSkills().PlayerRebirth();
			if (GameApp.GetInstance().GetGameMode().IsVSMode())
			{
				localPlayer.CreatePlayerSign();
			}
			return;
		}
		RemotePlayer remotePlayerByUserID = GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(playerID);
		if (remotePlayerByUserID != null)
		{
			if (GameApp.GetInstance().GetGameMode().IsUsePvpCustomizeMode())
			{
				remotePlayerByUserID.GetUserState().LoadCurrentCustomize(false, pvpLoadoutID);
			}
			if (GameApp.GetInstance().GetGameMode().IsVSMode())
			{
				remotePlayerByUserID.ReSpawnAtPointVS(spawnPointIndex);
			}
			else
			{
				remotePlayerByUserID.ReSpawnPVE();
			}
			if (GameApp.GetInstance().GetGameMode().IsVSMode())
			{
				remotePlayerByUserID.CreatePlayerSign();
			}
			remotePlayerByUserID.ReduceRemotePlayerQuality();
		}
	}
}
