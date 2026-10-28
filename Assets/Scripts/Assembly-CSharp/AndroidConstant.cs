public class AndroidConstant
{
	public enum Version
	{
		GooglePlay = 0,
		Kindle = 1
	}

	public static Version version = (Version)AndroidPluginScript.GetVersionType();

	public static bool isFacebook()
	{
		if (version == Version.GooglePlay)
		{
			return true;
		}
		return false;
	}
}
