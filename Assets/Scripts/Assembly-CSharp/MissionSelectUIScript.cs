using System;
using System.Collections.Generic;
using UnityEngine;

public class MissionSelectUIScript : MonoBehaviour, UIMsgListener
{
	public static MissionSelectUIScript m_Instance;

	public Transform PanelFrontPosition;

	public Transform PanelLeftPosition;

	public Transform PanelRightPosition;

	public Transform PanelBackPosition;

	public Transform PanelFrontInitPosition;

	public Transform PanelLeftInitPosition;

	public Transform PanelRightInitPosition;

	public Transform MapsInitPosition;

	public Transform MapsPosition;

	public Transform LeftClockwisePosition;

	public Transform LeftAntiClockwisePosition;

	public Transform FrontClockwisePosition;

	public Transform FrontAntiClockwisePosition;

	public GameObject MoneyStatusObj;

	public float m_DragSpeed = 0.1f;

	public GameObject m_MapsObj;

	public GameObject m_MapPanelTemplate;

	public GameObject m_MapBarsObj;

	public GameObject m_MapBarTemplate;

	public GameObject m_OKButton;

	public UILabel m_OKLabel;

	public float NotFrontPanelAlpha = 0.5f;

	[HideInInspector]
	[SerializeField]
	public List<MapPanelScript> m_MapPanels = new List<MapPanelScript>();

	[SerializeField]
	[HideInInspector]
	public List<UISprite> m_MapBars = new List<UISprite>();

	private MapPanelScript m_PanelLeft2;

	private MapPanelScript m_PanelLeft1;

	private MapPanelScript m_PanelCenter;

	private MapPanelScript m_PanelRight1;

	private MapPanelScript m_PanelRight2;

	private int m_Left2_ID;

	private int m_Left1_ID;

	private int m_Front_ID;

	private int m_Right1_ID;

	private int m_Right2_ID;

	private float m_ControlFactor;

	private bool m_WaitForMapRotating;

	private float m_WaitForRotatingTime = 0.5f;

	private float m_StartWaitTime;

	private float m_InitMapTweenDelay = 0.6f;

	private float m_InitMapTweenDelayContinue = 0.2f;

	private float m_InitMapTweenDuration = 0.2f;

	private float m_MapsTweenDuration = 0.7f;

	private bool m_IsStartTweenPlayed;

	private bool m_IsStartTweenFinised;

	private bool m_IsSoloPlay = true;

	public EDifficultMode m_difficultMode { get; set; }

	public int m_SeletecedMapID { get; set; }

	public int m_SeletecedLevelID { get; set; }

	private void Awake()
	{
		m_Instance = this;
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	private void Start()
	{
		Init();
		if (IapScript.mInstance == null)
		{
			GameObject original = Resources.Load("UI/Menu/IAP/Prefab/IapUI") as GameObject;
			UnityEngine.Object.Instantiate(original, new Vector3(0f, -1000f, 0f), Quaternion.identity);
		}
		IapScript.mInstance.gameObject.SetActive(false);
	}

	public void Init()
	{
		m_MapsObj.transform.localPosition = MapsPosition.localPosition;
		UserState userState = GameApp.GetInstance().GetUserState();
		if (userState != null)
		{
			m_MapPanels.Clear();
			m_MapBars.Clear();
			m_SeletecedMapID = GameApp.GetInstance().GetUserState().GetSelectedStageIndex();
			m_SeletecedLevelID = GameApp.GetInstance().GetUserState().GetSelectedSubStageIndex();
			m_difficultMode = GameApp.GetInstance().GetUserState().GetSelectedDifficultMode();
			for (int i = 0; i < Global.TOTAL_STAGE; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(m_MapPanelTemplate) as GameObject;
				gameObject.transform.parent = m_MapsObj.transform;
				gameObject.transform.localPosition = PanelBackPosition.localPosition;
				gameObject.transform.localRotation = PanelBackPosition.localRotation;
				gameObject.transform.localScale = PanelBackPosition.localScale;
				gameObject.GetComponent<Collider>().enabled = false;
				MapPanelScript component = gameObject.GetComponent<MapPanelScript>();
				component.Init(i);
				m_MapPanels.Add(component);
				GameObject gameObject2 = UnityEngine.Object.Instantiate(m_MapBarTemplate) as GameObject;
				gameObject2.transform.parent = m_MapBarsObj.transform;
				gameObject2.transform.localPosition = Vector3.zero;
				gameObject2.transform.localRotation = Quaternion.identity;
				gameObject2.transform.localScale = Vector3.one;
				m_MapBars.Add(gameObject2.GetComponent<UISprite>());
			}
			Vector3 localPosition = m_MapBarsObj.transform.localPosition;
			UIGrid component2 = m_MapBarsObj.GetComponent<UIGrid>();
			component2.Reposition();
			CalcMapPanelsPosition(true);
			m_IsStartTweenPlayed = false;
			m_IsStartTweenFinised = false;
			m_PanelRight2.SetAlphas(0f);
			m_PanelRight1.SetAlphas(0f);
			m_PanelLeft1.SetAlphas(0f);
			m_PanelLeft2.SetAlphas(0f);
			m_PanelCenter.SetAlphas(0f);
			if (LoadingUIScript.mInstance != null && !LoadingUIScript.mInstance.IsLoading())
			{
				StartTweenAlpha();
			}
			UIEventListener uIEventListener = UIEventListener.Get(m_OKButton);
			uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(ClickObject));
		}
	}

