public class SlasherWeaponGetDash02State : EnemyState
{
	public override void Start(Enemy enemy)
	{
		Slasher slasher = enemy as Slasher;
		if (slasher != null)
		{
			slasher.StartSlasherWeaponGetDash02();
		}
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		Slasher slasher = enemy as Slasher;
		if (slasher != null)
		{
			slasher.DoSlasherWeaponGetDash02();
		}
	}

	public override void End(Enemy enemy)
	{
		Slasher slasher = enemy as Slasher;
		if (slasher != null)
		{
			slasher.EndSlasherWeaponGetDash02();
		}
	}
}
