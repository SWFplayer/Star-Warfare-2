using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Facebook;
using UnityEngine;

public sealed class FB : ScriptableObject
{
    private static IFacebook FacebookImpl
    {
        get
        {
            if (FB.facebook == null)
            {
                throw new NullReferenceException("Facebook object is not yet loaded.  Did you call FB.Init()?");
            }
            return FB.facebook;
        }
    }

    public static string AppId
    {
        get
        {
            return FB.appId;
        }
    }

    public static string UserId
    {
        get
        {
            return (FB.facebook == null) ? string.Empty : FB.facebook.UserId;
        }
    }

    public static string AccessToken
    {
        get
        {
            return (FB.facebook == null) ? string.Empty : FB.facebook.AccessToken;
        }
    }

    public static DateTime AccessTokenExpiresAt
    {
        get
        {
            return (FB.facebook == null) ? DateTime.MinValue : FB.facebook.AccessTokenExpiresAt;
        }
    }

    public static bool IsLoggedIn
    {
        get
        {
            return FB.facebook != null && FB.facebook.IsLoggedIn;
        }
    }

    public static bool IsInitialized
    {
        get
        {
            return FB.facebook != null && FB.facebook.IsInitialized;
        }
    }

    public static void Init(InitDelegate onInitComplete, HideUnityDelegate onHideUnity = null, string authResponse = null)
    {
        FB.Init(onInitComplete, FBSettings.AppId, FBSettings.Cookie, FBSettings.Logging, FBSettings.Status, FBSettings.Xfbml, FBSettings.FrictionlessRequests, onHideUnity, authResponse);
    }

    public static void Init(InitDelegate onInitComplete, string appId, bool cookie = true, bool logging = true, bool status = true, bool xfbml = false, bool frictionlessRequests = true, HideUnityDelegate onHideUnity = null, string authResponse = null)
    {
        FB.appId = appId;
        FB.cookie = cookie;
        FB.logging = logging;
        FB.status = status;
        FB.xfbml = xfbml;
        FB.frictionlessRequests = frictionlessRequests;
        FB.authResponse = authResponse;
        FB.OnInitComplete = onInitComplete;
        FB.OnHideUnity = onHideUnity;
        if (!FB.isInitCalled)
        {
            FBBuildVersionAttribute versionAttributeOfType = FBBuildVersionAttribute.GetVersionAttributeOfType(typeof(IFacebook));
            if (versionAttributeOfType == null)
            {
                FbDebug.Warn("Cannot find Facebook SDK Version");
            }
            else
            {
                FbDebug.Info(string.Format("Using SDK {0}, Build {1}", versionAttributeOfType.SdkVersion, versionAttributeOfType.BuildVersion));
            }
            FBComponentFactory.GetComponent<AndroidFacebookLoader>(IfNotExist.AddNew);
            FB.isInitCalled = true;
            return;
        }
        FbDebug.Warn("FB.Init() has already been called.  You only need to call this once and only once.");
        if (FB.FacebookImpl != null)
        {
            FB.OnDllLoaded();
        }
    }

    private static void OnDllLoaded()
    {
        FBBuildVersionAttribute versionAttributeOfType = FBBuildVersionAttribute.GetVersionAttributeOfType(FB.FacebookImpl.GetType());
        if (versionAttributeOfType != null)
        {
            FbDebug.Log(string.Format("Finished loading Facebook dll. Version {0} Build {1}", versionAttributeOfType.SdkVersion, versionAttributeOfType.BuildVersion));
        }
        FB.FacebookImpl.Init(FB.OnInitComplete, FB.appId, FB.cookie, FB.logging, FB.status, FB.xfbml, FBSettings.ChannelUrl, FB.authResponse, FB.frictionlessRequests, FB.OnHideUnity);
    }

    public static void Login(string scope = "", FacebookDelegate callback = null)
    {
        FB.FacebookImpl.Login(scope, callback);
    }

    public static void Logout()
    {
        FB.FacebookImpl.Logout();
    }

    public static void AppRequest(string message, OGActionType actionType, string objectId, string[] to, string data = "", string title = "", FacebookDelegate callback = null)
    {
        FB.FacebookImpl.AppRequest(message, actionType, objectId, to, null, null, null, data, title, callback);
    }

    public static void AppRequest(string message, OGActionType actionType, string objectId, List<object> filters = null, string[] excludeIds = null, int? maxRecipients = null, string data = "", string title = "", FacebookDelegate callback = null)
    {
        FB.FacebookImpl.AppRequest(message, actionType, objectId, null, filters, excludeIds, maxRecipients, data, title, callback);
    }

    public static void AppRequest(string message, string[] to = null, List<object> filters = null, string[] excludeIds = null, int? maxRecipients = null, string data = "", string title = "", FacebookDelegate callback = null)
    {
        FB.FacebookImpl.AppRequest(message, null, null, to, filters, excludeIds, maxRecipients, data, title, callback);
    }

    public static void Feed(string toId = "", string link = "", string linkName = "", string linkCaption = "", string linkDescription = "", string picture = "", string mediaSource = "", string actionName = "", string actionLink = "", string reference = "", Dictionary<string, string[]> properties = null, FacebookDelegate callback = null)
    {
        FB.FacebookImpl.FeedRequest(toId, link, linkName, linkCaption, linkDescription, picture, mediaSource, actionName, actionLink, reference, properties, callback);
    }

