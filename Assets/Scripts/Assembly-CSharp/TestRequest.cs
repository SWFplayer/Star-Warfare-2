public class TestRequest : Request
{
	public TestRequest()
	{
		requestID = 50;
	}

	public override BytesBuffer GetBody()
	{
		byte length = 20;
		BytesBuffer bytesBuffer = new BytesBuffer(length);
		for (int i = 0; i < 5; i++)
		{
			bytesBuffer.AddInt(1);
		}
		return bytesBuffer;
	}
}
