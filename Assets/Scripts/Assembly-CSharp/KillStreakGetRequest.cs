public class KillStreakGetRequest : Request
{
	public KillStreakGetRequest()
	{
		requestID = 144;
	}

	public override BytesBuffer GetBody()
	{
		return new BytesBuffer(0);
	}
}
