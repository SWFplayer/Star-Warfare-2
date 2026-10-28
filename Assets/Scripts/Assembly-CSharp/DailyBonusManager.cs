using System.Collections.Generic;
using UnityEngine;

public class DailyBonusManager
{
	public enum EDailyBonusRewardType
	{
		NONE = 0,
		GOLD = 1,
		MITHRIL = 2,
		SW_VOID_MATERIAL = 3,
		POTION = 4,
		FLASK = 5,
		SP_SW_VOID_MATERIAL = 6
	}

	public enum EDailyLoginColumn
	{
		DAYS = 0,
		REWARD_TYPE = 1,
		VALUE = 2
	}

	public enum ERewardSourceType
	{
		DAILY_BONUS = 0,
		DAILY_TASK = 1,
		DAILY_TASK_FINAL = 2
	}

	private static DailyBonusManager mInstance;

	private bool ReceivedhasLoginReward;

	private byte mLoginDays;

	protected List<SwDailyReward> loginRewardList;

	private List<DailyQuest> dailyQuestList;

	protected List<SwDailyReward> rewardList;

	protected SwAnnouncement currentSwAnnouncement;

	public bool HasNewAnnouncement { get; set; }

	public static DailyBonusManager GetInstance()
	{
		if (mInstance == null)
		{
			mInstance = new DailyBonusManager();
		}
		return mInstance;
	}

	public void SetLoginDays(byte days)
	{
		mLoginDays = days;
	}

	public byte GetLoginDays()
	{
		return mLoginDays;
	}

	public void SetLoginRewardList(List<SwDailyReward> list)
	{
		loginRewardList = list;
	}

	public List<SwDailyReward> GetLoginRewardList()
	{
		return loginRewardList;
	}

	public void SetHasRecievedReward(byte hasRecieved)
	{
		if (hasRecieved == 0)
		{
			ReceivedhasLoginReward = false;
		}
		else
		{
			ReceivedhasLoginReward = true;
		}
	}

	public bool GetHasRecievedReward()
	{
		return ReceivedhasLoginReward;
	}

