public class TryGetDailyLoginRewardRequest : Request
{
	public TryGetDailyLoginRewardRequest()
	{
		requestID = 41;
	}

	public override BytesBuffer GetBody()
	{
		return new BytesBuffer(0);
	}
}
