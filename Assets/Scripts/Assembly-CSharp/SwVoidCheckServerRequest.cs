public class SwVoidCheckServerRequest : Request
{
	public SwVoidCheckServerRequest()
	{
		requestID = 61;
	}

	public override BytesBuffer GetBody()
	{
		return new BytesBuffer(0);
	}
}
