public class SwVoidHealState : SwVoidState
{
	public override void Start(SwVoid swVoid)
	{
		base.Start(swVoid);
		((SwVoidMedic)swVoid).StartHeal();
	}

	public override void Update(SwVoid swVoid, float deltaTime)
	{
		base.Update(swVoid, deltaTime);
		((SwVoidMedic)swVoid).DoHeal();
	}

	public override void End(SwVoid swVoid)
	{
		base.End(swVoid);
		((SwVoidMedic)swVoid).EndHeal();
	}
}
