public class SwAccountEmailCheckRequest : SwLoginRequest
{
	protected string mSendEmail;

	public SwAccountEmailCheckRequest(string email)
	{
		mSendEmail = email;
	}

	protected override string GetUrl()
	{
		return "http://" + base.LOGIN_SERVER_ADDRESS + "/Sw2/SwAccountEmailCheck?email=" + ToBase64String(mSendEmail) + "&ver=" + ToBase64String("124") + "&platform=" + ToBase64String(((int)base.Platform).ToString());
	}

	protected override void ReadData(BytesBuffer bb)
	{
		mReceiveErrorCode = bb.ReadInt();
	}

	public override void ProcessLogic()
	{
		if (LoginPageScript.mInstance != null)
		{
			LoginPageScript.mInstance.Register_CheckEmailResult(mReceiveErrorCode);
		}
	}
}
