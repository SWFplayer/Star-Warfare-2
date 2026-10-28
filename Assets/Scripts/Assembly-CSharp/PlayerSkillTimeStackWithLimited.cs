using System.Collections.Generic;

public class PlayerSkillTimeStackWithLimited : PlayerSpecialSkill
{
	private AdvancedSkill mAdvancedSkill;

	private List<Skill> skillsBackUp;

	private Timer addStackTimer;

	private int limitedTimes;

	private int hasStackTimes;

	private PlayerSkill mPlayerSkill;

	public void Init(AdvancedSkill advancedSkill, PlayerSkill playerSkill, float timeToStack, int limited)
	{
		mAdvancedSkill = advancedSkill;
		mPlayerSkill = playerSkill;
		skillsBackUp = new List<Skill>();
		foreach (Skill skill2 in mAdvancedSkill.GetSkills())
		{
			Skill skill = new Skill();
			skill.data = skill2.data;
			skill.skillType = skill2.skillType;
			skillsBackUp.Add(skill);
		}
		addStackTimer = new Timer(timeToStack, false);
		limitedTimes = limited;
		hasStackTimes = 1;
	}

	public override void Loop()
	{
		if (hasStackTimes >= limitedTimes || !addStackTimer.Ready())
		{
			return;
		}
		addStackTimer.Do();
		mPlayerSkill.RemoveSkills(mAdvancedSkill.GetSkills());
		foreach (Skill skill in mAdvancedSkill.GetSkills())
		{
			foreach (Skill item in skillsBackUp)
			{
				if (skill.skillType == item.skillType)
				{
					skill.data += item.data;
				}
			}
		}
		mPlayerSkill.AddSkills(mAdvancedSkill.GetSkills());
		hasStackTimes++;
	}

	public void Uninit()
	{
		foreach (Skill skill in mAdvancedSkill.GetSkills())
		{
			foreach (Skill item in skillsBackUp)
			{
				if (skill.skillType == item.skillType)
				{
					skill.data = item.data;
				}
			}
		}
	}

	public int GetStackTimes()
	{
		return hasStackTimes;
	}
}
