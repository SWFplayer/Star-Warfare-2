public class SwVoidInvisibleState : SwVoidState
{
	public override void Start(SwVoid swVoid)
	{
		base.Start(swVoid);
		((SwVoidSpy)swVoid).StartInvisible();
	}

	public override void Update(SwVoid swVoid, float deltaTime)
	{
		base.Update(swVoid, deltaTime);
		((SwVoidSpy)swVoid).DoInvisible();
	}

	public override void End(SwVoid swVoid)
	{
		base.End(swVoid);
		((SwVoidSpy)swVoid).EndInvisible();
	}
}
