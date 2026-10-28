using UnityEngine;

public class ConditionCatchState : EnemyState
{
	public override void Start(Enemy enemy)
	{
		enemy.StartCatching();
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		enemy.PlayAnimation(enemy.RunAnimationName, WrapMode.Loop);
		enemy.UpdateNavMeshInCatching(false);
		if (enemy.IsMasterPlayer)
		{
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && GameApp.GetInstance().GetGameMode().IsCoopMode())
			{
				if (enemy.GetUpdatePosTimeDuration() > enemy.GetUpdatePosInterval())
				{
					enemy.SetUpdatePosTimeNow();
					enemy.SendStateChangeRequest(enemy.mConditionState);
				}
				else
				{
					enemy.CheckTargetInCatching();
				}
			}
			else
			{
				enemy.CheckTargetInCatching();
			}
		}
		else
		{
			enemy.CheckTargetInCatching();
		}
	}

	public override void End(Enemy enemy)
	{
		enemy.EndCatching();
	}
}
