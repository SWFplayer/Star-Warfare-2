using System.Collections.Generic;
using UnityEngine;

public class SocialManager
{
	public const string TestAccount = "561629296";

	private static SocialManager me;

	public bool bGameCenterAuthencated;

	public List<SocialPlayer> SocialPlayerList = new List<SocialPlayer>();

	public SocialPlayer LocalSocialPlayer;

	public void InitSocialManager()
	{
		LocalSocialPlayer = new SocialPlayer();
		LocalSocialPlayer.friendType = GameApp.GetInstance().GetUserState().GetFriendType();
		LocalSocialPlayer.faAccountId = GameApp.GetInstance().GetUserState().GetFacebookId()
			.ToString();
		LocalSocialPlayer.gcAccountId = GameApp.GetInstance().GetUserState().GetGameCenterId()
			.ToString();
		LocalSocialPlayer.ttAccountId = GameApp.GetInstance().GetUserState().GetTwitterName();
		SocialPlayerList.Add(LocalSocialPlayer);
	}

	public static SocialManager GetInstance()
	{
		if (me == null)
		{
			me = new SocialManager();
		}
		return me;
	}

	public SocialPlayer GetSocialPlayer(FriendType type, long ID, string twitterName)
	{
		switch (type)
		{
		case FriendType.NA:
		case FriendType.Freyr:
			return null;
		case FriendType.Twitter:
		{
			foreach (SocialPlayer socialPlayer3 in SocialPlayerList)
			{
				if (socialPlayer3.friendType == type && socialPlayer3.GetSocialID() == twitterName)
				{
					return socialPlayer3;
				}
			}
			SocialPlayer socialPlayer2 = new SocialPlayer();
			socialPlayer2.friendType = type;
			socialPlayer2.SetSocialID(twitterName);
			SocialPlayerList.Add(socialPlayer2);
			return socialPlayer2;
		}
		default:
		{
			foreach (SocialPlayer socialPlayer4 in SocialPlayerList)
			{
				if (socialPlayer4.friendType == type && socialPlayer4.GetSocialID() == ID.ToString())
				{
					return socialPlayer4;
				}
			}
			SocialPlayer socialPlayer = new SocialPlayer();
			socialPlayer.friendType = type;
			socialPlayer.SetSocialID(ID.ToString());
			SocialPlayerList.Add(socialPlayer);
			return socialPlayer;
		}
		}
	}

	public void SetLocalPlayer(FriendType type, long ID)
	{
		if (LocalSocialPlayer.friendType == FriendType.NA)
		{
			LocalSocialPlayer.friendType = type;
			GameApp.GetInstance().GetUserState().SetFriendType(type);
		}
		LocalSocialPlayer.SetSocialID(type, ID.ToString());
		switch (type)
		{
		case FriendType.GameCenter:
			GameApp.GetInstance().GetUserState().SetGameCenterId(ID);
			break;
		case FriendType.Facebook:
			GameApp.GetInstance().GetUserState().SetFacebookId(ID);
			break;
		}
	}

	public void SetLocalPlayer(FriendType type, string ID)
	{
		if (LocalSocialPlayer.friendType == FriendType.NA)
		{
			LocalSocialPlayer.friendType = type;
			GameApp.GetInstance().GetUserState().SetFriendType(type);
		}
		LocalSocialPlayer.SetSocialID(type, ID);
		if (type == FriendType.Twitter)
		{
			GameApp.GetInstance().GetUserState().SetTwitterName(ID);
		}
	}

	public void LoadSocialData()
	{
		List<string> list = new List<string>();
		List<SocialPlayer> list2 = new List<SocialPlayer>();
		List<SocialPlayer> list3 = new List<SocialPlayer>();
		foreach (SocialPlayer socialPlayer in SocialPlayerList)
		{
			if (socialPlayer.friendType == FriendType.GameCenter && !socialPlayer.hasGameCenterProfilePhoto)
			{
				list.Add("G:" + socialPlayer.GetSocialID());
			}
			else if (socialPlayer.friendType == FriendType.Facebook && !socialPlayer.hasFacebookProfilePhoto)
			{
				list2.Add(socialPlayer);
			}
			else if (socialPlayer.friendType == FriendType.Twitter && !socialPlayer.hasTwitterProfilePhoto)
			{
				list3.Add(socialPlayer);
			}
		}
		if (list.Count > 0)
		{
			string text = string.Empty;
			foreach (string item in list)
			{
				text = text + item + "  ";
			}
			Debug.Log("..Load all image in SocialList: \n" + text);
		}
		foreach (SocialPlayer item2 in list2)
		{
			string url = "http://graph.facebook.com/" + item2.GetSocialID() + "/picture";
			if (AndroidConstant.isFacebook())
			{
				ApplicationPauseManager.mInstance.StartCoroutine(FacebookTools.instance.fetchImageAtUrl(url, item2.UpdateFBHead));
			}
		}
		foreach (SocialPlayer item3 in list3)
		{
			string url2 = "http://twitter.com/api/users/profile_image/" + item3.GetSocialID();
			if (AndroidConstant.isFacebook())
			{
				ApplicationPauseManager.mInstance.StartCoroutine(FacebookTools.instance.fetchImageAtUrl(url2, item3.UpdateTTHead));
			}
		}
	}

	public void LoadSocialData(SocialPlayer sp)
	{
		if (sp == null)
		{
			Debug.LogWarning("LoadSocialData sp = null");
		}
		else if (sp.friendType == FriendType.GameCenter && !sp.hasGameCenterProfilePhoto)
		{
			string text = "G:" + sp.GetSocialID();
		}
		else if (sp.friendType == FriendType.Facebook && !sp.hasFacebookProfilePhoto)
		{
			string url = "http://graph.facebook.com/" + sp.GetSocialID() + "/picture";
			if (AndroidConstant.isFacebook())
			{
				ApplicationPauseManager.mInstance.StartCoroutine(FacebookTools.instance.fetchImageAtUrl(url, sp.UpdateFBHead));
			}
		}
		else if (sp.friendType == FriendType.Twitter && !sp.hasTwitterProfilePhoto)
		{
			string url2 = "http://twitter.com/api/users/profile_image/" + sp.GetSocialID();
			if (AndroidConstant.isFacebook())
			{
				ApplicationPauseManager.mInstance.StartCoroutine(FacebookTools.instance.fetchImageAtUrl(url2, sp.UpdateTTHead));
			}
		}
	}

	public static void UpdateHead(SocialPlayer sp, UITexture HeadPortrait)
	{
		if (sp != null && sp.HasPhoto() && HeadPortrait != null && HeadPortrait.mainTexture != sp.GetPhoto())
		{
			HeadPortrait.gameObject.SetActive(true);
			HeadPortrait.mainTexture = sp.GetPhoto();
		}
	}

	public static void Log(string log)
	{
	}

	public void RefreshFacebook()
	{
		long num = long.Parse(FB.UserId);
		Debug.Log("facebook Id = " + num);
		GetInstance().SetLocalPlayer(FriendType.Facebook, num);
		LoginPageScript.mInstance.FetchFacebookPhoto();
	}

	public void RefreshTwitter()
	{
	}

	public static void completionHandler(string error, object result)
	{
	}
}
