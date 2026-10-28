using System.Runtime.InteropServices;

public class ExternScript
{
	[DllImport("__Internal")]
	protected static extern string _GetCFUUID();

	public static string GetCFUUID()
	{
		return string.Empty;
	}

	[DllImport("__Internal")]
	protected static extern string _GetMacAddress();

	public static string GetMacAddress()
	{
		return string.Empty;
	}

	[DllImport("__Internal")]
	protected static extern string _GetIDFA();

	public static string GetIDFA()
	{
		return string.Empty;
	}

	[DllImport("__Internal")]
	protected static extern int get_used_memory();

	public static int GetUsedMemory()
	{
		return 0;
	}

	[DllImport("__Internal")]
	protected static extern void remove_Notifications();

	public static void removeNotifications()
	{
	}

	[DllImport("__Internal")]
	protected static extern string getCurrentLanguage();

	public static string GetCurrentLanguage()
	{
		return AndroidPluginScript.GetCurrentLanguage();
	}
}