    public static void API(string query, HttpMethod method, FacebookDelegate callback = null, Dictionary<string, string> formData = null)
    {
        FB.FacebookImpl.API(query, method, formData, callback);
    }

    public static void API(string query, HttpMethod method, FacebookDelegate callback, WWWForm formData)
    {
        FB.FacebookImpl.API(query, method, formData, callback);
    }

    [Obsolete("use FB.ActivateApp()")]
    public static void PublishInstall(FacebookDelegate callback = null)
    {
        FB.FacebookImpl.PublishInstall(FB.AppId, callback);
    }

    public static void ActivateApp()
    {
        FB.FacebookImpl.ActivateApp(FB.AppId);
    }

    public static void GetDeepLink(FacebookDelegate callback)
    {
        FB.FacebookImpl.GetDeepLink(callback);
    }

    public static void GameGroupCreate(string name, string description, string privacy = "CLOSED", FacebookDelegate callback = null)
    {
        FB.FacebookImpl.GameGroupCreate(name, description, privacy, callback);
    }

    public static void GameGroupJoin(string id, FacebookDelegate callback = null)
    {
        FB.FacebookImpl.GameGroupJoin(id, callback);
    }

    public static InitDelegate OnInitComplete;

    public static HideUnityDelegate OnHideUnity;

    private static IFacebook facebook;

    private static string authResponse;

    private static bool isInitCalled;

    private static string appId;

    private static bool cookie;

    private static bool logging;

    private static bool status;

    private static bool xfbml;

    private static bool frictionlessRequests;

    public sealed class AppEvents
    {
        public static bool LimitEventUsage
        {
            get
            {
                return FB.facebook != null && FB.facebook.LimitEventUsage;
            }
            set
            {
                FB.facebook.LimitEventUsage = value;
            }
        }

        public static void LogEvent(string logEvent, float? valueToSum = null, Dictionary<string, object> parameters = null)
        {
            FB.FacebookImpl.AppEventsLogEvent(logEvent, valueToSum, parameters);
        }

        public static void LogPurchase(float logPurchase, string currency = "USD", Dictionary<string, object> parameters = null)
        {
            FB.FacebookImpl.AppEventsLogPurchase(logPurchase, currency, parameters);
        }
    }

    public sealed class Canvas
    {
        public static void Pay(string product, string action = "purchaseitem", int quantity = 1, int? quantityMin = null, int? quantityMax = null, string requestId = null, string pricepointId = null, string testCurrency = null, FacebookDelegate callback = null)
        {
            FB.FacebookImpl.Pay(product, action, quantity, quantityMin, quantityMax, requestId, pricepointId, testCurrency, callback);
        }

        public static void SetResolution(int width, int height, bool fullscreen, int preferredRefreshRate = 0, params FBScreen.Layout[] layoutParams)
        {
            FBScreen.SetResolution(width, height, fullscreen, preferredRefreshRate, layoutParams);
        }

        public static void SetAspectRatio(int width, int height, params FBScreen.Layout[] layoutParams)
        {
            FBScreen.SetAspectRatio(width, height, layoutParams);
        }
    }

    public sealed class Android
    {
        public static string KeyHash
        {
            get
            {
                AndroidFacebook androidFacebook = FB.facebook as AndroidFacebook;
                return (!(androidFacebook != null)) ? string.Empty : androidFacebook.KeyHash;
            }
        }
    }

    public abstract class RemoteFacebookLoader : MonoBehaviour
    {
        public static IEnumerator LoadFacebookClass(string className, FB.RemoteFacebookLoader.LoadedDllCallback callback)
        {
            string url = string.Format(IntegratedPluginCanvasLocation.DllUrl, className);
            WWW www = new WWW(url);
            FbDebug.Log("loading dll: " + url);
            yield return www;
            if (www.error != null)
            {
                FbDebug.Error(www.error);
                if (FB.RemoteFacebookLoader.retryLoadCount < 3)
                {
                    FB.RemoteFacebookLoader.retryLoadCount++;
                }
                www.Dispose();
                yield break;
            }


            IFacebook fb = typeof(FBComponentFactory).GetMethod("GetComponent").MakeGenericMethod(new Type[]
            {

            }).Invoke(null, new object[]
            {
                IfNotExist.AddNew
            }) as IFacebook;
            if (fb == null)
            {
                FbDebug.Error(className + " couldn't be created.");
                www.Dispose();
                yield break;
            }
            callback(fb);
            www.Dispose();
            yield break;
        }

        protected abstract string className { get; }

        private IEnumerator Start()
        {
            IEnumerator loader = FB.RemoteFacebookLoader.LoadFacebookClass(this.className, new FB.RemoteFacebookLoader.LoadedDllCallback(this.OnDllLoaded));
            while (loader.MoveNext())
            {
                object obj = loader.Current;
                yield return obj;
            }
            UnityEngine.Object.Destroy(this);
            yield break;
        }

        private void OnDllLoaded(IFacebook fb)
        {
            FB.facebook = fb;
            FB.OnDllLoaded();
        }

        private const string facebookNamespace = "Facebook.";

        private const int maxRetryLoadCount = 3;

        private static int retryLoadCount;

        public delegate void LoadedDllCallback(IFacebook fb);
    }

    public abstract class CompiledFacebookLoader : MonoBehaviour
    {
        protected abstract IFacebook fb { get; }

        private void Start()
        {
            FB.facebook = this.fb;
            FB.OnDllLoaded();
            UnityEngine.Object.Destroy(this);
        }
    }
}
