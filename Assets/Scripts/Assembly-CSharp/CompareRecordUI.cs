using System;
using UnityEngine;

public class CompareRecordUI : MonoBehaviour
{
	public static CompareRecordUI mInstance;

	public PlayerRecordDataInfo CloudInfo;

	public PlayerRecordDataInfo LocalInfo;

	public GameObject GetCloudButton;

	public UISprite CloudHighlight;

	public GameObject UseLocalButton;

	public UISprite LocalHighlight;

	public GameObject CancelButton;

	public bool IsRegisted;

	private void Awake()
	{
		mInstance = this;
		UseLocalButton.SetActive(false);
		UIEventListener uIEventListener = UIEventListener.Get(GetCloudButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(GetCloudButton);
		uIEventListener2.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener2.onPress, new UIEventListener.BoolDelegate(OnButtonPressed));
		UIEventListener uIEventListener3 = UIEventListener.Get(CancelButton);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UserState userState = GameApp.GetInstance().GetUserState();
		if (userState != null)
		{
			if (MainMenuScript.mInstance != null)
			{
				CharacterShaderController.TempData = MainMenuScript.mInstance.GetRecordFromServer().GetRecordData().mCustomizeData.mAvatarColorArray;
				LocalInfo.InitByUserState(userState);
				CloudInfo.InitByRecordData(MainMenuScript.mInstance.GetRecordFromServer().GetRecordData());
			}
			else if (MainMenu2DScript.mInstance != null)
			{
				CharacterShaderController.TempData = MainMenu2DScript.mInstance.GetRecordFromServer().GetRecordData().mCustomizeData.mAvatarColorArray;
				LocalInfo.InitByUserState(userState);
				CloudInfo.InitByRecordData(MainMenu2DScript.mInstance.GetRecordFromServer().GetRecordData());
			}
		}
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	private void OnButtonClick(GameObject go)
	{
		if (go == GetCloudButton)
		{
			if (MainMenuScript.mInstance != null)
			{
				MainMenuScript.mInstance.ResetRecordFromServer();
				MainMenuScript.mInstance.UpdateOrDownloadDataOver();
			}
			else if (MainMenu2DScript.mInstance != null)
			{
				MainMenu2DScript.mInstance.ResetRecordFromServer();
				MainMenu2DScript.mInstance.UpdateOrDownloadDataOver();
			}
			if (!IsRegisted)
			{
				return;
			}
			NetworkManager networkManager = GameApp.GetInstance().GetNetworkManager();
			if (networkManager != null)
			{
				networkManager.CloseConnection();
			}
			if (MainMenuScript.mInstance != null)
			{
				if (LoginPageScript.mInstance != null && LoginPageScript.mInstance.RegisterPage.activeSelf)
				{
					UIMsgBox.instance.ShowMessage(MainMenuScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_29"), 1, 21);
				}
				else
				{
					UIMsgBox.instance.ShowMessage(MainMenuScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_30"), 1, 21);
				}
			}
			else if (MainMenu2DScript.mInstance != null)
			{
				if (LoginPageScript.mInstance != null && LoginPageScript.mInstance.RegisterPage.activeSelf)
				{
					UIMsgBox.instance.ShowMessage(MainMenu2DScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_29"), 1, 21);
				}
				else
				{
					UIMsgBox.instance.ShowMessage(MainMenu2DScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_30"), 1, 21);
				}
			}
		}
		else if (go == UseLocalButton)
		{
			if (MainMenuScript.mInstance != null)
			{
				MainMenuScript.mInstance.UpLoadData();
				MainMenuScript.mInstance.UpdateOrDownloadDataOver();
			}
			else if (MainMenu2DScript.mInstance != null)
			{
				MainMenu2DScript.mInstance.UpLoadData();
				MainMenu2DScript.mInstance.UpdateOrDownloadDataOver();
			}
		}
		else if (go == CancelButton)
		{
			if (GameApp.GetInstance().GetNetworkManager() != null)
			{
				GameApp.GetInstance().GetNetworkManager().CloseConnection();
			}
			if (MainMenuScript.mInstance != null)
			{
				GameApp.GetInstance().GetUserState().CheckNeedResetFriendTime(MainMenuScript.mInstance.bNeedResetFriendTime);
				MainMenuScript.mInstance.OnLoginFail();
			}
			else if (MainMenu2DScript.mInstance != null)
			{
				GameApp.GetInstance().GetUserState().CheckNeedResetFriendTime(MainMenu2DScript.mInstance.bNeedResetFriendTime);
				MainMenu2DScript.mInstance.OnLoginFail();
			}
			if (OptionUIScript.mInstance != null)
			{
				OptionUIScript.mInstance.Exit();
			}
			GameApp.GetInstance().Save();
			UnityEngine.Object.DestroyImmediate(base.gameObject);
		}
	}

	private void OnButtonPressed(GameObject go, bool isPress)
	{
		if (go == GetCloudButton)
		{
			CloudHighlight.gameObject.SetActive(isPress);
		}
		else if (go == UseLocalButton)
		{
			LocalHighlight.gameObject.SetActive(isPress);
		}
	}

	public void ShowUseLocalButton(bool canUpload)
	{
		if (canUpload)
		{
			UseLocalButton.SetActive(true);
			UIEventListener uIEventListener = UIEventListener.Get(UseLocalButton);
			uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
			UIEventListener uIEventListener2 = UIEventListener.Get(UseLocalButton);
			uIEventListener2.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener2.onPress, new UIEventListener.BoolDelegate(OnButtonPressed));
		}
		else
		{
			UseLocalButton.SetActive(false);
		}
	}
}
