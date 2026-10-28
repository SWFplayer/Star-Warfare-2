public class SwVoidWWWRequest : WWWRequest
{
	protected string mVoidServerAddress;

	protected int mSendUserId;

	protected SwVoidBaseResponseData mResponseData;

	public SwVoidWWWRequest()
	{
		mVoidServerAddress = GameApp.GetInstance().GetUserState().GetServerAddress() + ":" + GameApp.GetInstance().GetUserState().GetVoidServerPort();
		mSendUserId = GameApp.GetInstance().GetUserState().GetOnlineUserId();
	}

	protected override void ReadData(BytesBuffer bb)
	{
		mResponseData.ReadData(bb);
	}

	public override void OnTimeOut()
	{
		base.OnTimeOut();
		if (SwVoidMenuScript.mInstance != null && SwVoidMenuScript.mInstance.OnlineWait != null && SwVoidMenuScript.mInstance.OnlineWait.activeSelf)
		{
			SwVoidMenuScript.mInstance.ReceiveOnlineInfo();
		}
		UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"));
	}

	public override void OnResponseError()
	{
		base.OnResponseError();
		if (mHasVerified)
		{
			if (SwVoidMenuScript.mInstance != null && SwVoidMenuScript.mInstance.OnlineWait != null && SwVoidMenuScript.mInstance.OnlineWait.activeSelf)
			{
				SwVoidMenuScript.mInstance.ReceiveOnlineInfo();
			}
			UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"));
		}
		else
		{
			Verify();
		}
	}

	public override void OnVerifyResponseError()
	{
		base.OnVerifyResponseError();
		if (SwVoidMenuScript.mInstance != null && SwVoidMenuScript.mInstance.OnlineWait != null && SwVoidMenuScript.mInstance.OnlineWait.activeSelf)
		{
			SwVoidMenuScript.mInstance.ReceiveOnlineInfo();
		}
		UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"));
	}

	public override void OnVerifyFail(int errorCode)
	{
		base.OnVerifyFail(errorCode);
		if (SwVoidMenuScript.mInstance != null && SwVoidMenuScript.mInstance.OnlineWait != null && SwVoidMenuScript.mInstance.OnlineWait.activeSelf)
		{
			SwVoidMenuScript.mInstance.ReceiveOnlineInfo();
		}
		UIMsgBox.instance.ShowErrorCodeMessage(errorCode);
	}

	protected bool NeedVerify(int result)
	{
		if (result == SwErrorCode.GLOBAL_ACCOUNT_NOT_VERIFIED && !mHasVerified)
		{
			Verify();
			return true;
		}
		return false;
	}

	protected void Verify()
	{
		mHasVerified = true;
		SwVoidLoginRequest request = new SwVoidLoginRequest(this);
		GameApp.GetInstance().GetWWWManager().SendRequest(request);
	}
}
