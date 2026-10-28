using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillPageScript : MonoBehaviour, UIMsgListener
{
	public List<LoadoutSkillScript> PvpSkillList;

	public List<LoadoutSkillScript> AwardSkillList;

	public GameObject BackButton;

	public GameObject SelectButton;

	public GameObject SelectBlinking;

	public UISprite SelectButtonBackground;

	public UIDraggablePanel SkillPanel;

	public UIGrid SkillSelectionGrid;

	public GameObject SkillBlockTemplete;

	public UILabel CurrentSkillName;

	public UILabel CurrentSkillDescription;

	public UILabel CurrentSkillUnlockTitle;

	public UILabel CurrentSkillUnlockCondition;

	private List<LoadoutSkillScript> mCandidateSkillList = new List<LoadoutSkillScript>();

	private UserState mUserState;

	private bool mPvpSkillSelected;

	private int mCurrentPvpSkillId;

	private int mCurrentAwardSkillId;

	private int mCurrentSkillTabId;

	private int mCurrentCandidateSkillId;

	private void Awake()
	{
		mUserState = GameApp.GetInstance().GetUserState();
		UIEventListener uIEventListener = UIEventListener.Get(BackButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(SelectButton);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		foreach (LoadoutSkillScript pvpSkill in PvpSkillList)
		{
			UIEventListener uIEventListener3 = UIEventListener.Get(pvpSkill.gameObject);
			uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		}
		foreach (LoadoutSkillScript awardSkill in AwardSkillList)
		{
			UIEventListener uIEventListener4 = UIEventListener.Get(awardSkill.gameObject);
			uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		}
		SelectPvpSkill(0);
		SelectBlinking.SetActive(false);
		SelectButtonBackground.alpha = 1f;
	}

	public void OnButtonClick(GameObject go)
	{
		if (go == BackButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			LoadoutUIScript.Instance.SaveCurrentCustomize();
			LoadoutUIScript.Instance.OpenLoadoutPage();
			return;
		}
		if (go == SelectButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (mCandidateSkillList[mCurrentCandidateSkillId].IsLocked())
			{
				UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_KILLSTREAK_TIPS_1", mCandidateSkillList[mCurrentCandidateSkillId].Skill.GetUnlockLevel()));
			}
			else if (mPvpSkillSelected)
			{
				EquipPvpSkill(mCandidateSkillList[mCurrentCandidateSkillId].Skill.GetId());
			}
			else
			{
				EquipAwardSkill(mCandidateSkillList[mCurrentCandidateSkillId].Skill.GetId());
			}
			SelectBlinking.SetActive(false);
			SelectButtonBackground.alpha = 1f;
			return;
		}
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
		for (int i = 0; i < PvpSkillList.Count; i++)
		{
			if (go == PvpSkillList[i].gameObject)
			{
				if (!PvpSkillList[i].IsSelected())
				{
					SelectPvpSkill(i);
					SkillPanel.ResetPosition();
				}
				break;
			}
		}
		for (int j = 0; j < AwardSkillList.Count; j++)
		{
			if (go == AwardSkillList[j].gameObject)
			{
				if (!AwardSkillList[j].IsSelected())
				{
					SelectAwardSkill(j);
					SkillPanel.ResetPosition();
				}
				break;
			}
		}
		for (int k = 0; k < mCandidateSkillList.Count; k++)
		{
			if (!(go == mCandidateSkillList[k].gameObject))
			{
				continue;
			}
			if (mCandidateSkillList[k].IsSelected())
			{
				if (mCandidateSkillList[k].IsLocked())
				{
					UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_KILLSTREAK_TIPS_1", mCandidateSkillList[k].Skill.GetUnlockLevel()));
				}
				else if (mPvpSkillSelected)
				{
					EquipPvpSkill(mCandidateSkillList[k].Skill.GetId());
				}
				else
				{
					EquipAwardSkill(mCandidateSkillList[k].Skill.GetId());
				}
				SelectBlinking.SetActive(false);
				SelectButtonBackground.alpha = 1f;
			}
			else
			{
				SelectCandidateSkill(k);
				SelectBlinking.SetActive(true);
				SelectButtonBackground.alpha = 0.5f;
			}
			break;
		}
	}

	private void Update()
	{
	}

	private void RefreshCurrentSkill()
	{
		OnlineCustomizeData currentCustomize = mUserState.GetCurrentCustomize();
		for (int i = 0; i < PvpSkillList.Count; i++)
		{
			if (i < currentCustomize.mPvpSkillIdArray.Length)
			{
				PvpSkillList[i].SetUnlocked();
				PvpSkillList[i].SetSkill(mUserState.GetPvpSkillManager().GetPerkById(currentCustomize.mPvpSkillIdArray[i]));
			}
			else
			{
				PvpSkillList[i].SetLocked();
			}
		}
		for (int j = 0; j < AwardSkillList.Count; j++)
		{
			if (j < currentCustomize.mAwardSkillIdArray.Length)
			{
				AwardSkillList[j].SetUnlocked();
				AwardSkillList[j].SetSkill(mUserState.GetPvpSkillManager().GetKillStreakById(currentCustomize.mAwardSkillIdArray[j]));
			}
			else
			{
				AwardSkillList[j].SetLocked();
			}
		}
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
	}

	private void SelectCandidateSkill(int index)
	{
		mCurrentCandidateSkillId = index;
		int rankID = GameApp.GetInstance().GetUserState().GetRank()
			.rankID;
		for (int i = 0; i < mCandidateSkillList.Count; i++)
		{
			if (i == index)
			{
				mCandidateSkillList[i].SetSelected();
				PvpSkill skill = mCandidateSkillList[i].Skill;
				if (skill != null)
				{
					CurrentSkillName.text = LocalizationManager.GetInstance().GetString(skill.GetName());
					CurrentSkillDescription.text = LocalizationManager.GetInstance().GetString(skill.GetDescription());
					if (skill.GetUnlockLevel() > rankID)
					{
						CurrentSkillUnlockTitle.gameObject.SetActive(true);
						CurrentSkillUnlockCondition.gameObject.SetActive(true);
						CurrentSkillUnlockCondition.text = LocalizationManager.GetInstance().GetString("LOC_KILLSTREAK_TIPS_1", skill.GetUnlockLevel());
					}
					else
					{
						CurrentSkillUnlockTitle.gameObject.SetActive(false);
						CurrentSkillUnlockCondition.gameObject.SetActive(false);
					}
				}
			}
			else
			{
				mCandidateSkillList[i].SetUnselected();
			}
		}
	}

	private void EquipPvpSkill(int skillId)
	{
		bool flag = false;
		OnlineCustomizeData currentCustomize = mUserState.GetCurrentCustomize();
		for (int i = 0; i < currentCustomize.mPvpSkillIdArray.Length; i++)
		{
			if (i != mCurrentPvpSkillId && currentCustomize.mPvpSkillIdArray[i] == skillId)
			{
				currentCustomize.mPvpSkillIdArray[i] = currentCustomize.mPvpSkillIdArray[mCurrentPvpSkillId];
				flag = true;
			}
		}
		if (!flag)
		{
			mUserState.ChangePerk(currentCustomize.mPvpSkillIdArray[mCurrentPvpSkillId], skillId);
		}
		currentCustomize.mPvpSkillIdArray[mCurrentPvpSkillId] = (short)skillId;
		mUserState.Achievement.CheckAchievement_5Perk(currentCustomize.mPvpSkillIdArray.Length);
		RefreshCurrentSkill();
	}

	private void EquipAwardSkill(int skillId)
	{
		bool flag = false;
		OnlineCustomizeData currentCustomize = mUserState.GetCurrentCustomize();
		for (int i = 0; i < currentCustomize.mAwardSkillIdArray.Length; i++)
		{
			if (i != mCurrentAwardSkillId && currentCustomize.mAwardSkillIdArray[i] == skillId)
			{
				currentCustomize.mAwardSkillIdArray[i] = currentCustomize.mAwardSkillIdArray[mCurrentAwardSkillId];
				flag = true;
			}
		}
		if (!flag)
		{
			mUserState.ChangeKillStreak(currentCustomize.mAwardSkillIdArray[mCurrentAwardSkillId], skillId);
		}
		currentCustomize.mAwardSkillIdArray[mCurrentAwardSkillId] = (short)skillId;
		RefreshCurrentSkill();
	}

	private void SelectPvpSkill(int index)
	{
		OnlineCustomizeData currentCustomize = mUserState.GetCurrentCustomize();
		if (index < currentCustomize.mPvpSkillIdArray.Length)
		{
			for (int i = 0; i < PvpSkillList.Count; i++)
			{
				if (i == index)
				{
					PvpSkillList[i].SetSelected();
				}
				else
				{
					PvpSkillList[i].SetUnselected();
				}
			}
			for (int j = 0; j < AwardSkillList.Count; j++)
			{
				AwardSkillList[j].SetUnselected();
			}
			mPvpSkillSelected = true;
			mCurrentPvpSkillId = index;
			RefreshCandidateSkills();
		}
		else
		{
			int lowestPrestigeForPerk = GameApp.GetInstance().GetUserState().GetLowestPrestigeForPerk(index);
			UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_KILLSTREAK_TIPS_8", lowestPrestigeForPerk));
		}
	}

	private void SelectAwardSkill(int index)
	{
		OnlineCustomizeData currentCustomize = mUserState.GetCurrentCustomize();
		if (index >= currentCustomize.mAwardSkillIdArray.Length)
		{
			return;
		}
		for (int i = 0; i < AwardSkillList.Count; i++)
		{
			if (i == index)
			{
				AwardSkillList[i].SetSelected();
			}
			else
			{
				AwardSkillList[i].SetUnselected();
			}
		}
		for (int j = 0; j < PvpSkillList.Count; j++)
		{
			PvpSkillList[j].SetUnselected();
		}
		mPvpSkillSelected = false;
		mCurrentAwardSkillId = index;
		RefreshCandidateSkills();
	}

	private void RefreshCandidateSkills()
	{
		ClearCandidateSkills();
		int rankID = GameApp.GetInstance().GetUserState().GetRank()
			.rankID;
		short num = 0;
		int index = 0;
		List<PvpSkill> list = new List<PvpSkill>();
		if (mPvpSkillSelected)
		{
			list = ((mCurrentPvpSkillId >= 3) ? mUserState.GetPvpSkillManager().GetAllPerk() : mUserState.GetPvpSkillManager().GetPerkByType((Perk.EPerkType)mCurrentPvpSkillId));
			num = GameApp.GetInstance().GetUserState().GetCurrentCustomize()
				.mPvpSkillIdArray[mCurrentPvpSkillId];
		}
		else
		{
			list = mUserState.GetPvpSkillManager().GetAllKillStreak();
			num = GameApp.GetInstance().GetUserState().GetCurrentCustomize()
				.mAwardSkillIdArray[mCurrentAwardSkillId];
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (i >= mCandidateSkillList.Count)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(SkillBlockTemplete) as GameObject;
				gameObject.name = SkillBlockTemplete.name + i;
				gameObject.transform.parent = SkillSelectionGrid.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				UIEventListener uIEventListener = UIEventListener.Get(gameObject);
				uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
				LoadoutSkillScript component = gameObject.GetComponent<LoadoutSkillScript>();
				mCandidateSkillList.Add(component);
			}
			mCandidateSkillList[i].SetSkill(list[i]);
			if (list[i].GetUnlockLevel() > rankID)
			{
				mCandidateSkillList[i].SetLocked();
			}
			else
			{
				mCandidateSkillList[i].SetUnlocked();
			}
			mCandidateSkillList[i].Show();
			if (list[i].GetId() == num)
			{
				index = i;
			}
		}
		SkillSelectionGrid.Reposition();
		SelectCandidateSkill(index);
	}

	private void ClearCandidateSkills()
	{
		foreach (LoadoutSkillScript mCandidateSkill in mCandidateSkillList)
		{
			mCandidateSkill.Hide();
		}
	}

	public void ShowSkillPage()
	{
		base.gameObject.SetActive(true);
		RefreshCurrentSkill();
		SelectPvpSkill(0);
	}

	public void HideSkillPage()
	{
		base.gameObject.SetActive(false);
	}
}
