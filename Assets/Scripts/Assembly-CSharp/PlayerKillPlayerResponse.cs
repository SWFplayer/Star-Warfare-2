using System.Collections.Generic;
using UnityEngine;

internal class PlayerKillPlayerResponse : Response
{
	protected int killerID;

	protected int goodAssistID;

	protected int killedID;

	protected byte assisterCount;

	protected int[] assisterList;

	protected byte specialInfoCount;

	protected List<PlayerVSStatistics.specialInfo> specialInfoList;

	protected byte attackerType;

	protected byte damageFactorID;

	protected byte weaponID;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		killerID = bytesBuffer.ReadInt();
		goodAssistID = bytesBuffer.ReadInt();
		killedID = bytesBuffer.ReadInt();
		assisterCount = bytesBuffer.ReadByte();
		if (assisterCount > 0)
		{
			assisterList = new int[assisterCount];
			for (int i = 0; i < assisterCount; i++)
			{
				assisterList[i] = bytesBuffer.ReadInt();
			}
		}
		specialInfoCount = bytesBuffer.ReadByte();
		if (specialInfoCount > 0)
		{
			specialInfoList = new List<PlayerVSStatistics.specialInfo>();
			for (int j = 0; j < specialInfoCount; j++)
			{
				PlayerVSStatistics.specialInfo specialInfo = new PlayerVSStatistics.specialInfo();
				specialInfo.type = (PlayerVSStatistics.specialInfoType)bytesBuffer.ReadByte();
				if (specialInfo.type == PlayerVSStatistics.specialInfoType.MULTI_KILL)
				{
					specialInfo.value = bytesBuffer.ReadByte();
				}
				specialInfoList.Add(specialInfo);
			}
		}
		attackerType = bytesBuffer.ReadByte();
		damageFactorID = bytesBuffer.ReadByte();
		weaponID = bytesBuffer.ReadByte();
	}

	public override void ProcessLogic()
	{
		Debug.Log(killerID + " killed " + killedID);
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null || gameWorld.State != GameState.Playing)
		{
			return;
		}
		LocalPlayer localPlayer = gameWorld.GetLocalPlayer();
		if (localPlayer == null)
		{
			return;
		}
		int channelID = Lobby.GetInstance().GetChannelID();
		bool flag = true;
		string empty = string.Empty;
		string empty2 = string.Empty;
		int num = 0;
		int num2 = 0;
		Player player = null;
		Player player2 = null;
		GameApp.GetInstance().GetUserState().Achievement.AddKillStat(killerID);
		if (killerID == channelID)
		{
			num = localPlayer.GetSeatID();
			empty = "P" + (num + 1);
			if (localPlayer.IsBoss)
			{
				localPlayer.VSStatistics.MakePlayerBossKill();
			}
			else
			{
				localPlayer.DeadWithoutKillNum = 0;
				localPlayer.GetSkills().PlayerKillOthers();
				localPlayer.GetUserState().Achievement.CheckAchievement_KillAll(killedID);
				RemotePlayer remotePlayerByUserID = GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(killedID);
				if (remotePlayerByUserID != null)
				{
					localPlayer.GetUserState().Achievement.CheckAchievement_AllWeaponKill(remotePlayerByUserID.GetWeapon().GetWeaponType());
					if (remotePlayerByUserID.isCapturingFlag)
					{
						localPlayer.GetUserState().Achievement.AddFlagerKills();
					}
					if (!remotePlayerByUserID.IsSameTeam(localPlayer))
					{
						localPlayer.VSStatistics.MakeKill(remotePlayerByUserID);
						localPlayer.VSStatistics.CheckComboInLocal();
					}
				}
				else
				{
					flag = false;
				}
				if (localPlayer.isCapturingFlag)
				{
					localPlayer.GetUserState().Achievement.AddKillWithFlagNum();
				}
				if (attackerType == 1 && damageFactorID == 1)
				{
					localPlayer.GetUserState().Achievement.AddSawKill();
				}
				else if (attackerType == 0 && ((localPlayer.GetWeapon().GetWeaponType() == WeaponType.BowSword && damageFactorID != 0) || localPlayer.GetWeapon().GetWeaponType() == WeaponType.Sword))
				{
					localPlayer.GetUserState().Achievement.AddSwordKill();
				}
				if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.VS_FFA)
				{
					localPlayer.VSStatistics.FfaScore = localPlayer.VSStatistics.Kills * 60;
				}
			}
			localPlayer.VSStatistics.UpdataScore();
			player = localPlayer;
		}
		else
		{
			RemotePlayer remotePlayerByUserID2 = GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(killerID);
			if (remotePlayerByUserID2 != null)
			{
				num = remotePlayerByUserID2.GetSeatID();
				empty = "P" + (num + 1);
				player = remotePlayerByUserID2;
				if (remotePlayerByUserID2.IsBoss)
				{
					remotePlayerByUserID2.VSStatistics.PlayerBossKills++;
				}
				else
				{
					remotePlayerByUserID2.VSStatistics.Kills++;
				}
				remotePlayerByUserID2.VSStatistics.UpdataScore();
				if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.VS_FFA)
				{
					remotePlayerByUserID2.VSStatistics.FfaScore = remotePlayerByUserID2.VSStatistics.Kills * 60;
				}
			}
			else
			{
				flag = false;
			}
		}
		bool flag2 = false;
		if (!GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
		{
			if (goodAssistID != 0)
			{
				if (goodAssistID == channelID)
				{
					localPlayer.VSStatistics.MakeGoodAssist();
					localPlayer.VSStatistics.PlayerHit++;
					localPlayer.VSStatistics.UpdataScore();
				}
				else
				{
					RemotePlayer remotePlayerByUserID3 = GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(goodAssistID);
					if (remotePlayerByUserID3 != null)
					{
						remotePlayerByUserID3.VSStatistics.GoodAssist++;
						remotePlayerByUserID3.VSStatistics.UpdataScore();
					}
					else
					{
						goodAssistID = 0;
					}
				}
			}
			if (assisterCount > 0)
			{
				for (int i = 0; i < assisterCount; i++)
				{
					if (assisterList[i] == channelID)
					{
						localPlayer.VSStatistics.MakeAssist();
						localPlayer.VSStatistics.PlayerHit++;
						flag2 = true;
						continue;
					}
					RemotePlayer remotePlayerByUserID4 = GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(goodAssistID);
					if (remotePlayerByUserID4 != null)
					{
						remotePlayerByUserID4.VSStatistics.Assist++;
						remotePlayerByUserID4.VSStatistics.UpdataScore();
					}
				}
			}
		}
		if (killedID == channelID)
		{
			num2 = localPlayer.GetSeatID();
			empty2 = "P" + (num2 + 1);
			localPlayer.VSStatistics.Death++;
			localPlayer.VSStatistics.Combo = 0;
			localPlayer.VSStatistics.ComboInOneLife = 0;
			player2 = localPlayer;
			localPlayer.DeadWithoutKillNum++;
			localPlayer.RevengePlayerId = killerID;
		}
		else
		{
			RemotePlayer remotePlayerByUserID5 = GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(killedID);
			if (remotePlayerByUserID5 != null)
			{
				num2 = remotePlayerByUserID5.GetSeatID();
				empty2 = "P" + (num2 + 1);
				player2 = remotePlayerByUserID5;
				remotePlayerByUserID5.VSStatistics.Death++;
			}
			else
			{
				flag = false;
			}
			if (killedID == localPlayer.RevengePlayerId)
			{
				localPlayer.StopRevengeRadar();
			}
		}
		if (player2 != null && localPlayer != null && player2.IsSameTeam(localPlayer) && player2 != localPlayer && localPlayer.InPlayingState() && Vector3.Distance(player2.GetPosition(), localPlayer.GetPosition()) < 20f)
		{
			localPlayer.GetSkills().AllyDead();
		}
		if (flag)
		{
			UserStateHUD.GetInstance().InfoBox.PushKillInfo(player, player2, attackerType, weaponID);
		}
		UserStateHUD.InfoBoxHUD infoBox = UserStateHUD.GetInstance().InfoBox;
		if (goodAssistID == channelID)
		{
			localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.GOOD_ASSIST);
		}
		else if (flag2)
		{
			localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.ASSIST);
		}
		if (specialInfoCount <= 0)
		{
			return;
		}
		foreach (PlayerVSStatistics.specialInfo specialInfo in specialInfoList)
		{
			switch (specialInfo.type)
			{
			case PlayerVSStatistics.specialInfoType.FIRST_BLOOD:
				if (channelID == killerID)
				{
					localPlayer.VSStatistics.MakeFirstBlood();
					localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.FIRST_BLOOD);
				}
				else
				{
					localPlayer.VSStatistics.ShowPublicInfo(infoBox, PlayerVSStatistics.EExpType.FIRST_BLOOD, player, player2, 0);
				}
				break;
			case PlayerVSStatistics.specialInfoType.PAY_BACK:
				if (channelID == killerID)
				{
					localPlayer.VSStatistics.MakePayBack();
					localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.PAY_BACK);
				}
				break;
			case PlayerVSStatistics.specialInfoType.DOUBLE_KILL:
				if (channelID == killerID)
				{
					if (localPlayer.IsBoss)
					{
						localPlayer.VSStatistics.MakePlayerBossDoubleKill();
						localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.PLAYER_BOSS_DOUBLE_KILL);
					}
					else
					{
						localPlayer.VSStatistics.MakeDoubleKill();
						localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.DOUBLE_KILL);
					}
				}
				break;
			case PlayerVSStatistics.specialInfoType.MULTI_KILL:
				if (channelID == killerID)
				{
					if (localPlayer.IsBoss)
					{
						localPlayer.VSStatistics.MakePlayerBossMultiKill();
						localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.PLAYER_BOSS_MULTI_KILL);
					}
					else
					{
						localPlayer.VSStatistics.MakeMultiKill(specialInfo.value);
						localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.MULTI_KILL);
					}
				}
				else
				{
					localPlayer.VSStatistics.ShowPublicInfo(infoBox, PlayerVSStatistics.EExpType.MULTI_KILL, player, null, 0);
				}
				break;
			case PlayerVSStatistics.specialInfoType.LONG_SHOT:
				if (channelID == killerID)
				{
					if (localPlayer.IsBoss)
					{
						localPlayer.VSStatistics.MakePlayerBossLongShotKill();
					}
					else
					{
						localPlayer.VSStatistics.MakeLongShot();
					}
					localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.LONG_SHOT);
				}
				break;
			case PlayerVSStatistics.specialInfoType.LEGENDARY:
				if (channelID == killerID)
				{
					localPlayer.VSStatistics.MakeLegendary();
					localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.LEGENDARY);
				}
				else
				{
					localPlayer.VSStatistics.ShowPublicInfo(infoBox, PlayerVSStatistics.EExpType.LEGENDARY, player, null, 0);
				}
				break;
			case PlayerVSStatistics.specialInfoType.LEGENDARY_NO_EXP:
				if (channelID == killerID)
				{
					localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.LEGENDARY, 0);
				}
				else
				{
					localPlayer.VSStatistics.ShowPublicInfo(infoBox, PlayerVSStatistics.EExpType.LEGENDARY, player, null, 0);
				}
				break;
			case PlayerVSStatistics.specialInfoType.STOP_HIM:
				if (channelID == killerID)
				{
					localPlayer.VSStatistics.MakeStopHim();
					localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.STOP_HIM);
				}
				else if (player != null)
				{
					player.VSStatistics.MakeStopHim();
					player.VSStatistics.UpdataScore();
				}
				break;
			case PlayerVSStatistics.specialInfoType.UNSTOPABLE:
				if (channelID == killerID)
				{
					localPlayer.VSStatistics.MakeUnStopable();
					localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.UNSTOPABLE);
				}
				else
				{
					localPlayer.VSStatistics.ShowPublicInfo(infoBox, PlayerVSStatistics.EExpType.UNSTOPABLE, player, null, 0);
				}
				break;
			case PlayerVSStatistics.specialInfoType.UNSTOPABLE_NO_EXP:
				if (channelID == killerID)
				{
					localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.UNSTOPABLE, 0);
				}
				else
				{
					localPlayer.VSStatistics.ShowPublicInfo(infoBox, PlayerVSStatistics.EExpType.UNSTOPABLE, player, null, 0);
				}
				break;
			case PlayerVSStatistics.specialInfoType.PLAYER_BOSS_RAMPAGE:
				if (channelID == killerID)
				{
					localPlayer.VSStatistics.MakeUnStopable();
					localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.PLAYER_BOSS_RAMPAGE);
				}
				else
				{
					localPlayer.VSStatistics.ShowPublicInfo(infoBox, PlayerVSStatistics.EExpType.PLAYER_BOSS_RAMPAGE, player, null, 0);
				}
				break;
			case PlayerVSStatistics.specialInfoType.PLAYER_BOSS_RAMPAGE_NO_EXP:
				if (channelID == killerID)
				{
					localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.PLAYER_BOSS_RAMPAGE, 0);
				}
				else
				{
					localPlayer.VSStatistics.ShowPublicInfo(infoBox, PlayerVSStatistics.EExpType.PLAYER_BOSS_RAMPAGE, player, null, 0);
				}
				break;
			case PlayerVSStatistics.specialInfoType.DOMINATE:
				if (channelID == killerID)
				{
					localPlayer.VSStatistics.MakeDominate();
					localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.DOMINATE);
				}
				else
				{
					localPlayer.VSStatistics.ShowPublicInfo(infoBox, PlayerVSStatistics.EExpType.DOMINATE, player, player2, 0);
				}
				break;
			case PlayerVSStatistics.specialInfoType.REVENGE:
				if (channelID == killerID)
				{
					localPlayer.VSStatistics.MakeRevenge();
					localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.REVENGE);
				}
				else
				{
					localPlayer.VSStatistics.ShowPublicInfo(infoBox, PlayerVSStatistics.EExpType.REVENGE, player, player2, 0);
				}
				break;
			case PlayerVSStatistics.specialInfoType.ASSASSINATE:
				if (channelID == killerID)
				{
					localPlayer.VSStatistics.MakeAssassinate();
					localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.ASSASSINATE);
				}
				else if (player != null)
				{
					player.VSStatistics.MakeAssassinate();
					player.VSStatistics.UpdataScore();
				}
				break;
			case PlayerVSStatistics.specialInfoType.RAMPAGE:
				if (channelID == killerID)
				{
					localPlayer.VSStatistics.MakeRampage();
					localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.RAMPAGE);
				}
				else
				{
					localPlayer.VSStatistics.ShowPublicInfo(infoBox, PlayerVSStatistics.EExpType.RAMPAGE, player, null, 0);
				}
				break;
			case PlayerVSStatistics.specialInfoType.RAMPAGE_NO_EXP:
				if (channelID == killerID)
				{
					localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.RAMPAGE, 0);
				}
				else
				{
					localPlayer.VSStatistics.ShowPublicInfo(infoBox, PlayerVSStatistics.EExpType.RAMPAGE, player, null, 0);
				}
				break;
			}
		}
	}
}
