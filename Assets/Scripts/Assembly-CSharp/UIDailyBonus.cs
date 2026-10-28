using System;
using System.Collections.Generic;
using UnityEngine;

public class UIDailyBonus : MonoBehaviour
{
	public enum EDailyBonusState
	{
		NONE = 0,
		ANNOUNCEMENT = 1,
		LOGIN = 2,
		QUEST = 3
	}

	public static UIDailyBonus mInstance;

	public GameObject tabAnnouncement;

	public GameObject tabLogin;

	public GameObject tabQuest;

	public GameObject exit;

	public GameObject questFinalReward;

	public UISprite tabIconAnnouncement;

	public UISprite tabIconLogin;

	public UISprite tabIconQuest;

	public UISprite tabBkAnnouncement;

	public UISprite tabBkLogin;

	public UISprite tabBkQuest;

	public UISprite tabSelectedAnnouncement;

	public UISprite tabSelectedLogin;

	public UISprite tabSelectedQuest;

	public GameObject pageAnnouncement;

	public GameObject pageLogin;

	public GameObject pageQuest;

	public GameObject loginTemplate;

	public GameObject dragPanel;

	public float loginTemplateSpace;

	public GameObject PointLeft;

	public GameObject PointLeftMiddle;

	public GameObject PointMiddle;

	public GameObject PointRightMiddle;

	public GameObject PointRight;

	public GameObject rewardInfoObj;

	public GameObject rewardInfoTemplate;

	public GameObject rewardInfoButton;

	public GameObject rewardInfoPanel;

	public GameObject questTemplate;

	public GameObject questList;

	public UILabel announcementLabel;

	[HideInInspector]
	public List<UIDailyQuestInfo> m_DailyQuestInfoList = new List<UIDailyQuestInfo>();

	[HideInInspector]
	public List<UIDailyLoginReward> m_DailyLoginRewards = new List<UIDailyLoginReward>();

	[HideInInspector]
	public List<UIDailyRewardInfo> m_DailyRewardInfo = new List<UIDailyRewardInfo>();

	protected UICenterOnChild mCenterOnChild;

	private int mSelectedIndex;

	private int templateCount;

	private EDailyBonusState mUIState;

	private Color ALPHA_COLOR = new Color(1f, 1f, 1f, 0.5f);

	private Color FULL_COLOR = new Color(1f, 1f, 1f, 1f);

	protected float XLeft;

	protected float XLeftMiddle;

	protected float XMiddle;

	protected float XRightMiddle;

	protected float XRight;

	private bool needUpdate = true;

	private float mCheckCenterChildTime = 0.15f;

	private float mLastCheckTime;

	private UIDailyLoginReward mLastCenterIcon;

	private void Awake()
	{
		mInstance = this;
		UIEventListener uIEventListener = UIEventListener.Get(tabAnnouncement);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(tabLogin);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(tabQuest);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener4 = UIEventListener.Get(exit);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener5 = UIEventListener.Get(rewardInfoButton);
		uIEventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener5.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener6 = UIEventListener.Get(questFinalReward);
		uIEventListener6.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener6.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		ChangePage(EDailyBonusState.ANNOUNCEMENT);
		Init();
	}

	private void Init()
	{
	}

