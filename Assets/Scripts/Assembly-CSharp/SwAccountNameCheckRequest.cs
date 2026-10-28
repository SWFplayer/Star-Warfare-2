public class SwAccountNameCheckRequest : SwLoginRequest
{
	protected string mSendName;

	public SwAccountNameCheckRequest(string name)
	{
		mSendName = name;
	}

	protected override string GetUrl()
	{
		return "http://" + base.LOGIN_SERVER_ADDRESS + "/Sw2/SwAccountNameCheck?nickname=" + ToBase64String(mSendName) + "&ver=" + ToBase64String("124") + "&platform=" + ToBase64String(((int)base.Platform).ToString());
	}

	protected override void ReadData(BytesBuffer bb)
	{
		mReceiveErrorCode = bb.ReadInt();
	}

	public override void ProcessLogic()
	{
		if (LoginPageScript.mInstance != null)
		{
			LoginPageScript.mInstance.Register_CheckNickNameResult(mReceiveErrorCode);
		}
		if (EditNamePageScript.mInstance != null)
		{
			EditNamePageScript.mInstance.Register_CheckNickNameResult(mReceiveErrorCode);
		}
	}
}
