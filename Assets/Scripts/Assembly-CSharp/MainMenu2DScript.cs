using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MainMenu2DScript : MonoBehaviour, UIMsgListener
{
	public struct PhotoInfo
	{
		public string fileName;

		public long time;
	}

	public static MainMenu2DScript mInstance;

	public Material AtlasMaterial;

	public Material AtlasMaterial_Runtime;

	public GameObject SingleButton;

	public GameObject MultiplayButton;

	public GameObject OptionButton;

	public GameObject GameCenterButton;

	public GameObject CreditsButton;

	public GameObject OptionHighlight;

	public GameObject GameCenterHighlight;

	public GameObject CreditsHighlight;

	public GameObject EffectRoot;

	public UILabel VersionLabel;

	public NetworkManager networkMgr;

	private float mWaitToLoadDelay;

	private float mWaitToLoadTime = 1f;

	private float mStartConnectToGameServerTime;

	private float mClickTime;

	private bool mCampaignClicked;

	private bool mMultiplayClicked;

	private string mLoadLevelName = string.Empty;

	private bool mDelayPassed;

	private bool mStartConnectToGameServer;

	private IRecordset mRecordFromServer;

	private GameObject Achievement;

	private Timer AchievementLoadTimer;

	private bool IsDispayQuitDlog = true;

	private string mithrilHandlerMessage = string.Empty;

	private bool isGoingToMultiMenu;

	[HideInInspector]
	public static string mInitTimeAnalyseString = string.Empty;

	private DateTime lastInitTime = DateTime.Now;

	public bool bConnection { get; set; }

	public byte loadResult { get; set; }

	public bool bIsRegist { get; set; }

	public bool bSendDownloadData { get; set; }

	public bool bCanUploadData { get; set; }

	public bool bNeedResetFriendTime { get; set; }

	private void Awake()
	{
		mInstance = this;
		GameApp.GetInstance().GetGameMode().TypeOfNetwork = NetworkType.Single;
		UIEventListener uIEventListener = UIEventListener.Get(SingleButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(MultiplayButton);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(CreditsButton);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener4 = UIEventListener.Get(GameCenterButton);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener5 = UIEventListener.Get(OptionButton);
		uIEventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener5.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener6 = UIEventListener.Get(CreditsButton);
		uIEventListener6.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener6.onPress, new UIEventListener.BoolDelegate(OnButtonPress));
		UIEventListener uIEventListener7 = UIEventListener.Get(GameCenterButton);
		uIEventListener7.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener7.onPress, new UIEventListener.BoolDelegate(OnButtonPress));
		UIEventListener uIEventListener8 = UIEventListener.Get(OptionButton);
		uIEventListener8.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener8.onPress, new UIEventListener.BoolDelegate(OnButtonPress));
		GameCenterButton.SetActive(false);
	}

	private void OnDestroy()
	{
		UIMsgBox.instance.CloseMessage();
		mInstance = null;
		GameApp.GetInstance().Save();
	}

	private void Start()
	{
		GameApp.GetInstance().CreateGameAppScript();
		bConnection = false;
		loadResult = 0;
		GameDataManager.GetInstance().Init();
		TimeManager.GetInstance().Init();
		TimeManager.GetInstance().setMaxLoopTimes(-1);
		TimeManager.GetInstance().setPeriod(3f);
		if (UIMsgBox.instance == null)
		{
			GameObject original = Resources.Load("UI/Common/MessageBox/Prefab/MsgUI") as GameObject;
			UnityEngine.Object.Instantiate(original, new Vector3(0f, -1500f, 0f), Quaternion.identity);
		}
		InitUserState();
		InitWordAtlas();
		GameObject gameObject = GameObject.Find(AudioName.MENU_MUSIC_NAME);
		if (gameObject == null)
		{
			GameObject original2 = Resources.Load(AudioName.BGM_MENU_SINGLE) as GameObject;
			gameObject = UnityEngine.Object.Instantiate(original2, Vector3.zero, Quaternion.identity) as GameObject;
			gameObject.name = AudioName.MENU_MUSIC_NAME;
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<MenuMusicScript>();
		}
		GameApp.GetInstance().GetUserState().Achievement.LateInit();
		MissionManager.GetInstance().Init();
		mCampaignClicked = false;
		mMultiplayClicked = false;
		if (LoadingUIScript.mInstance == null)
		{
			GameObject original3 = Resources.Load("UI/Common/LoadingUI/Prefab/LoadingUI") as GameObject;
			UnityEngine.Object.Instantiate(original3, new Vector3(0f, 500f, 0f), Quaternion.identity);
		}
		if (loadResult == 1)
		{
			UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_SAVE_DATA_CRASH"), 1, 92);
		}
		else if (!UIMsgBox.instance.IsMessageShow() && (DateTime.Now - GameApp.GetInstance().GetUserState().GetRemindSyncRecordTime()).Days >= 2)
		{
			UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_ONLINE_BACKUP_1"), 2, 65);
			GameApp.GetInstance().GetUserState().SetRemindSyncRecordTime(DateTime.Now);
		}
		if (FreyrPlugin.CanUseVirtualCursor() != GCType.GC_None && VirtualCursorScript.mInstance == null)
		{
			GameObject target = UnityEngine.Object.Instantiate(Resources.Load("JoyStick/VirtualMouseController")) as GameObject;
			UnityEngine.Object.DontDestroyOnLoad(target);
		}
		mDelayPassed = false;
		mStartConnectToGameServer = false;
		InitEffectRoot();
		Vector3 localPosition = Camera.main.transform.localPosition;
		Quaternion localRotation = Camera.main.transform.localRotation;
		Camera.main.gameObject.AddComponent<AudioManager>();
		Camera.main.transform.localPosition = localPosition;
		Camera.main.transform.localRotation = localRotation;
		if (GameObject.Find("ApplicationPauseManager") == null)
		{
			GameObject original4 = Resources.Load("UI/Common/ApplicationPauseManager") as GameObject;
			GameObject gameObject2 = UnityEngine.Object.Instantiate(original4) as GameObject;
			gameObject2.name = "ApplicationPauseManager";
			UnityEngine.Object.DontDestroyOnLoad(gameObject2);
		}
		if (!TutorialManager.GetInstance().GetTutorialWatched(TutorialTriggerType.REGISTER_TUTORIAL))
		{
			LoginPageScript.mTurnToRegister = true;
		}
		if (LoginPageScript.mTurnToRegister)
		{
			LoginPageScript.mTurnToRegister = false;
			UIMsgBox.instance.CloseMessage();
			SwAccountUdidCheckRequest request = new SwAccountUdidCheckRequest(false);
			GameApp.GetInstance().GetWWWManager().SendRequest(request);
			if (!UIMsgBox.instance.IsMessageShow() && GameObject.Find("TutorialUI(Clone)") == null)
			{
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_11"), 7);
			}
		}
		RemovePhoto();
		VersionLabel.text = "Version 1.27";
		UnityEngine.Object original5 = Resources.Load(EffectName.MENU_LIGHTING);
		GameObject gameObject3 = UnityEngine.Object.Instantiate(original5) as GameObject;
		GameApp.GetInstance().GetUserState().ClearTrialWeapon();
		GameApp.GetInstance().GetUserState().ClearWeaponUpgradeID();
		if (AndroidConstant.isFacebook())
		{
			FacebookTools.Init();
		}
		GameApp.GetInstance().isMainMenuLoaded = true;
		AdsScript.mInstance.InitAds();
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	public void SetInitAdTime(string type)
	{
		string text = mInitTimeAnalyseString;
		mInitTimeAnalyseString = text + type + ":" + (DateTime.Now - lastInitTime).TotalMilliseconds + "\n";
		lastInitTime = DateTime.Now;
	}

	private void Update()
	{
		UpdateNetwork();
		if (bConnection)
		{
			networkMgr = GameApp.GetInstance().GetNetworkManager();
			if (networkMgr != null)
			{
				if (networkMgr.conn != null)
				{
					TimeManager.GetInstance().Loop();
				}
				else
				{
					bConnection = false;
				}
			}
			else
			{
				bConnection = false;
			}
		}
		if (mCampaignClicked)
		{
			if (!mDelayPassed && Time.time - mClickTime > mWaitToLoadDelay)
			{
				mDelayPassed = true;
				OpenCampaign();
			}
			else if (mDelayPassed && Time.time - mClickTime > mWaitToLoadTime + mWaitToLoadDelay)
			{
				if (TutorialManager.GetInstance().Trigger(TutorialTriggerType.GAME_STORY))
				{
					mCampaignClicked = false;
					return;
				}
				LoadingUIScript.mInstance.ShowAfterMissionTip(GameState.Playing, MissionType.NONE, false, false);
				GameWorld.LoadLevel(mLoadLevelName, ELevelType.MENU);
			}
		}
		if (mMultiplayClicked && !mDelayPassed && Time.time - mClickTime > mWaitToLoadDelay)
		{
			mDelayPassed = true;
			OpenMulti();
			Login();
		}
		if (mStartConnectToGameServer && Time.time - mStartConnectToGameServerTime > Global.MAX_LOGIN_WAITING_TIME)
		{
			if (bIsRegist && GameApp.GetInstance().GetUserState().GetOnlineRegisted())
			{
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_29"), 1, 18);
				bIsRegist = false;
				mStartConnectToGameServerTime = 10000f;
			}
			else
			{
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"), 1, 18);
				mStartConnectToGameServerTime = 10000f;
			}
			if (networkMgr != null)
			{
				networkMgr.CloseConnection();
			}
		}
		if (AchievementLoadTimer != null && AchievementLoadTimer.Ready())
		{
			AchievementLoadTimer.Pause();
			UIMsgBox.instance.CloseMessage();
			Achievement.SetActive(true);
		}
		if (GameApp.GetInstance().GetUserState().showRewardMsg)
		{
			UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_AD_WIN_MITH", GameApp.GetInstance().GetUserState().rewardNumber, GameApp.GetInstance().GetUserState().rewardAdsName));
			GameApp.GetInstance().GetUserState().showRewardMsg = false;
			GameApp.GetInstance().GetUserState().rewardAdsName = string.Empty;
			GameApp.GetInstance().GetUserState().rewardNumber = 0;
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (IsDispayQuitDlog)
			{
				UIMsgBox.instance.ShowExitGameMessage(this);
				IsDispayQuitDlog = false;
			}
			else
			{
				UIMsgBox.instance.CloseMessage();
				IsDispayQuitDlog = true;
			}
		}
	}

	public void OnLoginFail()
	{
		if (mMultiplayClicked)
		{
			CloseMulti();
		}
		mCampaignClicked = false;
		mMultiplayClicked = false;
		mDelayPassed = false;
		mStartConnectToGameServer = false;
		GameApp.GetInstance().DestoryNetWork();
	}

	public void OnButtonClick(GameObject go)
	{
		if (mCampaignClicked || mMultiplayClicked)
		{
			return;
		}
		if (go == SingleButton)
		{
			if (GameApp.GetInstance().IsConnectedToInternet() && AdsScript.mInstance != null && GameApp.GetInstance().isAdInitFinished)
			{
				AdsScript.mInstance.TrackAppFlyer();
			}
			mCampaignClicked = true;
			mClickTime = Time.time;
			mLoadLevelName = "ShopAndCustomize2";
			AudioManager.GetInstance().PlaySound(AudioName.MENU_DOOR_OPEN);
		}
		else if (go == MultiplayButton)
		{
			if (GameApp.GetInstance().IsConnectedToInternet())
			{
				if (AdsScript.mInstance != null && GameApp.GetInstance().isAdInitFinished)
				{
					AdsScript.mInstance.TrackAppFlyer();
				}
				if (GameApp.GetInstance().GetUserState().HasLoginOnline())
				{
					mMultiplayClicked = true;
					mClickTime = Time.time;
					AudioManager.GetInstance().PlaySound(AudioName.MENU_DOOR_OPEN);
					return;
				}
				SwAccountUdidCheckRequest request = new SwAccountUdidCheckRequest(false);
				GameApp.GetInstance().GetWWWManager().SendRequest(request);
				if (!UIMsgBox.instance.IsMessageShow() && GameObject.Find("TutorialUI(Clone)") == null)
				{
					UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_11"), 7);
				}
			}
			else
			{
				UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_6"));
			}
		}
		else if (go == GameCenterButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
		}
		else if (go == CreditsButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (!UIMsgBox.instance.IsMessageShow())
			{
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_ACHIEVEMENT_TIPS_3"), 7);
			}
			AchievementLoadTimer = new Timer(0.2f, false);
			GameObject original = Resources.Load("UI/Menu/AchievementUI/Prefab/AchievementUI") as GameObject;
			Achievement = (GameObject)UnityEngine.Object.Instantiate(original, new Vector3(0f, 1500f, 0f), Quaternion.identity);
			Achievement.SetActive(false);
		}
		else if (go == OptionButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			GameObject original2 = Resources.Load("UI/Menu/OptionUI/Prefab/OptionUI") as GameObject;
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original2, new Vector3(0f, 1500f, 0f), Quaternion.identity);
			gameObject.GetComponent<OptionUIScript>().SetIsInMainMenu(true);
		}
	}

	private void OnButtonPress(GameObject go, bool isPress)
	{
		if (go == OptionButton)
		{
			OptionHighlight.SetActive(isPress);
		}
		else if (go == CreditsButton)
		{
			CreditsHighlight.SetActive(isPress);
		}
		else if (go == GameCenterButton)
		{
			GameCenterHighlight.SetActive(isPress);
		}
	}

	public void OpenCampaign()
	{
	}

	public void OpenMulti()
	{
	}

	public void CloseMulti()
	{
	}

	private void InitUserState()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		if (userState.bInit)
		{
			return;
		}
		loadResult = GameApp.GetInstance().Load();
		if (loadResult != 0)
		{
			userState.Init();
			if (mInstance != null)
			{
				mInstance.InitLanguage();
			}
		}
		userState.CheckSaveIdentity();
		userState.InitLevelName();
		userState.InitRank();
		userState.bInit = true;
		GameApp.GetInstance().StartHttpRequestThread();
		SocialManager.GetInstance().InitSocialManager();
		LoginBackground();
		if (AndroidConstant.version == AndroidConstant.Version.Kindle)
		{
			AndroidIAPPluginScript.GetRestorePurchse();
		}
	}

	private void UpdateNetwork()
	{
		networkMgr = GameApp.GetInstance().GetNetworkManager();
		if (networkMgr != null)
		{
			networkMgr.ProcessReceivedPackets();
		}
	}

	public void Login()
	{
		SwAccountLoginRequest request = new SwAccountLoginRequest(false);
		GameApp.GetInstance().GetWWWManager().SendRequest(request);
		if (!UIMsgBox.instance.IsMessageShow() && GameObject.Find("TutorialUI(Clone)") == null)
		{
			UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_11"), 7);
		}
	}

	public void ConnectToGameServer(string ip, int port, int userId, string date, EUserType userType, bool isRegist)
	{
		Lobby.GetInstance().FirstBossChallenge = false;
		GameApp.GetInstance().DestoryNetWork();
		GameApp.GetInstance().CreateNetwork(ip, port);
		RoleLoginRequest request = new RoleLoginRequest(Lobby.GetInstance().GetUserName(), userId, date, (byte)userType, isRegist);
		GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_PVP_CONNECT"), 7);
		mStartConnectToGameServer = true;
		mStartConnectToGameServerTime = Time.time;
		Debug.Log("login game server :" + ip + ":" + port + ",   userType = " + userType);
	}

	public string GetRandomString(int length)
	{
		string empty = string.Empty;
		char c = (char)UnityEngine.Random.Range(65, 91);
		empty += c;
		for (int i = 0; i < length; i++)
		{
			c = (char)UnityEngine.Random.Range(65, 91);
			empty += c;
		}
		return empty;
	}

	public void LogicServerFailed()
	{
		UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_ERROR_CODE_113"), 1, 17);
	}

	public void LogicSavaDataInvalid()
	{
		UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_SAVE_DATA_CRASH"), 1, 92);
	}

	public void UpLoadData()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		userState.SetSyncRecordTime(Lobby.GetInstance().GetLoginTime());
		userState.SetRemindSyncRecordTime(Lobby.GetInstance().GetLoginTime());
		userState.CheckNeedResetFriendTime(bNeedResetFriendTime);
		GameApp.GetInstance().Save();
		UploadDataRequest request = new UploadDataRequest(userState, bIsRegist);
		networkMgr.SendRequest(request);
		if (!UIMsgBox.instance.IsMessageShow() && GameObject.Find("TutorialUI(Clone)") == null)
		{
			UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_PVP_CONNECT"), 7, 61);
		}
	}

	public void UploadDataLogic()
	{
		if (mRecordFromServer != null && bSendDownloadData)
		{
			UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_28"), 1, 60);
			GameObject original = Resources.Load("UI/Menu/CompareRecord/Prefab/CompareRecordUI") as GameObject;
			GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
			CompareRecordUI component = gameObject.GetComponent<CompareRecordUI>();
			if (null != component)
			{
				component.IsRegisted = bIsRegist;
				component.ShowUseLocalButton(bCanUploadData);
			}
			mStartConnectToGameServer = false;
		}
		else
		{
			UpLoadData();
			ClearRecordFromServer();
		}
	}

	public void OnUploadDataComplete(bool isRegist)
	{
		mStartConnectToGameServer = false;
		UIMsgBox.instance.CloseMessage();
		if (isRegist)
		{
			networkMgr = GameApp.GetInstance().GetNetworkManager();
			if (networkMgr != null)
			{
				networkMgr.CloseConnection();
			}
			if (LoginPageScript.mInstance.RegisterPage.activeSelf)
			{
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_29"), 1, 21);
			}
			else
			{
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_30"), 1, 21);
			}
		}
		else
		{
			GotoMultiMenu();
		}
	}

	public void CorrectLogin()
	{
		bConnection = true;
	}

	public void GotoMultiMenu()
	{
		GameObject gameObject = GameObject.Find(AudioName.MENU_MUSIC_NAME);
		if (gameObject != null)
		{
			UnityEngine.Object.DestroyObject(gameObject);
		}
		Application.LoadLevel("MultiMenu");
	}

	public void SetRecordFromServer(IRecordset record)
	{
		mRecordFromServer = record;
	}

	public IRecordset GetRecordFromServer()
	{
		return mRecordFromServer;
	}

	public void ResetRecordFromServer()
	{
		if (mRecordFromServer == null)
		{
			return;
		}
		mRecordFromServer.ResetUserState();
		UserState userState = GameApp.GetInstance().GetUserState();
		if (userState.readVersion != "120")
		{
			if (userState.readVersion == "100" || userState.readVersion == "101" || userState.readVersion == "102" || userState.readVersion == "103")
			{
				GameApp.GetInstance().GetUserState().v100To104MithrilHandler();
			}
			int result = 0;
			int.TryParse(userState.readVersion, out result);
			if (result != 0 && result <= 113)
			{
				GameApp.GetInstance().GetUserState().SmoothMoveHandler();
			}
			GameApp.GetInstance().GetUserState().UnlockEquipsByLevel();
			if (GameApp.GetInstance().GetUserState().GetUnlockPackBought())
			{
				GameApp.GetInstance().GetUserState().UnlockAllLevel_Weapons_Equips();
			}
		}
		GameApp.GetInstance().GetUserState().CheckNeedResetFriendTime(bNeedResetFriendTime);
		GameApp.GetInstance().GetUserState().CheckSaveIdentity();
		GameApp.GetInstance().Save();
	}

	public void ClearRecordFromServer()
	{
		mRecordFromServer = null;
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (whichMsg.EventId == 18 || whichMsg.EventId == 17)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				UIMsgBox.instance.CloseMessage();
				if (LoginPageScript.mInstance != null)
				{
					LoginPageScript.mInstance.Refresh();
				}
				if (GameApp.GetInstance().GetUserState().GetOnlineRegisted())
				{
					GameApp.GetInstance().GetUserState().Achievement.CheckAchievement_Register();
				}
				OnLoginFail();
			}
		}
		else if (whichMsg.EventId == 16)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				UIMsgBox.instance.CloseMessage();
				if (LoginPageScript.mInstance != null)
				{
					LoginPageScript.mInstance.Refresh();
				}
				if (GameApp.GetInstance().GetUserState().GetOnlineRegisted())
				{
					GameApp.GetInstance().GetUserState().Achievement.CheckAchievement_Register();
				}
				OnLoginFail();
			}
		}
		else if (whichMsg.EventId == 21)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				UIMsgBox.instance.CloseMessage();
				if (!TutorialManager.GetInstance().GetTutorialWatched(TutorialTriggerType.VOID_TUTORIAL))
				{
					Application.LoadLevel("ShopAndCustomize2");
				}
				if (LoginPageScript.mInstance != null)
				{
					LoginPageScript.mInstance.Refresh();
				}
				if (CompareRecordUI.mInstance != null)
				{
					CompareRecordUI.mInstance.gameObject.SetActive(false);
				}
				ShowMithrilHandleMessage();
				GameApp.GetInstance().GetUserState().Achievement.CheckAchievement_Register();
			}
		}
		else if (whichMsg.EventId == 60)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				UIMsgBox.instance.CloseMessage();
			}
		}
		else if (whichMsg.EventId == 23)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				LoginPageScript.mTurnToRegister = true;
				OnButtonClick(OptionButton);
			}
			else
			{
				mMultiplayClicked = true;
				mClickTime = Time.time;
				AudioManager.GetInstance().PlaySound(AudioName.MENU_DOOR_OPEN);
			}
			UIMsgBox.instance.CloseMessage();
		}
		else if (whichMsg.EventId == 65)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				OnButtonClick(MultiplayButton);
			}
			UIMsgBox.instance.CloseMessage();
		}
		else if (whichMsg.EventId == 63)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok && isGoingToMultiMenu)
			{
				GotoMultiMenu();
				isGoingToMultiMenu = false;
			}
			UIMsgBox.instance.CloseMessage();
		}
		else if (whichMsg.EventId == 62)
		{
			switch (buttonId)
			{
			case UIMsg.UIMsgButton.Cancel:
				LoginPageScript.mTurnToLogin = true;
				OnButtonClick(OptionButton);
				break;
			case UIMsg.UIMsgButton.Ok:
				if (GameApp.GetInstance().IsConnectedToInternet())
				{
					SwAccountLoginRequest request = new SwAccountLoginRequest(false);
					GameApp.GetInstance().GetWWWManager().SendRequest(request);
					if (!UIMsgBox.instance.IsMessageShow() && GameObject.Find("TutorialUI(Clone)") == null)
					{
						UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_11"), 7);
					}
				}
				else
				{
					UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_6"));
				}
				break;
			}
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			UIMsgBox.instance.CloseMessage();
		}
		else if (whichMsg.EventId == 92)
		{
			Debug.Log("Send email.");
			UIMsgBox.instance.CloseMessage();
			OnLoginFail();
			if (Application.platform != RuntimePlatform.WindowsEditor)
			{
				string text = string.Empty;
				if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					text = "[IOS]";
				}
				else if (Application.platform == RuntimePlatform.Android)
				{
					text = "[Android]";
				}
				else if (Application.platform == RuntimePlatform.WP8Player)
				{
					text = "[WP]";
				}
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
				string text2 = LocalizationManager.GetInstance().GetString("LOC_REPORTMAIL_4") + text;
				string text3 = LocalizationManager.GetInstance().GetString("LOC_REPORTMAIL_5");
				string text4 = LocalizationManager.GetInstance().GetString("LOC_REPORTMAIL_6");
				string text5 = "Version: " + GameApp.GetInstance().GetUserState().readVersion;
				string text6 = "ID: " + GameApp.GetInstance().GetUserState().GetOnlineUserId();
				string text7 = "Language: " + LocalizationManager.GetInstance().currentLanguage;
				string text8 = "Device: " + SystemInfo.deviceModel;
				string text9 = "System: " + SystemInfo.operatingSystem;
				string text10 = "UUID: " + GameApp.GetInstance().UUID;
				string text11 = "mailto:ios@ifreyr.com?subject=" + text2 + "&body=" + text3 + "%0A%0A%0A%0A%0A%0A" + text4 + "%0A" + text5 + "%0A" + text6 + "%0A" + text7 + "%0A" + text8 + "%0A" + text9 + "%0A" + text10;
				Application.OpenURL(text11.Replace(" ", "%20"));
			}
		}
		else if (whichMsg.EventId == 30)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				GameApp.GetInstance().Save();
				Application.Quit();
			}
			else
			{
				UIMsgBox.instance.CloseMessage();
				IsDispayQuitDlog = true;
			}
		}
	}

	public void InitEffectRoot()
	{
		EffectRoot = GameObject.Find("_EffectRoot");
		if (EffectRoot == null)
		{
			EffectRoot = new GameObject("_EffectRoot");
			EffectRoot.AddComponent<EffectPool>();
			UnityEngine.Object.DontDestroyOnLoad(EffectRoot);
		}
	}

	public void GoMultiMenuForQuickRegist()
	{
		mMultiplayClicked = true;
		mClickTime = Time.time;
		AudioManager.GetInstance().PlaySound(AudioName.MENU_DOOR_OPEN);
	}

    public void InitLanguage()
    {
        string currentLanguage = ExternScript.GetCurrentLanguage();
        Debug.Log("The current language key is " + currentLanguage);
        EUserLanguage euserLanguage = GameApp.GetInstance().GetUserState().GetUserLanguage();
        if (euserLanguage == EUserLanguage.DEFAULT)
        {
            switch (currentLanguage)
            {
                case "zh-Hant":
                case "zh-Hant-CN":
                case "zh-HK":
                case "zh-TW":
                    euserLanguage = EUserLanguage.CHINESE_TRADITIONAL;
                    break;

                case "zh-Hans":
                case "zh-Hans-CN":
                    euserLanguage = EUserLanguage.CHINESE_SIMPLIFIED;
                    break;

                default:
                    string prefix = currentLanguage.Substring(0, 2);
                    switch (prefix)
                    {
                        case "en":
                            euserLanguage = EUserLanguage.ENGLISH;
                            break;
                        case "fr":
                            euserLanguage = EUserLanguage.FRENCH;
                            break;
                        case "de":
                            euserLanguage = EUserLanguage.GERMAN;
                            break;
                        case "ko":
                            euserLanguage = EUserLanguage.KOREAN;
                            break;
                        case "es":
                            euserLanguage = EUserLanguage.SPANISH;
                            break;
                        case "ja":
                            euserLanguage = EUserLanguage.JAPANESE;
                            break;
                        case "ru":
                            euserLanguage = EUserLanguage.RUSSIAN;
                            break;
                        default:
                            euserLanguage = EUserLanguage.ENGLISH;
                            break;
                    }
                    break;
            }
        }
        LocalizationManager.GetInstance().currentLanguage = euserLanguage;
        Debug.Log("The current language is " + LocalizationManager.GetInstance().currentLanguage.ToString());
    }

    private void InitWordAtlas()
	{
		UIAtlasManager.Mat_InGame = AtlasMaterial_Runtime;
		UIAtlasManager.Mat_OutOfGame = AtlasMaterial;
		UIAtlasManager.UpdateMatOutOfGame();
	}

	private void RemovePhoto()
	{
		string path = Application.persistentDataPath + "/Documents";
		if (!Directory.Exists(path))
		{
			return;
		}
		int num = 50;
		string[] files = Directory.GetFiles(path, "*.png");
		if (files.Length > num)
		{
			List<PhotoInfo> list = new List<PhotoInfo>();
			string[] array = files;
			PhotoInfo item = default(PhotoInfo);
			foreach (string fileName in array)
			{
				FileInfo fileInfo = new FileInfo(fileName);
				item.fileName = fileName;
				item.time = fileInfo.CreationTime.Ticks;
				list.Add(item);
			}
			list.Sort(SortByTime);
			for (int j = 0; j < files.Length - num; j++)
			{
				File.Delete(list[j].fileName);
			}
		}
	}

	public static int SortByTime(PhotoInfo a, PhotoInfo b)
	{
		if (a.time - b.time < 0)
		{
			return -1;
		}
		if (a.time - b.time > 0)
		{
			return 1;
		}
		return 0;
	}

	public void UpdateOrDownloadDataOver()
	{
		ClearRecordFromServer();
		UIMsgBox.instance.CloseMessage();
		if (!bIsRegist)
		{
			if (mithrilHandlerMessage != string.Empty)
			{
				ShowMithrilHandleMessage();
				isGoingToMultiMenu = true;
			}
			else
			{
				GotoMultiMenu();
			}
		}
	}

	private void LoginBackground()
	{
		long lastLoginTicks = GameApp.GetInstance().GetUserState().GetLastLoginTicks();
		long nextLoginInterval = GameApp.GetInstance().GetUserState().GetNextLoginInterval();
		long ticks = DateTime.Now.Ticks;
		if (ticks - lastLoginTicks > nextLoginInterval)
		{
			SwBackgroundLoginRequest request = new SwBackgroundLoginRequest();
			GameApp.GetInstance().GetWWWManager().SendRequest(request);
		}
	}

	public void SetMithrilHandleMessage(string value)
	{
		mithrilHandlerMessage = value;
	}

	public void ShowMithrilHandleMessage()
	{
		if (mithrilHandlerMessage != string.Empty)
		{
			UIMsgBox.instance.ShowMessage(this, mithrilHandlerMessage, 1, 63);
			mithrilHandlerMessage = string.Empty;
		}
	}
}
