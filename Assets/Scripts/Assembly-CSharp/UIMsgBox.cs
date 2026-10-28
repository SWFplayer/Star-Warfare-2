using UnityEngine;

public class UIMsgBox : MonoBehaviour, UIMsgListener
{
	public const byte EVENT_TYPE_NOT_DEFINED = 0;

	public const byte EVENT_TYPE_DISCONNECTION = 1;

	public const byte EVENT_CONFIRM_TO_USE_MITHRIL = 2;

	public const byte EVENT_MITHRIL_NOT_ENOUGH = 3;

	public const byte EVENT_GOLD_NOT_ENOUGH = 4;

	public const byte EVENT_OPTION_UNABLED = 5;

	public const byte EVENT_REVIEW = 6;

	public const byte EVENT_CONFIRM_ONLY_CLOSE_MESSAGE_BOX = 7;

	public const byte EVENT_FACEBOOKLIKE = 8;

	public const byte EVENT_POSTTOFACEBOOK = 9;

	public const byte EVENT_POSTTOTWITTER = 10;

	public const byte EVENT_CAN_NOT_ACCESS_SERVER = 11;

	public const byte EVENT_NET_TIMEOUT = 12;

	public const byte EVENT_JOIN_ROOM_FAILED = 13;

	public const byte EVENT_SERVER_MAINTAINEANCE = 14;

	public const byte EVENT_ACCOUNT_LOCKED = 15;

	public const byte EVENT_VERSION_MISMATCH = 16;

	public const byte EVENT_LOGIN_SERVER_FAILED = 17;

	public const byte EVENT_LOGIN_TIMEOUT = 18;

	public const byte EVENT_CREATE_ROOM_FAILED = 19;

	public const byte EVENT_PASSWORD_INCORRECT = 20;

	public const byte EVENT_REGISTER_SUCCESSED = 21;

	public const byte EVENT_FRIEND_DELETE_CONFIRM = 22;

	public const byte EVENT_ACCOUNT_NOT_REGIST = 23;

	public const byte EVENT_CANT_SHOW_USERINFO = 24;

	public const byte EVENT_FRIEND_CANNOT_SUMMON = 25;

	public const byte EVENT_BUY_UNLOCK_IAP = 26;

	public const byte EVENT_FAIL_TO_SHOP = 27;

	public const byte EVENT_EXTRA_GOLD_AND_MITHRIL = 28;

	public const byte EVENT_EXIT_IN_PVP = 29;

	public const byte EVENT_QUITGAME = 30;

	public const byte EVENT_IAP_SERVER_TIME_OUT = 31;

	public const byte EVENT_INVITAION_IN_MENU = 32;

	public const byte EVENT_MITHRIL_REWARDS = 33;

	public const byte EVENT_CONFIRM_TO_DISCONNECT = 34;

	public const byte EVENT_PLEASE_RATE_US = 35;

	public const byte EVENT_NO_VIDEO_TO_WATCH = 36;

	public const byte EVENT_FIRST_HUNTER = 37;

	public const byte EVENT_PAY_FOR_RESPAWN = 38;

	public const byte EVENT_FORCE_LOGOUT = 39;

	public const byte EVENT_MULTICOLOR_LOCKED = 40;

	public const byte EVENT_JOIN_FAILED_PVP_LEVEL_UNFIT = 41;

	public const byte EVENT_GIVE_UP_TRIAL_WEAPON = 42;

	public const byte EVENT_GIVE_UP_WEAPON_UPGRADE_FAVORABLE = 43;

	public const byte EVENT_FAIL_WITH_NO_CONTROL = 43;

	public const byte EVENT_PUSH_ITEM = 44;

	public const byte EVENT_SWVOID_UPGRADE = 50;

	public const byte EVENT_SWVOID_RESOLVE = 51;

	public const byte EVENT_SWVOID_KEEP_UPGRADE = 52;

	public const byte EVENT_SWVOID_WAITING = 53;

	public const byte EVENT_SWVOID_NO_ENOUGH_MATERIAL = 54;

	public const byte EVENT_UPLOAD_OR_DOWNLOAD_DATA = 60;

	public const byte EVENT_WAIT_UPLOAD_DATA = 61;

	public const byte EVENT_BIND_OR_LOGIN = 62;

	public const byte EVENT_MITHRIL_HANDLED = 63;

	public const byte EVENT_RENAME_CONFIRM = 64;

	public const byte EVENT_REMIND_UPLOAD_DATA = 65;

	public const byte EVENT_PRESTIGE = 70;

	public const byte NOT_EQUIP_SWVOID = 80;

	public const byte EVENT_NOT_ENOUGH_PVP_LEVEL = 81;

	public const byte QUICK_JOIN_NO_ROOM = 90;

	public const byte IAP_NO_INTERNET_CONNET = 91;

	public const byte RECORD_ERROR = 92;

	public const byte EVENT_IAP_WAIT = 93;

	public const byte EVENT_BUY_PACKAGE_SUCCESS = 94;

	public const byte EVENT_QUERY_KICK_PLAYER = 95;

	public const byte TYPE_UNKNOWN = 0;

	public const byte TYPE_CONFIRM = 1;

