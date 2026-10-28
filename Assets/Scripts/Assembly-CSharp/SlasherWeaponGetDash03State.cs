public class SlasherWeaponGetDash03State : EnemyState
{
	public override void Start(Enemy enemy)
	{
		Slasher slasher = enemy as Slasher;
		if (slasher != null)
		{
			slasher.StartSlasherWeaponGetDash03();
		}
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		Slasher slasher = enemy as Slasher;
		if (slasher != null)
		{
			slasher.DoSlasherWeaponGetDash03();
		}
	}

	public override void End(Enemy enemy)
	{
		Slasher slasher = enemy as Slasher;
		if (slasher != null)
		{
			slasher.EndSlasherWeaponGetDash03();
		}
	}
}
