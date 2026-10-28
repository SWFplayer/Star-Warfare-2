public class PlayerJoinTeamStartGameRequest : Request
{
	public PlayerJoinTeamStartGameRequest()
	{
		requestID = 19;
	}

	public override BytesBuffer GetBody()
	{
		byte length = 0;
		return new BytesBuffer(length);
	}
}
