using System;
using UnityEngine;

public class HUDOnline : HUDBase, UIMsgListener
{
	public GameObject TDMPage;

	public GameObject KOHPage;

	public GameObject DominationPage;

	public GameObject CTFPage;

	public GameObject FFAPage;

	public GameObject MutantPage;

	public GameObject ScorePage;

	public GameObject RebirthOnline;

	public GameObject ScoreButton;

	public GameObject ExitButton;
    

    public UIRadar BombRadar;

	public UIRadar ChoosePointRebirthRadar;

	private bool mIsScorePageOpen;

	private void Awake()
	{
		HUDBase.instance = this;
		CloseAll();
		mType = HUDType.None;
		TDMPage.SetActive(false);
		KOHPage.SetActive(false);
		DominationPage.SetActive(false);
		CTFPage.SetActive(false);
		FFAPage.SetActive(false);
		MutantPage.SetActive(false);
		ScorePage.SetActive(false);
		RebirthOnline.SetActive(false);
		switch (GameApp.GetInstance().GetGameMode().ModePlay)
		{
		case Mode.VS_KOTH:
			mType = HUDType.VSKOHBattle;
			KOHPage.SetActive(true);
			break;
		case Mode.Domination:
			mType = HUDType.VSDominationBattle;
			DominationPage.SetActive(true);
			break;
		case Mode.VS_TDM:
			mType = HUDType.VSTDMBattle;
			TDMPage.SetActive(true);
			break;
		case Mode.VS_CTF_TDM:
			mType = HUDType.VSCTFBattle;
			CTFPage.SetActive(true);
			break;
		case Mode.VS_FFA:
			mType = HUDType.VSFFABattle;
			FFAPage.SetActive(true);
			break;
		case Mode.GIANT_BETRAYAL:
			mType = HUDType.VSMutant;
			MutantPage.SetActive(true);
			break;
		}
		mIsScorePageOpen = false;
		UIEventListener uIEventListener = UIEventListener.Get(ScoreButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		UIEventListener uIEventListener2 = UIEventListener.Get(RebirthOnline);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		UIEventListener uIEventListener3 = UIEventListener.Get(ExitButton);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		if (m_PlayerHpLabel != null)
		{
			m_PlayerHpLabel.gameObject.SetActive(false);
		}
	}

	protected override void OnInit(GameUIBundle bundle)
	{
		base.OnInit(bundle);
		Debug.Log("ManagerHUD bundle : " + bundle);
		if (bundle != null)
		{
			object obj = bundle.Get("HUDType");
			if (obj != null)
			{
				LoadHUD((HUDType)(int)obj);
			}
		}
	}

	public new void LoadHUD(HUDType type)
	{
		if (type == HUDType.Battle)
		{
			LoadSubUI(new BattleSubHUD());
		}
	}

	public void OpenScorePage(bool open)
	{
        if (open)
        {
            ScorePage.SetActive(true);
            ScoreButton.GetComponentInChildren<UISprite>().spriteName = "score_d";
            
            if (CursorManager.Instance != null)
            {
                CursorManager.Instance.UnlockCursor();
            }
        }
        else
        {
            ScorePage.SetActive(false);
            ScoreButton.GetComponentInChildren<UISprite>().spriteName = "score_n";
            
            if (CursorManager.Instance != null)
            {
                CursorManager.Instance.LockCursor();
            }
        }
        mIsScorePageOpen = open;
    }

	private void OnClickButton(GameObject go)
	{
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
		if (go == ScoreButton)
		{
			if (!mIsScorePageOpen)
			{
				OpenScorePage(true);
			}
			else
			{
				OpenScorePage(false);
			}
		}
		else if (go == RebirthOnline)
		{
			GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.SendRebirthRequest();
			RebirthOnline.SetActive(false);
		}
		else if (go == ExitButton)
		{
			UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_PVP_EXIT_MIDDLE"), 2, 29);
		}
	}

	public void Exit()
	{
		LeaveRoomRequest request = new LeaveRoomRequest();
		GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		Lobby.GetInstance().IsMasterPlayer = false;
		Lobby.GetInstance().SetCurrentRoomID(-1);
		if (GameApp.GetInstance().GetGameWorld() != null)
		{
			GameApp.GetInstance().GetGameWorld().Exit = true;
			GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.OnQuitOnline();
			GameApp.GetInstance().GetGameWorld().SetGameState(GameState.GameOverUILose);
			GameApp.GetInstance().GetGameWorld().SubmitBattleScores();
		}
		GameApp.GetInstance().GetUIStateManager().FrGoToPhase(16, false, false, false);
	}

	public void ForceExit()
	{
		Lobby.GetInstance().IsMasterPlayer = false;
		Lobby.GetInstance().SetCurrentRoomID(-1);
		if (GameApp.GetInstance().GetGameWorld() != null)
		{
			GameApp.GetInstance().GetGameWorld().Exit = true;
			GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.OnQuitOnline();
			GameApp.GetInstance().GetGameWorld().SetGameState(GameState.GameOverUILose);
			GameApp.GetInstance().GetGameWorld().SubmitBattleScores();
		}
		GameApp.GetInstance().GetUIStateManager().FrGoToPhase(16, false, false, false);
	}

	protected override void OnUIUpdate()
	{
		base.OnUIUpdate();
		UpdatePlayerHP();
		if (BombRadar.gameObject.activeSelf)
		{
			Hide();
		}
		if (ChoosePointRebirthRadar.gameObject.activeSelf)
		{
			Hide();
		}
	}

	public override void HideWhenWaitingRebirth()
	{
		base.HideWhenWaitingRebirth();
		BombRadar.gameObject.SetActive(false);
	}

	public void ActiveBombPage(float radius, AdvancedSkill.EMissleType type, short skillID = 0)
	{
		if (!BombRadar.gameObject.activeSelf)
		{
			BombRadar.gameObject.SetActive(true);
			BombRadar.GetComponent<UIRadar>().SetBombIconRadius(radius);
			BombRadar.GetComponent<UIRadar>().BombType = type;
			BombRadar.GetComponent<UIRadar>().SkillId = skillID;
		}
	}

	public void HideRebirthPage()
	{
		if (ChoosePointRebirthRadar.gameObject.activeSelf)
		{
			ChoosePointRebirthRadar.gameObject.SetActive(false);
		}
	}

	public void ActiveRebirthPage()
	{
		if (!ChoosePointRebirthRadar.gameObject.activeSelf)
		{
			ChoosePointRebirthRadar.gameObject.SetActive(true);
		}
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (whichMsg.EventId == 29)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				Exit();
			}
			UIMsgBox.instance.CloseMessage();
		}
	}
}
