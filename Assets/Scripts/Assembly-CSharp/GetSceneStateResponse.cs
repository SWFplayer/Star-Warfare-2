using System.Collections.Generic;
using UnityEngine;

internal class GetSceneStateResponse : Response
{
	public byte playerLength;

	public PlayerInfo[] playerInfos;

	public short pvpKillCash;

	public short pvpAssistCash;

	public EnemyInfo[] enemyInfos;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		playerLength = bytesBuffer.ReadByte();
		playerInfos = new PlayerInfo[playerLength];
		for (int i = 0; i < playerLength; i++)
		{
			playerInfos[i] = new PlayerInfo();
			playerInfos[i].channelID = bytesBuffer.ReadInt();
			playerInfos[i].accountID = bytesBuffer.ReadInt();
			playerInfos[i].seatID = bytesBuffer.ReadByte();
			playerInfos[i].isRegisted = bytesBuffer.ReadBool();
			playerInfos[i].bagIdOfWeapon = bytesBuffer.ReadByte();
			if (GameApp.GetInstance().GetGameMode().IsUsePvpCustomizeMode())
			{
				playerInfos[i].pvpSpawned = bytesBuffer.ReadBool();
				playerInfos[i].pvpCustomizeSize = bytesBuffer.ReadByte();
				playerInfos[i].pvpSkillGridSize = bytesBuffer.ReadByte();
				playerInfos[i].currentPvpCustomizeID = bytesBuffer.ReadByte();
				playerInfos[i].pvpCustomizeDataArray = new OnlineCustomizeData[playerInfos[i].pvpCustomizeSize];
				for (int j = 0; j < playerInfos[i].pvpCustomizeSize; j++)
				{
					playerInfos[i].pvpCustomizeDataArray[j] = new OnlineCustomizeData();
					OnlineCustomizeRec100 onlineCustomizeRec = new OnlineCustomizeRec100(playerInfos[i].pvpCustomizeDataArray[j]);
					onlineCustomizeRec.ReadData(bytesBuffer);
				}
				for (int k = 0; k < playerInfos[i].pvpCustomizeSize; k++)
				{
					OnlineCustomizeUserStateInfo100 onlineCustomizeUserStateInfo = new OnlineCustomizeUserStateInfo100(playerInfos[i].pvpCustomizeDataArray[k]);
					onlineCustomizeUserStateInfo.ReadData(bytesBuffer);
				}
			}
			else
			{
				playerInfos[i].coopCustomize = new OnlineCustomizeData();
				OnlineCustomizeRec100 onlineCustomizeRec2 = new OnlineCustomizeRec100(playerInfos[i].coopCustomize);
				onlineCustomizeRec2.ReadData(bytesBuffer);
				OnlineCustomizeUserStateInfo100 onlineCustomizeUserStateInfo2 = new OnlineCustomizeUserStateInfo100(playerInfos[i].coopCustomize);
				onlineCustomizeUserStateInfo2.ReadData(bytesBuffer);
				playerInfos[i].maxHP = bytesBuffer.ReadInt();
				playerInfos[i].hp = bytesBuffer.ReadInt();
			}
			playerInfos[i].playerName = bytesBuffer.ReadString();
			string twitterName = bytesBuffer.ReadString();
			FriendType type = (FriendType)bytesBuffer.ReadByte();
			long id = bytesBuffer.ReadLong();
			playerInfos[i].SocialPlayerData.type = type;
			playerInfos[i].SocialPlayerData.id = id;
			playerInfos[i].SocialPlayerData.twitterName = twitterName;
			playerInfos[i].Exp = bytesBuffer.ReadInt();
			playerInfos[i].TitleId = bytesBuffer.ReadByte();
			playerInfos[i].HeadFrame = bytesBuffer.ReadByte();
			playerInfos[i].Prestige = bytesBuffer.ReadByte();
			playerInfos[i].MissionNum = bytesBuffer.ReadInt();
			playerInfos[i].WeaponNum = bytesBuffer.ReadInt();
			playerInfos[i].EquipNum = bytesBuffer.ReadInt();
			playerInfos[i].SwVoidNum = bytesBuffer.ReadInt();
			playerInfos[i].TotalPlayTime = bytesBuffer.ReadInt();
			long serverTicks = bytesBuffer.ReadLong();
			playerInfos[i].LastLoginTime = TimeUtil.ConvertServerTicksToLocalTime(serverTicks);
			short num = bytesBuffer.ReadShort();
			short num2 = bytesBuffer.ReadShort();
			short num3 = bytesBuffer.ReadShort();
			short num4 = bytesBuffer.ReadShort();
			playerInfos[i].Position = new Vector3((float)num / 10f, (float)num2 / 10f, (float)num3 / 10f);
			playerInfos[i].RotationY = (float)num4 / 10f;
		}
		pvpKillCash = bytesBuffer.ReadShort();
		pvpAssistCash = bytesBuffer.ReadShort();
		Debug.Log("...GetSceneStateResponse ");
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		int channelID = Lobby.GetInstance().GetChannelID();
		Room currentRoom = Lobby.GetInstance().GetCurrentRoom();
		for (int i = 0; i < playerLength; i++)
		{
			PlayerInfo playerInfo = playerInfos[i];
			if (channelID == playerInfo.channelID)
			{
				Debug.Log("has local Player");
				Player localPlayer = gameWorld.GetLocalPlayer();
				localPlayer.SetSeatID(playerInfos[i].seatID);
				if (GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
				{
					localPlayer.Team = TeamName.Blue;
				}
				else
				{
					localPlayer.Team = (TeamName)(localPlayer.GetSeatID() / 6);
				}
				localPlayer.SetName(playerInfos[i].playerName);
				localPlayer.SetIsRegisted(playerInfos[i].isRegisted);
				if (GameApp.GetInstance().GetGameMode().IsUsePvpCustomizeMode())
				{
					for (int j = 0; j < playerInfo.pvpCustomizeDataArray.Length; j++)
					{
						if (playerInfo.pvpCustomizeDataArray[j] != null)
						{
							localPlayer.GetUserState().SetOnlineCustomizeUserStateInfo(playerInfo.pvpCustomizeDataArray[j]);
						}
					}
				}
				else
				{
					localPlayer.GetUserState().SetOnlineCustomizeUserStateInfo(playerInfo.coopCustomize);
					gameWorld.GetLocalPlayer().GetUserState().LoadCurrentCustomize(true, 0);
					gameWorld.GetLocalPlayer().InitSkills();
				}
				if (GameApp.GetInstance().GetGameMode().IsVSMode())
				{
					gameWorld.GetLocalPlayer().DropAtSpawnPositionVS();
					gameWorld.GetLocalPlayer().CreatePlayerSign();
					List<RemotePlayer> remotePlayers = gameWorld.GetRemotePlayers();
					foreach (RemotePlayer item in remotePlayers)
					{
						if (item != null)
						{
							item.CreatePlayerSign();
						}
					}
				}
				else
				{
					gameWorld.GetLocalPlayer().DropAtSpawnPosition();
					if (GameApp.GetInstance().GetGameMode().IsCoopMode())
					{
						List<RemotePlayer> remotePlayers2 = gameWorld.GetRemotePlayers();
						foreach (RemotePlayer item2 in remotePlayers2)
						{
							if (item2 != null)
							{
								item2.CreatePlayerSign();
							}
						}
					}
				}
				localPlayer.GetUserState().Achievement.ResetPVPState();
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
				if (gameWorld.RemotePlayerExists(playerInfo.channelID))
				{
					continue;
				}
				RemotePlayer remotePlayer = new RemotePlayer();
				remotePlayer.SetChannelID(playerInfo.channelID);
				remotePlayer.SetAccountID(playerInfo.accountID);
				remotePlayer.SetSeatID(playerInfo.seatID);
				remotePlayer.SetIsRegisted(playerInfo.isRegisted);
				remotePlayer.SocialPlayer = SocialManager.GetInstance().GetSocialPlayer(playerInfo.SocialPlayerData.type, playerInfo.SocialPlayerData.id, playerInfo.SocialPlayerData.twitterName);
				remotePlayer.GetUserState().SetPrestige(playerInfo.Prestige);
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
				remotePlayer.ChangeWeaponInBag(playerInfo.bagIdOfWeapon);
				remotePlayer.SetName(playerInfo.playerName);
				remotePlayer.MaxHp = playerInfo.maxHP;
				remotePlayer.Hp = playerInfo.hp;
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
				if (GameApp.GetInstance().GetGameMode().IsUsePvpCustomizeMode() && !playerInfos[i].pvpSpawned)
				{
					remotePlayer.SetState(Player.VS_INIT_LOADOUT_STATE);
				}
				if (GameApp.GetInstance().GetGameMode().IsVSMode())
				{
					if (playerInfo.Position.x == 0f && playerInfo.Position.y == 0f && playerInfo.Position.z == 0f)
					{
						remotePlayer.DropAtSpawnPositionVS();
					}
					else
					{
						remotePlayer.DropAtSpawnPositionVS(playerInfo.Position, playerInfo.RotationY);
					}
					remotePlayer.CreatePlayerSign();
				}
				else
				{
					remotePlayer.DropAtSpawnPosition();
					if (GameApp.GetInstance().GetGameMode().IsCoopMode())
					{
						remotePlayer.CreatePlayerSign();
					}
				}
				gameWorld.AddRemotePlayer(remotePlayer);
			}
		}
		gameWorld.PVPReward.cashPerKill = pvpKillCash;
		gameWorld.PVPReward.cashPerAssist = pvpAssistCash;
		Debug.Log("pvp rewards:" + gameWorld.PVPReward.cashPerKill + "," + gameWorld.PVPReward.cashPerAssist);
		UserState userState = GameApp.GetInstance().GetUserState();
		gameWorld.VSTimeStopResume();
		SocialManager.GetInstance().LoadSocialData();
	}

	public override void ProcessRobotLogic(UIRobot robot)
	{
		robot.o.transform.position = new Vector3(-10f, 0.1f, -10f);
	}
}
