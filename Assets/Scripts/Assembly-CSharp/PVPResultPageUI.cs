using System;
using UnityEngine;

public class PVPResultPageUI : MonoBehaviour, UIMsgListener
{
	public enum PVPResultPageType
	{
		SelfPage = 0,
		RankPage = 1,
		AchievePage = 2,
		PageCount = 3
	}

	public static PVPResultPageUI mInstance;

	public UIWidgetSet SelfStatisticButton;

	public UIWidgetSet RankButton;

	public UIWidgetSet AchieveButton;

	public UISprite Highlight_Self;

	public UISprite Highlight_Rank;

	public UISprite Highlight_Achieve;

	public PVPSelfStatisticPageScript SelfStatiticPage;

	public GameObject RankPage;

	public GameObject AchievePage;

	public GameObject ExitButton;

	public GameObject FBButton;

	public GameObject TTButton;

	public UILabel CountDown;

	public GameObject WaitingTipObj;

	private PVPResultPageType mCurrentPage;

	private float mTime;

	private bool mIsWaiting;

	public PVPResultPageType CurrentPage
	{
		get
		{
			return mCurrentPage;
		}
	}

	private void Awake()
	{
		mInstance = this;
		SelfStatiticPage.gameObject.SetActive(false);
		RankPage.SetActive(false);
		AchievePage.SetActive(false);
		UIEventListener uIEventListener = UIEventListener.Get(SelfStatisticButton.gameObject);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(RankButton.gameObject);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(AchieveButton.gameObject);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener4 = UIEventListener.Get(ExitButton);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener5 = UIEventListener.Get(FBButton);
		uIEventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener5.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener6 = UIEventListener.Get(TTButton);
		uIEventListener6.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener6.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		if (HUDBase.instance != null)
		{
			HUDBase.instance.gameObject.SetActive(false);
		}
		if (UIRadar.mInstance != null)
		{
			UIRadar.mInstance.gameObject.SetActive(false);
		}
		WaitingTipObj.SetActive(Lobby.GetInstance().GetCurrentRoomID() != -1);
		if (!AndroidConstant.isFacebook())
		{
			FBButton.SetActive(false);
		}
		TTButton.SetActive(false);
	}

	private void Start()
	{
		mTime = Time.time;
		mIsWaiting = true;
		SelfStatisticButton.SetAlpha(0.5f);
		RankButton.SetAlpha(0.5f);
		AchieveButton.SetAlpha(0.5f);
		ChangePage(PVPResultPageType.RankPage);
		ExitButton.SetActive(false);
	}

	private void OnDestroy()
	{
		mInstance = null;
		UIMsgBox.instance.CloseMessage();
	}

	private void Update()
	{
		float num = Time.time - mTime;
		int num2 = Mathf.CeilToInt(15f - num);
		if (num2 >= 0)
		{
			CountDown.text = num2.ToString();
		}
		if (num2 <= 0 && Lobby.GetInstance().GetCurrentRoomID() == -1)
		{
			Exit();
		}
		if (mIsWaiting && num >= 5f)
		{
			mIsWaiting = false;
			ChangePage(PVPResultPageType.SelfPage);
			ExitButton.SetActive(true);
		}
	}

	public void ChangePage(PVPResultPageType page)
	{
		if (page != mCurrentPage)
		{
			switch (mCurrentPage)
			{
			case PVPResultPageType.SelfPage:
				SelfStatiticPage.gameObject.SetActive(false);
				SelfStatisticButton.SetAlpha(0.5f);
				Highlight_Self.gameObject.SetActive(false);
				break;
			case PVPResultPageType.RankPage:
				RankPage.SetActive(false);
				RankButton.SetAlpha(0.5f);
				Highlight_Rank.gameObject.SetActive(false);
				break;
			case PVPResultPageType.AchievePage:
				AchievePage.SetActive(false);
				AchieveButton.SetAlpha(0.5f);
				Highlight_Achieve.gameObject.SetActive(false);
				break;
			}
			switch (page)
			{
			case PVPResultPageType.SelfPage:
				SelfStatiticPage.OpenPage();
				SelfStatisticButton.SetAlpha(1f);
				Highlight_Self.gameObject.SetActive(false);
				break;
			case PVPResultPageType.RankPage:
				RankPage.SetActive(true);
				RankButton.SetAlpha(1f);
				Highlight_Rank.gameObject.SetActive(true);
				break;
			case PVPResultPageType.AchievePage:
				AchievePage.SetActive(true);
				AchieveButton.SetAlpha(1f);
				Highlight_Achieve.gameObject.SetActive(true);
				break;
			}
			mCurrentPage = page;
		}
	}

