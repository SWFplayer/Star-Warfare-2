using UnityEngine;

public class DeadEyeRoofRockAttackState : EnemyState
{
	public override void Start(Enemy enemy)
	{
		enemy.PlayAnimation(AnimationString.DEAD_EYE_ROOF_ROCK_ATTTACK, WrapMode.ClampForever);
		enemy.LookAtTargetHorizontal();
	}
}
