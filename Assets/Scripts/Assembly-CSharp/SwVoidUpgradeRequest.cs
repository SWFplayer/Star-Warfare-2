public class SwVoidUpgradeRequest : Request
{
	private int mSwVoidId;

	public SwVoidUpgradeRequest(int swVoidId)
	{
		requestID = 67;
		mSwVoidId = swVoidId;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(4);
		bytesBuffer.AddInt(mSwVoidId);
		return bytesBuffer;
	}
}
