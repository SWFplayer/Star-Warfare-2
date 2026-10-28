public class UploadBattleStateRequest : Request
{
	protected UserState userState;

	public UploadBattleStateRequest(UserState userState)
	{
		requestID = 22;
		this.userState = userState;
	}

	public override BytesBuffer GetBody()
	{
		byte length = 48;
		return new BytesBuffer(length);
	}
}
