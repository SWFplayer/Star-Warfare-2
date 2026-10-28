using System.Collections.Generic;

public class CreatureConditionInfo
{
	public byte mCurrentId = 1;

	public ConditionState mInitState;

	public ConditionState mMoveState;

	public ConditionState mMindState;

	public Dictionary<GameUnitState, ConditionState> mStateDic = new Dictionary<GameUnitState, ConditionState>();

	public Dictionary<string, ConditionState> mStateTag = new Dictionary<string, ConditionState>();

	public List<ConditionState> mAllConditionState = new List<ConditionState>();
}
