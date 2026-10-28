public class SwVoidCreateWWWRequest : SwVoidWWWRequest
{
	private ESwVoidType mSendType;

	public SwVoidCreateWWWRequest(ESwVoidType type)
	{
		mSendType = type;
		mResponseData = new SwVoidCreateResponseData();
		mSign = Hash(mSendUserId.ToString() + (byte)mSendType);
	}

	protected override string GetUrl()
	{
		return "http://" + mVoidServerAddress + "/Sw2/SwVoidCreate?userId=" + mSendUserId + "&type=" + (byte)mSendType + "&test=" + mSign;
	}

	public override void ProcessLogic()
	{
		SwVoidCreateResponseData swVoidCreateResponseData = mResponseData as SwVoidCreateResponseData;
		if (!NeedVerify(swVoidCreateResponseData.result))
		{
			SwVoidOnlineProcess.SwVoidCreateResponse(swVoidCreateResponseData);
		}
	}
}
