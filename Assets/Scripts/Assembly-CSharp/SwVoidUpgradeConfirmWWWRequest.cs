public class SwVoidUpgradeConfirmWWWRequest : SwVoidWWWRequest
{
	private int mSendVoidId;

	public SwVoidUpgradeConfirmWWWRequest(int voidId)
	{
		mSendVoidId = voidId;
		mResponseData = new SwVoidUpgradeConfirmResponseData();
		mSign = Hash(mSendUserId.ToString() + mSendVoidId);
	}

	protected override string GetUrl()
	{
		return "http://" + mVoidServerAddress + "/Sw2/SwVoidUpgradeConfirm?userId=" + mSendUserId + "&voidId=" + mSendVoidId + "&test=" + mSign;
	}

	public override void ProcessLogic()
	{
		SwVoidUpgradeConfirmResponseData swVoidUpgradeConfirmResponseData = mResponseData as SwVoidUpgradeConfirmResponseData;
		if (!NeedVerify(swVoidUpgradeConfirmResponseData.result))
		{
			SwVoidOnlineProcess.SwVoidUpgradeConfirmResopnse(swVoidUpgradeConfirmResponseData);
		}
	}
}
