public class SwAccountRenameRequest : SwLoginRequest
{
	protected string mSendName;

	protected string mReceiveName;

	public SwAccountRenameRequest(string name)
	{
		mSendName = name;
	}

	protected override string GetUrl()
	{
		mSign = Hash(GameApp.GetInstance().GetUserState().GetOnlineUserId()
			.ToString());
		return "http://" + base.LOGIN_SERVER_ADDRESS + "/Sw2/SwAccountRename?userId=" + GameApp.GetInstance().GetUserState().GetOnlineUserId() + "&nickname=" + ToBase64String(mSendName) + "&test=" + mSign;
	}

	protected override void ReadData(BytesBuffer bb)
	{
		mReceiveErrorCode = bb.ReadInt();
		if (mReceiveErrorCode == SwErrorCode.SUCCESS)
		{
			mReceiveName = bb.ReadStringShortLength();
		}
	}

	public override void ProcessLogic()
	{
		if (mReceiveErrorCode == SwErrorCode.SUCCESS)
		{
			UserState userState = GameApp.GetInstance().GetUserState();
			userState.SetOnlineNickname(mReceiveName);
			userState.BuyWithMithril(100);
			userState.AnalysisManager.mithrilInfo.RenameCost += 100;
			GameApp.GetInstance().Save();
			Lobby.GetInstance().SetUserName(mReceiveName);
			if (GameApp.GetInstance().IsConnectedToInternet())
			{
				SwFriendRenameRequest request = new SwFriendRenameRequest();
				GameApp.GetInstance().GetWWWManager().SendRequest(request);
				PlayerRenameRequest request2 = new PlayerRenameRequest(mReceiveName);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
			}
			if (EditNamePageScript.mInstance != null)
			{
				UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_SPHINT12"));
				EditNamePageScript.mInstance.Register_CheckNickNameResult(mReceiveErrorCode);
				EditNamePageScript.mInstance.OnButtonClick(EditNamePageScript.mInstance.CancelBtn);
			}
			if (PlayerInfoUI.mLocalInstance != null)
			{
				PlayerInfoUI.mLocalInstance.PlayerName.text = Lobby.GetInstance().GetUserName();
			}
		}
		else
		{
			UIMsgBox.instance.ShowErrorCodeMessage(mReceiveErrorCode);
			if (EditNamePageScript.mInstance != null)
			{
				EditNamePageScript.mInstance.Register_CheckNickNameResult(mReceiveErrorCode);
			}
		}
	}
}
