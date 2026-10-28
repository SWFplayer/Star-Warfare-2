public class SwFriendAcceptRequest : SwFriendRequest
{
	private int mSendFriendId;

	private OnlineFriendResponseData mReceiveFriendData;

	public SwFriendAcceptRequest(int friendId)
	{
		mSendFriendId = friendId;
		mSign = Hash(mSendUserId.ToString() + mSendFriendId);
	}

	protected override string GetUrl()
	{
		return "http://" + mFriendServerAddress + "/Sw2/SwFriendAccept?userId=" + mSendUserId + "&friendId=" + mSendFriendId + "&test=" + mSign;
	}

	protected override void ReadData(BytesBuffer bb)
	{
		mReceiveErrorCode = bb.ReadInt();
		if (mReceiveErrorCode == SwErrorCode.SUCCESS)
		{
			mReceiveFriendData = new OnlineFriendResponseData();
			mReceiveFriendData.ReadData(bb);
		}
	}

	public override void ProcessLogic()
	{
		if (mReceiveErrorCode == SwErrorCode.SUCCESS)
		{
			OnlineFriend onlineFriend = new OnlineFriend(mReceiveFriendData);
			GameApp.GetInstance().GetUserState().AddFriend(onlineFriend);
			GameApp.GetInstance().GetUserState().RemoveRequestByUserId(onlineFriend.GetUserId());
			if (FriendUI.mInstance != null)
			{
				UIMsgBox.instance.CloseMessage();
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
