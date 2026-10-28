using System.Runtime.InteropServices;
using UnityEngine;

public class FreyrPlugin
{
	private static GCType iVirtualCursor = GCType.GC_Unknow;

	[DllImport("__Internal")]
	protected static extern void _PlayFlurryVideo();

	public static void PlayFlurryVideo()
	{
	}

	[DllImport("__Internal")]
	protected static extern void _ShowFlurryAds();

	public static void ShowFlurryAds()
	{
	}

	[DllImport("__Internal")]
	protected static extern void _ShowTapjoy();

	public static void ShowTapjoy()
	{
	}

	[DllImport("__Internal")]
	protected static extern void _MoreGame();

	public static void MoreGame()
	{
	}

	[DllImport("__Internal")]
	protected static extern void _ShowChartBoost();

	public static void ShowChartBoost()
	{
	}

	[DllImport("__Internal")]
	protected static extern void _ShowAdcolony();

	public static void ShowAdcolony()
	{
	}

	[DllImport("__Internal")]
	protected static extern void _ShowFyber();

	public static void ShowFyber()
	{
	}

	[DllImport("__Internal")]
	protected static extern void _ShowNativeX();

	public static void ShowNativeX()
	{
	}

	[DllImport("__Internal")]
	protected static extern void _InitAds();

	public static void InitAds()
	{
	}

	[DllImport("__Internal")]
	protected static extern void _InitFyber();

	public static void InitFyber()
	{
	}

	[DllImport("__Internal")]
	protected static extern void _InitNativeX();

	public static void InitNativeX()
	{
		AndroidPluginScript.initNativeX();
	}

	[DllImport("__Internal")]
	protected static extern void _InitAdColony();

	public static void InitAdColony()
	{
		AndroidPluginScript.initAdColony();
	}

	[DllImport("__Internal")]
	protected static extern void _InitChartboost();

	public static void InitChartboost()
	{
		AndroidPluginScript.initChartboost();
	}

	[DllImport("__Internal")]
	protected static extern void _InitAppsFlyer();

	public static void InitAppsFlyer()
	{
		AndroidPluginScript.initAppsFlyer();
	}

	[DllImport("__Internal")]
	protected static extern void _InitTapjoy();

	public static void InitTapjoy()
	{
	}

	[DllImport("__Internal")]
	protected static extern void _InitFlurry();

	public static void InitFlurry()
	{
	}

	[DllImport("__Internal")]
	protected static extern void _InitYouMiVedio();

	public static void InitYouMiVedio()
	{
	}

	[DllImport("__Internal")]
	protected static extern void _trackFiksuuploadCustomEvent();

	public static void trackFiksuuploadCustomEvent()
	{
	}

	[DllImport("__Internal")]
	protected static extern void _appsflyerSetuserID(string userID);

	public static void appsflyerSetuserID(string userID)
	{
	}

	[DllImport("__Internal")]
	protected static extern void _appsflyertackRegistration();

	public static void appsflyertackRegistration()
	{
	}

	[DllImport("__Internal")]
	protected static extern void _appsflyertackpurchase(double amount, string userID);

	public static void appsflyertackpurchase(double amount, string userID)
	{
	}

	[DllImport("__Internal")]
	protected static extern void _appsflyerReportLv(int lv, string userID);

	public static void appsflyerReportLv(int lv, string userID)
	{
	}

	[DllImport("__Internal")]
	protected static extern void _AppsFlyerTracker();

	public static void AppsFlyerTracker()
	{
	}

	public static void InitSponsorPay()
	{
	}

	public static GCType CanUseVirtualCursor()
	{
		if (iVirtualCursor == GCType.GC_Unknow)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				iVirtualCursor = (GCType)CurrentActivity.getInstance().JavaObject.Call<int>("GetVirtualCursorStatus", new object[0]);
			}
			else
			{
				iVirtualCursor = GCType.GC_None;
			}
		}
		return iVirtualCursor;
	}
}
