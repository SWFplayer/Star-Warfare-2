public class CatchTheFlagRequest : Request
{
	public CatchTheFlagRequest()
	{
		requestID = 135;
	}

	public override BytesBuffer GetBody()
	{
		return new BytesBuffer(0);
	}
}
