public class SwFirstIapSuccessRequest : SwLoginRequest
{
	protected override string GetUrl()
	{
		string uUID = GameApp.GetInstance().UUID;
		mSign = Hash(uUID);
		return "http://" + base.LOGIN_SERVER_ADDRESS + "/Sw2/SwFirstIapSuccess?udid=" + ToBase64String(uUID) + "&ver=" + ToBase64String("124") + "&platform=" + ToBase64String(((int)base.Platform).ToString()) + "&test=" + mSign;
	}

	protected override void ReadData(BytesBuffer bb)
	{
		mReceiveErrorCode = bb.ReadInt();
	}

	public override void ProcessLogic()
	{
		if (mReceiveErrorCode == SwErrorCode.SUCCESS)
		{
			GameApp.GetInstance().GetUserState().SetFirstIapSuccess(true);
		}
	}

	public override void OnTimeOut()
	{
	}

	public override void OnResponseError()
	{
	}
}
