public class SwVoidFloatingState : SwVoidState
{
	public override void Start(SwVoid swVoid)
	{
		base.Start(swVoid);
		swVoid.StartIdle();
	}

	public override void Update(SwVoid swVoid, float deltaTime)
	{
		base.Update(swVoid, deltaTime);
		swVoid.DoIdleFloating(deltaTime);
	}
}
