using System;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : HUDBase
{
	public new const string BUNDLE_KEY_HUD_TYPE = "HUDType";

	public GameObject m_PauseCollision;

	public GameObject m_PauseMenu;

	public GameObject m_PauseButton;

	public GameObject m_ResumeButton;

	public UIWidgetSet m_ResumeButton2;

	public UIWidgetSet m_RestartButton;

	public UIWidgetSet m_OptionButton;

	public UIWidgetSet m_QuitButton;

	public UISprite m_ResumeHighlight;

	public UISprite m_RestartHighlight;

	public UISprite m_OptionHighlight;

	public UISprite m_QuitHighlight;

	public MissionDetailsHUD m_MissionDetails;

	public UIDialogBubble m_FightDialog;

	public UILabel m_RestartLabel;

	public GameObject[] PartnerHPObjs;

	public UILabel[] PartnerNames;

	public UITexture[] PartnerPortraits;

	public UISprite[] PartnerFrames;

	public UISprite[] PartnerHPs;

	public UISprite[] PartnerHPBackgrounds;

	public GameObject m_InvincibleButton;

	public GameObject m_SpawnEnemyButton;

	public GameObject m_LittleHuoBanButton;

	private bool mIsPartnerInited;

	private bool mIsSelfExit;

	public bool IsSelfExit
	{
		get
		{
			return mIsSelfExit;
		}
	}

	private void Awake()
	{
		HUDBase.instance = this;
		CloseAll();
		mType = HUDType.None;
		player = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		GameObject[] partnerHPObjs = PartnerHPObjs;
		foreach (GameObject gameObject in partnerHPObjs)
		{
			gameObject.SetActive(false);
		}
		InitPartner();
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			m_RestartButton.SetAlpha(0.1f);
			m_RestartLabel.alpha = 0.2f;
			m_RestartButton.GetComponent<Collider>().enabled = false;
		}
		UIEventListener uIEventListener = UIEventListener.Get(m_PauseButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		UIEventListener uIEventListener2 = UIEventListener.Get(m_ResumeButton);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		UIEventListener uIEventListener3 = UIEventListener.Get(m_ResumeButton2.gameObject);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		uIEventListener3.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener3.onPress, new UIEventListener.BoolDelegate(OnPressButton));
		UIEventListener uIEventListener4 = UIEventListener.Get(m_RestartButton.gameObject);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		uIEventListener4.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener4.onPress, new UIEventListener.BoolDelegate(OnPressButton));
		UIEventListener uIEventListener5 = UIEventListener.Get(m_OptionButton.gameObject);
		uIEventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener5.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		uIEventListener5.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener5.onPress, new UIEventListener.BoolDelegate(OnPressButton));
		UIEventListener uIEventListener6 = UIEventListener.Get(m_QuitButton.gameObject);
		uIEventListener6.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener6.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		uIEventListener6.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener6.onPress, new UIEventListener.BoolDelegate(OnPressButton));
		UIEventListener uIEventListener7 = UIEventListener.Get(m_InvincibleButton);
		uIEventListener7.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener7.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		UIEventListener uIEventListener8 = UIEventListener.Get(m_SpawnEnemyButton);
		uIEventListener8.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener8.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		UIEventListener uIEventListener9 = UIEventListener.Get(m_LittleHuoBanButton);
		uIEventListener9.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener9.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		mIsSelfExit = false;
	}

	protected override void OnInit(GameUIBundle bundle)
	{
		base.OnInit(bundle);
		if (bundle != null)
		{
			object obj = bundle.Get("HUDType");
			if (obj != null)
			{
				LoadHUD((HUDType)(int)obj);
			}
		}
        if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.Android)
        {
            m_JoystickManager.gameObject.SetActive(false);
        }
        byte selectedStageIndex = GameApp.GetInstance().GetUserState().GetSelectedStageIndex();
		byte selectedSubStageIndex = GameApp.GetInstance().GetUserState().GetSelectedSubStageIndex();
		if (selectedStageIndex == 0 && selectedSubStageIndex == 1 && !TutorialManager.GetInstance().GetTutorialWatched(TutorialTriggerType.FIRST_MISSION) && GameApp.GetInstance().GetGameMode().IsSingle() && !GameApp.GetInstance().GetUserState().GetSkipTutorial() && PlayerPrefs.GetInt("\ufffd\ufffd\u05b9\ufffd\u0333\ufffd") == 0)
		{
			TutorialManager.GetInstance().Trigger(TutorialTriggerType.FIRST_MISSION);
			Time.timeScale = 0f;
			GameApp.GetInstance().GetUIStateManager().FrGoToPhase(14, false, true, false);
		}
		else if (selectedStageIndex == 0 && selectedSubStageIndex == Global.BOSS_FINAL_SUB_STAGE_ID && !TutorialManager.GetInstance().GetTutorialWatched(TutorialTriggerType.BOSS_INSTRUCTION) && GameApp.GetInstance().GetGameMode().IsSingle() && !GameApp.GetInstance().GetUserState().GetSkipTutorial() && PlayerPrefs.GetInt("\ufffd\ufffd\u05b9\ufffd\u0333\ufffd") == 0)
		{
			TutorialManager.GetInstance().Trigger(TutorialTriggerType.BOSS_INSTRUCTION);
			Time.timeScale = 0f;
			GameApp.GetInstance().GetUIStateManager().FrGoToPhase(14, false, true, false);
		}
		else
		{
			VirtualCursorScript._HideCursor();
		}
	}

	public new void LoadHUD(HUDType type)
	{
		if (type == HUDType.Battle)
		{
			LoadSubUI(new BattleSubHUD());
		}
	}

	private void OnDestroy()
	{
		HUDBase.instance = null;
	}

	public new void Invalidate()
	{
		CloseAll();
		mType = HUDType.None;
	}

	protected override void OnShow()
	{
		hudState = HUDState.Show;
	}

	protected override void OnHide()
	{
		hudState = HUDState.Hide;
	}

	public new HUDType GetRunningHUDType()
	{
		return mType;
	}

	public override void OnPause()
	{
		UIRadar.mInstance.gameObject.SetActive(false);
		m_PauseCollision.SetActive(true);
		m_PauseMenu.SetActive(true);
		VirtualCursorScript._ShowCursor();
	}

	public override void OnResume()
	{
		UIRadar.mInstance.gameObject.SetActive(true);
		m_PauseCollision.SetActive(false);
		m_PauseMenu.SetActive(false);
		VirtualCursorScript._HideCursor();
	}

	public new void Hide()
	{
		m_SkillHUDManager.Hide();
		m_JoystickManager.Hide();
		m_Weapon.Hide();
		m_Aim.Hide();
		VirtualCursorScript._ShowCursor();
	}

	public new void Show()
	{
		m_SkillHUDManager.Show();
		m_JoystickManager.Show();
		m_Weapon.Show();
		m_Aim.Show();
		VirtualCursorScript._HideCursor();
	}

	public new void HideWhenWaitingRebirth()
	{
		m_SkillHUDManager.Hide();
		m_JoystickManager.Hide();
		m_Weapon.Hide();
		m_Aim.Hide();
	}

	public new bool InGameFinishState()
	{
		return player.State == Player.WIN_STATE || player.State == Player.LOSE_STATE;
	}

	public new bool InWaitRebirthState()
	{
		return player.State == Player.DEAD_STATE || player.State == Player.WAIT_REBIRTH_STATE || player.State == Player.WAIT_VS_REBIRTH_STATE;
	}

	private void OnClickButton(GameObject go)
	{
		if (go == m_PauseButton)
		{
			if (GameApp.GetInstance().GetGameWorld() != null && GameApp.GetInstance().GetGameWorld().GetLocalPlayer() != null && GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.State != Player.DEAD_STATE && GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.State != Player.WAIT_REBIRTH_STATE && GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.State != Player.WIN_STATE && GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.State != Player.LOSE_STATE)
			{
				if (GameApp.GetInstance().GetGameMode().IsSingle())
				{
					Time.timeScale = 0f;
				}
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
				OnPause();
				GameApp.GetInstance().GetUIStateManager().FrGoToPhase(14, false, true, false);
			}
		}
		else if (go == m_ResumeButton || go == m_ResumeButton2.gameObject)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			if (GameApp.GetInstance().GetGameMode().IsSingle())
			{
				Time.timeScale = 1f;
			}
			OnResume();
			GameApp.GetInstance().GetUIStateManager().FrGoToPhase(6, true, false, false);
		}
		else if (go == m_RestartButton.gameObject)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (GameApp.GetInstance().GetGameMode().IsSingle())
			{
				Time.timeScale = 1f;
			}
			UserState userState = GameApp.GetInstance().GetUserState();
			if (userState.GetSelectedSubStageIndex() == Global.BOSS_FINAL_SUB_STAGE_ID)
			{
				userState.SetSelectedSubStageIndex((byte)Global.BOSS_SUB_STAGE_ID);
				GameApp.GetInstance().GetGameMode().ModePlay = Mode.PRE_BOSS;
			}
			userState.AnalysisManager.GetCurrentPVEStageInfo().playTime++;
			userState.AnalysisManager.SetCurrentPlayRecord();
			string levelName = GameApp.GetInstance().GetUserState().GetLevelConfig(userState.GetSelectedStageIndex(), userState.GetSelectedSubStageIndex())
				.levelName;
			MissionSelectUIScript.ShowLoadingPage(userState.GetSelectedDifficultMode(), userState.GetSelectedStageIndex(), userState.GetSelectedSubStageIndex());
			GameWorld.LoadLevel(levelName, ELevelType.GAME);
		}
		else if (go == m_OptionButton.gameObject)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			GameObject original = Resources.Load("UI/Menu/OptionUI/Prefab/OptionUI") as GameObject;
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original, new Vector3(0f, 1500f, 0f), Quaternion.identity);
			gameObject.GetComponent<OptionUIScript>().SetIsInMainMenu(false);
		}
		else if (go == m_QuitButton.gameObject)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (GameApp.GetInstance().GetGameMode().IsSingle())
			{
				Time.timeScale = 1f;
			}
			Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			if (localPlayer != null)
			{
				localPlayer.OnMissionFail();
			}
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				LeaveRoomRequest request = new LeaveRoomRequest();
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				Lobby.GetInstance().IsMasterPlayer = false;
				Lobby.GetInstance().SetCurrentRoomID(-1);
			}
			OnResume();
			GameApp.GetInstance().GetUIStateManager().FrGoToPhase(6, false, false, false);
			mIsSelfExit = true;
		}
		else if (go == m_InvincibleButton)
		{
			Player localPlayer2 = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			if (localPlayer2 != null)
			{
				localPlayer2.ToggleInvincible();
			}
			if (localPlayer2.IsInvincible())
			{
				m_InvincibleButton.GetComponentInChildren<UISprite>().spriteName = "btn1_d";
			}
			else
			{
				m_InvincibleButton.GetComponentInChildren<UISprite>().spriteName = "btn1_n";
			}
		}
		else if (go == m_SpawnEnemyButton)
		{
			GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
			if (gameWorld != null)
			{
				gameWorld.ToggleSpawnEnemySwitch();
				if (gameWorld.CanSpawnEnemy())
				{
					m_SpawnEnemyButton.GetComponentInChildren<UISprite>().spriteName = "btn1_d";
				}
				else
				{
					m_SpawnEnemyButton.GetComponentInChildren<UISprite>().spriteName = "btn1_n";
				}
			}
		}
		else
		{
			if (!(go == m_LittleHuoBanButton))
			{
				return;
			}
			Dictionary<int, OnlineFriend> friendDictionary = GameApp.GetInstance().GetUserState().GetFriendDictionary();
			Debug.Log("friendDictionary.Count = " + friendDictionary.Count);
			if (friendDictionary.Count > 0)
			{
				using (Dictionary<int, OnlineFriend>.ValueCollection.Enumerator enumerator = friendDictionary.Values.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						OnlineFriend current = enumerator.Current;
						GameApp.GetInstance().GetGameWorld().CreateRobot(current);
					}
					return;
				}
			}
			GameApp.GetInstance().GetGameWorld().CreateRobot();
		}
	}

	private void OnPressButton(GameObject go, bool isPressed)
	{
		if (go == m_ResumeButton2.gameObject)
		{
			if (isPressed)
			{
				m_ResumeButton2.SetAlpha(1f);
			}
			else
			{
				m_ResumeButton2.SetAlpha(0.5f);
			}
			m_ResumeHighlight.gameObject.SetActive(isPressed);
		}
		else if (go == m_RestartButton.gameObject)
		{
			if (isPressed)
			{
				m_RestartButton.SetAlpha(1f);
			}
			else
			{
				m_RestartButton.SetAlpha(0.5f);
			}
			m_RestartHighlight.gameObject.SetActive(isPressed);
		}
		else if (go == m_OptionButton.gameObject)
		{
			if (isPressed)
			{
				m_OptionButton.SetAlpha(1f);
			}
			else
			{
				m_OptionButton.SetAlpha(0.5f);
			}
			m_OptionHighlight.gameObject.SetActive(isPressed);
		}
		else if (go == m_QuitButton.gameObject)
		{
			if (isPressed)
			{
				m_QuitButton.SetAlpha(1f);
			}
			else
			{
				m_QuitButton.SetAlpha(0.5f);
			}
			m_QuitHighlight.gameObject.SetActive(isPressed);
		}
	}

	protected override void OnUIUpdate()
	{
		base.OnUIUpdate();
		if (!mIsPartnerInited)
		{
			InitPartner();
		}
		else
		{
			UpdatePartnerHp();
		}
	}

	private void InitPartner()
	{
		if (!GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			return;
		}
		List<RemotePlayer> remotePlayers = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
		if (remotePlayers.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < remotePlayers.Count; i++)
		{
			if (i < PartnerHPObjs.Length && remotePlayers[i] != null)
			{
				PartnerHPObjs[i].SetActive(true);
				PartnerNames[i].text = remotePlayers[i].Name;
				GameWorld.RefreshHeadFrame(PartnerFrames[i], remotePlayers[i].GetUserState().GetCurrentTilteFrameID(), remotePlayers[i].GetUserState().GetOnlinePlayer().GetPrestige());
				mIsPartnerInited = true;
			}
		}
	}

	private void UpdatePartnerHp()
	{
		if (!GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			return;
		}
		List<RemotePlayer> remotePlayers = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
		for (int i = 0; i < PartnerHPObjs.Length; i++)
		{
			if (i < remotePlayers.Count && remotePlayers[i] != null)
			{
				if (!remotePlayers[i].HasLeavedGame())
				{
					if (!PartnerHPObjs[i].activeSelf || PartnerNames[i].text != remotePlayers[i].Name)
					{
						PartnerHPObjs[i].SetActive(true);
						PartnerNames[i].text = remotePlayers[i].Name;
						GameWorld.RefreshHeadFrame(PartnerFrames[i], remotePlayers[i].GetUserState().GetCurrentTilteFrameID(), remotePlayers[i].GetUserState().GetOnlinePlayer().GetPrestige());
					}
					float num = (float)remotePlayers[i].Hp / (float)remotePlayers[i].MaxHp;
					PartnerHPs[i].fillAmount = num;
					if (num < 0.3f)
					{
						PartnerHPs[i].spriteName = "small_full1";
						PartnerHPBackgrounds[i].spriteName = "small_empty1";
					}
					else
					{
						PartnerHPs[i].spriteName = "small_full";
						PartnerHPBackgrounds[i].spriteName = "small_empty";
					}
					SocialManager.UpdateHead(remotePlayers[i].SocialPlayer, PartnerPortraits[i]);
				}
				else
				{
					PartnerHPObjs[i].SetActive(false);
				}
			}
			else
			{
				PartnerHPObjs[i].SetActive(false);
			}
		}
	}
}
