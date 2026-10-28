using System;
using UnityEngine;

public class UIDailyQuestInfo : MonoBehaviour
{
	public enum EDailyQuestState
	{
		NO_QUEST = 0,
		NOT_FINISH = 1,
		FINISHED_NOT_RECIEVED_REWARD = 2,
		RECIEVED_REWARD = 3
	}

	public GameObject button;

	public UILabel buttonLable;

	public UILabel questLable;

	public UISprite hasDone;

	public UILabel hasGotValue;

	public UISprite goldIcon;

	public UILabel goldText;

	private DailyQuest mQuest;

	public EDailyQuestState mQuestState
	{
		get
		{
			if (mQuest == null)
			{
				return EDailyQuestState.NO_QUEST;
			}
			if (mQuest.hasRewardReceived == 1)
			{
				return EDailyQuestState.RECIEVED_REWARD;
			}
			if (mQuest.hasDone == 1)
			{
				return EDailyQuestState.FINISHED_NOT_RECIEVED_REWARD;
			}
			return EDailyQuestState.NOT_FINISH;
		}
	}

	private void Awake()
	{
		UIEventListener uIEventListener = UIEventListener.Get(button);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
	}

	public void Init(DailyQuest quest)
	{
		mQuest = quest;
		questLable.text = quest.GetDes();
		if (quest.hasDone == 0)
		{
			hasDone.spriteName = "star_n";
		}
		else
		{
			hasDone.spriteName = "star_u";
		}
		if (quest.value == 0)
		{
			hasGotValue.gameObject.SetActive(false);
		}
		else
		{
			hasGotValue.gameObject.SetActive(true);
			hasGotValue.text = quest.valueGot.ToString() + "/" + quest.value;
		}
		RefreshButton();
	}

	public void OnButtonClick(GameObject go)
	{
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
		if (!(go == button))
		{
			return;
		}
		switch (mQuestState)
		{
		case EDailyQuestState.NOT_FINISH:
			UIDailyBonus.mInstance.gameObject.SetActive(false);
			if (LobbyScript.mInstance != null)
			{
				AutoCreateRoom();
			}
			break;
		case EDailyQuestState.FINISHED_NOT_RECIEVED_REWARD:
			GetQuestGoldReward();
			break;
		}
	}

	public void RefreshButton()
	{
		switch (mQuestState)
		{
		case EDailyQuestState.NO_QUEST:
			buttonLable.gameObject.SetActive(false);
			goldIcon.gameObject.SetActive(false);
			goldText.gameObject.SetActive(false);
			break;
		case EDailyQuestState.NOT_FINISH:
			buttonLable.gameObject.SetActive(true);
			button.GetComponent<Collider>().enabled = true;
			buttonLable.text = LocalizationManager.GetInstance().GetString("LOC_ICON_24");
			goldIcon.gameObject.SetActive(false);
			goldText.gameObject.SetActive(false);
			break;
		case EDailyQuestState.FINISHED_NOT_RECIEVED_REWARD:
			buttonLable.gameObject.SetActive(true);
			button.GetComponent<Collider>().enabled = true;
			buttonLable.text = LocalizationManager.GetInstance().GetString("LOC_QIANDAO_BUTTON_1");
			goldIcon.gameObject.SetActive(true);
			goldText.gameObject.SetActive(true);
			goldText.text = (mQuest.rewardPoint * 500).ToString();
			break;
		case EDailyQuestState.RECIEVED_REWARD:
			buttonLable.gameObject.SetActive(true);
			button.GetComponent<Collider>().enabled = false;
			buttonLable.text = LocalizationManager.GetInstance().GetString("LOC_QIANDAO_BUTTON_2");
			goldIcon.gameObject.SetActive(false);
			goldText.gameObject.SetActive(false);
			break;
		}
	}

	public void AutoCreateRoom()
	{
		if (mQuest.type == 1 || mQuest.type == 2)
		{
			VSSelectionScript pVE = LobbyScript.mInstance.PVE;
			pVE.OnButtonClick(pVE.CreateGameButton);
			pVE.RoomUI.InitMapsFocusOn(GetCoopMapIndex());
		}
		else if (mQuest.type == 3)
		{
			VSSelectionScript pVP = LobbyScript.mInstance.PVP;
			pVP.OnButtonClick(pVP.CreateGameButton);
		}
	}

	public void GetQuestGoldReward()
	{
		TryGetQuestGoldRequest request = new TryGetQuestGoldRequest(mQuest.index);
		GameApp.GetInstance().GetNetworkManager().SendRequest(request);
	}

	private int GetCoopMapIndex()
	{
		int questId = mQuest.questId;
		byte isHeroic = mQuest.isHeroic;
		Mode mode = ((mQuest.type != 1) ? Mode.Survival : Mode.Boss);
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.LEVEL, DataSheetName.COOP_LEVEL_NAME);
		for (int i = 0; i < gameDataTable.Count; i++)
		{
			if (gameDataTable.GetInt(i, 1) == isHeroic && gameDataTable.GetInt(i, 3) == questId && mode == (Mode)gameDataTable.GetInt(i, 2))
			{
				return gameDataTable.GetInt(i, 0);
			}
		}
		return 0;
	}
}
