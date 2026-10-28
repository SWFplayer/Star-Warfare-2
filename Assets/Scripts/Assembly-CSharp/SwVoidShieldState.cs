public class SwVoidShieldState : SwVoidState
{
	public override void Start(SwVoid swVoid)
	{
		base.Start(swVoid);
		((SwVoidHeavy)swVoid).StartShield();
	}

	public override void Update(SwVoid swVoid, float deltaTime)
	{
		base.Update(swVoid, deltaTime);
		((SwVoidHeavy)swVoid).DoShield();
	}

	public override void End(SwVoid swVoid)
	{
		base.End(swVoid);
		((SwVoidHeavy)swVoid).EndShield();
	}
}
