public class IapPackageData : IapData
{
	public IapScript.IapPackageType PackageType;

	public int HeadID;

	public int ChestID;

	public int HandID;

	public int FootID;

	public int BagID1;

	public int BagID2;

	public int GunID1;

	public int GunID2;

	public int GunID3;

	public int ITEMID1;

	public int ITEMID2;

	public int ITEMID3;

	public int COLOR;

	public bool IsMaterialChest()
	{
		if (PackageType == IapScript.IapPackageType.CopperChest || PackageType == IapScript.IapPackageType.SilverChest || PackageType == IapScript.IapPackageType.GoldChest)
		{
			return true;
		}
		return false;
	}
}
