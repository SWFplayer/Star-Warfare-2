public class SwFriendRenameRequest : SwFriendRequest
{
	public SwFriendRenameRequest()
	{
		mSign = Hash(mSendUserId.ToString());
	}

	protected override string GetUrl()
	{
		return "http://" + mFriendServerAddress + "/Sw2/SwFriendRename?userId=" + mSendUserId + "&userName=" + ToBase64String(GameApp.GetInstance().GetUserState().GetOnlineNickname()) + "&test=" + mSign;
	}

	protected override void ReadData(BytesBuffer bb)
	{
		mReceiveErrorCode = bb.ReadInt();
	}

	public override void ProcessLogic()
	{
	}
}
