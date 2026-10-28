public class WeaponUpgrade
{
	protected int[] damage;

	protected int[] price;

	public void LoadConfig()
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.EQUIPMENT, DataSheetName.UPGRADES);
		damage = new int[gameDataTable.Count - 1];
		price = new int[gameDataTable.Count - 1];
		for (int i = 0; i < gameDataTable.Count - 1; i++)
		{
			price[i] = gameDataTable.GetInt(i, 1);
			damage[i] = gameDataTable.GetInt(i, 2);
		}
	}

	public int GetDamage(int level)
	{
		if (level < damage.Length)
		{
			return damage[level];
		}
		return 0;
	}

	public int GetPrice(int level)
	{
		if (level < price.Length)
		{
			return price[level];
		}
		return 0;
	}
}
