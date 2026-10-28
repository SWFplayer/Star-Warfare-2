using System.Text.RegularExpressions;
using UnityEngine;

public class NGUIPauseMenu : MonoBehaviour
{
	public UILabel[] m_MissionSummaryArray;

	public UISprite[] m_MissionProgressArray;

	private Mission[] mMissionArray;

	private void Awake()
	{
		mMissionArray = new Mission[m_MissionSummaryArray.Length];
		SetMissionDescription();
	}

	private void OnEnable()
	{
		SetMissionDescription();
		m_MissionSummaryArray[0].text = Regex.Replace(mMissionArray[0].GetProgressText(), "[\\n\\r]", string.Empty);
	}

	public void SetMissionDescription()
	{
		for (int i = 0; i < mMissionArray.Length; i++)
		{
			if (i >= Global.MISSION_SIZE)
			{
				continue;
			}
			mMissionArray[i] = MissionManager.GetInstance().GetCurrentStageMission(i);
			if (mMissionArray[i] != null)
			{
				m_MissionSummaryArray[i].text = mMissionArray[i].SummaryText;
				if (mMissionArray[i].IsComplete())
				{
					m_MissionSummaryArray[i].text += " COMPLETE";
				}
				m_MissionProgressArray[i].fillAmount = mMissionArray[i].GetProgressPercentage();
			}
			else
			{
				Debug.LogError("Mission is NULL, mission id = " + (i + 1));
			}
		}
	}
}
