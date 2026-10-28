using UnityEngine;

public class MissionIntroScript : MonoBehaviour
{
	public UILabel[] m_MissionSummaryArray;

	public UISprite[] m_MissionStarsArray;

	public UISprite[] m_MissionRewardCurrencys;

	public UILabel[] m_MissionRewards;

	private Mission[] mMissionArray;

	private int mAllStarReward;

	private MapPanelScript mParentMapPanel;

	private void Awake()
	{
		mMissionArray = new Mission[m_MissionSummaryArray.Length];
		mParentMapPanel = base.transform.parent.GetComponent<MapPanelScript>();
	}

	public void SetMissionDescription()
	{
		bool flag = true;
		mAllStarReward = 0;
		for (int i = 0; i < mMissionArray.Length; i++)
		{
			if (i >= Global.MISSION_SIZE)
			{
				continue;
			}
			mMissionArray[i] = MissionManager.GetInstance().GetMission((byte)mParentMapPanel.m_MapID, (byte)MissionSelectUIScript.m_Instance.m_SeletecedLevelID, i, MissionSelectUIScript.m_Instance.m_difficultMode);
			if (mMissionArray[i] != null)
			{
				m_MissionSummaryArray[i].text = "- " + mMissionArray[i].SummaryText;
				if (mMissionArray[i].IsComplete())
				{
					m_MissionStarsArray[i].spriteName = "star_u";
					m_MissionSummaryArray[i].alpha = 1f;
					m_MissionRewardCurrencys[i].gameObject.SetActive(false);
					m_MissionRewards[i].gameObject.SetActive(false);
					continue;
				}
				m_MissionStarsArray[i].spriteName = "star_n";
				m_MissionRewardCurrencys[i].gameObject.SetActive(true);
				m_MissionRewards[i].gameObject.SetActive(true);
				int rewardCash = mMissionArray[i].GetRewardCash(MissionSelectUIScript.m_Instance.m_difficultMode);
				if (rewardCash > 0)
				{
					m_MissionRewardCurrencys[i].spriteName = "icon_dollar";
					m_MissionRewards[i].text = UIConstant.CURRENCY_COLOR_GOLD_PREFIX + rewardCash + UIConstant.COLOR_POSTFIX;
				}
				else
				{
					m_MissionRewardCurrencys[i].spriteName = "icon_mithril";
					rewardCash = mMissionArray[i].GetRewardMithril(MissionSelectUIScript.m_Instance.m_difficultMode);
					m_MissionRewards[i].text = UIConstant.CURRENCY_COLOR_MITHRIL_PREFIX + rewardCash + UIConstant.COLOR_POSTFIX;
				}
				flag = false;
			}
			else
			{
				Debug.LogError("Mission is NULL, mission id = " + (i + 1));
			}
		}
		if (!flag)
		{
			mAllStarReward = mMissionArray[0].GetRewardMithril(MissionSelectUIScript.m_Instance.m_difficultMode);
		}
		else
		{
			mAllStarReward = 0;
		}
		int seletecedLevelID = MissionSelectUIScript.m_Instance.m_SeletecedLevelID;
		string text = seletecedLevelID.ToString();
		if (seletecedLevelID == Global.SURVIVAL_SUB_STAGE_ID)
		{
			text = "Explore";
		}
		else if (seletecedLevelID == Global.BOSS_SUB_STAGE_ID)
		{
			text = "Boss";
		}
		MapPanelScript centerMapPanel = MissionSelectUIScript.m_Instance.GetCenterMapPanel();
		if (centerMapPanel != null)
		{
			centerMapPanel.m_TitleLabel.text = LocalizationManager.GetInstance().GetString("LOC_MISSION_STAGE") + " " + (MissionSelectUIScript.m_Instance.m_SeletecedMapID + 1) + " - " + text;
		}
	}

	public int GetAllStarReward()
	{
		return mAllStarReward;
	}
}
