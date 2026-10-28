using System;
using UnityEngine;

public class WeaponUpgradePlanScript : MonoBehaviour, UIMsgListener
{
	public GameObject BuyButton;

	public UISprite Bar;

	public UISprite SaleOffIcon;

	public UILabel NormalPrice;

	public UILabel Price;

	public UISprite ReduceLine;

	public UISprite WeaponIcon;

	public UILabel CurrentLevel;

	public UILabel GoalLevel;

	public PushCompareScript DamageCompare;

	public PushCompareScript FireRateCompare;

	public PushCompareScript SpeedCompare;

	protected WeaponUpgradePushScript mParent;

	private Weapon mTargetWeapon;

	private int mMithril;

	private int mOldLevel = 1;

	private int mNewLevel = 1;

	private bool mInited;

	private float mCheckPriceTime = 0.15f;

	private float mLastCheckPriceTime;

	private void Awake()
	{
		UIEventListener uIEventListener = UIEventListener.Get(BuyButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		mInited = false;
	}

	private void Update()
	{
		if (mInited && Time.time - mLastCheckPriceTime >= mCheckPriceTime)
		{
			mLastCheckPriceTime = Time.time;
			CheckPrice();
		}
	}

	public void Init(Weapon targetWeapon, int oldLevel, int newLevel, float priceOff, WeaponUpgradePushScript parent)
	{
		mInited = true;
		mParent = parent;
		mTargetWeapon = targetWeapon;
		if (mTargetWeapon != null)
		{
			WeaponIcon.spriteName = "weapons_" + mTargetWeapon.GunID;
			CurrentLevel.text = oldLevel.ToString();
			GoalLevel.text = newLevel.ToString();
			DamageCompare.InitForWeaponUpgradePush(mTargetWeapon, oldLevel, newLevel);
			FireRateCompare.InitForWeaponUpgradePush(mTargetWeapon, oldLevel, newLevel);
			SpeedCompare.InitForWeaponUpgradePush(mTargetWeapon, oldLevel, newLevel);
			int upgradePriceToLevel = mTargetWeapon.GetUpgradePriceToLevel(oldLevel, newLevel);
			mMithril = Mathf.CeilToInt((float)upgradePriceToLevel / (float)Global.WEAPON_UPGRADE_PUSH_MITHRIL_EXCHANGE_RATE * priceOff);
			CheckPrice();
			NormalPrice.text = upgradePriceToLevel.ToString();
			ReduceLine.width = NormalPrice.width;
			if (priceOff == Global.WEAPON_UPGRADE_PUSH_OFF[0])
			{
				SaleOffIcon.spriteName = "sale_5";
			}
			else if (priceOff == Global.WEAPON_UPGRADE_PUSH_OFF[1])
			{
				SaleOffIcon.spriteName = "sale_6";
			}
			else if (priceOff == Global.WEAPON_UPGRADE_PUSH_OFF[2])
			{
				SaleOffIcon.spriteName = "sale_7";
			}
			mOldLevel = oldLevel;
			mNewLevel = newLevel;
			mLastCheckPriceTime = Time.time;
		}
	}

	private void OnButtonClick(GameObject go)
	{
		if (!(go == BuyButton))
		{
			return;
		}
		UserState userState = GameApp.GetInstance().GetUserState();
		if (userState.GetMithril() >= mMithril)
		{
			userState.BuyWithMithril(mMithril);
			for (int i = mOldLevel; i < mNewLevel; i++)
			{
				mTargetWeapon.UpgradeByPlan(mNewLevel);
			}
			GameApp.GetInstance().Save();
			GameObject original = Resources.Load("UI/Menu/LevelUpWindow/Prefab/WeaponLevelUp") as GameObject;
			GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
			WeaponLevelUpWindow component = gameObject.GetComponent<WeaponLevelUpWindow>();
			component.Init(mTargetWeapon, mOldLevel, mNewLevel);
			mParent.Close();
		}
		else
		{
			UIMsgBox.instance.ShowMithrilNotEnoughMessage(this);
		}
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (whichMsg.EventId == 3)
		{
			UIMsgBox.instance.CloseMessage();
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				IapScript.mInstance.OpenPage(IapScript.IapType.Mithril);
			}
		}
	}

	private void CheckPrice()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		if (userState.GetMithril() < mMithril)
		{
			Price.text = UIConstant.CURRENCY_COLOR_NOT_ENOUGH_PREFIX + mMithril + "[-]";
		}
		else
		{
			Price.text = UIConstant.CURRENCY_COLOR_MITHRIL_PREFIX + mMithril + "[-]";
		}
	}
}
