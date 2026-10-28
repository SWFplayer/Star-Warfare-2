using System;
using System.Collections.Generic;
using UnityEngine;

public class MapPanelScript : MonoBehaviour, UIMsgListener
{
	public GameObject m_LevelButtonsObj;

	public GameObject m_LevelButtonsTemplate;

	public UISprite StatusSprite;

	public UILabel m_TitleLabel;

	public UILabel m_StoryLabel;

	public MissionIntroScript Objectives;

	public TweenAlpha m_PanelTweenAlpha;

	public TweenAlpha m_MapSpriteTweenAlpha;

	public DifficultySelectScript DifficultySelection;

	public GameObject SummonPartnerPage;

	public GameObject OfflinePartnerButton;

	public UISprite OfflinePartnerHighlight;

	public GameObject OnlineLobbyButton;

	public UISprite OnlineLobbyHighlight;

	public GameObject BestRecordInfo;

	public UILabel BestRecordValue;

	public GameObject MapStarsObj;

	public UILabel StarCount;

	public GameObject LevelTotalRewardObj;

	public UILabel LevelRewardLabel;

	public UIGrid GoldHintObject;

	public GameObject GoldHintTemplate;

	public UIGrid MaterialHintObject;

	public GameObject MaterialHintTemplate;

	[HideInInspector]
	public List<LevelButtonScript> m_LevelButtons = new List<LevelButtonScript>();

	[HideInInspector]
	public UIPanel m_Panel;

	[HideInInspector]
	public UITexture m_MapSprite;

	private TweenTransform m_TweenTransform;

	private float m_ControlFactor;

	private GameObject m_MapImage;

	public int m_MapID { get; set; }

