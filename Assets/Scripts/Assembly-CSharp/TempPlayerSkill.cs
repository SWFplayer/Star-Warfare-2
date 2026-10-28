using System.Collections.Generic;
using UnityEngine;

public class TempPlayerSkill
{
	protected List<Skill> skills;

	protected List<Skill> equipedSkills;

	public static TempPlayerSkill instance;

	public TempPlayerSkill()
	{
		instance = this;
		CreateSkills();
	}

	public void CreateSkills()
	{
		skills = new List<Skill>();
		for (int i = 0; i < 104; i++)
		{
			Skill skill = new Skill();
			skill.skillType = (SkillsType)i;
			skill.data = 0f;
			skills.Add(skill);
		}
		equipedSkills = new List<Skill>();
		for (int j = 0; j < 104; j++)
		{
			Skill skill2 = new Skill();
			skill2.skillType = (SkillsType)j;
			skill2.data = 0f;
			equipedSkills.Add(skill2);
		}
	}

	public void AddSkill(Skill skill)
	{
		if (skill.skillType != SkillsType.SKILL_COUNT)
		{
			if (Skill.isPercent(skill.skillType))
			{
				skills[(int)skill.skillType].data += skill.data / 100f;
			}
			else
			{
				skills[(int)skill.skillType].data += skill.data;
			}
		}
	}

	public void AddSkills(List<Skill> skills)
	{
		foreach (Skill skill in skills)
		{
			AddSkill(skill);
		}
	}

	public void RemoveSkill(Skill skill)
	{
		if (skill.skillType != SkillsType.SKILL_COUNT)
		{
			if (Skill.isPercent(skill.skillType))
			{
				skills[(int)skill.skillType].data -= skill.data / 100f;
			}
			else
			{
				skills[(int)skill.skillType].data -= skill.data;
			}
			if ((double)skills[(int)skill.skillType].data < 1E-05 && (double)skills[(int)skill.skillType].data > -1E-05)
			{
				skills[(int)skill.skillType].data = 0f;
			}
		}
	}

	public void RemoveSkills(List<Skill> skills)
	{
		foreach (Skill skill in skills)
		{
			RemoveSkill(skill);
		}
	}

	public float GetSkill(SkillsType sType)
	{
		return (sType != SkillsType.SKILL_COUNT && sType != SkillsType.SKILL_NONE) ? skills[(int)sType].data : 0f;
	}

	public bool HasSkill(SkillsType sType)
	{
		return skills[(int)sType].data != 0f;
	}

	public void SetSkillFromUserState(UserState userState)
	{
		CreateSkills();
		for (int i = 0; i < skills.Count; i++)
		{
			skills[i].data = userState.playerSkill.GetSkill((SkillsType)i);
			equipedSkills[i].data = userState.playerSkill.GetSkill((SkillsType)i);
		}
	}

	private void CopySkillList()
	{
		for (int i = 0; i < skills.Count; i++)
		{
			skills[i].data = equipedSkills[i].data;
		}
	}

	public void ChangeEquipment(short[] currentStatus, short[] newStatus, UserState userState)
	{
		CopySkillList();
		bool flag = false;
		for (int i = 0; i < currentStatus.Length; i++)
		{
			if (currentStatus[i] != newStatus[i] && i <= 4)
			{
				ChangeEquipment(i, currentStatus[i], newStatus[i], userState);
				if (i <= 3)
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			ChangeArmorSet(currentStatus, newStatus, userState);
		}
	}

	public void ChangeEquipment(int i, short oldId, short newId, UserState userState)
	{
		Armor armor = userState.GetArmor(i, oldId);
		if (armor != null)
		{
			foreach (AdvancedSkill advancedSkill in armor.GetSkillGroup().GetAdvancedSkills())
			{
				if (advancedSkill.IsStaticForTempSkillUI())
				{
					RemoveSkills(advancedSkill.GetSkills());
				}
			}
		}
		armor = userState.GetArmor(i, newId);
		if (armor == null)
		{
			return;
		}
		foreach (AdvancedSkill advancedSkill2 in armor.GetSkillGroup().GetAdvancedSkills())
		{
			if (advancedSkill2.IsStaticForTempSkillUI())
			{
				AddSkills(advancedSkill2.GetSkills());
			}
		}
	}

	public void ChangeArmorSet(short[] currentStatus, short[] newStatus, UserState userState)
	{
		ArmorSet mCurrentArmorSet = userState.mCurrentArmorSet;
		short id = ArmorSetId(currentStatus, userState);
		bool flag = isSuitSet(currentStatus, userState);
		SkillGroup skillGropById = mCurrentArmorSet.GetSkillGropById(id, flag);
		if (skillGropById != null)
		{
			foreach (AdvancedSkill advancedSkill in skillGropById.GetAdvancedSkills())
			{
				if (advancedSkill.IsStaticForTempSkillUI())
				{
					RemoveSkills(advancedSkill.GetSkills());
				}
			}
		}
		short id2 = ArmorSetId(newStatus, userState);
		flag = isSuitSet(newStatus, userState);
		skillGropById = mCurrentArmorSet.GetSkillGropById(id2, flag);
		if (skillGropById == null)
		{
			return;
		}
		foreach (AdvancedSkill advancedSkill2 in skillGropById.GetAdvancedSkills())
		{
			if (advancedSkill2.IsStaticForTempSkillUI())
			{
				AddSkills(advancedSkill2.GetSkills());
			}
		}
	}

	private short ArmorSetId(short[] status, UserState userState)
	{
		byte[] array = new byte[4];
		bool flag = true;
		short num = status[0];
		for (int i = 0; i < array.Length; i++)
		{
			Armor armor = userState.GetArmor(i, status[i]);
			if (armor != null)
			{
				array[i] = armor.SetElement;
			}
			if (num != status[i])
			{
				flag = false;
			}
		}
		short num2 = 0;
		num2 += (short)(array[0] * 1000);
		num2 += (short)(array[1] * 100);
		num2 += (short)(array[2] * 10);
		num2 += array[3];
		if (flag)
		{
			return num;
		}
		return num2;
	}

	private bool isSuitSet(short[] status, UserState userState)
	{
		bool result = true;
		short num = status[0];
		for (int i = 0; i < 4; i++)
		{
			Armor armor = userState.GetArmor(i, status[i]);
			if (num != status[i])
			{
				result = false;
			}
		}
		return result;
	}

	public void DebugLog()
	{
		for (int i = 0; i < skills.Count; i++)
		{
			if (skills[i].data != 0f)
			{
				Debug.Log(string.Concat(skills[i].skillType, " : ", skills[i].data));
			}
		}
	}
}
