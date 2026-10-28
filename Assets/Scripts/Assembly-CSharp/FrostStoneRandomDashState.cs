using UnityEngine;

public class FrostStoneRandomDashState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public Vector3 Direction;

		public Vector3 Position;

		public Vector3 NextPosition;

		public float Speed;

		public float StartTime;

		public float[] Rotate;

		public Timer Timer = new Timer();

		public GameObject DashSmoke2;

		public Vector3 LastPosition;
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
		enemy.PlayAnimation(AnimationString.BOSS_DASH02, WrapMode.Loop);
		enemy.LookAtTargetHorizontal();
		myData.Speed = 0f;
		myData.LastPosition = enemy.GetPosition();
		myData.Rotate = new float[FrostStoneParameter.RANDOM_DASH_TIMES + 1];
		for (int i = 0; i < FrostStoneParameter.RANDOM_DASH_TIMES + 1; i++)
		{
			myData.Rotate[i] = Random.Range(0f - FrostStoneParameter.RANDOM_DASH_DELTA_ANGLE, FrostStoneParameter.RANDOM_DASH_DELTA_ANGLE);
		}
		if (enemy.IsMasterPlayer)
		{
			UpdateAttackTargetPosition(enemy, myData);
			myData.Position = myData.NextPosition;
			myData.Direction = myData.Position - enemy.GetPosition();
		}
		myData.StartTime = Time.time;
		enemy.LookAtPointHorizontalAtOnce(myData.Position);
		FrostStone frostStone = enemy as FrostStone;
		frostStone.ResetAttack(frostStone.mSkill.RandomDashAttackSkill);
		frostStone.CurrentRandomDashHitNum = 0f;
		Pool.sAllocate("SW2_Effect/B3_Dash01_smoke", enemy.GetPosition(), 4f);
		myData.DashSmoke2 = Pool.sGetInstance().Allocate("SW2_Effect/B3_Dash02_smoke");
		myData.DashSmoke2.transform.parent = enemy.GetTransform();
		myData.DashSmoke2.transform.localPosition = Vector3.zero;
		frostStone.PlaySoundLoop(AudioName.FROSTSTONE_DASH);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		MyData myData = enemy.StateData[this] as MyData;
		FrostStone frostStone = enemy as FrostStone;
		if (myData.InnerState >= FrostStoneParameter.RANDOM_DASH_TIMES)
		{
			return;
		}
		Vector3 vector = myData.Position - enemy.GetPosition();
		frostStone.MoveDelta(myData.Speed * vector.normalized * Time.deltaTime + Vector3.down);
		float target = ((!frostStone.IsHardMode()) ? FrostStoneParameter.RANDOM_DASH_MAX_SPEED[0] : FrostStoneParameter.RANDOM_DASH_MAX_SPEED[1]);
		myData.Speed = Mathf.MoveTowards(myData.Speed, target, Time.deltaTime * FrostStoneParameter.RANDOM_DASH_ACCELERATE);
		if (frostStone.HasIceArmor && Vector3.SqrMagnitude(enemy.GetPosition() - myData.LastPosition) >= FrostStoneParameter.ICE_GROUND_VFX_DISTANCE_SQR)
		{
			myData.LastPosition = enemy.GetPosition();
			GameObject gameObject = Pool.sAllocate("SW2_Effect/B3_IceGround", enemy.GetPosition() + new Vector3(0f, 0.4f, 0f));
			FrostStoneIceGroundScript component = gameObject.GetComponent<FrostStoneIceGroundScript>();
			component.Duration = 5f;
			component.SetDamageInfo(FrostStoneParameter.GetInstance(), 8);
			component.Attacker = enemy;
			component.Reset();
		}
		if (frostStone.IsMasterPlayer && frostStone.mWallCheck.IsHitWall && Time.time - myData.StartTime > 1f)
		{
			if (!frostStone.IsHardMode() && frostStone.CurrentRandomDashHitNum >= (float)FrostStoneParameter.RANDOM_DASH_QUIT_HIT_NUM)
			{
				myData.InnerState = FrostStoneParameter.RANDOM_DASH_TIMES;
			}
			else
			{
				myData.InnerState++;
				myData.Speed = 0f;
				UpdateAttackTargetPosition(enemy, myData);
				myData.Position = myData.NextPosition;
				myData.Direction = myData.Position - enemy.GetPosition();
				myData.StartTime = Time.time;
				enemy.LookAtPointHorizontalAtOnce(myData.Position);
				frostStone.ResetAttack(frostStone.mSkill.RandomDashAttackSkill);
			}
		}
		frostStone.CheckHit2(frostStone.mSkill.RandomDashAttackSkill);
	}

	public override void End(Enemy enemy)
	{
		MyData data = GetData<MyData>(enemy);
		if (data.DashSmoke2 != null)
		{
			Pool.sGetInstance().Deallocate(data.DashSmoke2);
		}
		enemy.StopSound(AudioName.FROSTSTONE_DASH);
	}

	public override bool IsFinish(GameUnit gameUnit)
	{
		MyData myData = gameUnit.StateData[this] as MyData;
		return myData.InnerState == FrostStoneParameter.RANDOM_DASH_TIMES;
	}

	private void UpdateAttackTargetPosition(Enemy enemy, MyData data)
	{
		Vector3 zero = Vector3.zero;
		if (Vector3.Magnitude(enemy.GetTargetPosition() - enemy.GetPosition()) < FrostStoneParameter.RANDOM_DASH_CHANGE_DIR_DISTANCE)
		{
			zero = -data.Direction;
			zero.y = 0f;
			zero = zero.normalized;
		}
		else
		{
			zero = enemy.GetTargetPosition() - enemy.GetPosition();
			zero.y = 0f;
			zero = zero.normalized;
		}
		if (data.InnerState % 2 == 1)
		{
			zero = Quaternion.Euler(0f, data.Rotate[data.InnerState], 0f) * zero;
		}
		Ray ray = new Ray(enemy.GetPosition() + Vector3.up, zero);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 500f, 1 << PhysicsLayer.WALL_AGAINST_PALYER))
		{
			data.NextPosition = hitInfo.point;
		}
		else
		{
			Debug.LogError("居然没墙？？");
		}
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			EnemyStateDataRequest enemyStateDataRequest = new EnemyStateDataRequest(enemy.EnemyID, enemy.GetConditionStateId(this), 13);
			enemyStateDataRequest.Data.AddByte((byte)data.InnerState);
			enemyStateDataRequest.Data.AddVector3(data.NextPosition);
			GameApp.GetInstance().GetNetworkManager().SendRequest(enemyStateDataRequest);
		}
	}

	public override void DoResponse(GameUnit gameUnit, BytesBuffer buffer)
	{
		MyData data = GetData<MyData>(gameUnit);
		data.InnerState = buffer.ReadByte();
		FrostStone frostStone = gameUnit as FrostStone;
		data.Position = buffer.ReadVector3();
		data.Direction = data.Position - gameUnit.GetPosition();
		data.Speed = 0f;
		data.StartTime = Time.time;
		frostStone.LookAtPointHorizontalAtOnce(data.Position);
		frostStone.ResetAttack(frostStone.mSkill.RandomDashAttackSkill);
	}
}
