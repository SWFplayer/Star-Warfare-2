public class SwFriendAddRequest : SwFriendRequest
{
	private int mSendFriendId;

	public SwFriendAddRequest(int friendId)
	{
		mSendFriendId = friendId;
		mSign = Hash(mSendUserId.ToString() + mSendFriendId);
	}

	protected override string GetUrl()
	{
		return "http://" + mFriendServerAddress + "/Sw2/SwFriendAdd?userId=" + mSendUserId + "&friendId=" + mSendFriendId + "&test=" + mSign;
	}

	protected override void ReadData(BytesBuffer bb)
	{
		mReceiveErrorCode = bb.ReadInt();
	}

	public override void ProcessLogic()
	{
		if (mReceiveErrorCode != SwErrorCode.SUCCESS)
		{
			UIMsgBox.instance.ShowErrorCodeMessage(mReceiveErrorCode);
		}
	}
}
