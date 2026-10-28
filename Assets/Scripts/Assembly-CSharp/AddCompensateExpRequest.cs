public class AddCompensateExpRequest : Request
{
	private int mExp;

	public AddCompensateExpRequest(int exp)
	{
		requestID = 76;
		mExp = exp;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(4);
		bytesBuffer.AddInt(mExp);
		return bytesBuffer;
	}
}
