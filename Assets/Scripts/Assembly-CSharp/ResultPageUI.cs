using System;
using System.Collections.Generic;
using UnityEngine;

public class ResultPageUI : MonoBehaviour, UIMsgListener
{
	private const int RESULT_STATE_INIT = 0;

	private const int RESULT_STATE_NORMAL_MAIN = 1;

	private const int RESULT_STATE_NORMAL_SUB2 = 2;

	private const int RESULT_STATE_NORMAL_SUB3 = 3;

	private const int RESULT_STATE_NORMAL_SUB4 = 4;

	private const int RESULT_STATE_NORMAL_SUB5 = 5;

	private const int RESULT_STATE_BOSS = 6;

	private const int RESULT_STATE_ELIMINATE = 7;

	private const int RESULT_STATE_PICKUP_COMBO = 8;

	private const int RESULT_STATE_EQUIP = 9;

	private const int RESULT_STATE_FRIEND = 10;

	private const int RESULT_STATE_FINAL = 11;

	private const int MISSION_STATE_1 = 21;

	private const int MISSION_STATE_2 = 22;

	private const int MISSION_STATE_3 = 23;

	private const int MISSION_STATE_4 = 24;

	private const int MISSION_STATE_5 = 25;

	private const int MISSION_STATE_6 = 26;

	private const int MISSION_STATE_7 = 27;

	public static ResultPageUI mInstance;

	public GameObject NormalResultPage;

	public GameObject BossResultPage;

	public UnlockListPageScript UnlockPage;

	public UIMoneyStatus MoneyStatus;

	public UILabel TotalRewardMoney;

	public UILabel TotalRewardMoneyEffect;

	public GameObject CoopMithrilRewardObj;

	public UILabel CoopRewardMithril;

	public UILabel CoopRewardMithrilEffect;

	public GameObject ButtonsObj;

	public GameObject ContinueButton;

	public GameObject TryAgainButton;

	public GameObject BackButton;

	public TweenTransform PickupComboRewardObj;

	public TweenTransform KillRewardObj;

	public TweenTransform BossRewardObj;

	public TweenTransform EquipRewardObj;

	public TweenTransform FriendRewardObj;

	public UILabel PickupComboReward;

	public UILabel KillReward;

	public UILabel BossReward;

	public UILabel EquipReward;

	public UILabel FriendReward;

	public Transform[] RewardInitPosition;

	public Transform[] RewardFinalPosition;

	public GameObject VoidRewardObj;

	public UIGrid VoidRewards;

	public GameObject VoidRewardTemplate;

	public UILabel VoidGetString;

	public GameObject FBButton;

	public GameObject TTButton;

	public GameObject GoldMithrilStatus;

	public MissionResultUI[] m_MissionResults;

	public GameObject MissionSummaryObj;

	public UISprite TreasureIcon;

	public UISprite TreasureEffect;

	public GameObject MissionStarsObj;

	public UISprite[] MissionStars;

	public UISprite[] MissionStarEffects;

	public GameObject MithrilRewardObj;

	public UILabel MithirlRewardAmount;

	public GameObject MithrilRewardTitles;

	public GameObject MithrilRewardAlreadyGetTitles;

	public GameObject BossInfoObject;

	public UISprite BossIcon;

	public UILabel BossName;

	public GameObject EnemyKillInfoTemplate;

	public Transform[] KillInfoPositions;

	public Transform KillInfoInitPosition;

	[HideInInspector]
	public static bool IsGoToShop;

	public static bool IsGoToIap;

	public static bool IsGoToRookiePackage;

	public static bool IsGoToExpertPackage;

	public static bool IsGoToTrialWeaponBuy;

	private static int mMissionFailTimeDuringAppRun = 2;

	private int mState = -1;

	private float mSwitchStateTime = 0.6f;

	private float mStateStayedTime;

	private bool mMissionStateInit_Entered;

	private bool mMissionState1_Entered;

	private bool mMissionState2_Entered;

	private bool mMissionState3_Entered;

	private bool mMissionState4_Entered;

	private bool mMissionState5_Entered;

	private bool mMissionState6_Entered;

	private bool mMissionState7_Entered;

	private bool mMissionState1_reward1_Entered;

	private bool mMissionState1_reward2_Entered;

	private bool mMissionState1_reward3_Entered;

	private bool mMissionState3_ScaleWatingComplete;

	private bool mMissionState5_ScaleWatingComplete;

	private bool mMissionState6_IsFlashOver;

	private float m_MissionStateTime_Init = 0.4f;

	private float m_MissionStateTime_1 = 0.8f;

	private float m_MissionStateTime_2 = 0.8f;

	private float m_MissionStateTime_3 = 1.5f;

	private float m_MissionStateTime_4 = 1f;

	private float m_MissionStateTime_5 = 0.8f;

	private float m_MissionStateTime_6 = 1.2f;

	private float m_StateStayedInInit_Time;

	private float m_StateStayedIn1_Time;

	private float m_MissionStateTime_1_reward = 0.07f;

	private float m_MissionStateTime_3_WaitingScale = 1f;

	private float m_MissionStateTime_5_WaitingScale = 0.6f;

	private float m_MissionStateTime_6_FlashOverTime = 0.9f;

	private bool m_HaveFirstCompleteMission;

	private bool m_FirstTimeChestOpen;

	private LocalPlayer mPlayer;

	private UserState mUserState;

	private Mission[] mMissionArray;

	private string _mCurrentMoney;

	private int mNewMoney;

	private bool mHasLevelUp;

	private SwVoidDataManager.ESwVoidGetRewardState mNeedWaitSwVoidDrop;

	private bool mIsInited;

	private int mRewardShowIndex;

	private bool rateShowed;

	private int voidGetStringWaitingCount;

	private Timer addCountTimer = new Timer(1f, true);

	private PVEStageAnalysisInfo stageInfo;

	public int mStarCount { get; set; }

	public bool[] mStartStates { get; set; }

	private int mCurrentMoney
	{
		get
		{
			return AntiCracking.DecryptBufferStr(_mCurrentMoney, "shuliang");
		}
		set
		{
			_mCurrentMoney = AntiCracking.CryptBufferStr(value, "shuliang");
		}
	}

