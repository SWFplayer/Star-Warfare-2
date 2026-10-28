public class SwVoidGetRewardWWWRequest : SwVoidWWWRequest
{
	private int mSendVoidId;

	private int mSendTotalKill;

	private byte mSendMode;

	private byte mSendStageId;

	private byte mSendSubStageId;

	private byte mSendIsWin;

	public SwVoidGetRewardWWWRequest(int swVoidId, int totalKill, byte difficultMode, byte stageId, byte subStageId, bool isWin)
	{
		mSendVoidId = swVoidId;
		mSendTotalKill = totalKill;
		mSendMode = difficultMode;
		mSendStageId = stageId;
		mSendSubStageId = subStageId;
		mSendIsWin = (byte)(isWin ? 1 : 0);
		mResponseData = new SwVoidGetRewardResponseData();
		mSign = Hash(mSendUserId.ToString() + mSendVoidId + mSendTotalKill + mSendMode + mSendStageId + mSendSubStageId + mSendIsWin);
	}

	protected override string GetUrl()
	{
		return "http://" + mVoidServerAddress + "/Sw2/SwVoidGetReward?userId=" + mSendUserId + "&voidId=" + mSendVoidId + "&totalKill=" + mSendTotalKill + "&difficultMode=" + mSendMode + "&stageId=" + mSendStageId + "&subStageId=" + mSendSubStageId + "&isWin=" + mSendIsWin + "&test=" + mSign;
	}

	public override void ProcessLogic()
	{
		SwVoidGetRewardResponseData swVoidGetRewardResponseData = mResponseData as SwVoidGetRewardResponseData;
		if (!NeedVerify(swVoidGetRewardResponseData.result))
		{
			SwVoidOnlineProcess.SwVoidGetRewardResponse(swVoidGetRewardResponseData);
		}
	}

	public override void OnTimeOut()
	{
		if (ResultPageUI.mInstance != null)
		{
			ResultPageUI.mInstance.ReceiveSwVoidDropMessage();
		}
		if (GameApp.GetInstance().GetGameWorld() != null)
		{
			SwVoidGetRewardResponseData swVoidGetRewardResponseData = mResponseData as SwVoidGetRewardResponseData;
			swVoidGetRewardResponseData.result = SwErrorCode.VOID_UNKNOWN_ERROR;
			GameApp.GetInstance().GetGameWorld().VoidRewardData = swVoidGetRewardResponseData;
		}
	}

	public override void OnResponseError()
	{
		if (ResultPageUI.mInstance != null)
		{
			ResultPageUI.mInstance.ReceiveSwVoidDropMessage();
		}
		if (GameApp.GetInstance().GetGameWorld() != null)
		{
			SwVoidGetRewardResponseData swVoidGetRewardResponseData = mResponseData as SwVoidGetRewardResponseData;
			swVoidGetRewardResponseData.result = SwErrorCode.VOID_UNKNOWN_ERROR;
			GameApp.GetInstance().GetGameWorld().VoidRewardData = swVoidGetRewardResponseData;
		}
	}

	public override void OnVerifyResponseError()
	{
		if (ResultPageUI.mInstance != null)
		{
			ResultPageUI.mInstance.ReceiveSwVoidDropMessage();
		}
		if (GameApp.GetInstance().GetGameWorld() != null)
		{
			SwVoidGetRewardResponseData swVoidGetRewardResponseData = mResponseData as SwVoidGetRewardResponseData;
			swVoidGetRewardResponseData.result = SwErrorCode.VOID_UNKNOWN_ERROR;
			GameApp.GetInstance().GetGameWorld().VoidRewardData = swVoidGetRewardResponseData;
		}
	}

	public override void OnVerifyFail(int errorCode)
	{
		if (ResultPageUI.mInstance != null)
		{
			ResultPageUI.mInstance.ReceiveSwVoidDropMessage();
		}
		if (GameApp.GetInstance().GetGameWorld() != null)
		{
			SwVoidGetRewardResponseData swVoidGetRewardResponseData = mResponseData as SwVoidGetRewardResponseData;
			swVoidGetRewardResponseData.result = SwErrorCode.VOID_UNKNOWN_ERROR;
			GameApp.GetInstance().GetGameWorld().VoidRewardData = swVoidGetRewardResponseData;
		}
	}
}
