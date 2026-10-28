using System;
using UnityEngine;

public class AdsUIScript : MonoBehaviour
{
	public GameObject SponsorPayButton;

	public GameObject TapjoyButton1;

	public GameObject TapjoyButton2;

	public GameObject NativeXButton;

	public GameObject AdColonyButton;

	public GameObject FlurryButton;

	public GameObject ExitButton;

	private void Start()
	{
		UIEventListener uIEventListener = UIEventListener.Get(SponsorPayButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(TapjoyButton1);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(TapjoyButton2);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener4 = UIEventListener.Get(NativeXButton);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener5 = UIEventListener.Get(AdColonyButton);
		uIEventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener5.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener6 = UIEventListener.Get(FlurryButton);
		uIEventListener6.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener6.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener7 = UIEventListener.Get(ExitButton);
		uIEventListener7.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener7.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		if (GameApp.GetInstance().FlurryStatus == 0)
		{
			FlurryButton.SetActive(false);
		}
		if (GameApp.GetInstance().TapJoyStatus == 0)
		{
			TapjoyButton1.SetActive(false);
		}
		if (GameApp.GetInstance().TapJoyStatus == 0)
		{
			TapjoyButton2.SetActive(false);
		}
		if (GameApp.GetInstance().NativeXStatus == 0)
		{
			NativeXButton.SetActive(false);
		}
		if (GameApp.GetInstance().AdcolonyStatus == 0)
		{
			AdColonyButton.SetActive(false);
		}
		if (GameApp.GetInstance().SponsorPayStatus == 0)
		{
			SponsorPayButton.SetActive(false);
		}
		if (GameApp.GetInstance().AppStatus != 1 || !GameApp.GetInstance().isAdInitFinished)
		{
			return;
		}
		if (GameApp.GetInstance().SponsorPaySize > GameApp.GetInstance().NativeXSize)
		{
			if (GameApp.GetInstance().SponsorPayStatus == 1)
			{
				AndroidPluginScript.CallSponsorPayOfferWall();
			}
		}
		else if (GameApp.GetInstance().NativeXStatus == 1)
		{
			AndroidPluginScript.CallNativeXAds();
		}
	}

	public void OnNativeExceptionReceivedFromSDK(string message)
	{
		Debug.Log("message = " + message);
	}

	public void OnOFWResultReceived(string message)
	{
	}

	private void OnButtonClick(GameObject go)
	{
		if (go == SponsorPayButton)
		{
			AndroidPluginScript.CallSponsorPayOfferWall();
		}
		else if (go == TapjoyButton1)
		{
			AndroidPluginScript.CallTapjoyOfferWall();
		}
		else if (go == TapjoyButton2)
		{
			Application.OpenURL("https://www.tapjoy.com/earn?eid=eec60921a2406fc8dfe568723fa4a818897b1b82168487c88fd575d168e3abd66e0b02edfb191eb3cfd4bc656c7c558f&referrer=StarWarfare");
		}
		else if (go == NativeXButton)
		{
			AndroidPluginScript.CallNativeXAds();
		}
		else if (go == AdColonyButton)
		{
			AndroidPluginScript.CallAdcolonyRewardVideo();
		}
		else if (go == FlurryButton)
		{
			AndroidPluginScript.CallFlurryAds();
		}
		else if (go == ExitButton)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
