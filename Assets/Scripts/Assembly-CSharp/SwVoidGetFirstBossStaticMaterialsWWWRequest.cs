public class SwVoidGetFirstBossStaticMaterialsWWWRequest : SwVoidWWWRequest
{
	public SwVoidGetFirstBossStaticMaterialsWWWRequest()
	{
		mResponseData = new SwVoidGetFirstBossStaticMaterialsResponseData();
		mSign = Hash(mSendUserId.ToString());
	}

	protected override string GetUrl()
	{
		return "http://" + mVoidServerAddress + "/Sw2/SwVoidGetFirstBossStaticMaterials?userId=" + mSendUserId + "&test=" + mSign;
	}

	public override void ProcessLogic()
	{
		SwVoidGetFirstBossStaticMaterialsResponseData swVoidGetFirstBossStaticMaterialsResponseData = mResponseData as SwVoidGetFirstBossStaticMaterialsResponseData;
		if (!NeedVerify(swVoidGetFirstBossStaticMaterialsResponseData.result))
		{
			SwVoidOnlineProcess.GetFirstBossStaticMaterialsResponse(swVoidGetFirstBossStaticMaterialsResponseData);
		}
	}
}
