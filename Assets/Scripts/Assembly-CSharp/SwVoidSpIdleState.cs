using UnityEngine;

public class SwVoidSpIdleState : SwVoidState
{
	public override void Start(SwVoid swVoid)
	{
		base.Start(swVoid);
		swVoid.PlayAnimation(AnimationString.SWVOID_IDLE_SP, WrapMode.ClampForever);
	}

	public override void Update(SwVoid swVoid, float deltaTime)
	{
		base.Update(swVoid, deltaTime);
		swVoid.DoSpecialIdle();
	}

	public override void End(SwVoid swVoid)
	{
		base.End(swVoid);
	}
}
