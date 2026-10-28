public class TryGetQuestGoldRequest : Request
{
	protected byte index;

	public TryGetQuestGoldRequest(byte index)
	{
		requestID = 43;
		this.index = index;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(1);
		bytesBuffer.AddByte(index);
		return bytesBuffer;
	}
}
