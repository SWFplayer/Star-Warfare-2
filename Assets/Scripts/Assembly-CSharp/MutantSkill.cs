public class MutantSkill : PvpSkill
{
	public enum EMutantColumn
	{
		ID = 0,
		BossIndex = 1,
		Order = 2,
		Name = 3,
		Damage = 4,
		CD = 5,
		Score = 6,
		IconName = 7,
		Description = 8,
		SkillType = 9,
		SKillId = 10,
		SkillInfoId = 11
	}

	protected byte mOrder;

	protected int mBossIndex;

	protected int mScore;

	protected float mCD;

	public override void LoadConfig(int row)
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.PVP_SKILL, DataSheetName.PVP_SKILL_MUTANT_SKILL);
		mId = gameDataTable.GetInt(row, 0);
		mOrder = (byte)gameDataTable.GetInt(row, 2);
		mBossIndex = gameDataTable.GetInt(row, 1);
		mName = gameDataTable.GetString(row, 3);
		mCD = gameDataTable.GetFloat(row, 5);
		mScore = gameDataTable.GetInt(row, 6);
		mIconName = gameDataTable.GetString(row, 7);
		mDescription = gameDataTable.GetString(row, 8);
	}

	public override EPvpSkillType GetPvpSkillType()
	{
		return EPvpSkillType.MUTANT_SKILL;
	}

	public int GetBossIndex()
	{
		return mBossIndex;
	}

	public float GetCD()
	{
		return mCD;
	}

	public int GetScore()
	{
		return mScore;
	}

	public byte GetOrder()
	{
		return mOrder;
	}

	public static bool isActiveBuff(int id)
	{
		if (id == 0)
		{
			return false;
		}
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.PVP_SKILL, DataSheetName.PVP_SKILL_MUTANT_SKILL);
		int row = id - 1;
		if (gameDataTable.GetString(row, 9) == "0")
		{
			return true;
		}
		return false;
	}

	public static float GetCdOfActiveBuff(int id)
	{
		if (id == 0)
		{
			return 0f;
		}
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.PVP_SKILL, DataSheetName.PVP_SKILL_MUTANT_SKILL);
		int row = id - 1;
		return gameDataTable.GetFloat(row, 5);
	}

	public static int GetSkillId(int id)
	{
		if (id == 0)
		{
			return 0;
		}
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.PVP_SKILL, DataSheetName.PVP_SKILL_MUTANT_SKILL);
		int row = id - 1;
		return gameDataTable.GetInt(row, 10);
	}

	public static int GetSkillInfoId(int id)
	{
		if (id == 0)
		{
			return 0;
		}
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.PVP_SKILL, DataSheetName.PVP_SKILL_MUTANT_SKILL);
		int row = id - 1;
		return gameDataTable.GetInt(row, 11);
	}
}
