public class StartGameRequest : Request
{
	public StartGameRequest()
	{
		requestID = 9;
	}

	public override BytesBuffer GetBody()
	{
		byte length = 0;
		return new BytesBuffer(length);
	}
}
