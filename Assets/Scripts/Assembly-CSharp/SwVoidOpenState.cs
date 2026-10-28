public class SwVoidOpenState : SwVoidState
{
	public override void Start(SwVoid swVoid)
	{
		swVoid.StartOpen();
	}

	public override void Update(SwVoid swVoid, float deltaTime)
	{
		swVoid.DoOpen();
	}
}
