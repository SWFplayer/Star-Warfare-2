using UnityEngine;

public class InGameUIScript : UIStateManager, UIHandler, UIMsgListener
{
	public static bool bInited;

	public NetworkManager networkMgr;

	private int dataIndex;

	private byte LoadingState;

	private static byte LOADING_SUBSTATE_RES;

	private static byte LOADING_SUBSTATE_MAINSCRIPT = 1;

	private static byte LOADING_SUBSTATE_START = 2;

	private void Awake()
	{
		bInited = false;
	}

	private void Start()
	{
		InitUIManager();
		LoadingState = LOADING_SUBSTATE_RES;
		GameApp.GetInstance().SetUIStateManager(this);
		FrGoToPhase(0, false, true, true);
	}

	private void InitUIManager()
	{
		m_UIManager = base.gameObject.AddComponent<UIManager>() as UIManager;
		m_UIManager.SetParameter(24, 3, false);
		m_UIManager.SetUIHandler(this);
		m_UIPopupManager = GetPopup();
	}

	protected override void FrInit(int phase)
	{
		switch (phase)
		{
		case 0:
			StartLoading(0, 100);
			SetIPadScreen();
			break;
		case 6:
			SetIPadScreen();
			SetIPhone5Screen();
			if (GameUIManager.GetInstance() == null)
			{
				GameObject original4 = Resources.Load("UI/GameUIManager") as GameObject;
				GameObject gameObject = Object.Instantiate(original4) as GameObject;
			}
			GameUIManager.GetInstance().LoadUI(GameUIStatus.UIRader);
			GameUIManager.GetInstance().LoadHUD(HUDBase.HUDType.Battle);
			break;
		case 7:
		{
			GameObject original3 = Resources.Load("UI/Menu/Result/Prefab/ResultPage") as GameObject;
			Object.Instantiate(original3);
			break;
		}
		case 16:
			if (GameApp.GetInstance().GetGameMode().IsCoopMode())
			{
				GameObject original = Resources.Load("UI/Menu/Result/Prefab/ResultPageCoop") as GameObject;
				Object.Instantiate(original);
			}
			else if (GameApp.GetInstance().GetGameMode().IsVSMode())
			{
				GameObject original2 = Resources.Load("UI/Menu/Result/Prefab/ResultPagePVP") as GameObject;
				Object.Instantiate(original2);
			}
			break;
		}
	}

	protected override void FrClose(int phase)
	{
		switch (phase)
		{
		case 0:
			base.transform.position = Vector3.up * 10000f;
			break;
		case 15:
			if (FrGetNextPhase() == 6)
			{
				GotoBattleUI();
			}
			break;
		}
	}

