using System;
using System.Collections;
using UnityEngine;

public class PackagePushScript : MonoBehaviour, UIMsgListener
{
	public static PackagePushScript mInstance;

	public GameObject Parent;

	public GameObject ExitButton;

	public UISprite GoodsIcon;

	public UILabel GoodsNameLabel;

	public UISprite GoodsSales;

	public GameObject BuyButton;

	public UISprite GoodsCurrencyIcon;

	public UILabel GoodsPriceLabel;

	public UILabel GoodsMithrilLabel;

	public UISprite PackageSummary;

	public GameObject HpGroup;

	public UISprite HpBase;

	public UISprite HpUp;

	public UILabel HpBaseLabel;

	public UILabel HpUpLabe;

	public GameObject DpsGroup;

	public UISprite DpsBase;

	public UISprite DpsUp;

	public UILabel DpsBaseLabel;

	public UILabel DpsUpLabe;

	public UILabel PackageDiscriptionLabel;

	public IapPackageData mPackageData;

	public PushCompareScript HPCompare;

	public PushCompareScript DPSCompare;

	private bool mIsExpert;

	private string mProductID;

	private bool mIsWaitingIAPResult;

	private float mWaitingIAPResultTimeOutLimit = 120f;

	private float mWaitingIAPResultStartTime;

	private bool mIsWaitingIAPInfo;

	private float mWaitingIAPInfoTimeOutLimit = 15f;

	private float mWaitingIAPInfoStartTime;

	private bool owned;

