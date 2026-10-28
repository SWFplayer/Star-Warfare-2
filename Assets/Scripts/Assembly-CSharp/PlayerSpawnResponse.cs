using UnityEngine;

internal class PlayerSpawnResponse : Response
{
	public PlayerInfo playerInfo;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		playerInfo = new PlayerInfo();
		playerInfo.channelID = bytesBuffer.ReadInt();
		playerInfo.accountID = bytesBuffer.ReadInt();
		playerInfo.seatID = bytesBuffer.ReadByte();
		playerInfo.isRegisted = bytesBuffer.ReadBool();
		if (GameApp.GetInstance().GetGameMode().IsUsePvpCustomizeMode())
		{
			playerInfo.pvpCustomizeSize = bytesBuffer.ReadByte();
			playerInfo.pvpSkillGridSize = bytesBuffer.ReadByte();
			playerInfo.currentPvpCustomizeID = bytesBuffer.ReadByte();
			playerInfo.pvpCustomizeDataArray = new OnlineCustomizeData[playerInfo.pvpCustomizeSize];
			for (int i = 0; i < playerInfo.pvpCustomizeSize; i++)
			{
				playerInfo.pvpCustomizeDataArray[i] = new OnlineCustomizeData();
				OnlineCustomizeRec100 onlineCustomizeRec = new OnlineCustomizeRec100(playerInfo.pvpCustomizeDataArray[i]);
				onlineCustomizeRec.ReadData(bytesBuffer);
			}
			for (int j = 0; j < playerInfo.pvpCustomizeSize; j++)
			{
				OnlineCustomizeUserStateInfo100 onlineCustomizeUserStateInfo = new OnlineCustomizeUserStateInfo100(playerInfo.pvpCustomizeDataArray[j]);
				onlineCustomizeUserStateInfo.ReadData(bytesBuffer);
			}
		}
		else
		{
			playerInfo.coopCustomize = new OnlineCustomizeData();
			OnlineCustomizeRec100 onlineCustomizeRec2 = new OnlineCustomizeRec100(playerInfo.coopCustomize);
			onlineCustomizeRec2.ReadData(bytesBuffer);
			OnlineCustomizeUserStateInfo100 onlineCustomizeUserStateInfo2 = new OnlineCustomizeUserStateInfo100(playerInfo.coopCustomize);
			onlineCustomizeUserStateInfo2.ReadData(bytesBuffer);
		}
		playerInfo.playerName = bytesBuffer.ReadString();
		string twitterName = bytesBuffer.ReadString();
		FriendType friendType = (FriendType)bytesBuffer.ReadByte();
		if (friendType != FriendType.NA)
		{
			long num = bytesBuffer.ReadLong();
			playerInfo.SocialPlayerData.type = friendType;
			playerInfo.SocialPlayerData.id = num;
			playerInfo.SocialPlayerData.twitterName = twitterName;
			SocialManager.Log(string.Concat("PlayerSpawnResponse: type: ", friendType, " Id: ", num));
		}
		playerInfo.Exp = bytesBuffer.ReadInt();
		playerInfo.TitleId = bytesBuffer.ReadByte();
		playerInfo.HeadFrame = bytesBuffer.ReadByte();
		playerInfo.Prestige = bytesBuffer.ReadByte();
		playerInfo.MissionNum = bytesBuffer.ReadInt();
		playerInfo.WeaponNum = bytesBuffer.ReadInt();
		playerInfo.EquipNum = bytesBuffer.ReadInt();
		playerInfo.SwVoidNum = bytesBuffer.ReadInt();
		playerInfo.TotalPlayTime = bytesBuffer.ReadInt();
		long serverTicks = bytesBuffer.ReadLong();
		playerInfo.LastLoginTime = TimeUtil.ConvertServerTicksToLocalTime(serverTicks);
	}

	public override void ProcessLogic()
	{
		Debug.Log("receive player spawn..");
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
		if (!gameWorld.RemotePlayerExists(playerInfo.channelID))
		{
			RemotePlayer remotePlayer = new RemotePlayer();
			remotePlayer.SetChannelID(playerInfo.channelID);
			remotePlayer.SetAccountID(playerInfo.accountID);
			remotePlayer.SetSeatID(playerInfo.seatID);
			remotePlayer.SetIsRegisted(playerInfo.isRegisted);
			remotePlayer.SocialPlayer = SocialManager.GetInstance().GetSocialPlayer(playerInfo.SocialPlayerData.type, playerInfo.SocialPlayerData.id, playerInfo.SocialPlayerData.twitterName);
			remotePlayer.GetUserState().SetOnlineUserId(playerInfo.accountID);
			remotePlayer.GetUserState().SetExp(playerInfo.Exp);
			remotePlayer.GetUserState().SetCurrentTitleId(playerInfo.TitleId);
			remotePlayer.GetUserState().SetCurrentTitleFrameId(playerInfo.HeadFrame);
			remotePlayer.GetUserState().SetPrestige(playerInfo.Prestige);
			remotePlayer.GetUserState().SetMissionNum(playerInfo.MissionNum);
			remotePlayer.GetUserState().SetWeaponNum(playerInfo.WeaponNum);
			remotePlayer.GetUserState().SetEquipNum(playerInfo.EquipNum);
			remotePlayer.GetUserState().SetSwVoidNum(playerInfo.SwVoidNum);
			remotePlayer.GetUserState().SetTotalPlayTime(playerInfo.TotalPlayTime);
			remotePlayer.GetUserState().SetSyncRecordTime(playerInfo.LastLoginTime);
			if (GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
			{
				remotePlayer.Team = TeamName.Blue;
			}
			else
			{
				remotePlayer.Team = (TeamName)(remotePlayer.GetSeatID() / 6);
			}
			if (GameApp.GetInstance().GetGameMode().IsUsePvpCustomizeMode())
			{
				remotePlayer.CreateUserState(playerInfo.pvpCustomizeDataArray, playerInfo.currentPvpCustomizeID);
			}
			else
			{
				remotePlayer.CreateUserState(playerInfo.coopCustomize);
				remotePlayer.GetUserState().LoadCurrentCustomize(true, 0);
			}
			remotePlayer.Init();
			remotePlayer.SetName(playerInfo.playerName);
			if (GameApp.GetInstance().GetGameMode().IsVSMode())
			{
				remotePlayer.DropAtSpawnPositionVS();
			}
			else
			{
				remotePlayer.DropAtSpawnPosition();
			}
			if (GameApp.GetInstance().GetGameMode().IsUsePvpCustomizeMode())
			{
				remotePlayer.SetState(Player.VS_INIT_LOADOUT_STATE);
			}
			remotePlayer.CreatePlayerSign();
			gameWorld.AddRemotePlayer(remotePlayer);
			gameWorld.VSTimeStopResume();
		}
		if (Lobby.GetInstance().IsMasterPlayer)
		{
			if (Lobby.GetInstance().IsScoreWinCondition == 0)
			{
				SendVSTimeRequest request = new SendVSTimeRequest(playerInfo.channelID, Lobby.GetInstance().GetVSClock().GetCurrentTime());
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			if (GameApp.GetInstance().GetGameMode().IsCatchTheFlagMode())
			{
				SendFlagTimeRequest request2 = new SendFlagTimeRequest(playerInfo.channelID, gameWorld.FlagClock.GetCurrentTime());
				GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
			}
			if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.VS_KOTH)
			{
				gameWorld.VsBattleLogic.SendBattleInfoToNewPlayer(playerInfo.channelID);
			}
			else if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Domination)
			{
				gameWorld.VsBattleLogic.SendBattleInfoToNewPlayer(playerInfo.channelID);
			}
			if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.GIANT_BETRAYAL)
			{
				gameWorld.VsBattleLogic.SendBattleInfoToNewPlayer(playerInfo.channelID);
			}
			if (RandomPickUpManagerScript.instance != null)
			{
				RandomPickUpManagerScript.instance.SendPickUpMessageToRemotePlayer(playerInfo.channelID);
			}
		}
		if (localPlayer.IsLocal() && localPlayer.GetWeapon() != null && localPlayer.GetWeapon().GetWeaponType() == WeaponType.ChargeDoublePistol)
		{
			ChargeDoublePistol chargeDoublePistol = localPlayer.GetWeapon() as ChargeDoublePistol;
			if (chargeDoublePistol.GetCharge() != 0)
			{
				PlayerChargeWeaponInfoRequest request3 = new PlayerChargeWeaponInfoRequest(chargeDoublePistol.GetCharge(), chargeDoublePistol.IsPower(), playerInfo.channelID);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request3);
			}
		}
	}
}
