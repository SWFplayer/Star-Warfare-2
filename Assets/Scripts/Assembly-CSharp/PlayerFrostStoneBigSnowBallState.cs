using UnityEngine;

public class PlayerFrostStoneBigSnowBallState : PlayerBossState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;
	}

	public const float SnowBallDistance = 40f;

	public GameUnitStateComponent component = new GameUnitStateComponent();

	public override void Start(GameUnit gameUnit)
	{
		MyData data = GetData<MyData>(gameUnit);
		data.InnerState = 0;
		Player player = gameUnit as Player;
		player.PlayAnimation("snowball2", WrapMode.ClampForever);
		player.GetAnimationObject().GetComponent<Animation>()["snowball2"].speed = FrostStoneParameter.FROST_STONE_PLAYER_BOSS_ANIMATION_SPEED[3];
		Pool.sAllocate("SW2_Effect/B3_Snowball02_smoke", gameUnit.GetPosition(), 4f);
		component.UpdateTargetPositionOnGround(gameUnit, 40f, true);
	}

	public override void Update(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		StartIdleWhenAnimFinish(player);
		RestrictRotationSpeed(player, FrostStoneParameter.BIG_SNOW_BALL_ROTATE_SPEED);
		component.UpdateTargetPositionOnGround(gameUnit, 40f, false);
		if (player.IsAnimationTriggered(FrostStoneParameter.BIG_SNOW_BALL_TRIGGER_TIME))
		{
			component.UpdateTargetPositionOnGround(gameUnit, 40f, true);
			Vector3 vector = player.GetPosition() + player.GetTransform().forward * 4f;
			Vector3 vector2 = player.GetTargetPosition() - vector;
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
			frostStoneBigSnowBallScript.Reset();
			frostStoneBigSnowBallScript.SetRadius(2.6f);
		}
	}

	public override void End(GameUnit gameUnit)
	{
	}

	public override void DoResponse(GameUnit gameUnit, BytesBuffer buffer)
	{
	}
}