	public void OnButtonClick(GameObject go)
	{
		if (go == SelfStatisticButton.gameObject)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (!mIsWaiting)
			{
				ChangePage(PVPResultPageType.SelfPage);
			}
		}
		else if (go == RankButton.gameObject)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (!mIsWaiting)
			{
				ChangePage(PVPResultPageType.RankPage);
			}
		}
		else if (go == AchieveButton.gameObject)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (!mIsWaiting)
			{
				ChangePage(PVPResultPageType.AchievePage);
			}
		}
		else if (go == ExitButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			if (!mIsWaiting)
			{
				ExitButton.SetActive(false);
				Exit();
			}
		}
		else if (go == FBButton)
		{
			if (GameApp.GetInstance().IsConnectedToInternet())
			{
				ScreenCapture.CaptureScreenshot("tempscreens.png");
				if (!FacebookTools.isSessionValid())
				{
					ApplicationPauseManager.mInstance.IsPostingToFacebook = true;
					ApplicationPauseManager.mInstance.currentType = ApplicationPauseManager.EPostingType.PVP_SCORE;
					FacebookTools.CallFBLogin();
				}
				else
				{
					ApplicationPauseManager.mInstance.StartCoroutine(ApplicationPauseManager.mInstance.postToFacebook(ApplicationPauseManager.EPostingType.PVP_SCORE));
				}
			}
		}
		else if (go == TTButton && GameApp.GetInstance().IsConnectedToInternet())
		{
			ScreenCapture.CaptureScreenshot("tempscreens.png");
			ApplicationPauseManager.mInstance.StartCoroutine(ApplicationPauseManager.mInstance.postToTwitter(ApplicationPauseManager.EPostingType.PVP_SCORE));
		}
	}

	public void Exit()
	{
		if (GameApp.GetInstance().GetGameWorld().State == GameState.GameOverUIWin)
		{
			GameApp.GetInstance().GetUserState().Achievement.AddWeaponWinWith();
			GameApp.GetInstance().GetUserState().Achievement.CheckComboWins(true);
		}
		else
		{
			GameApp.GetInstance().GetUserState().Achievement.CheckComboWins(false);
		}
		GameApp.GetInstance().GetUserState().AnalysisManager.GetCurrentPVPInfo().playTime++;
		GameApp.GetInstance().GetUserState().Achievement.AddWeaponUsed();
		GameApp.GetInstance().GetUserState().AnalysisManager.SetCurrentPlayRecord();
		Lobby.GetInstance().IsMasterPlayer = false;
		Lobby.GetInstance().SetCurrentRoomID(-1);
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

    private void LoadLobbyOrMainMenu()
    {
        if (GameApp.GetInstance().GetNetworkManager().IsConnected())
        {
            LeaveRoomRequest request = new LeaveRoomRequest();
            GameApp.GetInstance().GetNetworkManager().SendRequest(request);
            Application.LoadLevel("MultiMenu");
        }
        else
        {
            GameWorld.LoadLevel("MainMenu", ELevelType.MENU);
        }
    }

    public void RestartGame()
	{
		if (HUDBase.instance != null)
		{
			HUDBase.instance.gameObject.SetActive(true);
		}
		if (UIRadar.mInstance != null)
		{
			UIRadar.mInstance.gameObject.SetActive(true);
		}
		foreach (PlayerInfoUI playerInfoUI_ in PlayerInfoUI.PlayerInfoUI_List)
		{
			UnityEngine.Object.Destroy(playerInfoUI_.gameObject);
		}
		PlayerInfoUI.PlayerInfoUI_List.Clear();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (whichMsg.EventId == 23)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				Lobby.GetInstance().NeedToTurnRegister = true;
				Exit();
			}
			UIMsgBox.instance.CloseMessage();
		}
	}
}
