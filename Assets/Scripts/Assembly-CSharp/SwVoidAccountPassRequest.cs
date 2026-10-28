public class SwVoidAccountPassRequest : WWWRequest
{
	private WWWRequest mRequestToSend;

	private int mSendUserId;

	private string mVoidServerAddress;

	public SwVoidAccountPassRequest(WWWRequest request)
	{
		mRequestToSend = request;
		mVoidServerAddress = GameApp.GetInstance().GetUserState().GetServerAddress() + ":" + GameApp.GetInstance().GetUserState().GetVoidServerPort();
		mSendUserId = GameApp.GetInstance().GetUserState().GetOnlineUserId();
		mSign = Hash(mSendUserId.ToString());
	}

	protected override string GetUrl()
	{
		return "http://" + mVoidServerAddress + "/Sw2/SwVoidAccountPass?userId=" + mSendUserId + "&test=" + mSign;
	}

	protected override void ReadData(BytesBuffer bb)
	{
		mReceiveErrorCode = bb.ReadInt();
	}

	public override void ProcessLogic()
	{
		if (mReceiveErrorCode == SwErrorCode.SUCCESS)
		{
			GameApp.GetInstance().GetWWWManager().SendRequest(mRequestToSend);
		}
		else if (SwVoidMenuScript.mInstance != null)
		{
			SwVoidMenuScript.mInstance.ReceiveOnlineInfo();
			UIMsgBox.instance.ShowErrorCodeMessage(mReceiveErrorCode);
		}
	}

	public override void OnTimeOut()
	{
		if (mRequestToSend != null)
		{
			mRequestToSend.OnTimeOut();
		}
	}

	public override void OnResponseError()
	{
		if (mRequestToSend != null)
		{
			mRequestToSend.OnVerifyResponseError();
		}
	}
}
