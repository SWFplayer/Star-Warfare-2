using System.Collections.Generic;
using UnityEngine;

public class ConditionState : GraphNode
{
	public byte ID;

	public GameUnitState mState;

	public List<Condition> mConditionList = new List<Condition>();

	public List<EConditionMessage> mStartMessageList = new List<EConditionMessage>();

	public List<EConditionMessage> mEndMessageList = new List<EConditionMessage>();

	public ConditionState()
	{
		Type = EGraphNodeType.CONDITION_STATE;
	}

	public void SetState(GameUnitState state)
	{
		mState = state;
	}

	public void AddCondition(Condition condition)
	{
		mConditionList.Add(condition);
	}

	public void AddCondition(ConditionDelegate conditionDelegate, GraphNode nextState)
	{
		Condition condition = new Condition();
		VoidCompareCondition voidCompareCondition = new VoidCompareCondition();
		voidCompareCondition.mDelegate = conditionDelegate;
		condition.mBaseConditionList.Add(voidCompareCondition);
		condition.mNextNode = nextState;
		mConditionList.Add(condition);
	}

	public void AddCondition(ConditionDelegate conditonDelegate1, ConditionDelegate conditonDelegate2, GraphNode nextState)
	{
		Condition condition = new Condition();
		VoidCompareCondition voidCompareCondition = new VoidCompareCondition();
		voidCompareCondition.mDelegate = conditonDelegate1;
		VoidCompareCondition voidCompareCondition2 = new VoidCompareCondition();
		voidCompareCondition2.mDelegate = conditonDelegate2;
		condition.mBaseConditionList.Add(voidCompareCondition);
		condition.mBaseConditionList.Add(voidCompareCondition2);
		condition.mNextNode = nextState;
		mConditionList.Add(condition);
	}

	public void AddCondition(ConditionDelegate conditonDelegate1, ConditionDelegate conditonDelegate2, ConditionDelegate conditonDelegate3, GraphNode nextState)
	{
		Condition condition = new Condition();
		VoidCompareCondition voidCompareCondition = new VoidCompareCondition();
		voidCompareCondition.mDelegate = conditonDelegate1;
		VoidCompareCondition voidCompareCondition2 = new VoidCompareCondition();
		voidCompareCondition2.mDelegate = conditonDelegate2;
		VoidCompareCondition voidCompareCondition3 = new VoidCompareCondition();
		voidCompareCondition3.mDelegate = conditonDelegate3;
		condition.mBaseConditionList.Add(voidCompareCondition);
		condition.mBaseConditionList.Add(voidCompareCondition2);
		condition.mBaseConditionList.Add(voidCompareCondition3);
		condition.mNextNode = nextState;
		mConditionList.Add(condition);
	}

	public void AddCondition(FloatConditionDelegate conditionDelegate, float value, GraphNode nextState)
	{
		Condition condition = new Condition();
		FloatCompareCondition floatCompareCondition = new FloatCompareCondition();
		floatCompareCondition.mDelegate = conditionDelegate;
		floatCompareCondition.mValue = value;
		condition.mBaseConditionList.Add(floatCompareCondition);
		condition.mNextNode = nextState;
		mConditionList.Add(condition);
	}

	public void AddCondition(Float2ConditionDelegate conditionDelegate, float value1, float value2, GraphNode nextState)
	{
		Condition condition = new Condition();
		Float2CompareCondition float2CompareCondition = new Float2CompareCondition();
		float2CompareCondition.mDelegate = conditionDelegate;
		float2CompareCondition.mValue1 = value1;
		float2CompareCondition.mValue2 = value2;
		condition.mBaseConditionList.Add(float2CompareCondition);
		condition.mNextNode = nextState;
		mConditionList.Add(condition);
	}

	public void AddCondition(GraphNode nextState)
	{
		Condition condition = new Condition();
		condition.mNextNode = nextState;
		mConditionList.Add(condition);
	}

	public void AddConditonStateFinish(ConditionDelegate conditionDelegate, GraphNode nextState)
	{
		Condition condition = new Condition();
		StateFinishCondition stateFinishCondition = new StateFinishCondition();
		stateFinishCondition.mGameUnitState = mState;
		StateFinishCondition baseCondition = stateFinishCondition;
		condition.AddBaseCondition(baseCondition);
		VoidCompareCondition voidCompareCondition = new VoidCompareCondition();
		voidCompareCondition.mDelegate = conditionDelegate;
		condition.AddBaseCondition(voidCompareCondition);
		condition.mNextNode = nextState;
		AddCondition(condition);
	}

	public void AddConditonStateFinish(GraphNode nextState)
	{
		Condition condition = new Condition();
		StateFinishCondition stateFinishCondition = new StateFinishCondition();
		stateFinishCondition.mGameUnitState = mState;
		StateFinishCondition baseCondition = stateFinishCondition;
		condition.AddBaseCondition(baseCondition);
		condition.mNextNode = nextState;
		AddCondition(condition);
	}

	public void AddStartMessage(EConditionMessage message)
	{
		mStartMessageList.Add(message);
	}

	public void AddEndMessage(EConditionMessage message)
	{
		mEndMessageList.Add(message);
	}

	public void Update(GameUnit creature)
	{
		bool flag = false;
		if (creature.GameUnitType == EGameUnitType.ROBOT)
		{
			mState.Update(creature);
			flag = true;
		}
		else if (creature.GameUnitType == EGameUnitType.ENEMY)
		{
			((EnemyState)mState).Update((Enemy)creature, Time.deltaTime);
			flag = ((Enemy)creature).IsMasterPlayer;
		}
		if (flag)
		{
			for (int i = 0; i < mConditionList.Count && (!mConditionList[i].IsTriggered(creature) || !mConditionList[i].ActivateNextState(creature)); i++)
			{
			}
		}
	}
}
