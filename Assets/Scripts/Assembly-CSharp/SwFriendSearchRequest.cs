public class SwFriendSearchRequest : SwFriendRequest
{
	private string mSendFriendName;

	public SwFriendSearchRequest(string friendName)
	{
		mSendFriendName = friendName;
		mSign = Hash(mSendUserId.ToString());
	}

	protected override string GetUrl()
	{
		return "http://" + mFriendServerAddress + "/Sw2/SwFriendSearch?userId=" + mSendUserId + "&friendName=" + ToBase64String(mSendFriendName) + "&test=" + mSign;
	}

	protected override void ReadData(BytesBuffer bb)
	{
		mReceiveErrorCode = bb.ReadInt();
	}

	public override void ProcessLogic()
	{
		if (mReceiveErrorCode == SwErrorCode.SUCCESS)
		{
			if (FriendUI.mInstance != null)
			{
				UIMsgBox.instance.ShowMessage(FriendUI.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_19"), 1);
			}
			else
			{
				UIMsgBox.instance.CloseMessage();
			}
		}
		else
		{
			UIMsgBox.instance.ShowErrorCodeMessage(mReceiveErrorCode);
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
		UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"));
	}
}
