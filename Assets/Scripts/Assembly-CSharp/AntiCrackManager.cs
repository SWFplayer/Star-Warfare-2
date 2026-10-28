public class AntiCrackManager
{
	private int Gold;

	private int Mithril;

	private static AntiCrackManager mInstance;

	public static AntiCrackManager GetInstance()
	{
		if (mInstance == null)
		{
			mInstance = new AntiCrackManager();
		}
		return mInstance;
	}

	public void SetGold(int value)
	{
		Gold = value;
	}

	public void SetMithril(int value)
	{
		Mithril = value;
	}

	public int GetGold()
	{
		return Gold;
	}

	public int GetMithril()
	{
		return Mithril;
	}
}
