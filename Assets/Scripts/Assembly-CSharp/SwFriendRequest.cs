public class SwFriendRequest : WWWRequest
{
	protected string mFriendServerAddress;

	protected int mSendUserId;

	public SwFriendRequest()
	{
		mFriendServerAddress = GameApp.GetInstance().GetUserState().GetServerAddress() + ":" + GameApp.GetInstance().GetUserState().GetFriendServerPort();
		mSendUserId = GameApp.GetInstance().GetUserState().GetOnlineUserId();
	}

	public override void OnTimeOut()
	{
		base.OnTimeOut();
	}

	public override void OnResponseError()
	{
		base.OnResponseError();
	}
}
