using System;
using UnityEngine;

public class LoadoutUIScript : MonoBehaviour, UIMsgListener
{
	public static LoadoutUIScript Instance;

	public GameObject ExitButton;

	public LoadoutPageScript LoadoutPage;

	public SkillPageScript SkillPage;

	public int CurrentVsTabId;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		UIEventListener uIEventListener = UIEventListener.Get(ExitButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
	}

	public void OnButtonClick(GameObject go)
	{
		if (go == ExitButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			if (CustomizeUIScript.mInstance != null && CustomizeUIScript.mInstance.gameObject.activeSelf)
			{
				LoadoutPage.ReadyToLeaveCustomize();
				return;
			}
			SaveCurrentCustomize();
			HideLoadoutUI();
			LobbyScript.mInstance.ShowLobbyUI();
			UploadLoadoutRequest request = new UploadLoadoutRequest();
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			UploadStoreDataRequest request2 = new UploadStoreDataRequest(GameApp.GetInstance().GetUserState());
			GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
		}
	}

	public void Leave()
	{
		if (CustomizeUIScript.mInstance != null && CustomizeUIScript.mInstance.gameObject.activeSelf)
		{
			LoadoutPage.LeaveCustomize();
		}
		SaveCurrentCustomize();
		HideLoadoutUI();
		LobbyScript.mInstance.ShowLobbyUI();
		UploadLoadoutRequest request = new UploadLoadoutRequest();
		GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		UploadStoreDataRequest request2 = new UploadStoreDataRequest(GameApp.GetInstance().GetUserState());
		GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
	}

	private void Update()
	{
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
	}

	public void OpenSkillPage()
	{
		LoadoutPage.HideLoadoutPage();
		SkillPage.ShowSkillPage();
	}

	public void OpenLoadoutPage()
	{
		LoadoutPage.ShowLoadoutPage();
		SkillPage.HideSkillPage();
	}

	public void ShowLoadoutUI()
	{
		base.gameObject.SetActive(true);
		OpenLoadoutPage();
		TutorialManager.GetInstance().Trigger(TutorialTriggerType.FIRST_COMBAT);
	}

	public void HideLoadoutUI()
	{
		LoadoutPage.HideLoadoutPage();
		SkillPage.HideSkillPage();
		base.gameObject.SetActive(false);
	}

	public void SaveCurrentCustomize()
	{
		LoadoutPage.SaveCurrentCustomize();
	}
}
