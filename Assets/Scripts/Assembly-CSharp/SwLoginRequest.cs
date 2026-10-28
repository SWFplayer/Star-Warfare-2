using UnityEngine;

public class SwLoginRequest : WWWRequest
{
	protected const string LOGIN_SERVER_PORT = "10365";

	protected string LOGIN_SERVER_ADDRESS
	{
		get
		{
			string text = "124.232.150.72";
			text = "sw2.freyrgames.com";
			return text + ":10365";
		}
	}

	protected string Device
	{
		get
		{
			return SystemInfo.deviceModel;
		}
	}

	protected string OS
	{
		get
		{
			return SystemInfo.operatingSystem;
		}
	}

	protected EDevicePlatform Platform
	{
		get
		{
			return EDevicePlatform.ANDROID;
		}
	}

	public override void OnTimeOut()
	{
		UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"));
	}

	public override void OnResponseError()
	{
		UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"));
	}
}