	public void StartTweenAlpha()
	{
		m_PanelRight1.SetTweenAlphaAtStart(0.25f);
		m_PanelLeft1.SetTweenAlphaAtStart(0.5f);
		m_PanelCenter.SetTweenAlphaAtStart(0.75f);
		m_IsStartTweenPlayed = true;
	}

	public void CalcMapPanelsPosition(bool setRotationImmediately)
	{
		m_Front_ID = m_SeletecedMapID;
		m_Left1_ID = ((m_Front_ID - 1 < 0) ? (m_Front_ID - 1 + Global.TOTAL_STAGE) : (m_Front_ID - 1));
		m_Left2_ID = ((m_Left1_ID - 1 < 0) ? (m_Left1_ID - 1 + Global.TOTAL_STAGE) : (m_Left1_ID - 1));
		m_Right1_ID = ((m_Front_ID + 1 >= Global.TOTAL_STAGE) ? (m_Front_ID + 1 - Global.TOTAL_STAGE) : (m_Front_ID + 1));
		m_Right2_ID = ((m_Right1_ID + 1 >= Global.TOTAL_STAGE) ? (m_Right1_ID + 1 - Global.TOTAL_STAGE) : (m_Right1_ID + 1));
		for (int i = 0; i < m_MapPanels.Count; i++)
		{
			MapPanelScript mapPanelScript = m_MapPanels[i];
			UISprite uISprite = m_MapBars[i];
			if (setRotationImmediately)
			{
				mapPanelScript.transform.localRotation = PanelBackPosition.localRotation;
			}
			mapPanelScript.CancelLevelButtons();
			mapPanelScript.m_Panel.alpha = 0f;
			mapPanelScript.m_Panel.depth = 0;
			mapPanelScript.m_MapSprite.alpha = 0f;
			mapPanelScript.DifficultySelection.gameObject.SetActive(false);
			uISprite.spriteName = "move_n";
			if (mapPanelScript.m_MapID == m_Front_ID)
			{
				if (setRotationImmediately)
				{
					mapPanelScript.transform.localRotation = PanelFrontPosition.localRotation;
				}
				mapPanelScript.GetComponent<Collider>().enabled = true;
				m_PanelCenter = mapPanelScript;
				mapPanelScript.SelectLevelButton();
				mapPanelScript.m_Panel.alpha = 1f;
				mapPanelScript.m_Panel.depth = 1;
				mapPanelScript.m_MapSprite.alpha = 1f;
				uISprite.spriteName = "move_u";
			}
			else if (mapPanelScript.m_MapID == m_Left1_ID)
			{
				if (setRotationImmediately)
				{
					mapPanelScript.transform.localRotation = PanelLeftPosition.localRotation;
				}
				mapPanelScript.m_Panel.alpha = NotFrontPanelAlpha;
				mapPanelScript.m_MapSprite.alpha = NotFrontPanelAlpha;
				m_PanelLeft1 = mapPanelScript;
			}
			else if (mapPanelScript.m_MapID == m_Left2_ID)
			{
				m_PanelLeft2 = mapPanelScript;
			}
			else if (mapPanelScript.m_MapID == m_Right1_ID)
			{
				if (setRotationImmediately)
				{
					mapPanelScript.transform.localRotation = PanelRightPosition.localRotation;
				}
				mapPanelScript.m_Panel.alpha = NotFrontPanelAlpha;
				mapPanelScript.m_MapSprite.alpha = NotFrontPanelAlpha;
				m_PanelRight1 = mapPanelScript;
			}
			else if (mapPanelScript.m_MapID == m_Right2_ID)
			{
				m_PanelRight2 = mapPanelScript;
			}
		}
		if (m_PanelCenter.m_MapID != Global.TOTAL_ACTIVATED_STAGE)
		{
			m_PanelCenter.DifficultySelection.gameObject.SetActive(true);
		}
	}

