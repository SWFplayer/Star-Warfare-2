using System;
using System.Collections;
using Facebook;
using UnityEngine;

internal class FacebookTools
{
	private static FacebookTools _instance;

	public static FacebookTools instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new FacebookTools();
			}
			return _instance;
		}
	}

	public static bool isSessionValid()
	{
		if (FB.IsInitialized)
		{
			Debug.Log("FB is inited");
			if (FB.IsLoggedIn)
			{
				return true;
			}
			Debug.Log("FB start to login");
			return false;
		}
		Debug.Log("FB start to init");
		FB.Init(OnInitComplete, OnHideUnity);
		return false;
	}

	public static void Init()
	{
		FB.Init(OnInitComplete, OnHideUnity);
	}

	private static void OnInitComplete()
	{
		Debug.Log("FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
	}

	private static void OnHideUnity(bool isGameShown)
	{
		Debug.Log("Is game showing? " + isGameShown);
	}

	public static void CallFBLogin()
	{
		Debug.Log("CallFBLogin");
		FB.Login("user_about_me,user_friends,publish_actions");
	}

	public static void Feed(WWWForm wwwForm)
	{
		FB.API("/me/photos", HttpMethod.POST, FeedCallback, wwwForm);
	}

	private static void FeedCallback(FBResult result)
	{
		Debug.Log("FeedCallback over.");
		if (result.Error != null)
		{
			Debug.LogError(result.Error);
			UIMsgBox.instance.CloseMessage();
			UIMsgBox.instance.ShowMessage(UIMsgBox.instance, LocalizationManager.GetInstance().GetString("SNS_Post2") + ": " + result.Error, 1, 9);
		}
		else
		{
			Debug.Log(result);
			UIMsgBox.instance.CloseMessage();
			UIMsgBox.instance.ShowMessage(UIMsgBox.instance, LocalizationManager.GetInstance().GetString("SNS_Post1"), 1, 9);
		}
	}

	public IEnumerator fetchImageAtUrl(string url, Action<Texture2D> completionHandler)
	{
		WWW www = new WWW(url);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			Debug.Log("Error attempting to load profile image: " + www.error);
			if (completionHandler != null)
			{
				completionHandler(null);
			}
		}
		else if (completionHandler != null)
		{
			completionHandler(www.texture);
		}
	}
}
