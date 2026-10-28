public class BowSwordChangeRequest : Request
{
	public bool mIsBow;

	public BowSwordChangeRequest(bool isBow)
	{
		requestID = 142;
		mIsBow = isBow;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(1);
		bytesBuffer.AddBool(mIsBow);
		return bytesBuffer;
	}
}
