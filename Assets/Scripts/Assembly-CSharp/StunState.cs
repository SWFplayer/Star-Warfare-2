using UnityEngine;

public class StunState : EnemyState
{
	public float mStunStartTime;

	public override void Start(Enemy enemy)
	{
		enemy.EnemyGotHitAnimation();
		mStunStartTime = Time.time;
		EffectPool.GetInstance().GetEffectPool(EffectPoolType.STUN_EFFECT).CreateObject(enemy.GetPosition(), Vector3.zero, Quaternion.identity);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		if (Time.time - mStunStartTime >= 1f)
		{
			enemy.MoveToward(enemy.GetPosition(), 0f);
		}
		if (Time.time - mStunStartTime >= 10f)
		{
			enemy.SetState(Enemy.IDLE_STATE);
		}
	}

	public override void End(Enemy enemy)
	{
		enemy.StopNavMesh();
	}
}
