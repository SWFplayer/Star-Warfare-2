using System;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScript : MonoBehaviour, UIMsgListener
{
	public static LobbyScript mInstance;

	public Camera SceneCamera;

	public Transform CameraStartPosition;

	public VSSelectionScript PVE;

	public VSSelectionScript PVP;

	public GameObject NeedHideObj;

	public GameObject InfoButton;

	public GameObject CustomizeButton;

	public GameObject ShopButton;

	public GameObject DailyBonusButton;

	public TweenAlpha DailyBonusAlpha;

	public Transform DailyBonusButtonTransfrom;

	public UISprite InfoClicked;

	public UISprite CustomizeClicked;

	public UISprite ShopClicked;

	public GameObject ExitButton;

	public GameObject FriendNotifyButton;

	public PvpExpIapUI pvpExpIap;

	public UILabel ServerLabal;

	public UISprite ServerBackground;

	public bool IsShowExtraGoldAndMithril;

	public GameObject MaterialChestOkButton;

	public GameObject MaterialChestObj;

	public GameObject MaterialTemplate;

	public GameObject MaterialPanel;

	[HideInInspector]
	public List<UIDailyRewardInfo> materialChestInfo = new List<UIDailyRewardInfo>();

	protected ShopUIScript ShopUI;

	private void Awake()
	{
		mInstance = this;
		UIEventListener uIEventListener = UIEventListener.Get(InfoButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(CustomizeButton);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(ShopButton);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener4 = UIEventListener.Get(ExitButton);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener5 = UIEventListener.Get(DailyBonusButton);
		uIEventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener5.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener6 = UIEventListener.Get(FriendNotifyButton);
		uIEventListener6.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener6.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener7 = UIEventListener.Get(MaterialChestOkButton);
		uIEventListener7.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener7.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener8 = UIEventListener.Get(InfoButton);
		uIEventListener8.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener8.onPress, new UIEventListener.BoolDelegate(OnButtonPress));
		UIEventListener uIEventListener9 = UIEventListener.Get(CustomizeButton);
		uIEventListener9.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener9.onPress, new UIEventListener.BoolDelegate(OnButtonPress));
		UIEventListener uIEventListener10 = UIEventListener.Get(ShopButton);
		uIEventListener10.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener10.onPress, new UIEventListener.BoolDelegate(OnButtonPress));
		UIEventListener uIEventListener11 = UIEventListener.Get(ExitButton);
		uIEventListener11.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener11.onPress, new UIEventListener.BoolDelegate(OnButtonPress));
		if (Lobby.GetInstance().NeedToTurnRegister)
		{
			Lobby.GetInstance().NeedToTurnRegister = false;
			GameObject original = Resources.Load("UI/Menu/OptionUI/Prefab/OptionUI") as GameObject;
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original);
			gameObject.GetComponent<OptionUIScript>().SetIsInMainMenu(true);
			LoginPageScript.mTurnToRegister = true;
		}
		UIAtlasManager.UpdateMatOutOfGame();
	}

	private void Start()
	{
		TweenTransform.Begin(SceneCamera.gameObject, 0.2f, CameraStartPosition);
		if (UnlockListPageScript.IsGoToCustomize)
		{
			OnButtonClick(CustomizeButton);
		}
		UserState userState = GameApp.GetInstance().GetUserState();
		if (userState.GetCompensateMaterial())
		{
			if (userState.GetCompensateMaterialList() == null)
			{
				CompensateMaterialRequest request = new CompensateMaterialRequest();
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			else
			{
				ShowCompensateMaterial(userState.GetCompensateMaterialList());
			}
		}
		if (userState.NeedShowExtraGoldAndMithril())
		{
			IsShowExtraGoldAndMithril = true;
			UIMsgBox.instance.ShowExtraGoldAndMithrilMessage(this, userState.GetBonusMithril(), userState.GetCompensateMithril(), userState.GetBonusGold(), userState.GetCompensateGold(), userState.GetCompensateExp());
			userState.ApplyExtraGoldAndMithril();
		}
		else
		{
			IsShowExtraGoldAndMithril = false;
			CheckDailyBonus();
		}
		if (IapScript.mInstance == null)
		{
			GameObject original = Resources.Load("UI/Menu/IAP/Prefab/IapUI") as GameObject;
			UnityEngine.Object.Instantiate(original, new Vector3(0f, -1000f, 0f), Quaternion.identity);
		}
		IapScript.mInstance.gameObject.SetActive(false);
		SetServerInfo();
		CheckFriendNotify();
	}

	private void SetServerInfo()
	{
		switch (Lobby.GetInstance().CurrentServerType)
		{
		case ServerType.USA_ANDROID:
			ServerLabal.text = LocalizationManager.GetInstance().GetString("TIPS_UPDATE11_9");
			break;
		case ServerType.ASIA_IOS:
			ServerLabal.text = LocalizationManager.GetInstance().GetString("TIPS_UPDATE11_10");
			break;
		case ServerType.ASIA_ANDROID:
			ServerLabal.text = LocalizationManager.GetInstance().GetString("TIPS_UPDATE11_11");
			break;
		default:
			ServerLabal.text = LocalizationManager.GetInstance().GetString("TIPS_UPDATE11_8");
			break;
		}
		ServerBackground.width = ServerLabal.width + 200;
	}

	private void CheckDailyBonus()
	{
		if (!DailyBonusManager.GetInstance().GetHasRecievedReward() || DailyBonusManager.GetInstance().HasNewAnnouncement)
		{
			DailyBonusAlpha.enabled = true;
			if (DailyBonusManager.GetInstance().HasNewAnnouncement)
			{
				DailyBonusManager.GetInstance().AutoShowInUI(UIDailyBonus.EDailyBonusState.ANNOUNCEMENT);
			}
			else if (GameApp.GetInstance().GetUserState().DontMindIfRegisted())
			{
				DailyBonusManager.GetInstance().AutoShowInUI(UIDailyBonus.EDailyBonusState.LOGIN);
			}
		}
		else
		{
			DailyBonusAlpha.enabled = false;
		}
	}

	public void ShowCompensateMaterial(Dictionary<short, short> materialList)
	{
		if (materialList != null && materialList.Count > 0)
		{
			ShowRewardInfo(materialList);
		}
		GameApp.GetInstance().GetUserState().SetCompensateMaterialList(null);
		GameApp.GetInstance().GetUserState().SetCompensateMaterial(false);
	}

	private void OnDestroy()
	{
		UnlockListPageScript.IsGoToCustomize = false;
		mInstance = null;
		Resources.UnloadAsset(UIAtlasManager.Mat_OutOfGame);
	}

    private void LoadLobbyOrMainMenu()
    {
        Application.LoadLevel("MainMenu");
    }

    public void OnButtonClick(GameObject go)
	{
		if (go == ExitButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			if (ShopUI != null && ShopUI.gameObject.activeSelf)
			{
				ShopUI.gameObject.SetActive(false);
				ShowLobbyPage();
				UploadStoreDataRequest request = new UploadStoreDataRequest(GameApp.GetInstance().GetUserState());
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				return;
			}
			if (MultiMenuScript.mInstance != null)
			{
				MultiMenuScript.mInstance.Logout();
			}
			GameObject gameObject = GameObject.Find(AudioName.MENU_MUSIC_NAME);
			if (gameObject != null)
			{
				UnityEngine.Object.DestroyObject(gameObject);
			}

            #if UNITY_EDITOR

                        Debug.Log("Editor, No Need To Restart The Game");
                        LoadLobbyOrMainMenu();
            #else
                
                            if (Application.platform == RuntimePlatform.WindowsPlayer)
                            {
                                Debug.Log("Windows Build Freeze Bug Prevention");
            
                                GameApp.GetInstance().Save(); 

                                try
                                {
                       
                                    string exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            
                                    System.Diagnostics.Process.Start(exePath);
                                }
                                catch (System.Exception ex)
                                {
                                    Debug.LogError("Failed to restart game: " + ex.Message);
                                }
            
                                Application.Quit();
                            }
                            else
                            {
                    
                                Debug.Log("Not On Windows Build");
                                LoadLobbyOrMainMenu();
                            }
            #endif
            
		}
		else if (go == CustomizeButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (GameApp.GetInstance().GetUserState().DontMindIfRegisted())
			{
				HideLobbyUI();
				if (!LoadoutUIScript.Instance)
				{
					GameObject original = Resources.Load("UI/Menu/Loadout/Prefab/LoadoutUI") as GameObject;
					UnityEngine.Object.Instantiate(original);
				}
				LoadoutUIScript.Instance.ShowLoadoutUI();
			}
			else
			{
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_5"), 8, 23);
			}
		}
		else if (go == InfoButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (GameApp.GetInstance().GetUserState().DontMindIfRegisted())
			{
				HideLobbyUI();
				GameObject original2 = Resources.Load("UI/Menu/PlayerInfo/Prefab/PlayerInfoUI") as GameObject;
				GameObject gameObject2 = UnityEngine.Object.Instantiate(original2) as GameObject;
				PlayerInfoUI component = gameObject2.GetComponent<PlayerInfoUI>();
				component.InitByUserState(GameApp.GetInstance().GetUserState(), true);
			}
			else
			{
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_8"), 8, 24);
			}
		}
		else if (go == DailyBonusButton)
		{
			if (ShopUI != null && ShopUI.gameObject.activeSelf)
			{
				ShowLobbyPage();
				ShopUI.gameObject.SetActive(false);
			}
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			OpenDailyBonusPage();
		}
		else if (go == ShopButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			HideLobbyPage();
			if (ShopUI == null)
			{
				GameObject original3 = Resources.Load("UI/Menu/Shop/Prefab/ShopUI") as GameObject;
				GameObject gameObject3 = UnityEngine.Object.Instantiate(original3) as GameObject;
				ShopUI = gameObject3.GetComponent<ShopUIScript>();
			}
			else
			{
				ShopUI.gameObject.SetActive(true);
			}
		}
		else if (go == FriendNotifyButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (GameApp.GetInstance().GetUserState().DontMindIfRegisted())
			{
				HideLobbyUI();
				GameObject original4 = Resources.Load("UI/Menu/PlayerInfo/Prefab/PlayerInfoUI") as GameObject;
				GameObject gameObject4 = UnityEngine.Object.Instantiate(original4) as GameObject;
				PlayerInfoUI component2 = gameObject4.GetComponent<PlayerInfoUI>();
				component2.InitByUserState(GameApp.GetInstance().GetUserState(), true);
				GameObject original5 = Resources.Load("UI/Menu/FriendUI/Prefab/FriendUI") as GameObject;
				GameObject gameObject5 = UnityEngine.Object.Instantiate(original5) as GameObject;
				FriendUI component3 = gameObject5.GetComponent<FriendUI>();
				component3.SetOpenFromNotify();
			}
		}
		else if (go == MaterialChestOkButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			MaterialChestObj.SetActive(false);
		}
	}

	public void OnButtonPress(GameObject go, bool isPress)
	{
		if (go == InfoButton)
		{
			if (isPress)
			{
				InfoClicked.gameObject.SetActive(true);
			}
			else
			{
				InfoClicked.gameObject.SetActive(false);
			}
		}
		else if (go == CustomizeButton)
		{
			if (isPress)
			{
				CustomizeClicked.gameObject.SetActive(true);
			}
			else
			{
				CustomizeClicked.gameObject.SetActive(false);
			}
		}
		else if (go == ShopButton)
		{
			if (isPress)
			{
				ShopClicked.gameObject.SetActive(true);
			}
			else
			{
				ShopClicked.gameObject.SetActive(false);
			}
		}
	}

	public void ShowLobbyPage()
	{
		NeedHideObj.SetActive(true);
		PVE.gameObject.SetActive(true);
		PVP.gameObject.SetActive(true);
		CheckFriendNotify();
	}

	public void HideLobbyPage()
	{
		NeedHideObj.SetActive(false);
		PVE.gameObject.SetActive(false);
		PVP.gameObject.SetActive(false);
	}

	public void ShowLobbyUI()
	{
		base.gameObject.SetActive(true);
		CheckFriendNotify();
	}

	public void HideLobbyUI()
	{
		base.gameObject.SetActive(false);
	}

	public void StartGame()
	{
		if (GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			if (Lobby.GetInstance().IsMasterPlayer)
			{
				StartGameRequest request = new StartGameRequest();
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			else
			{
				PlayerJoinTeamStartGameRequest request2 = new PlayerJoinTeamStartGameRequest();
				GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
			}
		}
		else
		{
			PlayerJoinTeamStartGameRequest request3 = new PlayerJoinTeamStartGameRequest();
			GameApp.GetInstance().GetNetworkManager().SendRequest(request3);
		}
		MultiMenuScript.ShowLoadingPageByMultiMode(GameApp.GetInstance().GetGameMode().ModePlay);
	}

	public void OnRegistComplete()
	{
		UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_29"), 1, 21);
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (buttonId == UIMsg.UIMsgButton.Ok)
		{
			switch (whichMsg.EventId)
			{
			case 13:
				if (VSRoomUIScript.mInstance != null && VSRoomUIScript.mInstance.gameObject.activeSelf)
				{
					VSRoomUIScript.mInstance.RefreshRoomList();
				}
				break;
			case 20:
				UIMsgBox.instance.CloseMessage();
				VSRoomUIScript.mInstance.ChangePage(RoomPage.ListPage);
				break;
			}
		}
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
			}
		}
		else if (whichMsg.EventId == 21)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				UIMsgBox.instance.CloseMessage();
				if (LoginPageScript.mInstance != null)
				{
					LoginPageScript.mInstance.Refresh();
				}
				GameApp.GetInstance().GetUserState().Achievement.CheckAchievement_Register();
			}
		}
		else if (whichMsg.EventId == 23)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				GameObject original = Resources.Load("UI/Menu/OptionUI/Prefab/OptionUI") as GameObject;
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original);
				gameObject.GetComponent<OptionUIScript>().SetIsInMainMenu(true);
				LoginPageScript.mTurnToRegister = true;
				UIMsgBox.instance.CloseMessage();
				return;
			}
			UIMsgBox.instance.CloseMessage();
			HideLobbyUI();
			if (!LoadoutUIScript.Instance)
			{
				GameObject original2 = Resources.Load("UI/Menu/Loadout/Prefab/LoadoutUI") as GameObject;
				UnityEngine.Object.Instantiate(original2);
			}
			LoadoutUIScript.Instance.ShowLoadoutUI();
		}
		else if (whichMsg.EventId == 24)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				GameObject original3 = Resources.Load("UI/Menu/OptionUI/Prefab/OptionUI") as GameObject;
				GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(original3);
				gameObject2.GetComponent<OptionUIScript>().SetIsInMainMenu(true);
				LoginPageScript.mTurnToRegister = true;
				UIMsgBox.instance.CloseMessage();
			}
			else
			{
				UIMsgBox.instance.CloseMessage();
			}
		}
		else if (whichMsg.EventId == 90)
		{
			if (buttonId != UIMsg.UIMsgButton.Ok)
			{
				UIMsgBox.instance.CloseMessage();
				if (GameApp.GetInstance().GetGameMode().IsVsMode(GlobalData.CreateRoomData.mGameMode))
				{
					PVP.ListGames();
					return;
				}
				CreateRoomRequest request = new CreateRoomRequest(Lobby.GetInstance().GetUserName(), 0, 3, false, GlobalData.CreateRoomData.mGameMode, GlobalData.CreateRoomData.mMapID, GameApp.GetInstance().GetUserState().GetRank()
					.rankID, TimeManager.GetInstance().Ping, 0, 99, GlobalData.CreateRoomData.mDiffMode);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			else
			{
				if (GameApp.GetInstance().GetGameMode().IsVsMode(GlobalData.CreateRoomData.mGameMode))
				{
					CreateVSRoomRequest request2 = new CreateVSRoomRequest(Lobby.GetInstance().GetUserName(), 0, 8, false, GlobalData.CreateRoomData.mGameMode, GlobalData.CreateRoomData.mMapID, GameApp.GetInstance().GetUserState().GetRank()
						.rankID, TimeManager.GetInstance().Ping, (byte)Global.PVP_LEVEL_LIMIT_SELECTIONS[0], (byte)Global.PVP_LEVEL_LIMIT_SELECTIONS[Global.PVP_LEVEL_LIMIT_SELECTIONS.Count - 1], GlobalData.CreateRoomData.mWinCondition, GlobalData.CreateRoomData.mWinValue, (GlobalData.CreateRoomData.mGameMode != 13) ? true : false);
					GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
				}
				else
				{
					PVE.ListGames();
				}
				UIMsgBox.instance.CloseMessage();
			}
		}
		else if (whichMsg.EventId == 28 && buttonId == UIMsg.UIMsgButton.Ok)
		{
			UIMsgBox.instance.CloseMessage();
			IsShowExtraGoldAndMithril = false;
			CheckDailyBonus();
		}
	}

	public void CheckFriendNotify()
	{
		if (GameApp.GetInstance().GetUserState().GetRequestList()
			.Count > 0)
		{
			FriendNotifyButton.SetActive(true);
		}
		else
		{
			FriendNotifyButton.SetActive(false);
		}
	}

	public void OpenDailyBonusPage()
	{
		if (UIDailyBonus.mInstance == null)
		{
			GameObject original = Resources.Load("UI/Menu/DailyBonus/Prefab/DailyBonusUI") as GameObject;
			GameObject gameObject = UnityEngine.Object.Instantiate(original, DailyBonusButtonTransfrom.position, DailyBonusButtonTransfrom.rotation) as GameObject;
		}
		else
		{
			UIDailyBonus.mInstance.gameObject.SetActive(true);
		}
	}

	public void ShowRewardInfo(Dictionary<short, short> materialList)
	{
		MaterialChestObj.SetActive(true);
		MaterialPanel.transform.localPosition = new Vector3(-300f, 100f, 0f);
		if (materialChestInfo != null)
		{
			foreach (UIDailyRewardInfo item in materialChestInfo)
			{
				item.gameObject.SetActive(false);
				UnityEngine.Object.DestroyImmediate(item.gameObject);
			}
		}
		materialChestInfo = new List<UIDailyRewardInfo>();
		int count = materialList.Count;
		int num = 0;
		int num2 = 150;
		int num3 = 150;
		foreach (KeyValuePair<short, short> material in materialList)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(MaterialTemplate) as GameObject;
			gameObject.SetActive(true);
			gameObject.transform.parent = MaterialPanel.transform;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			UIDailyRewardInfo component = gameObject.GetComponent<UIDailyRewardInfo>();
			component.Init(3, material.Key, material.Value);
			materialChestInfo.Add(component);
		}
		MaterialPanel.GetComponent<UIGrid>().Reposition();
		MaterialPanel.GetComponent<UIPanel>().clipRange = new Vector4(300f, -100f, 750f, 350f);
	}
}
