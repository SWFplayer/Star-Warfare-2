using System.Runtime.InteropServices;

public class IAP
{
	public static int sPurchaseStatus = -1;

	[DllImport("__Internal")]
	protected static extern void PurchaseProduct(string productId, string productCount);

	public static void NowPurchaseProduct(string productId, string productCount)
	{
		AndroidIAPPluginScript.CallPurchaseProduct(productId);
	}

	[DllImport("__Internal")]
	protected static extern int PurchaseStatus();

	public static int purchaseStatus(object stateInfo)
	{
		return AndroidIAPPluginScript.GetPurchaseStatus();
	}

	[DllImport("__Internal")]
	protected static extern int RestoreProduct();

	public static void NowRestoreProduct()
	{
		AndroidIAPPluginScript.GetRestorePurchse();
	}

	[DllImport("__Internal")]
	protected static extern int ProvideContent();

	public static void provideContent()
	{
		AndroidIAPPluginScript.ProvideContent();
	}

	[DllImport("__Internal")]
	protected static extern int UserCanceled();

	public static void userCanceled()
	{
		AndroidIAPPluginScript.UserCanceled();
	}

	[DllImport("__Internal")]
	protected static extern void MithrilRewardInitialized();

	public static void InitIAP()
	{
	}

	[DllImport("__Internal")]
	protected static extern string GetMithirlReward();

	public static string GetIAPMithril()
	{
		return string.Empty;
	}

	[DllImport("__Internal")]
	protected static extern void RegisterPurchase();

	public static void RegisterIAP()
	{
	}

	[DllImport("__Internal")]
	protected static extern void _AddPVPProductID(string productId);

	public static void AddPVPProductID(string productId)
	{
	}

	[DllImport("__Internal")]
	protected static extern void _AddMithrilProductID(string productId);

	public static void AddMithrilProductID(string productId)
	{
	}

	[DllImport("__Internal")]
	protected static extern void _AddPackageProductID(string productId);

	public static void AddPackageProductID(string productId)
	{
	}

	[DllImport("__Internal")]
	protected static extern void _GetProductInfo(byte type);

	public static void GetProductInfo(byte type)
	{
	}

	[DllImport("__Internal")]
	protected static extern void _AddSkipBackupAttributetoItemAtURL();

	public static void DisableICloud()
	{
	}
}
