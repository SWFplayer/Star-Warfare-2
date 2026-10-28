public class SwAccountRegistRequest : SwLoginRequest
{
	protected string mSendEmail;

	protected string mSendName;

	protected string mSendPassword;

	protected string mReceiveEmail;

	protected string mReceiveName;

	protected string mReceivePassword;

	protected int mReceiveUserId;

	public SwAccountRegistRequest(string email, string name, string password)
	{
		mSendEmail = email;
		mSendName = name;
		mSendPassword = password;
	}

	protected override string GetUrl()
	{
		string uUID = GameApp.GetInstance().UUID;
		mSign = Hash(uUID);
		return "http://" + base.LOGIN_SERVER_ADDRESS + "/Sw2/SwAccountRegister?email=" + ToBase64String(mSendEmail) + "&nickname=" + ToBase64String(mSendName) + "&pwd=" + ToBase64String(mSendPassword) + "&udid=" + ToBase64String(uUID) + "&mac=" + ToBase64String(GameApp.GetInstance().MacAddress) + "&ver=" + ToBase64String("124") + "&platform=" + ToBase64String(((int)base.Platform).ToString()) + "&device=" + ToBase64String(base.Device) + "&os=" + ToBase64String(base.OS) + "&test=" + mSign;
	}

	protected override void ReadData(BytesBuffer bb)
	{
		mReceiveErrorCode = bb.ReadInt();
		if (mReceiveErrorCode == SwErrorCode.SUCCESS)
		{
			mReceiveEmail = bb.ReadStringShortLength();
			mReceiveName = bb.ReadStringShortLength();
			mReceivePassword = bb.ReadStringShortLength();
			mReceiveUserId = bb.ReadInt();
		}
	}

	public override void ProcessLogic()
	{
		if (mReceiveErrorCode == SwErrorCode.SUCCESS)
		{
			UserState userState = GameApp.GetInstance().GetUserState();
			userState.SetOnlineRegisted(true);
			userState.SetOnlineEmail(mReceiveEmail);
			userState.SetOnlineNickname(mReceiveName);
			userState.SetOnlinePassword(mReceivePassword);
			userState.SetOnlineUserId(mReceiveUserId);
			if (MainMenuScript.mInstance != null)
			{
				MainMenuScript.mInstance.bIsRegist = true;
			}
			else if (MainMenu2DScript.mInstance != null)
			{
				MainMenu2DScript.mInstance.bIsRegist = true;
			}
			GameApp.GetInstance().Save();
			AndroidPluginScript.appsflyertackRegistration();
			SwAccountLoginRequest request = new SwAccountLoginRequest(true);
			GameApp.GetInstance().GetWWWManager().SendRequest(request);
			if (LoginPageScript.mInstance != null)
			{
				UIMsgBox.instance.ShowMessage(LoginPageScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_11"), 7);
				LoginPageScript.mInstance.Register_CheckPasswordResult(mReceiveErrorCode);
				LoginPageScript.mInstance.Register_CheckNickNameResult(mReceiveErrorCode);
				LoginPageScript.mInstance.Register_CheckEmailResult(mReceiveErrorCode);
			}
		}
		else if (mReceiveErrorCode == SwErrorCode.LOGIN_VERSION_MISMATCH)
		{
			UIMsgBox.instance.ShowErrorCodeMessage(LoginPageScript.mInstance, mReceiveErrorCode, 16);
		}
		else if (mReceiveErrorCode == SwErrorCode.ACCOUNT_EMAIL_EXIST || mReceiveErrorCode == SwErrorCode.ACCOUNT_EMAIL_INVALID || mReceiveErrorCode == SwErrorCode.ACCOUNT_NAME_EXIST || mReceiveErrorCode == SwErrorCode.ACCOUNT_NAME_INVALID || mReceiveErrorCode == SwErrorCode.ACCOUNT_PASSWORD_INVALID)
		{
			if (LoginPageScript.mInstance != null)
			{
				UIMsgBox.instance.ShowMessage(LoginPageScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_13"), 1);
				LoginPageScript.mInstance.Register_CheckPasswordResult(mReceiveErrorCode);
				LoginPageScript.mInstance.Register_CheckNickNameResult(mReceiveErrorCode);
				LoginPageScript.mInstance.Register_CheckEmailResult(mReceiveErrorCode);
			}
		}
		else if (LoginPageScript.mInstance != null)
		{
			UIMsgBox.instance.ShowErrorCodeMessage(mReceiveErrorCode);
		}
	}
}
