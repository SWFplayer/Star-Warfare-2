public class SwFriendRemoveRequest : SwFriendRequest
{
	private int mSendFriendId;

	public SwFriendRemoveRequest(int friendId)
	{
		mSendFriendId = friendId;
		mSign = Hash(mSendUserId.ToString() + mSendFriendId);
	}

	protected override string GetUrl()
	{
		return "http://" + mFriendServerAddress + "/Sw2/SwFriendRemove?userId=" + mSendUserId + "&friendId=" + mSendFriendId + "&test=" + mSign;
	}

	protected override void ReadData(BytesBuffer bb)
	{
		mReceiveErrorCode = bb.ReadInt();
	}

	public override void ProcessLogic()
	{
		if (mReceiveErrorCode == SwErrorCode.SUCCESS)
		{
			GameApp.GetInstance().GetUserState().RemoveFriendByUserId(mSendFriendId);
			if (FriendUI.mInstance != null)
			{
				FriendUI.mInstance.RefreshPage();
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
