using System;
using System.Collections.Generic;
using UnityEngine;

public class FriendContent : MonoBehaviour, UIMsgListener
{
	public UILabel PlayerName;

	public UITexture Head;

	public UISprite HeadFrame;

	public UISprite RankIcon;

	public UILabel RankLevel;

	public GameObject HideWhenSummonObj;

	public UILabel SummonTip;

	public GameObject HighlightOBJ;

	public GameObject InfoButton;

	public GameObject InviteButton;

	public GameObject DeleteButton;

	public GameObject InfoHighlight;

	public GameObject InviteHighlight;

	public GameObject DeleteHighlight;

	public UISprite[] SummonCounts;

	private OnlineFriend mFriend;

	private UIWidgetSet mWidgetSet;

	private SocialPlayer mSocialPlayer;

	private void Awake()
	{
		UIEventListener uIEventListener = UIEventListener.Get(InfoButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(InviteButton);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(DeleteButton);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener4 = UIEventListener.Get(InfoButton);
		uIEventListener4.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener4.onPress, new UIEventListener.BoolDelegate(OnButtonPress));
		UIEventListener uIEventListener5 = UIEventListener.Get(InviteButton);
		uIEventListener5.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener5.onPress, new UIEventListener.BoolDelegate(OnButtonPress));
		UIEventListener uIEventListener6 = UIEventListener.Get(DeleteButton);
		uIEventListener6.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener6.onPress, new UIEventListener.BoolDelegate(OnButtonPress));
		mWidgetSet = GetComponent<UIWidgetSet>();
		SummonTip.gameObject.SetActive(false);
	}

	public void Init(OnlineFriend friend, bool isForOfflineSummon, int remainNum)
	{
		mFriend = friend;
		PlayerName.text = mFriend.GetName();
		int rankID = GameApp.GetInstance().GetUserState().GetRank(mFriend.GetExp())
			.rankID;
		RankIcon.spriteName = Rank.GetRankIconName(rankID);
		RankLevel.text = Rank.GetRankLevelString(rankID);
		if (isForOfflineSummon || friend.GetUserId() == 24)
		{
			DeleteButton.SetActive(false);
		}
		if (!isForOfflineSummon)
		{
			InviteButton.SetActive(false);
		}
		HighlightOBJ.GetComponent<UIGrid>().Reposition();
		SetHighlight(false);
		mSocialPlayer = friend.SocialPlayer;
		SocialManager.GetInstance().LoadSocialData(mSocialPlayer);
		GameWorld.RefreshHeadFrame(HeadFrame, friend.GetFrameType(), friend.GetPrestige());
		for (int i = 0; i < remainNum; i++)
		{
			if (i < SummonCounts.Length)
			{
				SummonCounts[i].spriteName = "friend_n";
			}
		}
	}

	private void OnButtonClick(GameObject go)
	{
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
		if (go == InfoButton)
		{
			GameObject original = Resources.Load("UI/Menu/PlayerInfo/Prefab/PlayerInfoUI") as GameObject;
			GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
			PlayerInfoUI component = gameObject.GetComponent<PlayerInfoUI>();
			component.InitByOnlineFriend(mFriend);
		}
		else if (go == InviteButton)
		{
			Dictionary<int, int> friendRemainDictionary = GameApp.GetInstance().GetUserState().GetFriendRemainDictionary();
			if (friendRemainDictionary[mFriend.GetUserId()] > 0)
			{
				if (MissionSelectUIScript.m_Instance != null)
				{
					MissionSelectUIScript.m_Instance.SetSoloPlay(false);
					GlobalData.GetInstance().Partner = mFriend;
					MissionSelectUIScript.m_Instance.OKClicked();
				}
			}
			else
			{
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_40"), 1, 25);
			}
		}
		else if (go == DeleteButton)
		{
			UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_24"), 2, 22);
		}
	}

	private void OnButtonPress(GameObject go, bool isPress)
	{
		if (go == InviteButton)
		{
			InviteHighlight.SetActive(isPress);
		}
		else if (go == InfoButton)
		{
			InfoHighlight.SetActive(isPress);
		}
		else if (go == DeleteButton)
		{
			DeleteHighlight.SetActive(isPress);
		}
	}

	public void SetHighlight(bool highlight)
	{
		if (highlight)
		{
			mWidgetSet.SetAlpha(1f);
		}
		else
		{
			mWidgetSet.SetAlpha(0.5f);
		}
		HighlightOBJ.SetActive(highlight);
		if (InviteButton.activeSelf)
		{
			SummonTip.gameObject.SetActive(highlight);
			HideWhenSummonObj.SetActive(!highlight);
		}
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (whichMsg.EventId == 22)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				SwFriendRemoveRequest request = new SwFriendRemoveRequest(mFriend.GetUserId());
				GameApp.GetInstance().GetWWWManager().SendRequest(request);
			}
			UIMsgBox.instance.CloseMessage();
		}
		else if (whichMsg.EventId == 25 && buttonId == UIMsg.UIMsgButton.Ok)
		{
			UIMsgBox.instance.CloseMessage();
		}
	}

	private void Update()
	{
		SocialManager.UpdateHead(mSocialPlayer, Head);
	}
}
