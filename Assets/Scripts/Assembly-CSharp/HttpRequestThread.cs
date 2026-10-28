using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class HttpRequestThread
{
	public string CryptMD5String(string oriStr)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(oriStr);
		MD5 mD = new MD5CryptoServiceProvider();
		byte[] array = mD.ComputeHash(bytes);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i].ToString("X2"));
		}
		return stringBuilder.ToString();
	}

	public void DoWork()
	{
		try
		{
			string text = "http://sw1.freyrgames.com:8088/AdvertServer/GetAward";
			string aDSUUID = GameApp.GetInstance().ADSUUID;
			string text2 = CryptMD5String("ag006:ad001:" + aDSUUID + ":420C93EC7D1DCE0D42FBD729D46755D572B91235");
			string text3 = "appcode=ag006&adcode=ad001&udid=" + aDSUUID + "&fhash=" + text2;
			string text4 = HttpRequest.Get(text + "?" + text3);
			if (!text4.StartsWith("error&") && !text4.Equals("ok"))
			{
				string[] array = text4.Split('#');
				string[] array2 = array;
				foreach (string text5 in array2)
				{
					string[] array3 = text5.Split('&');
					int num = int.Parse(array3[2]);
					if (num > 1500)
					{
						num = 1500;
					}
					GameApp.GetInstance().GetUserState().AddMithril(num);
					GameApp.GetInstance().GetUserState().AnalysisManager.AddADMithrilAnalysis(num);
					GameApp.GetInstance().Save();
					GameApp.GetInstance().GetUserState().rewardNumber = num;
					GameApp.GetInstance().GetUserState().rewardAdsName = "Flurry";
					GameApp.GetInstance().GetUserState().showRewardMsg = true;
				}
			}
		}
		catch (Exception ex)
		{
			Debug.Log("HttpRequest Exception:" + ex.Message);
		}
		try
		{
			string text6 = "http://sw1.freyrgames.com:8088/AdvertServer/GetAward";
			string aDSUUID2 = GameApp.GetInstance().ADSUUID;
			string text7 = CryptMD5String("ag006:ad002:" + aDSUUID2 + ":33de097b11f4ab373158bb00d98e8099");
			string text8 = "appcode=ag006&adcode=ad002&udid=" + aDSUUID2 + "&fhash=" + text7;
			string text9 = HttpRequest.Get(text6 + "?" + text8);
			if (!text9.StartsWith("error&") && !text9.Equals("ok"))
			{
				string[] array4 = text9.Split('#');
				string[] array5 = array4;
				foreach (string text10 in array5)
				{
					string[] array6 = text10.Split('&');
					int num2 = int.Parse(array6[2]);
					if (num2 > 1500)
					{
						num2 = 1500;
					}
					GameApp.GetInstance().GetUserState().AddMithril(num2);
					GameApp.GetInstance().GetUserState().AnalysisManager.AddADMithrilAnalysis(num2);
					GameApp.GetInstance().Save();
					GameApp.GetInstance().GetUserState().rewardNumber = num2;
					GameApp.GetInstance().GetUserState().rewardAdsName = "SponsorPay";
					GameApp.GetInstance().GetUserState().showRewardMsg = true;
				}
			}
		}
		catch (Exception ex2)
		{
			Debug.Log("HttpRequest Exception:" + ex2.Message);
		}
		try
		{
			string text11 = "http://sw1.freyrgames.com:8088/AdvertServer/GetAward";
			string aDSUUID3 = GameApp.GetInstance().ADSUUID;
			string text12 = CryptMD5String("ig006:ad020:" + aDSUUID3 + ":25b4d5ce9c892e52331020fe836e6bd3");
			string text13 = "appcode=ig006&adcode=ad020&udid=" + aDSUUID3 + "&fhash=" + text12;
			string text14 = HttpRequest.Get(text11 + "?" + text13);
			if (text14.StartsWith("error&") || text14.Equals("ok"))
			{
				return;
			}
			string[] array7 = text14.Split('#');
			string[] array8 = array7;
			foreach (string text15 in array8)
			{
				string[] array9 = text15.Split('&');
				int num3 = int.Parse(array9[2]);
				if (num3 > 1500)
				{
					num3 = 1500;
				}
				GameApp.GetInstance().GetUserState().AddMithril(num3);
				GameApp.GetInstance().GetUserState().AnalysisManager.AddADMithrilAnalysis(num3);
				GameApp.GetInstance().Save();
				GameApp.GetInstance().GetUserState().rewardNumber = num3;
				GameApp.GetInstance().GetUserState().rewardAdsName = "YouMi";
				GameApp.GetInstance().GetUserState().showRewardMsg = true;
			}
		}
		catch (Exception ex3)
		{
			Debug.Log("HttpRequest Exception:" + ex3.Message);
		}
	}
}
