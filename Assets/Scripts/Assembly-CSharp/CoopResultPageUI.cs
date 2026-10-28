using System;
using System.Collections.Generic;
using UnityEngine;

public class CoopResultPageUI : MonoBehaviour, UIMsgListener
{
	public static CoopResultPageUI mInstance;

	public UILabel Gold;

	public UILabel BonusTipGold;

	public UILabel BonusTipMithril;

	public GameObject DetailButton;

	public GameObject ContinueButton;

	public UISprite BossIcon;

	public UISprite EnemyIcon;

	public GameObject PlayersObj;

	public GameObject PlayerTemplate;

	public int PlayerSpaceH = 25;

	public int PlayerSpaceV = 70;

	private List<Player> mPlayers = new List<Player>();

	private bool mIsWaitingInit;

	private string _mLocalPlayerGetCash;

	private string _mLocalPlayerGetMithril;

	private SwVoidDataManager.ESwVoidGetRewardState needWaitOnlineInfo = SwVoidDataManager.ESwVoidGetRewardState.GOT;

	private List<CoopPlayerResultScript> playerResultList = new List<CoopPlayerResultScript>();

	private CoopPlayerResultScript localPlayerResult;

	private ResultPageUI mDetailResult;

	private int mLocalPlayerGetCash
	{
		get
		{
			return AntiCracking.DecryptBufferStr(_mLocalPlayerGetCash, "liangjijiangli");
		}
		set
		{
			_mLocalPlayerGetCash = AntiCracking.CryptBufferStr(value, "liangjijiangli");
		}
	}

	private int mLocalPlayerGetMithril
	{
		get
		{
			return AntiCracking.DecryptBufferStr(_mLocalPlayerGetMithril, "liangjijiangli");
		}
		set
		{
			_mLocalPlayerGetMithril = AntiCracking.CryptBufferStr(value, "liangjijiangli");
		}
	}

	private void Awake()
	{
		mInstance = this;
		mDetailResult = null;
		UIEventListener uIEventListener = UIEventListener.Get(DetailButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(ContinueButton);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		if (GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			needWaitOnlineInfo = SwVoidDataManager.TryGetRewardFromOnline();
		}
		Debug.Log("first time" + needWaitOnlineInfo);
		TutorialManager.GetInstance().Trigger(TutorialTriggerType.FIRST_COOPERATION);
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	private void Start()
	{
		Init();
	}

	private void Update()
	{
		if (needWaitOnlineInfo != SwVoidDataManager.ESwVoidGetRewardState.WAITING)
		{
			return;
		}
		SwVoidGetRewardResponseData voidRewardData = GameApp.GetInstance().GetGameWorld().VoidRewardData;
		if (voidRewardData != null)
		{
			int voidParts = voidRewardData.materialTypeArray.Length;
			localPlayerResult.SetVoidParts(voidParts);
			if (voidRewardData.result == SwErrorCode.SUCCESS)
			{
				needWaitOnlineInfo = SwVoidDataManager.ESwVoidGetRewardState.GOT;
			}
			else
			{
				needWaitOnlineInfo = SwVoidDataManager.ESwVoidGetRewardState.NO_CONNECT;
			}
		}
	}

	public void Init()
	{
		BonusTipGold.gameObject.SetActive(false);
		BonusTipMithril.gameObject.SetActive(false);
		Mission currentStageMission = MissionManager.GetInstance().GetCurrentStageMission(0);
		if (currentStageMission != null && currentStageMission.Type == MissionType.BOSS)
		{
			GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.ENEMY_LIST, DataSheetName.ENEMY_OVERALL);
			EnemyType enemyType = ((MissionBoss)currentStageMission).GetBossList()[0].EnemyType;
			BossIcon.spriteName = gameDataTable.GetString((int)enemyType, 17);
		}
		else
		{
			BossIcon.gameObject.SetActive(false);
		}
		mPlayers.Clear();
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld != null)
		{
			Sort(gameWorld.GetLocalPlayer());
			foreach (RemotePlayer remotePlayer in gameWorld.GetRemotePlayers())
			{
				if (remotePlayer != null)
				{
					Sort(remotePlayer);
				}
			}
		}
		playerResultList.Clear();
		for (int i = 0; i < mPlayers.Count; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(PlayerTemplate) as GameObject;
			gameObject.transform.parent = PlayersObj.transform;
			gameObject.transform.localPosition = new Vector3(i * PlayerSpaceH, -i * PlayerSpaceV, 0f);
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			CoopPlayerResultScript component = gameObject.GetComponent<CoopPlayerResultScript>();
			int num = mPlayers[i].GetBossCash();
			int num2 = mPlayers[i].GetMonsterCash() + mPlayers[i].GetPickupCash() + mPlayers[i].GetBonusCash();
			if (mPlayers[i].IsLocal())
			{
				num = (int)((float)num * (1f + mPlayers[i].GetSkills().GetSkill(SkillsType.MONEY_BOOTH)));
				num2 = (int)((float)num2 * (1f + mPlayers[i].GetSkills().GetSkill(SkillsType.MONEY_BOOTH)));
			}
			component.Init(i + 1, mPlayers[i].GetName(), num, num2, mPlayers[i].GetBossMithril(), -1, false, mPlayers[i]);
			if (mPlayers[i].IsLocal())
			{
				if (!mPlayers[i].GetUserState().HasLoginOnline())
				{
					component.VoidParts.text = "0";
				}
				localPlayerResult = component;
				mLocalPlayerGetCash = (int)((float)(mPlayers[i].GetBossCash() + mPlayers[i].GetMonsterCash() + mPlayers[i].GetPickupCash() + mPlayers[i].GetBonusCash()) * (1f + mPlayers[i].GetSkills().GetSkill(SkillsType.MONEY_BOOTH)));
				mLocalPlayerGetMithril = mPlayers[i].GetBossMithril();
				if (currentStageMission != null && currentStageMission.Type == MissionType.BOSS && mPlayers[i].GetBossCash() > 0 && gameWorld.FirstChallenge)
				{
					BonusTipGold.gameObject.SetActive(true);
				}
				component.SelfFrame.gameObject.SetActive(true);
				Gold.text = mLocalPlayerGetCash.ToString();
			}
			playerResultList.Add(component);
		}
	}

