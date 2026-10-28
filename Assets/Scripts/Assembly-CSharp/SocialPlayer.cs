using System;
using System.IO;
using UnityEngine;

public class SocialPlayer
{
	public FriendType friendType;

	public string playerId;

	public long playerIdNum;

	public int userId;

	public string alias;

	public string lv;

	public int schoolLv;

	public int prestigeLv;

	public string faAccountId = string.Empty;

	public string gcAccountId = string.Empty;

	public string ttAccountId = string.Empty;

	public bool installed;

	public byte isAccessed;

	public bool IsFriend = true;

	public int VipLv;

	public Texture2D facebookTexture;

	public Texture2D gamecenterTexture;

	public Texture2D twitterTexture;

	private string _facebookProfilePhotoPath;

	private string _gamecenterProfilePhotoPath;

	private string _twitterProfilePhotoPath;

	public int HeadID { get; set; }

	public bool hasFacebookProfilePhoto
	{
		get
		{
			return File.Exists(facebookProfilePhotoPath);
		}
	}

	public Texture2D facebookProfilePhoto
	{
		get
		{
			if (!hasFacebookProfilePhoto)
			{
				return null;
			}
			if (facebookTexture == null)
			{
				try
				{
					Debug.Log("start read file..");
					byte[] data = File.ReadAllBytes(facebookProfilePhotoPath);
					Debug.Log("bytes readed..");
					facebookTexture = new Texture2D(256, 256, TextureFormat.ARGB32, false);
					Debug.Log("new texture..");
					if (!facebookTexture.LoadImage(data))
					{
						return null;
					}
					Debug.Log("texture loaded..");
				}
				catch (Exception ex)
				{
					Debug.Log("facebookProfilePhoto exception:" + ex.StackTrace);
				}
			}
			return facebookTexture;
		}
	}

	public string facebookProfilePhotoPath
	{
		get
		{
			_facebookProfilePhotoPath = Path.Combine(Application.persistentDataPath + "/Documents", string.Format("F{0}.png", faAccountId));
			return _facebookProfilePhotoPath;
		}
	}

	public bool hasGameCenterProfilePhoto
	{
		get
		{
			return File.Exists(gamecenterProfilePhotoPath);
		}
	}

	public Texture2D gameCenterProfilePhoto
	{
		get
		{
			if (!hasGameCenterProfilePhoto)
			{
				return null;
			}
			if (gamecenterTexture == null)
			{
				try
				{
					Debug.Log("start read file..");
					byte[] data = File.ReadAllBytes(gamecenterProfilePhotoPath);
					Debug.Log("bytes readed..");
					gamecenterTexture = new Texture2D(256, 256, TextureFormat.ARGB32, false);
					Debug.Log("new texture..");
					bool flag = gamecenterTexture.LoadImage(data);
					Debug.Log("texture loaded..");
					if (!flag)
					{
						Debug.Log("load image failed. texture is null.");
						return null;
					}
					Debug.Log("social gc player load image succ.. " + gcAccountId);
				}
				catch (Exception ex)
				{
					Debug.Log("gameCenterProfilePhoto exception:" + ex.StackTrace);
				}
			}
			return gamecenterTexture;
		}
	}

	public string gamecenterProfilePhotoPath
	{
		get
		{
			_gamecenterProfilePhotoPath = Path.Combine(Application.persistentDataPath + "/Documents", string.Format("G{0}.png", gcAccountId));
			return _gamecenterProfilePhotoPath;
		}
	}

	public bool hasTwitterProfilePhoto
	{
		get
		{
			return File.Exists(twitterProfilePhotoPath);
		}
	}

	public Texture2D twitterProfilePhoto
	{
		get
		{
			if (!hasTwitterProfilePhoto)
			{
				return null;
			}
			if (twitterTexture == null)
			{
				try
				{
					Debug.Log("start read file..");
					byte[] data = File.ReadAllBytes(twitterProfilePhotoPath);
					Debug.Log("bytes readed..");
					twitterTexture = new Texture2D(256, 256, TextureFormat.ARGB32, false);
					Debug.Log("new texture..");
					if (!twitterTexture.LoadImage(data))
					{
						return null;
					}
					Debug.Log("texture loaded..");
				}
				catch (Exception ex)
				{
					Debug.Log("twitterProfilePhoto exception:" + ex.StackTrace);
				}
			}
			return twitterTexture;
		}
	}

