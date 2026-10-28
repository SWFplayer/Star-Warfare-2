using UnityEngine;

public class DeadEyeShoulderLaserState : EnemyState
{
	private DeadEyeShoulderLaserComponent component = new DeadEyeShoulderLaserComponent();

	public override void Start(Enemy enemy)
	{
		enemy.PlayAnimation(AnimationString.DEAD_EYE_SHOULDER_RAY, WrapMode.ClampForever);
		enemy.LookAtTargetHorizontal();
		component.Start(enemy, this);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		DeadEyeShoulderLaserComponent.MyData myData = enemy.StateData[this] as DeadEyeShoulderLaserComponent.MyData;
		LookAtTargetPosition(enemy, DeadEyeParameter.SHOULDER_RAY_ROTATE_SPEED);
		component.Update(enemy, this);
	}

	public override void End(Enemy enemy)
	{
		component.End(enemy, this);
	}
}
