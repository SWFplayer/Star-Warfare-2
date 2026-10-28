using UnityEngine;

public class ShopItemContainter : MonoBehaviour
{
	public ScrollEquipsScript ScrollEquipsObject;

	public UISprite Background;

	public GameObject PriceObj;

	public UISprite CurrencyIcon;

	public UILabel Price;

	public UISprite EquipIcon;

	public UILabel UnlockTip;

	public UISprite LockIcon;

	public GameObject LockObj;

	public UILabel PVPUnlockTip;

	public UISprite SuitStateSprite;

	public UISprite SuitStateSpriteBackground;

	public UILabel ItemCount;

	public UISprite OwnedTag;

	public GameObject SaleTimeObj;

	public UILabel SaleCountDown;

	private UIWidgetSet mWidgetSet;

	private int mTag;

	private bool mIsGold = true;

	private string _mPrice;

	public int EquipID { get; set; }

	private int mPrice
	{
		get
		{
			return AntiCracking.DecryptBufferStr(_mPrice, "zhuangbeijiage");
		}
		set
		{
			_mPrice = AntiCracking.CryptBufferStr(value, "zhuangbeijiage");
		}
	}

	public UIWidgetSet WidgetSet
	{
		get
		{
			return mWidgetSet;
		}
	}

	private void Awake()
	{
		mWidgetSet = GetComponent<UIWidgetSet>();
		mWidgetSet.SetAlpha(0.5f);
		if (ItemCount != null)
		{
			ItemCount.gameObject.SetActive(false);
		}
		if (SaleTimeObj != null)
		{
			SaleTimeObj.SetActive(false);
		}
	}

	private void Update()
	{
		if (SaleTimeObj != null && SaleTimeObj.activeSelf && SaleCountDown != null)
		{
			if (GameApp.GetInstance().GetUserState().GetTrialWeaponBuySeconds() > 0f)
			{
				SaleCountDown.text = GameApp.GetInstance().GetUserState().GetTrialWeaponBuySecondsString();
				return;
			}
			GameApp.GetInstance().GetUserState().CheckTrialWeaponBuySecondsOver();
			SaleTimeObj.SetActive(false);
			mPrice = Mathf.CeilToInt((float)mPrice / Global.TRIAL_WEAPON_SALE_RATE);
			CheckMoneyOrMithrilEnough();
		}
	}

	public void SetPrice(Weapon weapon)
	{
		if (weapon.Mithril == 0)
		{
			CurrencyIcon.spriteName = "icon_dollar1";
			CurrencyIcon.width = 35;
			CurrencyIcon.height = 35;
			mIsGold = true;
			mPrice = weapon.Price;
		}
		else
		{
			CurrencyIcon.spriteName = "icon_mithril1";
			CurrencyIcon.width = 35;
			CurrencyIcon.height = 35;
			mIsGold = false;
			mPrice = weapon.Mithril;
		}
		CheckMoneyOrMithrilEnough();
		CurrencyIcon.transform.localScale = Vector3.one * 0.5f;
		PriceObj.SetActive(false);
		if (weapon.Level != 0 && weapon.Level != 15)
		{
			OwnedTag.gameObject.SetActive(true);
		}
		string str = string.Empty;
		if (weapon.UnlockLevel.Difficult == EDifficultMode.NORMAL)
		{
			str = LocalizationManager.GetInstance().GetString("LOC_BUTTON_MODE_1");
		}
		else if (weapon.UnlockLevel.Difficult == EDifficultMode.HEROIC)
		{
			str = LocalizationManager.GetInstance().GetString("LOC_BUTTON_MODE_2");
		}
		UnlockTip.text = LocalizationManager.GetInstance().GetString("LOC_ICON_25", weapon.UnlockLevel.StageID + 1, str);
		if (PVPUnlockTip != null)
		{
			int rankID = GameApp.GetInstance().GetUserState().GetRank()
				.rankID;
			if (rankID < weapon.UnlockPVPLevel)
			{
				PVPUnlockTip.text = LocalizationManager.GetInstance().GetString("LOC_ICON_26", weapon.UnlockPVPLevel);
			}
			else
			{
				PVPUnlockTip.text = string.Empty;
			}
		}
		if (GameApp.GetInstance().GetUserState().GetTrialWeaponBuyID() == weapon.GunID)
		{
			SaleTimeObj.SetActive(true);
		}
		mTag = 5;
	}