	private void Update()
	{
		if (mUIState != EDailyBonusState.LOGIN)
		{
			return;
		}
		if (needUpdate)
		{
			ArrangeIcons();
		}
		if (!(Time.time - mLastCheckTime > mCheckCenterChildTime))
		{
			return;
		}
		mLastCheckTime = Time.time;
		if (!(mCenterOnChild.centeredObject != null))
		{
			return;
		}
		UIDailyLoginReward component = mCenterOnChild.centeredObject.GetComponent<UIDailyLoginReward>();
		if (component != null && component != mLastCenterIcon)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_DRAG);
			if (mLastCenterIcon != null)
			{
				mLastCenterIcon.UnSelect();
			}
			mLastCenterIcon = component;
			mLastCenterIcon.Select();
			mSelectedIndex = component.mIndex;
		}
	}

	private void Start()
	{
		XLeft = PointLeft.transform.position.x;
		XLeftMiddle = PointLeftMiddle.transform.position.x;
		XMiddle = PointMiddle.transform.position.x;
		XRightMiddle = PointRightMiddle.transform.position.x;
		XRight = PointRight.transform.position.x;
	}

	public void ChangePage(EDailyBonusState state)
	{
		if (state == mUIState)
		{
			return;
		}
		if (!GameApp.GetInstance().GetUserState().DontMindIfRegisted() && state != EDailyBonusState.ANNOUNCEMENT)
		{
			UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_QIANDAO_NOREG"));
			return;
		}
		switch (mUIState)
		{
		case EDailyBonusState.ANNOUNCEMENT:
			pageAnnouncement.SetActive(false);
			tabIconAnnouncement.color = ALPHA_COLOR;
			tabSelectedAnnouncement.gameObject.SetActive(false);
			tabBkAnnouncement.color = ALPHA_COLOR;
			break;
		case EDailyBonusState.LOGIN:
			pageLogin.SetActive(false);
			tabIconLogin.color = ALPHA_COLOR;
			tabSelectedLogin.gameObject.SetActive(false);
			tabBkLogin.color = ALPHA_COLOR;
			break;
		case EDailyBonusState.QUEST:
			pageQuest.SetActive(false);
			tabIconQuest.color = ALPHA_COLOR;
			tabSelectedQuest.gameObject.SetActive(false);
			tabBkQuest.color = ALPHA_COLOR;
			break;
		}
		mUIState = state;
		switch (mUIState)
		{
		case EDailyBonusState.ANNOUNCEMENT:
			pageAnnouncement.SetActive(true);
			tabIconAnnouncement.color = FULL_COLOR;
			tabSelectedAnnouncement.gameObject.SetActive(true);
			ShowAnnouncementPage();
			tabBkAnnouncement.color = FULL_COLOR;
			break;
		case EDailyBonusState.LOGIN:
			pageLogin.SetActive(true);
			tabIconLogin.color = FULL_COLOR;
			tabSelectedLogin.gameObject.SetActive(true);
			ShowLoginPage();
			tabBkLogin.color = FULL_COLOR;
			break;
		case EDailyBonusState.QUEST:
			pageQuest.SetActive(true);
			tabIconQuest.color = FULL_COLOR;
			tabSelectedQuest.gameObject.SetActive(true);
			ShowQuestPage();
			tabBkQuest.color = FULL_COLOR;
			break;
		}
	}

	public void OnButtonClick(GameObject go)
	{
		if (mUIState != EDailyBonusState.ANNOUNCEMENT && go == tabAnnouncement)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			ChangePage(EDailyBonusState.ANNOUNCEMENT);
		}
		else if (mUIState != EDailyBonusState.LOGIN && go == tabLogin)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			ChangePage(EDailyBonusState.LOGIN);
		}
		else if (mUIState != EDailyBonusState.QUEST && go == tabQuest)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			ChangePage(EDailyBonusState.QUEST);
		}
		else if (go == exit)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			if (LobbyScript.mInstance != null)
			{
				if (!DailyBonusManager.GetInstance().GetHasRecievedReward() || DailyBonusManager.GetInstance().HasNewAnnouncement)
				{
					LobbyScript.mInstance.DailyBonusAlpha.enabled = true;
				}
				else
				{
					LobbyScript.mInstance.DailyBonusAlpha.enabled = false;
				}
			}
			base.gameObject.SetActive(false);
		}
		else if (go == rewardInfoButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			HideRewardInfo();
		}
		else if (go == questFinalReward)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			SendGetDailyQuestRewardRequest();
		}
	}

	public void ShowLoginPage()
	{
		pageLogin.SetActive(true);
		RefreshLoginPageList();
	}

	public void ShowQuestPage()
	{
		pageQuest.SetActive(true);
		RefreshQuestPageList();
	}

	public void ShowAnnouncementPage()
	{
		pageAnnouncement.SetActive(true);
		RefreshAnnouncement();
	}

	public void RefreshQuestPageList()
	{
		for (int i = 0; i < m_DailyQuestInfoList.Count; i++)
		{
			UnityEngine.Object.DestroyImmediate(m_DailyQuestInfoList[i].gameObject);
		}
		bool flag = true;
		m_DailyQuestInfoList = new List<UIDailyQuestInfo>();
		if (DailyBonusManager.GetInstance().GetQuestList() == null)
		{
			return;
		}
		for (int j = 0; j < DailyBonusManager.GetInstance().GetQuestList().Count; j++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(questTemplate) as GameObject;
			gameObject.SetActive(true);
			gameObject.transform.parent = questList.transform;
			gameObject.transform.localPosition = new Vector3(0f, (float)j * -70f, 0f);
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			UIDailyQuestInfo component = gameObject.GetComponent<UIDailyQuestInfo>();
			component.Init(DailyBonusManager.GetInstance().GetQuestList()[j]);
			m_DailyQuestInfoList.Add(component);
			if (DailyBonusManager.GetInstance().GetQuestList()[j].hasDone == 0)
			{
				flag = false;
			}
		}
		if (flag)
		{
			questFinalReward.SetActive(true);
		}
		else
		{
			questFinalReward.SetActive(false);
		}
	}

	public void RefreshLoginPageList()
	{
		if (templateCount == 0)
		{
			mSelectedIndex = DailyBonusManager.GetInstance().GetLoginDays() - 1;
			mCenterOnChild = dragPanel.GetComponent<UICenterOnChild>();
			mCenterOnChild.onFinished = DragFinished;
			templateCount = 30;
			for (int i = 0; i < templateCount; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(loginTemplate) as GameObject;
				gameObject.SetActive(true);
				gameObject.transform.parent = dragPanel.transform;
				gameObject.transform.localPosition = new Vector3((float)(i - mSelectedIndex) * loginTemplateSpace, 0f, 0f);
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				UIDailyLoginReward component = gameObject.GetComponent<UIDailyLoginReward>();
				component.Init(i);
				m_DailyLoginRewards.Add(component);
			}
			mCenterOnChild.Recenter();
			UIDailyLoginReward component2 = mCenterOnChild.centeredObject.GetComponent<UIDailyLoginReward>();
			component2.Select();
		}
	}

	public void RefreshAnnouncement()
	{
		announcementLabel.text = DailyBonusManager.GetInstance().GetAnnouncement();
		DailyBonusManager.GetInstance().HasNewAnnouncement = false;
	}

	public void ArrangeIcons()
	{
		for (int i = 0; i < m_DailyLoginRewards.Count; i++)
		{
			float x = m_DailyLoginRewards[i].transform.position.x;
			float num = 0f;
			float num2 = 0f;
			float num3 = 1f;
			float num4 = 1f;
			float num5 = 1f;
			float num6 = 1f;
			bool flag = false;
			if (XLeft <= x && x <= XLeftMiddle)
			{
				num = XLeft;
				num2 = XLeftMiddle;
				num3 = 0.7f;
				num4 = 0.85f;
				num5 = 0.3f;
				num6 = 0.5f;
			}
			else if (XLeftMiddle < x && x <= XMiddle)
			{
				num = XLeftMiddle;
				num2 = XMiddle;
				num3 = 0.85f;
				num4 = 1f;
				num5 = 0.5f;
				num6 = 1f;
			}
			else if (XMiddle < x && x <= XRightMiddle)
			{
				num = XMiddle;
				num2 = XRightMiddle;
				num3 = 0.85f;
				num4 = 1f;
				flag = true;
				num5 = 0.5f;
				num6 = 1f;
			}
			else if (XRightMiddle < x && x <= XRight)
			{
				num = XRightMiddle;
				num2 = XRight;
				num3 = 0.7f;
				num4 = 0.85f;
				flag = true;
				num5 = 0.3f;
				num6 = 0.5f;
			}
			else
			{
				num = x;
				num2 = x + 1f;
				num3 = 0.7f;
				num4 = 0.7f;
				num5 = 0.3f;
				num6 = 0.3f;
			}
			float t = ((!flag) ? ((x - num) / (num2 - num)) : (1f - (x - num) / (num2 - num)));
			float num7 = Mathf.Lerp(num3, num4, t);
			float alpha = Mathf.Lerp(num5, num6, t);
			m_DailyLoginRewards[i].transform.localScale = Vector3.one * num7;
			m_DailyLoginRewards[i].SetAlpha(alpha);
		}
	}

	public void SetNeedUpdate()
	{
		needUpdate = true;
	}

	public void DragFinished()
	{
		needUpdate = false;
	}

	public void SetFocusIndex(int newIndex)
	{
		mSelectedIndex = newIndex;
	}

	public void ShowLoginRewardReceived(byte days)
	{
		if (m_DailyLoginRewards.Count > days - 1 && m_DailyLoginRewards[days - 1] != null)
		{
			m_DailyLoginRewards[days - 1].SetButton(true);
		}
	}

	public void ShowRewardInfo(byte rewardType, short rewardId, int rewardValue)
	{
		rewardInfoObj.SetActive(true);
		rewardInfoPanel.transform.localPosition = new Vector3(0f, 0f, 0f);
		if (m_DailyRewardInfo != null)
		{
			foreach (UIDailyRewardInfo item in m_DailyRewardInfo)
			{
				item.gameObject.SetActive(false);
				UnityEngine.Object.DestroyImmediate(item.gameObject);
			}
		}
		m_DailyRewardInfo = new List<UIDailyRewardInfo>();
		int num = 1;
		int num2 = 0;
		int num3 = 150;
		int num4 = 150;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				num2++;
				GameObject gameObject = UnityEngine.Object.Instantiate(rewardInfoTemplate) as GameObject;
				gameObject.SetActive(true);
				gameObject.transform.parent = rewardInfoPanel.transform;
				gameObject.transform.localPosition = new Vector3(i * num3, j * num4, 0f);
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				UIDailyRewardInfo component = gameObject.GetComponent<UIDailyRewardInfo>();
				component.Init(rewardType, rewardId, rewardValue);
				m_DailyRewardInfo.Add(component);
				if (num2 == num)
				{
					break;
				}
			}
			if (num2 == num)
			{
				break;
			}
		}
		rewardInfoPanel.GetComponent<UIGrid>().Reposition();
	}

	public void ShowRewardInfo(List<SwDailyReward> rewardList)
	{
		rewardInfoObj.SetActive(true);
		rewardInfoPanel.transform.localPosition = new Vector3(-150f, 60f, 0f);
		if (m_DailyRewardInfo != null)
		{
			foreach (UIDailyRewardInfo item in m_DailyRewardInfo)
			{
				item.gameObject.SetActive(false);
				UnityEngine.Object.DestroyImmediate(item.gameObject);
			}
		}
		m_DailyRewardInfo = new List<UIDailyRewardInfo>();
		int count = rewardList.Count;
		int num = 0;
		int num2 = 150;
		int num3 = 150;
		for (int i = 0; i < count; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(rewardInfoTemplate) as GameObject;
				gameObject.SetActive(true);
				gameObject.transform.parent = rewardInfoPanel.transform;
				gameObject.transform.localPosition = new Vector3(j * num2, i * num3, 0f);
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				UIDailyRewardInfo component = gameObject.GetComponent<UIDailyRewardInfo>();
				component.Init(rewardList[num].GetRewardType(), rewardList[num].GetRewardId(), rewardList[num].GetRewardValue());
				m_DailyRewardInfo.Add(component);
				num++;
				if (num == count)
				{
					break;
				}
			}
			if (num == count)
			{
				break;
			}
		}
		rewardInfoPanel.GetComponent<UIGrid>().Reposition();
	}

	public void HideRewardInfo()
	{
		rewardInfoObj.SetActive(false);
		foreach (UIDailyRewardInfo item in m_DailyRewardInfo)
		{
			item.gameObject.SetActive(false);
			UnityEngine.Object.DestroyImmediate(item.gameObject);
		}
		m_DailyRewardInfo = null;
	}

	public void RefreshQuestInfo(byte index)
	{
		if (m_DailyQuestInfoList != null && m_DailyQuestInfoList.Count > index && DailyBonusManager.GetInstance().GetQuestList() != null && DailyBonusManager.GetInstance().GetQuestList().Count > index)
		{
			m_DailyQuestInfoList[index].Init(DailyBonusManager.GetInstance().GetQuestList()[index]);
		}
	}

	public void SendGetDailyQuestRewardRequest()
	{
		TryGetDailyQuestFinalRequest request = new TryGetDailyQuestFinalRequest();
		GameApp.GetInstance().GetNetworkManager().SendRequest(request);
	}
}
