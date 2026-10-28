using System;
using UnityEngine;

public class OptionMainPageScript : MonoBehaviour, UIMsgListener
{
	public static OptionMainPageScript mInstance;

	public GameObject ControlBtn;

	public GameObject AudioBtn;

	public GameObject VideoBtn;

	public GameObject AccountBtn;

	public GameObject CreditsBtn;

	public GameObject ContactBtn;

	public GameObject LanguageBtn;

	public GameObject TutorialBtn;

	public GameObject PrivacyBtn;

	public GameObject AccountPage;

	public GameObject ControlPage;

	public GameObject AudioPage;

	public GameObject VideoPage;

	public GameObject CreditsPage;

	public GameObject ContactPage;

	public GameObject LanguagePage;

	public GameObject BackBtn;

	public GameObject QualitySettingBtn;

	public TweenPosition StaffRollTween;

	private GameObject CurrentPage;

	private bool mIsMainMenu;

	public GameObject CheatBtn;

	public GameObject EnableIap;

	private void Start()
	{
		mInstance = this;
		BackBtn.SetActive(false);
		UIEventListener uIEventListener = UIEventListener.Get(ControlBtn);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(AudioBtn);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(VideoBtn);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener4 = UIEventListener.Get(AccountBtn);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener5 = UIEventListener.Get(CreditsBtn);
		uIEventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener5.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener6 = UIEventListener.Get(ContactBtn);
		uIEventListener6.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener6.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener7 = UIEventListener.Get(BackBtn);
		uIEventListener7.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener7.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener8 = UIEventListener.Get(CheatBtn);
		uIEventListener8.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener8.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener9 = UIEventListener.Get(LanguageBtn);
		uIEventListener9.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener9.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener10 = UIEventListener.Get(TutorialBtn);
		uIEventListener10.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener10.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener11 = UIEventListener.Get(QualitySettingBtn);
		uIEventListener11.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener11.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener12 = UIEventListener.Get(PrivacyBtn);
		uIEventListener12.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener12.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener13 = UIEventListener.Get(EnableIap);
		uIEventListener13.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener13.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		if (LoginPageScript.mTurnToLogin)
		{
			ChangeToAccountPage();
		}
		else if (LoginPageScript.mTurnToRegister)
		{
			OnButtonClick(AccountBtn);
		}
		else if (LoginPageScript.mTurnToAccount)
		{
			OpenAccountPage();
			LoginPageScript.mTurnToAccount = false;
		}
		if (!mIsMainMenu)
		{
			VideoBtn.transform.Find("Label").GetComponent<UILabel>().color = new Color32(100, 100, 100, byte.MaxValue);
			AccountBtn.transform.Find("Label").GetComponent<UILabel>().color = new Color32(100, 100, 100, byte.MaxValue);
			ContactBtn.transform.Find("Label").GetComponent<UILabel>().color = new Color32(100, 100, 100, byte.MaxValue);
			LanguageBtn.transform.Find("Label").GetComponent<UILabel>().color = new Color32(100, 100, 100, byte.MaxValue);
		}
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	public void SetCurrentPage(GameObject page)
	{
		CurrentPage = page;
	}

	public void OpenAccountPage()
	{
		if (!LoginPageScript.mTurnToRegister)
		{
			BackBtn.SetActive(true);
		}
		AccountPage.SetActive(true);
		CurrentPage = AccountPage;
		base.gameObject.SetActive(false);
	}

	private void OnButtonClick(GameObject go)
	{
		if (go == ControlBtn)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			ControlPage.SetActive(true);
			CurrentPage = ControlPage;
			BackBtn.SetActive(true);
			base.gameObject.SetActive(false);
		}
		else if (go == AudioBtn)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			AudioPage.SetActive(true);
			CurrentPage = AudioPage;
			BackBtn.SetActive(true);
			base.gameObject.SetActive(false);
		}
		else if (go == VideoBtn)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (mIsMainMenu)
			{
				VideoPage.SetActive(true);
				CurrentPage = VideoPage;
				BackBtn.SetActive(true);
				base.gameObject.SetActive(false);
			}
			else
			{
				ShowUnableMsg();
			}
		}
		else if (go == AccountBtn)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (mIsMainMenu)
			{
				if (GameApp.GetInstance().GetUserState().GetOnlineRegisted())
				{
					OpenAccountPage();
				}
				else if (GameApp.GetInstance().IsConnectedToInternet())
				{
					SwAccountUdidCheckRequest request = new SwAccountUdidCheckRequest(true);
					GameApp.GetInstance().GetWWWManager().SendRequest(request);
					if (!UIMsgBox.instance.IsMessageShow() && GameObject.Find("TutorialUI(Clone)") == null)
					{
						UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_11"), 7);
					}
				}
				else
				{
					UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_6"));
				}
			}
			else
			{
				ShowUnableMsg();
			}
		}
		else if (go == CreditsBtn)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			StaffRollTween.Reset();
			StaffRollTween.PlayForward();
			CreditsPage.SetActive(true);
			CurrentPage = CreditsPage;
			BackBtn.SetActive(true);
			base.gameObject.SetActive(false);
		}
		else if (go == ContactBtn)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (mIsMainMenu)
			{
				ContactPage.SetActive(true);
				CurrentPage = ContactPage;
				BackBtn.SetActive(true);
				base.gameObject.SetActive(false);
			}
			else
			{
				ShowUnableMsg();
			}
		}
		else if (go == QualitySettingBtn)
		{
			QualitySettings.SetQualityLevel(1 - QualitySettings.GetQualityLevel());
		}
		else if (go == BackBtn)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			CurrentPage.SetActive(false);
			BackBtn.SetActive(false);
			base.gameObject.SetActive(true);
		}
		else if (go == CheatBtn)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			GameApp.GetInstance().GetUserState().UnlockAllLevel_Weapons_Equips();
			GameApp.GetInstance().GetUserState().SetSkipTutorial(true);
			GameApp.GetInstance().Save();
		}
		else if (go == LanguageBtn)
		{
			if (mIsMainMenu)
			{
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
				LanguagePage.SetActive(true);
				CurrentPage = LanguagePage;
				BackBtn.SetActive(true);
				base.gameObject.SetActive(false);
			}
			else
			{
				ShowUnableMsg();
			}
		}
		else if (go == TutorialBtn)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			TutorialManager.GetInstance().Trigger(TutorialTriggerType.FREE_WATCH);
		}
		else if (go == PrivacyBtn)
		{
			Application.OpenURL("http://www.freyrgames.com/en/yszc.php");
		}
		else if (go == EnableIap)
		{
			Global.ENABLE_IAP = !Global.ENABLE_IAP;
		}
	}

	private void ChangeToAccountPage()
	{
		AccountPage.SetActive(true);
		base.gameObject.SetActive(false);
	}

	private void ShowUnableMsg()
	{
		UIMsgBox.instance.ShowOptionUnabledMessage(this);
	}

	public void SetEnable(bool isMain)
	{
		mIsMainMenu = isMain;
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (whichMsg.EventId == 62)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			UIMsgBox.instance.CloseMessage();
			switch (buttonId)
			{
			case UIMsg.UIMsgButton.Cancel:
				LoginPageScript.mTurnToLogin = true;
				ChangeToAccountPage();
				break;
			case UIMsg.UIMsgButton.Ok:
				if (GameApp.GetInstance().IsConnectedToInternet())
				{
					ChangeToAccountPage();
					SwAccountLoginRequest request = new SwAccountLoginRequest(true);
					GameApp.GetInstance().GetWWWManager().SendRequest(request);
					if (!UIMsgBox.instance.IsMessageShow() && GameObject.Find("TutorialUI(Clone)") == null)
					{
						UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_11"), 7);
					}
				}
				else
				{
					UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_6"));
				}
				break;
			}
		}
		else if (whichMsg.EventId == 16)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				UIMsgBox.instance.CloseMessage();
				if (GameApp.GetInstance().GetUserState().GetOnlineRegisted())
				{
					GameApp.GetInstance().GetUserState().Achievement.CheckAchievement_Register();
				}
				switch (AndroidConstant.version)
				{
				case AndroidConstant.Version.GooglePlay:
					AndroidPluginScript.ShowAppUrl(0);
					break;
				case AndroidConstant.Version.Kindle:
					AndroidPluginScript.ShowAppUrl(1);
					break;
				}
			}
		}
		else
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			UIMsgBox.instance.CloseMessage();
		}
	}
}
