using UnityEngine;

public class AndroidPluginScript
{
    public static int GetLanguage()
    {
        return 6;
    }

    public static string GetMacAddress()
    {
        return System.Environment.MachineName;
    }

    public static string GetAndroidId()
    {
        return System.Environment.UserName;
    }

    public static void GetIsVideoRebirth(bool VideoRebirth)
    {
        if (!IsPC())
        {
            CurrentActivity.getInstance().JavaObject.Call("GetIsVideoRebirth", VideoRebirth);
        }
        else
        {
            Debug.Log("GetIsVideoRebirth");
        }
    }

    public static bool IsReadyAdcolony()
    {
        if (!IsPC())
        {
            return CurrentActivity.getInstance().JavaObject.Call<bool>("IsReadyAdcolony", new object[0]);
        }
        return true;
    }

    public static void InitAds()
    {
        if (!IsPC())
        {
            CurrentActivity.getInstance().JavaObject.Call("InitAds");
        }
        else
        {
            Debug.Log("InitAds");
        }
    }

    public static void AppsFlyerTracker()
    {
        if (!IsPC())
        {
            CurrentActivity.getInstance().JavaObject.Call("AppsFlyerTracker");
        }
        else
        {
            Debug.Log("AppsFlyerTracker");
        }
    }

    public static void appsflyertackRegistration()
    {
        if (!IsPC())
        {
            CurrentActivity.getInstance().JavaObject.Call("AppsflyertackRegistration");
        }
        else
        {
            Debug.Log("AppsflyertackRegistration");
        }
    }

    public static void CallTapjoyOfferWall()
    {
        if (!IsPC())
        {
            CurrentActivity.getInstance().JavaObject.Call("CallTapjoyOfferWall");
        }
        else
        {
            Debug.Log("CallTapjoyOfferWall");
        }
    }

    public static void GetTapJoyPoints()
    {
        if (!IsPC())
        {
            CurrentActivity.getInstance().JavaObject.Call("GetTapJoyPoints");
        }
        else
        {
            Debug.Log("GetTapJoyPoints");
        }
    }

    public static void ShowTapjoyDialog(int num)
    {
        if (!IsPC())
        {
            CurrentActivity.getInstance().JavaObject.Call("ShowTapjoyDialog", num);
        }
        else
        {
            Debug.Log("ShowTapjoyDialog");
        }
    }

    public static void ShowNativeXDialog(int num)
    {
        if (!IsPC())
        {
            CurrentActivity.getInstance().JavaObject.Call("ShowNativeXDialog", num);
        }
        else
        {
            Debug.Log("ShowNativeXDialog");
        }
    }

    public static void CallFlurryAds()
    {
        if (!IsPC())
        {
            CurrentActivity.getInstance().JavaObject.Call("CallFlurryAds");
        }
        else
        {
            Debug.Log("CallFlurryAds");
        }
    }

    public static void CallSponsorPayOfferWall()
    {
        if (!IsPC())
        {
            CurrentActivity.getInstance().JavaObject.Call("CallSponsorPayOfferWall");
        }
        else
        {
            Debug.Log("CallSponsorPayOfferWall");
        }
    }

    public static void CallAdcolonyVideo()
    {
        if (!IsPC())
        {
            CurrentActivity.getInstance().JavaObject.Call("CallAdcolonyVideo");
        }
        else
        {
            Debug.Log("CallAdcolonyVideo");
        }
    }

    public static void CallAdcolonyRewardVideo()
    {
        if (!IsPC())
        {
            CurrentActivity.getInstance().JavaObject.Call("CallAdcolonyRewardVideo");
        }
        else
        {
            Debug.Log("CallAdcolonyRewardVideo");
        }
    }

    public static void CallNativeXAds()
    {
        if (!IsPC())
        {
            CurrentActivity.getInstance().JavaObject.Call("CallNativeXAds");
        }
        else
        {
            Debug.Log("CallNativeXAds");
        }
    }

    public static string GetCurrentLanguage()
    {
        if (!IsPC())
        {
            return CurrentActivity.getInstance().JavaObject.Call<string>("getCurrentLanguage", new object[0]);
        }
        return "en";
    }

    public static bool IsPC()
    {
        return Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;
    }

    public static int GetRandom(int randomMax)
    {
        int num = Random.Range(0, randomMax);
        Debug.Log("Random Count = " + num);
        return num;
    }

    public static int CheckDevice()
    {
        if (!IsPC())
        {
            return CurrentActivity.getInstance().JavaObject.Call<int>("CheckDevice", new object[0]);
        }
        return 1;
    }

    public static bool isCheckNvsyscaps()
    {
        if (!IsPC())
        {
            return CurrentActivity.getInstance().JavaObject.Call<bool>("isCheckNvsyscaps", new object[0]);
        }
        return true;
    }

    public static int GetVersionType()
    {
        if (!IsPC())
        {
            return CurrentActivity.getInstance().JavaObject.Call<int>("GetVersionType", new object[0]);
        }
        return 1;
    }

    public static void ShowAppUrl(int type)
    {
        if (!IsPC())
        {
            CurrentActivity.getInstance().JavaObject.Call("ShowAPPUrl", type);
        }
        else
        {
            Debug.Log("ShowAppUrl");
        }
    }

    public static void initNativeX()
    {
        if (!IsPC())
        {
            CurrentActivity.getInstance().JavaObject.Call("initNativeX");
        }
        else
        {
            Debug.Log("initNativeX");
        }
    }

    public static void initAdColony()
    {
        if (!IsPC())
        {
            CurrentActivity.getInstance().JavaObject.Call("initAdColony");
        }
        else
        {
            Debug.Log("initAdColony");
        }
    }

    public static void initChartboost()
    {
        if (!IsPC())
        {
            CurrentActivity.getInstance().JavaObject.Call("initChartboost");
        }
        else
        {
            Debug.Log("initChartboost");
        }
    }

    public static void initAppsFlyer()
    {
        if (!IsPC())
        {
            CurrentActivity.getInstance().JavaObject.Call("initAppsFlyer");
        }
        else
        {
            Debug.Log("initAppsFlyer");
        }
    }
}