	public void SetPrice(Armor armor, int tag)
	{
		if (armor.Mithril == 0)
		{
			CurrencyIcon.spriteName = "icon_dollar1";
			CurrencyIcon.width = 35;
			CurrencyIcon.height = 35;
			mIsGold = true;
			mPrice = armor.Price;
		}
		else
		{
			CurrencyIcon.spriteName = "icon_mithril1";
			CurrencyIcon.width = 35;
			CurrencyIcon.height = 35;
			mIsGold = false;
			mPrice = armor.Mithril;
		}
		CheckMoneyOrMithrilEnough();
		CurrencyIcon.transform.localScale = Vector3.one * 0.5f;
		PriceObj.SetActive(false);
		if (armor.Level != 0 && armor.Level != 15)
		{
			OwnedTag.gameObject.SetActive(true);
		}
		string str = string.Empty;
		if (armor.UnlockLevel.Difficult == EDifficultMode.NORMAL)
		{
			str = LocalizationManager.GetInstance().GetString("LOC_BUTTON_MODE_1");
		}
		else if (armor.UnlockLevel.Difficult == EDifficultMode.HEROIC)
		{
			str = LocalizationManager.GetInstance().GetString("LOC_BUTTON_MODE_2");
		}
		UnlockTip.text = LocalizationManager.GetInstance().GetString("LOC_ICON_25", armor.UnlockLevel.StageID + 1, str);
		if (PVPUnlockTip != null)
		{
			int rankID = GameApp.GetInstance().GetUserState().GetRank()
				.rankID;
			if (rankID < armor.UnlockPVPLevel)
			{
				PVPUnlockTip.text = LocalizationManager.GetInstance().GetString("LOC_ICON_26", armor.UnlockPVPLevel);
			}
			else
			{
				PVPUnlockTip.text = string.Empty;
			}
		}
		mTag = tag;
	}

	public void SetPrice(ShopItem shopItem)
	{
		if (shopItem.GetMithril() == 0)
		{
			CurrencyIcon.spriteName = "icon_dollar1";
			CurrencyIcon.width = 35;
			CurrencyIcon.height = 35;
			mIsGold = true;
			mPrice = shopItem.GetGold();
		}
		else
		{
			CurrencyIcon.spriteName = "icon_mithril1";
			CurrencyIcon.width = 35;
			CurrencyIcon.height = 35;
			mIsGold = false;
			mPrice = shopItem.GetMithril();
		}
		CheckMoneyOrMithrilEnough();
		CurrencyIcon.transform.localScale = Vector3.one * 0.5f;
		PriceObj.SetActive(false);
		mTag = 6;
		ItemCount.gameObject.SetActive(true);
		ItemCount.text = "x" + shopItem.GetCount();
		if (PVPUnlockTip != null)
		{
			PVPUnlockTip.text = string.Empty;
		}
	}

	public void SetSuitStateSprite(Armor armor)
	{
		if (SuitStateSprite != null && SuitStateSpriteBackground != null)
		{
			if (armor.GetArmorType() != BodyType.Bag)
			{
				SuitStateSprite.gameObject.SetActive(true);
				SuitStateSprite.spriteName = "suits_" + armor.SetElement;
				SuitStateSpriteBackground.gameObject.SetActive(true);
			}
			else
			{
				SuitStateSprite.gameObject.SetActive(false);
				SuitStateSpriteBackground.gameObject.SetActive(false);
			}
		}
	}

	private void OnPress(bool isPressed)
	{
		if (isPressed)
		{
			ScrollEquipsObject.ShopUI.Description.EffectLine.fillAmount = 0f;
			ScrollEquipsObject.SetNeedUpdate();
		}
	}

	public void SetLock()
	{
		LockObj.SetActive(true);
		LockIcon.gameObject.SetActive(true);
		EquipIcon.alpha = 0.6f;
	}

	public void CheckMoneyOrMithrilEnough()
	{
		if (mIsGold)
		{
			if (GameApp.GetInstance().GetUserState().GetCash() >= mPrice)
			{
				Price.text = UIConstant.CURRENCY_COLOR_GOLD_PREFIX + mPrice + UIConstant.COLOR_POSTFIX;
			}
			else
			{
				Price.text = UIConstant.CURRENCY_COLOR_NOT_ENOUGH_PREFIX + mPrice + UIConstant.COLOR_POSTFIX;
			}
		}
		else if (GameApp.GetInstance().GetUserState().GetMithril() >= mPrice)
		{
			Price.text = UIConstant.CURRENCY_COLOR_MITHRIL_PREFIX + mPrice + UIConstant.COLOR_POSTFIX;
		}
		else
		{
			Price.text = UIConstant.CURRENCY_COLOR_NOT_ENOUGH_PREFIX + mPrice + UIConstant.COLOR_POSTFIX;
		}
	}

	public bool NeedShowPVPUnlockTip()
	{
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && PVPUnlockTip != null && PVPUnlockTip.text != string.Empty && !LockObj.activeSelf)
		{
			return true;
		}
		return false;
	}
}
