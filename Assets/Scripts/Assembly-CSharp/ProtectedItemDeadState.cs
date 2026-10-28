public class ProtectedItemDeadState : ProtectedItemState
{
	public override void Update(ProtectedItem item, float deltaTime)
	{
		item.DoDead();
	}
}
