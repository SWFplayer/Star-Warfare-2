using System;
using UnityEngine;

public class NewPasswordPageScript : MonoBehaviour
{
	public GameObject exitBtn;

	public GameObject LoginBtn;

	public GameObject CancelBtn;

	public UIInput Input_LoginPassword;

	public UILabel Register_PasswordResult;

	private bool isEmptyPassword;

	private void Start()
	{
		UIEventListener uIEventListener = UIEventListener.Get(exitBtn);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(LoginBtn);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(CancelBtn);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener4 = UIEventListener.Get(Input_LoginPassword.gameObject);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		isEmptyPassword = false;
	}

	private void Update()
	{
	}

	private void OnButtonClick(GameObject go)
	{
		if (go == exitBtn || go == CancelBtn)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			if (MainMenu2DScript.mInstance != null)
			{
				MainMenu2DScript.mInstance.OnLoginFail();
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else if (go == LoginBtn)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			CheckPassword();
			if (!isEmptyPassword && MainMenu2DScript.mInstance != null)
			{
				GameApp.GetInstance().GetUserState().SetOnlinePassword(Input_LoginPassword.value);
				MainMenu2DScript.mInstance.Login();
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
		else if (go == Input_LoginPassword.gameObject)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			Register_PasswordResult.text = string.Empty;
		}
	}

	public void CheckPassword()
	{
		if (Input_LoginPassword.value.Length == 0)
		{
			Register_PasswordResult.text = LocalizationManager.GetInstance().GetString("LOC_MESSEGE_15");
			Register_PasswordResult.color = Color.red;
			isEmptyPassword = true;
		}
		else
		{
			isEmptyPassword = false;
		}
	}
}
