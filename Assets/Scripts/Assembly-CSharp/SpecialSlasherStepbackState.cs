public class SpecialSlasherStepbackState : EnemyState
{
	public override void Start(Enemy enemy)
	{
		SpecialSlasher specialSlasher = enemy as SpecialSlasher;
		if (specialSlasher != null)
		{
			specialSlasher.StartSpecialSlasherStepback();
		}
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		SpecialSlasher specialSlasher = enemy as SpecialSlasher;
		if (specialSlasher != null)
		{
			specialSlasher.DoSpecialSlasherStepback();
		}
	}

	public override void End(Enemy enemy)
	{
		SpecialSlasher specialSlasher = enemy as SpecialSlasher;
		if (specialSlasher != null)
		{
			specialSlasher.EndSpecialSlasherStepback();
		}
	}
}
