using System;
using UnityEngine;

public class StateRebirth : MonoBehaviour
{
	private const int OPEN_NULL = 0;

	private const int OPEN_LOADOUT = 1;

	private const int OPEN_BOSSSKILL = 2;

	public UILabel m_Number;

	public UILabel mRebirthMithril;

	public GameObject RebirthObj;

	public GameObject ExitButton;

	public GameObject RebirthButton;

	public GameObject UnabledButton;

	public GameObject GoButton;

	public GameObject VideoRebirthButton;

	public bool IsForPVP;

	public LoadoutPageScript LoadoutPage;

	public SelectMutantSkillPage MutantSkillPage;

	private bool bRebirthInit;

	private float rebirthTimer;

	private float rebirthAnimaStartTimer;

	private Vector3 mBaseScale;

	private Color mBaseColor;

	private Player player;

	private int mCurrentPVPLoadoutID;

	private static string _currentRebirthMithril;

	private Timer chooseTimer;

	private bool mIsCoopRebirthClicked;

	private float mCoopRebirthClickedTime;

	private float mCoopRebirthClickedWaitingTime = 15f;

	private bool mIsjudgeAD = true;

	private bool mIsRebirth;

	private bool mIsReadyToShowMovie;

	private SystemLanguage language;

	private int mWhichIsOpen;

	public static bool needShowRebithObj;

	private static int currentRebirthMithril
	{
		get
		{
			return AntiCracking.DecryptBufferStr(_currentRebirthMithril, "fuhuojiage");
		}
		set
		{
			_currentRebirthMithril = AntiCracking.CryptBufferStr(value, "fuhuojiage");
		}
	}

