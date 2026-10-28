using System;

public class Perk : PvpSkill
{
	public enum EPerkColumn
	{
		ID = 0,
		TYPE = 1,
		UNLOCK = 2,
		NAME = 3,
		SKILL_INFO_ID = 4,
		ASSAULT_BOOTH = 5,
		SHOTGUN_BOOTH = 6,
		RPG_BOOTH = 7,
		SWORD_BOOTH = 8,
		SNIPER_BOOTH = 9,
		PISTOL_BOOTH = 10,
		ASSAULT_ACC = 11,
		SHOTGUN_ACC = 12,
		RPG_ACC = 13,
		SWORD_ACC = 14,
		SNIPER_ACC = 15,
		PISTOL_ACC = 16,
		ASSAULT_DEFENCE = 17,
		SHOTGUN_DEFENCE = 18,
		RPG_DEFENCE = 19,
		SWORD_DEFENCE = 20,
		SNIPER_DEFENCE = 21,
		PISTOL_DEFENCE = 22,
		REBIRTH_COOLDOWN_DECREASE = 23,
		ASS_ATTACK = 24,
		RECOVERY_BOOTH = 25,
		HP_RECOVERY_WHEN_MAKE_KILL = 26,
		KILL_STREAK_DECREASE = 27,
		KILL_STREAK_ASSIST = 28,
		RADAR_RANGE_ENLARGE = 29,
		RADAR_INVISIBLE = 30,
		ICON = 31,
		DESCRIPTION = 32
	}

	public enum EPerkType
	{
		ATTACK = 0,
		DEFENCE = 1,
		GENERAL = 2,
		NUM = 3
	}

	private EPerkType mPerkType;

	public override void LoadConfig(int row)
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.PVP_SKILL, DataSheetName.PVP_SKILL_PERK);
		if (gameDataTable == null)
		{
			return;
		}
		mId = gameDataTable.GetInt(row, 0);
		mSkillGroup.source = "Perk " + mId;
		mPerkType = (EPerkType)(gameDataTable.GetInt(row, 1) - 1);
		mUnlockLevel = gameDataTable.GetInt(row, 2);
		mName = gameDataTable.GetString(row, 3);
		mIconName = gameDataTable.GetString(row, 31);
		mDescription = gameDataTable.GetString(row, 32);
		if (gameDataTable.GetString(row, 4) != string.Empty)
		{
			mExternalId = gameDataTable.GetInt(row, 4);
			if (mExternalId != 0)
			{
				mSkillGroup.InitSkillInfo(mExternalId);
			}
		}
		for (EPerkColumn ePerkColumn = EPerkColumn.ASSAULT_BOOTH; ePerkColumn < EPerkColumn.ICON; ePerkColumn++)
		{
			if (gameDataTable.GetString(row, (int)ePerkColumn) != string.Empty)
			{
				float num = gameDataTable.GetFloat(row, (int)ePerkColumn);
				if (num != 0f)
				{
					SkillsType type = (SkillsType)(int)Enum.Parse(typeof(SkillsType), ePerkColumn.ToString());
					mSkillGroup.AddStaticSkill(type, num);
				}
			}
		}
	}

	public override EPvpSkillType GetPvpSkillType()
	{
		return EPvpSkillType.PERK;
	}

	public EPerkType GetPerkType()
	{
		return mPerkType;
	}
}
