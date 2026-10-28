using System;
using System.Collections.Generic;
using UnityEngine;

public class SelectMutantSkillPage : MonoBehaviour
{
	public LoadoutSkillScript[] EquipedSkills;

	public UIGrid SkillSelectPanel;

	public GameObject SkillSelectionTemplate;

	public UILabel EquipedSkillScores;

	public UILabel SkillName;

	public UILabel SkillDescription;

	public GameObject ClearButton;

	private int mBossIndex = -1;

	private int mMaxScore;

	private LoadoutSkillScript mSelectedSelection;

	private List<LoadoutSkillScript> mSkillSelections = new List<LoadoutSkillScript>();

	private Timer chooseSkillTimer = new Timer();

	private void Awake()
	{
		LoadoutSkillScript[] equipedSkills = EquipedSkills;
		foreach (LoadoutSkillScript loadoutSkillScript in equipedSkills)
		{
			UIEventListener uIEventListener = UIEventListener.Get(loadoutSkillScript.gameObject);
			uIEventListener.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener.onPress, new UIEventListener.BoolDelegate(OnButtonPressed));
			UIEventListener uIEventListener2 = UIEventListener.Get(loadoutSkillScript.gameObject);
			uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		}
		UIEventListener uIEventListener3 = UIEventListener.Get(ClearButton.gameObject);
		uIEventListener3.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener3.onPress, new UIEventListener.BoolDelegate(OnButtonPressed));
		UIEventListener uIEventListener4 = UIEventListener.Get(ClearButton.gameObject);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
	}

	public void OpenPage(int bossIndex, int maxScore)
	{
		chooseSkillTimer.SetTimer(20f, false);
		LoadoutSkillScript[] equipedSkills = EquipedSkills;
		foreach (LoadoutSkillScript loadoutSkillScript in equipedSkills)
		{
			loadoutSkillScript.SetSkill(null);
			loadoutSkillScript.SetUnselected();
		}
		foreach (LoadoutSkillScript mSkillSelection in mSkillSelections)
		{
			UnityEngine.Object.DestroyImmediate(mSkillSelection.gameObject);
		}
		mSkillSelections.Clear();
		while (SkillSelectPanel.transform.childCount > 0)
		{
			UnityEngine.Object.DestroyImmediate(SkillSelectPanel.transform.GetChild(0));
		}
		mBossIndex = bossIndex;
		mMaxScore = maxScore;
		EquipedSkillScores.text = "0/" + mMaxScore;
		mSelectedSelection = null;
		SkillName.text = string.Empty;
		SkillDescription.text = string.Empty;
		List<MutantSkill> allMutantSkill = GameApp.GetInstance().GetUserState().GetPvpSkillManager()
			.GetAllMutantSkill();
		foreach (MutantSkill item in allMutantSkill)
		{
			if (item.GetBossIndex() == mBossIndex)
			{
				string text = "Selection" + item.GetOrder();
				GameObject gameObject = UnityEngine.Object.Instantiate(SkillSelectionTemplate) as GameObject;
				gameObject.name = text;
				gameObject.transform.parent = SkillSelectPanel.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				LoadoutSkillScript component = gameObject.GetComponent<LoadoutSkillScript>();
				component.SetSkill(item);
				component.LabelId.gameObject.SetActive(true);
				component.LabelId.text = item.GetScore().ToString();
				mSkillSelections.Add(component);
				UIEventListener uIEventListener = UIEventListener.Get(gameObject);
				uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
			}
		}
		SkillSelectPanel.Reposition();
	}

	private void OnButtonPressed(GameObject go, bool isPress)
	{
		LoadoutSkillScript[] equipedSkills = EquipedSkills;
		foreach (LoadoutSkillScript loadoutSkillScript in equipedSkills)
		{
			if (go == loadoutSkillScript.gameObject)
			{
				if (isPress)
				{
					loadoutSkillScript.SetSelected();
				}
				else
				{
					loadoutSkillScript.SetUnselected();
				}
				break;
			}
		}
	}

	private void OnButtonClick(GameObject go)
	{
		if (go == ClearButton)
		{
			ClearSelections();
			return;
		}
		LoadoutSkillScript[] equipedSkills = EquipedSkills;
		foreach (LoadoutSkillScript loadoutSkillScript in equipedSkills)
		{
			if (!(go == loadoutSkillScript.gameObject))
			{
				continue;
			}
			if (!(mSelectedSelection != null))
			{
				return;
			}
			int num = 0;
			int num2 = 0;
			num = ((loadoutSkillScript.Skill != null) ? ((MutantSkill)loadoutSkillScript.Skill).GetScore() : 0);
			num2 = ((MutantSkill)mSelectedSelection.Skill).GetScore();
			if (isTempScoreLegal(num2, num))
			{
				LoadoutSkillScript[] equipedSkills2 = EquipedSkills;
				foreach (LoadoutSkillScript loadoutSkillScript2 in equipedSkills2)
				{
					if (loadoutSkillScript2 != loadoutSkillScript && loadoutSkillScript2.Skill == mSelectedSelection.Skill)
					{
						loadoutSkillScript2.SetSkill(loadoutSkillScript.Skill);
						break;
					}
				}
				loadoutSkillScript.SetSkill(mSelectedSelection.Skill);
				mSelectedSelection.SetUnselected();
				mSelectedSelection = null;
				RefreshEquipedSelections();
			}
			else
			{
				UserStateHUD.GetInstance().InfoBox.SetPrivateAchieveWithoutNum(LocalizationManager.GetInstance().GetString("TIPS_UPDATE11_13"));
			}
			return;
		}
		foreach (LoadoutSkillScript mSkillSelection in mSkillSelections)
		{
			if (go == mSkillSelection.gameObject)
			{
				if (mSelectedSelection != null)
				{
					mSelectedSelection.SetUnselected();
				}
				mSelectedSelection = mSkillSelection;
				mSelectedSelection.SetSelected();
				MutantSkill mutantSkill = mSkillSelection.Skill as MutantSkill;
				SkillName.text = LocalizationManager.GetInstance().GetString(mutantSkill.GetName());
				SkillDescription.text = LocalizationManager.GetInstance().GetString(mutantSkill.GetDescription());
				TryEquipNewSkill();
				RefreshEquipedSelections();
			}
		}
	}

	private void ClearSelections()
	{
		LoadoutSkillScript[] equipedSkills = EquipedSkills;
		foreach (LoadoutSkillScript loadoutSkillScript in equipedSkills)
		{
			if (loadoutSkillScript.Skill != null)
			{
				loadoutSkillScript.SetSkill(null);
				loadoutSkillScript.SetUnselected();
			}
		}
		RefreshEquipedSelections();
	}

	private void RefreshEquipedSelections()
	{
		foreach (LoadoutSkillScript mSkillSelection in mSkillSelections)
		{
			mSkillSelection.EquipedFrame.gameObject.SetActive(false);
		}
		EquipedSkillScores.text = GetScoreEquiped() + "/" + mMaxScore;
	}

	private int GetScoreEquiped()
	{
		int num = 0;
		LoadoutSkillScript[] equipedSkills = EquipedSkills;
		foreach (LoadoutSkillScript loadoutSkillScript in equipedSkills)
		{
			if (loadoutSkillScript.Skill == null)
			{
				continue;
			}
			num += (loadoutSkillScript.Skill as MutantSkill).GetScore();
			foreach (LoadoutSkillScript mSkillSelection in mSkillSelections)
			{
				if (loadoutSkillScript.Skill == mSkillSelection.Skill)
				{
					mSkillSelection.EquipedFrame.gameObject.SetActive(true);
					break;
				}
			}
		}
		return num;
	}

	private void TryEquipNewSkill()
	{
		LoadoutSkillScript loadoutSkillScript = null;
		LoadoutSkillScript[] equipedSkills = EquipedSkills;
		foreach (LoadoutSkillScript loadoutSkillScript2 in equipedSkills)
		{
			if (loadoutSkillScript2.Skill == mSelectedSelection.Skill)
			{
				return;
			}
			if (loadoutSkillScript2.isEmpty())
			{
				loadoutSkillScript = loadoutSkillScript2;
				break;
			}
		}
		if (loadoutSkillScript != null && mSelectedSelection.Skill != null && isTempScoreLegal(((MutantSkill)mSelectedSelection.Skill).GetScore(), 0))
		{
			loadoutSkillScript.SetSkill(mSelectedSelection.Skill);
		}
		else
		{
			UserStateHUD.GetInstance().InfoBox.SetPrivateAchieveWithoutNum(LocalizationManager.GetInstance().GetString("TIPS_UPDATE11_13"));
		}
	}

	public byte[] GetSelectedSkills()
	{
		byte[] array = new byte[4];
		int num = 0;
		LoadoutSkillScript[] equipedSkills = EquipedSkills;
		foreach (LoadoutSkillScript loadoutSkillScript in equipedSkills)
		{
			if (num < array.Length)
			{
				if (loadoutSkillScript.Skill != null)
				{
					array[num] = (byte)loadoutSkillScript.Skill.GetId();
				}
				else
				{
					array[num] = 0;
				}
			}
			num++;
		}
		return array;
	}

	private void Update()
	{
		if (chooseSkillTimer.Ready())
		{
			PlayerBossChooseSkillRequest request = new PlayerBossChooseSkillRequest(GetSelectedSkills());
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			base.gameObject.SetActive(false);
		}
	}

	public float GetChooseSkillRestTime()
	{
		return chooseSkillTimer.GetInterval() - chooseSkillTimer.GetTimeSpan();
	}

	public float GetChooseSkillSpanTime()
	{
		return chooseSkillTimer.GetTimeSpan();
	}

	private bool isScoreLegal()
	{
		if (GetScoreEquiped() >= mMaxScore)
		{
			return false;
		}
		return true;
	}

	private bool isTempScoreLegal(int newScore, int oldScore)
	{
		if (GetScoreEquiped() - oldScore + newScore > mMaxScore)
		{
			return false;
		}
		return true;
	}
}
