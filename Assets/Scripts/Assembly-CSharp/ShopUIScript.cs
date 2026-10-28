using System;
using System.Collections;
using UnityEngine;

public class ShopUIScript : MonoBehaviour, UIMsgListener
{
	private static ShopUIScript me;

	public int CurrentEquipTag;

	public int CurrentEquipID;

	public ScrollEquipsScript ItemList;

	public ShopDescriptionScript Description;

	public ItemFiltersScript ArmorFilter;

	public ItemFiltersScript WeaponFilter;

	public UIButton ArmorPage;

	public UIButton WeaponPage;

	public UIButton BagPage;

	public UIButton ItemPage;

	public GameObject BuyEquip_Button;

	public UILabel BuyEquip_Label;

	public TweenPosition RefreshPageEffect;

	public GameObject ChangeEquipPageButton;

	public GameObject ChangeWeaponPageButton;

	public GameObject ChangeBagPageButton;

	public GameObject ChangeItemPageButton;

	public GameObject ShowAllWeaponButton;

	public GameObject ShowBulletWeaponButton;

	public GameObject ShowEnergyWeaponButton;

	public GameObject ShowExplosionWeaponButton;

	public GameObject ShowMeleeWeaponButton;

	public UISprite[] ChangePageEffectLines;

	public UISprite RankIcon;

	public SuitStateScript SuitState;

	private int PageEffectLineIndex = -1;

	private PlayerDamageType mShowWeaponType;

	public GameObject m_Effect;

	public static bool IsGoingToExpItemPage;

	public PlayerDamageType ShowWeaponType
	{
		get
		{
			return mShowWeaponType;
		}
	}

	public static ShopUIScript GetInstance()
	{
		return me;
	}

