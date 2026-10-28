public class GraphNode
{
	public EGraphNodeType Type = EGraphNodeType.NONE;

	public static implicit operator GraphNode(EConditionMessage message)
	{
		return new ConditionMessage(message);
	}
}
