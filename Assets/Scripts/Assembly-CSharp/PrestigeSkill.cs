using System;

public class PrestigeSkill : PvpSkill
{
	public enum EPrestigeColumn
	{
		PRESTIGE = 0,
		FINAL_RES = 1,
		FINAL_DAMAGE = 2,
		SPEED_BOOTH = 3,
		FINAL_PVP_HP_RATE = 4,
		COUNT = 5
	}

	private bool hasSkill;

	public override EPvpSkillType GetPvpSkillType()
	{
		return EPvpSkillType.PRESTIGE_SKILL;
	}

	public override void LoadConfig(int row)
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.PVP_SKILL, DataSheetName.PVP_SKILL_PRESTIGE_SKILL);
		if (gameDataTable == null)
		{
			return;
		}
		mSkillGroup.source = "Prestige " + row;
		for (int i = 1; i < 5; i++)
		{
			if (gameDataTable.GetString(row, i) != null && gameDataTable.GetString(row, i) != string.Empty)
			{
				float num = gameDataTable.GetFloat(row, i);
				if (num != 0f)
				{
					SkillsType type = (SkillsType)(int)Enum.Parse(typeof(SkillsType), ((EPrestigeColumn)i).ToString());
					mSkillGroup.AddStaticSkill(type, num);
					hasSkill = true;
				}
			}
		}
	}

	public bool GetHasSkill()
	{
		return hasSkill;
	}
}
