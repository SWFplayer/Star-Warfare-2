public class SwAccountBindRequest : SwLoginRequest
{
	protected string mSendEmail;

	protected string mSendPassword;

	protected string mReceiveEmail;

	protected string mReceiveName;

	protected string mReceivePassword;

	protected int mReceiveUserId;

	public SwAccountBindRequest(string email, string password)
	{
		mSendEmail = email;
		mSendPassword = password;
		mSign = Hash(GameApp.GetInstance().UUID);
	}

	protected override string GetUrl()
	{
		return "http://" + base.LOGIN_SERVER_ADDRESS + "/Sw2/SwAccountBind?email=" + ToBase64String(mSendEmail) + "&pwd=" + ToBase64String(mSendPassword) + "&udid=" + ToBase64String(GameApp.GetInstance().UUID) + "&mac=" + ToBase64String(GameApp.GetInstance().MacAddress) + "&ver=" + ToBase64String("124") + "&platform=" + ToBase64String(((int)base.Platform).ToString()) + "&test=" + mSign;
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
			GameApp.GetInstance().Save();
			SwAccountLoginRequest request = new SwAccountLoginRequest(true);
			GameApp.GetInstance().GetWWWManager().SendRequest(request);
			if (LoginPageScript.mInstance != null)
			{
				UIMsgBox.instance.ShowMessage(LoginPageScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_11"), 7);
			}
		}
		else if (LoginPageScript.mInstance != null)
		{
			if (mReceiveErrorCode != SwErrorCode.LOGIN_VERSION_MISMATCH)
			{
				UIMsgBox.instance.ShowErrorCodeMessage(mReceiveErrorCode);
			}
			else
			{
				UIMsgBox.instance.ShowErrorCodeMessage(LoginPageScript.mInstance, mReceiveErrorCode, 16);
			}
		}
	}
}
