using System.Collections.Generic;

public abstract class PvpSkill
{
	protected int mId;

	protected int mUnlockLevel;

	protected string mName;

	protected string mIconName;

	protected int mExternalId;

	protected string mDescription;

	protected SkillGroup mSkillGroup = new SkillGroup();

	public abstract void LoadConfig(int row);

	public abstract EPvpSkillType GetPvpSkillType();

	public void TakeOn(PlayerSkill playerSkill)
	{
		mSkillGroup.InitToPlayerSkill(playerSkill);
		List<AdvancedSkill> advancedSkills = mSkillGroup.GetAdvancedSkills();
		foreach (AdvancedSkill item in advancedSkills)
		{
			item.SetIconName(mIconName);
		}
	}

	public void TakeOff()
	{
		mSkillGroup.UninitFromPlayerSkill();
	}

	public int GetId()
	{
		return mId;
	}

	public string GetIconName()
	{
		return mIconName;
	}

	public int GetUnlockLevel()
	{
		return mUnlockLevel;
	}

	public string GetName()
	{
		return LocalizationManager.GetInstance().GetString(mName);
	}

	public int GetExternalID()
	{
		return mExternalId;
	}

	public virtual string GetDescription()
	{
		return LocalizationManager.GetInstance().GetString(mDescription);
	}

	public virtual string GetMessage()
	{
		return string.Empty;
	}
}
