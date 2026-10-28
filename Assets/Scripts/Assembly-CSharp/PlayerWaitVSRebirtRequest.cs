public class PlayerWaitVSRebirtRequest : Request
{
	public PlayerWaitVSRebirtRequest()
	{
		requestID = 146;
	}

	public override BytesBuffer GetBody()
	{
		return new BytesBuffer(0);
	}
}
