public class StateFinishCondition : BaseCompareCondition
{
	public GameUnitState mGameUnitState;

	public override bool IsTriggered(GameUnit gameUnit)
	{
		return mGameUnitState.IsFinish(gameUnit);
	}
}
