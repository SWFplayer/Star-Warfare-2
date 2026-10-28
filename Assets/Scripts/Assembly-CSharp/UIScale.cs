using UnityEngine;

public class UIScale : MonoBehaviour
{
	[SerializeField]
	private GameObject m_ScaleTarget;

	[SerializeField]
	private GameObject[] m_IgnoreScaleTargets;

	[SerializeField]
	private Vector3[] m_ScaleList;

	[SerializeField]
	private int m_IndexInScaleList;

	private void Awake()
	{
		if (m_ScaleList == null || m_ScaleList.Length == 0)
		{
			Object.Destroy(this);
			return;
		}
		if (m_ScaleTarget == null)
		{
			m_ScaleTarget = base.gameObject;
		}
		if (GameApp.GetInstance().GetGameMode().IsSingle() || GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			Mission currentStageMission = MissionManager.GetInstance().GetCurrentStageMission(0);
			if (currentStageMission != null && currentStageMission.Type == MissionType.BOSS)
			{
				m_ScaleList = new Vector3[1]
				{
					new Vector3(0.25f, 0.25f, 0.25f)
				};
			}
		}
		ResetTargetScale();
	}

	private void ResetTargetScale()
	{
		m_IndexInScaleList %= m_ScaleList.Length;
		Vector3 localScale = m_ScaleTarget.transform.localScale;
		Vector3 vector = new Vector3(m_ScaleList[m_IndexInScaleList].x / localScale.x, m_ScaleList[m_IndexInScaleList].y / localScale.y, m_ScaleList[m_IndexInScaleList].z / localScale.z);
		m_ScaleTarget.transform.localScale = m_ScaleList[m_IndexInScaleList];
		for (int i = 0; i < m_IgnoreScaleTargets.Length; i++)
		{
			localScale = m_IgnoreScaleTargets[i].transform.localScale;
			m_IgnoreScaleTargets[i].transform.localScale = new Vector3(localScale.x / vector.x, localScale.y / vector.y, localScale.z / vector.z);
		}
	}

	public Vector3 NextScale()
	{
		m_IndexInScaleList++;
		ResetTargetScale();
		return m_ScaleList[m_IndexInScaleList];
	}

	public Vector3 PrevScale()
	{
		m_IndexInScaleList--;
		ResetTargetScale();
		return m_ScaleList[m_IndexInScaleList];
	}

	public Vector3 CurrScale()
	{
		return m_ScaleList[m_IndexInScaleList];
	}

	public void SetPowerUpScale(bool isPowerUp)
	{
		if (UIRadar.mInstance.IsForPVP)
		{
			if (isPowerUp)
			{
				m_ScaleList = new Vector3[1]
				{
					new Vector3(0.5f, 0.5f, 0.5f)
				};
			}
			else if (GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
			{
				m_ScaleList = new Vector3[1]
				{
					new Vector3(0.25f, 0.25f, 0.25f)
				};
			}
			else
			{
				m_ScaleList = new Vector3[1]
				{
					new Vector3(1f, 1f, 1f)
				};
			}
			ResetTargetScale();
		}
	}
}
