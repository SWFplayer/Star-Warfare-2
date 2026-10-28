public class SwVoidAttackState : SwVoidState
{
	public override void Start(SwVoid swVoid)
	{
		base.Start(swVoid);
		((SwVoidSoldier)swVoid).StartAttack();
	}

	public override void Update(SwVoid swVoid, float deltaTime)
	{
		base.Update(swVoid, deltaTime);
		((SwVoidSoldier)swVoid).DoAttack(deltaTime);
	}

	public override void End(SwVoid swVoid)
	{
		base.End(swVoid);
		((SwVoidSoldier)swVoid).EndAttack();
	}
}
