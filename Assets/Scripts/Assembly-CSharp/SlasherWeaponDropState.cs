public class SlasherWeaponDropState : EnemyState
{
	public override void Start(Enemy enemy)
	{
		Slasher slasher = enemy as Slasher;
		if (slasher != null)
		{
			slasher.StartSlasherWeaponDrop();
		}
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		Slasher slasher = enemy as Slasher;
		if (slasher != null)
		{
			slasher.DoSlasherWeaponDrop();
		}
	}

	public override void End(Enemy enemy)
	{
		Slasher slasher = enemy as Slasher;
		if (slasher != null)
		{
			slasher.EndSlasherWeaponDrop();
		}
	}
}
