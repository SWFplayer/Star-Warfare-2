public class SwVoidResolveWWWRequest : SwVoidWWWRequest
{
	private int mSendVoidId;

	public SwVoidResolveWWWRequest(int voidId)
	{
		mSendVoidId = voidId;
		mResponseData = new SwVoidResolveResponseData();
		mSign = Hash(mSendUserId.ToString() + mSendVoidId);
	}

	protected override string GetUrl()
	{
		return "http://" + mVoidServerAddress + "/Sw2/SwVoidResolve?userId=" + mSendUserId + "&voidId=" + mSendVoidId + "&test=" + mSign;
	}

	public override void ProcessLogic()
	{
		SwVoidResolveResponseData swVoidResolveResponseData = mResponseData as SwVoidResolveResponseData;
		if (!NeedVerify(swVoidResolveResponseData.result))
		{
			SwVoidOnlineProcess.SwVoidResolveResponse(swVoidResolveResponseData);
		}
	}
}
