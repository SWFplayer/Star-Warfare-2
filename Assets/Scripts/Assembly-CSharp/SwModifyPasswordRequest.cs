public class SwModifyPasswordRequest : SwLoginRequest
{
	protected override string GetUrl()
	{
		mSign = Hash(GameApp.GetInstance().GetUserState().GetOnlineUserId()
			.ToString());
		int num = 0;
		if (LocalizationManager.GetInstance().currentLanguage == EUserLanguage.CHINESE_SIMPLIFIED || LocalizationManager.GetInstance().currentLanguage == EUserLanguage.CHINESE_TRADITIONAL)
		{
			num = 1;
		}
		return "http://" + base.LOGIN_SERVER_ADDRESS + "/Sw2/SwModifyPasswordRequest?userId=" + GameApp.GetInstance().GetUserState().GetOnlineUserId() + "&ver=" + ToBase64String("124") + "&platform=" + ToBase64String(((int)base.Platform).ToString()) + "&language=" + num + "&test=" + mSign;
	}

	protected override void ReadData(BytesBuffer bb)
	{
		mReceiveErrorCode = bb.ReadInt();
	}

	public override void ProcessLogic()
	{
		if (mReceiveErrorCode == SwErrorCode.SUCCESS)
		{
			UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_CHANGE_PW_08", GameApp.GetInstance().GetUserState().GetOnlineEmail()));
		}
		else if (mReceiveErrorCode == SwErrorCode.LOGIN_VERSION_MISMATCH)
		{
			UIMsgBox.instance.ShowErrorCodeMessage(LoginPageScript.mInstance, mReceiveErrorCode, 16);
		}
		else
		{
			UIMsgBox.instance.ShowErrorCodeMessage(mReceiveErrorCode);
		}
	}
}
