using UnityEngine;

public class DeadEye4LaserState : EnemyState
{
	private DeadEye4LaserComponent componnet = new DeadEye4LaserComponent();

	public override void Start(Enemy enemy)
	{
		enemy.PlayAnimation(AnimationString.DEAD_EYE_4_RAY, WrapMode.ClampForever);
		enemy.LookAtTargetHorizontal();
		componnet.Start(enemy, this);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		DeadEye4LaserComponent.MyData myData = enemy.StateData[this] as DeadEye4LaserComponent.MyData;
		LookAtTargetPosition(enemy, DeadEyeParameter.DEAD_EYE_4_RAY_ROTATE_SPEED);
		componnet.Update(enemy, this);
	}

	public override void End(Enemy enemy)
	{
	}
}
