using UnityEngine;

public class LevelButtonScript : MonoBehaviour
{
	public enum LevelButtonStatus
	{
		Normal = 0,
		Locked = 1,
		Boss = 2,
		Infinity = 3
	}

	public UISprite Background;

	public UISprite Highlight;

	public UILabel Number;

	public GameObject BossMark;

	private LevelButtonStatus m_Status;

	private MapPanelScript m_ParentMapPanel;

	private bool mIsHighLight;

	public int m_MapID { get; set; }

	public int m_LevelID { get; set; }

	public void Init(int mapID, int levelID, MapPanelScript mapPanel)
	{
		m_MapID = mapID;
		m_LevelID = levelID;
		m_ParentMapPanel = mapPanel;
		Number.text = m_LevelID.ToString();
		CheckStatus();
		if (m_MapID == MissionSelectUIScript.m_Instance.m_SeletecedMapID && m_LevelID == MissionSelectUIScript.m_Instance.m_SeletecedLevelID)
		{
			OnClick();
		}
		mIsHighLight = false;
	}

	public void CheckStatus()
	{
		m_Status = LevelButtonStatus.Normal;
		Background.spriteName = "level_n";
		if (m_LevelID == Global.SURVIVAL_SUB_STAGE_ID)
		{
			m_Status = LevelButtonStatus.Infinity;
			Number.gameObject.SetActive(false);
			Background.spriteName = "any";
		}
		else if (m_LevelID == Global.BOSS_SUB_STAGE_ID)
		{
			m_Status = LevelButtonStatus.Boss;
		}
		if (m_Status == LevelButtonStatus.Boss)
		{
			BossMark.SetActive(true);
		}
		else
		{
			BossMark.SetActive(false);
		}
		if (m_MapID < Global.TOTAL_STAGE && m_LevelID < Global.TOTAL_SUB_STAGE)
		{
			UserState userState = GameApp.GetInstance().GetUserState();
			bool flag = false;
			if (MissionSelectUIScript.m_Instance.m_difficultMode > userState.GetUnlockedDifficultMode())
			{
				flag = true;
			}
			else if (m_MapID > userState.GetUnlockedStageId(MissionSelectUIScript.m_Instance.m_difficultMode))
			{
				flag = true;
			}
			else if (m_MapID == userState.GetUnlockedStageId(MissionSelectUIScript.m_Instance.m_difficultMode) && m_LevelID > userState.GetUnlockedSubStageId(MissionSelectUIScript.m_Instance.m_difficultMode))
			{
				flag = true;
			}
			if (flag)
			{
				m_Status = LevelButtonStatus.Locked;
				Number.gameObject.SetActive(false);
				Background.spriteName = "level_lock";
			}
			else if (m_Status != LevelButtonStatus.Infinity)
			{
				Number.gameObject.SetActive(true);
			}
		}
	}

	public void OnClick()
	{
		if (m_MapID != MissionSelectUIScript.m_Instance.m_SeletecedMapID)
		{
			return;
		}
		if (m_MapID == MissionSelectUIScript.m_Instance.m_SeletecedMapID && m_LevelID == MissionSelectUIScript.m_Instance.m_SeletecedLevelID && mIsHighLight)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (m_Status != LevelButtonStatus.Locked)
			{
				GameApp.GetInstance().GetUserState().GenerateTrialWeaponID(MissionSelectUIScript.m_Instance.m_difficultMode, m_MapID, m_LevelID);
				if (GameApp.GetInstance().GetUserState().GetTrialWeaponID() > 0)
				{
					MissionSelectUIScript.m_Instance.PushTrialWeapon();
				}
				else if (GameApp.GetInstance().GetUserState().IsThereNullItem() && GameApp.GetInstance().GetUserState().CanPushItem())
				{
					MissionSelectUIScript.m_Instance.PushItem();
				}
				else
				{
					MissionSelectUIScript.m_Instance.OKClicked();
				}
				return;
			}
			MissionSelectUIScript.m_Instance.LockMsg();
		}
		if (m_Status != LevelButtonStatus.Locked)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_SELECT);
		}
		else
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
		}
		MissionSelectUIScript.m_Instance.m_SeletecedLevelID = m_LevelID;
		m_ParentMapPanel.CancelLevelButtons();
		mIsHighLight = true;
		Highlight.gameObject.SetActive(true);
		switch (m_Status)
		{
		case LevelButtonStatus.Normal:
		case LevelButtonStatus.Locked:
		case LevelButtonStatus.Boss:
		case LevelButtonStatus.Infinity:
			MissionSelectUIScript.m_Instance.m_SeletecedLevelID = m_LevelID;
			BossMark.transform.localPosition = new Vector3(0f, -30f, 0f);
			break;
		}
		m_ParentMapPanel.ResetMissionInfos(m_MapID, m_LevelID);
	}

	public void Cancel()
	{
		BossMark.transform.localPosition = new Vector3(0f, -22f, 0f);
		Highlight.gameObject.SetActive(false);
		mIsHighLight = false;
	}

	public bool IsLocked()
	{
		return m_Status == LevelButtonStatus.Locked;
	}
}