	private void Awake()
	{
		mInstance = this;
		UIEventListener uIEventListener = UIEventListener.Get(ExitButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		UIEventListener uIEventListener2 = UIEventListener.Get(BuyButton);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnClickButton));
	}

	public void Init(bool isExpert)
	{
		if (!GameApp.GetInstance().IsConnectedToInternet())
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		if (isExpert)
		{
			mProductID = "com.ifreyr.starwarfare2.ExpertPack";
			GoodsIcon.spriteName = "box2";
			GoodsNameLabel.text = LocalizationManager.GetInstance().GetString("LOC_IAP_PACKETS_2");
			GoodsSales.spriteName = "sale_2";
			PackageSummary.spriteName = "new_txt1";
			PackageDiscriptionLabel.text = LocalizationManager.GetInstance().GetString("LOC_IAP_PACKAGE_DESCRIPTION_2");
			HpBase.fillAmount = UnityEngine.Random.Range(0.35f, 0.4f);
			HpUp.fillAmount = UnityEngine.Random.Range(0.55f, 0.6f);
			DpsBase.fillAmount = UnityEngine.Random.Range(0.35f, 0.4f);
			DpsUp.fillAmount = UnityEngine.Random.Range(0.55f, 0.6f);
		}
		else
		{
			mProductID = "com.ifreyr.starwarfare2.RookiePack";
			GoodsIcon.spriteName = "box1";
			GoodsNameLabel.text = LocalizationManager.GetInstance().GetString("LOC_IAP_PACKETS_1");
			GoodsSales.spriteName = "sale_1";
			PackageSummary.spriteName = "new_txt";
			PackageDiscriptionLabel.text = LocalizationManager.GetInstance().GetString("LOC_IAP_PACKAGE_DESCRIPTION_1");
			HpBase.fillAmount = UnityEngine.Random.Range(0.25f, 0.3f);
			HpUp.fillAmount = UnityEngine.Random.Range(0.4f, 0.45f);
			DpsBase.fillAmount = UnityEngine.Random.Range(0.25f, 0.3f);
			DpsUp.fillAmount = UnityEngine.Random.Range(0.4f, 0.45f);
		}
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.IAP_LIST, DataSheetName.IAP_PACKAGE);
		for (int i = 0; i < gameDataTable.Count; i++)
		{
			string text = gameDataTable.GetString(i, 17);
			if (text.Equals(mProductID))
			{
				mPackageData = new IapPackageData();
				float num = gameDataTable.GetFloat(i, 0);
				mPackageData.Type = IapScript.IapType.Package;
				mPackageData.Index = i;
				mPackageData.PackageType = (IapScript.IapPackageType)(int)Enum.Parse(typeof(IapScript.IapPackageType), gameDataTable.GetString(i, 3), true);
				mPackageData.HeadID = gameDataTable.GetInt(i, 4);
				mPackageData.ChestID = gameDataTable.GetInt(i, 5);
				mPackageData.HandID = gameDataTable.GetInt(i, 6);
				mPackageData.FootID = gameDataTable.GetInt(i, 7);
				mPackageData.BagID1 = gameDataTable.GetInt(i, 8);
				mPackageData.BagID2 = gameDataTable.GetInt(i, 9);
				mPackageData.GunID1 = gameDataTable.GetInt(i, 10);
				mPackageData.GunID2 = gameDataTable.GetInt(i, 11);
				mPackageData.GunID3 = gameDataTable.GetInt(i, 12);
				mPackageData.ITEMID1 = gameDataTable.GetInt(i, 13);
				mPackageData.ITEMID2 = gameDataTable.GetInt(i, 14);
				mPackageData.ITEMID3 = gameDataTable.GetInt(i, 15);
				mPackageData.COLOR = gameDataTable.GetInt(i, 16);
				mPackageData.ProductID = gameDataTable.GetString(i, 17);
				string text2 = ((Global.ENABLE_IAP && Application.platform == RuntimePlatform.IPhonePlayer) ? MithrilRewardsScript.Instance.GetIAPLocalizedPrice(mPackageData.ProductID) : ((Application.platform != RuntimePlatform.Android) ? num.ToString() : ("$" + num)));
				GoodsPriceLabel.text = text2;
				break;
			}
		}
		if (Global.ENABLE_IAP && Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if (!MithrilRewardsScript.Instance.IsLocalizedPriceInitialized(IapScript.IapType.Package))
			{
				Parent.SetActive(false);
				if (!MithrilRewardsScript.Instance.IsLocalizedPriceInitialized(IapScript.IapType.Package))
				{
					IAP.GetProductInfo(0);
				}
				WaitIAPInfoStart();
			}
			else
			{
				ShowLocalizedPrice();
			}
		}
		HPCompare.Init(mPackageData.PackageType);
		DPSCompare.Init(mPackageData.PackageType);
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	private void Update()
	{
		if (mIsWaitingIAPInfo)
		{
			if (Time.time - mWaitingIAPInfoStartTime >= mWaitingIAPInfoTimeOutLimit)
			{
				WaitIAPInfoEnd();
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			if (MithrilRewardsScript.Instance.IsLocalizedPriceInitialized(IapScript.IapType.Package))
			{
				WaitIAPInfoEnd();
				Parent.SetActive(true);
				ShowLocalizedPrice();
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

	public void WaitIAPResultEnd()
	{
		UIMsgBox.instance.CloseMessage();
		mIsWaitingIAPResult = false;
	}

	public void IAPFailed()
	{
		WaitIAPResultEnd();
		UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_IAP_TIPS_4"));
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
	}

	public void IAPSuccess(IapData data)
	{
		if (!GameApp.GetInstance().GetUserState().GetFirstIapSuccess())
		{
			SwFirstIapSuccessRequest request = new SwFirstIapSuccessRequest();
			GameApp.GetInstance().GetWWWManager().SendRequest(request);
		}
		WaitIAPResultEnd();
		UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_IAP_TIPS_5"), 1, 94);
		GoodsPriceLabel.GetComponent<UILocalizationX>().SetText("LOC_ITEM_TIPS_2");
		owned = true;
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_BUY);
	}

	public void WaitIAPResultStart()
	{
		UIMsgBox.instance.ShowMessage(UIMsgBox.instance, string.Empty, 9, 93);
		mIsWaitingIAPResult = true;
		mWaitingIAPResultStartTime = Time.time;
	}

	public void RefreshPrice()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			string iAPLocalizedPrice = MithrilRewardsScript.Instance.GetIAPLocalizedPrice(mProductID);
			GoodsPriceLabel.text = iAPLocalizedPrice;
		}
	}

	public void ShowLocalizedPrice()
	{
		RefreshPrice();
	}

	public void OnClickButton(GameObject go)
	{
		if (go == ExitButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			if (!(go == BuyButton))
			{
				return;
			}
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			if (owned)
			{
				return;
			}
			WaitIAPResultStart();
			MithrilRewardsScript.mIapFrom = MithrilRewardsScript.IAPFrom.PACKAGE_PUSH;
			if (Global.ENABLE_IAP)
			{
				if (GameApp.GetInstance().IsConnectedToInternet())
				{
					IAP.NowPurchaseProduct(mProductID, "1");
					return;
				}
				IapScript.mInstance.WaitIAPResultEnd();
				UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_6"));
			}
			else
			{
				StartCoroutine("ProvideContent");
			}
		}
	}

	private IEnumerator ProvideContent()
	{
		IAP.sPurchaseStatus = 0;
		yield return new WaitForSeconds(1f);
		MithrilRewardsScript.ProvideContent(mPackageData);
		yield return null;
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (whichMsg.EventId == 94 && buttonId == UIMsg.UIMsgButton.Ok)
		{
			UIMsgBox.instance.CloseMessage();
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
