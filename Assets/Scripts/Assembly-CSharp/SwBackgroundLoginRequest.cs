using System;

public class SwBackgroundLoginRequest : SwLoginRequest
{
	protected long mReceiveNextLoginInterval;

	protected override string GetUrl()
	{
		string uUID = GameApp.GetInstance().UUID;
		string text = GameApp.GetInstance().IDFA;
		if (text == null)
		{
			text = string.Empty;
		}
		string empty = string.Empty;
		mSign = Hash(uUID);
		return "http://" + base.LOGIN_SERVER_ADDRESS + "/Sw2/SwBackgroundLogin?udid=" + ToBase64String(uUID) + "&idfa=" + ToBase64String(text) + "&ver=" + ToBase64String("124") + "&platform=" + ToBase64String(((int)base.Platform).ToString()) + "&device=" + ToBase64String(base.Device) + "&os=" + ToBase64String(base.OS) + "&token=" + ToBase64String(empty) + "&test=" + mSign;
	}

	protected override void ReadData(BytesBuffer bb)
	{
		mReceiveErrorCode = bb.ReadInt();
		if (mReceiveErrorCode == SwErrorCode.SUCCESS)
		{
			mReceiveNextLoginInterval = bb.ReadLong();
		}
	}

	public override void ProcessLogic()
	{
		if (mReceiveErrorCode == SwErrorCode.SUCCESS)
		{
			GameApp.GetInstance().GetUserState().SetLastLoginTicks(DateTime.Now.Ticks);
			GameApp.GetInstance().GetUserState().SetNextLoginInterval(mReceiveNextLoginInterval * 10000);
		}
	}

	public override void OnTimeOut()
	{
	}

	public override void OnResponseError()
	{
	}
}
