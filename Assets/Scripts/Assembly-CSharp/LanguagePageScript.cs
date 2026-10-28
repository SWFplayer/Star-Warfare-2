using System;
using UnityEngine;

public class LanguagePageScript : MonoBehaviour
{
	public GameObject Chinese;

	public GameObject English;

	public GameObject French;

	public GameObject Germany;

	public GameObject Spanish;

	public GameObject Hongkong;

	public GameObject Korean;

	public GameObject Japanese;

	public GameObject Russian;

	private void Start()
	{
		switch (LocalizationManager.GetInstance().currentLanguage)
		{
		case EUserLanguage.DEFAULT:
		case EUserLanguage.ENGLISH:
			English.transform.Find("OffSprite").gameObject.SetActive(false);
			break;
		case EUserLanguage.CHINESE_SIMPLIFIED:
			Chinese.transform.Find("OffSprite").gameObject.SetActive(false);
			break;
		case EUserLanguage.FRENCH:
			French.transform.Find("OffSprite").gameObject.SetActive(false);
			break;
		case EUserLanguage.GERMAN:
			Germany.transform.Find("OffSprite").gameObject.SetActive(false);
			break;
		case EUserLanguage.SPANISH:
			Spanish.transform.Find("OffSprite").gameObject.SetActive(false);
			break;
		case EUserLanguage.CHINESE_TRADITIONAL:
			Hongkong.transform.Find("OffSprite").gameObject.SetActive(false);
			break;
		case EUserLanguage.KOREAN:
			Korean.transform.Find("OffSprite").gameObject.SetActive(false);
			break;
		case EUserLanguage.JAPANESE:
			Japanese.transform.Find("OffSprite").gameObject.SetActive(false);
			break;
		case EUserLanguage.RUSSIAN:
			Russian.transform.Find("OffSprite").gameObject.SetActive(false);
			break;
		}
		UIEventListener uIEventListener = UIEventListener.Get(Chinese);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnLanguageClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(English);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnLanguageClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(French);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnLanguageClick));
		UIEventListener uIEventListener4 = UIEventListener.Get(Germany);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnLanguageClick));
		UIEventListener uIEventListener5 = UIEventListener.Get(Spanish);
		uIEventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener5.onClick, new UIEventListener.VoidDelegate(OnLanguageClick));
		UIEventListener uIEventListener6 = UIEventListener.Get(Hongkong);
		uIEventListener6.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener6.onClick, new UIEventListener.VoidDelegate(OnLanguageClick));
		UIEventListener uIEventListener7 = UIEventListener.Get(Korean);
		uIEventListener7.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener7.onClick, new UIEventListener.VoidDelegate(OnLanguageClick));
		UIEventListener uIEventListener8 = UIEventListener.Get(Japanese);
		uIEventListener8.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener8.onClick, new UIEventListener.VoidDelegate(OnLanguageClick));
		UIEventListener uIEventListener9 = UIEventListener.Get(Russian);
		uIEventListener9.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener9.onClick, new UIEventListener.VoidDelegate(OnLanguageClick));
		if (AndroidConstant.version == AndroidConstant.Version.GooglePlay)
		{
			Chinese.gameObject.SetActive(false);
		}
	}

	private void Update()
	{
	}

	public void OnLanguageClick(GameObject go)
	{
		EUserLanguage userLanguage = GameApp.GetInstance().GetUserState().GetUserLanguage();
		UserState userState = GameApp.GetInstance().GetUserState();
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
		if (go == Chinese)
		{
			if (userLanguage != EUserLanguage.CHINESE_SIMPLIFIED)
			{
				userState.SetUserLanguage(EUserLanguage.CHINESE_SIMPLIFIED);
				GameApp.GetInstance().Save();
				MissionManager.GetInstance().OnLanguageChanged();
				LocalizationManager.GetInstance().currentLanguage = GameApp.GetInstance().GetUserState().GetUserLanguage();
			}
		}
		else if (go == English)
		{
			if (userLanguage != EUserLanguage.ENGLISH)
			{
				userState.SetUserLanguage(EUserLanguage.ENGLISH);
				GameApp.GetInstance().Save();
				MissionManager.GetInstance().OnLanguageChanged();
				LocalizationManager.GetInstance().currentLanguage = GameApp.GetInstance().GetUserState().GetUserLanguage();
			}
		}
		else if (go == French)
		{
			if (userLanguage != EUserLanguage.FRENCH)
			{
				userState.SetUserLanguage(EUserLanguage.FRENCH);
				GameApp.GetInstance().Save();
				MissionManager.GetInstance().OnLanguageChanged();
				LocalizationManager.GetInstance().currentLanguage = GameApp.GetInstance().GetUserState().GetUserLanguage();
			}
		}
		else if (go == Germany)
		{
			if (userLanguage != EUserLanguage.GERMAN)
			{
				userState.SetUserLanguage(EUserLanguage.GERMAN);
				GameApp.GetInstance().Save();
				MissionManager.GetInstance().OnLanguageChanged();
				LocalizationManager.GetInstance().currentLanguage = GameApp.GetInstance().GetUserState().GetUserLanguage();
			}
		}
		else if (go == Spanish)
		{
			if (userLanguage != EUserLanguage.SPANISH)
			{
				userState.SetUserLanguage(EUserLanguage.SPANISH);
				GameApp.GetInstance().Save();
				MissionManager.GetInstance().OnLanguageChanged();
				LocalizationManager.GetInstance().currentLanguage = GameApp.GetInstance().GetUserState().GetUserLanguage();
			}
		}
		else if (go == Hongkong)
		{
			if (userLanguage != EUserLanguage.CHINESE_TRADITIONAL)
			{
				userState.SetUserLanguage(EUserLanguage.CHINESE_TRADITIONAL);
				GameApp.GetInstance().Save();
				MissionManager.GetInstance().OnLanguageChanged();
				LocalizationManager.GetInstance().currentLanguage = GameApp.GetInstance().GetUserState().GetUserLanguage();
			}
		}
		else if (go == Korean)
		{
			if (userLanguage != EUserLanguage.KOREAN)
			{
				userState.SetUserLanguage(EUserLanguage.KOREAN);
				GameApp.GetInstance().Save();
				MissionManager.GetInstance().OnLanguageChanged();
				LocalizationManager.GetInstance().currentLanguage = GameApp.GetInstance().GetUserState().GetUserLanguage();
			}
		}
		else if (go == Japanese)
		{
			if (userLanguage != EUserLanguage.JAPANESE)
			{
				userState.SetUserLanguage(EUserLanguage.JAPANESE);
				GameApp.GetInstance().Save();
				MissionManager.GetInstance().OnLanguageChanged();
				LocalizationManager.GetInstance().currentLanguage = GameApp.GetInstance().GetUserState().GetUserLanguage();
			}
		}
		else if (go == Russian && userLanguage != EUserLanguage.RUSSIAN)
		{
			userState.SetUserLanguage(EUserLanguage.RUSSIAN);
			GameApp.GetInstance().Save();
			MissionManager.GetInstance().OnLanguageChanged();
			LocalizationManager.GetInstance().currentLanguage = GameApp.GetInstance().GetUserState().GetUserLanguage();
		}
		UnityEngine.Object.Destroy(UIMsgBox.instance.gameObject);
		Application.LoadLevel("MainMenu");
	}
}
