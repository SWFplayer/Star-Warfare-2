public class PlayerBossWinRequest : Request
{
	public PlayerBossWinRequest()
	{
		requestID = 256;
	}

	public override BytesBuffer GetBody()
	{
		return new BytesBuffer();
	}
}
