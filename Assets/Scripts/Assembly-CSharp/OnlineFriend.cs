using System;

public class OnlineFriend
{
	private int mUserId;

	private string mName;

	private int mExp;

	private int mPrestige;

	private FriendType mPortraitType;

	private long mPortraitId;

	private string mTwitterName;

	private int mMaxCount;

	private int mTitleId;

	private int mFrameType;

	private int mMissionNum;

	private int mWeaponNum;

	private int mEquipNum;

	private int mSwVoidNum;

	private int mTotalPlayTime;

	private DateTime mLastLoginTime;

	private OnlineCustomizeData mCustomize = new OnlineCustomizeData();

	private SwVoidSavedata mSwVoidData = new SwVoidSavedata();

	private AchievementState mAchievement = new AchievementState();

	public SocialPlayer SocialPlayer;

	public OnlineFriend()
	{
	}

	public OnlineFriend(OnlineFriendResponseData data)
	{
		mUserId = data.mUserId;
		mName = data.mName;
		mExp = data.mExp;
		mPrestige = data.mPrestige;
		mPortraitType = data.mPortraitType;
		mPortraitId = data.mPortraitId;
		mTwitterName = data.mTwitterName;
		mTitleId = data.mTitleId;
		mFrameType = data.mTitleFrameId;
		mMaxCount = data.mMaxCount;
		mMissionNum = data.mMissionNum;
		mWeaponNum = data.mWeaponNum;
		mEquipNum = data.mEquipNum;
		mSwVoidNum = data.mSwVoidNum;
		mTotalPlayTime = data.mTotalPlayTime;
		mLastLoginTime = data.mLastLoginTime;
		mCustomize = new OnlineCustomizeData(data.mCustomize);
		if (mCustomize.mSwVoidId != 0)
		{
			mSwVoidData = new SwVoidSavedata(data.mSwVoid);
		}
		mAchievement = data.mAchievement;
		SocialPlayer = SocialManager.GetInstance().GetSocialPlayer(mPortraitType, mPortraitId, mTwitterName);
	}

	public void SetUserId(int userId)
	{
		mUserId = userId;
	}

	public int GetUserId()
	{
		return mUserId;
	}

	public void SetName(string name)
	{
		mName = name;
	}

	public string GetName()
	{
		return mName;
	}

	public void SetExp(int exp)
	{
		mExp = exp;
	}

	public int GetExp()
	{
		return mExp;
	}

	public void SetProtraitType(FriendType type)
	{
		mPortraitType = type;
	}

	public FriendType GetPortraitType()
	{
		return mPortraitType;
	}

	public void SetPortraitId(long id)
	{
		mPortraitId = id;
	}

	public long GetPortraitId()
	{
		return mPortraitId;
	}

	public void SetTwitterName(string name)
	{
		mTwitterName = name;
	}

	public string GetTwitterName()
	{
		return mTwitterName;
	}

	public void SetMaxCount(int maxCount)
	{
		mMaxCount = maxCount;
	}

	public int GetMaxCount()
	{
		return mMaxCount;
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

	public void SetCustomize(OnlineCustomizeData data)
	{
		mCustomize = new OnlineCustomizeData(data);
	}

	public OnlineCustomizeData GetCustomize()
	{
		return mCustomize;
	}

	public void SetSwVoidData(SwVoidSavedata data)
	{
		mSwVoidData = data;
	}

	public SwVoidSavedata GetSwVoidData()
	{
		return mSwVoidData;
	}

	public void SetAchievement(AchievementState achievement)
	{
		mAchievement = achievement;
	}

	public AchievementState GetAchievement()
	{
		return mAchievement;
	}

	public int GetPrestige()
	{
		return mPrestige;
	}

	public void SetPrestige(int prestige)
	{
		mPrestige = prestige;
	}

	public int GetFrameType()
	{
		return mFrameType;
	}

	public void SetFrameType(int frameType)
	{
		mFrameType = frameType;
	}

	public int GetTitleId()
	{
		return mTitleId;
	}

	public void SetTitleId(int id)
	{
		mTitleId = id;
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

	public PlayerSkill GetFriendSkill()
	{
		PlayerSkill playerSkill = new PlayerSkill();
		UserState userState = GameApp.GetInstance().GetUserState();
		playerSkill.Clear();
		playerSkill.CreateSkills();
		byte[] array = new byte[4];
		byte[] array2 = new byte[4];
		for (int i = 0; i < mCustomize.mAvatarIdArray.Length; i++)
		{
			Armor armor = userState.GetArmor(i, mCustomize.mAvatarIdArray[i]);
			armor.TakeOn(playerSkill);
			if (i < array.Length)
			{
				array[i] = armor.SetElement;
			}
			if (i < array2.Length)
			{
				array2[i] = armor.SuitId;
			}
		}
		if (mCustomize.mPvpSkillIdArray != null)
		{
			for (int j = 0; j < mCustomize.mPvpSkillIdArray.Length; j++)
			{
				PvpSkill perkById = userState.GetPvpSkillManager().GetPerkById(mCustomize.mPvpSkillIdArray[j]);
				if (perkById != null)
				{
					perkById.TakeOn(playerSkill);
				}
			}
		}
		if (mCustomize.mAwardSkillIdArray != null)
		{
			for (int k = 0; k < mCustomize.mAwardSkillIdArray.Length; k++)
			{
				PvpSkill killStreakById = userState.GetPvpSkillManager().GetKillStreakById(mCustomize.mAwardSkillIdArray[k]);
				if (killStreakById != null)
				{
					killStreakById.TakeOn(playerSkill);
				}
			}
		}
		userState.mCurrentArmorSet.SetElementSets(array, array2);
		userState.mCurrentArmorSet.InitArmorSet(playerSkill);
		return playerSkill;
	}
}
