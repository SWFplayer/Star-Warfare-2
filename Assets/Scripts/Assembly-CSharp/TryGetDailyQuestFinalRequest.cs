public class TryGetDailyQuestFinalRequest : Request
{
	public TryGetDailyQuestFinalRequest()
	{
		requestID = 44;
	}

	public override BytesBuffer GetBody()
	{
		return new BytesBuffer(0);
	}
}
