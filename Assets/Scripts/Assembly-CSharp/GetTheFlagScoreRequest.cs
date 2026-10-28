public class GetTheFlagScoreRequest : Request
{
	protected int playerID;

	public GetTheFlagScoreRequest(int playerID)
	{
		requestID = 136;
		this.playerID = playerID;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(4);
		bytesBuffer.AddInt(playerID);
		return bytesBuffer;
	}
}
