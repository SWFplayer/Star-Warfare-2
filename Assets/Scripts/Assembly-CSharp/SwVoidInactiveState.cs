public class SwVoidInactiveState : SwVoidState
{
	public override void Start(SwVoid swVoid)
	{
		swVoid.StartInactive();
	}

	public override void Update(SwVoid swVoid, float deltaTime)
	{
		swVoid.DoInactive();
	}

	public override void End(SwVoid swVoid)
	{
		swVoid.EndInactive();
	}
}
