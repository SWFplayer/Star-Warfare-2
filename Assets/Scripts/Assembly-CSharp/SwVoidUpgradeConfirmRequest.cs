public class SwVoidUpgradeConfirmRequest : Request
{
	private int mSwVoidId;

	public SwVoidUpgradeConfirmRequest(int swVoidId)
	{
		requestID = 68;
		mSwVoidId = swVoidId;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(4);
		bytesBuffer.AddInt(mSwVoidId);
		return bytesBuffer;
	}
}
