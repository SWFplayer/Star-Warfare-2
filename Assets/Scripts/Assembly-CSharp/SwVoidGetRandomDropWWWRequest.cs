public class SwVoidGetRandomDropWWWRequest : SwVoidWWWRequest
{
	private int mSendVoidId;

	private byte mSendMode;

	private byte mSendStageId;

	private byte mSendSubStageId;

	public SwVoidGetRandomDropWWWRequest(int swVoidId, byte mMode, byte stageId, byte subStageId)
	{
		mSendVoidId = swVoidId;
		mSendMode = mMode;
		mSendStageId = stageId;
		mSendSubStageId = subStageId;
		mResponseData = new SwVoidGetRandomDropResponseData();
		mSign = Hash(mSendUserId.ToString() + mSendVoidId + mSendMode + mSendStageId + mSendSubStageId);
	}

	protected override string GetUrl()
	{
		return "http://" + mVoidServerAddress + "/Sw2/SwVoidGetRandomDrop?userId=" + mSendUserId + "&voidId=" + mSendVoidId + "&difficultMode=" + mSendMode + "&stageId=" + mSendStageId + "&subStageId=" + mSendSubStageId + "&test=" + mSign;
	}

	public override void ProcessLogic()
	{
		SwVoidGetRandomDropResponseData swVoidGetRandomDropResponseData = mResponseData as SwVoidGetRandomDropResponseData;
		if (!NeedVerify(swVoidGetRandomDropResponseData.result))
		{
			SwVoidOnlineProcess.SwVoidGetRandomDropResponse(swVoidGetRandomDropResponseData);
		}
	}

	public override void OnTimeOut()
	{
	}

	public override void OnResponseError()
	{
	}

	public override void OnVerifyResponseError()
	{
	}

	public override void OnVerifyFail(int errorCode)
	{
	}
}
