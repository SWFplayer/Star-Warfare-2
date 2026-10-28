public class SwVoidLoginRequest : SwLoginRequest
{
	protected ELoginType mReceiveLoginType;

	protected string mReceiveEmail;

	protected string mReceiveName;

	protected string mReceivePassword;

	protected int mReceiveUserId;

	protected string mReceiveIp;

	protected int mReceiveLogicServerPort;

	protected int mReceiveVoidServerPort;

	protected int mReceiveFriendServerPort;

	protected string mReceiveDate;

	protected int mReceiveBonusMithril;

	protected int mReceiveCompensateMithril;

	protected int mReceiveBonusGold;

	protected int mReceiveCompensateGold;

	protected int mReceiveCompensateExp;

	protected ServerType mReceiveServerType;

	protected bool mIsRegist;

	private WWWRequest mRequestToSend;

	public SwVoidLoginRequest(WWWRequest request)
	{
		mRequestToSend = request;
	}

	protected override string GetUrl()
	{
		string uUID = GameApp.GetInstance().UUID;
		mSign = Hash(uUID);
		UserState userState = GameApp.GetInstance().GetUserState();
		return "http://" + base.LOGIN_SERVER_ADDRESS + "/Sw2/SwAccountLogin?email=" + ToBase64String(userState.GetOnlineEmail()) + "&pwd=" + ToBase64String(userState.GetOnlinePassword()) + "&udid=" + ToBase64String(uUID) + "&ver=" + ToBase64String("124") + "&device=" + ToBase64String(base.Device) + "&os=" + ToBase64String(base.OS) + "&platform=" + ToBase64String(((int)base.Platform).ToString()) + "&test=" + mSign;
	}

	protected override void ReadData(BytesBuffer bb)
	{
		mReceiveErrorCode = bb.ReadInt();
		if (mReceiveErrorCode == SwErrorCode.SUCCESS)
		{
			mReceiveLoginType = (ELoginType)bb.ReadByte();
			mReceiveUserId = bb.ReadInt();
			mReceiveIp = bb.ReadStringShortLength();
			mReceiveLogicServerPort = bb.ReadShort();
			mReceiveVoidServerPort = bb.ReadShort();
			mReceiveFriendServerPort = bb.ReadShort();
			mReceiveDate = bb.ReadStringShortLength();
			mReceiveBonusMithril = bb.ReadInt();
			mReceiveCompensateMithril = bb.ReadInt();
			mReceiveBonusGold = bb.ReadInt();
			mReceiveCompensateGold = bb.ReadInt();
			if (mReceiveLoginType == ELoginType.SUCCESS_ACCOUNT)
			{
				mReceiveName = bb.ReadStringShortLength();
			}
			if (mReceiveLoginType == ELoginType.SUCCESS_BIND)
			{
				mReceiveEmail = bb.ReadStringShortLength();
				mReceiveName = bb.ReadStringShortLength();
				mReceivePassword = bb.ReadStringShortLength();
			}
			mReceiveCompensateExp = bb.ReadInt();
			mReceiveServerType = (ServerType)bb.ReadByte();
		}
	}

	public override void ProcessLogic()
	{
		if (mReceiveErrorCode == SwErrorCode.SUCCESS)
		{
			UserState userState = GameApp.GetInstance().GetUserState();
			userState.SetOnlineUserId(mReceiveUserId);
			userState.SetServerAddress(mReceiveIp);
			userState.SetVoidServerPort(mReceiveVoidServerPort);
			userState.SetFriendServerPort(mReceiveFriendServerPort);
			userState.SetBonusMithril(mReceiveBonusMithril);
			userState.SetCompensateMithril(mReceiveCompensateMithril);
			userState.SetBonusGold(mReceiveBonusGold);
			userState.SetCompensateGold(mReceiveCompensateGold);
			userState.SetCompensateExp(mReceiveCompensateExp);
			if (mReceiveLoginType == ELoginType.SUCCESS_ACCOUNT)
			{
				userState.SetOnlineNickname(mReceiveName);
			}
			if (mReceiveLoginType == ELoginType.SUCCESS_BIND)
			{
				userState.SetOnlineRegisted(true);
				userState.SetOnlineEmail(mReceiveEmail);
				userState.SetOnlineNickname(mReceiveName);
				userState.SetOnlinePassword(mReceivePassword);
			}
			if (mReceiveLoginType == ELoginType.SUCCESS_ANONYMOUS)
			{
				userState.SetGuestRegisted(true);
				userState.SetOnlineNickname("Guest" + mReceiveUserId);
			}
			GameApp.GetInstance().Save();
			SwVoidAccountPassRequest request = new SwVoidAccountPassRequest(mRequestToSend);
			GameApp.GetInstance().GetWWWManager().SendRequest(request);
		}
		else
		{
			mRequestToSend.OnVerifyFail(mReceiveErrorCode);
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
