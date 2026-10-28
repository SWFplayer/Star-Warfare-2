public class SlasherWeaponGetState : EnemyState
{
	public override void Start(Enemy enemy)
	{
		Slasher slasher = enemy as Slasher;
		if (slasher != null)
		{
			slasher.StartSlasherWeaponGet();
		}
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		Slasher slasher = enemy as Slasher;
		if (slasher != null)
		{
			slasher.DoSlasherWeaponGet();
		}
	}

	public override void End(Enemy enemy)
	{
		Slasher slasher = enemy as Slasher;
		if (slasher != null)
		{
			slasher.EndSlasherWeaponGet();
		}
	}
}
