using System;
using UnityEngine;

public class QuickRegisterScript : MonoBehaviour
{
	public GameObject QuickRegister;

	public GameObject DetailRegister;

	public UILabel QuickPlayTitle;

	public UILabel QuickPlayTips;

	public UILabel RegisterTitle;

	public UILabel RegisterTips;

	public bool HasGuestAccount;

	public int UserId;

	public bool IsFromOption;

	private void Start()
	{
		UIEventListener uIEventListener = UIEventListener.Get(QuickRegister);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(DetailRegister);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		if (HasGuestAccount)
		{
			QuickPlayTitle.text = LocalizationManager.GetInstance().GetString("LOC_QUICKPLAY_02");
			QuickPlayTips.text = LocalizationManager.GetInstance().GetString("LOC_QUICKPLAY_TIPS_02", "Guest" + UserId);
			RegisterTitle.text = LocalizationManager.GetInstance().GetString("LOC_ACCOUNTPLAY_02");
			RegisterTips.text = LocalizationManager.GetInstance().GetString("LOC_ACCOUNTPLAY_TIPS_02");
			if (IsFromOption)
			{
				OnButtonClick(DetailRegister);
			}
		}
		else
		{
			QuickPlayTitle.text = LocalizationManager.GetInstance().GetString("LOC_QUICKPLAY_01");
			QuickPlayTips.text = LocalizationManager.GetInstance().GetString("LOC_QUICKPLAY_TIPS_01");
			RegisterTitle.text = LocalizationManager.GetInstance().GetString("LOC_ACCOUNTPLAY_01");
			RegisterTips.text = LocalizationManager.GetInstance().GetString("LOC_ACCOUNTPLAY_TIPS_01");
		}
	}

	private void OnButtonClick(GameObject go)
	{
		if (go == QuickRegister)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			LoginPageScript.mTurnToRegister = false;
			if (MainMenu2DScript.mInstance != null)
			{
				MainMenu2DScript.mInstance.GoMultiMenuForQuickRegist();
			}
			if (IsFromOption && OptionUIScript.mInstance != null)
			{
				OptionUIScript.mInstance.Exit();
			}
			UnityEngine.Object.Destroy(base.gameObject);
			UIMsgBox.instance.CloseMessage();
		}
		else if (go == DetailRegister)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (IsFromOption)
			{
				OptionMainPageScript.mInstance.OpenAccountPage();
			}
			else if (MainMenu2DScript.mInstance != null)
			{
				MainMenu2DScript.mInstance.OnButtonClick(MainMenu2DScript.mInstance.OptionButton);
				LoginPageScript.mTurnToRegister = false;
				LoginPageScript.mTurnToAccount = true;
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
