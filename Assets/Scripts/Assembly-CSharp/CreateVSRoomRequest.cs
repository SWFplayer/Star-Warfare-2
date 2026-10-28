using UnityEngine;

public class CreateVSRoomRequest : Request
{
	public string roomName;

	public short passWord;

	public byte maxPlayer;

	public bool hasPass;

	public byte mode;

	public byte mapID;

	public byte rankID;

	public short ping;

	public byte minJoinRankID;

	public byte maxJoinRankID;

	public byte winCondition;

	public short winValue;

	public bool isAutoBalance;

	public FriendType mSocialType;

	public long mSocialId;

	public CreateVSRoomRequest(string name, short pass, byte playerNum, bool hasPassword, byte mode, byte mapID, byte rankID, short ping, byte minJoinRankID, byte maxJoinRankID, byte winCondition, short winValue, bool isAutoBalance)
	{
		requestID = 18;
		roomName = name;
		passWord = -1;
		passWord = pass;
		maxPlayer = playerNum;
		hasPass = hasPassword;
		this.mode = mode;
		this.mapID = mapID;
		this.rankID = rankID;
		this.ping = ping;
		this.minJoinRankID = minJoinRankID;
		this.maxJoinRankID = maxJoinRankID;
		this.winCondition = winCondition;
		this.winValue = winValue;
		this.isAutoBalance = isAutoBalance;
		DealWithRandom();
		Lobby.GetInstance().IsScoreWinCondition = winCondition;
		Lobby.GetInstance().WinValue = winValue;
		if (SocialManager.GetInstance().LocalSocialPlayer != null)
		{
			mSocialType = SocialManager.GetInstance().LocalSocialPlayer.friendType;
			if (mSocialType != FriendType.Twitter && mSocialType != FriendType.NA)
			{
				long.TryParse(SocialManager.GetInstance().LocalSocialPlayer.GetSocialID(), out mSocialId);
			}
			else
			{
				mSocialId = 0L;
			}
		}
		Debug.Log("maxPlayer = " + maxPlayer);
	}

	public override BytesBuffer GetBody()
	{
		byte stringByteLength = BytesBuffer.GetStringByteLength(roomName);
		int num = 13 + stringByteLength + 1;
		if (hasPass)
		{
			num += 2;
		}
		if (mSocialType != FriendType.NA)
		{
			num += 8;
		}
		BytesBuffer bytesBuffer = new BytesBuffer(num);
		bytesBuffer.AddBool(hasPass);
		bytesBuffer.AddByte(maxPlayer);
		bytesBuffer.AddByte(mode);
		bytesBuffer.AddByte(mapID);
		bytesBuffer.AddByte(rankID);
		bytesBuffer.AddShort(ping);
		bytesBuffer.AddByte(minJoinRankID);
		bytesBuffer.AddByte(maxJoinRankID);
		bytesBuffer.AddByte(winCondition);
		bytesBuffer.AddShort(winValue);
		bytesBuffer.AddBool(isAutoBalance);
		bytesBuffer.AddString(roomName);
		if (hasPass)
		{
			bytesBuffer.AddShort(passWord);
		}
		bytesBuffer.AddByte((byte)mSocialType);
		if (mSocialType != FriendType.NA)
		{
			bytesBuffer.AddLong(mSocialId);
		}
		return bytesBuffer;
	}

	private void DealWithRandom()
	{
		if (mode == 0)
		{
			while (!GameApp.GetInstance().GetGameMode().IsVSMode((Mode)mode) || mode == 13)
			{
				mode = (byte)Random.Range(1, 14);
			}
		}
		if (winCondition == UIConstant.SEARCH_ROOM_WIN_CONDITION[0] || winValue == 0)
		{
			winCondition = UIConstant.SEARCH_ROOM_WIN_CONDITION[Random.Range(1, 3)];
			winValue = 0;
			int num = Random.Range(1, 4);
			if (winCondition == UIConstant.SEARCH_ROOM_WIN_CONDITION[1])
			{
				winValue = UIConstant.SEARCH_ROOM_WIN_VALUE_FOR_SCORE[num];
			}
			else if (winCondition == UIConstant.SEARCH_ROOM_WIN_CONDITION[2])
			{
				winValue = UIConstant.SEARCH_ROOM_WIN_VALUE_FOR_TIMER[num];
			}
		}
	}
}
