using System;

public class OnlineFriendResponseData
{
	public int mUserId;

	public string mName;

	public int mExp;

	public byte mPrestige;

	public FriendType mPortraitType;

	public long mPortraitId;

	public string mTwitterName;

	public byte mTitleId;

	public byte mTitleFrameId;

	public int mMaxCount;

	public int mMissionNum;

	public int mWeaponNum;

	public int mEquipNum;

	public int mSwVoidNum;

	public int mTotalPlayTime;

	public DateTime mLastLoginTime;

	public OnlineCustomizeData mCustomize = new OnlineCustomizeData();

	public SwVoidResponseData mSwVoid = new SwVoidResponseData();

	public AchievementState mAchievement = new AchievementState();

	public void ReadData(BytesBuffer bb)
	{
		ReadRequestData(bb);
		mMaxCount = bb.ReadByte();
		mMissionNum = bb.ReadInt();
		mWeaponNum = bb.ReadInt();
		mEquipNum = bb.ReadInt();
		mSwVoidNum = bb.ReadInt();
		mTotalPlayTime = bb.ReadInt();
		long serverTicks = bb.ReadLong();
		mLastLoginTime = TimeUtil.ConvertServerTicksToLocalTime(serverTicks);
		OnlineCustomizeRec100 onlineCustomizeRec = new OnlineCustomizeRec100(mCustomize);
		onlineCustomizeRec.ReadData(bb);
		if (mCustomize.mSwVoidId != 0)
		{
			mSwVoid.ReadData(bb);
		}
		BytesBuffer bytesBuffer = bb.ReadBytesBuffer();
		if (bytesBuffer.GetBytes().Length > 0)
		{
			string version = bytesBuffer.ReadString();
			IRecordPart recordPart = RecordFactory.CreateAchievementRecord(version, mAchievement);
			recordPart.LoadData(bytesBuffer);
		}
	}

	public void ReadRequestData(BytesBuffer bb)
	{
		mUserId = bb.ReadInt();
		mName = bb.ReadStringShortLength();
		mExp = bb.ReadInt();
		mPrestige = bb.ReadByte();
		mPortraitType = (FriendType)bb.ReadByte();
		mPortraitId = bb.ReadLong();
		mTwitterName = bb.ReadStringShortLength();
		mTitleId = bb.ReadByte();
		mTitleFrameId = bb.ReadByte();
	}
}
