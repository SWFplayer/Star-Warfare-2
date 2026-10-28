using UnityEngine;

public class DeadEyeRoofDeadState : EnemyState
{
	public override void Start(Enemy enemy)
	{
		enemy.mDeadTimer.Do();
		enemy.GetObject().layer = PhysicsLayer.DEFAULT;
		GameObject original = Resources.Load("_Old/_Effect/Dragon/dragon_dead_blood") as GameObject;
		Object.Instantiate(original, enemy.bipObject.transform.position, Quaternion.identity);
		enemy.PlayAnimation(AnimationString.DEAD_EYE_ROOF_DEAD, WrapMode.ClampForever);
		if (GameApp.GetInstance().GetGameMode().IsSingle())
		{
			enemy.AddCashAndExpToPlayer();
		}
		enemy.PlaySound(AudioName.DEADEYE_DEAD);
		GameApp.GetInstance().GetUserState().Achievement.CheckAchievement_RoofKill();
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		enemy.DoDead();
	}
}
