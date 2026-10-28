using UnityEngine;

public class QuickJoinAdvancedRequest : Request
{
	private byte mOnlineMode;

	private byte mPlayerRankID;

	private byte mLowerRankID;

	private byte mUpperRankID;

	private byte mGameMode;

	private byte mMapID;

	private byte mWinCondition;

	private short mWinValue;

	private bool mIsAutoBalance;

	private byte mPlayerNumber;

	private short mPing;

	private byte mRandomWinCondition;

	private short mRandomWinValue;

	private byte mDiffMode;

	public FriendType mSocialType;

	public long mSocialId;

	public QuickJoinAdvancedRequest(byte onlineMode, byte playerRankID, byte lowerRankID, byte upperRankID, byte gameMode, byte mapID, byte winCondition, short winValue, bool autoBalance, byte playerNumber, short ping, EDifficultMode diffMode)
	{
		requestID = 28;
		mOnlineMode = onlineMode;
		mPlayerRankID = playerRankID;
		mLowerRankID = lowerRankID;
		mUpperRankID = upperRankID;
		mGameMode = gameMode;
		mMapID = mapID;
		mWinCondition = winCondition;
		mWinValue = winValue;
		mIsAutoBalance = autoBalance;
		mPlayerNumber = playerNumber;
		mPing = ping;
		mRandomWinCondition = 0;
		mRandomWinValue = 0;
		DealWithRandom();
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
		mDiffMode = (byte)diffMode;
	}

	public override BytesBuffer GetBody()
	{
		int num = 18;
		if (mSocialType != FriendType.NA)
		{
			num += 8;
		}
		BytesBuffer bytesBuffer = new BytesBuffer(num);
		bytesBuffer.AddByte(mOnlineMode);
		bytesBuffer.AddByte(mPlayerRankID);
		bytesBuffer.AddByte(mLowerRankID);
		bytesBuffer.AddByte(mUpperRankID);
		bytesBuffer.AddByte(mGameMode);
		bytesBuffer.AddByte(mMapID);
		bytesBuffer.AddByte(mWinCondition);
		bytesBuffer.AddShort(mWinValue);
		bytesBuffer.AddBool(mIsAutoBalance);
		bytesBuffer.AddByte(mPlayerNumber);
		bytesBuffer.AddShort(mPing);
		bytesBuffer.AddByte(mRandomWinCondition);
		bytesBuffer.AddShort(mRandomWinValue);
		bytesBuffer.AddByte(mDiffMode);
		bytesBuffer.AddByte((byte)mSocialType);
		if (mSocialType != FriendType.NA)
		{
			bytesBuffer.AddLong(mSocialId);
		}
		return bytesBuffer;
	}

	private void DealWithRandom()
	{
		if (mWinCondition == UIConstant.SEARCH_ROOM_WIN_CONDITION[0] || mWinValue == 0)
		{
			mRandomWinCondition = UIConstant.SEARCH_ROOM_WIN_CONDITION[Random.Range(1, 3)];
			mWinValue = 0;
			int num = Random.Range(1, 4);
			if (mRandomWinCondition == UIConstant.SEARCH_ROOM_WIN_CONDITION[1])
			{
				mRandomWinValue = UIConstant.SEARCH_ROOM_WIN_VALUE_FOR_SCORE[num];
			}
			else if (mRandomWinCondition == UIConstant.SEARCH_ROOM_WIN_CONDITION[2])
			{
				mRandomWinValue = UIConstant.SEARCH_ROOM_WIN_VALUE_FOR_TIMER[num];
			}
		}
	}
}
