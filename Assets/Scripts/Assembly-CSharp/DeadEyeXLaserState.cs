using UnityEngine;

public class DeadEyeXLaserState : EnemyState
{
	private DeadEyeXLaserComponent component = new DeadEyeXLaserComponent();

	public override void Start(Enemy enemy)
	{
		enemy.PlayAnimation(AnimationString.DEAD_EYE_X_RAY, WrapMode.ClampForever);
		enemy.LookAtTargetHorizontal();
		component.Start(enemy, this);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		enemy.LookAtTargetHorizontal();
		component.Update(enemy, this);
	}

	public override void End(Enemy enemy)
	{
		component.End(enemy, this);
	}
}
