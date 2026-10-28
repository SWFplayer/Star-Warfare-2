using System;
using System.Collections;
using UnityEngine;

public class AdsScript : MonoBehaviour
{
	public static AdsScript mInstance;

	private bool initAdsSuccessCalled;

	private void Awake()
	{
		mInstance = this;
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	private IEnumerator Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (string.IsNullOrEmpty(GameApp.GetInstance().UUID))
		{
			GameApp.GetInstance().UUID = "android:" + AndroidPluginScript.GetAndroidId();
			GameApp.GetInstance().ADSUUID = AndroidPluginScript.GetAndroidId();
			GameApp.GetInstance().IDFA = AndroidPluginScript.GetAndroidId();
		}
		if (string.IsNullOrEmpty(GameApp.GetInstance().MacAddress))
		{
			GameApp.GetInstance().MacAddress = AndroidPluginScript.GetMacAddress();
			Debug.Log("MacAddress = " + GameApp.GetInstance().MacAddress);
			if (GameApp.GetInstance().MacAddress == null)
			{
				GameApp.GetInstance().MacAddress = string.Empty;
			}
		}
		if (!GameApp.GetInstance().IsConnectedToInternet())
		{
			yield break;
		}
		if (AndroidConstant.version == AndroidConstant.Version.Kindle)
		{
			WWW getAdControlWWW = new WWW("http://sw1.freyrgames.com:8088/AdvertServer/GetAdvertControl?appcode=ag006&v=127&adcodes=iap003");
			while (!getAdControlWWW.isDone)
			{
				yield return new WaitForSeconds(0.5f);
			}
			try
			{
				byte[] adControlBytes = getAdControlWWW.bytes;
				BytesBuffer bb = new BytesBuffer(adControlBytes);
				if (bb.ReadShort() == 0)
				{
					byte ad_num = bb.ReadByte();
					for (int i = 0; i < ad_num; i++)
					{
						string code = bb.ReadStringShortLength();
						if (code.Equals("iap003"))
						{
							GameApp.GetInstance().IapColorPackStatus = bb.ReadByte();
							bb.ReadByte();
							Debug.Log("IapColorPackStatus = " + GameApp.GetInstance().IapColorPackStatus);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				Debug.Log(ex2.Message);
				yield break;
			}
		}
		else if (AndroidConstant.version == AndroidConstant.Version.GooglePlay)
		{
			WWW getAppStatusWWW = new WWW("http://sw1.freyrgames.com:8088/AdvertServer/GetAppStatus?appcode=ag006&v=127");
			WWW getAdControlWWW2 = new WWW("http://sw1.freyrgames.com:8088/AdvertServer/GetAdvertControl?appcode=ag006&v=127&adcodes=ad001,ad002,ad003,ad004,ad005,ad011,ad012,ad013");
			while (!getAppStatusWWW.isDone)
			{
				yield return new WaitForSeconds(0.5f);
			}
			try
			{
				byte[] appStatusBytes = getAppStatusWWW.bytes;
				BytesBuffer bb2 = new BytesBuffer(appStatusBytes);
				if (bb2.ReadShort() == 0)
				{
					byte status = bb2.ReadByte();
					GameApp.GetInstance().AppStatus = status;
					Debug.Log("appstatus:" + status);
				}
			}
			catch (Exception ex3)
			{
				Exception ex4 = ex3;
				Debug.Log(ex4.Message);
				yield break;
			}
			if (GameApp.GetInstance().AppStatus == 0)
			{
				yield break;
			}
			while (!getAdControlWWW2.isDone)
			{
				yield return new WaitForSeconds(0.5f);
			}
			try
			{
				byte[] adControlBytes2 = getAdControlWWW2.bytes;
				BytesBuffer bb3 = new BytesBuffer(adControlBytes2);
				DownLoadAdCode(bb3);
			}
			catch (Exception ex5)
			{
				Debug.Log(ex5.Message);
				yield break;
			}
		}
		GameApp.GetInstance().isAdCodeDownloaded = true;
		InitAds();
	}

	public void InitAds()
	{
		if (GameApp.GetInstance().isAdCodeDownloaded && GameApp.GetInstance().isMainMenuLoaded && GameApp.GetInstance().AppStatus != 0 && !GameApp.GetInstance().isAdInitFinished && !initAdsSuccessCalled)
		{
			StartCoroutine("_InitAdsTimer");
			StartCoroutine("_InitAds");
			initAdsSuccessCalled = true;
		}
	}

	private IEnumerator _InitAdsTimer()
	{
		yield return new WaitForSeconds(3f);
		StopCoroutine("_InitAds");
		Debug.Log("Time out, Stop AdInit Coroutine");
	}

	private IEnumerator _InitAds()
	{
		MainMenu2DScript.mInstance.SetInitAdTime("Start");
		FreyrPlugin.InitAppsFlyer();
		MainMenu2DScript.mInstance.SetInitAdTime("InitAppsFlyer");
		yield return null;
		FreyrPlugin.InitSponsorPay();
		MainMenu2DScript.mInstance.SetInitAdTime("SponsorPay");
		yield return null;
		FreyrPlugin.InitNativeX();
		MainMenu2DScript.mInstance.SetInitAdTime("InitNativeX");
		yield return null;
		FreyrPlugin.InitChartboost();
		MainMenu2DScript.mInstance.SetInitAdTime("InitChartboost");
		yield return null;
		FreyrPlugin.InitAdColony();
		MainMenu2DScript.mInstance.SetInitAdTime("InitAdColony");
		AndroidPluginScript.GetTapJoyPoints();
		yield return null;
		GameApp.GetInstance().isAdInitFinished = true;
		Debug.Log("AD Init Finished!");
		yield return null;
	}

	public void TrackAppFlyer()
	{
		AndroidPluginScript.AppsFlyerTracker();
	}

	public void ShowAds()
	{
		Debug.Log("ShowAds");
		AndroidPluginScript.GetTapJoyPoints();
	}

	private void RewardMithril(string mithril)
	{
		string[] array = new string[2];
		array = mithril.Split(':');
		int num = int.Parse(array[0]);
		string text = array[1];
		if (num > 1500)
		{
			num = 1500;
		}
		GameApp.GetInstance().GetUserState().AddMithril(num);
		GameApp.GetInstance().GetUserState().AnalysisManager.AddADMithrilAnalysis(num);
		GameApp.GetInstance().Save();
		if (text.Equals("tapjoy"))
		{
			Debug.Log("show tapjoy Dialog!");
			AndroidPluginScript.ShowTapjoyDialog(num);
		}
		if (text.Equals("nativex"))
		{
			Debug.Log("Show NativeX Dialog!");
			AndroidPluginScript.ShowNativeXDialog(num);
		}
	}

	private void RewardCash(string cash)
	{
		int num = int.Parse(cash);
		GameApp.GetInstance().GetUserState().AddCash(num);
		GameApp.GetInstance().GetUserState().AnalysisManager.AddADCashAnalysis(num);
		GameApp.GetInstance().Save();
	}

	public void PlayerVideoRebirth()
	{
		GameApp.IsVideo = true;
	}

	public void NativeXVideoCancel(string str)
	{
		Time.timeScale = 1f;
	}

	public void DownLoadAdCode(BytesBuffer bb)
	{
		if (bb.ReadShort() != 0)
		{
			return;
		}
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
			else if (text.Equals("ad008"))
			{
				GameApp.GetInstance().COAStatus = bb.ReadByte();
				GameApp.GetInstance().COASize = bb.ReadByte();
			}
			else if (text.Equals("ad009"))
			{
				GameApp.GetInstance().AppLovinStatus = bb.ReadByte();
				GameApp.GetInstance().AppLovinSize = bb.ReadByte();
			}
			else if (text.Equals("ad010"))
			{
				GameApp.GetInstance().BHStatus = bb.ReadByte();
				GameApp.GetInstance().BHSize = bb.ReadByte();
			}
			else if (text.Equals("ad011"))
			{
				GameApp.GetInstance().YouMiStatus = bb.ReadByte();
				GameApp.GetInstance().YouMiSize = bb.ReadByte();
			}
			else if (text.Equals("ad012"))
			{
				GameApp.GetInstance().NativeXStatus = bb.ReadByte();
				GameApp.GetInstance().NativeXSize = bb.ReadByte();
			}
			else if (text.Equals("ad013"))
			{
				GameApp.GetInstance().NativeXVideoStatus = bb.ReadByte();
				GameApp.GetInstance().NativeXVideoSize = bb.ReadByte();
				Debug.Log("NativeXVideoStatus = " + GameApp.GetInstance().NativeXVideoStatus);
				Debug.Log("NativeXVideoSize = " + GameApp.GetInstance().NativeXVideoSize);
				if (AndroidConstant.version == AndroidConstant.Version.Kindle)
				{
					GameApp.GetInstance().NativeXVideoStatus = 0;
				}
			}
		}
	}

	public void RestorePurchase(string s)
	{
		Debug.Log("get RestorePurchase " + s);
		if (MithrilRewardsScript.Instance != null && MithrilRewardsScript.Instance.mProductIdList != null)
		{
			MithrilRewardsScript.Instance.mProductIdList.Add(s);
		}
	}
}