	public const byte TYPE_QUERY = 2;

	public const byte TYPE_REVIEW = 3;

	public const byte TYPE_TIMED_CONFIRM = 4;

	public const byte TYPE_TIMED_QUERY = 5;

	public const byte TYPE_TIMED_WAIT = 6;

	public const byte TYPE_WAIT = 7;

	public const byte TYPE_REGISTER = 8;

	public const byte TYPE_WAIT_ROLLING = 9;

	public const byte TYPE_CALL_STACK = 10;

	public const byte TYPE_BIND_OR_LOGIN = 11;

	public static UIMsgBox instance;

	public GameObject m_Anchor;

	public GameObject m_Camera;

	public UIMsg[] m_Msg;

	private UIMsg m_CurrentMsg;

	private bool mNeedCloseSelfWhenChangeScene;

	private int currentErrorCode;

	private bool mIsWaitingMsgShowing;

	private float mWaitingMsgOverTime = 30f;

	private float mWaitingIapTime = 120f;

	private float mWaitingMsgShowTime;

	private byte mCurrentEvent;

	private void Awake()
	{
		instance = this;
		InactivateSelf();
		UIMsg[] msg = m_Msg;
		foreach (UIMsg uIMsg in msg)
		{
			uIMsg.gameObject.SetActive(false);
		}
		Object.DontDestroyOnLoad(base.gameObject);
		mIsWaitingMsgShowing = false;
	}

	private void Destroy()
	{
		instance = null;
	}

	public UIMsg ShowSystemMessage(UIMsgListener listener, string info, byte type, byte eventId, float time)
	{
		return ShowMessage(listener, info, type, eventId, true, time);
	}

	public UIMsg ShowSystemMessage(UIMsgListener listener, string info, byte type, byte eventId)
	{
		return ShowMessage(listener, info, type, eventId, true, -1f);
	}

	public UIMsg ShowMessage(UIMsgListener listener, string info, byte type, byte eventId, float time)
	{
		return ShowMessage(listener, info, type, eventId, false, time);
	}

	public UIMsg ShowMessage(UIMsgListener listener, string info, byte type, byte eventId)
	{
		return ShowMessage(listener, info, type, eventId, false, -1f);
	}

	public UIMsg ShowMessage(UIMsgListener listener, string info, byte type)
	{
		return ShowMessage(listener, info, type, 0, false, -1f);
	}

	public UIMsg ShowMessage(UIMsgListener listener, string info, byte type, float time)
	{
		return ShowMessage(listener, info, type, 0, false, time);
	}

	public UIMsg ShowMoneyNotEnoughMessage(UIMsgListener listener)
	{
		return ShowMessage(listener, LocalizationManager.GetInstance().GetString("LOC_IAP_TIPS_1"), 2, 4);
	}

	public UIMsg ShowMithrilNotEnoughMessage(UIMsgListener listener)
	{
		return ShowMessage(listener, LocalizationManager.GetInstance().GetString("LOC_IAP_TIPS_2"), 2, 3);
	}

