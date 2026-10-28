using System;
using UnityEngine;

public class SwAccountLoginRequest : SwLoginRequest
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

	protected bool mReceiveCompensateMaterial;

	protected ServerType mReceiveServerType;

	protected bool mIsRegist;

	public SwAccountLoginRequest(bool isRegist)
	{
		mIsRegist = isRegist;
	}

	protected override string GetUrl()
	{
		string uUID = GameApp.GetInstance().UUID;
		mSign = Hash(uUID);
		UserState userState = GameApp.GetInstance().GetUserState();
		return "http://" + base.LOGIN_SERVER_ADDRESS + "/Sw2/SwAccountLogin?email=" + ToBase64String(userState.GetOnlineEmail()) + "&pwd=" + ToBase64String(userState.GetOnlinePassword()) + "&udid=" + ToBase64String(uUID) + "&mac=" + ToBase64String(GameApp.GetInstance().MacAddress) + "&ver=" + ToBase64String("124") + "&device=" + ToBase64String(base.Device) + "&os=" + ToBase64String(base.OS) + "&platform=" + ToBase64String(((int)base.Platform).ToString()) + "&test=" + mSign;
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
			mReceiveCompensateMaterial = bb.ReadBool();
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
			userState.SetCompensateMaterial(mReceiveCompensateMaterial);
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
			EUserType eUserType = EUserType.USER;
			if (mReceiveLoginType == ELoginType.SUCCESS_ANONYMOUS)
			{
				eUserType = EUserType.GUEST;
				userState.SetGuestRegisted(true);
				userState.SetOnlineNickname("Guest" + mReceiveUserId);
			}
			GameApp.GetInstance().Save();
			Lobby.GetInstance().SetUserName(userState.GetOnlineNickname());
			Lobby.GetInstance().SetLoginTime(DateTime.Now);
			Lobby.GetInstance().CurrentServerType = mReceiveServerType;
			SwFriendGetRequest request = new SwFriendGetRequest(mReceiveDate);
			GameApp.GetInstance().GetWWWManager().SendRequest(request);
			if (MainMenuScript.mInstance != null)
			{
				MainMenuScript.mInstance.ConnectToGameServer(mReceiveIp, mReceiveLogicServerPort, mReceiveUserId, mReceiveDate, eUserType, mIsRegist);
			}
			else if (MainMenu2DScript.mInstance != null)
			{
				MainMenu2DScript.mInstance.ConnectToGameServer(mReceiveIp, mReceiveLogicServerPort, mReceiveUserId, mReceiveDate, eUserType, mIsRegist);
			}
			else if (LobbyScript.mInstance != null)
			{
				RoleLoginRequest request2 = new RoleLoginRequest(Lobby.GetInstance().GetUserName(), mReceiveUserId, mReceiveDate, (byte)eUserType, mIsRegist);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
				if (MainMenuScript.mInstance != null)
				{
					UIMsgBox.instance.ShowMessage(MainMenuScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_PVP_CONNECT"), 7);
				}
				else if (MainMenu2DScript.mInstance != null)
				{
					UIMsgBox.instance.ShowMessage(MainMenu2DScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_PVP_CONNECT"), 7);
				}
			}
		}
		else if (mReceiveErrorCode == SwErrorCode.LOGIN_VERSION_MISMATCH)
		{
			if (MainMenuScript.mInstance != null)
			{
				UIMsgBox.instance.ShowErrorCodeMessage(MainMenuScript.mInstance, mReceiveErrorCode, 16);
			}
			else if (MainMenu2DScript.mInstance != null)
			{
				UIMsgBox.instance.ShowErrorCodeMessage(MainMenu2DScript.mInstance, mReceiveErrorCode, 16);
			}
			else if (LobbyScript.mInstance != null)
			{
				UIMsgBox.instance.ShowErrorCodeMessage(LobbyScript.mInstance, mReceiveErrorCode, 16);
			}
		}
		else if (mReceiveErrorCode == SwErrorCode.LOGIN_PASSWORD_INCORRECT)
		{
			UIMsgBox.instance.CloseMessage();
			GameObject original = Resources.Load("UI/Menu/OptionUI/Prefab/NewPasswordUI") as GameObject;
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original, new Vector3(0f, 1500f, 0f), Quaternion.identity);
		}
		else if (MainMenuScript.mInstance != null)
		{
			UIMsgBox.instance.ShowErrorCodeMessage(MainMenuScript.mInstance, mReceiveErrorCode, 17);
		}
		else if (MainMenu2DScript.mInstance != null)
		{
			UIMsgBox.instance.ShowErrorCodeMessage(MainMenu2DScript.mInstance, mReceiveErrorCode, 17);
		}
		else if (LobbyScript.mInstance != null)
		{
			UIMsgBox.instance.ShowErrorCodeMessage(LobbyScript.mInstance, mReceiveErrorCode, 17);
		}
	}

	public override void OnTimeOut()
	{
		if (mIsRegist && GameApp.GetInstance().GetUserState().GetOnlineRegisted())
		{
			if (MainMenuScript.mInstance != null)
			{
				UIMsgBox.instance.ShowMessage(MainMenuScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_29"), 1, 18);
			}
			else if (MainMenu2DScript.mInstance != null)
			{
				UIMsgBox.instance.ShowMessage(MainMenu2DScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_29"), 1, 18);
			}
			else if (LobbyScript.mInstance != null)
			{
				UIMsgBox.instance.ShowMessage(LobbyScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_29"), 1, 18);
			}
		}
		else if (MainMenuScript.mInstance != null)
		{
			UIMsgBox.instance.ShowMessage(MainMenuScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"), 1, 18);
		}
		else if (MainMenu2DScript.mInstance != null)
		{
			UIMsgBox.instance.ShowMessage(MainMenu2DScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"), 1, 18);
		}
		else if (LobbyScript.mInstance != null)
		{
			UIMsgBox.instance.ShowMessage(LobbyScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"), 1, 18);
		}
	}

	public override void OnResponseError()
	{
		if (mIsRegist && GameApp.GetInstance().GetUserState().GetOnlineRegisted())
		{
			if (MainMenuScript.mInstance != null)
			{
				UIMsgBox.instance.ShowMessage(MainMenuScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_29"), 1, 17);
			}
			else if (MainMenu2DScript.mInstance != null)
			{
				UIMsgBox.instance.ShowMessage(MainMenu2DScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_29"), 1, 17);
			}
			else if (LobbyScript.mInstance != null)
			{
				UIMsgBox.instance.ShowMessage(LobbyScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_29"), 1, 17);
			}
		}
		else if (MainMenuScript.mInstance != null)
		{
			UIMsgBox.instance.ShowMessage(MainMenuScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"), 1, 17);
		}
		else if (MainMenu2DScript.mInstance != null)
		{
			UIMsgBox.instance.ShowMessage(MainMenu2DScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"), 1, 17);
		}
		else if (LobbyScript.mInstance != null)
		{
			UIMsgBox.instance.ShowMessage(LobbyScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"), 1, 17);
		}
	}
}
