public class PlayerRenameRequest : Request
{
	protected string mPlayerName;

	public PlayerRenameRequest(string playerName)
	{
		requestID = 75;
		mPlayerName = playerName;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer();
		bytesBuffer.AddString(mPlayerName);
		return bytesBuffer;
	}
}
