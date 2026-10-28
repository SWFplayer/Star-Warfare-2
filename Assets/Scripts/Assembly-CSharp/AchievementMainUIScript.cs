using System;
using System.Collections.Generic;
using UnityEngine;

public class AchievementMainUIScript : MonoBehaviour, UIMsgListener
{
	private const string AchievementName = "LOC_ACHIEVEMENT_";

	public GameObject Achievement;

	private List<AchievementSlotScript> AchievementList = new List<AchievementSlotScript>();

	private AchievementState AchievementStates;

	public GameObject Type1;

	public GameObject Type2;

	public GameObject Type3;

	public GameObject Type4;

	public GameObject Type5;

	public GameObject Type6;

	public UIScrollBar Bar;

	private bool bInited;

	private void Start()
	{
		AchievementStates = GameApp.GetInstance().GetUserState().Achievement;
		for (int i = 0; i < AchievementState.ACHIEVEMENT_COUNT; i++)
		{
			if (!AchievementStates.GetAchievements()[i].Available)
			{
				AchievementList.Add(null);
				continue;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(Achievement) as GameObject;
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = Vector3.one;
			string text = AchievementStates.GetAchievements()[i].ID.ToString().PadLeft(3, '0');
			gameObject.name = AchievementStates.GetAchievements()[i].Series + AchievementStates.GetAchievements()[i].Type + text;
			AchievementList.Add(gameObject.GetComponent<AchievementSlotScript>());
		}
		base.gameObject.GetComponent<UIGrid>().Reposition();
		UIEventListener uIEventListener = UIEventListener.Get(Type1);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnBtnClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(Type2);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnBtnClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(Type3);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnBtnClick));
		UIEventListener uIEventListener4 = UIEventListener.Get(Type4);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnBtnClick));
		UIEventListener uIEventListener5 = UIEventListener.Get(Type5);
		uIEventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener5.onClick, new UIEventListener.VoidDelegate(OnBtnClick));
		UIEventListener uIEventListener6 = UIEventListener.Get(Type6);
		uIEventListener6.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener6.onClick, new UIEventListener.VoidDelegate(OnBtnClick));
	}

	private void Update()
	{
		if (bInited)
		{
			return;
		}
		bInited = true;
		for (int i = 0; i < AchievementState.ACHIEVEMENT_COUNT; i++)
		{
			if (!(AchievementList[i] == null))
			{
				if (GetComplete(i))
				{
					AchievementList[i].SetComplete(AchievementStates.GetAchievements()[i].TitleId);
				}
				else
				{
					AchievementList[i].SetProgress(GetProgress(i), AchievementStates.GetAchievements()[i].TitleId);
				}
				AchievementList[i].NameLabel.GetComponent<UILabel>().text = AchievementStates.GetAchievements()[i].Name;
				AchievementList[i].DescriptionLabel.GetComponent<UILabel>().text = AchievementStates.GetAchievements()[i].DisplayDescription;
				if (AchievementStates.GetAchievements()[i].TitleId != 0)
				{
					UILabel component = AchievementList[i].DescriptionLabel.GetComponent<UILabel>();
					string text = component.text;
					component.text = text + "@# " + '@' + "LOC_ACHIEVEMENT_TIPS_4@#:" + '@' + GameApp.GetInstance().GetUserState().TitleDictionary[AchievementStates.GetAchievements()[i].TitleId];
				}
				AchievementList[i].NameLabel.GetComponent<UILocalizationX>().enabled = true;
				AchievementList[i].DescriptionLabel.GetComponent<UILocalizationX>().enabled = true;
				if (AchievementState.HideAchivement.Contains(i) && !AchievementStates.GetAchievements()[i].complete)
				{
					AchievementList[i].HideAchievement();
				}
			}
		}
	}

	private void OnBtnClick(GameObject go)
	{
		if (go == Type1)
		{
			Bar.value = EAchivementType.AchivementReference[0];
		}
		else if (go == Type2)
		{
			Bar.value = EAchivementType.AchivementReference[1];
		}
		else if (go == Type3)
		{
			Bar.value = EAchivementType.AchivementReference[2];
		}
		else if (go == Type4)
		{
			Bar.value = EAchivementType.AchivementReference[3];
		}
		else if (go == Type5)
		{
			Bar.value = EAchivementType.AchivementReference[4];
		}
		else if (go == Type6)
		{
			Bar.value = EAchivementType.AchivementReference[5];
		}
	}

	private int[] GetProgress(int i)
	{
		return new int[2]
		{
			AchievementStates.GetAchievements()[i].CurrentNum,
			AchievementStates.GetAchievements()[i].Num
		};
	}

	private bool GetComplete(int i)
	{
		return AchievementStates.GetAchievements()[i].complete;
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
	}
}
