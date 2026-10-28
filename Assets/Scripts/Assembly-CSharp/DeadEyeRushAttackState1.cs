using UnityEngine;

public class DeadEyeRushAttackState1 : EnemyState
{
	public override void Start(Enemy enemy)
	{
		enemy.PlayAnimation(AnimationString.BOSS_DASH01, WrapMode.ClampForever);
		enemy.LookAtTargetHorizontal();
		GameObject original = Resources.Load("SW2_Effect/B2_Dash01_smoke") as GameObject;
		Object.Instantiate(original, enemy.GetPosition(), Quaternion.identity);
	}
}
