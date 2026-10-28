public class PlayerPutNuclearRequest : Request
{
	public PlayerPutNuclearRequest()
	{
		requestID = 270;
	}

	public override BytesBuffer GetBody()
	{
		return new BytesBuffer(0);
	}
}