	private void Awake()
	{
		mInstance = this;
		mIsInited = false;
		mState = 0;
		NormalResultPage.SetActive(false);
		BossResultPage.SetActive(false);
		UnlockPage.gameObject.SetActive(false);
		mMissionArray = new Mission[m_MissionResults.Length];
		mPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		mUserState = GameApp.GetInstance().GetUserState();
		mUserState.MainMissionRewardGold = 0;
		mUserState.MainMissionRewardMithril = 0;
		if (HUDBase.instance != null && HUDBase.instance is HUDManager)
		{
			(HUDBase.instance as HUDManager).gameObject.SetActive(false);
		}
		if (UIRadar.mInstance != null)
		{
			UIRadar.mInstance.gameObject.SetActive(false);
		}
		if (!GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			mNeedWaitSwVoidDrop = SwVoidDataManager.TryGetRewardFromOnline();
			switch (mNeedWaitSwVoidDrop)
			{
			case SwVoidDataManager.ESwVoidGetRewardState.FIRST_BOSS:
				VoidGetString.gameObject.SetActive(false);
				break;
			case SwVoidDataManager.ESwVoidGetRewardState.GOT:
				VoidGetString.gameObject.SetActive(false);
				break;
			case SwVoidDataManager.ESwVoidGetRewardState.NO_CONNECT:
				VoidGetString.gameObject.SetActive(true);
				VoidGetString.text = LocalizationManager.GetInstance().GetString("LOC_VOID_GET_FAIL");
				break;
			case SwVoidDataManager.ESwVoidGetRewardState.NO_REWARD:
				VoidGetString.gameObject.SetActive(false);
				break;
			case SwVoidDataManager.ESwVoidGetRewardState.WAITING:
				VoidGetString.gameObject.SetActive(true);
				VoidGetString.text = string.Empty;
				break;
			}
		}
		else if (CoopResultPageUI.mInstance != null)
		{
			mNeedWaitSwVoidDrop = CoopResultPageUI.mInstance.GetNeedWaitSwVoidDrop();
		}
		bool flag = GameApp.GetInstance().GetGameMode().IsMultiPlayer();
		if (flag || GameApp.GetInstance().GetGameWorld().State == GameState.GameOverUIWin)
		{
			TryAgainButton.gameObject.SetActive(false);
		}
		BackButton.gameObject.SetActive(flag);
		CoopMithrilRewardObj.SetActive(flag);
		UIEventListener uIEventListener = UIEventListener.Get(FBButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		UIEventListener uIEventListener2 = UIEventListener.Get(TTButton);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		stageInfo = mUserState.AnalysisManager.GetCurrentPVEStageInfo();
		if (!AndroidConstant.isFacebook())
		{
			FBButton.SetActive(false);
		}
		TTButton.SetActive(false);
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	private void Start()
	{
	}

	private void InitResultPage()
	{
		mIsInited = true;
		mRewardShowIndex = 0;
		InitCommon();
		ContinueButton.GetComponent<Collider>().enabled = false;
		TryAgainButton.GetComponent<Collider>().enabled = false;
		BackButton.GetComponent<Collider>().enabled = false;
		switch (GameApp.GetInstance().GetGameMode().ModePlay)
		{
		case Mode.CamPain:
			VoidRewardObj.SetActive(false);
			GoldMithrilStatus.SetActive(false);
			TotalRewardMoney.gameObject.SetActive(false);
			TotalRewardMoneyEffect.gameObject.SetActive(false);
			ButtonsObj.SetActive(false);
			MoneyStatus.gameObject.SetActive(false);
			InitNormalResultPage();
			break;
		case Mode.BOSS_FINAL:
			VoidRewardObj.SetActive(true);
			GoldMithrilStatus.SetActive(true);
			TotalRewardMoney.gameObject.SetActive(true);
			TotalRewardMoneyEffect.gameObject.SetActive(true);
			ButtonsObj.SetActive(true);
			MoneyStatus.gameObject.SetActive(true);
			InitBossResultPage(false);
			break;
		case Mode.Survival:
		case Mode.Boss:
		case Mode.PRE_BOSS:
			VoidRewardObj.SetActive(true);
			GoldMithrilStatus.SetActive(true);
			TotalRewardMoney.gameObject.SetActive(true);
			TotalRewardMoneyEffect.gameObject.SetActive(true);
			ButtonsObj.SetActive(true);
			MoneyStatus.gameObject.SetActive(true);
			InitBossResultPage(true);
			break;
		}
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			CoopRewardMithril.text = mPlayer.GetBossMithril().ToString();
			CoopRewardMithrilEffect.text = CoopRewardMithril.text;
		}
		if (GameApp.GetInstance().GetGameMode().IsSingle())
		{
			CheckTrialWeaponPush();
		}
		VirtualCursorScript._ShowCursor();
	}

	public void InitSwVoidRewardInfo()
	{
		SwVoidGetRewardResponseData voidRewardData = GameApp.GetInstance().GetGameWorld().VoidRewardData;
		if (voidRewardData != null)
		{
			if (voidRewardData.result == SwErrorCode.SUCCESS)
			{
				Dictionary<short, int> dictionary = new Dictionary<short, int>();
				if (voidRewardData.materialTypeArray.Length != 0)
				{
					short[] materialTypeArray = voidRewardData.materialTypeArray;
					foreach (short num in materialTypeArray)
					{
						if (dictionary.ContainsKey(num))
						{
							Dictionary<short, int> dictionary3;
							Dictionary<short, int> dictionary2 = (dictionary3 = dictionary);
							short key2;
							short key = (key2 = num);
							int num2 = dictionary3[key2];
							dictionary2[key] = num2 + 1;
						}
						else
						{
							dictionary.Add(num, 1);
						}
					}
				}
				foreach (short key3 in dictionary.Keys)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(VoidRewardTemplate) as GameObject;
					gameObject.transform.parent = VoidRewards.transform;
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localRotation = Quaternion.identity;
					gameObject.transform.localScale = Vector3.one;
					SwVoidMaterialInfo component = gameObject.GetComponent<SwVoidMaterialInfo>();
					component.info.text = dictionary[key3].ToString();
					component.Icon.spriteName = SwVoidDataManager.GetMaterialIconName(key3);
					component.Rarity.spriteName = SwVoidDataManager.GetMaterialRarityName(key3);
				}
				VoidRewards.Reposition();
				mNeedWaitSwVoidDrop = SwVoidDataManager.ESwVoidGetRewardState.GOT;
			}
			else
			{
				mNeedWaitSwVoidDrop = SwVoidDataManager.ESwVoidGetRewardState.NO_CONNECT;
				VoidGetString.text = LocalizationManager.GetInstance().GetString("LOC_VOID_GET_FAIL");
				VoidGetString.gameObject.SetActive(true);
			}
			return;
		}
		VoidGetString.gameObject.SetActive(true);
		if (addCountTimer.Ready())
		{
			addCountTimer.Do();
			if (voidGetStringWaitingCount >= 6)
			{
				voidGetStringWaitingCount = 0;
			}
			else
			{
				voidGetStringWaitingCount++;
			}
		}
		string text = string.Empty;
		for (int j = 0; j < voidGetStringWaitingCount; j++)
		{
			text += ".";
		}
		VoidGetString.text = text;
	}

