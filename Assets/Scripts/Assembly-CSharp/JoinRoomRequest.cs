public class JoinRoomRequest : Request
{
	public short roomID;

	public byte rankID;

	public short ping;

	public FriendType mSocialType;

	public long mSocialId;

	public JoinRoomRequest(short id, byte rankID, short ping)
	{
		requestID = 5;
		roomID = id;
		this.rankID = rankID;
		this.ping = ping;
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
		int num = 6;
		if (mSocialType != FriendType.NA)
		{
			num += 8;
		}
		BytesBuffer bytesBuffer = new BytesBuffer(num);
		bytesBuffer.AddShort(roomID);
		bytesBuffer.AddByte(rankID);
		bytesBuffer.AddShort(ping);
		bytesBuffer.AddByte((byte)mSocialType);
		if (mSocialType != FriendType.NA)
		{
			bytesBuffer.AddLong(mSocialId);
		}
		return bytesBuffer;
	}
}
