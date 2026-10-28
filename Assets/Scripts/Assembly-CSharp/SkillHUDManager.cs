using System.Collections.Generic;
using UnityEngine;

public class SkillHUDManager : MonoBehaviour
{
	public List<StateSkill> m_SkillList;

	private Player player;

	private UserState userState;

	private void Awake()
	{
		player = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		userState = GameApp.GetInstance().GetUserState();
		if (player != null)
		{
			RefreshSkillButtons();
		}
	}

	private void Update()
	{
		if (player == null)
		{
			player = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			if (player != null)
			{
				RefreshSkillButtons();
			}
		}
	}

	public void RefreshSkillButtons()
	{
		foreach (StateSkill skill in m_SkillList)
		{
			skill.gameObject.SetActive(false);
		}
		if (player == null)
		{
			player = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		}
		if (player == null)
		{
			return;
		}
		int num = 0;
		if (player.mSwVoid != null)
		{
			player.mSwVoid.InitSkillButton(m_SkillList[0]);
			if (m_SkillList[0].gameObject.activeSelf)
			{
				num++;
			}
		}
		player.InitSkillButton(m_SkillList, num);
	}

	public void Hide()
	{
		foreach (StateSkill skill in m_SkillList)
		{
			if (skill.gameObject.activeSelf)
			{
				skill.Hide();
			}
		}
	}

	public void Show()
	{
		if (player != null && player.IsBoss)
		{
			return;
		}
		foreach (StateSkill skill in m_SkillList)
		{
			if (skill.gameObject.activeSelf)
			{
				skill.Show();
			}
		}
	}

	public void ReduceCoolDown(float reduceTime)
	{
		foreach (StateSkill skill in m_SkillList)
		{
			if (skill.gameObject.activeSelf)
			{
				skill.ReduceCoolDown(reduceTime);
			}
		}
	}

	public bool IsAllSkillNotCoolDown()
	{
		bool flag = true;
		bool flag2 = true;
		foreach (StateSkill skill in m_SkillList)
		{
			if (skill.gameObject.activeSelf)
			{
				flag = flag && !skill.IsSkillCoolDown();
				flag2 = false;
			}
		}
		return flag && !flag2;
	}
}