	public void DragStart()
	{
		m_ControlFactor = 0.5f;
		m_PanelCenter.DragStart(LeftAntiClockwisePosition, PanelRightPosition, 0.5f);
		m_PanelLeft1.DragStart(PanelBackPosition, FrontClockwisePosition, 0.5f);
		m_PanelLeft2.DragStart(PanelBackPosition, PanelLeftPosition, 0f);
		m_PanelRight1.DragStart(FrontAntiClockwisePosition, PanelBackPosition, 0.5f);
		m_PanelRight2.DragStart(PanelRightPosition, PanelBackPosition, 1f);
	}

	public void DragStop()
	{
		if (m_ControlFactor > 0.75f)
		{
			m_PanelCenter.DragStop(m_PanelCenter.transform, PanelRightPosition, true);
			m_PanelLeft1.DragStop(m_PanelLeft1.transform, PanelFrontPosition, true);
			m_PanelLeft2.DragStop(m_PanelLeft2.transform, PanelLeftPosition, true);
			m_PanelRight1.DragStop(m_PanelRight1.transform, PanelBackPosition, true);
			m_PanelRight2.DragStop(m_PanelRight2.transform, PanelBackPosition, true);
			m_SeletecedMapID = m_PanelLeft1.m_MapID;
			m_SeletecedLevelID = 1;
			CalcMapPanelsPosition(false);
		}
		else if (m_ControlFactor < 0.25f)
		{
			m_PanelCenter.DragStop(PanelLeftPosition, m_PanelCenter.transform, false);
			m_PanelLeft1.DragStop(PanelBackPosition, m_PanelLeft1.transform, false);
			m_PanelLeft2.DragStop(PanelBackPosition, m_PanelLeft2.transform, false);
			m_PanelRight1.DragStop(PanelFrontPosition, m_PanelRight1.transform, false);
			m_PanelRight2.DragStop(PanelRightPosition, m_PanelRight2.transform, false);
			m_SeletecedMapID = m_PanelRight1.m_MapID;
			m_SeletecedLevelID = 1;
			CalcMapPanelsPosition(false);
		}
		else if (m_ControlFactor < 0.5f)
		{
			m_PanelCenter.DragStop(m_PanelCenter.transform, PanelFrontPosition, true);
			m_PanelLeft1.DragStop(m_PanelLeft1.transform, PanelLeftPosition, true);
			m_PanelLeft2.DragStop(m_PanelLeft2.transform, PanelBackPosition, true);
			m_PanelRight1.DragStop(m_PanelRight1.transform, PanelRightPosition, true);
			m_PanelRight2.DragStop(m_PanelRight2.transform, PanelBackPosition, true);
		}
		else
		{
			m_PanelCenter.DragStop(PanelFrontPosition, m_PanelCenter.transform, false);
			m_PanelLeft1.DragStop(PanelLeftPosition, m_PanelLeft1.transform, false);
			m_PanelLeft2.DragStop(PanelBackPosition, m_PanelLeft2.transform, false);
			m_PanelRight1.DragStop(PanelRightPosition, m_PanelRight1.transform, false);
			m_PanelRight2.DragStop(PanelBackPosition, m_PanelRight2.transform, false);
		}
	}

	public void Dragging(float delta)
	{
		m_ControlFactor += delta * Time.deltaTime * m_DragSpeed;
		float alpha = Mathf.Lerp(NotFrontPanelAlpha, 1f, (!(m_ControlFactor < 0.5f)) ? (2f - m_ControlFactor * 2f) : (m_ControlFactor * 2f));
		float alpha2 = Mathf.Lerp(0f, 1f, m_ControlFactor);
		float alpha3 = Mathf.Lerp(0f, NotFrontPanelAlpha, m_ControlFactor);
		float alpha4 = Mathf.Lerp(1f, 0f, m_ControlFactor);
		float alpha5 = Mathf.Lerp(NotFrontPanelAlpha, 0f, m_ControlFactor);
		m_PanelCenter.Dragging(m_ControlFactor);
		m_PanelCenter.m_Panel.alpha = alpha;
		m_PanelCenter.m_MapSprite.alpha = alpha;
		m_PanelLeft1.Dragging(m_ControlFactor);
		m_PanelLeft1.m_Panel.alpha = alpha2;
		m_PanelLeft1.m_MapSprite.alpha = alpha2;
		m_PanelLeft2.Dragging(m_ControlFactor);
		m_PanelLeft2.m_Panel.alpha = alpha3;
		m_PanelLeft2.m_MapSprite.alpha = alpha3;
		m_PanelRight1.Dragging(m_ControlFactor);
		m_PanelRight1.m_Panel.alpha = alpha4;
		m_PanelRight1.m_MapSprite.alpha = alpha4;
		m_PanelRight2.Dragging(m_ControlFactor);
		m_PanelRight2.m_Panel.alpha = alpha5;
		m_PanelRight2.m_MapSprite.alpha = alpha5;
	}

