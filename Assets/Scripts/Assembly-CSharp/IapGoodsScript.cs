using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IapGoodsScript : MonoBehaviour, UIMsgListener
{
	public UISprite GoodsIcon;

	public UILabel GoodsAmountLabel;

	public GameObject BuyButton;

	public UISprite GoodsCurrencyIcon;

	public UILabel GoodsPriceLabel;

	public GameObject Arrow;

	public UISprite GoodsSelectBackground;

	public GameObject SalesObject;

	public UILabel PackageDescription;

	public GameObject BonusObject;

	public UISprite BonusBackground;

	public UILabel BonusLabel;

	[HideInInspector]
	public IapScript.IapType m_IapType;

	[HideInInspector]
	public int mAmount;

	[HideInInspector]
	public float mPrice;

	[HideInInspector]
	public bool mIsDollar;

	[HideInInspector]
	public int mIndex;

	private List<UIWidget> mWidgets = new List<UIWidget>();

	public IapPackageData mPackageData;

	private string mProductID;

	private bool owned;

	public string priceString;

	private void Awake()
	{
		mWidgets.Clear();
		UIWidget[] componentsInChildren = GetComponentsInChildren<UIWidget>();
		foreach (UIWidget item in componentsInChildren)
		{
			mWidgets.Add(item);
		}
		SalesObject.SetActive(false);
		UnSelect();
		UIEventListener uIEventListener = UIEventListener.Get(BuyButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnBuyClicked));
		UIEventListener uIEventListener2 = UIEventListener.Get(base.gameObject);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnBuyClicked));
		if (BonusObject != null)
		{
			BonusObject.SetActive(true);
		}
	}

	public void Init(int index, bool isDollar, string price, int amount, string productID)
	{
		mIndex = index;
		mIsDollar = isDollar;
		mAmount = amount;
		m_IapType = IapScript.mInstance.CurrentPage;
		if (m_IapType == IapScript.IapType.Gold)
		{
			GoodsCurrencyIcon.gameObject.SetActive(true);
		}
		mProductID = productID;
		GoodsAmountLabel.text = "x" + mAmount;
		GoodsPriceLabel.text = price;
		priceString = price;
		if (isDollar)
		{
			GoodsCurrencyIcon.spriteName = "dollar2";
		}
		else
		{
			GoodsCurrencyIcon.spriteName = "mithril1";
		}
		if (m_IapType == IapScript.IapType.Mithril)
		{
			GoodsIcon.spriteName = "LV" + (char)(71 - index);
		}
		else if (m_IapType == IapScript.IapType.Gold)
		{
			GoodsIcon.spriteName = "LV" + (char)(71 - index) + "2";
		}
	}

	public void Init(int index, bool isDollar, string price, string name, string productID)
	{
		mIndex = index;
		mIsDollar = isDollar;
		mAmount = 1;
		m_IapType = IapScript.mInstance.CurrentPage;
		mProductID = productID;
		GoodsAmountLabel.text = name;
		GoodsAmountLabel.gameObject.GetComponent<UILocalizationX>().enabled = true;
		if (GameApp.GetInstance().GetUserState().AnalysisManager.GetPackageBuy(index) > 0 && mPackageData != null && !mPackageData.IsMaterialChest())
		{
			GoodsPriceLabel.text = "LOC_ITEM_TIPS_2";
			GoodsPriceLabel.GetComponent<UILocalizationX>().enabled = true;
			owned = true;
		}
		else if (mPackageData.IsMaterialChest())
		{
			GoodsPriceLabel.text = LocalizationManager.GetInstance().GetString("LOC_VOID_M_CHOICE_TIPS_3");
			priceString = price;
		}
		else
		{
			GoodsPriceLabel.text = price;
			priceString = price;
		}
		if (isDollar)
		{
			GoodsCurrencyIcon.spriteName = "dollar2";
		}
		else
		{
			GoodsCurrencyIcon.spriteName = "mithril1";
		}
		GoodsIcon.spriteName = "box" + (index + 1);
		GoodsIcon.MakePixelPerfect();
		if (mPackageData.IsMaterialChest())
		{
			GoodsIcon.transform.localScale = Vector3.one * 0.9f;
		}
		if (PackageDescription != null)
		{
			PackageDescription.text = LocalizationManager.GetInstance().GetString("LOC_IAP_DETAIL_" + (index + 1));
		}
		if (IapScript.mInstance.IsGotoAllMaterialList != IapScript.IapPackageType.RookiePack && IapScript.mInstance.IsGotoAllMaterialList == mPackageData.PackageType)
		{
			IapScript.mInstance.IsGotoAllMaterialList = IapScript.IapPackageType.RookiePack;
			IapScript.mInstance.ShowAllMaterialPage(this);
		}
	}

	public void Select()
	{
		GoodsSelectBackground.gameObject.SetActive(true);
		Arrow.SetActive(true);
		BuyButton.GetComponent<Collider>().enabled = true;
	}

	public void UnSelect()
	{
		GoodsSelectBackground.gameObject.SetActive(false);
		Arrow.SetActive(false);
		BuyButton.GetComponent<Collider>().enabled = false;
	}

	private void OnPress(bool isPress)
	{
		if (isPress)
		{
			IapScript.mInstance.SetNeedUpdate();
		}
	}

	public void OnBuyClicked(GameObject go)
	{
		if (go == base.gameObject && m_IapType == IapScript.IapType.Gold)
		{
			return;
		}
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
		if ((!(go == BuyButton) && (!BuyButton.GetComponent<Collider>().enabled || !(go == base.gameObject))) || owned)
		{
			return;
		}
		if (this == IapScript.mInstance.FreeMithrilGoods)
		{
			GameObject original = Resources.Load("UI/Menu/Ads/Prefab/AdsUI") as GameObject;
			GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
			return;
		}
		UserState userState = GameApp.GetInstance().GetUserState();
		MithrilRewardsScript.mIapFrom = MithrilRewardsScript.IAPFrom.NORMAL;
		switch (m_IapType)
		{
		case IapScript.IapType.Gold:
			if (userState.GetCash() >= Global.MAX_CASH)
			{
				UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_43"));
			}
			else if ((float)userState.GetMithril() >= mPrice)
			{
				userState.BuyWithMithril(Mathf.CeilToInt(mPrice));
				IapData iapData = new IapData();
				iapData.Type = m_IapType;
				iapData.Index = mIndex;
				iapData.Amount = mAmount;
				MithrilRewardsScript.ProvideContent(iapData);
			}
			else
			{
				UIMsgBox.instance.ShowMithrilNotEnoughMessage(this);
			}
			break;
		case IapScript.IapType.Package:
		case IapScript.IapType.Mithril:
			if (mPackageData != null && mPackageData.IsMaterialChest())
			{
				IapScript.mInstance.ShowAllMaterialPage(this);
				break;
			}
			if (mPackageData != null && mPackageData.IsMaterialChest())
			{
				if (!GameApp.GetInstance().GetUserState().HasLoginOnline())
				{
					SwVoidOnlineProcess.NotRegistedMessage();
					break;
				}
				IapScript.mInstance.WaitPackageCheckStart(this);
			}
			else
			{
				IapScript.mInstance.WaitIAPResultStart(this);
			}
			if (mPackageData != null && mPackageData.IsMaterialChest())
			{
				IAP.sPurchaseStatus = 0;
				SwVoidDataManager.SendCheckServerRequest();
			}
			else if (Global.ENABLE_IAP)
			{
				if (GameApp.GetInstance().IsConnectedToInternet())
				{
					IAP.NowPurchaseProduct(mProductID, "1");
					break;
				}
				IapScript.mInstance.WaitIAPResultEnd();
				UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_6"));
			}
			else
			{
				StartCoroutine("ProvideContent");
			}
			break;
		}
	}

	public void SetAlpha(float alpha)
	{
		foreach (UIWidget mWidget in mWidgets)
		{
			mWidget.alpha = alpha;
		}
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (whichMsg.EventId == 3)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				UIMsgBox.instance.CloseMessage();
				IapScript.mInstance.OpenPage(IapScript.IapType.Mithril);
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			}
			else
			{
				UIMsgBox.instance.CloseMessage();
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			}
		}
		else if (buttonId == UIMsg.UIMsgButton.Ok)
		{
			UIMsgBox.instance.CloseMessage();
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
		}
	}

	private IEnumerator ProvideContent()
	{
		IAP.sPurchaseStatus = 0;
		yield return new WaitForSeconds(1f);
		UserState userState = GameApp.GetInstance().GetUserState();
		switch (m_IapType)
		{
		case IapScript.IapType.Mithril:
			MithrilRewardsScript.ProvideContent(new IapData
			{
				Type = m_IapType,
				Index = mIndex,
				Amount = mAmount
			});
			break;
		case IapScript.IapType.Package:
			MithrilRewardsScript.ProvideContent(mPackageData);
			break;
		}
		yield return null;
	}

	public string GetProductID()
	{
		return mProductID;
	}

	public void RefreshPrice()
	{
		if (this != IapScript.mInstance.FreeMithrilGoods && m_IapType != IapScript.IapType.Gold && Application.platform == RuntimePlatform.IPhonePlayer)
		{
			string iAPLocalizedPrice = MithrilRewardsScript.Instance.GetIAPLocalizedPrice(mProductID);
			GoodsPriceLabel.text = iAPLocalizedPrice;
			priceString = iAPLocalizedPrice;
		}
	}

	public void ShowOwned()
	{
		if (mPackageData != null && !mPackageData.IsMaterialChest())
		{
			GoodsPriceLabel.GetComponent<UILocalizationX>().SetText("LOC_ITEM_TIPS_2");
			owned = true;
		}
	}
}
