using System.Collections.Generic;

public class ShopItemManager
{
	private static ShopItemManager mInstance;

	private List<ShopItem> mShopItemList = new List<ShopItem>();

	private ShopItemManager()
	{
		LoadConfig();
	}

	public static ShopItemManager GetInstance()
	{
		if (mInstance == null)
		{
			mInstance = new ShopItemManager();
		}
		return mInstance;
	}

	private void LoadConfig()
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.EQUIPMENT, DataSheetName.SHOP_ITEMS);
		if (gameDataTable != null)
		{
			for (int i = 0; i < gameDataTable.Count - 1; i++)
			{
				ShopItem shopItem = new ShopItem();
				shopItem.LoadConfig(i);
				mShopItemList.Add(shopItem);
			}
		}
	}

	public List<ShopItem> GetShopItemList()
	{
		return mShopItemList;
	}
}
