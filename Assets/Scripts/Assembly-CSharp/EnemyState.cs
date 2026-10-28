using UnityEngine;

public abstract class EnemyState : GameUnitState
{
	public virtual void Start(Enemy enemy)
	{
	}

	public virtual void Update(Enemy enemy, float deltaTime)
	{
	}

	public virtual void End(Enemy enemy)
	{
	}

	public virtual void OnHit(Enemy enemy)
	{
		enemy.SetState(Enemy.GOTHIT_STATE);
	}

	public void LookAtTargetPosition(Enemy enemy, float radianSpeed)
	{
		if (!enemy.IsStateRotateFinish)
		{
			enemy.IsStateRotateFinish = enemy.IsReachLookAtAngle();
			return;
		}
		enemy.SetCanTurn(false);
		Vector3 target = enemy.GetTargetPosition() - enemy.GetPosition();
		target = Vector3.RotateTowards(enemy.GetTransform().forward, target, radianSpeed * Time.deltaTime, 10000f);
		target.y = 0f;
		enemy.GetTransform().LookAt(enemy.GetPosition() + target);
	}

	public Vector3 ChooseDashOverPoint(EnemyBoss enemy, sDashAttack dashParam, int innerState, float wallDistance)
	{
		Vector3 vector = enemy.GetTargetPosition() - enemy.GetPosition();
		float magnitude = vector.magnitude;
		vector = vector.normalized;
		Ray ray = new Ray(enemy.GetPosition() + new Vector3(0f, 0.5f, 0f), vector);
		int layerMask = 1 << PhysicsLayer.WALL_AGAINST_PALYER;
		RaycastHit hitInfo;
		magnitude = ((!Physics.Raycast(ray, out hitInfo, magnitude + dashParam.DASH_OVER_DISTANCE + wallDistance, layerMask)) ? (magnitude + dashParam.DASH_OVER_DISTANCE) : ((enemy.GetPosition() + new Vector3(0f, 0.5f, 0f) - hitInfo.point).magnitude - wallDistance));
		Vector3 vector2 = enemy.GetPosition() + vector * magnitude;
		float speed = dashParam.DASH_SPEED1[enemy.GetMindStateInt()];
		enemy.MoveToward(vector2, speed);
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && enemy.IsMasterPlayer)
		{
			EnemyStateDataRequest enemyStateDataRequest = new EnemyStateDataRequest(enemy.EnemyID, enemy.GetConditionStateId(this), 13);
			enemyStateDataRequest.Data.AddByte((byte)innerState);
			enemyStateDataRequest.Data.AddVector3(vector2);
			GameApp.GetInstance().GetNetworkManager().SendRequest(enemyStateDataRequest);
		}
		return vector2;
	}
}
