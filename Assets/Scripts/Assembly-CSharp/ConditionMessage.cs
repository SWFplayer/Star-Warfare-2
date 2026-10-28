public class ConditionMessage : GraphNode
{
	public EConditionMessage mMessage;

	public MessageBaseData mData;

	public ConditionMessage()
	{
		Type = EGraphNodeType.CONDITION_MESSAGE;
	}

	public ConditionMessage(EConditionMessage message)
	{
		Type = EGraphNodeType.CONDITION_MESSAGE;
		mMessage = message;
	}
}