	private void Awake()
	{
		m_TweenTransform = GetComponent<TweenTransform>();
		m_Panel = GetComponent<UIPanel>();
		DifficultySelection.gameObject.SetActive(false);
		UIEventListener uIEventListener = UIEventListener.Get(OfflinePartnerButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(OnlineLobbyButton);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(OfflinePartnerButton);
		uIEventListener3.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener3.onPress, new UIEventListener.BoolDelegate(OnButtonPressed));
		UIEventListener uIEventListener4 = UIEventListener.Get(OnlineLobbyButton);
		uIEventListener4.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener4.onPress, new UIEventListener.BoolDelegate(OnButtonPressed));
		OnlineLobbyHighlight.gameObject.SetActive(false);
		OfflinePartnerHighlight.gameObject.SetActive(false);
		SummonPartnerPage.gameObject.SetActive(false);
		BestRecordInfo.SetActive(false);
	}

	private void OnDrag(Vector2 delta)
	{
		MissionSelectUIScript.m_Instance.Dragging(delta.x);
	}

	public void DragStart(Transform from, Transform to, float initFactor)
	{
		m_TweenTransform.from = from;
		m_TweenTransform.to = to;
		m_TweenTransform.enabled = false;
		m_TweenTransform.Sample(initFactor, false);
	}

	public void DragStop(Transform from, Transform to, bool forward)
	{
		m_TweenTransform.from = from;
		m_TweenTransform.to = to;
		m_TweenTransform.Reset();
		m_TweenTransform.Play(forward);
	}

	public void Dragging(float factor)
	{
		m_TweenTransform.Sample(factor, false);
	}

	private void OnPress(bool isPressed)
	{
		if (isPressed)
		{
			MissionSelectUIScript.m_Instance.DragStart();
		}
		else
		{
			MissionSelectUIScript.m_Instance.DragStop();
		}
	}

	public void Init(int mapID)
	{
		m_MapID = mapID;
		LevelConfig levelConfig = GameApp.GetInstance().GetUserState().GetLevelConfig(m_MapID, 1);
		if (m_MapImage != null)
		{
			MemoryManager.FreeNGUI(m_MapImage);
		}
		string text = "map11";
		GameObject gameObject = Resources.Load("UI/Common/MiniMaps/" + levelConfig.mapName) as GameObject;
		if (gameObject == null)
		{
			gameObject = Resources.Load("UI/Common/MiniMaps/" + text) as GameObject;
		}
		m_MapImage = UnityEngine.Object.Instantiate(gameObject) as GameObject;
		m_MapImage.SetActive(true);
		m_MapImage.transform.parent = m_MapSpriteTweenAlpha.transform;
		m_MapImage.transform.localPosition = Vector3.zero;
		m_MapImage.transform.localRotation = Quaternion.identity;
		m_MapImage.transform.localScale = Vector3.one * 0.5f;
		m_MapSprite = m_MapImage.GetComponent<UITexture>();
		if (mapID != Global.TOTAL_ACTIVATED_STAGE)
		{
			m_LevelButtons.Clear();
			for (int i = 0; i < Global.BOSS_FINAL_SUB_STAGE_ID; i++)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(m_LevelButtonsTemplate) as GameObject;
				gameObject2.transform.parent = m_LevelButtonsObj.transform;
				gameObject2.transform.localPosition = Vector3.zero;
				gameObject2.transform.localRotation = Quaternion.identity;
				gameObject2.transform.localScale = Vector3.one;
				LevelButtonScript component = gameObject2.GetComponent<LevelButtonScript>();
				component.Init(m_MapID, i, this);
				m_LevelButtons.Add(component);
			}
			m_LevelButtonsObj.GetComponent<UIGrid>().Reposition();
		}
		else
		{
			m_TitleLabel.text = "Coming Soon...";
			DifficultySelection.gameObject.SetActive(false);
		}
	}

	public void CancelLevelButtons()
	{
		foreach (LevelButtonScript levelButton in m_LevelButtons)
		{
			levelButton.Cancel();
		}
	}

	public void SelectLevelButton()
	{
		if (MissionSelectUIScript.m_Instance.m_SeletecedLevelID < m_LevelButtons.Count)
		{
			m_LevelButtons[MissionSelectUIScript.m_Instance.m_SeletecedLevelID].OnClick();
		}
	}

	public TweenTransform GetTweenTransform()
	{
		return m_TweenTransform;
	}

	public void SetTweenAlphaAtStart(float delay)
	{
		m_PanelTweenAlpha.delay = delay;
		m_MapSpriteTweenAlpha.delay = delay;
		m_PanelTweenAlpha.enabled = true;
		m_MapSpriteTweenAlpha.enabled = true;
	}

	public void SetAlphas(float alpha)
	{
		m_Panel.alpha = alpha;
		m_MapSpriteTweenAlpha.GetComponent<UIPanel>().alpha = alpha;
	}

	public void ResetMissionInfos(int stageID, int subStageID)
	{
		int num = 0;
		for (int i = 1; i < Global.BOSS_SUB_STAGE_ID; i++)
		{
			Mission[] array = new Mission[Global.MISSION_SIZE];
			for (int j = 0; j < Global.MISSION_SIZE; j++)
			{
				array[j] = MissionManager.GetInstance().GetMission((byte)stageID, (byte)i, j, MissionSelectUIScript.m_Instance.m_difficultMode);
				if (array[j].IsComplete())
				{
					num++;
				}
			}
		}
		StarCount.text = num.ToString();
		if (subStageID == Global.SURVIVAL_SUB_STAGE_ID || subStageID == Global.BOSS_SUB_STAGE_ID || MissionSelectUIScript.m_Instance.m_SeletecedLevelID == Global.SURVIVAL_SUB_STAGE_ID || MissionSelectUIScript.m_Instance.m_SeletecedLevelID == Global.BOSS_SUB_STAGE_ID)
		{
			if (Objectives.gameObject.activeSelf)
			{
				Objectives.gameObject.SetActive(false);
			}
			if (!SummonPartnerPage.activeSelf)
			{
				SummonPartnerPage.SetActive(true);
			}
			if (LevelTotalRewardObj.activeSelf)
			{
				LevelTotalRewardObj.SetActive(false);
			}
			BestRecordInfo.SetActive(subStageID == Global.SURVIVAL_SUB_STAGE_ID);
			if (MissionSelectUIScript.m_Instance.m_difficultMode == EDifficultMode.NORMAL)
			{
				BestRecordValue.text = GameApp.GetInstance().GetUserState().Achievement.GetInfiniteKills()[stageID].ToString();
			}
			else if (MissionSelectUIScript.m_Instance.m_difficultMode == EDifficultMode.HEROIC)
			{
				BestRecordValue.text = GameApp.GetInstance().GetUserState().Achievement.GetHeroInfiniteKills()[stageID].ToString();
			}
			if (stageID < Global.TOTAL_STAGE && subStageID < Global.TOTAL_SUB_STAGE)
			{
				LevelConfig levelConfig = GameApp.GetInstance().GetUserState().GetLevelConfig(stageID, subStageID);
				if (levelConfig != null)
				{
					string locId = levelConfig.locId;
					m_StoryLabel.text = LocalizationManager.GetInstance().GetString(locId);
				}
			}
			string text = subStageID.ToString();
			if (subStageID == Global.SURVIVAL_SUB_STAGE_ID)
			{
				text = LocalizationManager.GetInstance().GetString("LOC_MISSION_STAGE_EXPLORE");
			}
			else if (subStageID == Global.BOSS_SUB_STAGE_ID)
			{
				text = LocalizationManager.GetInstance().GetString("LOC_MISSION_STAGE_BOSS");
			}
			m_TitleLabel.text = LocalizationManager.GetInstance().GetString("LOC_MISSION_STAGE") + " " + (MissionSelectUIScript.m_Instance.m_SeletecedMapID + 1) + " - " + text;
			MissionSelectUIScript.m_Instance.m_OKLabel.text = LocalizationManager.GetInstance().GetString("LOC_BUTTON_SOLO_2");
		}
		else
		{
			if (SummonPartnerPage.activeSelf)
			{
				SummonPartnerPage.SetActive(false);
			}
			if (BestRecordInfo.activeSelf)
			{
				BestRecordInfo.SetActive(false);
			}
			if (!Objectives.gameObject.activeSelf)
			{
				Objectives.gameObject.SetActive(true);
			}
			Objectives.SetMissionDescription();
			int allStarReward = Objectives.GetAllStarReward();
			if (allStarReward > 0)
			{
				if (!LevelTotalRewardObj.activeSelf)
				{
					LevelTotalRewardObj.SetActive(true);
				}
				LevelRewardLabel.text = allStarReward.ToString();
			}
			else
			{
				LevelTotalRewardObj.SetActive(false);
			}
			if (stageID < Global.TOTAL_STAGE && subStageID < Global.TOTAL_SUB_STAGE)
			{
				LevelConfig levelConfig2 = GameApp.GetInstance().GetUserState().GetLevelConfig(stageID, subStageID);
				if (levelConfig2 != null)
				{
					string locId2 = levelConfig2.locId;
					m_StoryLabel.text = LocalizationManager.GetInstance().GetString(locId2);
				}
			}
			MissionSelectUIScript.m_Instance.m_OKLabel.text = LocalizationManager.GetInstance().GetString("LOC_BUTTON_SOLO_1");
		}
		MissionState missionState = GameApp.GetInstance().GetUserState().GetNormalMissionState();
		if (MissionSelectUIScript.m_Instance.m_difficultMode == EDifficultMode.HEROIC)
		{
			missionState = GameApp.GetInstance().GetUserState().GetHeroicMissionState();
		}
		if (subStageID != Global.SURVIVAL_SUB_STAGE_ID)
		{
			StatusSprite.gameObject.SetActive(true);
			if (stageID < missionState.GetUnlockedStageId())
			{
				StatusSprite.spriteName = "clear";
			}
			else if (stageID == missionState.GetUnlockedStageId())
			{
				if (subStageID < missionState.GetUnlockedSubStageId())
				{
					StatusSprite.spriteName = "clear";
				}
				else if (subStageID == missionState.GetUnlockedSubStageId())
				{
					StatusSprite.spriteName = "new";
				}
				else
				{
					StatusSprite.spriteName = "clear1";
				}
			}
			else
			{
				StatusSprite.spriteName = "clear1";
			}
			ClearGoldHint();
			ClearMaterialHint();
		}
		else
		{
			StatusSprite.gameObject.SetActive(false);
			ClearGoldHint();
			ShowGoldHint(stageID);
			ClearMaterialHint();
			ShowMaterialHint(stageID, subStageID);
		}
	}

	public void ChangeDifficulty(EDifficultMode diffMode)
	{
		MissionSelectUIScript.m_Instance.m_difficultMode = diffMode;
		MissionSelectUIScript.m_Instance.m_SeletecedLevelID = 1;
		foreach (MapPanelScript mapPanel in MissionSelectUIScript.m_Instance.m_MapPanels)
		{
			if (mapPanel.m_MapID == Global.TOTAL_ACTIVATED_STAGE)
			{
				continue;
			}
			mapPanel.CancelLevelButtons();
			foreach (LevelButtonScript levelButton in mapPanel.m_LevelButtons)
			{
				levelButton.Init(mapPanel.m_MapID, levelButton.m_LevelID, mapPanel);
			}
			mapPanel.ResetMissionInfos(mapPanel.m_MapID, 1);
		}
	}

	private void OnButtonClick(GameObject go)
	{
		if (go == OfflinePartnerButton)
		{
			if (m_LevelButtons[MissionSelectUIScript.m_Instance.m_SeletecedLevelID].IsLocked())
			{
				MissionSelectUIScript.m_Instance.LockMsg();
			}
			else if (GameApp.GetInstance().GetUserState().HasLoginOnline())
			{
				GameObject original = Resources.Load("UI/Menu/FriendUI/Prefab/FriendUI") as GameObject;
				GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
				gameObject.transform.localPosition = new Vector3(0f, -1000f, 0f);
				FriendUI component = gameObject.GetComponent<FriendUI>();
				component.SetForOfflineSummon();
			}
			else
			{
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_2"), 8, 23);
			}
		}
		else if (!(go == OnlineLobbyButton))
		{
		}
	}

	private void OnButtonPressed(GameObject go, bool isPressed)
	{
		if (go == OfflinePartnerButton)
		{
			OfflinePartnerHighlight.gameObject.SetActive(isPressed);
		}
		else if (go == OnlineLobbyButton)
		{
			OnlineLobbyHighlight.gameObject.SetActive(isPressed);
		}
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (whichMsg.EventId == 23)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				UIMsgBox.instance.CloseMessage();
				LoginPageScript.mTurnToRegister = true;
				GameWorld.LoadLevel("MainMenu", ELevelType.MENU);
			}
			else
			{
				UIMsgBox.instance.CloseMessage();
			}
		}
	}

	private void ClearGoldHint()
	{
		int childCount = GoldHintObject.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			UnityEngine.Object.DestroyImmediate(GoldHintObject.transform.GetChild(0).gameObject);
		}
		GoldHintObject.gameObject.SetActive(false);
	}

	private void ShowGoldHint(int stageID)
	{
		GoldHintObject.gameObject.SetActive(true);
		int num = stageID / 2 + 1;
		if (MissionSelectUIScript.m_Instance.m_difficultMode == EDifficultMode.HEROIC)
		{
			num = (stageID + 1 + Global.IRREGULAR_STAGE_ID + 1 + 2) / 2;
		}
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(GoldHintTemplate) as GameObject;
			gameObject.transform.parent = GoldHintObject.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
		}
		GoldHintObject.Reposition();
	}

	private void ClearMaterialHint()
	{
		if (MaterialHintObject != null)
		{
			int childCount = MaterialHintObject.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				UnityEngine.Object.DestroyImmediate(MaterialHintObject.transform.GetChild(0).gameObject);
			}
			MaterialHintObject.gameObject.SetActive(false);
		}
	}

	private void ShowMaterialHint(int stageID, int subStageID)
	{
		if (!(MaterialHintObject != null))
		{
			return;
		}
		MaterialHintObject.gameObject.SetActive(true);
		EDifficultMode difficultMode = MissionSelectUIScript.m_Instance.m_difficultMode;
		foreach (short materialDrop in SwVoidDataManager.GetMaterialDrops(difficultMode, stageID + 1, subStageID))
		{
			Debug.Log("material id : " + materialDrop);
			GameObject gameObject = UnityEngine.Object.Instantiate(MaterialHintTemplate) as GameObject;
			gameObject.transform.parent = MaterialHintObject.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one * 0.8f;
			SwVoidMaterialInfo component = gameObject.GetComponent<SwVoidMaterialInfo>();
			component.info.gameObject.SetActive(false);
			component.materialId = materialDrop;
			component.Rarity.spriteName = SwVoidDataManager.GetMaterialRarityName(materialDrop);
			component.Icon.spriteName = SwVoidDataManager.GetMaterialIconName(materialDrop);
		}
		MaterialHintObject.Reposition();
	}
}
