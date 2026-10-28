using System.Collections.Generic;

public class Shop
{
	protected List<IAPItem> itemList = new List<IAPItem>();

	public void CreateIAPShopData()
	{
		IAPItem iAPItem = new IAPItem();
		iAPItem.ID = "com.ifreyr.starwarfare.rookie099";
		iAPItem.iType = IAPType.RookiePackage;
		iAPItem.Name = IAPName.ROOKIE;
		iAPItem.Desc = "ROOKIE PACK";
		AddIAPItem(iAPItem);
		IAPItem iAPItem2 = new IAPItem();
		iAPItem2.ID = "com.ifreyr.starwarfare.sergeant299";
		iAPItem2.iType = IAPType.Sergeant;
		iAPItem2.Name = IAPName.SERGEANT;
		iAPItem2.Desc = "SERGEANT";
		AddIAPItem(iAPItem2);
		IAPItem iAPItem3 = new IAPItem();
		iAPItem3.ID = "com.ifreyr.starwarfare.10m099cents01";
		iAPItem3.iType = IAPType.Mithril;
		iAPItem3.Name = IAPName.M10;
		iAPItem3.Desc = "Mithril 10";
		AddIAPItem(iAPItem3);
		IAPItem iAPItem4 = new IAPItem();
		iAPItem4.ID = "com.ifreyr.starwarfare.24m199cents";
		iAPItem4.iType = IAPType.Mithril;
		iAPItem4.Name = IAPName.M24;
		iAPItem4.Desc = "Mithril 24";
		AddIAPItem(iAPItem4);
		IAPItem iAPItem5 = new IAPItem();
		iAPItem5.ID = "com.ifreyr.starwarfare.72m499cents";
		iAPItem5.iType = IAPType.Mithril;
		iAPItem5.Name = IAPName.M72;
		iAPItem5.Desc = "Mithril 72";
		AddIAPItem(iAPItem5);
		IAPItem iAPItem6 = new IAPItem();
		iAPItem6.ID = "com.ifreyr.starwarfare.168m999cents";
		iAPItem6.iType = IAPType.Mithril;
		iAPItem6.Name = IAPName.M168;
		iAPItem6.Desc = "Mithril 168";
		AddIAPItem(iAPItem6);
		IAPItem iAPItem7 = new IAPItem();
		iAPItem7.ID = "com.ifreyr.starwarfare.666m2999cents";
		iAPItem7.iType = IAPType.Mithril;
		iAPItem7.Name = IAPName.M666;
		iAPItem7.Desc = "Mithril 666";
		AddIAPItem(iAPItem7);
		IAPItem iAPItem8 = new IAPItem();
		iAPItem8.ID = "com.ifreyr.starwarfare.1430m4999cents";
		iAPItem8.iType = IAPType.Mithril;
		iAPItem8.Name = IAPName.M1430;
		iAPItem8.Desc = "Mithril 1430";
		AddIAPItem(iAPItem8);
		IAPItem iAPItem9 = new IAPItem();
		iAPItem9.ID = "com.ifreyr.starwarfare.4000m9999cents";
		iAPItem9.iType = IAPType.Mithril;
		iAPItem9.Name = IAPName.M4000;
		iAPItem9.Desc = "Mithril 4000";
		AddIAPItem(iAPItem9);
	}

	public void AddIAPItem(IAPItem item)
	{
		itemList.Add(item);
	}

	public List<IAPItem> GetIAPList()
	{
		return itemList;
	}
}
