public class SwPvpExpIapWWWRequest : WWWRequest
{
	protected string mVoidServerAddress;

	protected int mSendUserId;

	protected byte mReceivePrestige;

	protected int mReceiveExp;

	public SwPvpExpIapWWWRequest()
	{
		mVoidServerAddress = GameApp.GetInstance().GetUserState().GetServerAddress() + ":" + GameApp.GetInstance().GetUserState().GetVoidServerPort();
		mSendUserId = GameApp.GetInstance().GetUserState().GetOnlineUserId();
		mSign = Hash(mSendUserId.ToString());
	}

	protected override string GetUrl()
	{
		return "http://" + mVoidServerAddress + "/Sw2/SwPvpExpIap?userId=" + mSendUserId + "&test=" + mSign;
	}

	protected override void ReadData(BytesBuffer bb)
	{
		mReceiveErrorCode = bb.ReadInt();
		if (mReceiveErrorCode == SwErrorCode.SUCCESS)
		{
			mReceivePrestige = bb.ReadByte();
			mReceiveExp = bb.ReadInt();
		}
	}

	public override void ProcessLogic()
	{
		if (!NeedVerify(mReceiveErrorCode))
		{
			UIMsgBox.instance.CloseMessage();
			UIMsgBox.instance.ShowMessage(UIMsgBox.instance, LocalizationManager.GetInstance().GetString("LOC_PVP_IAP_HINT2"), 1, 7);
			GameApp.GetInstance().GetUserState().GetOnlinePlayer()
				.SetPrestige(mReceivePrestige);
			GameApp.GetInstance().GetUserState().GetOnlinePlayer()
				.SetExp(mReceiveExp);
			if (PlayerInfoUI.mLocalInstance != null && PlayerInfoUI.mLocalInstance.gameObject.activeSelf)
			{
				PlayerInfoUI.mLocalInstance.PrestigeComplete(mReceivePrestige, mReceiveExp);
			}
			else if (CustomizeUIScript.mInstance != null && CustomizeUIScript.mInstance.gameObject.activeSelf)
			{
				CustomizeUIScript.mInstance.Refresh();
			}
			else if (ShopUIScript.GetInstance() != null)
			{
				ShopUIScript.GetInstance().RefreshLockIcon();
			}
		}
	}

	public override void OnTimeOut()
	{
		base.OnTimeOut();
		UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"));
	}

	public override void OnResponseError()
	{
		base.OnResponseError();
		if (mHasVerified)
		{
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
		UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"));
	}

	public override void OnVerifyFail(int errorCode)
	{
		base.OnVerifyFail(errorCode);
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
