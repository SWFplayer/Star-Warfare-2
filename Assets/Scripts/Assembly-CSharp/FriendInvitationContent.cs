using System;
using UnityEngine;

public class FriendInvitationContent : MonoBehaviour
{
	public UILabel PlayerName;

	public UISprite Head;

	public UISprite RankIcon;

	public UILabel RankLevel;

	public GameObject AcceptButton;

	public GameObject RejectButton;

	public GameObject AcceptHighlight;

	public GameObject RejectHighlight;

	private OnlineFriend mFriend;

	private void Awake()
	{
		UIEventListener uIEventListener = UIEventListener.Get(AcceptButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(RejectButton);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(AcceptButton);
		uIEventListener3.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener3.onPress, new UIEventListener.BoolDelegate(OnButtonPress));
		UIEventListener uIEventListener4 = UIEventListener.Get(RejectButton);
		uIEventListener4.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener4.onPress, new UIEventListener.BoolDelegate(OnButtonPress));
	}

	public void Init(OnlineFriend friend)
	{
		mFriend = friend;
		PlayerName.text = mFriend.GetName();
		int rankID = GameApp.GetInstance().GetUserState().GetRank(mFriend.GetExp())
			.rankID;
		RankIcon.spriteName = Rank.GetRankIconName(rankID);
		RankLevel.text = Rank.GetRankLevelString(rankID);
	}

	private void OnButtonClick(GameObject go)
	{
		if (go == AcceptButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			SwFriendAcceptRequest request = new SwFriendAcceptRequest(mFriend.GetUserId());
			GameApp.GetInstance().GetWWWManager().SendRequest(request);
			if (FriendUI.mInstance != null)
			{
				UIMsgBox.instance.ShowMessage(FriendUI.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_22"), 7);
			}
		}
		else if (go == RejectButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			SwFriendIgnoreRequest request2 = new SwFriendIgnoreRequest(mFriend.GetUserId());
			GameApp.GetInstance().GetWWWManager().SendRequest(request2);
			GameApp.GetInstance().GetUserState().RemoveRequest(mFriend);
			if (FriendUI.mInstance != null)
			{
				FriendUI.mInstance.RefreshPage();
			}
		}
	}

	private void OnButtonPress(GameObject go, bool isPress)
	{
		if (go == AcceptButton)
		{
			AcceptHighlight.SetActive(isPress);
		}
		else if (go == RejectButton)
		{
			RejectHighlight.SetActive(isPress);
		}
	}
}
