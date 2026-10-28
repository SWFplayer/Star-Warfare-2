using UnityEngine;

public class MultiMenuScript : UIStateManager, UIHandler
{
	public static MultiMenuScript mInstance;

	private float startTime;

	public NetworkManager networkMgr;

	protected Timer fadeTimer = new Timer();

	private byte LoadingState;

	private static byte LOADING_SUBSTATE_RES;

	private static byte LOADING_SUBSTATE_AVATAR = 1;

	private bool mIsLogout;

	public GUIStyle playerNameStyle;

	public GUIStyle searchRoomStyle;

	private void Awake()
	{
		mInstance = this;
	}

	private void Destroy()
	{
		mInstance = null;
	}

	private void Start()
	{
		GameApp.GetInstance().GetGameMode().TypeOfNetwork = NetworkType.MultiPlayer_Internet;
		GameApp.GetInstance().GetGameMode().ModePlay = Mode.VS_TDM;
		GameApp.GetInstance().ClearGameWorld();
		LoadingState = LOADING_SUBSTATE_RES;
		FrGoToPhase(0, false, true, true);
		GameObject gameObject = GameObject.Find(AudioName.MENU_MUSIC_NAME);
		Camera.main.transform.position = Vector3.zero;
		if (gameObject == null)
		{
			GameObject original = Resources.Load(AudioName.BGM_MENU_ONLINE) as GameObject;
			gameObject = Object.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject;
			gameObject.name = AudioName.MENU_MUSIC_NAME;
			Object.DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<MenuMusicScript>();
		}
		TimeManager.GetInstance().Init();
		TimeManager.GetInstance().setMaxLoopTimes(-1);
		TimeManager.GetInstance().setPeriod(1f);
		mIsLogout = false;
		TutorialManager.GetInstance().Trigger(TutorialTriggerType.FIRST_ONLINE);
	}

	private void OnDestroy()
	{
		GameApp.GetInstance().Save();
	}

	private void InitUIManager()
	{
		m_UIManager = base.gameObject.AddComponent<UIManager>() as UIManager;
		m_UIManager.SetParameter(24, 1, false);
		m_UIManager.SetUIHandler(this);
		m_UIPopupManager = GetPopup();
		m_UIPopupManager.SetParameter(24, 3, false);
	}

	protected override void FrInit(int phase)
	{
		switch (phase)
		{
		case 0:
			StartLoading(0, 100);
			break;
		case 1:
			break;
		case 9:
			break;
		case 12:
			break;
		case 3:
			UIConstant.GotoShopAndCustomize(2, 3);
			FrFreeGotoShop();
			Application.LoadLevel("ShopAndCustomize");
			break;
		case 4:
			UIConstant.GotoShopAndCustomize(2, 4);
			FrFreeGotoShop();
			Application.LoadLevel("ShopAndCustomize");
			break;
		case 5:
			break;
		case 10:
			break;
		case 8:
			break;
		case 11:
			break;
		case 2:
		case 6:
		case 7:
			break;
		}
	}

	protected override void FrClose(int phase)
	{
		switch (phase)
		{
		case 0:
			break;
		case 1:
			break;
		case 9:
			break;
		case 12:
			break;
		case 3:
			break;
		case 4:
			break;
		case 5:
			break;
		case 10:
			break;
		case 8:
			break;
		case 11:
			break;
		case 2:
		case 6:
		case 7:
			break;
		}
	}

	protected override void FrUpdate(int phase)
	{
		if (UpdateNetwork())
		{
			return;
		}
		TimeManager.GetInstance().Loop();
		switch (phase)
		{
		case 0:
			FrGoToPhase(9, false, false, false);
			break;
		case 1:
			if (FrGetPreviousPhase() == 0)
			{
				if (LoadingState == LOADING_SUBSTATE_RES)
				{
					LoadingState = LOADING_SUBSTATE_AVATAR;
				}
				else if (LoadingState == LOADING_SUBSTATE_AVATAR)
				{
					FrGoToPhase(FrGetPreviousPhase(), true, false, false);
				}
			}
			break;
		case 9:
			break;
		case 12:
			break;
		case 3:
			break;
		case 4:
			break;
		case 5:
			break;
		case 10:
			break;
		case 8:
			break;
		case 11:
			break;
		case 2:
		case 6:
		case 7:
			break;
		}
	}

