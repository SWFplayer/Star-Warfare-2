using UnityEngine;

public class SwVoidAutoPickState : SwVoidState
{
	public override void Start(SwVoid swVoid)
	{
		swVoid.StartAutoPickup();
	}

	public override void Update(SwVoid swVoid, float deltaTime)
	{
		swVoid.DoAutoPickup();
	}

	public override void End(SwVoid swVoid)
	{
		swVoid.endPickupTime = Time.time;
	}
}
