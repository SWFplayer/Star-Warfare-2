public class Float2CompareCondition : BaseCompareCondition
{
	public Float2ConditionDelegate mDelegate;

	public float mValue1;

	public float mValue2;

	public override bool IsTriggered(GameUnit creature)
	{
		return mDelegate(creature, mValue1, mValue2);
	}
}