	private bool UpdateNetwork()
	{
		networkMgr = GameApp.GetInstance().GetNetworkManager();
		if (!mIsLogout && networkMgr != null)
		{
			if (networkMgr.IsDisconnected || !networkMgr.IsConnected())
			{
				if (!UIMsgBox.instance.IsMessageShow())
				{
					if (networkMgr.IsTimeout)
					{
						UIMsgBox.instance.ShowNetworkTimeoutMessage(UIMsgBox.instance);
					}
					else if (networkMgr.exceptionMessage == null || networkMgr.exceptionMessage == string.Empty)
					{
						UIMsgBox.instance.ShowNetworkTimeoutMessage(UIMsgBox.instance);
					}
					else
					{
						SwHandlerExceptionRequest request = new SwHandlerExceptionRequest(GameApp.GetInstance().GetUserState().GetOnlineUserId(), networkMgr.exceptionMessage);
						GameApp.GetInstance().GetWWWManager().SendRequest(request);
						UIMsgBox.instance.ShowException(UIMsgBox.instance);
					}
				}
				return true;
			}
			if (networkMgr.IsDisconnected || networkMgr.IsDisplayErrorBox)
			{
				return true;
			}
			networkMgr.ProcessReceivedPackets();
			return false;
		}
		return false;
	}

	public void OnGUI()
	{
	}

	public void StartGame(int mapId)
	{
		GameObject gameObject = GameObject.Find(AudioName.MENU_MUSIC_NAME);
		if (gameObject != null)
		{
			Object.DestroyObject(gameObject);
		}
		ShowLoadingPageByMultiMode(GameApp.GetInstance().GetGameMode().ModePlay);
		if (GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			string levelName = GameApp.GetInstance().GetUserState().GetVSLevelConfig(0, mapId)
				.levelName;
			GameWorld.LoadLevel(levelName, ELevelType.GAME);
			return;
		}
		if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Boss)
		{
			SwVoidDataManager.TryGetRandomDropFromOnline(GameApp.GetInstance().GetUserState().GetSelectedDifficultMode(), (byte)mapId, (byte)Global.BOSS_SUB_STAGE_ID, true);
		}
		else
		{
			SwVoidDataManager.TryGetRandomDropFromOnline(GameApp.GetInstance().GetUserState().GetSelectedDifficultMode(), (byte)mapId, (byte)Global.SURVIVAL_SUB_STAGE_ID, true);
		}
		string levelName2 = GameApp.GetInstance().GetUserState().GetLevelConfig(mapId, 0)
			.levelName;
		GameWorld.LoadLevel(levelName2, ELevelType.GAME);
	}

	public static void StartGameInGame(int mapId)
	{
		if (GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			ShowLoadingPageByMultiMode(GameApp.GetInstance().GetGameMode().ModePlay);
			string levelName = GameApp.GetInstance().GetUserState().GetVSLevelConfig(0, mapId)
				.levelName;
			GameWorld.LoadLevel(levelName, ELevelType.GAME);
		}
	}

	public static void ShowLoadingPageByMultiMode(Mode mode)
	{
		bool flag = false;
		string text = string.Empty;
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.TIP_LIST, DataSheetName.TIP_MISSION);
		for (int i = 0; i < gameDataTable.Count; i++)
		{
			string text2 = gameDataTable.GetString(i, 0);
			Debug.Log("missionData = " + text2);
			switch (mode)
			{
			case Mode.VS_TDM:
				if (text2.Equals("MP.TDM"))
				{
					flag = true;
				}
				break;
			case Mode.VS_FFA:
				if (text2.Equals("MP.FFA"))
				{
					flag = true;
				}
				break;
			case Mode.VS_CTF_TDM:
				if (text2.Equals("MP.CTF"))
				{
					flag = true;
				}
				break;
			case Mode.VS_KOTH:
				if (text2.Equals("MP.KOTH"))
				{
					flag = true;
				}
				break;
			case Mode.Domination:
				if (text2.Equals("MP.DOM"))
				{
					flag = true;
				}
				break;
			case Mode.GIANT_BETRAYAL:
				if (text2.Equals("MP.Boss"))
				{
					flag = true;
				}
				break;
			case Mode.Survival:
				if (text2.Equals("CP.Infinite"))
				{
					flag = true;
				}
				break;
			case Mode.Boss:
				if (text2.Equals("CP.Boss"))
				{
					flag = true;
				}
				break;
			}
			if (flag)
			{
				text = gameDataTable.GetString(i, 1);
				break;
			}
		}
		string randomTip = LoadingUIScript.mInstance.GetRandomTip();
		while (randomTip.Equals(text))
		{
			randomTip = LoadingUIScript.mInstance.GetRandomTip();
		}
		LoadingUIScript.mInstance.StartLoading(text, randomTip);
	}

	public void SetRoomData(Room room)
	{
		Lobby.GetInstance().SetCurrentRoom(room);
		if (VSRoomUIScript.mInstance != null)
		{
			VSRoomUIScript.mInstance.ResetParnterPage();
		}
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
	}

	public void StartLoading(byte mode, int nMax)
	{
		FrGoToPhase(1, false, true, false);
	}

	public override void FrFree()
	{
	}

	public void FrFreeGotoShop()
	{
	}

	public void Logout()
	{
		if (networkMgr != null)
		{
			networkMgr.CloseConnection();
		}
		mIsLogout = true;
	}
}
