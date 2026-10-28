public class PlayerPrestigeRequest : Request
{
	public PlayerPrestigeRequest()
	{
		requestID = 71;
	}

	public override BytesBuffer GetBody()
	{
		byte length = 0;
		return new BytesBuffer(length);
	}
}