	private void Awake()
	{
		language = Application.systemLanguage;
		Debug.Log("EUserLanguage language = " + language);
		chooseTimer = new Timer(30f, false);
		RebirthObj.SetActive(false);
		bRebirthInit = false;
		rebirthTimer = 0f;
		rebirthAnimaStartTimer = 0f;
		mBaseScale = m_Number.transform.localScale;
		mBaseColor = m_Number.color;
		player = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		if (IsForPVP)
		{
			mCurrentPVPLoadoutID = GameApp.GetInstance().GetUserState().GetCurrentCustomizeDataID();
			currentRebirthMithril = 5;
		}
		else
		{
			currentRebirthMithril = 15;
		}
		if (ExitButton != null)
		{
			UIEventListener uIEventListener = UIEventListener.Get(ExitButton);
			uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClicked));
		}
		if (GameApp.GetInstance().GetGameMode().IsSingle())
		{
			UIEventListener uIEventListener2 = UIEventListener.Get(VideoRebirthButton);
			uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClicked));
		}
	}

	private void CheckVideo()
	{
		if (GameApp.IsVideo)
		{
			Debug.Log("call VideoRebirth!");
			if (player != null && VideoRebirthButton != null)
			{
				Debug.Log("call VideoRebirth 2!");
				Time.timeScale = 1f;
				player.ReSpawnPVE();
				VideoRebirthButton.SetActive(false);
				GameApp.IsVideo = false;
			}
		}
	}

	private void Update()
	{
		if (player == null)
		{
			player = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		}
		if (GameApp.GetInstance().AppStatus == 1 && GameApp.GetInstance().NativeXVideoStatus == 1)
		{
			CheckVideo();
		}
		if (player == null)
		{
			return;
		}
		if (player.State == Player.WAIT_REBIRTH_STATE || player.State == Player.WAIT_VS_REBIRTH_STATE || player.State == Player.VS_INIT_LOADOUT_STATE || player.IsInChooseBossSkillState())
		{
			if (GameApp.GetInstance().GetGameMode().IsSingle())
			{
				if (GameApp.GetInstance().AppStatus == 1)
				{
					if (GameApp.GetInstance().NativeXVideoStatus == 1)
					{
						if (!mIsRebirth)
						{
							if (AndroidPluginScript.IsReadyAdcolony())
							{
								if (!VideoRebirthButton.activeSelf)
								{
									VideoRebirthButton.SetActive(true);
								}
							}
							else if (VideoRebirthButton.activeSelf)
							{
								VideoRebirthButton.SetActive(false);
							}
						}
					}
					else if (VideoRebirthButton.activeSelf)
					{
						VideoRebirthButton.SetActive(false);
					}
				}
				else if (VideoRebirthButton.activeSelf)
				{
					VideoRebirthButton.SetActive(false);
				}
			}
			if (needShowRebithObj)
			{
				RebirthObj.SetActive(true);
				needShowRebithObj = false;
			}
			if (!bRebirthInit)
			{
				bRebirthInit = true;
				if (player.GetSkills().HasSkill(SkillsType.CHOOSE_SPAWN_POINT) && player.State == Player.WAIT_VS_REBIRTH_STATE)
				{
					((HUDOnline)HUDBase.instance).ActiveRebirthPage();
				}
				else
				{
					RebirthObj.SetActive(true);
				}
				rebirthAnimaStartTimer = 0f;
				rebirthTimer = 1f;
				RebirthButton.GetComponent<Collider>().enabled = true;
				mIsCoopRebirthClicked = false;
				if (HUDBase.instance != null && HUDBase.instance.m_Weapon != null && HUDBase.instance.m_Weapon.gameObject.activeSelf)
				{
					HUDBase.instance.m_Weapon.gameObject.SetActive(false);
					HUDBase.instance.PlayerHPObj.SetActive(false);
				}
				if (StateBuff.mInstance != null && StateBuff.mInstance.gameObject.activeSelf)
				{
					StateBuff.mInstance.gameObject.SetActive(false);
				}
				if (player.State == Player.WAIT_REBIRTH_STATE || player.State == Player.WAIT_VS_REBIRTH_STATE)
				{
					mRebirthMithril.text = currentRebirthMithril.ToString();
					if (player.GetUserState().GetMithril() < currentRebirthMithril)
					{
						RebirthButton.SetActive(false);
						UnabledButton.SetActive(true);
						if (IsForPVP)
						{
							GoButton.SetActive(false);
						}
					}
					else
					{
						RebirthButton.SetActive(true);
						UnabledButton.SetActive(false);
						if (IsForPVP)
						{
							GoButton.SetActive(false);
						}
					}
				}
				else
				{
					UnabledButton.SetActive(false);
					RebirthButton.SetActive(false);
					if (IsForPVP)
					{
						GoButton.SetActive(true);
						chooseTimer.Do();
					}
				}
				if (GameApp.GetInstance().GetGameMode().IsVSMode())
				{
					if (HUDBase.instance != null && UIRadar.mInstance != null)
					{
						HUDBase.instance.m_JoystickManager.gameObject.SetActive(false);
						HUDBase.instance.m_BossStickmanager.gameObject.SetActive(false);
						UIRadar.mInstance.gameObject.SetActive(false);
					}
					if (player.IsInChooseBossSkillState())
					{
						MutantSkillPage.gameObject.SetActive(true);
						MutantSkillPage.OpenPage(player.BossIndex, player.BossSkillScore);
						LoadoutPage.gameObject.SetActive(false);
						if (HUDBase.instance != null && HUDBase.instance.m_Weapon != null)
						{
							HUDBase.instance.m_Weapon.Hide();
							HUDBase.instance.PlayerHPObj.SetActive(false);
						}
						if (StateBuff.mInstance != null)
						{
							StateBuff.mInstance.gameObject.SetActive(false);
						}
						mWhichIsOpen = 2;
					}
					else
					{
						MutantSkillPage.gameObject.SetActive(false);
						LoadoutPage.gameObject.SetActive(true);
						if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.GIANT_BETRAYAL)
						{
							LoadoutPage.CoopTabScript.gameObject.SetActive(true);
							for (int i = 0; i < LoadoutPage.VsTabScriptList.Count; i++)
							{
								LoadoutPage.VsTabScriptList[i].gameObject.SetActive(false);
							}
						}
						else
						{
							LoadoutPage.CoopTabScript.gameObject.SetActive(false);
							int onlineCustomizeSize = GameApp.GetInstance().GetUserState().GetOnlinePlayer()
								.GetOnlineCustomizeSize();
							for (int j = 0; j < onlineCustomizeSize; j++)
							{
								LoadoutPage.VsTabScriptList[j].gameObject.SetActive(true);
							}
						}
						mWhichIsOpen = 1;
					}
				}
			}
			if (mWhichIsOpen == 1)
			{
				if (player.State != Player.WAIT_REBIRTH_STATE && player.State != Player.WAIT_VS_REBIRTH_STATE && player.State != Player.VS_INIT_LOADOUT_STATE)
				{
					mWhichIsOpen = 0;
					bRebirthInit = false;
					return;
				}
			}
			else if (mWhichIsOpen == 2 && !player.IsInChooseBossSkillState())
			{
				mWhichIsOpen = 0;
				bRebirthInit = false;
				return;
			}
			Timer timer = player.GetRebirthTimer();
			float num = 1f;
			if (player.IsInChooseBossSkillState())
			{
				float chooseSkillSpanTime = MutantSkillPage.GetChooseSkillSpanTime();
				if (chooseSkillSpanTime >= rebirthTimer)
				{
					rebirthAnimaStartTimer = chooseSkillSpanTime;
					rebirthTimer = chooseSkillSpanTime + 1f;
					return;
				}
				if (m_Number != null && GameApp.GetInstance().GetGameMode().IsVSMode())
				{
					m_Number.text = Mathf.CeilToInt(MutantSkillPage.GetChooseSkillRestTime()).ToString();
				}
				num = 1f + (chooseSkillSpanTime - rebirthAnimaStartTimer);
				m_Number.gameObject.SetActive(true);
				m_Number.transform.localScale = new Vector3(mBaseScale.x * num, mBaseScale.y * num, 1f);
				m_Number.color = new Color(mBaseColor.r, mBaseColor.g, mBaseColor.b, mBaseColor.a + 1f - num);
			}
			else if (player.State == Player.VS_INIT_LOADOUT_STATE)
			{
				float timeSpan = chooseTimer.GetTimeSpan();
				if (timeSpan >= rebirthTimer)
				{
					rebirthAnimaStartTimer = timeSpan;
					rebirthTimer = timeSpan + 1f;
				}
				else
				{
					if (m_Number != null && GameApp.GetInstance().GetGameMode().IsVSMode())
					{
						m_Number.text = Mathf.CeilToInt(chooseTimer.GetInterval() - chooseTimer.GetTimeSpan()).ToString();
					}
					num = 1f + (timeSpan - rebirthAnimaStartTimer);
					m_Number.gameObject.SetActive(true);
					m_Number.transform.localScale = new Vector3(mBaseScale.x * num, mBaseScale.y * num, 1f);
					m_Number.color = new Color(mBaseColor.r, mBaseColor.g, mBaseColor.b, mBaseColor.a + 1f - num);
				}
				if (chooseTimer.Ready())
				{
					ClickEffectFinied();
				}
			}
			else
			{
				if (timer.Ready())
				{
					return;
				}
				if (!mIsCoopRebirthClicked)
				{
					float timeSpan2 = timer.GetTimeSpan();
					if (timeSpan2 >= rebirthTimer)
					{
						rebirthAnimaStartTimer = timeSpan2;
						rebirthTimer = timeSpan2 + 1f;
						return;
					}
					int num2 = (int)(6f - timeSpan2);
					if (m_Number != null)
					{
						if (GameApp.GetInstance().GetGameMode().IsVSMode())
						{
							m_Number.text = player.GetVSRebirthRemainingTime().ToString();
						}
						else
						{
							m_Number.text = num2.ToString();
						}
					}
					num = 1f + (timeSpan2 - rebirthAnimaStartTimer);
					m_Number.gameObject.SetActive(true);
					m_Number.transform.localScale = new Vector3(mBaseScale.x * num, mBaseScale.y * num, 1f);
					m_Number.color = new Color(mBaseColor.r, mBaseColor.g, mBaseColor.b, mBaseColor.a + 1f - num);
				}
				else if (Time.time - mCoopRebirthClickedTime >= mCoopRebirthClickedWaitingTime)
				{
					mIsCoopRebirthClicked = false;
					(player as LocalPlayer).IsCoopRebirthTimerBlock = false;
				}
			}
			return;
		}
		rebirthAnimaStartTimer = 0f;
		rebirthTimer = 0f;
		bRebirthInit = false;
		RebirthObj.SetActive(false);
		if (!player.IsBoss && HUDBase.instance != null && HUDBase.instance.m_Weapon != null && !HUDBase.instance.m_Weapon.gameObject.activeSelf)
		{
			HUDBase.instance.m_Weapon.gameObject.SetActive(true);
		}
		if (!player.IsBoss && StateBuff.mInstance != null && !StateBuff.mInstance.gameObject.activeSelf)
		{
			StateBuff.mInstance.gameObject.SetActive(true);
		}
		if (HUDBase.instance != null)
		{
			HUDBase.instance.PlayerHPObj.SetActive(true);
		}
		if (GameApp.GetInstance().GetGameMode().IsVSMode() && HUDBase.instance != null && UIRadar.mInstance != null)
		{
			if (GameApp.GetInstance().GetGameMode().ModePlay != Mode.GIANT_BETRAYAL)
			{
                if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
                {
                    HUDBase.instance.m_JoystickManager.gameObject.SetActive(true);
                }
            }
			else if (!player.IsBoss)
			{
                if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
                {
                    HUDBase.instance.m_JoystickManager.gameObject.SetActive(true);
                }

			}
			UIRadar.mInstance.gameObject.SetActive(true);
		}
	}

	public void RebirthClicked()
	{
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
		if ((player.State == Player.WAIT_REBIRTH_STATE || player.State == Player.WAIT_VS_REBIRTH_STATE) && player.GetUserState().GetMithril() >= currentRebirthMithril)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_SELECT);
			RebirthButton.GetComponent<Collider>().enabled = false;
			RebirthButton.SetActive(false);
			if (GoButton != null)
			{
				GoButton.SetActive(false);
			}
			if (player.State == Player.WAIT_REBIRTH_STATE)
			{
				player.GetUserState().AnalysisManager.GetCurrentPVEStageInfo().MithrilUsed += currentRebirthMithril;
				player.GetUserState().AnalysisManager.GetCurrentPVEStageInfo().RebirthTime++;
			}
			else if (player.State == Player.WAIT_VS_REBIRTH_STATE)
			{
				player.GetUserState().AnalysisManager.GetCurrentPVPInfo().mithrilUsed += currentRebirthMithril;
				player.GetUserState().AnalysisManager.GetCurrentPVPInfo().rebirthTime++;
			}
			if (GameApp.GetInstance().GetGameMode().IsSingle())
			{
				Time.timeScale = 0f;
			}
			player.GetUserState().BuyWithMithril(currentRebirthMithril);
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				SyncMithrilRequest request = new SyncMithrilRequest();
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			if (GameApp.GetInstance().GetGameMode().IsCoopMode())
			{
				mCoopRebirthClickedTime = Time.time;
				mIsCoopRebirthClicked = true;
				(player as LocalPlayer).IsCoopRebirthTimerBlock = true;
			}
			currentRebirthMithril *= 2;
		}
		ClickEffectFinied();
	}

	public void ClickEffectFinied()
	{
		Time.timeScale = 1f;
		if (IsForPVP)
		{
			if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.GIANT_BETRAYAL && player.IsBoss)
			{
				PlayerBossChooseSkillRequest request = new PlayerBossChooseSkillRequest(MutantSkillPage.GetSelectedSkills());
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				return;
			}
			mCurrentPVPLoadoutID = LoadoutPage.GetSelectedVSTabId();
		}
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			RebirthButton.SetActive(false);
			((LocalPlayer)player).SendOnlineRebirthRequest(mCurrentPVPLoadoutID);
		}
		else
		{
			player.ReSpawnPVE();
			mIsRebirth = true;
			VideoRebirthButton.SetActive(false);
		}
	}

	private void OnButtonClicked(GameObject go)
	{
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
		if (go == ExitButton)
		{
			if (HUDBase.instance != null)
			{
				((HUDOnline)HUDBase.instance).Exit();
			}
		}
		else
		{
			if (!(go == VideoRebirthButton))
			{
				return;
			}
			if (GameApp.GetInstance().IsConnectedToInternet())
			{
				if (GameApp.GetInstance().AppStatus == 1 && GameApp.GetInstance().NativeXVideoStatus == 1)
				{
					Time.timeScale = 0f;
					mIsRebirth = true;
					VideoRebirthButton.SetActive(false);
					AndroidPluginScript.GetIsVideoRebirth(true);
					AndroidPluginScript.CallAdcolonyVideo();
				}
			}
			else
			{
				Time.timeScale = 1f;
				UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_6"));
			}
		}
	}

	public static int GetCurrentRebirthPrice()
	{
		return currentRebirthMithril;
	}
}
