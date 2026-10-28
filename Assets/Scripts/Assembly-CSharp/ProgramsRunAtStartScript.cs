using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ProgramsRunAtStartScript : MonoBehaviour
{
	public class AdCode
	{
		public const string FlurryCode = "ad001";

		public const string SponsorPayCode = "ad002";

		public const string ChartboostCode = "ad003";

		public const string TapjoyCode = "ad004";

		public string Code { get; set; }

		public byte Status { get; set; }

		public byte Size { get; set; }
	}

	public bool tokenSent;

	private byte scale;

	private Dictionary<string, AdCode> adCodeDic = new Dictionary<string, AdCode>();

	private IEnumerator Start()
	{
		GameApp.GetInstance().UUID = AndroidPluginScript.GetAndroidId();
		GameApp.GetInstance().DataPath = Application.dataPath + "/../../Documents/";
		GameApp.GetInstance().AppStatus = 0;
		GameApp.GetInstance().FlurrySize = 8;
		GameApp.GetInstance().FlurryStatus = 0;
		GameApp.GetInstance().SponsorPaySize = 0;
		GameApp.GetInstance().SponsorPayStatus = 0;
		GameApp.GetInstance().CharBoostSize = 2;
		GameApp.GetInstance().CharBoostStatus = 0;
		GameApp.GetInstance().TapJoyStatus = 0;
		GameApp.GetInstance().TapJoySize = 0;
		GameApp.GetInstance().AdcolonyStatus = 0;
		GameApp.GetInstance().AdcolonySize = 0;
		GameApp.GetInstance().RushStatus = 0;
		GameApp.GetInstance().RushSize = 0;
		GameApp.GetInstance().MovieStatus = 0;
		GameApp.GetInstance().MovieSize = 0;
		adCodeDic.Add("ad001", new AdCode());
		adCodeDic.Add("ad002", new AdCode());
		adCodeDic.Add("ad003", new AdCode());
		WWW getAppStatusWWW = new WWW("http://sw1.freyrgames.com:8088/AdvertServer/GetAppStatus?appcode=g001&v=127");
		WWW getAdControlWWW = new WWW("http://sw1.freyrgames.com:8088/AdvertServer/GetAdvertControl?appcode=g001&v=127&adcodes=ad001,ad002,ad003,ad004,ad005,ad006,ad007");
		while (!getAppStatusWWW.isDone)
		{
			yield return new WaitForSeconds(0.5f);
		}
		try
		{
			byte[] appStatusBytes = getAppStatusWWW.bytes;
			BytesBuffer bb = new BytesBuffer(appStatusBytes);
			if (bb.ReadShort() == 0)
			{
				byte status = bb.ReadByte();
				GameApp.GetInstance().AppStatus = status;
				Debug.Log("appstatus:" + status);
			}
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			Debug.Log(ex2.Message);
		}
		if (GameApp.GetInstance().AppStatus != 0)
		{
			while (!getAdControlWWW.isDone)
			{
				yield return new WaitForSeconds(0.5f);
			}
			try
			{
				byte[] adControlBytes = getAdControlWWW.bytes;
				BytesBuffer bb2 = new BytesBuffer(adControlBytes);
				DownLoadAdCode(bb2);
			}
			catch (Exception ex3)
			{
				Exception ex4 = ex3;
				Debug.Log(ex4.Message);
			}
		}
		WWW getBase64MinLengthWWW = new WWW("http://sw1.freyrgames.com:7671/IapStatServer/GetVerifyLen?appcode=g001");
		while (!getBase64MinLengthWWW.isDone)
		{
			yield return new WaitForSeconds(0.5f);
		}
		try
		{
			if (getBase64MinLengthWWW.error == null)
			{
				GameApp.GetInstance().Base64MinLength = int.Parse(getBase64MinLengthWWW.text);
				Debug.Log("Base64MinLength:" + GameApp.GetInstance().Base64MinLength);
			}
		}
		catch (Exception ex5)
		{
			Debug.Log(ex5.Message);
		}
		string path = GameApp.GetInstance().DataPath;
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		string deviceid = GameApp.GetInstance().UUID;
		string productid = "486314228";
		string apikey = "ff12c9401a731df69a9190229eac27ea";
		string checkurl = "http://cpa.adtilt.com/has_user_action?api_key=" + apikey + "&product_id=" + productid + "&device_id=" + deviceid;
		string checking = string.Empty;
		WWW checkingad = new WWW(checkurl);
		while (!checkingad.isDone)
		{
			yield return new WaitForSeconds(0.5f);
		}
		try
		{
		}
		catch (Exception ex6)
		{
			Debug.Log(ex6.Message);
		}
		if (!checking.Equals("True"))
		{
			string reporturl = "http://cpa.adtilt.com/on_user_action?api_key=" + apikey + "&product_id=" + productid + "&raw_udid=" + deviceid + "&odin1=" + deviceid;
			WWW reportingad = new WWW(reporturl);
			while (!reportingad.isDone)
			{
				yield return new WaitForSeconds(0.5f);
			}
			try
			{
			}
			catch (Exception ex7)
			{
				Exception ex8 = ex7;
				Debug.Log(ex8.Message);
			}
		}
	}

	private void Update()
	{
	}

	public void DownLoadAdCode(BytesBuffer bb)
	{
		if (bb.ReadShort() == 0)
		{
			byte b = bb.ReadByte();
			for (int i = 0; i < b; i++)
			{
				string text = bb.ReadStringShortLength();
				if (text.Equals("ad001"))
				{
					GameApp.GetInstance().FlurryStatus = bb.ReadByte();
					GameApp.GetInstance().FlurrySize = bb.ReadByte();
				}
				else if (text.Equals("ad002"))
				{
					GameApp.GetInstance().SponsorPayStatus = bb.ReadByte();
					GameApp.GetInstance().SponsorPaySize = bb.ReadByte();
				}
				else if (text.Equals("ad003"))
				{
					GameApp.GetInstance().CharBoostStatus = bb.ReadByte();
					GameApp.GetInstance().CharBoostSize = bb.ReadByte();
				}
				else if (text.Equals("ad004"))
				{
					GameApp.GetInstance().TapJoyStatus = bb.ReadByte();
					GameApp.GetInstance().TapJoySize = bb.ReadByte();
				}
				else if (text.Equals("ad005"))
				{
					GameApp.GetInstance().AdcolonyStatus = bb.ReadByte();
					GameApp.GetInstance().AdcolonySize = bb.ReadByte();
				}
				else if (text.Equals("ad006"))
				{
					GameApp.GetInstance().RushStatus = bb.ReadByte();
					GameApp.GetInstance().RushSize = bb.ReadByte();
				}
				else if (text.Equals("ad007"))
				{
					GameApp.GetInstance().MovieStatus = bb.ReadByte();
					GameApp.GetInstance().MovieSize = bb.ReadByte();
				}
			}
		}
		Debug.Log("AdCode Finish");
	}
}
