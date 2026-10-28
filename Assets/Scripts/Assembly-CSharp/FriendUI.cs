using System;
using System.Collections.Generic;
using UnityEngine;

public class FriendUI : MonoBehaviour, UIMsgListener
{
	public enum FriendUIPageType
	{
		FriendList = 0,
		FriendSearch = 1,
		FriendInvitation = 2,
		TypeCount = 3
	}

	public static FriendUI mInstance;

	public UIWidgetSet ListButton;

	public UIWidgetSet SearchButton;

	public UIWidgetSet InvitationButton;

	public GameObject CloseButton;

	public GameObject ListPage;

	public GameObject SearchPage;

	public GameObject InvitationPage;

	public UICamera FriendUICamera;

	public UIGrid FriendList;

	public GameObject FriendTemplate;

	public UIInput SearchName;

	public GameObject SearchFriendButton;

	public UIGrid InvitationList;

	public GameObject InvitationTemplate;

	public GameObject InvitationCountObj;

	public UILabel InvitationCount;

	private FriendUIPageType mCurrentPage = FriendUIPageType.TypeCount;

	private UserState mUserState;

	private List<FriendContent> mFriendContents = new List<FriendContent>();

	private FriendContent mSelectFriend;

	private List<FriendInvitationContent> mInvitationContents = new List<FriendInvitationContent>();

	private bool mIsForOfflineSummon;

	private bool mOpenFromLobbyNotify;

	private void Awake()
	{
		mInstance = this;
		UIEventListener uIEventListener = UIEventListener.Get(ListButton.gameObject);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(SearchButton.gameObject);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(InvitationButton.gameObject);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener4 = UIEventListener.Get(CloseButton);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener5 = UIEventListener.Get(SearchFriendButton);
		uIEventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener5.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		mIsForOfflineSummon = false;
		if (InterMissionScript.GetInstance() != null)
		{
			InterMissionScript.GetInstance().ShareUICamera.enabled = false;
			FriendUICamera.GetComponent<Camera>().depth = InterMissionScript.GetInstance().ShareUICamera.GetComponent<Camera>().depth + 1f;
		}
	}

	private void OnDestroy()
	{
		if (InterMissionScript.GetInstance() != null)
		{
			InterMissionScript.GetInstance().ShareUICamera.enabled = true;
		}
		mInstance = null;
	}

	private void Start()
	{
		mUserState = GameApp.GetInstance().GetUserState();
		ListButton.SetAlpha(0.5f);
		SearchButton.SetAlpha(0.5f);
		InvitationButton.SetAlpha(0.5f);
		ListPage.SetActive(false);
		SearchPage.SetActive(false);
		InvitationPage.SetActive(false);
		ChangePage(FriendUIPageType.FriendList);
		RefreshPage();
	}

	private void CheckFriendRemainCount()
	{
		Dictionary<int, OnlineFriend> friendDictionary = mUserState.GetFriendDictionary();
		Dictionary<int, int> friendRemainDictionary = mUserState.GetFriendRemainDictionary();
		bool needResetFriendTime = mUserState.GetNeedResetFriendTime();
		foreach (OnlineFriend value in friendDictionary.Values)
		{
			if (friendRemainDictionary.ContainsKey(value.GetUserId()))
			{
				if (needResetFriendTime && value.GetMaxCount() > friendRemainDictionary[value.GetUserId()])
				{
					friendRemainDictionary[value.GetUserId()] = value.GetMaxCount();
				}
			}
			else
			{
				friendRemainDictionary.Add(value.GetUserId(), value.GetMaxCount());
			}
		}
		if (needResetFriendTime)
		{
			mUserState.SetNeedResetFriendTime(false);
		}
		List<int> list = new List<int>();
		foreach (int key in friendRemainDictionary.Keys)
		{
			if (!friendDictionary.ContainsKey(key))
			{
				list.Add(key);
			}
		}
		foreach (int item in list)
		{
			friendRemainDictionary.Remove(item);
		}
	}

	public void SetForOfflineSummon()
	{
		mIsForOfflineSummon = true;
		SearchButton.gameObject.SetActive(false);
		InvitationButton.gameObject.SetActive(false);
	}