	protected override void FrUpdate(int phase)
	{
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && phase != 16 && UpdateNetwork())
		{
			return;
		}
		switch (phase)
		{
		case 0:
			FrGoToPhase(6, false, false, false);
			break;
		case 1:
			if (FrGetPreviousPhase() != 0)
			{
				break;
			}
			if (LoadingState == LOADING_SUBSTATE_RES)
			{
				LoadingState = LOADING_SUBSTATE_MAINSCRIPT;
			}
			else if (LoadingState == LOADING_SUBSTATE_MAINSCRIPT)
			{
				MainScript component = GameObject.Find("Game").GetComponent<MainScript>();
				component.Init();
				LoadingState = LOADING_SUBSTATE_START;
			}
			else if (LoadingState == LOADING_SUBSTATE_START)
			{
				if (!LoadingUIScript.mInstance.gameObject.activeSelf)
				{
					bInited = true;
					FadeAnimationScript.GetInstance().FadeOutBlack();
					FrGoToPhase(FrGetPreviousPhase(), true, false, false);
				}
				else if (!GameApp.GetInstance().GetLevelManager().IsLoadingFinished())
				{
					GameApp.GetInstance().GetLevelManager().OnLoadingFinished();
				}
			}
			break;
		case 6:
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				if (GameApp.GetInstance().GetGameWorld().State == GameState.GameOverUIWin)
				{
					if (GameApp.GetInstance().GetGameMode().IsCoopMode())
					{
						LeaveRoomRequest request = new LeaveRoomRequest();
						GameApp.GetInstance().GetNetworkManager().SendRequest(request);
						Lobby.GetInstance().IsMasterPlayer = false;
						Lobby.GetInstance().SetCurrentRoomID(-1);
						if (GameApp.GetInstance().GetGameWorld() != null)
						{
							GameApp.GetInstance().GetGameWorld().Exit = true;
						}
					}
					if (GameApp.GetInstance().GetGameWorld() != null)
					{
						GameApp.GetInstance().GetGameWorld().SubmitBattleScores();
					}
					FrGoToPhase(16, false, false, false);
				}
				else
				{
					if (GameApp.GetInstance().GetGameWorld().State != GameState.GameOverUILose)
					{
						break;
					}
					if (GameApp.GetInstance().GetGameMode().IsCoopMode())
					{
						LeaveRoomRequest request2 = new LeaveRoomRequest();
						GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
						Lobby.GetInstance().IsMasterPlayer = false;
						Lobby.GetInstance().SetCurrentRoomID(-1);
						if (GameApp.GetInstance().GetGameWorld() != null)
						{
							GameApp.GetInstance().GetGameWorld().Exit = true;
						}
					}
					if (GameApp.GetInstance().GetGameWorld() != null)
					{
						GameApp.GetInstance().GetGameWorld().SubmitBattleScores();
					}
					FrGoToPhase(16, false, false, false);
				}
			}
			else if (GameApp.GetInstance().GetGameWorld().State == GameState.GameOverUIWin)
			{
				GameApp.GetInstance().GetGameWorld().SubmitBattleScores();
				FrGoToPhase(7, false, false, false);
			}
			else if (GameApp.GetInstance().GetGameWorld().State == GameState.GameOverUILose)
			{
				GameApp.GetInstance().GetGameWorld().SubmitBattleScores();
				FrGoToPhase(7, false, false, false);
			}
			break;
		case 7:
			if (ResultPageUI.mInstance != null)
			{
				ResultPageUI.mInstance.OnUpdate();
			}
			break;
		case 16:
			if (ResultPageUI.mInstance != null)
			{
				ResultPageUI.mInstance.OnUpdate();
			}
			break;
		}
	}

	public void RestartGame()
	{
		FrGoToPhase(6, false, false, false);
	}

	private void SetIPadScreen()
	{
		if (Screen.width == 1024)
		{
			UIConstant.ScreenLocalWidth = Screen.width;
			UIConstant.ScreenLocalHeight = Screen.height;
			UIConstant.ScreenAdaptived.x = 1f;
			UIConstant.ScreenAdaptived.y = 1f;
		}
	}

	private void SetIPhone5Screen()
	{
		if (Screen.width == 1136)
		{
			UIConstant.ScreenLocalWidth = Screen.width;
			UIConstant.ScreenLocalHeight = Screen.height;
			UIConstant.ScreenAdaptived.x = 1f;
			UIConstant.ScreenAdaptived.y = 1f;
		}
	}

	private bool UpdateNetwork()
	{
		networkMgr = GameApp.GetInstance().GetNetworkManager();
		if (networkMgr != null)
		{
			if (networkMgr.IsDisconnected || !networkMgr.IsConnected())
			{
				if (!UIMsgBox.instance.IsMessageShow())
				{
					if (networkMgr.IsTimeout)
					{
						UIMsgBox.instance.ShowNetworkTimeoutMessage(this);
					}
					else if (networkMgr.exceptionMessage == null || networkMgr.exceptionMessage == string.Empty)
					{
						UIMsgBox.instance.ShowNetworkTimeoutMessage(this);
					}
					else
					{
						SwHandlerExceptionRequest request = new SwHandlerExceptionRequest(GameApp.GetInstance().GetUserState().GetOnlineUserId(), networkMgr.exceptionMessage);
						GameApp.GetInstance().GetWWWManager().SendRequest(request);
						UIMsgBox.instance.ShowException(this);
					}
				}
				return true;
			}
			if (networkMgr.IsDisconnected || networkMgr.IsDisplayErrorBox)
			{
				return true;
			}
		}
		return false;
	}

	public void GotoBattleUI()
	{
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
	}

	public override void FrFree()
	{
		m_UIManager.Destory();
		m_UIManager.RemoveAll();
		m_UIPopupManager.Destory();
		m_UIPopupManager.RemoveAll();
	}

	public void StartLoading(byte mode, int nMax)
	{
		FrGoToPhase(1, false, true, false);
	}

	public void VSGameAutoBalance()
	{
	}

	public void WaitVSRebirthStart()
	{
	}

	public void WaitVSRebirthEnd()
	{
	}

	public void OnVSRebirth()
	{
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if ((whichMsg.EventId == 34 || whichMsg.EventId == 39) && buttonId == UIMsg.UIMsgButton.Ok)
		{
			UIMsgBox.instance.CloseMessage();
			Lobby.GetInstance().IsMasterPlayer = false;
			Lobby.GetInstance().SetCurrentRoomID(-1);
			if (GameApp.GetInstance().GetGameWorld() != null)
			{
				GameApp.GetInstance().GetGameWorld().Exit = true;
			}
			FrGoToPhase(16, false, false, false);
		}
	}
}
