using System.Collections.Generic;

public class SwVoidIapChestWWWRequest : SwVoidWWWRequest
{
	private byte chestLevel;

	private string materialWishListStr;

	public SwVoidIapChestWWWRequest(IapScript.IapPackageType type, List<short> materialWishList)
	{
		mResponseData = new SwVoidIapChestResponseData();
		switch (type)
		{
		case IapScript.IapPackageType.CopperChest:
			chestLevel = 1;
			break;
		case IapScript.IapPackageType.SilverChest:
			chestLevel = 2;
			break;
		case IapScript.IapPackageType.GoldChest:
			chestLevel = 3;
			break;
		}
		materialWishListStr = SwJson.ToJson(materialWishList);
		mSign = Hash(mSendUserId.ToString() + chestLevel);
	}

	protected override string GetUrl()
	{
		return "http://" + mVoidServerAddress + "/Sw2/SwVoidIapChest?userId=" + mSendUserId + "&chestLevel=" + chestLevel + "&materialWishList=" + materialWishListStr + "&test=" + mSign;
	}

	public override void ProcessLogic()
	{
		SwVoidIapChestResponseData swVoidIapChestResponseData = mResponseData as SwVoidIapChestResponseData;
		if (!NeedVerify(swVoidIapChestResponseData.result))
		{
			SwVoidOnlineProcess.SwVoidShowChestReward(swVoidIapChestResponseData);
		}
	}

	public override void OnTimeOut()
	{
		if (IapScript.mInstance != null)
		{
			IapScript.mInstance.WaitIAPResultEnd();
			UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"));
		}
	}

	public override void OnResponseError()
	{
		if (mHasVerified)
		{
			if (IapScript.mInstance != null)
			{
				IapScript.mInstance.WaitIAPResultEnd();
				UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"));
			}
		}
		else
		{
			Verify();
		}
	}

	public override void OnVerifyResponseError()
	{
		if (IapScript.mInstance != null)
		{
			IapScript.mInstance.WaitIAPResultEnd();
			UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"));
		}
	}

	public override void OnVerifyFail(int errorCode)
	{
		if (IapScript.mInstance != null)
		{
			IapScript.mInstance.WaitIAPResultEnd();
			UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_7"));
		}
	}
}