	private void OnButtonClick(GameObject go)
	{
		if (go == ListButton.gameObject)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			ChangePage(FriendUIPageType.FriendList);
			return;
		}
		if (go == SearchButton.gameObject)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			ChangePage(FriendUIPageType.FriendSearch);
			return;
		}
		if (go == InvitationButton.gameObject)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			ChangePage(FriendUIPageType.FriendInvitation);
			return;
		}
		if (go == CloseButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		if (go == SearchFriendButton)
		{
			if (SearchName.value != string.Empty)
			{
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
				SwFriendSearchRequest request = new SwFriendSearchRequest(SearchName.value);
				GameApp.GetInstance().GetWWWManager().SendRequest(request);
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_23"), 7);
			}
			return;
		}
		for (int i = 0; i < mFriendContents.Count; i++)
		{
			if (!(go == mFriendContents[i].gameObject))
			{
				continue;
			}
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (mFriendContents[i] == mSelectFriend)
			{
				mSelectFriend.SetHighlight(false);
				mSelectFriend = null;
				continue;
			}
			if (mSelectFriend != null)
			{
				mSelectFriend.SetHighlight(false);
			}
			mSelectFriend = mFriendContents[i];
			mSelectFriend.SetHighlight(true);
		}
	}

	public void ChangePage(FriendUIPageType page)
	{
		if (mCurrentPage != page)
		{
			switch (mCurrentPage)
			{
			case FriendUIPageType.FriendList:
				ListPage.SetActive(false);
				ListButton.SetAlpha(0.5f);
				mSelectFriend = null;
				break;
			case FriendUIPageType.FriendSearch:
				SearchPage.SetActive(false);
				SearchButton.SetAlpha(0.5f);
				break;
			case FriendUIPageType.FriendInvitation:
				InvitationPage.SetActive(false);
				InvitationButton.SetAlpha(0.5f);
				break;
			}
			switch (page)
			{
			case FriendUIPageType.FriendList:
				ListPage.SetActive(true);
				ListButton.SetAlpha(1f);
				InitFriendList();
				break;
			case FriendUIPageType.FriendSearch:
				SearchPage.SetActive(true);
				SearchButton.SetAlpha(1f);
				break;
			case FriendUIPageType.FriendInvitation:
				InvitationPage.SetActive(true);
				InvitationButton.SetAlpha(1f);
				InitInvitation();
				break;
			}
			mCurrentPage = page;
		}
	}

	public void RefreshPage()
	{
		List<OnlineFriend> requestList = mUserState.GetRequestList();
		if (requestList.Count > 0)
		{
			InvitationCountObj.SetActive(true);
			InvitationCount.text = requestList.Count.ToString();
		}
		else
		{
			InvitationCountObj.SetActive(false);
		}
		switch (mCurrentPage)
		{
		case FriendUIPageType.FriendList:
			InitFriendList();
			break;
		case FriendUIPageType.FriendSearch:
			SearchName.value = string.Empty;
			break;
		case FriendUIPageType.FriendInvitation:
			InitInvitation();
			break;
		}
	}

	public void InitFriendList()
	{
		foreach (FriendContent mFriendContent in mFriendContents)
		{
			UnityEngine.Object.DestroyImmediate(mFriendContent.gameObject);
		}
		mFriendContents.Clear();
		CheckFriendRemainCount();
		Dictionary<int, OnlineFriend> friendDictionary = mUserState.GetFriendDictionary();
		Dictionary<int, int> friendRemainDictionary = mUserState.GetFriendRemainDictionary();
		foreach (KeyValuePair<int, OnlineFriend> item in friendDictionary)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(FriendTemplate) as GameObject;
			gameObject.transform.parent = FriendList.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			UIEventListener uIEventListener = UIEventListener.Get(gameObject.gameObject);
			uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
			FriendContent component = gameObject.GetComponent<FriendContent>();
			component.Init(item.Value, mIsForOfflineSummon, friendRemainDictionary[item.Key]);
			mFriendContents.Add(component);
		}
		FriendList.Reposition();
	}

	public void InitInvitation()
	{
		foreach (FriendInvitationContent mInvitationContent in mInvitationContents)
		{
			UnityEngine.Object.DestroyImmediate(mInvitationContent.gameObject);
		}
		mInvitationContents.Clear();
		List<OnlineFriend> requestList = mUserState.GetRequestList();
		foreach (OnlineFriend item in requestList)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(InvitationTemplate) as GameObject;
			gameObject.transform.parent = InvitationList.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			FriendInvitationContent component = gameObject.GetComponent<FriendInvitationContent>();
			component.Init(item);
			mInvitationContents.Add(component);
		}
		InvitationList.Reposition();
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (buttonId == UIMsg.UIMsgButton.Ok)
		{
			UIMsgBox.instance.CloseMessage();
		}
	}

	public void SetOpenFromNotify()
	{
		mOpenFromLobbyNotify = true;
	}

	private void Update()
	{
		if (mOpenFromLobbyNotify)
		{
			mOpenFromLobbyNotify = false;
			ChangePage(FriendUIPageType.FriendInvitation);
		}
	}
}
