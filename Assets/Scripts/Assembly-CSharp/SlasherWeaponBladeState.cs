public class SlasherWeaponBladeState : EnemyState
{
	public override void Start(Enemy enemy)
	{
		Slasher slasher = enemy as Slasher;
		if (slasher != null)
		{
			slasher.StartSlasherWeaponBlade();
		}
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		Slasher slasher = enemy as Slasher;
		if (slasher != null)
		{
			slasher.DoSlasherWeaponBlade();
		}
	}

	public override void End(Enemy enemy)
	{
		Slasher slasher = enemy as Slasher;
		if (slasher != null)
		{
			slasher.EndSlasherWeaponBlade();
		}
	}
}
