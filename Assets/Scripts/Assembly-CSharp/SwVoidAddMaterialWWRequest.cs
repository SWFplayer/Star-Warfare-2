public class SwVoidAddMaterialWWRequest : SwVoidWWWRequest
{
	public SwVoidAddMaterialWWRequest()
	{
		mResponseData = new SwVoidGetDataResponseData();
	}

	protected override string GetUrl()
	{
		return "http://" + mVoidServerAddress + "/Sw2/SwVoidAddMaterial?userId=" + mSendUserId;
	}

	public override void ProcessLogic()
	{
		SwVoidGetDataResponseData swVoidGetDataResponseData = mResponseData as SwVoidGetDataResponseData;
		if (!NeedVerify(swVoidGetDataResponseData.result))
		{
			SwVoidOnlineProcess.SwVoidGetDataResponse(swVoidGetDataResponseData);
			SwVoidMenuScript.mInstance.GetLastCenterIcon().Refresh();
		}
	}
}
