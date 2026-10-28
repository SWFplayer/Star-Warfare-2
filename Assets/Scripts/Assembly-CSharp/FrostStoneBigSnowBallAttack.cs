using UnityEngine;

public class FrostStoneBigSnowBallAttack : EnemyState
{
	public override void Start(Enemy enemy)
	{
		enemy.PlayAnimation("snowball2", WrapMode.ClampForever);
		enemy.LookAtTargetHorizontal();
		Pool.sAllocate("SW2_Effect/B3_Snowball02_smoke", enemy.GetPosition(), 4f);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		if (enemy.IsAnimationTriggered(0.26f))
		{
			enemy.PlaySound(AudioName.FROSTSTONE_SNOWBALL2);
		}
		if (enemy.CurrentAnimTime < FrostStoneParameter.BIG_SNOW_BALL_TRIGGER_TIME)
		{
			LookAtTargetPosition(enemy, FrostStoneParameter.BIG_SNOW_BALL_ROTATE_SPEED);
		}
		if (enemy.IsAnimationTriggered(FrostStoneParameter.BIG_SNOW_BALL_TRIGGER_TIME))
		{
			Vector3 vector = enemy.GetPosition() + enemy.GetTransform().forward * 4f;
			Vector3 vector2 = enemy.GetTargetPosition() - vector;
			vector2.y = 0f;
			Vector3 speed = vector2 / FrostStoneParameter.BIG_SNOW_BALL_TIME;
			float num = FrostStoneParameter.BIG_SNOW_BALL_HEIGHT * 2f / (FrostStoneParameter.BIG_SNOW_BALL_TIME * FrostStoneParameter.BIG_SNOW_BALL_TIME);
			speed += new Vector3(0f, num * FrostStoneParameter.BIG_SNOW_BALL_TIME, 0f) / 2f;
			GameObject gameObject = Pool.sAllocate("SW2_Effect/B3_Snowball02_ball");
			gameObject.transform.position = vector;
			FrostStoneBigSnowBallScript frostStoneBigSnowBallScript = gameObject.GetComponent<FrostStoneBigSnowBallScript>();
			if (frostStoneBigSnowBallScript == null)
			{
				frostStoneBigSnowBallScript = gameObject.AddComponent<FrostStoneBigSnowBallScript>();
			}
			frostStoneBigSnowBallScript.Speed = speed;
			frostStoneBigSnowBallScript.Gravity = 0f - num;
			frostStoneBigSnowBallScript.SetDamageInfo(FrostStoneParameter.GetInstance(), 4);
			frostStoneBigSnowBallScript.ExplodeRadius = FrostStoneParameter.BIG_SNOW_BALL_EXPLODE_RADIUS;
			frostStoneBigSnowBallScript.Vfx = "SW2_Effect/B3_Snowball02_hit";
			frostStoneBigSnowBallScript.VfxSound = AudioName.FROSTSTONE_SNOWBALL2_END;
			frostStoneBigSnowBallScript.VfxDuration = 2f;
			frostStoneBigSnowBallScript.Attacker = enemy;
			frostStoneBigSnowBallScript.Reset();
		}
	}
}
