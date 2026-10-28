using System;

public class SwGetServerTimeRequest : SwLoginRequest
{
	private long mReceiveServerTicks;

	private int mPushItemIntermission;

	protected override string GetUrl()
	{
		string uUID = GameApp.GetInstance().UUID;
		mSign = Hash(uUID);
		return "http://" + base.LOGIN_SERVER_ADDRESS + "/Sw2/SwGetServerTime?udid=" + ToBase64String(uUID) + "&ver=" + ToBase64String("124") + "&platform=" + ToBase64String(((int)base.Platform).ToString()) + "&test=" + mSign;
	}

	protected override void ReadData(BytesBuffer bb)
	{
		mReceiveErrorCode = bb.ReadInt();
		if (mReceiveErrorCode == SwErrorCode.SUCCESS)
		{
			mReceiveServerTicks = bb.ReadLong();
			mPushItemIntermission = bb.ReadInt();
		}
	}

	public override void ProcessLogic()
	{
		if (mReceiveErrorCode == SwErrorCode.SUCCESS)
		{
			DateTime serverTime = TimeUtil.ConvertServerTicksToLocalTime(mReceiveServerTicks);
			if (GameApp.GetInstance().GetUserState() != null)
			{
				GameApp.GetInstance().GetUserState().CheckTrialPushDate(serverTime);
				GameApp.GetInstance().GetUserState().CheckWeaponUpgradePushDate(serverTime);
				GameApp.GetInstance().GetUserState().SetPushItemInterval(mPushItemIntermission);
				GameApp.GetInstance().Save();
			}
		}
	}

	public override void OnTimeOut()
	{
	}

	public override void OnResponseError()
	{
	}
}
