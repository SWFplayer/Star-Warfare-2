using UnityEngine;

public class DeadEyeRoofMoveBackState : EnemyState
{
	public override void Start(Enemy enemy)
	{
		enemy.PlayAnimation(AnimationString.DEAD_EYE_ROOF_RUN, WrapMode.Loop);
		Vector3 vector = enemy.GetPosition() - enemy.GetTargetPosition();
		vector.y = 0f;
		Vector3 position = enemy.GetPosition() + vector * DeadEyeParameter.ROOF_RUN_BACK_TIME * DeadEyeParameter.ROOF_RUN_BACK_TIME;
		enemy.MoveToward(position, DeadEyeParameter.ROOF_RUN_BACK_TIME);
		enemy.SetCanTurn(true);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		if (enemy.IsMasterPlayer && GameApp.GetInstance().GetGameMode().IsMultiPlayer() && GameApp.GetInstance().GetGameMode().IsCoopMode() && enemy.GetUpdatePosTimeDuration() > enemy.GetUpdatePosInterval())
		{
			enemy.SetUpdatePosTimeNow();
			enemy.SendStateChangeRequest(enemy.mConditionState);
		}
	}

	public override void End(Enemy enemy)
	{
		enemy.StopNavMesh();
		enemy.PlaySound(AudioName.DEADEYE_ROOF_DOWN);
	}
}
