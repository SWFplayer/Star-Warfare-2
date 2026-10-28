public class ReStartGameRequest : Request
{
	public ReStartGameRequest()
	{
		requestID = 131;
	}

	public override BytesBuffer GetBody()
	{
		byte length = 0;
		return new BytesBuffer(length);
	}
}
