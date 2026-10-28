public class SwVoidDashState : SwVoidState
{
	public override void Start(SwVoid swVoid)
	{
		base.Start(swVoid);
		((SwVoidScout)swVoid).StartDash();
	}

	public override void Update(SwVoid swVoid, float deltaTime)
	{
		base.Update(swVoid, deltaTime);
		((SwVoidScout)swVoid).DoDash();
	}

	public override void End(SwVoid swVoid)
	{
		base.End(swVoid);
		((SwVoidScout)swVoid).EndDash();
	}
}
