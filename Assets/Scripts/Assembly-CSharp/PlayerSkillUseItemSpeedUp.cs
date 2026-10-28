public class PlayerSkillUseItemSpeedUp : PlayerSpecialSkill
{
	private AdvancedSkill mAdvancedSkill;

	private PlayerSkill mPlayerSkill;

	private int currentStack;

	private Skill mSkill;

	private float addNumPerStack;

	public void Init(AdvancedSkill advancedSkill, PlayerSkill mPlayerSkill, float addNumPerStack)
	{
		mAdvancedSkill = advancedSkill;
		this.mPlayerSkill = mPlayerSkill;
		this.addNumPerStack = addNumPerStack;
		currentStack = 0;
		mSkill = new Skill();
		mSkill.skillType = SkillsType.SPEED_BOOTH;
		mSkill.data = 0f;
	}

	public void AddStack()
	{
		currentStack++;
		mPlayerSkill.RemoveSkill(mSkill);
		mSkill.data = (float)currentStack * addNumPerStack;
		mPlayerSkill.AddSkill(mSkill);
	}

	public int GetStackTimes()
	{
		return currentStack;
	}
}