	public UIMsg ShowOptionUnabledMessage(UIMsgListener listener)
	{
		return ShowMessage(listener, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_9"), 1, 5);
	}

	public UIMsg ShowReview(UIMsgListener listener)
	{
		switch (AndroidConstant.version)
		{
		case AndroidConstant.Version.GooglePlay:
			return ShowMessage(listener, LocalizationManager.GetInstance().GetString("LOC_IAP_TIPS_3_GOOGLE"), 3, 6);
		case AndroidConstant.Version.Kindle:
			return ShowMessage(listener, LocalizationManager.GetInstance().GetString("LOC_IAP_TIPS_3_KINDLE"), 3, 6);
		default:
			return ShowMessage(listener, LocalizationManager.GetInstance().GetString("LOC_IAP_TIPS_3"), 3, 6);
		}
	}

	public UIMsg ShowNetworkTimeoutMessage(UIMsgListener listener)
	{
		return ShowMessage(listener, LocalizationManager.GetInstance().GetString("LOC_ERROR_TIMEOUT1"), 1, 34);
	}

	public UIMsg ShowForceLogoutMessage()
	{
		return ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_39"), 1, 39);
	}

	public UIMsg ShowException(UIMsgListener listener)
	{
		string info = LocalizationManager.GetInstance().GetString("LOC_ERROR_TIMEOUT2", GameApp.GetInstance().GetUserState().GetOnlineUserId());
		return ShowMessage(listener, info, 1, 34);
	}

	public UIMsg ShowConfirmMessage(string info)
	{
		return ShowMessage(this, info, 1, 7);
	}

	public UIMsg ShowErrorCodeMessage(int errorCode)
	{
		currentErrorCode = errorCode;
		return ShowConfirmMessage(LocalizationManager.GetInstance().GetErrorCodeString(errorCode));
	}

	public UIMsg ShowErrorCodeMessage(UIMsgListener listener, int errorCode, byte eventId)
	{
		currentErrorCode = errorCode;
		return ShowMessage(listener, LocalizationManager.GetInstance().GetErrorCodeString(errorCode), 1, eventId);
	}

	public UIMsg ShowExitGameMessage(UIMsgListener listener)
	{
		return ShowMessage(listener, "Quit Game?", 2, 30);
	}

	public UIMsg ShowExtraGoldAndMithrilMessage(UIMsgListener listener, int bonusMithril, int compensateMithril, int bonusGold, int compensateGold, int compensateExp)
	{
		string text = string.Empty;
		if (bonusMithril != 0)
		{
			text += LocalizationManager.GetInstance().GetString("LOC_SPECIAL_REWARD_1", bonusMithril);
		}
		if (compensateMithril != 0)
		{
			text += "\n";
			text += LocalizationManager.GetInstance().GetString("LOC_SPECIAL_REWARD_2", compensateMithril);
		}
		if (bonusGold != 0)
		{
			text += "\n";
			text += LocalizationManager.GetInstance().GetString("LOC_SPECIAL_REWARD_3", bonusGold);
		}
		if (compensateGold != 0)
		{
			text += "\n";
			text += LocalizationManager.GetInstance().GetString("LOC_SPECIAL_REWARD_4", compensateGold);
		}
		if (compensateExp != 0)
		{
			text += "\n";
			text += LocalizationManager.GetInstance().GetString("LOC_SPECIAL_REWARD_5", compensateExp);
		}
		return ShowMessage(listener, text, 1, 28);
	}

	public UIMsg ShowMessage(UIMsgListener listener, string info, byte type, byte eventId, bool ignoreOthers, float time)
	{
		if (m_CurrentMsg == null || !m_CurrentMsg.IgnoreOthers)
		{
			if (m_CurrentMsg != null)
			{
				m_CurrentMsg.gameObject.SetActive(false);
			}
			if (type == 7 || type == 9)
			{
				mIsWaitingMsgShowing = true;
				mWaitingMsgShowTime = Time.realtimeSinceStartup;
				mCurrentEvent = eventId;
			}
			else
			{
				mIsWaitingMsgShowing = false;
			}
			if (type < 1 || type > m_Msg.Length)
			{
				m_CurrentMsg = null;
			}
			else
			{
				m_CurrentMsg = m_Msg[type - 1];
			}
			if (time > 0f)
			{
				UIProgressBarX componentInChildren = m_CurrentMsg.GetComponentInChildren<UIProgressBarX>();
				if (componentInChildren != null)
				{
					componentInChildren.time = time;
				}
			}
			if (m_CurrentMsg != null)
			{
				ActivateSelf();
				m_CurrentMsg.m_Information.text = info;
				m_CurrentMsg.EventId = eventId;
				m_CurrentMsg.Type = type;
				m_CurrentMsg.IgnoreOthers = ignoreOthers;
				m_CurrentMsg.SetListener(listener);
				m_CurrentMsg.gameObject.SetActive(true);
			}
		}
		return m_CurrentMsg;
	}

	public void CloseMessage()
	{
		if (m_CurrentMsg != null)
		{
			m_CurrentMsg.gameObject.SetActive(false);
			m_CurrentMsg = null;
			InactivateSelf();
			mIsWaitingMsgShowing = false;
		}
	}

	public byte GetCurrentMessageEventID()
	{
		if (m_CurrentMsg != null)
		{
			return m_CurrentMsg.EventId;
		}
		return 0;
	}

	public byte GetCurrentMessageType()
	{
		if (m_CurrentMsg != null)
		{
			return m_CurrentMsg.Type;
		}
		return 0;
	}

	public bool IsMessageShow()
	{
		return m_CurrentMsg != null;
	}

	private void ActivateSelf()
	{
		base.gameObject.SetActive(true);
	}

	private void InactivateSelf()
	{
		base.gameObject.SetActive(false);
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (currentErrorCode == SwErrorCode.LOGIN_VERSION_MISMATCH)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				CloseMessage();
			}
		}
		else if (whichMsg.EventId == 34 || whichMsg.EventId == 39)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				mNeedCloseSelfWhenChangeScene = true;
				Lobby.GetInstance().IsMasterPlayer = false;
				Lobby.GetInstance().SetCurrentRoomID(-1);
				GameWorld.LoadLevel("MainMenu", ELevelType.MENU);
			}
		}
		else if (whichMsg.EventId == 23)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				CloseMessage();
			}
		}
		else if (whichMsg.EventId == 7)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				CloseMessage();
			}
		}
		else if ((whichMsg.EventId == 9 || whichMsg.EventId == 10) && buttonId == UIMsg.UIMsgButton.Ok)
		{
			CloseMessage();
		}
	}

	private void Update()
	{
		if (mNeedCloseSelfWhenChangeScene)
		{
			CloseMessage();
			mNeedCloseSelfWhenChangeScene = false;
		}
		if (!mIsWaitingMsgShowing)
		{
			return;
		}
		if (mCurrentEvent == 93)
		{
			if (Time.realtimeSinceStartup - mWaitingMsgShowTime >= mWaitingIapTime)
			{
				CloseMessage();
			}
		}
		else if (Time.realtimeSinceStartup - mWaitingMsgShowTime >= mWaitingMsgOverTime)
		{
			CloseMessage();
		}
	}
}
