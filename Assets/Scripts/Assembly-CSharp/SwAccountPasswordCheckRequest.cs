public class SwAccountPasswordCheckRequest : SwLoginRequest
{
	protected string mSendPassword;

	public SwAccountPasswordCheckRequest(string password)
	{
		mSendPassword = password;
	}

	protected override string GetUrl()
	{
		return "http://" + base.LOGIN_SERVER_ADDRESS + "/Sw2/SwAccountPasswordCheck?pwd=" + ToBase64String(mSendPassword) + "&ver=" + ToBase64String("124") + "&platform=" + ToBase64String(((int)base.Platform).ToString());
	}

	protected override void ReadData(BytesBuffer bb)
	{
		mReceiveErrorCode = bb.ReadInt();
	}

	public override void ProcessLogic()
	{
		if (LoginPageScript.mInstance != null)
		{
			LoginPageScript.mInstance.Register_CheckPasswordResult(mReceiveErrorCode);
		}
	}
}
