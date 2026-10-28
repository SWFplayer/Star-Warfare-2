public class SwVoidGetDataWWWRequest : SwVoidWWWRequest
{
	public SwVoidGetDataWWWRequest()
	{
		mResponseData = new SwVoidGetDataResponseData();
		mSign = Hash(mSendUserId.ToString());
	}

	protected override string GetUrl()
	{
		return "http://" + mVoidServerAddress + "/Sw2/SwVoidGetData?userId=" + mSendUserId + "&test=" + mSign;
	}

	public override void ProcessLogic()
	{
		SwVoidGetDataResponseData swVoidGetDataResponseData = mResponseData as SwVoidGetDataResponseData;
		if (!NeedVerify(swVoidGetDataResponseData.result))
		{
			SwVoidOnlineProcess.SwVoidGetDataResponse(swVoidGetDataResponseData);
		}
	}
}