	public void SetInfoToUI(UIDailyLoginReward dailyLoginUI)
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.DAILY_BONUS, DataSheetName.DAILY_LOGIN);
		int mIndex = dailyLoginUI.mIndex;
		switch (gameDataTable.GetInt(mIndex, 1))
		{
		case 1:
			dailyLoginUI.SetIcon("LVE1");
			dailyLoginUI.SetValue(gameDataTable.GetInt(mIndex, 2));
			break;
		case 2:
			dailyLoginUI.SetIcon("LVE");
			dailyLoginUI.SetValue(gameDataTable.GetInt(mIndex, 2));
			break;
		default:
			dailyLoginUI.SetIcon("itembox");
			dailyLoginUI.SetValue(1);
			break;
		}
		if (mIndex == mLoginDays - 1)
		{
			dailyLoginUI.SetButton(ReceivedhasLoginReward);
		}
		else
		{
			dailyLoginUI.InactiveButton();
		}
	}

	public void SetInfoToUIRewardInfo(UIDailyRewardInfo uiRewardInfo, byte rewardType, short rewardId, int rewardValue)
	{
		switch (rewardType)
		{
		case 1:
			uiRewardInfo.SetIcon("LVE1", rewardType);
			uiRewardInfo.SetName(LocalizationManager.GetInstance().GetString("LOC_M_GOLD"));
			uiRewardInfo.SetValue(rewardValue);
			uiRewardInfo.SetRarity(false, string.Empty);
			break;
		case 2:
			uiRewardInfo.SetIcon("LVE", rewardType);
			uiRewardInfo.SetName(LocalizationManager.GetInstance().GetString("LOC_M_Mithril"));
			uiRewardInfo.SetValue(rewardValue);
			uiRewardInfo.SetRarity(false, string.Empty);
			break;
		case 3:
			uiRewardInfo.SetIcon(SwVoidDataManager.GetMaterialIconName(rewardId), rewardType);
			uiRewardInfo.SetName(SwVoidDataManager.GetMaterialName(rewardId));
			uiRewardInfo.SetValue(rewardValue);
			uiRewardInfo.SetRarity(true, SwVoidDataManager.GetMaterialRarityName(rewardId));
			break;
		case 4:
		{
			Item itemById2 = GameApp.GetInstance().GetUserState().GetItemById(rewardId);
			if (itemById2 != null)
			{
				uiRewardInfo.SetIcon(itemById2.GetIconName(), rewardType);
				uiRewardInfo.SetName(itemById2.GetItemName());
			}
			else
			{
				uiRewardInfo.SetIcon(string.Empty, rewardType);
				uiRewardInfo.SetName(string.Empty);
			}
			uiRewardInfo.SetValue(rewardValue);
			uiRewardInfo.SetRarity(false, string.Empty);
			break;
		}
		case 5:
		{
			Item itemById = GameApp.GetInstance().GetUserState().GetItemById(rewardId);
			if (itemById != null)
			{
				uiRewardInfo.SetIcon(itemById.GetIconName(), rewardType);
				string key = "LOC_QIANDAO_FLASK_" + rewardId;
				uiRewardInfo.SetName(LocalizationManager.GetInstance().GetString(key));
			}
			else
			{
				uiRewardInfo.SetIcon(string.Empty, rewardType);
				uiRewardInfo.SetName(string.Empty);
			}
			uiRewardInfo.SetValue(rewardValue);
			uiRewardInfo.SetRarity(false, string.Empty);
			break;
		}
		case 6:
			uiRewardInfo.SetIcon(SwVoidDataManager.GetMaterialIconName(rewardId), rewardType);
			uiRewardInfo.SetName(SwVoidDataManager.GetMaterialName(rewardId));
			uiRewardInfo.SetValue(rewardValue);
			uiRewardInfo.SetRarity(true, SwVoidDataManager.GetMaterialRarityName(rewardId));
			break;
		}
	}

	public void RecievedQuestFinalRewards(List<SwDailyReward> rewardList)
	{
		this.rewardList = rewardList;
		foreach (SwDailyReward reward in rewardList)
		{
			AddReward(reward.GetRewardType(), reward.GetRewardId(), reward.GetRewardValue(), ERewardSourceType.DAILY_TASK_FINAL);
		}
		if (UIDailyBonus.mInstance.gameObject.activeSelf)
		{
			UIDailyBonus.mInstance.ShowRewardInfo(rewardList);
		}
	}

	public void ReceivedLoginReward(List<SwDailyReward> rewardList)
	{
		foreach (SwDailyReward reward in rewardList)
		{
			AddReward(reward.GetRewardType(), reward.GetRewardId(), reward.GetRewardValue(), ERewardSourceType.DAILY_BONUS);
		}
		if (UIDailyBonus.mInstance.gameObject.activeSelf)
		{
			UIDailyBonus.mInstance.ShowRewardInfo(rewardList);
			UIDailyBonus.mInstance.ShowLoginRewardReceived(mLoginDays);
		}
		GetInstance().SetHasRecievedReward(1);
		if (LobbyScript.mInstance != null)
		{
			if (GetInstance().GetHasRecievedReward())
			{
				LobbyScript.mInstance.DailyBonusAlpha.enabled = false;
			}
			else
			{
				LobbyScript.mInstance.DailyBonusAlpha.enabled = true;
			}
		}
	}

	public void AddReward(byte rewardType, short rewardId, int rewardValue, ERewardSourceType type)
	{
		switch (rewardType)
		{
		case 1:
		{
			GameApp.GetInstance().GetUserState().AddCash(rewardValue);
			if (type == ERewardSourceType.DAILY_BONUS)
			{
				GameApp.GetInstance().GetUserState().AnalysisManager.goldInfo.DailyBonusGold += rewardValue;
			}
			else
			{
				GameApp.GetInstance().GetUserState().AnalysisManager.goldInfo.DailyTaskGold += rewardValue;
			}
			SyncCashRequest request2 = new SyncCashRequest();
			GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
			break;
		}
		case 2:
		{
			GameApp.GetInstance().GetUserState().AddMithril(rewardValue);
			if (type == ERewardSourceType.DAILY_BONUS)
			{
				GameApp.GetInstance().GetUserState().AnalysisManager.mithrilInfo.DailyBonusMithril += rewardValue;
			}
			else
			{
				GameApp.GetInstance().GetUserState().AnalysisManager.mithrilInfo.DailyTaskMithril += rewardValue;
			}
			SyncMithrilRequest request = new SyncMithrilRequest();
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			break;
		}
		case 3:
			break;
		case 4:
		{
			Item itemById2 = GameApp.GetInstance().GetUserState().GetItemById(rewardId);
			itemById2.AddCount(rewardValue);
			break;
		}
		case 5:
		{
			Item itemById = GameApp.GetInstance().GetUserState().GetItemById(rewardId);
			itemById.AddCount(rewardValue);
			break;
		}
		case 6:
			break;
		}
	}

	public void ReceivedReward(byte rewardType, short rewardId, int rewardValue)
	{
		if (UIDailyBonus.mInstance.gameObject.activeSelf)
		{
			UIDailyBonus.mInstance.ShowRewardInfo(rewardType, rewardId, rewardValue);
		}
		AddReward(rewardType, rewardId, rewardValue, ERewardSourceType.DAILY_TASK);
	}

	public void SetQuestList(List<DailyQuestData> dailyQuestDataList)
	{
		dailyQuestList = new List<DailyQuest>();
		for (int i = 0; i < dailyQuestDataList.Count; i++)
		{
			DailyQuest dailyQuest = new DailyQuest(dailyQuestDataList[i]);
			dailyQuestList.Add(dailyQuest);
			Debug.Log("add quest" + dailyQuest.type);
		}
	}

	public void SetQuestInfo(DailyQuestData questData)
	{
		if (questData.index < dailyQuestList.Count)
		{
			if (dailyQuestList[questData.index] != null)
			{
				dailyQuestList[questData.index].SetData(questData);
				return;
			}
			DailyQuest value = new DailyQuest(questData);
			dailyQuestList[questData.index] = value;
		}
	}

	public List<DailyQuest> GetQuestList()
	{
		return dailyQuestList;
	}

	public void CheckBossQuest(EnemyType bossType, byte isHeroic)
	{
		byte b = (byte)(bossType - 9 + 1);
		if (dailyQuestList == null)
		{
			return;
		}
		foreach (DailyQuest dailyQuest in dailyQuestList)
		{
			if (dailyQuest.type == 1 && dailyQuest.hasDone == 0 && dailyQuest.questId == b && isHeroic == dailyQuest.isHeroic)
			{
				DailyQuestBossCheckRequest request = new DailyQuestBossCheckRequest(dailyQuest.index, b, isHeroic);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
	}

	public void SetSwAnnouncement(SwAnnouncement swAnnouncement)
	{
		currentSwAnnouncement = swAnnouncement;
		if (GameApp.GetInstance().GetUserState().lastAnnouncementId != swAnnouncement.GetId())
		{
			HasNewAnnouncement = true;
			GameApp.GetInstance().GetUserState().lastAnnouncementId = swAnnouncement.GetId();
			if (LobbyScript.mInstance != null && !LobbyScript.mInstance.IsShowExtraGoldAndMithril)
			{
				AutoShowInUI(UIDailyBonus.EDailyBonusState.ANNOUNCEMENT);
			}
		}
	}

	public string GetAnnouncement()
	{
		if (currentSwAnnouncement == null)
		{
			return string.Empty;
		}
		return currentSwAnnouncement.GetAnnouncement();
	}

	public void AutoShowInUI(UIDailyBonus.EDailyBonusState state)
	{
		if (LobbyScript.mInstance != null)
		{
			LobbyScript.mInstance.OpenDailyBonusPage();
			Debug.Log(UIDailyBonus.mInstance);
			if (UIDailyBonus.mInstance != null)
			{
				UIDailyBonus.mInstance.ChangePage(state);
			}
		}
	}
}
