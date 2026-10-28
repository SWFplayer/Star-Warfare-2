using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class GameApp
{
	public const byte RECORD_CORRECT = 0;

	public const byte RECORD_ERROR = 1;

	public const byte RECORD_MISMATCH = 2;

	public const byte RECORD_NOT_EXIST = 3;

	protected static GameApp instance;

	protected GameWorld gameWorld;

	protected NetworkManager networkMgr;

	protected NetworkManager networkMgrIAP;

	protected WWWManager mWWWManager;

	protected LevelManager mLevelManager;

	protected GameConfig gameConfig = new GameConfig();

	protected UserState userState = new UserState();

	protected OnlinePlayer mOnlinePlayer = new OnlinePlayer();

	protected GameMode gameMode = new GameMode(NetworkType.Single, Mode.CamPain);

	protected UIStateManager mUIStateManager;

	public float AverageSendPacket;

	public float AverageSendByte;

	public float AverageReceivePacket;

	public float AverageReceiveByte;

	private float gameTime;

	public bool httpRequestSent;

	public int Base64MinLength = 50;

	private int MithrilFromTapjoy;

	public bool BladePad;

	public bool GameStart;

	public bool openfreemithril;

	public bool IsFirtBackground = true;

	public static bool IsVideo;

	public static bool IsVideoReady;

	public bool isAdCodeDownloaded;

	public bool isAdInitFinished;

	public bool isMainMenuLoaded;

	public DeviceOrientation PreviousOrientation { get; set; }

	public bool LogoFirstPop { get; set; }

	public bool AdsPoped { get; set; }

	public string UUID { get; set; }

	public string IDFA { get; set; }

	public string MacAddress { get; set; }

	public string ADSUUID { get; set; }

	public string DataPath { get; set; }

	public byte AppStatus { get; set; }

	public int IapColorPackStatus { get; set; }

	public int FlurryStatus { get; set; }

	public int FlurrySize { get; set; }

	public int SponsorPayStatus { get; set; }

	public int SponsorPaySize { get; set; }

	public int CharBoostStatus { get; set; }

	public int CharBoostSize { get; set; }

	public int TapJoyStatus { get; set; }

	public int TapJoySize { get; set; }

	public int AdcolonyStatus { get; set; }

	public int AdcolonySize { get; set; }

	public int RushStatus { get; set; }

	public int RushSize { get; set; }

	public int MovieStatus { get; set; }

	public int MovieSize { get; set; }

	public int BHStatus { get; set; }

	public int BHSize { get; set; }

	public int AppLovinStatus { get; set; }

	public int AppLovinSize { get; set; }

	public int COAStatus { get; set; }

	public int COASize { get; set; }

	public int YouMiStatus { get; set; }

	public int YouMiSize { get; set; }

	public int NativeXStatus { get; set; }

	public int NativeXSize { get; set; }

	public int NativeXVideoStatus { get; set; }

	public int NativeXVideoSize { get; set; }

	public int YouMiVideoStatus { get; set; }

	public int YouMiVideoSize { get; set; }

	public static GameApp GetInstance()
	{
		if (instance == null)
		{
			instance = new GameApp();
			instance.PreviousOrientation = DeviceOrientation.Portrait;
			instance.LogoFirstPop = true;
			instance.AdsPoped = false;
			Application.targetFrameRate = 60;
			instance.userState.mGraphicQuality.ApplayQuality();
			if (Application.platform == RuntimePlatform.Android && AndroidPluginScript.CheckDevice() == 1)
			{
				Debug.Log("sheild check");
				DebugUI.selectResolution = DebugUI.selectResolution + nvsyscaps.GpuFamily.ToString() + " ";
				if (nvsyscaps.GpuFamily < nvsyscaps.Nv_TegraGpuFamily.TEGRA_GPU_T4T4i)
				{
					List<AndroidResolution> list = new List<AndroidResolution>();
					list.Add(new AndroidResolution(1.7777778f, 1280, 720));
					list.Add(new AndroidResolution(1.3333334f, 1024, 768));
					list.Add(new AndroidResolution(1.5f, 960, 640));
					list.Add(new AndroidResolution(1.6f, 1280, 800));
					List<AndroidResolution> list2 = list;
					float num = (float)Screen.width / (float)Screen.height;
					float num2 = 100f;
					AndroidResolution androidResolution = null;
					foreach (AndroidResolution item in list2)
					{
						float num3 = Math.Abs(num - item.ratio);
						if (num3 < num2 && Screen.width >= item.width && Screen.height >= item.height)
						{
							num2 = num3;
							androidResolution = item;
						}
					}
					if (androidResolution != null)
					{
						Screen.SetResolution(androidResolution.width, androidResolution.height, true);
						string selectResolution = DebugUI.selectResolution;
						DebugUI.selectResolution = selectResolution + androidResolution.width + "X" + androidResolution.height + " Changed!";
					}
					else
					{
						string selectResolution = DebugUI.selectResolution;
						DebugUI.selectResolution = selectResolution + Screen.width + "X" + Screen.height + " Default!";
					}
				}
			}
		}
		return instance;
	}

	public void ShowAdMobBanner()
	{
	}

	public int GetMithrilFromTapjoy()
	{
		return MithrilFromTapjoy;
	}

	public void SetMithrilFromTapjoy(int mithril)
	{
		MithrilFromTapjoy = mithril;
	}

	public bool isGameStart()
	{
		return GameStart;
	}

	public void SetGameStart()
	{
		GameStart = true;
	}

	public bool IsConnectedToInternet()
	{
		bool result = false;
		if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork || Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
		{
			result = true;
		}
		return result;
	}

	public bool IsConnectedToServer()
	{
		if (networkMgr != null && !networkMgr.IsDisconnected && networkMgr.IsConnected())
		{
			return true;
		}
		return false;
	}

	public bool CanShowAds()
	{
		return MainMenu2DScript.mInstance != null || InterMissionScript.GetInstance() != null || (IsInGameScene() && GetGameMode().IsSingle());
	}

	public void SetGameMode(GameMode mode)
	{
		gameMode = mode;
	}

	public GameMode GetGameMode()
	{
		return gameMode;
	}

	public void SetUIStateManager(UIStateManager stateManager)
	{
		mUIStateManager = stateManager;
	}

	public UIStateManager GetUIStateManager()
	{
		return mUIStateManager;
	}

	public GameWorld GetGameWorld()
	{
		return gameWorld;
	}

	public void CreateGameWorld()
	{
		gameWorld = new GameWorld();
		gameWorld.Init();
		Lobby.GetInstance().GetVSClock().Reset();
		if (GetInstance().GetGameMode().IsMultiPlayer())
		{
			if (GetInstance().GetGameMode().ModePlay == Mode.BOSS_FINAL)
			{
				ChangeToBossSceneRequest request = new ChangeToBossSceneRequest();
				networkMgr.SendRequest(request);
				List<RemotePlayer> remotePlayerList = GlobalData.GetInstance().RemotePlayerList;
				if (remotePlayerList != null)
				{
					int num = 0;
					foreach (RemotePlayer item in remotePlayerList)
					{
						if (item != null && !item.HasLeavedGame())
						{
							num++;
						}
					}
					gameWorld.PlayerCountForBoss = num + 1;
				}
			}
			else
			{
				GetSceneStateRequest request2 = new GetSceneStateRequest();
				networkMgr.SendRequest(request2);
			}
		}
		else
		{
			gameWorld.GetLocalPlayer().SetSeatID(0);
			gameWorld.GetLocalPlayer().DropAtSpawnPosition();
			if (GlobalData.GetInstance().Partner != null)
			{
				Dictionary<int, int> friendRemainDictionary = GetInstance().GetUserState().GetFriendRemainDictionary();
				if (friendRemainDictionary[GlobalData.GetInstance().Partner.GetUserId()] > 0)
				{
					Debug.Log("Friend Name = " + GlobalData.GetInstance().Partner.GetName());
					gameWorld.CreateRobot(GlobalData.GetInstance().Partner);
					if (MissionManager.GetInstance().GetCurrentStageMission(0).Type != MissionType.BOSS)
					{
						Dictionary<int, int> dictionary2;
						Dictionary<int, int> dictionary = (dictionary2 = friendRemainDictionary);
						int userId;
						int key = (userId = GlobalData.GetInstance().Partner.GetUserId());
						userId = dictionary2[userId];
						dictionary[key] = userId - 1;
					}
					Save();
				}
			}
			if (GetInstance().GetGameMode().ModePlay != Mode.BOSS_FINAL)
			{
				GetInstance().GetUserState().LoadCurrentCustomize(true, 0);
				gameWorld.GetLocalPlayer().RecalculateMaxHp();
			}
		}
		if (GetInstance().GetGameMode().ModePlay == Mode.BOSS_FINAL)
		{
			AudioManager.GetInstance().PlayMusic(AudioName.BGM_BOSS);
		}
		else
		{
			AudioManager.GetInstance().PlayMusic(AudioName.BGM_NORMAL_STAGE);
		}
	}

	public void CreateGameAppScript()
	{
		GameObject gameObject = GameObject.Find(Global.GAME_APP_SCRIPT_OBJECT_NAME);
		if (null == gameObject)
		{
			gameObject = new GameObject(Global.GAME_APP_SCRIPT_OBJECT_NAME);
		}
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		GameAppScript component = gameObject.GetComponent<GameAppScript>();
		if (null == component)
		{
			gameObject.AddComponent<GameAppScript>();
		}
	}

	public void StartHttpRequestThread()
	{
		if (IsConnectedToInternet() && !httpRequestSent)
		{
			HttpRequestThread httpRequestThread = new HttpRequestThread();
			Thread thread = new Thread(httpRequestThread.DoWork);
			thread.Start();
			httpRequestSent = true;
		}
	}

	public void ClearGameWorld()
	{
		gameWorld = null;
	}

	public void EnterBossLevel()
	{
	}

	public void LoadConfig()
	{
		if (gameConfig.monsterConfTable.Count == 0)
		{
			if (Application.isEditor || Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
			{
				gameConfig.LoadFromXML(null);
			}
			else
			{
				gameConfig.LoadFromXML("/");
			}
		}
	}

	public NetworkManager CreateNetwork()
	{
		networkMgr = new NetworkManager();
		networkMgr.StartNetwork(networkMgr.strIP, networkMgr.port);
		return networkMgr;
	}

	public NetworkManager CreateNetwork(string strIP, int port)
	{
		networkMgr = new NetworkManager();
		networkMgr.StartNetwork(strIP, port);
		return networkMgr;
	}

	public void DestoryNetWork()
	{
		if (networkMgr != null)
		{
			networkMgr.CloseConnection();
			networkMgr = null;
		}
	}

	public NetworkManager GetNetworkManager()
	{
		return networkMgr;
	}

	public NetworkManager CreateNetworkIAP(string strIP, int port)
	{
		networkMgrIAP = new NetworkManager();
		networkMgrIAP.StartNetwork(strIP, port);
		return networkMgrIAP;
	}

	public void DestoryNetWorkIAP()
	{
		if (networkMgrIAP != null)
		{
			networkMgrIAP.CloseConnection();
			networkMgrIAP = null;
		}
	}

	public NetworkManager GetNetworkManagerIAP()
	{
		return networkMgrIAP;
	}

	public WWWManager GetWWWManager()
	{
		if (mWWWManager == null)
		{
			mWWWManager = new WWWManager();
		}
		return mWWWManager;
	}

	public LevelManager GetLevelManager()
	{
		if (mLevelManager == null)
		{
			mLevelManager = new LevelManager();
		}
		return mLevelManager;
	}

	public UserState GetUserState()
	{
		return userState;
	}

	public OnlinePlayer GetOnlinePlayer()
	{
		return mOnlinePlayer;
	}

	public void AccumulateTotalPlayTime(float deltaTime)
	{
		gameTime += deltaTime;
		if (gameTime > 1f && userState != null)
		{
			userState.AddTotalPlayTime(Mathf.FloorToInt(gameTime));
			gameTime -= Mathf.FloorToInt(gameTime);
		}
	}

	public void Loop(float deltaTime)
	{
		AccumulateTotalPlayTime(deltaTime);
		if (userState != null)
		{
			userState.TrialWeaponBuySecondsCountDown(deltaTime);
		}
       
    }

	public void LateLoop(float deltaTime)
	{
	}

	public GameConfig GetGameConfig()
	{
		return gameConfig;
	}

	public void Save()
	{
		if (!userState.bInit)
		{
			Debug.Log("no save");
			return;
		}
		string text = Application.persistentDataPath + "/Documents/";
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXEditor)
		{
			text = Application.dataPath + "/../../Documents/";
		}
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		userState.SaveData(binaryWriter);
		byte[] data = memoryStream.ToArray();
		memoryStream.Close();
		binaryWriter.Close();
		data = userState.CryptBuffer(data);
		Stream stream = File.Open(text + "SW2_Map_01", FileMode.Create);
		stream.Write(data, 0, data.Length);
		stream.Flush();
		stream.Close();
		byte[] array = new byte[data.Length];
		data.CopyTo(array, 0);
		array = userState.CryptMD5Buffer(array);
		Stream stream2 = File.Open(text + "SW2_Map_02", FileMode.Create);
		stream2.Write(array, 0, array.Length);
		stream2.Flush();
		stream2.Close();
	}

	public byte Load()
	{
		string text = Application.persistentDataPath + "/Documents/";
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXEditor)
		{
			text = Application.dataPath + "/../../Documents/";
		}
		if (File.Exists(text + "SW2_Map_01") && File.Exists(text + "SW2_Map_02"))
		{
			Stream stream = File.Open(text + "SW2_Map_01", FileMode.Open);
			Stream stream2 = File.Open(text + "SW2_Map_02", FileMode.Open);
			byte[] bytesFromStream = userState.GetBytesFromStream(stream);
			byte[] bytesFromStream2 = userState.GetBytesFromStream(stream2);
			if (!userState.VerifyMD5(bytesFromStream, bytesFromStream2))
			{
				return 2;
			}
			byte[] buffer = userState.DecryptBuffer(bytesFromStream);
			stream.Close();
			stream2.Close();
			MemoryStream memoryStream = new MemoryStream(buffer);
			BinaryReader binaryReader = new BinaryReader(memoryStream);
			string version = string.Empty;
			try
			{
				userState.LoadData(binaryReader, out version);
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Version: " + version);
				Debug.LogWarning(ex.StackTrace);
				if (!File.Exists(text + "SW2_Map_01_" + version))
				{
					File.Copy(text + "SW2_Map_01", text + "SW2_Map_01_" + version);
				}
				if (!File.Exists(text + "SW2_Map_02_" + version))
				{
					File.Copy(text + "SW2_Map_02", text + "SW2_Map_02_" + version);
				}
				return 1;
			}
			finally
			{
				binaryReader.Close();
				memoryStream.Close();
			}
			if (MainMenu2DScript.mInstance != null)
			{
				MainMenu2DScript.mInstance.InitLanguage();
			}
			if (!version.Equals("120"))
			{
				switch (version)
				{
				case "100":
				case "101":
				case "102":
				case "103":
					userState.v100To104MithrilHandler();
					if (MainMenu2DScript.mInstance != null)
					{
						MainMenu2DScript.mInstance.ShowMithrilHandleMessage();
					}
					break;
				}
				int result = 0;
				int.TryParse(userState.readVersion, out result);
				if (result != 0 && result <= 113)
				{
					GetInstance().GetUserState().SmoothMoveHandler();
				}
				userState.UnlockEquipsByLevel();
				if (userState.GetUnlockPackBought())
				{
					userState.UnlockAllLevel_Weapons_Equips();
				}
				Save();
			}
			return 0;
		}
		return 3;
	}

	public void LoadGameDataIllegalWay()
	{
		GameDataManager.GetInstance().Init();
		if (GetInstance().Load() != 0)
		{
			userState.Init();
		}
		userState.InitLevelName();
		userState.InitRank();
		userState.bInit = true;
		GetInstance().StartHttpRequestThread();
		userState.SetSelectedStageIndex(0);
		userState.SetSelectedSubStageIndex(0);
		GameObject gameObject = GameObject.Find("__MissionTest");
		if (gameObject != null && gameObject.GetComponent<MissionIllegalLoad>() != null)
		{
			MissionIllegalLoad component = gameObject.GetComponent<MissionIllegalLoad>();
			userState.SetSelectedStageIndex((byte)(component.StageId - 1));
			userState.SetSelectedSubStageIndex(component.SubStageId);
			if (component.StageId == 0)
			{
				userState.SetSelectedStageIndex(0);
				userState.SetSelectedSubStageIndex(0);
			}
			if (component.SubStageId == 7)
			{
				GetInstance().GetGameMode().ModePlay = Mode.BOSS_FINAL;
			}
		}
		LocalizationManager.GetInstance().currentLanguage = GetInstance().GetUserState().GetUserLanguage();
		Debug.Log("The current language is " + GetInstance().GetUserState().GetUserLanguage());
		GameObject gameObject2 = GameObject.Find("_EffectRoot");
		if (gameObject2 == null)
		{
			gameObject2 = new GameObject("_EffectRoot");
			EffectPool effectPool = gameObject2.AddComponent<EffectPool>();
			effectPool.Start();
			UnityEngine.Object.DontDestroyOnLoad(gameObject2);
		}
		MissionManager.GetInstance().Init();
	}

	public static void IphoneGenerationModify()
	{
	}

	public bool IsInGameScene()
	{
		GameObject gameObject = GameObject.Find("Game");
		if (null == gameObject)
		{
			return false;
		}
		MainScript component = gameObject.GetComponent<MainScript>();
		if (null == component)
		{
			return false;
		}
		return true;
	}
}
