public class ProtectedItemIdleState : ProtectedItemState
{
	public override void Update(ProtectedItem item, float deltaTime)
	{
		if (item.Hp <= 0)
		{
			item.StartDead();
		}
		else
		{
			item.DoIdle();
		}
	}
}
