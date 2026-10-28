using System;
using System.Collections;
using UnityEngine;

public class PvpExpIapUI : MonoBehaviour
{
	public UILabel priceInfo;

	public UILabel levelUpInfo;

	public UILabel priceOnButton;

	public GameObject cancelObj;

	public GameObject confirmObj;

	private string localPrice;

	private bool loadPriceSuccess;

	private bool mIsWaitingIAPResult;

	private float mWaitingIAPResultTimeOutLimit = 30f;

	private float mWaitingIAPResultStartTime;

	private bool mIsWaitingIAPInfo;

	private float mWaitingIAPInfoTimeOutLimit = 15f;

	private float mWaitingIAPInfoStartTime;

	private string productId;

	private void Awake()
	{
		UIEventListener uIEventListener = UIEventListener.Get(cancelObj);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(confirmObj);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
	}

	private void Update()
	{
		if (mIsWaitingIAPInfo)
		{
			if (Time.time - mWaitingIAPInfoStartTime >= mWaitingIAPInfoTimeOutLimit)
			{
				WaitIAPInfoEnd();
				UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_27"));
				base.gameObject.SetActive(false);
			}
			if (MithrilRewardsScript.Instance.IsLocalizedPriceInitialized(IapScript.IapType.PVP))
			{
				WaitIAPInfoEnd();
				SetLocalPrice();
				loadPriceSuccess = true;
				RefreshBuyButton();
			}
		}
		if (!mIsWaitingIAPResult)
		{
			return;
		}
		if (Time.time - mWaitingIAPResultStartTime >= mWaitingIAPResultTimeOutLimit)
		{
			IAPFailed();
			return;
		}
		switch (IAP.purchaseStatus(null))
		{
		case 0:
			break;
		case 1:
		case 10:
			WaitIAPResultEnd();
			break;
		default:
			IAPFailed();
			break;
		}
	}

	public void WaitIAPInfoStart()
	{
		UIMsgBox.instance.ShowMessage(UIMsgBox.instance, LocalizationManager.GetInstance().GetString("LOC_IAP_PRICECHECK"), 7);
		mIsWaitingIAPInfo = true;
		mWaitingIAPInfoStartTime = Time.time;
	}

	public void WaitIAPInfoEnd()
	{
		UIMsgBox.instance.CloseMessage();
		mIsWaitingIAPInfo = false;
	}

	public void WaitIAPResultStart()
	{
		UIMsgBox.instance.ShowMessage(UIMsgBox.instance, string.Empty, 9);
		mIsWaitingIAPResult = true;
		mWaitingIAPResultStartTime = Time.time;
	}

	public void WaitPackageCheckStart()
	{
		UIMsgBox.instance.ShowMessage(UIMsgBox.instance, string.Empty, 9);
	}

	public void WaitIAPResultEnd()
	{
		UIMsgBox.instance.CloseMessage();
		mIsWaitingIAPResult = false;
		base.gameObject.SetActive(false);
	}

	public void IAPFailedInVoidServer(int errorCode)
	{
		WaitIAPResultEnd();
		UIMsgBox.instance.ShowErrorCodeMessage(errorCode);
	}

	public void IAPFailed()
	{
		WaitIAPResultEnd();
		UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_IAP_TIPS_4"));
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
	}

	public void InitInfo(string LocString)
	{
		InitIapInfo();
		UserState userState = GameApp.GetInstance().GetUserState();
		int num = 50000;
		Rank rank = userState.GetRank();
		Rank rank2 = userState.GetRank(userState.GetExp() + num);
		string str = LocalizationManager.GetInstance().GetString(rank.rankName);
		string str2 = LocalizationManager.GetInstance().GetString(rank2.rankName);
		priceInfo.text = string.Empty;
		levelUpInfo.text = LocalizationManager.GetInstance().GetString(LocString, num, rank.rankID, rank2.rankID, string.Empty, str, str2, string.Empty);
		RefreshBuyButton();
	}

	public void OnButtonClick(GameObject go)
	{
		if (go == cancelObj)
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			if (!(go == confirmObj))
			{
				return;
			}
			MithrilRewardsScript.mIapFrom = MithrilRewardsScript.IAPFrom.NORMAL;
			WaitIAPResultStart();
			if (Global.ENABLE_IAP)
			{
				if (GameApp.GetInstance().IsConnectedToInternet())
				{
					IAP.NowPurchaseProduct(productId, "1");
					return;
				}
				WaitIAPResultEnd();
				UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_6"));
			}
			else
			{
				StartCoroutine("ProvideContent");
			}
		}
	}

	public void InitIapInfo()
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.IAP_LIST, DataSheetName.IAP_PVP);
		productId = gameDataTable.GetString(0, 4);
		loadPriceSuccess = true;
		SetLocalPriceFromDataTable();
	}

	public void SetLocalPrice()
	{
		localPrice = MithrilRewardsScript.Instance.GetIAPLocalizedPrice(productId);
	}

	public void SetLocalPriceFromDataTable()
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.IAP_LIST, DataSheetName.IAP_PVP);
		localPrice = gameDataTable.GetString(0, 1);
	}

	public void RefreshBuyButton()
	{
		confirmObj.SetActive(true);
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.IAP_LIST, DataSheetName.IAP_PVP);
		priceOnButton.text = "$" + gameDataTable.GetString(0, 1);
	}

	private IEnumerator ProvideContent()
	{
		IAP.sPurchaseStatus = 0;
		yield return new WaitForSeconds(1f);
		MithrilRewardsScript.ProvideContent(new IapData
		{
			Type = IapScript.IapType.PVP,
			Index = 0,
			Amount = 1
		});
		base.gameObject.SetActive(false);
		yield return null;
	}
}
