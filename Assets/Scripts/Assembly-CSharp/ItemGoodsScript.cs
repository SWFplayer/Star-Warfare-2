using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemGoodsScript : MonoBehaviour
{
	public UISprite GoodsIcon;

	public UILabel GoodsAmountLabel;

	public GameObject BuyButton;

	public UISprite GoodsCurrencyIcon;

	public UILabel GoodsPriceLabel;

	public UILabel DecriptionLabel;

	public GameObject Arrow;

	public UISprite GoodsSelectBackground;

	[HideInInspector]
	public int mAmount;

	[HideInInspector]
	public float mPrice;

	private int mItemId;

	private int mShopItemId;

	private List<UIWidget> mWidgets = new List<UIWidget>();

	private void Awake()
	{
		mWidgets.Clear();
		UIWidget[] componentsInChildren = GetComponentsInChildren<UIWidget>();
		foreach (UIWidget item in componentsInChildren)
		{
			mWidgets.Add(item);
		}
		UIEventListener uIEventListener = UIEventListener.Get(BuyButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClicked));
	}

	public void Init(int index)
	{
		ShopItem shopItem = new ShopItem();
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.EQUIPMENT, DataSheetName.SHOP_ITEMS);
		if (gameDataTable != null)
		{
			for (int i = 0; i < gameDataTable.Count - 1; i++)
			{
				shopItem.LoadConfig(i);
				if (shopItem.GetItemID() == index)
				{
					mShopItemId = i;
					break;
				}
			}
		}
		mItemId = index;
		mAmount = shopItem.GetCount();
		if (shopItem.GetMithril() > 0)
		{
			mPrice = shopItem.GetMithril();
			GoodsCurrencyIcon.spriteName = "mithril1";
		}
		else
		{
			mPrice = shopItem.GetGold();
			GoodsCurrencyIcon.spriteName = "dollar1";
		}
		GoodsIcon.spriteName = shopItem.GetIconName();
		GoodsAmountLabel.text = "x" + mAmount;
		int count = GameApp.GetInstance().GetUserState().GetItems()[shopItem.GetItemID()].GetCount();
		if (count + shopItem.GetCount() < Global.ITEM_MAX_COUNT)
		{
			GoodsPriceLabel.text = mPrice.ToString();
		}
		else
		{
			GoodsPriceLabel.text = LocalizationManager.GetInstance().GetString("LOC_BUTTON_EQUIP");
		}
		DecriptionLabel.text = shopItem.GetDescription();
		Select();
	}

	private void OnButtonClicked(GameObject go)
	{
		if (!(go == BuyButton))
		{
			return;
		}
		UserState userState = GameApp.GetInstance().GetUserState();
		ShopItem shopItem = ShopItemManager.GetInstance().GetShopItemList()[mShopItemId];
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
					userState.AnalysisManager.GetItemBuyNum()[mShopItemId]++;
					if (GameApp.GetInstance().IsConnectedToServer())
					{
						SyncCashRequest request = new SyncCashRequest();
						GameApp.GetInstance().GetNetworkManager().SendRequest(request);
					}
					item.AddCount(shopItem.GetCount());
					bool flag = false;
					int num = -1;
					for (int i = 0; i < 3; i++)
					{
						if (userState.GetBattleItemIDs()[i] == 0 || userState.GetItems()[userState.GetBattleItemIDs()[i]].GetCount() == 0)
						{
							num = i;
						}
						if (userState.GetBattleItemIDs()[i] == item.GetItemID())
						{
							flag = true;
						}
					}
					if (!flag && num != -1 && item.GetItemType() == Item.ItemType.Potion)
					{
						userState.GetBattleItemIDs()[num] = (short)item.GetItemID();
					}
					ItemPushScript.mInstance.ShowEffect(base.transform.position.x);
					GoodsPriceLabel.text = LocalizationManager.GetInstance().GetString("LOC_BUTTON_EQUIPED");
					BuyButton.GetComponent<Collider>().enabled = false;
				}
				else
				{
					UIMsgBox.instance.ShowMoneyNotEnoughMessage(ItemPushScript.mInstance);
				}
				return;
			}
			int mithril = shopItem.GetMithril();
			if (userState.GetMithril() >= mithril)
			{
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_BUY);
				if (InterMissionScript.GetInstance() != null)
				{
					InterMissionScript.GetInstance().ConfirmEquipAnimation(false);
				}
				userState.BuyWithMithril(mithril);
				userState.AnalysisManager.GetItemBuyNum()[mShopItemId]++;
				if (GameApp.GetInstance().IsConnectedToServer())
				{
					SyncMithrilRequest request2 = new SyncMithrilRequest();
					GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
				}
				item.AddCount(shopItem.GetCount());
				bool flag2 = false;
				int num2 = -1;
				for (int j = 0; j < 3; j++)
				{
					if (userState.GetBattleItemIDs()[j] == 0 || userState.GetItems()[userState.GetBattleItemIDs()[j]].GetCount() == 0)
					{
						num2 = j;
					}
					if (userState.GetBattleItemIDs()[j] == item.GetItemID())
					{
						flag2 = true;
					}
				}
				if (!flag2 && num2 != -1 && item.GetItemType() == Item.ItemType.Potion)
				{
					userState.GetBattleItemIDs()[num2] = (short)item.GetItemID();
				}
				ItemPushScript.mInstance.ShowEffect(base.transform.position.x);
				GoodsPriceLabel.text = LocalizationManager.GetInstance().GetString("LOC_BUTTON_EQUIPED");
				BuyButton.GetComponent<Collider>().enabled = false;
			}
			else
			{
				UIMsgBox.instance.ShowMithrilNotEnoughMessage(ItemPushScript.mInstance);
			}
			return;
		}
		GoodsPriceLabel.text = LocalizationManager.GetInstance().GetString("LOC_BUTTON_EQUIPED");
		BuyButton.GetComponent<Collider>().enabled = false;
		bool flag3 = false;
		int num3 = -1;
		for (int k = 0; k < 3; k++)
		{
			if (userState.GetBattleItemIDs()[k] == 0)
			{
				num3 = k;
			}
			if (userState.GetBattleItemIDs()[k] == item.GetItemID())
			{
				flag3 = true;
			}
		}
		if (!flag3 && num3 != -1 && item.GetItemType() == Item.ItemType.Potion)
		{
			userState.GetBattleItemIDs()[num3] = (short)item.GetItemID();
		}
		ItemPushScript.mInstance.ShowEffect(base.transform.position.x);
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

	public void SetAlpha(float alpha)
	{
		foreach (UIWidget mWidget in mWidgets)
		{
			mWidget.alpha = alpha;
		}
	}

	private void OnPress(bool isPress)
	{
		if (isPress)
		{
			ItemPushScript.mInstance.SetNeedUpdate();
		}
	}
}
