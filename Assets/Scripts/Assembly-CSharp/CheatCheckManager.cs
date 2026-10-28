public class CheatCheckManager
{
	private static CheatCheckManager instance;

	public static CheatCheckManager sGetInstance()
	{
		if (instance == null)
		{
			instance = new CheatCheckManager();
		}
		return instance;
	}

	public void dataException()
	{
	}
}
