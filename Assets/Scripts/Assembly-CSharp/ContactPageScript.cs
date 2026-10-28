using System;
using UnityEngine;

public class ContactPageScript : MonoBehaviour
{
	public static ContactPageScript mInstance;

	public GameObject FacebookBtn;

	public GameObject TwitterBtn;

	public GameObject Tip1;

	public GameObject Tip2;

	public GameObject Contact;

	public GameObject Bug;

	public UILabel DeviceID;

	private bool refreshed;

	private float refreshInterval;

	private string platformString = string.Empty;

	private void Awake()
	{
		mInstance = this;
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	private void Start()
	{
		UIEventListener uIEventListener = UIEventListener.Get(FacebookBtn);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(TwitterBtn);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(Tip1);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener4 = UIEventListener.Get(Tip2);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener5 = UIEventListener.Get(Contact);
		uIEventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener5.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener6 = UIEventListener.Get(Bug);
		uIEventListener6.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener6.onClick, new UIEventListener.VoidDelegate(OnClick));
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			platformString = "[IOS]";
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			platformString = "[Android]";
		}
		else if (Application.platform == RuntimePlatform.WP8Player)
		{
			platformString = "[WP]";
		}
		if (GameApp.GetInstance().GetUserState().GetFollowFacebook())
		{
			FacebookBtn.transform.Find("AddMithril").gameObject.SetActive(false);
		}
		if (GameApp.GetInstance().GetUserState().GetFollowTwitter())
		{
			TwitterBtn.transform.Find("AddMithril").gameObject.SetActive(false);
		}
	}

	private void OnEnable()
	{
		if (GameApp.GetInstance().UUID != null && GameApp.GetInstance().UUID != string.Empty)
		{
			DeviceID.text = GameApp.GetInstance().UUID;
		}
	}

	private void OnClick(GameObject go)
	{
		if (go == FacebookBtn)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			ApplicationPauseManager.mInstance.FacebookClicked = true;
			string url = "https://www.facebook.com/159454344141173";
			Application.OpenURL(url);
		}
		else if (go == TwitterBtn)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			ApplicationPauseManager.mInstance.TwitterClicked = true;
			string url2 = "https://twitter.com/FreyrGames";
			Application.OpenURL(url2);
		}
		else if (go == Tip1)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			OptionUIScript.mInstance.SetTip("LOC_CONTACT_BUG_TIPS");
		}
		else if (go == Tip2)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			OptionUIScript.mInstance.SetTip("LOC_CONTACT_CONTACT_TIPS");
		}
		else if (go == Contact)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			string text = LocalizationManager.GetInstance().GetString("LOC_REPORTMAIL_1") + platformString;
			string text2 = LocalizationManager.GetInstance().GetString("LOC_REPORTMAIL_2");
			string text3 = LocalizationManager.GetInstance().GetString("LOC_REPORTMAIL_3");
			string text4 = "Version 1.27";
			string text5 = "ID: " + GameApp.GetInstance().GetUserState().GetOnlineUserId();
			string text6 = "Language: " + LocalizationManager.GetInstance().currentLanguage;
			string text7 = "Device: " + SystemInfo.deviceModel;
			string text8 = "System: " + SystemInfo.operatingSystem;
			string text9 = "UUID: " + GameApp.GetInstance().UUID;
			string text10 = "mailto:ios@ifreyr.com?subject=" + text + "&body=" + text2 + "%0A%0A%0A%0A%0A%0A" + text3 + "%0A" + text4 + "%0A" + text5 + "%0A" + text6 + "%0A" + text7 + "%0A" + text8 + "%0A" + text9;
			Application.OpenURL(text10.Replace(" ", "%20"));
		}
		else if (go == Bug)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			string text11 = LocalizationManager.GetInstance().GetString("LOC_REPORTMAIL_4") + platformString;
			string text12 = LocalizationManager.GetInstance().GetString("LOC_REPORTMAIL_5");
			string text13 = LocalizationManager.GetInstance().GetString("LOC_REPORTMAIL_6");
			string text14 = "Version 1.27";
			string text15 = "ID: " + GameApp.GetInstance().GetUserState().GetOnlineUserId();
			string text16 = "Language: " + LocalizationManager.GetInstance().currentLanguage;
			string text17 = "Device: " + SystemInfo.deviceModel;
			string text18 = "System: " + SystemInfo.operatingSystem;
			string text19 = "UUID: " + GameApp.GetInstance().UUID;
			string text20 = "mailto:ios@ifreyr.com?subject=" + text11 + "&body=" + text12 + "%0A%0A%0A%0A%0A%0A" + text13 + "%0A" + text14 + "%0A" + text15 + "%0A" + text16 + "%0A" + text17 + "%0A" + text18 + "%0A" + text19;
			Application.OpenURL(text20.Replace(" ", "%20"));
		}
	}
}
