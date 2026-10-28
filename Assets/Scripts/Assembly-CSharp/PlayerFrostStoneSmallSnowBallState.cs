using UnityEngine;

public class PlayerFrostStoneSmallSnowBallState : PlayerBossState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public Vector3[] Positions;

		public Timer Timer = new Timer();
	}

	public const float SnowBallDistance = 40f;

	public GameUnitStateComponent component = new GameUnitStateComponent();

	public override void Start(GameUnit gameUnit)
	{
		MyData data = GetData<MyData>(gameUnit);
		data.InnerState = 0;
		data.Timer.SetTimer(0.1f, false);
		Player player = gameUnit as Player;
		player.PlayAnimation("snowball", WrapMode.ClampForever);
		player.GetAnimationObject().GetComponent<Animation>()["snowball"].speed = FrostStoneParameter.FROST_STONE_PLAYER_BOSS_ANIMATION_SPEED[2];
		if (player.IsLocal() && GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			component.UpdateTargetPositionOnGround(gameUnit, 40f, true, false);
			data.Positions = GetPositions(player.GetPosition(), player.GetTargetPosition());
			SendPositions(player);
		}
		Pool.sAllocate("SW2_Effect/B3_Snowball01_smoke", player.GetPosition(), 4f);
	}

	public override void Update(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		if (player.IsAnimationTriggered(0.35f))
		{
			player.PlaySound(AudioName.FROSTSTONE_SNOWBALL);
		}
		MyData myData = player.StateData[this] as MyData;
		if (player.IsLocal() && player.CurrentAnimTime < FrostStoneParameter.SMALL_SNOW_BALL_TRIGGER_TIME && myData.Timer.Ready() && GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			myData.Timer.Do();
			component.UpdateTargetPositionOnGround(gameUnit, 40f, true, false);
			myData.Positions = GetPositions(player.GetPosition(), player.GetTargetPosition());
			SendPositions(player);
		}
		if (player.IsAnimationTriggered(FrostStoneParameter.SMALL_SNOW_BALL_TRIGGER_TIME))
		{
			if (player.IsLocal())
			{
				component.UpdateTargetPositionOnGround(gameUnit, 40f, true, false);
				myData.Positions = GetPositions(player.GetPosition(), player.GetTargetPosition());
			}
			Vector3[] positions = myData.Positions;
			foreach (Vector3 vector in positions)
			{
				Vector3 vector2 = player.GetPosition() + player.GetTransform().forward * 4f;
				Vector3 vector3 = vector - vector2;
				vector3.y = 0f;
				float num = vector3.magnitude / FrostStoneParameter.SMALL_SNOW_BALL_HORIZONTAL_SPEED;
				Vector3 speed = vector3.normalized * FrostStoneParameter.SMALL_SNOW_BALL_HORIZONTAL_SPEED;
				speed.y += FrostStoneParameter.SMALL_SNOW_BALL_HEIGHT / num;
				float num2 = 2f * speed.y / num;
				GameObject gameObject = Pool.sAllocate("SW2_Effect/B3_Snowball01_ball");
				gameObject.transform.position = vector2;
				ParaHitCheckScript paraHitCheckScript = gameObject.GetComponent<ParaHitCheckScript>();
				if (paraHitCheckScript == null)
				{
					paraHitCheckScript = gameObject.AddComponent<ParaHitCheckScript>();
				}
				paraHitCheckScript.Speed = speed;
				paraHitCheckScript.Gravity = 0f - num2;
				paraHitCheckScript.SetDamageInfo(FrostStoneParameter.GetInstance(), 3);
				paraHitCheckScript.Vfx = "SW2_Effect/B3_Snowball01_hit";
				paraHitCheckScript.VfxSound = AudioName.FROSTSTONE_SNOWBALL;
				paraHitCheckScript.VfxDuration = 2f;
				paraHitCheckScript.Attacker = gameUnit;
				paraHitCheckScript.Reset();
				paraHitCheckScript.SetRadius(1.2f);
			}
		}
		StartIdleWhenAnimFinish(player);
	}

	public override void End(GameUnit gameUnit)
	{
	}

	public override void DoResponse(GameUnit gameUnit, BytesBuffer buffer)
	{
		MyData data = GetData<MyData>(gameUnit);
		data.Positions = new Vector3[FrostStoneParameter.SMALL_SNOW_BALL_NUM];
		for (int i = 0; i < data.Positions.Length; i++)
		{
			data.Positions[i] = buffer.ReadVector3();
		}
	}

	private void SendPositions(Player player)
	{
		MyData data = GetData<MyData>(player);
		PlayerBossStateDataRequest playerBossStateDataRequest = new PlayerBossStateDataRequest(player.GetChannelID(), ID, data.Positions.Length * 3 * 4);
		for (int i = 0; i < data.Positions.Length; i++)
		{
			playerBossStateDataRequest.Data.AddVector3(data.Positions[i]);
		}
		GameApp.GetInstance().GetNetworkManager().SendRequest(playerBossStateDataRequest);
	}

	private Vector3[] GetPositions(Vector3 thisPos, Vector3 targetPos)
	{
		Vector3[] array = new Vector3[FrostStoneParameter.SMALL_SNOW_BALL_NUM];
		Vector3 normalized = (targetPos - thisPos).normalized;
		for (int i = 0; i < FrostStoneParameter.SMALL_SNOW_BALL_NUM - 1; i++)
		{
			int num = ((i % 2 == 0) ? 1 : (-1));
			array[i] = targetPos + Quaternion.Euler(0f, Random.Range((float)num * 45f, (float)num * 135f), 0f) * normalized * Random.Range(FrostStoneParameter.SAMLL_SNOW_BALL_RADIUS.x, FrostStoneParameter.SAMLL_SNOW_BALL_RADIUS.y);
		}
		array[array.Length - 1] = targetPos;
		return array;
	}
}
