public class SwVoidExplodeState : SwVoidState
{
	public override void Start(SwVoid swVoid)
	{
		base.Start(swVoid);
		((SwVoidDemoman)swVoid).StartExplode();
	}

	public override void Update(SwVoid swVoid, float deltaTime)
	{
		base.Update(swVoid, deltaTime);
		((SwVoidDemoman)swVoid).DoExplode();
	}

	public override void End(SwVoid swVoid)
	{
		base.End(swVoid);
		((SwVoidDemoman)swVoid).EndExplode();
	}
}
