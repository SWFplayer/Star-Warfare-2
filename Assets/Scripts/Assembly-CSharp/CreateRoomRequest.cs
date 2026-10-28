public class CreateRoomRequest : Request
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

	public FriendType mSocialType;

	public long mSocialId;

	public byte DifficultMode;

	public CreateRoomRequest(string name, short pass, byte playerNum, bool hasPassword, byte mode, byte mapID, byte rankID, short ping, byte minJoinRankID, byte maxJoinRankID, byte DifficultMode)
	{
		requestID = 4;
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
		this.DifficultMode = DifficultMode;
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
	}

	public override BytesBuffer GetBody()
	{
		byte stringByteLength = BytesBuffer.GetStringByteLength(roomName);
		int num = 9 + stringByteLength + 1 + 1;
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
		bytesBuffer.AddByte(DifficultMode);
		return bytesBuffer;
	}
}