	public void InitCommon()
	{
		mStateStayedTime = 0f;
		MoneyStatus.Refresh();
		MoneyStatus.SetUpdate(false);
		SetKillRewards();
		UIEventListener uIEventListener = UIEventListener.Get(ContinueButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		UIEventListener uIEventListener2 = UIEventListener.Get(TryAgainButton);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		UIEventListener uIEventListener3 = UIEventListener.Get(BackButton);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		mHasLevelUp = false;
		SwVoidGetRewardResponseData voidRewardData = GameApp.GetInstance().GetGameWorld().VoidRewardData;
		if (voidRewardData != null)
		{
			InitSwVoidRewardInfo();
		}
	}

	public void InitNormalResultPage()
	{
		NormalResultPage.SetActive(true);
		MithrilRewardTitles.SetActive(true);
		MithrilRewardAlreadyGetTitles.SetActive(false);
		mStarCount = 0;
		mStartStates = new bool[m_MissionResults.Length];
		for (int i = 0; i < mStartStates.Length; i++)
		{
			mStartStates[i] = false;
		}
		SetMissionDescription();
	}

	public void InitBossResultPage(bool isSurvival)
	{
		if (isSurvival)
		{
			BossInfoObject.SetActive(false);
		}
		else if (GameApp.GetInstance().GetGameWorld().State != GameState.GameOverUIWin)
		{
			BossInfoObject.SetActive(false);
		}
		else
		{
			Mission currentStageMission = MissionManager.GetInstance().GetCurrentStageMission(0);
			if (currentStageMission != null && currentStageMission.Type == MissionType.BOSS)
			{
				GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.ENEMY_LIST, DataSheetName.ENEMY_OVERALL);
				EnemyType enemyType = ((MissionBoss)currentStageMission).GetBossList()[0].EnemyType;
				BossIcon.spriteName = gameDataTable.GetString((int)enemyType, 17);
				BossIcon.MakePixelPerfect();
				BossName.text = LocalizationManager.GetInstance().GetString(gameDataTable.GetString((int)enemyType, 18));
			}
		}
		List<KillEnemyInfo> sortList = mPlayer.PVEStatistics.GetSortList();
		for (int i = 0; i < sortList.Count && i < KillInfoPositions.Length; i++)
		{
			KillEnemyInfo killEnemyInfo = sortList[sortList.Count - 1 - i];
			if (!Enemy.IsBoss(killEnemyInfo.mEnemyType))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(EnemyKillInfoTemplate) as GameObject;
				gameObject.transform.parent = KillInfoInitPosition;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				TweenTransform component = gameObject.GetComponent<TweenTransform>();
				component.to = KillInfoPositions[i];
				component.delay = (float)i * 0.15f;
				EnemyKillInfoScript component2 = gameObject.GetComponent<EnemyKillInfoScript>();
				component2.Init(killEnemyInfo);
			}
		}
		SetMissionDescription();
		BossResultPage.SetActive(true);
		if (GameApp.GetInstance().GetGameWorld().State == GameState.GameOverUIWin && GameApp.GetInstance().GetGameWorld().GetFirstComplete() && mUserState.GetSelectedStageIndex() == 0)
		{
			TutorialManager.GetInstance().SetTutorialWatched(TutorialTriggerType.FIRST_BOSS, false);
		}
		if (CheckLosePackagePush())
		{
			TryAgainButton.gameObject.SetActive(false);
		}
	}

	public void SetKillRewards()
	{
		PickupComboReward.text = "+" + mPlayer.GetBonusCash();
		KillReward.text = "+" + mPlayer.GetPickupCash();
	}

	public void SetTotalReward()
	{
		TotalRewardMoney.text = (mCurrentMoney + mUserState.MainMissionRewardGold).ToString();
		mUserState.AddCash(mCurrentMoney);
		mUserState.Achievement.AddTotalCash(mCurrentMoney);
		stageInfo.goldCollect += mCurrentMoney;
		MoneyStatus.Refresh();
	}

	public void SetMissionDescription()
	{
		List<int> firstCompleteSubMissionIdList = MissionManager.GetInstance().GetFirstCompleteSubMissionIdList();
		bool flag = true;
		for (int i = 0; i < mMissionArray.Length; i++)
		{
			if (i >= Global.MISSION_SIZE)
			{
				continue;
			}
			mMissionArray[i] = MissionManager.GetInstance().GetCurrentStageMission(i);
			if (mMissionArray[i] != null)
			{
				bool flag2 = false;
				if (i == 0)
				{
					MithirlRewardAmount.text = mMissionArray[0].GetRewardMithril().ToString();
				}
				if (firstCompleteSubMissionIdList.Contains(i))
				{
					flag2 = true;
				}
				m_MissionResults[i].Init(mMissionArray[i], flag2);
				if (GameApp.GetInstance().GetGameWorld().State != GameState.GameOverUIWin || !mMissionArray[i].IsComplete() || flag2)
				{
					flag = (GameApp.GetInstance().GetGameWorld().State != GameState.GameOverUIWin || !mMissionArray[i].IsComplete() || !flag2) && false;
					continue;
				}
				MissionStars[i].spriteName = "star_u";
				mStarCount++;
			}
			else
			{
				flag = false;
				Debug.LogError("Mission is NULL, mission id = " + (i + 1));
			}
		}
		if (flag)
		{
			TreasureIcon.spriteName = "box_open";
			TreasureIcon.MakePixelPerfect();
			MithrilRewardObj.SetActive(false);
			MithrilRewardTitles.SetActive(false);
			MithrilRewardAlreadyGetTitles.SetActive(true);
		}
	}

	public void ReceiveSwVoidDropMessage()
	{
	}

	public void OnUpdate()
	{
		if (!mIsInited)
		{
			InitResultPage();
		}
		if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.CamPain)
		{
			switch (mState)
			{
			case 0:
				if (!mMissionStateInit_Entered)
				{
					if (mMissionArray[0].IsComplete())
					{
						m_MissionResults[0].ShowReward();
						if (m_MissionResults[0].IsFirstTimeCompleted())
						{
							mUserState.MainMissionRewardGold = mMissionArray[0].GetRewardCash();
							UpdateTotalMoneyRewardEffect(0, 0);
						}
					}
					CheckMissionState(1);
					CheckMissionState(2);
					CheckMissionState(3);
					CheckMissionState(4);
					mMissionStateInit_Entered = true;
					break;
				}
				if (!mMissionArray[0].IsComplete())
				{
					m_StateStayedInInit_Time += Time.deltaTime;
					if (!mMissionState1_reward1_Entered && m_StateStayedInInit_Time >= m_MissionStateTime_1_reward * 1f)
					{
						ShowReward(7);
						mMissionState1_reward1_Entered = true;
					}
					else if (!mMissionState1_reward2_Entered && m_StateStayedInInit_Time >= m_MissionStateTime_1_reward * 2f)
					{
						ShowReward(8);
						mMissionState1_reward2_Entered = true;
					}
					else if (!mMissionState1_reward3_Entered && m_StateStayedInInit_Time >= m_MissionStateTime_1_reward * 3f)
					{
						ShowReward(9);
						mMissionState1_reward3_Entered = true;
					}
				}
				mStateStayedTime += Time.deltaTime;
				if (mStateStayedTime >= m_MissionStateTime_Init)
				{
					if (mMissionArray[0].IsComplete())
					{
						mState = 21;
					}
					else
					{
						mState = 26;
					}
					mStateStayedTime = 0f;
				}
				break;
			case 21:
				if (!mMissionState1_Entered)
				{
					MissionStarsObj.SetActive(true);
					mMissionState1_Entered = true;
					break;
				}
				m_StateStayedIn1_Time += Time.deltaTime;
				if (!mMissionState1_reward3_Entered && m_StateStayedIn1_Time >= m_MissionStateTime_1_reward * 3f)
				{
					ShowReward(9);
					mMissionState1_reward3_Entered = true;
				}
				else if (!mMissionState1_reward2_Entered && m_StateStayedIn1_Time >= m_MissionStateTime_1_reward * 2f)
				{
					ShowReward(8);
					mMissionState1_reward2_Entered = true;
				}
				else if (!mMissionState1_reward1_Entered && m_StateStayedIn1_Time >= m_MissionStateTime_1_reward * 1f)
				{
					ShowReward(7);
					mMissionState1_reward1_Entered = true;
				}
				mStateStayedTime += Time.deltaTime;
				if (mStateStayedTime >= m_MissionStateTime_1)
				{
					mState = 22;
					mStateStayedTime = 0f;
				}
				break;
			case 22:
				if (!mMissionState2_Entered)
				{
					MissionResultUI[] missionResults = m_MissionResults;
					foreach (MissionResultUI missionResultUI in missionResults)
					{
						missionResultUI.StarBackgroundIcon.gameObject.SetActive(true);
					}
					mMissionState2_Entered = true;
				}
				else
				{
					mStateStayedTime += Time.deltaTime;
					if (mStateStayedTime >= m_MissionStateTime_2)
					{
						mState = 23;
						mStateStayedTime = 0f;
					}
				}
				break;
			case 23:
				if (!mMissionState3_Entered)
				{
					VoidRewardObj.SetActive(true);
					MissionResultUI[] missionResults2 = m_MissionResults;
					foreach (MissionResultUI missionResultUI2 in missionResults2)
					{
						if (missionResultUI2.IsFirstTimeCompleted())
						{
							missionResultUI2.StarForewardIcon.gameObject.SetActive(true);
							m_HaveFirstCompleteMission = true;
						}
					}
					mMissionState3_Entered = true;
					break;
				}
				mStateStayedTime += Time.deltaTime;
				if (!mMissionState3_ScaleWatingComplete && mStateStayedTime >= m_MissionStateTime_3_WaitingScale)
				{
					MissionResultUI[] missionResults3 = m_MissionResults;
					foreach (MissionResultUI missionResultUI3 in missionResults3)
					{
						if (missionResultUI3.IsFirstTimeCompleted())
						{
							missionResultUI3.StarBackgroundIcon.spriteName = "star_u";
						}
					}
					mMissionState3_ScaleWatingComplete = true;
				}
				else if (mStateStayedTime >= m_MissionStateTime_3)
				{
					if (m_HaveFirstCompleteMission)
					{
						mState = 24;
					}
					else
					{
						mState = 26;
					}
					mStateStayedTime = 0f;
				}
				break;
			case 24:
				if (!mMissionState4_Entered)
				{
					for (int j = 0; j < m_MissionResults.Length; j++)
					{
						if (m_MissionResults[j].IsFirstTimeCompleted())
						{
							MissionStarEffects[j].gameObject.SetActive(true);
							CheckMissionStarComplete();
						}
					}
					mMissionState4_Entered = true;
					break;
				}
				mStateStayedTime += Time.deltaTime;
				if (mStateStayedTime >= m_MissionStateTime_4)
				{
					if (m_FirstTimeChestOpen)
					{
						mState = 25;
					}
					else
					{
						mState = 26;
					}
					mStateStayedTime = 0f;
				}
				break;
			case 25:
				if (!mMissionState5_Entered)
				{
					TreasureEffect.gameObject.SetActive(true);
					mMissionState5_Entered = true;
					break;
				}
				mStateStayedTime += Time.deltaTime;
				if (!mMissionState5_ScaleWatingComplete && mStateStayedTime >= m_MissionStateTime_5_WaitingScale)
				{
					TreasureIcon.spriteName = "box_open";
					TreasureIcon.MakePixelPerfect();
					MithrilRewardTitles.SetActive(false);
					MithrilRewardAlreadyGetTitles.SetActive(true);
					mUserState.Achievement.AddAllStar();
					mUserState.MainMissionRewardMithril = mMissionArray[0].GetRewardMithril();
					mMissionState5_ScaleWatingComplete = true;
				}
				else if (mStateStayedTime >= m_MissionStateTime_5)
				{
					mState = 26;
					mStateStayedTime = 0f;
				}
				break;
			case 26:
				if (!mMissionState6_Entered)
				{
					GoldMithrilStatus.SetActive(true);
					mMissionState6_Entered = true;
					break;
				}
				mStateStayedTime += Time.deltaTime;
				if (!mMissionState6_IsFlashOver && mStateStayedTime >= m_MissionStateTime_6_FlashOverTime)
				{
					TotalRewardMoney.gameObject.SetActive(true);
					mMissionState6_IsFlashOver = true;
				}
				else if (mStateStayedTime >= m_MissionStateTime_6)
				{
					mState = 27;
					mStateStayedTime = 0f;
				}
				break;
			case 27:
				if (!mMissionState7_Entered)
				{
					ButtonsObj.SetActive(true);
					MoneyStatus.gameObject.SetActive(true);
					SetTotalReward();
					if (mPlayer.State == Player.WIN_STATE && GameApp.GetInstance().GetGameMode().IsSingle())
					{
						string dialogId = mUserState.GetSelectedStageIndex() + 1 + "-" + mUserState.GetSelectedSubStageIndex();
						List<UIDialogData> list = UserStateUI.GetInstance().StoryDialogManager.LoadDialog(dialogId);
						GameUIManager.GetInstance().LoadStoryDialog(list);
						AudioManager.GetInstance().PlaySound(AudioName.MENU_MISSION_DIALOGUE);
					}
					mMissionState7_Entered = true;
					if (CheckLosePackagePush())
					{
						TryAgainButton.gameObject.SetActive(false);
					}
				}
				break;
			}
		}
		else
		{
			mStateStayedTime += Time.deltaTime;
			if (mState != 11 && mStateStayedTime > mSwitchStateTime)
			{
				mStateStayedTime = 0f;
				SwitchState();
			}
			if (mNeedWaitSwVoidDrop == SwVoidDataManager.ESwVoidGetRewardState.WAITING)
			{
				InitSwVoidRewardInfo();
			}
		}
	}

	private void SwitchState()
	{
		AudioManager.GetInstance().PlaySound(AudioName.MENU_MISSION_END_SUM);
		switch (mState)
		{
		case 0:
			switch (GameApp.GetInstance().GetGameMode().ModePlay)
			{
			case Mode.CamPain:
				if (mMissionArray[0].IsComplete())
				{
					mState = 1;
					m_MissionResults[0].ShowReward();
					if (m_MissionResults[0].IsFirstTimeCompleted())
					{
						mUserState.MainMissionRewardGold = mMissionArray[0].GetRewardCash();
						UpdateTotalMoneyRewardEffect(0, 0);
					}
				}
				else
				{
					mState = 7;
					ShowReward(mState);
				}
				break;
			case Mode.Survival:
			case Mode.Boss:
			case Mode.PRE_BOSS:
				mState = 7;
				ShowReward(mState);
				break;
			case Mode.BOSS_FINAL:
				mState = 6;
				ShowReward(mState);
				break;
			}
			break;
		case 1:
			CheckMissionState(1);
			break;
		case 2:
			CheckMissionState(2);
			break;
		case 3:
			CheckMissionState(3);
			break;
		case 4:
			CheckMissionState(4);
			break;
		case 5:
			if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.BOSS_FINAL)
			{
				mState = 6;
				ShowReward(mState);
			}
			else
			{
				mState = 7;
				ShowReward(mState);
			}
			break;
		case 6:
			mState = 7;
			ShowReward(mState);
			break;
		case 7:
			mState = 8;
			ShowReward(mState);
			break;
		case 8:
			mState = 9;
			ShowReward(mState);
			break;
		case 9:
			mState = 11;
			SetTotalReward();
			if (mPlayer.State == Player.WIN_STATE && GameApp.GetInstance().GetGameMode().IsSingle() && !TutorialManager.GetInstance().Trigger(TutorialTriggerType.FIRST_BOSS))
			{
				string dialogId2 = mUserState.GetSelectedStageIndex() + 1 + "-" + mUserState.GetSelectedSubStageIndex();
				List<UIDialogData> list2 = UserStateUI.GetInstance().StoryDialogManager.LoadDialog(dialogId2);
				GameUIManager.GetInstance().LoadStoryDialog(list2);
				AudioManager.GetInstance().PlaySound(AudioName.MENU_MISSION_DIALOGUE);
			}
			break;
		case 10:
			mState = 11;
			SetTotalReward();
			if (mPlayer.State == Player.WIN_STATE && GameApp.GetInstance().GetGameMode().IsSingle())
			{
				string dialogId = mUserState.GetSelectedStageIndex() + 1 + "-" + mUserState.GetSelectedSubStageIndex();
				List<UIDialogData> list = UserStateUI.GetInstance().StoryDialogManager.LoadDialog(dialogId);
				GameUIManager.GetInstance().LoadStoryDialog(list);
				AudioManager.GetInstance().PlaySound(AudioName.MENU_MISSION_DIALOGUE);
			}
			UnlockButtons();
			break;
		case 11:
			break;
		}
	}

	private void CheckMissionState(int id)
	{
		if (id < mMissionArray.Length && mMissionArray[id].IsComplete() && id < m_MissionResults.Length)
		{
			m_MissionResults[id].ShowReward();
			if (m_MissionResults[id].IsFirstTimeCompleted())
			{
				UpdateTotalMoneyRewardEffect(mMissionArray[id].GetRewardCash(), mMissionArray[id].GetRewardMithril());
			}
		}
		if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.BOSS_FINAL)
		{
			mState = 6;
			ShowReward(mState);
			PickupComboReward.gameObject.SetActive(true);
			UpdateTotalMoneyRewardEffect(mPlayer.GetBonusCash(), 0);
		}
	}

	private void UpdateTotalMoneyRewardEffect(int gold, int mithril)
	{
		mCurrentMoney += gold;
		if (gold > 0)
		{
			TotalRewardMoney.text = (mCurrentMoney + mUserState.MainMissionRewardGold).ToString();
			TotalRewardMoneyEffect.text = TotalRewardMoney.text;
			TotalRewardMoneyEffect.GetComponent<TweenAlpha>().Reset();
			TotalRewardMoneyEffect.GetComponent<TweenAlpha>().PlayForward();
			TotalRewardMoneyEffect.GetComponent<TweenScale>().Reset();
			TotalRewardMoneyEffect.GetComponent<TweenScale>().PlayForward();
		}
		else if (mithril > 0)
		{
			if (!mUserState.AnalysisManager.IsMissionMithrilLimited())
			{
				mUserState.AddMithril(mithril);
				stageInfo.mithrilCollect += mithril;
			}
			MoneyStatus.Refresh();
		}
	}

	public void Exit()
	{
		Time.timeScale = 1f;
		Lobby.GetInstance().IsMasterPlayer = false;
		if (mUserState.GetPlayTimesAfterIgnoreRating() >= 0)
		{
			mUserState.AddPlayTimesAfterIgnoreRating();
		}
		if (mUserState.GetPlayTimesAfterIgnoreRating() >= 10 && !rateShowed)
		{
			rateShowed = true;
			UIMsgBox.instance.ShowReview(this);
			return;
		}
		if (GameApp.GetInstance().GetGameWorld().State == GameState.GameOverUIWin)
		{
			if (GameApp.GetInstance().GetGameMode().IsSingle())
			{
				CheckWinPackagePush();
				UserState userState = GameApp.GetInstance().GetUserState();
				int num = userState.GetSelectedStageIndex();
				if (num < Global.TOTAL_STAGE)
				{
					int selectedSubStageIndex = userState.GetSelectedSubStageIndex();
					EDifficultMode eDifficultMode = userState.GetSelectedDifficultMode();
					if (selectedSubStageIndex == Global.BOSS_FINAL_SUB_STAGE_ID)
					{
						mUserState.Achievement.AddMissionPlayed(eDifficultMode, num, Global.BOSS_SUB_STAGE_ID, true);
					}
					else
					{
						mUserState.Achievement.AddMissionPlayed(eDifficultMode, num, selectedSubStageIndex, true);
					}
					stageInfo.playTime++;
					stageInfo.passTime++;
					mUserState.AnalysisManager.SetCurrentPlayRecord();
					switch (eDifficultMode)
					{
					case EDifficultMode.NORMAL:
						mUserState.Achievement.CheckAchievement_Mission(num);
						break;
					case EDifficultMode.HEROIC:
						mUserState.Achievement.CheckAchievement_HeroMission(num);
						break;
					}
					bool flag = false;
					selectedSubStageIndex++;
					if (selectedSubStageIndex == Global.TOTAL_SUB_STAGE)
					{
						int num2 = (num + 1) % Global.TOTAL_ACTIVATED_STAGE;
						Debug.Log("nextStage = " + num2);
						if (num + 1 < Global.TOTAL_ACTIVATED_STAGE)
						{
							num++;
							selectedSubStageIndex = 1;
						}
						else
						{
							selectedSubStageIndex = Global.BOSS_SUB_STAGE_ID;
						}
						flag = true;
					}
					if (num > userState.GetUnlockedStageId())
					{
						userState.SetUnlockedStageId((byte)num);
						userState.SetUnlockedSubStageId((byte)selectedSubStageIndex);
					}
					else if (num == userState.GetUnlockedStageId() && selectedSubStageIndex > userState.GetUnlockedSubStageId())
					{
						userState.SetUnlockedSubStageId((byte)selectedSubStageIndex);
					}
					if (num - 1 == Global.IRREGULAR_STAGE_ID && flag)
					{
						switch (eDifficultMode)
						{
						case EDifficultMode.NORMAL:
							num = 0;
							selectedSubStageIndex = 1;
							eDifficultMode = EDifficultMode.HEROIC;
							break;
						case EDifficultMode.HEROIC:
							selectedSubStageIndex = 1;
							eDifficultMode = EDifficultMode.HEROIC;
							break;
						}
						userState.SetSelectedDifficultMode(eDifficultMode);
						if (userState.GetUnlockedDifficultMode() == EDifficultMode.NORMAL)
						{
							userState.SetUnlockedDifficultMode(eDifficultMode);
							userState.SetUnlockedStageId((byte)num);
							userState.SetUnlockedSubStageId((byte)selectedSubStageIndex);
						}
					}
					userState.SetSelectedStageIndex((byte)num);
					userState.SetSelectedSubStageIndex((byte)selectedSubStageIndex);
					userState.UnlockEquipsByLevel();
				}
			}
			else
			{
				UserState userState2 = GameApp.GetInstance().GetUserState();
				userState2.SetSelectedStageIndex(0);
				userState2.SetSelectedSubStageIndex((byte)Global.SURVIVAL_SUB_STAGE_ID);
			}
			mUserState.Achievement.AddWeaponWinWith();
		}
		else if (GameApp.GetInstance().GetGameWorld().State == GameState.GameOverUILose)
		{
			if (GameApp.GetInstance().GetGameMode().IsSingle())
			{
				UserState userState3 = GameApp.GetInstance().GetUserState();
				int num3 = userState3.GetSelectedSubStageIndex();
				int selectedStageIndex = userState3.GetSelectedStageIndex();
				EDifficultMode selectedDifficultMode = userState3.GetSelectedDifficultMode();
				if (num3 == Global.BOSS_FINAL_SUB_STAGE_ID)
				{
					num3 = Global.BOSS_SUB_STAGE_ID;
					userState3.SetSelectedSubStageIndex((byte)Global.BOSS_SUB_STAGE_ID);
				}
				mUserState.Achievement.AddMissionPlayed(selectedDifficultMode, selectedStageIndex, num3, false);
				stageInfo.playTime++;
				mUserState.AnalysisManager.SetCurrentPlayRecord();
			}
			else
			{
				UserState userState4 = GameApp.GetInstance().GetUserState();
				userState4.SetSelectedStageIndex(0);
				userState4.SetSelectedSubStageIndex((byte)Global.SURVIVAL_SUB_STAGE_ID);
			}
		}
		mUserState.Achievement.AddWeaponUsed();
		mUserState.Achievement.SetInfiniteKillRecord();
		if (GameApp.GetInstance().GetGameMode().IsSingle())
		{
			mUserState.AddCash(mUserState.MainMissionRewardGold);
			mUserState.Achievement.AddTotalCash(mUserState.MainMissionRewardGold);
			stageInfo.goldCollect += mUserState.MainMissionRewardGold;
			mUserState.MainMissionRewardGold = 0;
			if (!mUserState.AnalysisManager.IsMissionMithrilLimited())
			{
				mUserState.AddMithril(mUserState.MainMissionRewardMithril);
				stageInfo.mithrilCollect += mUserState.MainMissionRewardMithril;
			}
			mUserState.MainMissionRewardMithril = 0;
		}
		GameApp.GetInstance().Save();
		if (GameApp.GetInstance().GetGameWorld().State == GameState.GameOverUIWin && !GameApp.GetInstance().GetUserState().GetOnlineRegisted() && !TutorialManager.GetInstance().GetTutorialWatched(TutorialTriggerType.REGISTER_TUTORIAL))
		{
			GameWorld.LoadLevel("MainMenu", ELevelType.MENU);
		}
		else if (GameApp.GetInstance().GetGameMode().IsSingle())
		{
			LoadingUIScript.mInstance.ShowAfterMissionTip(GameApp.GetInstance().GetGameWorld().State, mMissionArray[0].Type, false, mHasLevelUp);
			GameWorld.LoadLevel("ShopAndCustomize2", ELevelType.MENU);
		}
		else
		{
			LoadingUIScript.mInstance.ShowAfterMissionTip(GameApp.GetInstance().GetGameWorld().State, mMissionArray[0].Type, false, mHasLevelUp);

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
            LeaveRoomRequest request = new LeaveRoomRequest();
            GameApp.GetInstance().GetNetworkManager().SendRequest(request);
            GameWorld.LoadLevel("MultiMenu", ELevelType.MENU);
        }
        else
        {
            GameWorld.LoadLevel("MainMenu", ELevelType.MENU);
        }
    }

    private void OnClickButton(GameObject go)
	{
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
		if (go == ContinueButton)
		{
			if (mState == 11 || mState == 27)
			{
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
				if (ShowUnlockPage())
				{
					return;
				}
				bool flag = false;
				if (GameApp.GetInstance().GetGameMode().IsSingle() && GameApp.GetInstance().GetGameWorld().State == GameState.GameOverUILose && GameApp.GetInstance().GetGameMode().ModePlay != Mode.Survival && HUDBase.instance as HUDManager != null && !(HUDBase.instance as HUDManager).IsSelfExit && !IsGoToRookiePackage && !IsGoToExpertPackage)
				{
					mMissionFailTimeDuringAppRun++;
					Debug.Log(mMissionFailTimeDuringAppRun);
					if (mMissionFailTimeDuringAppRun >= 3)
					{
						flag = true;
						mMissionFailTimeDuringAppRun = 0;
					}
				}
				if (flag)
				{
					string empty = string.Empty;
					empty = ((UnityEngine.Random.Range(0, 2) != 0) ? LocalizationManager.GetInstance().GetString("LOC_FAIL_STORE_2") : LocalizationManager.GetInstance().GetString("LOC_FAIL_STORE_1"));
					UIMsgBox.instance.ShowMessage(this, empty, 2, 27);
				}
				else
				{
					Exit();
				}
			}
			else if (GameApp.GetInstance().GetGameMode().ModePlay != Mode.CamPain)
			{
				SwitchState();
			}
		}
		else if (go == TryAgainButton)
		{
			UserState userState = GameApp.GetInstance().GetUserState();
			if (GameApp.GetInstance().GetGameMode().IsSingle())
			{
				int num = userState.GetSelectedSubStageIndex();
				int selectedStageIndex = userState.GetSelectedStageIndex();
				EDifficultMode selectedDifficultMode = userState.GetSelectedDifficultMode();
				if (num == Global.BOSS_FINAL_SUB_STAGE_ID)
				{
					num = Global.BOSS_SUB_STAGE_ID;
					userState.SetSelectedSubStageIndex((byte)Global.BOSS_SUB_STAGE_ID);
					GameApp.GetInstance().GetGameMode().ModePlay = Mode.PRE_BOSS;
				}
				mUserState.Achievement.AddMissionPlayed(selectedDifficultMode, selectedStageIndex, num, false);
				stageInfo.playTime++;
				mUserState.AnalysisManager.SetCurrentPlayRecord();
			}
			mUserState.Achievement.AddWeaponUsed();
			string levelName = GameApp.GetInstance().GetUserState().GetLevelConfig(userState.GetSelectedStageIndex(), userState.GetSelectedSubStageIndex())
				.levelName;
			MissionSelectUIScript.ShowLoadingPage(userState.GetSelectedDifficultMode(), userState.GetSelectedStageIndex(), userState.GetSelectedSubStageIndex());
			GameWorld.LoadLevel(levelName, ELevelType.GAME);
		}
		else if (go == BackButton)
		{
			if (CoopResultPageUI.mInstance != null)
			{
				CoopResultPageUI.mInstance.gameObject.SetActive(true);
			}
			base.gameObject.SetActive(false);
		}
		else if (go == FBButton)
		{
			if (GameApp.GetInstance().IsConnectedToInternet())
			{
				ScreenCapture.CaptureScreenshot("tempscreens.png");
				if (!FacebookTools.isSessionValid())
				{
					ApplicationPauseManager.mInstance.IsPostingToFacebook = true;
					ApplicationPauseManager.mInstance.currentType = ApplicationPauseManager.EPostingType.PVE_SCORE;
					FacebookTools.CallFBLogin();
				}
				else
				{
					ApplicationPauseManager.mInstance.StartCoroutine(ApplicationPauseManager.mInstance.postToFacebook(ApplicationPauseManager.EPostingType.PVE_SCORE));
				}
			}
		}
		else if (go == TTButton && GameApp.GetInstance().IsConnectedToInternet())
		{
			ScreenCapture.CaptureScreenshot("tempscreens.png");
			ApplicationPauseManager.mInstance.StartCoroutine(ApplicationPauseManager.mInstance.postToTwitter(ApplicationPauseManager.EPostingType.PVE_SCORE));
		}
	}

	public void CheckMissionStarComplete()
	{
		mStarCount++;
		bool flag = true;
		if (mStarCount < 5)
		{
			flag = false;
		}
		if (flag)
		{
			m_FirstTimeChestOpen = true;
		}
	}

	private void ShowReward(int state)
	{
		if (mRewardShowIndex < RewardInitPosition.Length)
		{
			TweenTransform tweenTransform = null;
			int num = 0;
			switch (state)
			{
			case 6:
				tweenTransform = BossRewardObj;
				num = mPlayer.GetBossCash();
				UpdateTotalMoneyRewardEffect(num, 0);
				mUserState.AddMithril(mPlayer.GetBossMithril());
				stageInfo.mithrilCollect += mPlayer.GetBossMithril();
				BossReward.text = num.ToString();
				break;
			case 7:
				tweenTransform = KillRewardObj;
				num = mPlayer.GetMonsterCash();
				UpdateTotalMoneyRewardEffect(num, 0);
				KillReward.text = num.ToString();
				break;
			case 8:
				tweenTransform = PickupComboRewardObj;
				num = mPlayer.GetPickupCash() + mPlayer.GetBonusCash();
				UpdateTotalMoneyRewardEffect(num, 0);
				PickupComboReward.text = num.ToString();
				break;
			case 9:
				tweenTransform = EquipRewardObj;
				num = (int)((float)(mPlayer.GetBossCash() + mPlayer.GetMonsterCash() + mPlayer.GetPickupCash() + mPlayer.GetBonusCash()) * mPlayer.GetSkills().GetSkill(SkillsType.MONEY_BOOTH));
				UpdateTotalMoneyRewardEffect(num, 0);
				EquipReward.text = num.ToString();
				break;
			case 10:
				tweenTransform = FriendRewardObj;
				UpdateTotalMoneyRewardEffect(0, 0);
				FriendReward.text = "0";
				break;
			}
			if (tweenTransform != null)
			{
				tweenTransform.from = RewardInitPosition[mRewardShowIndex];
				tweenTransform.to = RewardFinalPosition[mRewardShowIndex];
				tweenTransform.enabled = true;
				tweenTransform.Reset();
				tweenTransform.PlayForward();
			}
			mRewardShowIndex++;
		}
	}

	public bool ShowUnlockPage()
	{
		if (GameApp.GetInstance().GetGameWorld().State == GameState.GameOverUIWin && GameApp.GetInstance().GetGameMode().IsSingle())
		{
			UserState userState = GameApp.GetInstance().GetUserState();
			if (userState.GetSelectedDifficultMode() == userState.GetUnlockedDifficultMode() && userState.GetSelectedStageIndex() == userState.GetUnlockedStageId(userState.GetSelectedDifficultMode()) && userState.GetSelectedSubStageIndex() >= userState.GetUnlockedSubStageId(userState.GetSelectedDifficultMode()))
			{
				int selectedStageIndex = userState.GetSelectedStageIndex();
				if (selectedStageIndex < Global.TOTAL_STAGE)
				{
					int selectedSubStageIndex = userState.GetSelectedSubStageIndex();
					if (selectedSubStageIndex == Global.BOSS_FINAL_SUB_STAGE_ID)
					{
						EDifficultMode selectedDifficultMode = userState.GetSelectedDifficultMode();
						UnlockPage.OpenByStageClear(selectedDifficultMode, (byte)selectedStageIndex, (byte)selectedSubStageIndex);
						return true;
					}
				}
			}
		}
		return false;
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (whichMsg.EventId == 6)
		{
			UserState userState = GameApp.GetInstance().GetUserState();
			switch (buttonId)
			{
			case UIMsg.UIMsgButton.Ok:
				switch (AndroidConstant.version)
				{
				case AndroidConstant.Version.GooglePlay:
					AndroidPluginScript.ShowAppUrl(0);
					break;
				case AndroidConstant.Version.Kindle:
					AndroidPluginScript.ShowAppUrl(1);
					break;
				}
				userState.ResetPlayTimesAfterIgnoreRating(false);
				UIMsgBox.instance.CloseMessage();
				break;
			case UIMsg.UIMsgButton.Cancel:
				userState.ResetPlayTimesAfterIgnoreRating(true);
				UIMsgBox.instance.CloseMessage();
				break;
			case UIMsg.UIMsgButton.Never:
				userState.ResetPlayTimesAfterIgnoreRating(false);
				UIMsgBox.instance.CloseMessage();
				break;
			}
		}
		else if (whichMsg.EventId == 27)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				IsGoToShop = true;
			}
			UIMsgBox.instance.CloseMessage();
			Exit();
		}
		else if (whichMsg.EventId == 43)
		{
			UIMsgBox.instance.CloseMessage();
		}
	}

	private void CheckWinPackagePush()
	{
		if (mUserState.GetSelectedDifficultMode() != EDifficultMode.NORMAL || mUserState.GetSelectedSubStageIndex() != Global.PACKAGE_PUSH_SUB_STAGE)
		{
			return;
		}
		if (mUserState.GetSelectedStageIndex() <= Global.ROOKIE_PACKAGE_PUSH_STAGE)
		{
			if (!mUserState.GetRookiePackBought())
			{
				if (!mUserState.GetIsWinRookiePush() && mUserState.GetBestAvatarIndex() <= Global.ROOKIE_PACKAGE_AVATAR_INDEX)
				{
					IsGoToRookiePackage = true;
					mUserState.SetIsWinRookiePush(true);
				}
			}
			else if (!mUserState.GetExpertPackBought() && !mUserState.GetIsWinExpertPush() && mUserState.GetBestAvatarIndex() <= Global.EXPERT_PACKAGE_AVATAR_INDEX)
			{
				IsGoToExpertPackage = true;
				mUserState.SetIsWinExpertPush(true);
			}
		}
		else if (mUserState.GetSelectedStageIndex() <= Global.EXPERT_PACKAGE_PUSH_STAGE && !mUserState.GetIsWinExpertPush() && !mUserState.GetExpertPackBought() && mUserState.GetBestAvatarIndex() <= Global.EXPERT_PACKAGE_AVATAR_INDEX)
		{
			IsGoToExpertPackage = true;
			mUserState.SetIsWinExpertPush(true);
		}
	}

	private bool CheckLosePackagePush()
	{
		if (!GameApp.GetInstance().IsConnectedToInternet())
		{
			return false;
		}
		bool result = false;
		if (GameApp.GetInstance().GetGameMode().IsSingle() && GameApp.GetInstance().GetGameWorld().State == GameState.GameOverUILose && GameApp.GetInstance().GetGameMode().ModePlay != Mode.Survival && mUserState.GetSelectedDifficultMode() == EDifficultMode.NORMAL && HUDBase.instance as HUDManager != null && !(HUDBase.instance as HUDManager).IsSelfExit)
		{
			if (mUserState.GetSelectedStageIndex() <= Global.ROOKIE_PACKAGE_PUSH_STAGE)
			{
				if (!mUserState.GetRookiePackBought())
				{
					if (mUserState.GetLoseRookiePushTimes() < Global.ROOKIE_PACKAGE_LOSE_PUSH_TIMES && mUserState.GetBestAvatarIndex() <= Global.ROOKIE_PACKAGE_AVATAR_INDEX)
					{
						IsGoToRookiePackage = true;
						result = true;
						mUserState.SetLoseRookiePushTimes(mUserState.GetLoseRookiePushTimes() + 1);
					}
				}
				else if (!mUserState.GetExpertPackBought() && mUserState.GetLoseExpertPushTimes() < Global.EXPERT_PACKAGE_LOSE_PUSH_TIMES && mUserState.GetBestAvatarIndex() <= Global.EXPERT_PACKAGE_AVATAR_INDEX)
				{
					IsGoToExpertPackage = true;
					result = true;
					mUserState.SetLoseExpertPushTimes(mUserState.GetLoseExpertPushTimes() + 1);
				}
			}
			else if (mUserState.GetSelectedStageIndex() <= Global.EXPERT_PACKAGE_PUSH_STAGE && mUserState.GetLoseExpertPushTimes() < Global.EXPERT_PACKAGE_LOSE_PUSH_TIMES && !mUserState.GetExpertPackBought() && mUserState.GetBestAvatarIndex() <= Global.EXPERT_PACKAGE_AVATAR_INDEX)
			{
				IsGoToExpertPackage = true;
				result = true;
				mUserState.SetLoseExpertPushTimes(mUserState.GetLoseExpertPushTimes() + 1);
			}
		}
		return result;
	}

	public void CheckTrialWeaponPush()
	{
		if (mUserState.GetTrialWeaponID() > 0 && mUserState.GetTrialWeaponBuyID() > 0)
		{
			IsGoToTrialWeaponBuy = true;
		}
		else
		{
			IsGoToTrialWeaponBuy = false;
		}
		mUserState.ClearTrialWeapon();
	}

	public void UnlockButtons()
	{
		ContinueButton.GetComponent<Collider>().enabled = true;
		TryAgainButton.GetComponent<Collider>().enabled = true;
		BackButton.GetComponent<Collider>().enabled = true;
		if (GameApp.GetInstance().GetUserState().GetSelectedDifficultMode() == EDifficultMode.NORMAL && GameApp.GetInstance().GetUserState().GetSelectedStageIndex() == 0 && GameApp.GetInstance().GetUserState().GetSelectedSubStageIndex() != Global.BOSS_FINAL_SUB_STAGE_ID && GameApp.GetInstance().GetGameMode().IsSingle() && GameApp.GetInstance().GetGameWorld().State == GameState.GameOverUILose && HUDBase.instance as HUDManager != null && !(HUDBase.instance as HUDManager).IsSelfExit)
		{
			if (GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.inputController.NeedShowContorlTip())
			{
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("TIPS_UPDATE11_2"), 1, 43);
			}
			else if (!MissionManager.GetInstance().IsCurrentMissionKillingEnemy() && GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.Kills != 0)
			{
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("TIPS_UPDATE11_1"), 1, 43);
			}
		}
	}
}
