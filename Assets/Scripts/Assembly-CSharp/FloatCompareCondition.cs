public class FloatCompareCondition : BaseCompareCondition
{
	public FloatConditionDelegate mDelegate;

	public float mValue;

	public override bool IsTriggered(GameUnit creature)
	{
		return mDelegate(creature, mValue);
	}
}
