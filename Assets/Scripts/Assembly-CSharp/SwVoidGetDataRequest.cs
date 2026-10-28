public class SwVoidGetDataRequest : Request
{
	public SwVoidGetDataRequest()
	{
		requestID = 63;
	}

	public override BytesBuffer GetBody()
	{
		return new BytesBuffer(0);
	}
}
