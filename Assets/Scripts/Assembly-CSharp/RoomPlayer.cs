using System;

public class RoomPlayer
{
	private string playerName;

	private int channelId;

	private int accountId;

	private int exp;

	private bool mIsRegisted;

	private int mMissionNum;

	private int mWeaponNum;

	private int mEquipNum;

	private int mSwVoidNum;

	private int mTotalPlayTime;

	private DateTime mLastLoginTime;

	private byte mTitleId;

	private byte mFrameType;

	private byte mPrestige;

	public SocialPlayerData SocialPlayerData;

	public SocialPlayer SocialPlayer;

	public OnlineCustomizeData CustomizeData;

	public int ChannelId
	{
		get
		{
			return channelId;
		}
		set
		{
			channelId = value;
		}
	}

	public int AccountId
	{
		get
		{
			return accountId;
		}
		set
		{
			accountId = value;
		}
	}

	public int EXP
	{
		get
		{
			return exp;
		}
		set
		{
			exp = value;
		}
	}

	public string getPlayerName()
	{
		return playerName;
	}

	public void setPlayerName(string playerName)
	{
		this.playerName = playerName;
	}

	public void SetIsRegisted(bool isRegisted)
	{
		mIsRegisted = isRegisted;
	}

	public bool GetIsRegisted()
	{
		return mIsRegisted;
	}

	public void SetMissionNum(int num)
	{
		mMissionNum = num;
	}

	public int GetMissionNum()
	{
		return mMissionNum;
	}

	public void SetWeaponNum(int num)
	{
		mWeaponNum = num;
	}

	public int GetWeaponNum()
	{
		return mWeaponNum;
	}

	public void SetEquipNum(int num)
	{
		mEquipNum = num;
	}

	public int GetEquipNum()
	{
		return mEquipNum;
	}

	public void SetSwVoidNum(int num)
	{
		mSwVoidNum = num;
	}

	public int GetSwVoidNum()
	{
		return mSwVoidNum;
	}

	public int GetTotalPlayTime()
	{
		return mTotalPlayTime;
	}

	public void SetTotalPlayTime(int time)
	{
		mTotalPlayTime = time;
	}

	public DateTime GetLastLoginTime()
	{
		return mLastLoginTime;
	}

	public void SetLastLoginTime(DateTime date)
	{
		mLastLoginTime = date;
	}

	public byte GetPrestige()
	{
		return mPrestige;
	}

	public void SetPrestige(byte prestige)
	{
		mPrestige = prestige;
	}

	public byte GetFrameType()
	{
		return mFrameType;
	}

	public void SetFrameType(byte frameType)
	{
		mFrameType = frameType;
	}

	public byte GetTitleId()
	{
		return mTitleId;
	}

	public void SetTitleId(byte id)
	{
		mTitleId = id;
	}
}