	private void Start()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		userState.CheckTrialWeaponBuySecondsOver();
		UIEventListener uIEventListener = UIEventListener.Get(BuyEquip_Button);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(ChangeEquipPageButton);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(ChangeWeaponPageButton);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener4 = UIEventListener.Get(ChangeBagPageButton);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener5 = UIEventListener.Get(ChangeItemPageButton);
		uIEventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener5.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener6 = UIEventListener.Get(ShowAllWeaponButton);
		uIEventListener6.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener6.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener7 = UIEventListener.Get(ShowBulletWeaponButton);
		uIEventListener7.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener7.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener8 = UIEventListener.Get(ShowEnergyWeaponButton);
		uIEventListener8.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener8.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener9 = UIEventListener.Get(ShowExplosionWeaponButton);
		uIEventListener9.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener9.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener10 = UIEventListener.Get(ShowMeleeWeaponButton);
		uIEventListener10.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener10.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		if (IsGoingToExpItemPage)
		{
			ItemList.StartPosition = new Vector3(-(ItemList.ItemSpace * 9), 0f, 0f);
			ChangePage(6);
			ClearPageEffectLine();
			PageEffectLineIndex = -1;
			WeaponFilter.gameObject.SetActive(false);
			ItemList.StartPosition = new Vector3(204f, 0f, 0f);
			IsGoingToExpItemPage = false;
		}
		else
		{
			ChangePage(5);
			ClearPageEffectLine();
			PageEffectLineIndex = 1;
		}
		RankIcon.spriteName = "rank_" + (userState.GetRank(userState.GetExp()).rankID + 1);
		me = this;
	}

	public IEnumerator ChangeToExpItemPage()
	{
		yield return 1;
		ItemList.StartPosition = new Vector3(-(ItemList.ItemSpace * 9), 0f, 0f);
		if (CurrentEquipTag != 6)
		{
			ChangePage(6);
		}
		else
		{
			ItemList.ShowEquips(6);
		}
		ClearPageEffectLine();
		PageEffectLineIndex = -1;
		ItemList.StartPosition = new Vector3(204f, 0f, 0f);
		yield return 0;
	}

	private void OnDestroy()
	{
		me = null;
	}

	public void ChangePage(int equipTag)
	{
		if (equipTag == CurrentEquipTag)
		{
			return;
		}
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_SELECT);
		if (CurrentEquipTag < 4)
		{
			switch (CurrentEquipTag)
			{
			case 0:
				ArmorFilter.Filter1.tweenTarget.GetComponent<UISprite>().spriteName = "sheiled_btn01_n";
				ArmorFilter.Filter1.transform.Find("Highlight").gameObject.SetActive(false);
				break;
			case 1:
				ArmorFilter.Filter2.tweenTarget.GetComponent<UISprite>().spriteName = "sheiled_btn02_n";
				ArmorFilter.Filter2.transform.Find("Highlight").gameObject.SetActive(false);
				break;
			case 2:
				ArmorFilter.Filter3.tweenTarget.GetComponent<UISprite>().spriteName = "sheiled_btn03_n";
				ArmorFilter.Filter3.transform.Find("Highlight").gameObject.SetActive(false);
				break;
			case 3:
				ArmorFilter.Filter4.tweenTarget.GetComponent<UISprite>().spriteName = "sheiled_btn04_n";
				ArmorFilter.Filter4.transform.Find("Highlight").gameObject.SetActive(false);
				break;
			}
			if (equipTag >= 4)
			{
				ArmorFilter.gameObject.SetActive(false);
				ArmorPage.tweenTarget.GetComponent<UISprite>().spriteName = "btn01_n";
			}
		}
		else if (CurrentEquipTag == 4)
		{
			BagPage.tweenTarget.GetComponent<UISprite>().spriteName = "btn01_n";
			if (InterMissionScript.GetInstance() != null)
			{
				InterMissionScript.GetInstance().ShowAvatarBag(false);
			}
		}
		else if (CurrentEquipTag == 5)
		{
			WeaponFilter.gameObject.SetActive(false);
			WeaponPage.tweenTarget.GetComponent<UISprite>().spriteName = "btn01_n";
		}
		else if (CurrentEquipTag == 6)
		{
			ItemPage.tweenTarget.GetComponent<UISprite>().spriteName = "btn01_n";
		}
		CurrentEquipTag = equipTag;
		ItemList.ShowEquips(equipTag);
		if (equipTag < 4)
		{
			ArmorFilter.gameObject.SetActive(true);
			ArmorPage.tweenTarget.GetComponent<UISprite>().spriteName = "btn01_u";
			switch (equipTag)
			{
			case 0:
				ArmorFilter.Filter1.tweenTarget.GetComponent<UISprite>().spriteName = "sheiled_btn01_u";
				ArmorFilter.Filter1.transform.Find("Highlight").gameObject.SetActive(true);
				break;
			case 1:
				ArmorFilter.Filter2.tweenTarget.GetComponent<UISprite>().spriteName = "sheiled_btn02_u";
				ArmorFilter.Filter2.transform.Find("Highlight").gameObject.SetActive(true);
				break;
			case 2:
				ArmorFilter.Filter3.tweenTarget.GetComponent<UISprite>().spriteName = "sheiled_btn03_u";
				ArmorFilter.Filter3.transform.Find("Highlight").gameObject.SetActive(true);
				break;
			case 3:
				ArmorFilter.Filter4.tweenTarget.GetComponent<UISprite>().spriteName = "sheiled_btn04_u";
				ArmorFilter.Filter4.transform.Find("Highlight").gameObject.SetActive(true);
				break;
			}
			SuitState.gameObject.SetActive(true);
			SuitState.SetHighlight(equipTag);
		}
		else
		{
			switch (equipTag)
			{
			case 4:
				BagPage.tweenTarget.GetComponent<UISprite>().spriteName = "btn01_u";
				if (InterMissionScript.GetInstance() != null)
				{
					InterMissionScript.GetInstance().ShowAvatarBag(true);
				}
				SuitState.gameObject.SetActive(false);
				break;
			case 5:
				WeaponFilter.gameObject.SetActive(true);
				WeaponPage.tweenTarget.GetComponent<UISprite>().spriteName = "btn01_u";
				SuitState.gameObject.SetActive(false);
				break;
			case 6:
				ItemPage.tweenTarget.GetComponent<UISprite>().spriteName = "btn01_u";
				SuitState.gameObject.SetActive(false);
				break;
			}
		}
		RefreshPageEffect.Reset();
		RefreshPageEffect.PlayForward();
	}

	public void PurchaseEquip()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		byte rankID = userState.GetRank().rankID;
		if (CurrentEquipTag < 5)
		{
			Armor armor = userState.GetArmor(CurrentEquipTag, CurrentEquipID);
			if (armor.Level == 15)
			{
				if (armor.Mithril == 0)
				{
					if (userState.GetCash() >= armor.Price)
					{
						AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_BUY);
						if (InterMissionScript.GetInstance() != null)
						{
							InterMissionScript.GetInstance().ConfirmEquipAnimation(false);
						}
						userState.Buy(armor.Price);
						armor.Level = 1;
						userState.SetAvatar((BodyType)CurrentEquipTag, CurrentEquipID);
						NotificationManager.GetInstance().SendLocalNotificationForEquip();
						if (CustomizeUIScript.mInstance != null)
						{
							CustomizeUIScript.mInstance.Refresh();
							CustomizeUIScript.mInstance.SelectItemPage.InitEquipSel();
						}
						ItemList.BuyOver();
						if (LobbyScript.mInstance != null && rankID < armor.UnlockPVPLevel)
						{
							LobbyScript.mInstance.pvpExpIap.gameObject.SetActive(true);
							LobbyScript.mInstance.pvpExpIap.InitInfo("LOC_PVP_IAP_HINT1");
						}
					}
					else
					{
						UIMsgBox.instance.ShowMoneyNotEnoughMessage(this);
					}
				}
				else
				{
					int mithril = armor.Mithril;
					if (userState.GetMithril() >= mithril)
					{
						AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_BUY);
						if (InterMissionScript.GetInstance() != null)
						{
							InterMissionScript.GetInstance().ConfirmEquipAnimation(false);
						}
						userState.BuyWithMithril(mithril);
						armor.Level = 1;
						userState.SetAvatar((BodyType)CurrentEquipTag, CurrentEquipID);
						NotificationManager.GetInstance().SendLocalNotificationForEquip();
						if (CustomizeUIScript.mInstance != null)
						{
							CustomizeUIScript.mInstance.Refresh();
							CustomizeUIScript.mInstance.SelectItemPage.InitEquipSel();
						}
						ItemList.BuyOver();
						if (LobbyScript.mInstance != null && rankID < armor.UnlockPVPLevel)
						{
							LobbyScript.mInstance.pvpExpIap.gameObject.SetActive(true);
							LobbyScript.mInstance.pvpExpIap.InitInfo("LOC_PVP_IAP_HINT1");
						}
					}
					else
					{
						UIMsgBox.instance.ShowMithrilNotEnoughMessage(this);
					}
				}
			}
			else if (armor.Level != 0)
			{
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_EQUIPPED);
				if (InterMissionScript.GetInstance() != null)
				{
					InterMissionScript.GetInstance().ConfirmEquipAnimation(false);
				}
				if (CurrentEquipTag == 4)
				{
					int num = userState.GetAvatar()[4];
				}
				userState.SetAvatar((BodyType)CurrentEquipTag, CurrentEquipID);
				if (CustomizeUIScript.mInstance != null)
				{
					CustomizeUIScript.mInstance.Refresh();
					CustomizeUIScript.mInstance.SelectItemPage.InitEquipSel();
				}
			}
			else if (armor.Level == 0)
			{
				LockMsg(armor.UnlockLevel.Difficult, armor.UnlockLevel.StageID);
			}
		}
		else if (CurrentEquipTag == 5)
		{
			Weapon weapon = userState.GetWeapons()[CurrentEquipID];
			if (weapon.Level == 15)
			{
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
						if (userState.GetTrialWeaponBuyID() == weapon.GunID)
						{
							userState.ClearTrialWeaponBuyInfo();
							weapon.IrregularSetLevel(1, Global.TRIAL_UNLOCK_VERSION_STRING);
						}
						else
						{
							weapon.Level = 1;
						}
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
						ItemList.BuyOver();
						if (LobbyScript.mInstance != null && rankID < weapon.UnlockPVPLevel)
						{
							LobbyScript.mInstance.pvpExpIap.gameObject.SetActive(true);
							LobbyScript.mInstance.pvpExpIap.InitInfo("LOC_PVP_IAP_HINT1");
						}
					}
					else
					{
						UIMsgBox.instance.ShowMoneyNotEnoughMessage(this);
					}
				}
				else
				{
					int mithril2 = weapon.Mithril;
					if (userState.GetMithril() >= mithril2)
					{
						AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_BUY);
						if (InterMissionScript.GetInstance() != null)
						{
							InterMissionScript.GetInstance().ConfirmEquipAnimation(true);
						}
						userState.BuyWithMithril(weapon.Mithril);
						if (userState.GetTrialWeaponBuyID() == weapon.GunID)
						{
							userState.ClearTrialWeaponBuyInfo();
							weapon.IrregularSetLevel(1, Global.TRIAL_UNLOCK_VERSION_STRING);
						}
						else
						{
							weapon.Level = 1;
						}
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
						ItemList.BuyOver();
						if (LobbyScript.mInstance != null && rankID < weapon.UnlockPVPLevel)
						{
							LobbyScript.mInstance.pvpExpIap.gameObject.SetActive(true);
							LobbyScript.mInstance.pvpExpIap.InitInfo("LOC_PVP_IAP_HINT1");
						}
					}
					else
					{
						UIMsgBox.instance.ShowMithrilNotEnoughMessage(this);
					}
				}
			}
			else if (weapon.Level != 0)
			{
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_EQUIPPED);
				if (InterMissionScript.GetInstance() != null)
				{
					InterMissionScript.GetInstance().ConfirmEquipAnimation(true);
				}
				int gunID = weapon.GetGunID();
				int id = userState.GetBattleWeaponIDs()[0];
				for (int k = 0; k < userState.GetBattleWeaponIDs().Length; k++)
				{
					if (gunID == userState.GetBattleWeaponIDs()[k])
					{
						userState.SetBattleWeaponID(k, id);
						break;
					}
				}
				userState.SetBattleWeaponID(0, gunID);
				if (CustomizeUIScript.mInstance != null)
				{
					CustomizeUIScript.mInstance.Refresh();
					CustomizeUIScript.mInstance.SelectItemPage.InitEquipSel();
				}
			}
			else if (weapon.Level == 0)
			{
				LockMsg(weapon.UnlockLevel.Difficult, weapon.UnlockLevel.StageID);
			}
		}
		else if (CurrentEquipTag == 6)
		{
			ShopItem shopItem = ShopItemManager.GetInstance().GetShopItemList()[CurrentEquipID];
			Item item = userState.GetItems()[shopItem.GetItemID()];
			if (item.GetCount() + shopItem.GetCount() < Global.ITEM_MAX_COUNT)
			{
				if (shopItem.GetMithril() == 0)
				{
					if (userState.GetCash() >= shopItem.GetGold())
					{
						AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_BUY);
						if (InterMissionScript.GetInstance() != null)
						{
							InterMissionScript.GetInstance().ConfirmEquipAnimation(false);
						}
						userState.Buy(shopItem.GetGold());
						userState.AnalysisManager.GetItemBuyNum()[CurrentEquipID]++;
						if (GameApp.GetInstance().IsConnectedToServer())
						{
							SyncCashRequest request = new SyncCashRequest();
							GameApp.GetInstance().GetNetworkManager().SendRequest(request);
						}
						item.AddCount(shopItem.GetCount());
					}
					else
					{
						UIMsgBox.instance.ShowMoneyNotEnoughMessage(this);
					}
				}
				else
				{
					int mithril3 = shopItem.GetMithril();
					if (userState.GetMithril() >= mithril3)
					{
						AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_BUY);
						if (InterMissionScript.GetInstance() != null)
						{
							InterMissionScript.GetInstance().ConfirmEquipAnimation(false);
						}
						userState.BuyWithMithril(mithril3);
						userState.AnalysisManager.GetItemBuyNum()[CurrentEquipID]++;
						if (GameApp.GetInstance().IsConnectedToServer())
						{
							SyncMithrilRequest request2 = new SyncMithrilRequest();
							GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
						}
						item.AddCount(shopItem.GetCount());
					}
					else
					{
						UIMsgBox.instance.ShowMithrilNotEnoughMessage(this);
					}
				}
			}
		}
		Description.UpdateDescription();
		userState.Achievement.CheckAchievement_GoodEquipment();
	}

	public void OnButtonClick(GameObject go)
	{
		if (go == BuyEquip_Button)
		{
			m_Effect.SetActive(false);
			m_Effect.SetActive(true);
			PurchaseEquip();
		}
		else if (go == ChangeEquipPageButton)
		{
			ClearPageEffectLine();
			PageEffectLineIndex = 0;
		}
		else if (go == ChangeWeaponPageButton)
		{
			UnlightWeaponFilters();
			CurrentEquipTag = -1;
			mShowWeaponType = PlayerDamageType.NORMAL;
			ClearPageEffectLine();
			PageEffectLineIndex = 1;
			ChangePage(5);
			WeaponFilter.Filter1.tweenTarget.GetComponent<UISprite>().spriteName = "weapon_btn04_u";
			WeaponFilter.Filter1.transform.Find("Highlight").gameObject.SetActive(true);
		}
		else if (go == ChangeBagPageButton)
		{
			ClearPageEffectLine();
			PageEffectLineIndex = -1;
		}
		else if (go == ChangeItemPageButton)
		{
			ClearPageEffectLine();
			PageEffectLineIndex = -1;
		}
		else if (go == ShowAllWeaponButton)
		{
			UnlightWeaponFilters();
			mShowWeaponType = PlayerDamageType.NORMAL;
			ItemList.ShowEquips(5);
			WeaponFilter.Filter1.tweenTarget.GetComponent<UISprite>().spriteName = "weapon_btn04_u";
			WeaponFilter.Filter1.transform.Find("Highlight").gameObject.SetActive(true);
			RefreshPageEffect.Reset();
			RefreshPageEffect.PlayForward();
			ItemList.gameObject.GetComponent<UICenterOnChild>().Recenter();
		}
		else if (go == ShowBulletWeaponButton)
		{
			UnlightWeaponFilters();
			mShowWeaponType = PlayerDamageType.BULLET;
			ItemList.ShowEquips(5);
			WeaponFilter.Filter2.tweenTarget.GetComponent<UISprite>().spriteName = "weapon_btn01_u";
			WeaponFilter.Filter2.transform.Find("Highlight").gameObject.SetActive(true);
			RefreshPageEffect.Reset();
			RefreshPageEffect.PlayForward();
			ItemList.gameObject.GetComponent<UICenterOnChild>().Recenter();
		}
		else if (go == ShowEnergyWeaponButton)
		{
			UnlightWeaponFilters();
			mShowWeaponType = PlayerDamageType.ENERGY;
			ItemList.ShowEquips(5);
			WeaponFilter.Filter3.tweenTarget.GetComponent<UISprite>().spriteName = "weapon_btn02_u";
			WeaponFilter.Filter3.transform.Find("Highlight").gameObject.SetActive(true);
			RefreshPageEffect.Reset();
			RefreshPageEffect.PlayForward();
			ItemList.gameObject.GetComponent<UICenterOnChild>().Recenter();
		}
		else if (go == ShowExplosionWeaponButton)
		{
			UnlightWeaponFilters();
			mShowWeaponType = PlayerDamageType.EXPLOSION;
			ItemList.ShowEquips(5);
			WeaponFilter.Filter4.tweenTarget.GetComponent<UISprite>().spriteName = "weapon_btn03_u";
			WeaponFilter.Filter4.transform.Find("Highlight").gameObject.SetActive(true);
			RefreshPageEffect.Reset();
			RefreshPageEffect.PlayForward();
			ItemList.gameObject.GetComponent<UICenterOnChild>().Recenter();
		}
		else if (go == ShowMeleeWeaponButton)
		{
			UnlightWeaponFilters();
			mShowWeaponType = PlayerDamageType.MELEE;
			ItemList.ShowEquips(5);
			WeaponFilter.Filter5.tweenTarget.GetComponent<UISprite>().spriteName = "weapon_btn05_u";
			WeaponFilter.Filter5.transform.Find("Highlight").gameObject.SetActive(true);
			RefreshPageEffect.Reset();
			RefreshPageEffect.PlayForward();
			ItemList.gameObject.GetComponent<UICenterOnChild>().Recenter();
		}
	}

	private void Update()
	{
		if (PageEffectLineIndex == -1 || PageEffectLineIndex >= ChangePageEffectLines.Length)
		{
			return;
		}
		if (ChangePageEffectLines[PageEffectLineIndex].fillAmount < 1f)
		{
			ChangePageEffectLines[PageEffectLineIndex].fillAmount += RealTime.deltaTime * 1.5f;
			return;
		}
		if (PageEffectLineIndex == 0)
		{
			TweenScale component = ArmorFilter.Filter1.GetComponent<TweenScale>();
			component.Reset();
			component.PlayForward();
		}
		else if (PageEffectLineIndex == 1)
		{
			TweenScale component2 = WeaponFilter.Filter1.GetComponent<TweenScale>();
			component2.Reset();
			component2.PlayForward();
		}
		PageEffectLineIndex = -1;
	}

	public void ClearPageEffectLine()
	{
		UISprite[] changePageEffectLines = ChangePageEffectLines;
		foreach (UISprite uISprite in changePageEffectLines)
		{
			uISprite.fillAmount = 0f;
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
		else if (whichMsg.EventId == 26)
		{
			UIMsgBox.instance.CloseMessage();
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				IapScript.mInstance.OpenPage(IapScript.IapType.Nothing);
				IapScript.mInstance.IsGotoUnlockIAP = true;
				IapScript.mInstance.OpenPage(IapScript.IapType.Package);
			}
		}
		else if (buttonId == UIMsg.UIMsgButton.Ok)
		{
			UIMsgBox.instance.CloseMessage();
		}
	}

	public void ReturnToShopPreview()
	{
		ItemList.RefreshAvatar();
	}

	private void UnlightWeaponFilters()
	{
		WeaponFilter.Filter1.tweenTarget.GetComponent<UISprite>().spriteName = "weapon_btn04_n";
		WeaponFilter.Filter1.transform.Find("Highlight").gameObject.SetActive(false);
		WeaponFilter.Filter2.tweenTarget.GetComponent<UISprite>().spriteName = "weapon_btn01_n";
		WeaponFilter.Filter2.transform.Find("Highlight").gameObject.SetActive(false);
		WeaponFilter.Filter3.tweenTarget.GetComponent<UISprite>().spriteName = "weapon_btn02_n";
		WeaponFilter.Filter3.transform.Find("Highlight").gameObject.SetActive(false);
		WeaponFilter.Filter4.tweenTarget.GetComponent<UISprite>().spriteName = "weapon_btn03_n";
		WeaponFilter.Filter4.transform.Find("Highlight").gameObject.SetActive(false);
		WeaponFilter.Filter5.tweenTarget.GetComponent<UISprite>().spriteName = "weapon_btn05_n";
		WeaponFilter.Filter5.transform.Find("Highlight").gameObject.SetActive(false);
	}

	private void LockMsg(EDifficultMode diffMode, int unlockStage)
	{
		string str = string.Empty;
		switch (diffMode)
		{
		case EDifficultMode.NORMAL:
			str = LocalizationManager.GetInstance().GetString("LOC_BUTTON_MODE_1");
			break;
		case EDifficultMode.HEROIC:
			str = LocalizationManager.GetInstance().GetString("LOC_BUTTON_MODE_2");
			break;
		}
		string info = LocalizationManager.GetInstance().GetString("LOC_ICON_27", unlockStage + 1, str) + "\n" + LocalizationManager.GetInstance().GetString("LOC_UNLOCK_LEVEL");
		UIMsgBox.instance.ShowMessage(this, info, 2, 26);
	}

	public void RefreshLockIcon()
	{
		ItemList.ShowEquips(CurrentEquipTag);
	}
}
