using UnityEngine;

public class LoadingUIScript : MonoBehaviour
{
	public static LoadingUIScript mInstance;

	public UILabel Fixed_Tip;

	public UILabel Random_Tip;

	public UISprite BlackBack;

	public UISprite[] LoadingSprites;

	public GameObject EffectObject;

	private bool mIsLoading;

	private float mLeastWaitingTime = 2f;

	private float mFadeOutTime = 0.2f;

	private float mStartLoadingTime;

	private int mFirstLoadingSpriteIndex;

	private float mLoadingSpriteUpdateInterval = 0.07f;

	private float mLoadingSpriteUpdateTime;

	private string[] mAfterMissionConditions;

	private string[] mAfterMissionTips;

	private int[] mAfterMissionCoolDowns;

	private int[] mAfterMissionTipCounts;

	private void Awake()
	{
		mInstance = this;
		mIsLoading = false;
		base.gameObject.SetActive(false);
		Object.DontDestroyOnLoad(base.gameObject);
		float num = (float)Screen.width / (float)Screen.height;
		if (num > 1.3333334f)
		{
			Fixed_Tip.width = Screen.width - 70;
		}
		else
		{
			Fixed_Tip.width = Screen.width - 222;
		}
		InitAfterMissionTips();
	}

	private void Destroy()
	{
		mInstance = null;
	}

	public void StartLoading(string fixedTip, string randomTip)
	{
		EffectObject.SetActive(true);
		Fixed_Tip.text = LocalizationManager.GetInstance().GetString(fixedTip);
		Random_Tip.text = GetWrapString(LocalizationManager.GetInstance().GetString(randomTip), 762, Random_Tip);
		mIsLoading = true;
		mStartLoadingTime = Time.time;
		BlackBack.alpha = 1f;
		mFirstLoadingSpriteIndex = 0;
		SetLoadingSprites(mFirstLoadingSpriteIndex++);
		VirtualCursorScript._HideCursor();
		base.gameObject.SetActive(true);
	}

	public void LoadingFinished()
	{
		Fixed_Tip.text = string.Empty;
		Random_Tip.text = string.Empty;
		mIsLoading = false;
		VirtualCursorScript._ShowCursor();
		base.gameObject.SetActive(false);
		if (!(InterMissionScript.GetInstance() != null))
		{
			return;
		}
		if (ResultPageUI.IsGoToRookiePackage || ResultPageUI.IsGoToExpertPackage)
		{
			if (ResultPageUI.IsGoToRookiePackage)
			{
				InterMissionScript.GetInstance().ShowPackagePush(false);
			}
			else
			{
				InterMissionScript.GetInstance().ShowPackagePush(true);
			}
			ResultPageUI.IsGoToShop = false;
			ResultPageUI.IsGoToIap = false;
			ResultPageUI.IsGoToRookiePackage = false;
			ResultPageUI.IsGoToExpertPackage = false;
		}
		else if (UnlockListPageScript.IsGoToCustomize || ResultPageUI.IsGoToShop)
		{
			InterMissionScript.GetInstance().ChangePage(InterMissionPageType.Shop);
			UnlockListPageScript.IsGoToCustomize = false;
			ResultPageUI.IsGoToShop = false;
			if (ResultPageUI.IsGoToIap)
			{
				ResultPageUI.IsGoToIap = false;
				if (IapScript.mInstance != null)
				{
					IapScript.mInstance.OpenPage(IapScript.IapType.Nothing);
					IapScript.mInstance.IsGotoEquipmentSuitPackage = true;
					IapScript.mInstance.OpenPage(IapScript.IapType.Package);
				}
			}
		}
		else if (ResultPageUI.IsGoToTrialWeaponBuy)
		{
			InterMissionScript.GetInstance().ShowTrialWeaponBuy();
			ResultPageUI.IsGoToTrialWeaponBuy = false;
		}
	}

	private void Update()
	{
		if (Time.time - mLoadingSpriteUpdateTime >= mLoadingSpriteUpdateInterval)
		{
			SetLoadingSprites(mFirstLoadingSpriteIndex++);
		}
		if (mIsLoading && GameApp.GetInstance().GetLevelManager().IsLoadingFinished() && Time.time - mStartLoadingTime >= mLeastWaitingTime)
		{
			if (EffectObject.activeSelf)
			{
				EffectObject.SetActive(false);
			}
			BlackBack.alpha -= RealTime.deltaTime / mFadeOutTime;
			if (BlackBack.alpha <= 0f)
			{
				LoadingFinished();
			}
		}
	}

	private void SetLoadingSprites(int firstIndex)
	{
		int num = 0;
		for (int i = 0; i < LoadingSprites.Length; i++)
		{
			num = (firstIndex + i) % UIConstant.LOADING_SPRITE_NAMES.Length;
			if (UIConstant.LOADING_SPRITE_NAMES[num].Equals("loading_null"))
			{
				LoadingSprites[i].gameObject.SetActive(false);
				continue;
			}
			LoadingSprites[i].gameObject.SetActive(true);
			LoadingSprites[i].spriteName = UIConstant.LOADING_SPRITE_NAMES[num];
			LoadingSprites[i].MakePixelPerfect();
		}
		mLoadingSpriteUpdateTime = Time.time;
	}

