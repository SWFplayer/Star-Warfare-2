public class KillStreak : PvpSkill
{
	public enum EKillStreakType
	{
		COMBO = 0,
		ACCUMULATE = 1
	}

	public enum EKillStreakColumn
	{
		ID = 0,
		UNLOCK = 1,
		NAME = 2,
		TYPE = 3,
		KILL_AMOUNT = 4,
		SKILL_INFO_ID = 5,
		ICON_NAME = 6,
		DESCRIPTION = 7,
		MESSAGE = 8
	}

	private EKillStreakType mKillStreakType;

	private int mKillAmountTarget;

	private string mMessage;

	public override void LoadConfig(int row)
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.PVP_SKILL, DataSheetName.PVP_SKILL_KILL_STREAK);
		if (gameDataTable == null)
		{
			return;
		}
		mId = gameDataTable.GetInt(row, 0);
		mSkillGroup.source = "Kill Streak " + mId;
		mUnlockLevel = gameDataTable.GetInt(row, 1);
		mName = gameDataTable.GetString(row, 2);
		mKillStreakType = (EKillStreakType)gameDataTable.GetInt(row, 3);
		mKillAmountTarget = gameDataTable.GetInt(row, 4);
		mIconName = gameDataTable.GetString(row, 6);
		mDescription = gameDataTable.GetString(row, 7);
		mMessage = gameDataTable.GetString(row, 8);
		if (gameDataTable.GetString(row, 5) != string.Empty)
		{
			mExternalId = gameDataTable.GetInt(row, 5);
			if (mExternalId != 0)
			{
				mSkillGroup.InitSkillInfo(mExternalId);
			}
		}
	}

	public override EPvpSkillType GetPvpSkillType()
	{
		return EPvpSkillType.KILL_STREAK;
	}

	public override string GetDescription()
	{
		if (mKillStreakType == EKillStreakType.COMBO)
		{
			return LocalizationManager.GetInstance().GetString(mDescription) + " " + LocalizationManager.GetInstance().GetString("LOC_KILLSTREAK_TIPS_2", mKillAmountTarget);
		}
		return LocalizationManager.GetInstance().GetString(mDescription) + " " + LocalizationManager.GetInstance().GetString("LOC_KILLSTREAK_TIPS_3", mKillAmountTarget);
	}

	public override string GetMessage()
	{
		return mMessage;
	}
}
