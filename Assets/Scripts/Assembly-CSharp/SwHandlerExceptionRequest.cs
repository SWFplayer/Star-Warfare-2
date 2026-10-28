public class SwHandlerExceptionRequest : WWWRequest
{
	private int mAccountId;

	private string mExMessage;

	public SwHandlerExceptionRequest(int accountId, string exMessage)
	{
		mAccountId = accountId;
		mExMessage = exMessage;
	}

	protected override string GetUrl()
	{
		return "http://" + GameApp.GetInstance().GetUserState().GetServerAddress() + ":" + GameApp.GetInstance().GetUserState().GetVoidServerPort() + "/Sw2/SwHandlerException";
	}

	public override void ProcessLogic()
	{
	}

	protected override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(16384);
		bytesBuffer.AddInt(mAccountId);
		bytesBuffer.AddStringShortLength(mExMessage);
		return bytesBuffer;
	}
}