	public void OKClicked()
	{
		if (m_SeletecedMapID != Global.TOTAL_ACTIVATED_STAGE && m_SeletecedMapID != -1 && m_SeletecedLevelID != -1 && !ShowTutorial(m_SeletecedMapID, m_SeletecedLevelID))
		{
			GameApp.GetInstance().GetUserState().SetSelectedStageIndex((byte)m_SeletecedMapID);
			GameApp.GetInstance().GetUserState().SetSelectedSubStageIndex((byte)m_SeletecedLevelID);
			GameApp.GetInstance().GetUserState().SetSelectedDifficultMode(m_difficultMode);
			GameApp.GetInstance().Save();
			if (m_IsSoloPlay)
			{
				GlobalData.GetInstance().Partner = null;
			}
			if (m_SeletecedLevelID == Global.SURVIVAL_SUB_STAGE_ID)
			{
				GameApp.GetInstance().GetGameMode().ModePlay = Mode.Survival;
			}
			else if (m_SeletecedLevelID == Global.BOSS_SUB_STAGE_ID)
			{
				GameApp.GetInstance().GetGameMode().ModePlay = Mode.Boss;
			}
			else if (m_SeletecedLevelID == Global.BOSS_FINAL_SUB_STAGE_ID)
			{
				GameApp.GetInstance().GetGameMode().ModePlay = Mode.BOSS_FINAL;
			}
			else
			{
				GameApp.GetInstance().GetGameMode().ModePlay = Mode.CamPain;
			}
			if (MenuMusicScript.mInstance != null)
			{
				MenuMusicScript.mInstance.Stop();
			}
			string levelName = GameApp.GetInstance().GetUserState().GetLevelConfig(m_SeletecedMapID, m_SeletecedLevelID)
				.levelName;
			Debug.Log("levelName = " + levelName);
			ShowLoadingPage(m_difficultMode, m_SeletecedMapID, m_SeletecedLevelID);
			SwVoidDataManager.TryGetRandomDropFromOnline(m_difficultMode, (byte)m_SeletecedMapID, (byte)m_SeletecedLevelID, false);
			GameWorld.LoadLevel(levelName, ELevelType.GAME);
		}
	}

	public void SetWaitForMapRotate()
	{
		m_WaitForMapRotating = true;
	}

	private void Update()
	{
		if (!m_IsStartTweenPlayed)
		{
			if (!LoadingUIScript.mInstance.IsLoading())
			{
				StartTweenAlpha();
			}
		}
		else
		{
			if (!m_IsStartTweenPlayed || m_IsStartTweenFinised || !(m_PanelCenter.m_Panel.alpha >= 1f))
			{
				return;
			}
			foreach (MapPanelScript mapPanel in m_MapPanels)
			{
				mapPanel.m_MapSpriteTweenAlpha.GetComponent<UIPanel>().alpha = 1f;
			}
			m_IsStartTweenFinised = true;
		}
	}

	public void StopRotation()
	{
	}

