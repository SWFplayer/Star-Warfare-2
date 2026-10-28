using UnityEngine;

public class BlackHoleState : EnemyState
{
	public float mBlackHoleStartTime;

	public override void Start(Enemy enemy)
	{
		if (AttractiveForce.mInstance != null)
		{
			enemy.MoveToward(AttractiveForce.mInstance.transform.position, AttractiveForce.mInstance.enemySpeed);
		}
		mBlackHoleStartTime = Time.time;
		enemy.SetCanTurn(true);
		if (enemy.GetTarget() != null)
		{
			enemy.LookAtPointHorizontal(enemy.GetTarget().GetPosition());
		}
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		if (Time.time - mBlackHoleStartTime >= SwVoidParameter.EXPLODE_TIME)
		{
			enemy.SetState(Enemy.IDLE_STATE);
		}
	}

	public override void End(Enemy enemy)
	{
		enemy.StopNavMesh();
	}
}
