public class SwVoidCheckServerWWWRequest : SwVoidWWWRequest
{
	public SwVoidCheckServerWWWRequest()
	{
		mResponseData = new SwVoidCheckServerResponseData();
		mSign = Hash(mSendUserId.ToString());
	}

	protected override string GetUrl()
	{
		return "http://" + mVoidServerAddress + "/Sw2/SwVoidCheckServer?userId=" + mSendUserId + "&test=" + mSign;
	}

	public override void ProcessLogic()
	{
		SwVoidCheckServerResponseData swVoidCheckServerResponseData = mResponseData as SwVoidCheckServerResponseData;
		if (!NeedVerify(swVoidCheckServerResponseData.result))
		{
			SwVoidOnlineProcess.SwVoidCheckServerResponse(swVoidCheckServerResponseData);
		}
	}

	public override void OnTimeOut()
	{
		if (IapScript.mInstance != null)
		{
			IapScript.mInstance.WaitIAPResultEnd();
			UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"));
		}
	}

	public override void OnResponseError()
	{
		if (mHasVerified)
		{
			if (IapScript.mInstance != null)
			{
				IapScript.mInstance.WaitIAPResultEnd();
				UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"));
			}
		}
		else
		{
			Verify();
		}
	}

	public override void OnVerifyResponseError()
	{
		if (IapScript.mInstance != null)
		{
			IapScript.mInstance.WaitIAPResultEnd();
			UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"));
		}
	}

	public override void OnVerifyFail(int errorCode)
	{
		if (IapScript.mInstance != null)
		{
			IapScript.mInstance.WaitIAPResultEnd();
			UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"));
		}
	}
}
