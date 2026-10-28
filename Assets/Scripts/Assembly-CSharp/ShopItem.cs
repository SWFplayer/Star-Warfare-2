public class ShopItem
{
	public enum EShopItemColumn
	{
		ItemAnalysisID = 0,
		ItemName = 1,
		ItemID = 2,
		ItemCount = 3,
		ItemPrice = 4,
		ItemMithril = 5,
		ItemIcon = 6,
		ItemDescription = 7,
		ItemBuffDesc = 8
	}

	protected string mName;

	protected int mItemID;

	protected int mCount;

	protected string _mGold;

	protected string _mMithril;

	protected string mDescription;

	protected string mIconName;

	protected int mGold
	{
		get
		{
			return AntiCracking.DecryptBufferStr(_mGold, "shuliang");
		}
		set
		{
			_mGold = AntiCracking.CryptBufferStr(value, "shuliang");
		}
	}

	protected int mMithril
	{
		get
		{
			return AntiCracking.DecryptBufferStr(_mMithril, "shuliang");
		}
		set
		{
			_mMithril = AntiCracking.CryptBufferStr(value, "shuliang");
		}
	}

	public void LoadConfig(int row)
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.EQUIPMENT, DataSheetName.SHOP_ITEMS);
		if (gameDataTable != null)
		{
			mName = gameDataTable.GetString(row, 1);
			mItemID = gameDataTable.GetInt(row, 2);
			mCount = gameDataTable.GetInt(row, 3);
			mGold = gameDataTable.GetInt(row, 4);
			mMithril = gameDataTable.GetInt(row, 5);
			mIconName = gameDataTable.GetString(row, 6);
			mDescription = gameDataTable.GetString(row, 7);
		}
	}

	public string GetName()
	{
		return mName;
	}

	public int GetItemID()
	{
		return mItemID;
	}

	public int GetCount()
	{
		return mCount;
	}

	public int GetGold()
	{
		return mGold;
	}

	public int GetMithril()
	{
		return mMithril;
	}

	public string GetIconName()
	{
		return mIconName;
	}

	public string GetDescription()
	{
		return LocalizationManager.GetInstance().GetString(mDescription);
	}
}