	public void ClickObject(GameObject go)
	{
		if (!(go == m_OKButton))
		{
			return;
		}
		if (m_SeletecedMapID == Global.TOTAL_ACTIVATED_STAGE || m_SeletecedMapID == -1 || m_SeletecedLevelID == -1 || ShowTutorial(m_SeletecedMapID, m_SeletecedLevelID))
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
		}
		else if (!m_PanelCenter.m_LevelButtons[m_SeletecedLevelID].IsLocked())
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			GameApp.GetInstance().GetUserState().GenerateTrialWeaponID(m_difficultMode, m_SeletecedMapID, m_SeletecedLevelID);
			Debug.Log("mTrialWeaponID = " + GameApp.GetInstance().GetUserState().GetTrialWeaponID());
			Debug.Log("mTodayTrialed = " + GameApp.GetInstance().GetUserState().GetTodayTrailed());
			if (GameApp.GetInstance().GetUserState().GetTrialWeaponID() > 0)
			{
				PushTrialWeapon();
			}
			else if (GameApp.GetInstance().GetUserState().IsThereNullItem() && GameApp.GetInstance().GetUserState().CanPushItem())
			{
				PushItem();
			}
			else
			{
				OKClicked();
			}
		}
		else
		{
			LockMsg();
		}
	}

	public MapPanelScript GetCenterMapPanel()
	{
		return m_PanelCenter;
	}

	public static void ShowLoadingPage(EDifficultMode diffMode, int mapID, int levelID)
	{
		string text = ((mapID + 1 >= 10) ? (mapID + 1).ToString() : ("0" + (mapID + 1)));
		text += ((levelID >= 10) ? levelID.ToString() : ("0" + levelID));
		string text2 = string.Empty;
		bool flag = false;
		bool flag2 = false;
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.TIP_LIST, DataSheetName.TIP_LEVEL);
		for (int i = 0; i < gameDataTable.Count; i++)
		{
			string text3 = gameDataTable.GetString(i, 0);
			if (text3.Equals(text))
			{
				flag = true;
				text2 = gameDataTable.GetString(i, 1);
				break;
			}
		}
		if (!flag)
		{
			Mission mission = MissionManager.GetInstance().GetMission((byte)mapID, (byte)levelID, 0, diffMode);
			gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.TIP_LIST, DataSheetName.TIP_MISSION);
			for (int j = 0; j < gameDataTable.Count; j++)
			{
				string text4 = gameDataTable.GetString(j, 0);
				Debug.Log("missionData = " + text4);
				switch (mission.Type)
				{
				case MissionType.DESTINATION:
					if (text4.Equals("SP.Destination"))
					{
						flag2 = true;
					}
					break;
				case MissionType.SURVIVAL:
					if (text4.Equals("SP.Suvival"))
					{
						flag2 = true;
					}
					break;
				case MissionType.KILL_ENEMIES_AREA:
					if (text4.Equals("SP.KillArea"))
					{
						flag2 = true;
					}
					break;
				case MissionType.PROTECTION:
					if (text4.Equals("SP.Protect"))
					{
						flag2 = true;
					}
					break;
				case MissionType.EXPLOSION_KILL_ENEMY:
					if (text4.Equals("SP.Explosion"))
					{
						flag2 = true;
					}
					break;
				case MissionType.CHARGE_DESTROY:
					if (text4.Equals("SP.ChageDestory"))
					{
						flag2 = true;
					}
					break;
				}
				if (flag2)
				{
					text2 = gameDataTable.GetString(j, 1);
					break;
				}
			}
			if (!flag2)
			{
				text2 = LoadingUIScript.mInstance.GetRandomTip();
			}
		}
		string randomTip = LoadingUIScript.mInstance.GetRandomTip();
		while (randomTip.Equals(text2))
		{
			randomTip = LoadingUIScript.mInstance.GetRandomTip();
		}
		LoadingUIScript.mInstance.StartLoading(text2, randomTip);
	}

	private bool ShowTutorial(int MapID, int LevelID)
	{
		return false;
	}

	public void SetSoloPlay(bool isSolo)
	{
		m_IsSoloPlay = isSolo;
	}

	public void LockMsg()
	{
		UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_ICON_29") + "\n" + LocalizationManager.GetInstance().GetString("LOC_UNLOCK_LEVEL"), 2, 26);
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (whichMsg.EventId == 26)
		{
			UIMsgBox.instance.CloseMessage();
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				IapScript.mInstance.OpenPage(IapScript.IapType.Nothing);
				IapScript.mInstance.IsGotoUnlockIAP = true;
				IapScript.mInstance.OpenPage(IapScript.IapType.Package);
			}
		}
		else if (buttonId == UIMsg.UIMsgButton.Ok)
		{
			UIMsgBox.instance.CloseMessage();
		}
	}

	public void SetMoneyStatusObjActive(bool active)
	{
		MoneyStatusObj.SetActive(active);
	}

	public void PushTrialWeapon()
	{
		GameObject original = Resources.Load("UI/Menu/IAP/Prefab/WeaponTrialPush") as GameObject;
		GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
	}

	public void PushItem()
	{
		GameObject original = Resources.Load("UI/Menu/IAP/Prefab/ItemPushUI") as GameObject;
		GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
		GameApp.GetInstance().GetUserState().SetPushItemTime(DateTime.Now);
	}
}
