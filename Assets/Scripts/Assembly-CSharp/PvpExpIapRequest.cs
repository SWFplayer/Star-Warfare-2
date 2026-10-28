public class PvpExpIapRequest : Request
{
	public PvpExpIapRequest()
	{
		requestID = 74;
	}

	public override BytesBuffer GetBody()
	{
		return new BytesBuffer(0);
	}
}
