public class SwVoidUpgradeWWWRequest : SwVoidWWWRequest
{
	private int mSendVoidId;

	public SwVoidUpgradeWWWRequest(int voidId)
	{
		mSendVoidId = voidId;
		mResponseData = new SwVoidUpgradeResponseData();
		mSign = Hash(mSendUserId.ToString() + mSendVoidId);
	}

	protected override string GetUrl()
	{
		return "http://" + mVoidServerAddress + "/Sw2/SwVoidUpgrade?userId=" + mSendUserId + "&voidId=" + mSendVoidId + "&test=" + mSign;
	}

	public override void ProcessLogic()
	{
		SwVoidUpgradeResponseData swVoidUpgradeResponseData = mResponseData as SwVoidUpgradeResponseData;
		if (!NeedVerify(swVoidUpgradeResponseData.result))
		{
			SwVoidOnlineProcess.SwVoidUpgradeResponse(swVoidUpgradeResponseData);
		}
	}
}
