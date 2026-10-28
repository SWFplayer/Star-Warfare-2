public class VoidCompareCondition : BaseCompareCondition
{
	public ConditionDelegate mDelegate;

	public override bool IsTriggered(GameUnit creature)
	{
		return mDelegate(creature);
	}
}