	public string GetRandomTip()
	{
		string empty = string.Empty;
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.TIP_LIST, DataSheetName.TIP_RANDOM);
		int row = Random.Range(0, gameDataTable.Count - 1);
		return gameDataTable.GetString(row, 1);
	}

	private void InitAfterMissionTips()
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.TIP_LIST, DataSheetName.TIP_AFTER);
		mAfterMissionConditions = new string[gameDataTable.Count];
		mAfterMissionTips = new string[gameDataTable.Count];
		mAfterMissionCoolDowns = new int[gameDataTable.Count];
		mAfterMissionTipCounts = new int[gameDataTable.Count];
		for (int i = 0; i < gameDataTable.Count; i++)
		{
			mAfterMissionConditions[i] = gameDataTable.GetString(i, 0);
			mAfterMissionTips[i] = gameDataTable.GetString(i, 1);
			mAfterMissionCoolDowns[i] = gameDataTable.GetInt(i, 2);
			mAfterMissionTipCounts[i] = gameDataTable.GetInt(i, 2);
		}
	}

	public void ShowAfterMissionTip(GameState gameState, MissionType missionType, bool isTimeUpFail, bool isLevelUp)
	{
		string text = string.Empty;
		bool flag = false;
		for (int i = 0; i < mAfterMissionConditions.Length; i++)
		{
			bool flag2 = false;
			bool flag3 = true;
			if (mAfterMissionTipCounts[i] >= mAfterMissionCoolDowns[i] && !flag)
			{
				if (mAfterMissionConditions[i].Contains("Fail") && gameState != GameState.GameOverUILose)
				{
					flag3 = false;
				}
				if (flag3)
				{
					if (mAfterMissionConditions[i].Contains("Boss") && missionType != MissionType.BOSS && missionType != MissionType.PRE_BOSS)
					{
						flag3 = false;
					}
					if (mAfterMissionConditions[i].Contains("Protect") && missionType != MissionType.PROTECTION && missionType != MissionType.PROTECTION_MOVE)
					{
						flag3 = false;
					}
					if (mAfterMissionConditions[i].Contains("KillArea") && missionType != MissionType.KILL_ENEMIES_AREA)
					{
						flag3 = false;
					}
				}
				if (flag3)
				{
					if (mAfterMissionConditions[i].Contains("TimerUp") && !isTimeUpFail)
					{
						flag3 = false;
					}
					if (mAfterMissionConditions[i].Equals("Shop.Newgun") && GameApp.GetInstance().GetUserState().GetWeaponListForUnLock()
						.Count <= 0)
					{
						flag3 = false;
					}
					if (mAfterMissionConditions[i].Equals("Shop.NewgunUpgrade") && !GameApp.GetInstance().GetUserState().HaveWeaponToUpgrade())
					{
						flag3 = false;
					}
					if (mAfterMissionConditions[i].Equals("Shop.NewMission") && (GameApp.GetInstance().GetUserState().GetUnlockedDifficultMode() == EDifficultMode.NORMAL || GameApp.GetInstance().GetUserState().GetUnlockedStageId() < Global.TOTAL_ACTIVATED_STAGE))
					{
						flag3 = false;
					}
					if (mAfterMissionConditions[i].Equals("Shop.NoNewMission") && GameApp.GetInstance().GetUserState().GetUnlockedDifficultMode() == EDifficultMode.HEROIC && GameApp.GetInstance().GetUserState().GetUnlockedStageId() == Global.TOTAL_ACTIVATED_STAGE)
					{
						flag3 = false;
					}
				}
				if (flag3)
				{
					flag2 = true;
				}
			}
			if (flag2)
			{
				text = LocalizationManager.GetInstance().GetString(mAfterMissionTips[i]);
				mAfterMissionTipCounts[i] = 0;
				flag = true;
			}
			else
			{
				mAfterMissionTipCounts[i]++;
			}
		}
		if (!flag)
		{
			text = GetRandomTip();
		}
		string randomTip = GetRandomTip();
		while (randomTip.Equals(text))
		{
			randomTip = GetRandomTip();
		}
		StartLoading(text, randomTip);
	}

	public bool IsLoading()
	{
		return mIsLoading;
	}

	private string GetWrapString(string text, int length, UILabel label)
	{
		EUserLanguage userLanguage = GameApp.GetInstance().GetUserState().GetUserLanguage();
		int num = 0;
		bool flag = userLanguage == EUserLanguage.CHINESE_SIMPLIFIED || userLanguage == EUserLanguage.CHINESE_TRADITIONAL || userLanguage == EUserLanguage.KOREAN || userLanguage == EUserLanguage.JAPANESE;
		num = ((!flag) ? (label.fontSize / 2) : label.fontSize);
		int num2 = 0;
		for (int i = 0; i < text.Length; i++)
		{
			num2 += num;
			if (num2 < length)
			{
				continue;
			}
			if (text[i] != ' ' && !flag)
			{
				while (text[i] != ' ' && text.Substring(0, i).Contains(" "))
				{
					i--;
				}
			}
			text = text.Insert(i, "\n");
			i++;
			num2 = num;
		}
		return text;
	}
}
