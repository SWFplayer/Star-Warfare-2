using UnityEngine;

internal class GetRoomDataResponse : Response
{
	private Room room;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		int masterChannelID = bytesBuffer.ReadInt();
		byte mapID = bytesBuffer.ReadByte();
		byte difficultMode = bytesBuffer.ReadByte();
		byte b = bytesBuffer.ReadByte();
		room = new Room(b);
		RoomPlayer[] allPlayers = room.GetAllPlayers();
		byte b2 = 0;
		for (int i = 0; i < b; i++)
		{
			string text = bytesBuffer.ReadString();
			if (text == null || text == string.Empty)
			{
				allPlayers[i] = null;
				continue;
			}
			int channelId = bytesBuffer.ReadInt();
			int accountId = bytesBuffer.ReadInt();
			int eXP = bytesBuffer.ReadInt();
			byte titleId = bytesBuffer.ReadByte();
			byte frameType = bytesBuffer.ReadByte();
			byte prestige = bytesBuffer.ReadByte();
			string twitterName = bytesBuffer.ReadString();
			byte b3 = bytesBuffer.ReadByte();
			long num = 0L;
			if (b3 != 0)
			{
				num = bytesBuffer.ReadLong();
			}
			bool isRegisted = bytesBuffer.ReadBool();
			int missionNum = bytesBuffer.ReadInt();
			int weaponNum = bytesBuffer.ReadInt();
			int equipNum = bytesBuffer.ReadInt();
			int swVoidNum = bytesBuffer.ReadInt();
			int totalPlayTime = bytesBuffer.ReadInt();
			long serverTicks = bytesBuffer.ReadLong();
			OnlineCustomizeData onlineCustomizeData = new OnlineCustomizeData();
			OnlineCustomizeRec100 onlineCustomizeRec = new OnlineCustomizeRec100(onlineCustomizeData);
			onlineCustomizeRec.ReadData(bytesBuffer);
			allPlayers[i] = new RoomPlayer();
			allPlayers[i].setPlayerName(text);
			allPlayers[i].ChannelId = channelId;
			allPlayers[i].AccountId = accountId;
			allPlayers[i].EXP = eXP;
			allPlayers[i].SetTitleId(titleId);
			allPlayers[i].SetFrameType(frameType);
			allPlayers[i].SetPrestige(prestige);
			allPlayers[i].SetIsRegisted(isRegisted);
			allPlayers[i].SetMissionNum(missionNum);
			allPlayers[i].SetWeaponNum(weaponNum);
			allPlayers[i].SetEquipNum(equipNum);
			allPlayers[i].SetSwVoidNum(swVoidNum);
			allPlayers[i].SetTotalPlayTime(totalPlayTime);
			allPlayers[i].SetLastLoginTime(TimeUtil.ConvertServerTicksToLocalTime(serverTicks));
			allPlayers[i].CustomizeData = onlineCustomizeData;
			b2++;
			if (num != 0L)
			{
				Debug.Log(string.Concat("GetRoomDataResponse: ", (FriendType)b3, "  ", num));
				allPlayers[i].SocialPlayerData.type = (FriendType)b3;
				allPlayers[i].SocialPlayerData.id = num;
				allPlayers[i].SocialPlayerData.twitterName = twitterName;
			}
		}
		room.setMapID(mapID);
		room.setJoinedPlayer(b2);
		Lobby.GetInstance().SetCurrentRoom(room);
		Lobby.GetInstance().DifficultMode = (EDifficultMode)difficultMode;
		Lobby.GetInstance().MasterChannelID = masterChannelID;
	}

	public override void ProcessLogic()
	{
		RoomPlayer[] allPlayers = room.GetAllPlayers();
		RoomPlayer[] array = allPlayers;
		foreach (RoomPlayer roomPlayer in array)
		{
			if (roomPlayer != null)
			{
				roomPlayer.SocialPlayer = SocialManager.GetInstance().GetSocialPlayer(roomPlayer.SocialPlayerData.type, roomPlayer.SocialPlayerData.id, roomPlayer.SocialPlayerData.twitterName);
			}
		}
		SocialManager.GetInstance().LoadSocialData();
		GameObject gameObject = GameObject.Find("MultiMenu");
		if (gameObject != null)
		{
			MultiMenuScript component = gameObject.GetComponent<MultiMenuScript>();
			if (component != null)
			{
				component.SetRoomData(room);
			}
		}
	}

	public override void ProcessRobotLogic(UIRobot robot)
	{
		robot.joinedPlayer = room.getJoinedPlayer();
		RoomPlayer[] allPlayers = room.GetAllPlayers();
		foreach (RoomPlayer roomPlayer in allPlayers)
		{
			if (roomPlayer != null)
			{
				if (!robot.Friends.Contains(roomPlayer.getPlayerName()) && roomPlayer.getPlayerName() != robot.userName)
				{
					robot.Friends.Add(roomPlayer.getPlayerName());
				}
				if (!robot.uiScript.IsRobot(roomPlayer.ChannelId))
				{
					robot.uiScript.AddRobotRemotePlayer(roomPlayer.ChannelId, robot.lobby.GetCurrentRoomID());
				}
			}
		}
	}
}