	public void RefreshResult(int channelID)
	{
		foreach (CoopPlayerResultScript playerResult in playerResultList)
		{
			if (playerResult.GetPlayerChannelID() == channelID)
			{
				playerResult.Refresh();
			}
		}
	}

	public void OnButtonClick(GameObject go)
	{
		if (go == DetailButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (mDetailResult == null)
			{
				GameObject original = Resources.Load("UI/Menu/Result/Prefab/ResultPage") as GameObject;
				GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
				mDetailResult = gameObject.GetComponent<ResultPageUI>();
			}
			else
			{
				mDetailResult.gameObject.SetActive(true);
			}
			base.gameObject.SetActive(false);
		}
		else if (go == ContinueButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			NotificationManager.GetInstance().SendLocalNotificationForBoss();
			if (GameApp.GetInstance().GetGameWorld().State == GameState.GameOverUIWin)
			{
				GameApp.GetInstance().GetUserState().Achievement.AddWeaponWinWith();
				GameApp.GetInstance().GetUserState().AnalysisManager.GetCurrentPVEStageInfo().playTime++;
				GameApp.GetInstance().GetUserState().AnalysisManager.GetCurrentPVEStageInfo().passTime++;
			}
			else
			{
				GameApp.GetInstance().GetUserState().AnalysisManager.GetCurrentPVEStageInfo().playTime++;
			}
			GameApp.GetInstance().GetUserState().Achievement.AddWeaponUsed();
			GameApp.GetInstance().GetUserState().AnalysisManager.SetCurrentPlayRecord();
			if (mDetailResult == null)
			{
				GameApp.GetInstance().GetUserState().AddCash(mLocalPlayerGetCash);
				GameApp.GetInstance().GetUserState().Achievement.AddTotalCash(mLocalPlayerGetCash);
				GameApp.GetInstance().GetUserState().AnalysisManager.GetCurrentPVEStageInfo().goldCollect += mLocalPlayerGetCash;
				SyncCashRequest request = new SyncCashRequest();
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				GameApp.GetInstance().GetUserState().AddMithril(mLocalPlayerGetMithril);
				GameApp.GetInstance().GetUserState().AnalysisManager.GetCurrentPVEStageInfo().mithrilCollect += mLocalPlayerGetMithril;
				SyncMithrilRequest request2 = new SyncMithrilRequest();
				GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
			}
			GameApp.GetInstance().GetUserState().SetSelectedStageIndex(0);
			GameApp.GetInstance().GetUserState().SetSelectedSubStageIndex((byte)Global.SURVIVAL_SUB_STAGE_ID);
			GameApp.GetInstance().Save();
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
    }

    private void LoadLobbyOrMainMenu()
    {
        if (GameApp.GetInstance().GetNetworkManager().IsConnected())
        {
            
            Application.LoadLevel("MultiMenu");
        }
        else
        {
            GameWorld.LoadLevel("MainMenu", ELevelType.MENU);
        }
    }

    public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
	}

	public bool Sort(Player newPlayer)
	{
		if (mPlayers.Count == 0)
		{
			mPlayers.Add(newPlayer);
		}
		else
		{
			int num = 0;
			int num2 = mPlayers.Count - 1;
			int num3 = (num + num2) / 2;
			if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.Survival)
			{
				if (newPlayer.GetMonsterCash() >= mPlayers[num].GetMonsterCash())
				{
					mPlayers.Insert(num, newPlayer);
				}
				else if (newPlayer.GetMonsterCash() <= mPlayers[num2].GetMonsterCash())
				{
					mPlayers.Insert(num2 + 1, newPlayer);
				}
				else
				{
					while (num2 - num > 1)
					{
						float num4 = mPlayers[num3].GetMonsterCash();
						if ((float)newPlayer.GetMonsterCash() == num4)
						{
							num = num3;
							break;
						}
						if ((float)newPlayer.GetMonsterCash() < num4)
						{
							num = num3;
						}
						else
						{
							num2 = num3;
						}
						num3 = (num + num2) / 2;
					}
					mPlayers.Insert(num + 1, newPlayer);
				}
			}
			else if (newPlayer.GetBossCash() >= mPlayers[num].GetBossCash())
			{
				mPlayers.Insert(num, newPlayer);
			}
			else if (newPlayer.GetBossCash() <= mPlayers[num2].GetBossCash())
			{
				mPlayers.Insert(num2 + 1, newPlayer);
			}
			else
			{
				while (num2 - num > 1)
				{
					float num5 = mPlayers[num3].GetBossCash();
					if ((float)newPlayer.GetBossCash() == num5)
					{
						num = num3;
						break;
					}
					if ((float)newPlayer.GetBossCash() < num5)
					{
						num = num3;
					}
					else
					{
						num2 = num3;
					}
					num3 = (num + num2) / 2;
				}
				mPlayers.Insert(num + 1, newPlayer);
			}
		}
		return true;
	}

	public SwVoidDataManager.ESwVoidGetRewardState GetNeedWaitSwVoidDrop()
	{
		return needWaitOnlineInfo;
	}
}