	public string twitterProfilePhotoPath
	{
		get
		{
			_twitterProfilePhotoPath = Path.Combine(Application.persistentDataPath + "/Documents", string.Format("T{0}.png", ttAccountId));
			return _twitterProfilePhotoPath;
		}
	}

	public bool HasPhoto()
	{
		if (friendType == FriendType.NA || friendType == FriendType.Freyr)
		{
			return false;
		}
		if (friendType == FriendType.GameCenter)
		{
			return hasGameCenterProfilePhoto;
		}
		if (friendType == FriendType.Facebook)
		{
			return hasFacebookProfilePhoto;
		}
		if (friendType == FriendType.Twitter)
		{
			return hasTwitterProfilePhoto;
		}
		Debug.Log(friendType);
		Debug.LogError("friendType wrong.");
		return false;
	}

	public Texture2D GetPhoto()
	{
		if (friendType == FriendType.NA || friendType == FriendType.Freyr)
		{
			return null;
		}
		if (friendType == FriendType.GameCenter)
		{
			return gameCenterProfilePhoto;
		}
		if (friendType == FriendType.Facebook)
		{
			return facebookProfilePhoto;
		}
		if (friendType == FriendType.Twitter)
		{
			return twitterProfilePhoto;
		}
		Debug.LogError("friendType wrong.");
		return null;
	}

	public string GetSocialID()
	{
		if (friendType == FriendType.NA || friendType == FriendType.Freyr)
		{
			return string.Empty;
		}
		if (friendType == FriendType.GameCenter)
		{
			return gcAccountId;
		}
		if (friendType == FriendType.Facebook)
		{
			return faAccountId;
		}
		if (friendType == FriendType.Twitter)
		{
			return ttAccountId;
		}
		Debug.LogError("friendType wrong.");
		return string.Empty;
	}

	public void SetSocialID(string ID)
	{
		if (friendType != FriendType.NA && friendType != FriendType.Freyr)
		{
			if (friendType == FriendType.GameCenter)
			{
				gcAccountId = ID;
			}
			else if (friendType == FriendType.Facebook)
			{
				faAccountId = ID;
			}
			else if (friendType == FriendType.Twitter)
			{
				ttAccountId = ID;
			}
			else
			{
				Debug.LogError("SetSocialID friendType wrong." + friendType);
			}
		}
	}

	public void SetSocialID(FriendType friendType, string ID)
	{
		switch (friendType)
		{
		case FriendType.NA:
		case FriendType.Freyr:
			break;
		case FriendType.GameCenter:
			gcAccountId = ID;
			break;
		case FriendType.Facebook:
			faAccountId = ID;
			break;
		case FriendType.Twitter:
			ttAccountId = ID;
			break;
		default:
			Debug.LogError("SetSocialID friendType wrong." + friendType);
			break;
		}
	}

	public static void Fake(SocialPlayer player)
	{
		player.friendType = FriendType.GameCenter;
		player.SetSocialID(((long)UnityEngine.Random.Range(0, 100)).ToString());
	}

	public void UpdateGCHead()
	{
		string text = Path.Combine(Application.persistentDataPath, string.Format("G{0}.png", gcAccountId));
		string text2 = Path.Combine(Application.persistentDataPath + "/Documents", string.Format("G{0}.png", gcAccountId));
		if (File.Exists(text2))
		{
			File.Delete(text2);
		}
		if (File.Exists(text))
		{
			File.Move(text, text2);
		}
	}

	public void UpdateFBHead(Texture2D portrait)
	{
		if (portrait != null)
		{
			byte[] bytes = portrait.EncodeToPNG();
			File.WriteAllBytes(facebookProfilePhotoPath, bytes);
		}
	}

	public void UpdateTTHead(Texture2D portrait)
	{
		if (portrait != null)
		{
			byte[] bytes = portrait.EncodeToPNG();
			File.WriteAllBytes(twitterProfilePhotoPath, bytes);
		}
	}
}
