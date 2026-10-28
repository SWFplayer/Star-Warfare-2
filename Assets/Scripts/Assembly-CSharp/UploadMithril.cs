public class UploadMithril : Request
{
	protected UserState userState;

	public UploadMithril(UserState userState)
	{
		requestID = 16;
		this.userState = userState;
	}

	public override BytesBuffer GetBody()
	{
		byte length = 4;
		BytesBuffer bytesBuffer = new BytesBuffer(length);
		userState.WriteMithril(bytesBuffer);
		return bytesBuffer;
	}
}
