using System;
using UnityEngine;

public class VSSelectionScript : MonoBehaviour
{
	public VSTarget Type;

	public GameObject StartPage;

	public GameObject SelectPage;

	public GameObject QuickJoinButton;

	public GameObject CreateGameButton;

	public GameObject SearchGameButton;

	public GameObject ListGameButton;

	public VSSelectionScript Other;

	public VSRoomUIScript RoomUI;

	public UISprite BonusIcon;

	private void Awake()
	{
		UIEventListener uIEventListener = UIEventListener.Get(StartPage);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(SelectPage);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(QuickJoinButton);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener4 = UIEventListener.Get(CreateGameButton);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener5 = UIEventListener.Get(SearchGameButton);
		uIEventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener5.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener6 = UIEventListener.Get(ListGameButton);
		uIEventListener6.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener6.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		HideSelection();
		CheckBonus();
	}

	public void OnButtonClick(GameObject go)
	{
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
		if (go == StartPage)
		{
			ShowSelection();
			Other.HideSelection();
		}
		else if (go == QuickJoinButton)
		{
			if (Type == VSTarget.PVE)
			{
				RoomUI.OpenPage(RoomPage.QuickJoinPVEPage, RoomRequestType.Join, Type);
			}
			else if (Type == VSTarget.PVP)
			{
				RoomUI.OpenPage(RoomPage.QuickJoinPVPPage, RoomRequestType.Join, Type);
			}
			LobbyScript.mInstance.HideLobbyPage();
		}
		else if (go == CreateGameButton)
		{
			if (Type == VSTarget.PVE)
			{
				RoomUI.OpenPage(RoomPage.MapSelectPage, RoomRequestType.Create, Type);
			}
			else if (Type == VSTarget.PVP)
			{
				RoomUI.OpenPage(RoomPage.ModeSelectPage, RoomRequestType.Create, Type);
			}
			LobbyScript.mInstance.HideLobbyPage();
		}
		else if (go == SearchGameButton)
		{
			RoomUI.OpenPage(RoomPage.SearchPage, RoomRequestType.Join, Type);
			LobbyScript.mInstance.HideLobbyPage();
		}
		else if (go == ListGameButton)
		{
			ListGames();
		}
	}

	public void ShowSelection()
	{
		StartPage.SetActive(false);
		SelectPage.SetActive(true);
	}

	public void HideSelection()
	{
		StartPage.SetActive(true);
		SelectPage.SetActive(false);
	}

	public void ListGames()
	{
		RoomUI.OpenPage(RoomPage.ListPage, RoomRequestType.Join, Type);
		RoomUI.SearchInput.value = string.Empty;
		LobbyScript.mInstance.HideLobbyPage();
	}

	private void CheckBonus()
	{
		if (Type == VSTarget.PVE)
		{
			bool firstBossChallenge = Lobby.GetInstance().FirstBossChallenge;
			BonusIcon.gameObject.SetActive(firstBossChallenge);
		}
	}
}
