public class VSGameEndRequest : Request
{
	public VSGameEndRequest()
	{
		requestID = 128;
	}

	public override BytesBuffer GetBody()
	{
		return new BytesBuffer(0);
	}
}
