using UnityEngine;

public class FrostStoneSmallSnowBallAttackState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public Vector3[] Positions;

		public Timer Timer = new Timer();
	}

	public override void Start(Enemy enemy)
	{
		if (!enemy.StateData.ContainsKey(this))
		{
			enemy.StateData.Add(this, new MyData());
		}
		MyData myData = enemy.StateData[this] as MyData;
		myData.InnerState = 0;
		myData.Timer.SetTimer(0.1f, false);
		enemy.PlayAnimation("snowball", WrapMode.ClampForever);
		if (enemy.IsMasterPlayer && GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			myData.Positions = GetPositions(enemy.GetPosition(), enemy.GetTargetPosition());
			SendPositions(enemy);
		}
		enemy.LookAtTargetHorizontal();
		Pool.sAllocate("SW2_Effect/B3_Snowball01_smoke", enemy.GetPosition(), 4f);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		if (enemy.IsAnimationTriggered(0.35f))
		{
			enemy.PlaySound(AudioName.FROSTSTONE_SNOWBALL);
		}
		MyData myData = enemy.StateData[this] as MyData;
		if (enemy.IsMasterPlayer && enemy.CurrentAnimTime < FrostStoneParameter.SMALL_SNOW_BALL_TRIGGER_TIME && myData.Timer.Ready() && GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			myData.Timer.Do();
			myData.Positions = GetPositions(enemy.GetPosition(), enemy.GetTargetPosition());
			SendPositions(enemy);
		}
		if (!enemy.IsAnimationTriggered(FrostStoneParameter.SMALL_SNOW_BALL_TRIGGER_TIME))
		{
			return;
		}
		if (enemy.IsMasterPlayer)
		{
			myData.Positions = GetPositions(enemy.GetPosition(), enemy.GetTargetPosition());
		}
		Vector3[] positions = myData.Positions;
		foreach (Vector3 vector in positions)
		{
			Vector3 vector2 = enemy.GetPosition() + enemy.GetTransform().forward * 4f;
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
			paraHitCheckScript.Attacker = enemy;
			paraHitCheckScript.Reset();
		}
	}

	public override void End(Enemy enemy)
	{
	}

	private void SendPositions(Enemy enemy)
	{
		MyData data = GetData<MyData>(enemy);
		EnemyStateDataRequest enemyStateDataRequest = new EnemyStateDataRequest(enemy.EnemyID, enemy.GetConditionStateId(this), data.Positions.Length * 3 * 4);
		for (int i = 0; i < data.Positions.Length; i++)
		{
			enemyStateDataRequest.Data.AddVector3(data.Positions[i]);
		}
		GameApp.GetInstance().GetNetworkManager().SendRequest(enemyStateDataRequest);
	}

	private Vector3[] GetPositions(Vector3 enemyPos, Vector3 playerPos)
	{
		Vector3[] array = new Vector3[FrostStoneParameter.SMALL_SNOW_BALL_NUM];
		Vector3 normalized = (playerPos - enemyPos).normalized;
		Vector3 vector = playerPos + normalized * FrostStoneParameter.SMALL_SNOW_BALL_BEHIND_PLAYER_DISTANCE;
		for (int i = 0; i < FrostStoneParameter.SMALL_SNOW_BALL_NUM - 1; i++)
		{
			int num = ((i % 2 == 0) ? 1 : (-1));
			array[i] = vector + Quaternion.Euler(0f, Random.Range((float)num * 45f, (float)num * 135f), 0f) * normalized * Random.Range(FrostStoneParameter.SAMLL_SNOW_BALL_RADIUS.x, FrostStoneParameter.SAMLL_SNOW_BALL_RADIUS.y);
		}
		array[array.Length - 1] = vector;
		return array;
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
}
