using System;
using UnityEngine;

public class WeaponTrialScript : MonoBehaviour, UIMsgListener
{
	public enum TrialType
	{
		TrialUse = 0,
		TrialBuy = 1
	}

	public TrialType WeaponTrialType;

	public GameObject TryButton;

	public GameObject ExitButton;

	public UISprite WeaponIcon;

	public UISprite WeaponIcon_D;

	public UILabel WeaponLevel;

	public UISprite Currency;

	public UILabel Price;

	public UILabel SaleCountDown;

	public UIGrid CompareGrid;

	public PushCompareScript DPSCompare;

	public PushCompareScript FireRateCompare;

	public PushCompareScript SpeedCompare;

	private float mCheckPriceTime = 0.15f;

	private float mLastCheckPriceTime;

	private void Awake()
	{
		short num = -1;
		if (WeaponTrialType == TrialType.TrialUse)
		{
			num = GameApp.GetInstance().GetUserState().GetTrialWeaponID();
		}
		else if (WeaponTrialType == TrialType.TrialBuy)
		{
			num = GameApp.GetInstance().GetUserState().GetTrialWeaponBuyID();
		}
		if (num > 0)
		{
			Init(num);
		}
		else
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
		}
	}

	private void Update()
	{
		if (WeaponTrialType == TrialType.TrialBuy && SaleCountDown != null)
		{
			if (GameApp.GetInstance().GetUserState().GetTrialWeaponBuySeconds() > 0f)
			{
				SaleCountDown.text = GameApp.GetInstance().GetUserState().GetTrialWeaponBuySecondsString();
			}
			else
			{
				GameApp.GetInstance().GetUserState().CheckTrialWeaponBuySecondsOver();
				UnityEngine.Object.DestroyImmediate(base.gameObject);
			}
			if (Time.time - mLastCheckPriceTime >= mCheckPriceTime)
			{
				mLastCheckPriceTime = Time.time;
				CheckPrice();
			}
		}
	}

	private void Init(short weaponID)
	{
		WeaponIcon.spriteName = "weapons_" + weaponID;
		WeaponIcon_D.spriteName = "weapons_" + weaponID;
		if (WeaponTrialType == TrialType.TrialUse)
		{
			WeaponLevel.text = Global.TRIAL_WEAPON_LEVEL.ToString();
		}
		else if (WeaponTrialType == TrialType.TrialBuy)
		{
			WeaponLevel.text = "1";
			CheckPrice();
		}
		DPSCompare.Init(weaponID);
		FireRateCompare.Init(weaponID);
		SpeedCompare.Init(weaponID);
		CompareGrid.Reposition();
		UIEventListener uIEventListener = UIEventListener.Get(TryButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(ExitButton);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
	}

	private void OnButtonClick(GameObject go)
	{
		if (go == TryButton)
		{
			if (WeaponTrialType == TrialType.TrialUse)
			{
				GameApp.GetInstance().GetUserState().ConfirmTrialWeapon();
				MissionSelectUIScript.m_Instance.OKClicked();
			}
			else
			{
				if (WeaponTrialType != TrialType.TrialBuy)
				{
					return;
				}
				UserState userState = GameApp.GetInstance().GetUserState();
				Weapon weapon = userState.GetWeapons()[userState.GetTrialWeaponBuyID()];
				if (weapon.Level != 15)
				{
					return;
				}
				if (weapon.Mithril == 0)
				{
					if (userState.GetCash() >= weapon.Price)
					{
						AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_BUY);
						if (InterMissionScript.GetInstance() != null)
						{
							InterMissionScript.GetInstance().ConfirmEquipAnimation(true);
						}
						userState.Buy(weapon.Price);
						userState.ClearTrialWeaponBuyInfo();
						weapon.IrregularSetLevel(1, Global.TRIAL_UNLOCK_VERSION_STRING);
						bool flag = true;
						short[] battleWeaponIDs = userState.GetBattleWeaponIDs();
						for (int i = 0; i < battleWeaponIDs.Length; i++)
						{
							if (battleWeaponIDs[i] == 0)
							{
								if (i == 0)
								{
									flag = false;
									userState.SetBattleWeaponID(0, weapon.GetGunID());
								}
								else
								{
									userState.SetBattleWeaponID(i, battleWeaponIDs[0]);
								}
								break;
							}
						}
						if (flag)
						{
							userState.SetBattleWeaponID(0, weapon.GetGunID());
						}
						NotificationManager.GetInstance().SendLocalNotificationForEquip();
						UnityEngine.Object.DestroyImmediate(base.gameObject);
					}
					else
					{
						UIMsgBox.instance.ShowMoneyNotEnoughMessage(this);
					}
					return;
				}
				int mithril = weapon.Mithril;
				if (userState.GetMithril() >= mithril)
				{
					AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_BUY);
					if (InterMissionScript.GetInstance() != null)
					{
						InterMissionScript.GetInstance().ConfirmEquipAnimation(true);
					}
					userState.BuyWithMithril(weapon.Mithril);
					userState.ClearTrialWeaponBuyInfo();
					weapon.IrregularSetLevel(1, Global.TRIAL_UNLOCK_VERSION_STRING);
					bool flag2 = true;
					short[] battleWeaponIDs2 = userState.GetBattleWeaponIDs();
					for (int j = 0; j < battleWeaponIDs2.Length; j++)
					{
						if (battleWeaponIDs2[j] == 0)
						{
							if (j == 0)
							{
								flag2 = false;
								userState.SetBattleWeaponID(0, weapon.GetGunID());
							}
							else
							{
								userState.SetBattleWeaponID(j, battleWeaponIDs2[0]);
							}
							break;
						}
					}
					if (flag2)
					{
						userState.SetBattleWeaponID(0, weapon.GetGunID());
					}
					NotificationManager.GetInstance().SendLocalNotificationForEquip();
					UnityEngine.Object.DestroyImmediate(base.gameObject);
				}
				else
				{
					UIMsgBox.instance.ShowMithrilNotEnoughMessage(this);
				}
			}
		}
		else if (go == ExitButton)
		{
			if (WeaponTrialType == TrialType.TrialUse)
			{
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_PROMO_DESC_10"), 2, 42);
			}
			else if (WeaponTrialType == TrialType.TrialBuy)
			{
				UnityEngine.Object.DestroyImmediate(base.gameObject);
			}
		}
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (whichMsg.EventId == 42)
		{
			UIMsgBox.instance.CloseMessage();
			switch (buttonId)
			{
			case UIMsg.UIMsgButton.Ok:
				GameApp.GetInstance().GetUserState().ClearTrialWeapon();
				MissionSelectUIScript.m_Instance.OKClicked();
				break;
			}
		}
		else if (whichMsg.EventId == 3)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				UIMsgBox.instance.CloseMessage();
				IapScript.mInstance.OpenPage(IapScript.IapType.Mithril);
			}
			else
			{
				UIMsgBox.instance.CloseMessage();
			}
		}
		else if (whichMsg.EventId == 4)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				UIMsgBox.instance.CloseMessage();
				IapScript.mInstance.OpenPage(IapScript.IapType.Gold);
			}
			else
			{
				UIMsgBox.instance.CloseMessage();
			}
		}
	}

	private void CheckPrice()
	{
		if (!(Currency != null) || !(Price != null))
		{
			return;
		}
		UserState userState = GameApp.GetInstance().GetUserState();
		Weapon weapon = userState.GetWeapons()[userState.GetTrialWeaponBuyID()];
		if (weapon.Mithril == 0)
		{
			Currency.spriteName = "icon_dollar1";
			string text = weapon.Price.ToString();
			if (userState.GetCash() < weapon.Price)
			{
				Price.text = UIConstant.CURRENCY_COLOR_NOT_ENOUGH_PREFIX + text + "[-]";
			}
			else
			{
				Price.text = UIConstant.CURRENCY_COLOR_GOLD_PREFIX + text + "[-]";
			}
		}
		else
		{
			Currency.spriteName = "icon_mithril1";
			string text2 = weapon.Mithril.ToString();
			if (userState.GetMithril() < weapon.Mithril)
			{
				Price.text = UIConstant.CURRENCY_COLOR_NOT_ENOUGH_PREFIX + text2 + "[-]";
			}
			else
			{
				Price.text = UIConstant.CURRENCY_COLOR_MITHRIL_PREFIX + text2 + "[-]";
			}
		}
	}
}
