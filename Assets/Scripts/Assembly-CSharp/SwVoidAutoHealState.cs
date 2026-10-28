public class SwVoidAutoHealState : SwVoidState
{
	public override void Start(SwVoid swVoid)
	{
		((SwVoidMedic)swVoid).StartAutoHeal();
	}

	public override void Update(SwVoid swVoid, float deltaTime)
	{
		((SwVoidMedic)swVoid).DoAutoHeal(deltaTime);
	}

	public override void End(SwVoid swVoid)
	{
		((SwVoidMedic)swVoid).EndAutoHeal();
	}
}
