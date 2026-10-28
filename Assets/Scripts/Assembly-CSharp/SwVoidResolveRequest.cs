public class SwVoidResolveRequest : Request
{
	private int mSwVoidId;

	public SwVoidResolveRequest(int swVoidId)
	{
		requestID = 66;
		mSwVoidId = swVoidId;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(4);
		bytesBuffer.AddInt(mSwVoidId);
		return bytesBuffer;
	}
}
