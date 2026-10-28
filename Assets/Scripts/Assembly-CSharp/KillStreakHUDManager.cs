using System.Collections.Generic;
using UnityEngine;

public class KillStreakHUDManager : MonoBehaviour
{
	public GameObject m_KillStreakTemplete;

	public UIGrid m_KillStreakGrid;

	private List<StateKillStreak> mSkillList = new List<StateKillStreak>();

	private Player mLocalPlayer;

	private UserState mUserState;

	private void Awake()
	{
		mLocalPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		mUserState = GameApp.GetInstance().GetUserState();
		if (mLocalPlayer != null)
		{
			RefreshSkillButtons();
		}
	}

	private void Update()
	{
		if (mLocalPlayer == null)
		{
			mLocalPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			if (mLocalPlayer != null)
			{
				RefreshSkillButtons();
			}
		}
	}

	public void AddSkill(int killStreakId, AdvancedSkill skill)
	{
		foreach (StateKillStreak mSkill in mSkillList)
		{
			if (mSkill.GetAdvancedSkill() != null && mSkill.GetAdvancedSkill().GetExternalId() == skill.GetExternalId())
			{
				return;
			}
		}
		GameObject gameObject = Object.Instantiate(m_KillStreakTemplete) as GameObject;
		StateKillStreak component = gameObject.GetComponent<StateKillStreak>();
		if (null != component)
		{
			mSkillList.Add(component);
			component.SetKillStreakId(killStreakId);
			component.SetAdvancedSkill(skill);
			gameObject.transform.parent = m_KillStreakGrid.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			HUDBase.instance.ItemBarReposition();
			ItemTween.GetInstance().RefreshAfterAddKillStreak();
		}
	}

	public void RemoveSkill(StateKillStreak killStreak)
	{
		if (mSkillList.Contains(killStreak))
		{
			mSkillList.Remove(killStreak);
			Object.DestroyImmediate(killStreak.gameObject);
			HUDBase.instance.ItemBarReposition();
		}
	}

	public void RefreshSkillButtons()
	{
		mSkillList.Clear();
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < m_KillStreakGrid.transform.childCount; i++)
		{
			list.Add(m_KillStreakGrid.transform.GetChild(i).gameObject);
		}
		foreach (GameObject item in list)
		{
			Object.DestroyImmediate(item);
		}
		KillStreakGetRequest request = new KillStreakGetRequest();
		GameApp.GetInstance().GetNetworkManager().SendRequest(request);
	}

	public void Reposition()
	{
		float num = (float)(m_KillStreakGrid.transform.childCount - 1) * m_KillStreakGrid.cellWidth;
		Vector3 localPosition = m_KillStreakGrid.transform.localPosition;
		m_KillStreakGrid.transform.localPosition = new Vector3((0f - num) / 2f, localPosition.y, localPosition.z);
		m_KillStreakGrid.Reposition();
	}

	public List<StateKillStreak> GetKillStreakList()
	{
		return mSkillList;
	}
}
