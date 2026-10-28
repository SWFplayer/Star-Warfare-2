using UnityEngine;

public class DeadEyeRoofCatchState : EnemyState
{
	public override void Start(Enemy enemy)
	{
		enemy.PlayAnimation(AnimationString.DEAD_EYE_ROOF_RUN, WrapMode.Loop);
		enemy.StartCatching();
		enemy.UpdateNavMeshInDash(true, DeadEyeParameter.ROOF_RUN_SPEED);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		enemy.UpdateNavMeshInDash(false, DeadEyeParameter.ROOF_RUN_SPEED);
		if (enemy.IsMasterPlayer && GameApp.GetInstance().GetGameMode().IsMultiPlayer() && GameApp.GetInstance().GetGameMode().IsCoopMode() && enemy.GetUpdatePosTimeDuration() > enemy.GetUpdatePosInterval())
		{
			enemy.SetUpdatePosTimeNow();
			enemy.SendStateChangeRequest(enemy.mConditionState);
		}
	}

	public override void End(Enemy enemy)
	{
		enemy.StopNavMesh();
	}
}
