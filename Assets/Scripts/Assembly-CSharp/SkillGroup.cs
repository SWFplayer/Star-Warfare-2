using System;
using System.Collections.Generic;

public class SkillGroup
{
	protected List<AdvancedSkill> advancedSkills = new List<AdvancedSkill>();

	protected PlayerSkill mPlayerSkill;

	public string source = string.Empty;

	public void InitToPlayerSkill(PlayerSkill playerSkill)
	{
		mPlayerSkill = playerSkill;
		foreach (AdvancedSkill advancedSkill in advancedSkills)
		{
			advancedSkill.InitToPlayerSkill(playerSkill);
		}
	}

	public void UninitFromPlayerSkill()
	{
		mPlayerSkill = null;
		foreach (AdvancedSkill advancedSkill in advancedSkills)
		{
			advancedSkill.UninitFromPlayerSkill();
		}
	}

	public void AddStaticSkill(SkillsType type, float val)
	{
		AdvancedSkill advancedSkill = null;
		foreach (AdvancedSkill advancedSkill2 in advancedSkills)
		{
			if (advancedSkill2.GetOnTriggerType() == AdvancedSkill.EOnTriggerType.ALWAYS && advancedSkill2.GetOffTriggerType() == AdvancedSkill.EOffTriggerType.NEVER)
			{
				advancedSkill = advancedSkill2;
			}
		}
		if (advancedSkill == null)
		{
			advancedSkill = new AdvancedSkill();
			advancedSkill.source = source;
			advancedSkills.Add(advancedSkill);
		}
		advancedSkill.AddSkill(type, val);
	}

	public List<Skill> GetSkills()
	{
		return advancedSkills[0].GetSkills();
	}

	public AdvancedSkill GetFirstAdvancedSkill()
	{
		return advancedSkills[0];
	}

	public List<AdvancedSkill> GetAdvancedSkills()
	{
		return advancedSkills;
	}

	public void InitSkillInfo(int externalId)
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.SKILL, DataSheetName.SKILL_ALL);
		for (int i = 0; i < gameDataTable.Count - 1; i++)
		{
			if (gameDataTable.GetInt(i, 1) != externalId)
			{
				continue;
			}
			AdvancedSkill advancedSkill = new AdvancedSkill();
			advancedSkill.source = source;
			advancedSkill.SetExternalId(externalId);
			advancedSkill.SetIconName(gameDataTable.GetString(i, 3));
			advancedSkill.SetBuffDesc(gameDataTable.GetString(i, 4));
			advancedSkill.SetBuffState((AdvancedSkill.EBuffState)gameDataTable.GetInt(i, 5));
			advancedSkill.SetOnTriggerType(gameDataTable.GetInt(i, 6));
			advancedSkill.SetOffTriggerType(gameDataTable.GetInt(i, 9));
			advancedSkill.SetOnTriggerValues(gameDataTable.GetString(i, 7), gameDataTable.GetString(i, 8));
			advancedSkill.SetOffTriggerValues(gameDataTable.GetString(i, 10), gameDataTable.GetString(i, 11));
			advancedSkill.SetSpecialSkillType(gameDataTable.GetString(i, 12));
			advancedSkill.SetSpecialSkillValue(gameDataTable.GetString(i, 13));
			for (int j = 14; j < 79; j++)
			{
				if (gameDataTable.GetString(i, j) != string.Empty)
				{
					float num = gameDataTable.GetFloat(i, j);
					if (num != 0f)
					{
						SkillsType type = (SkillsType)(int)Enum.Parse(typeof(SkillsType), ((ESkillInfoColumn)j).ToString());
						advancedSkill.AddSkill(type, num);
					}
				}
			}
			advancedSkills.Add(advancedSkill);
		}
	}
}
