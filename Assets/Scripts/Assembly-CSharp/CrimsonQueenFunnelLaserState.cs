using UnityEngine;

public class CrimsonQueenFunnelLaserState : CrimsonQueenBaseFunnelLaserState
{
	public override void Start(Enemy enemy)
	{
		MyData data = GetData<MyData>(enemy);
		data.InnerState = 0;
		data.InnerState2 = 0;
		data.AttackNum = 0;
		data.Funnels = null;
		GameObject gameObject = GameObject.Find("__BossStartPosition");
		if (gameObject == null)
		{
			gameObject = new GameObject("__BossStartPosition");
			gameObject.transform.position = enemy.GetTarget().GetPosition();
		}
		data.StartPosition = gameObject.transform.position;
		enemy.LookAtPointHorizontalAtOnce(data.StartPosition);
		enemy.PlayAnimation("run", WrapMode.Loop);
		enemy.MoveToward(data.StartPosition, CrimsonQueenParameter.FUNNEL_LASER_MOVE_TO_POINT_SPEED);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		MyData data = GetData<MyData>(enemy);
		if (data.InnerState2 == 0 && Vector3.SqrMagnitude(enemy.GetPosition() - data.StartPosition) < 1f)
		{
			data.InnerState2++;
			enemy.PlayAnimation("bee01", WrapMode.ClampForever);
			enemy.StopNavMesh();
			enemy.LookAtTargetHorizontal();
		}
		if (data.InnerState2 == 1)
		{
			if (enemy.IsAnimationTriggered(CrimsonQueenParameter.BEE_OPEN_SOUND_TIME))
			{
				enemy.PlaySound(AudioName.CRIMSONQUEEN_BEE_OPEN);
			}
			if (enemy.IsAnimationFinish())
			{
				enemy.PlayAnimation("bee02", WrapMode.Loop);
				data.InnerState2++;
			}
		}
		else if (data.InnerState2 == 2 && data.InnerState == 4 && Time.time - data.BeeFlyBackStartTime > CrimsonQueenParameter.BEE_START_BACK_ANIM_TIME)
		{
			enemy.PlayAnimation("bee03", WrapMode.ClampForever);
			enemy.PlaySound(AudioName.CRIMSONQUEEN_BEE_CLOSE);
			data.InnerState2 = 3;
		}
		if (data.InnerState == 0)
		{
			if (data.InnerState2 != 1 || !enemy.IsAnimationPlayed(CrimsonQueenParameter.FUNNEL_LASER_BEE_SPAWN_TIME))
			{
				return;
			}
			CrimsonQueen crimsonQueen = enemy as CrimsonQueen;
			int num = CrimsonQueenParameter.FUNNEL_LASER_NUM[crimsonQueen.GetMindStateInt()];
			data.Funnels = crimsonQueen.GetFunnels(num);
			data.EndPositions.Clear();
			for (int i = 0; i < num; i++)
			{
				data.EndPositions.Add(Vector3.zero);
			}
			float y = 360f / (float)num;
			Vector3 vector = enemy.GetTransform().forward;
			foreach (Funnel funnel2 in data.Funnels)
			{
				funnel2.SetActive(true);
				funnel2.SetPosition(enemy.GetPosition() + new Vector3(0f, 4f, 0f));
				vector = Quaternion.Euler(0f, y, 0f) * vector;
				funnel2.GetTransform().LookAt(funnel2.GetPosition() + vector);
				funnel2.PlaySpawnAnimation();
			}
			data.InnerState++;
		}
		else if (data.InnerState == 1)
		{
			bool flag = true;
			foreach (Funnel funnel3 in data.Funnels)
			{
				if (funnel3.CurrentAnimTime < 1f)
				{
					flag = false;
					break;
				}
			}
			if (!flag || !enemy.IsMasterPlayer)
			{
				return;
			}
			data.InnerState++;
			foreach (Funnel funnel4 in data.Funnels)
			{
				funnel4.SpawnFinishPosition = funnel4.GetPosition();
				funnel4.SpawnFinishRotation = funnel4.GetTransform().eulerAngles;
				funnel4.ShowLaser();
			}
			enemy.PlaySoundLoop(AudioName.CRIMSONQUEEN_LASER_BEE);
			CalculateDirection(enemy, data, CrimsonQueenParameter.FUNNEL_LASER_RADIUS);
		}
		else if (data.InnerState == 2)
		{
			data.MoveSpeed += CrimsonQueenParameter.FUNNEL_LASER_MOVE_ACC * Time.deltaTime;
			float maxDistanceDelta = data.MoveSpeed * Time.deltaTime;
			bool flag2 = true;
			for (int j = 0; j < data.Funnels.Count; j++)
			{
				Funnel funnel = data.Funnels[j];
				Vector3 vector2 = data.EndPositions[j];
				funnel.GetTransform().position = Vector3.MoveTowards(funnel.GetPosition(), vector2, maxDistanceDelta);
				if (flag2 && Vector3.SqrMagnitude(funnel.GetPosition() - vector2) > 0.5f)
				{
					flag2 = false;
				}
			}
			if (flag2)
			{
				CrimsonQueen crimsonQueen2 = enemy as CrimsonQueen;
				if (data.AttackNum >= CrimsonQueenParameter.FUNNEL_LASER_ATTACK_NUM[crimsonQueen2.GetMindStateInt()] + 1)
				{
					data.InnerState++;
				}
				else if (enemy.IsMasterPlayer)
				{
					CalculateDirection(enemy, data, CrimsonQueenParameter.FUNNEL_LASER_RADIUS);
				}
			}
		}
		else if (data.InnerState == 3)
		{
			bool flag3 = true;
			foreach (Funnel funnel5 in data.Funnels)
			{
				if (!funnel5.MoveToSpawnPosition())
				{
					flag3 = false;
				}
			}
			if (!flag3)
			{
				return;
			}
			data.InnerState++;
			foreach (Funnel funnel6 in data.Funnels)
			{
				funnel6.PlayBackAnimation();
				funnel6.HideLaser();
			}
			enemy.StopSound(AudioName.CRIMSONQUEEN_LASER_BEE);
			data.BeeFlyBackStartTime = Time.time;
		}
		else
		{
			if (data.InnerState != 4)
			{
				return;
			}
			bool flag4 = true;
			foreach (Funnel funnel7 in data.Funnels)
			{
				if (funnel7.CurrentAnimTime < 1f)
				{
					flag4 = false;
					break;
				}
			}
			if (!flag4)
			{
				return;
			}
			data.InnerState++;
			foreach (Funnel funnel8 in data.Funnels)
			{
				funnel8.SetActive(false);
			}
		}
	}

	public override void End(Enemy enemy)
	{
		MyData data = GetData<MyData>(enemy);
		if (data.Funnels != null)
		{
			foreach (Funnel funnel in data.Funnels)
			{
				funnel.SetActive(false);
				funnel.HideLaser();
			}
		}
		enemy.StopNavMesh();
		enemy.StopSound(AudioName.CRIMSONQUEEN_LASER_BEE);
	}

	public override bool IsFinish(GameUnit gameUnit)
	{
		MyData data = GetData<MyData>(gameUnit);
		return data.InnerState2 == 3 && gameUnit.IsAnimationFinish();
	}

	public override void DoResponse(GameUnit gameUnit, BytesBuffer buffer)
	{
		MyData data = GetData<MyData>(gameUnit);
		byte b = buffer.ReadByte();
		byte b2 = buffer.ReadByte();
		byte b3 = buffer.ReadByte();
		CrimsonQueen crimsonQueen = gameUnit as CrimsonQueen;
		if (data.InnerState == 0)
		{
			int num = CrimsonQueenParameter.FUNNEL_LASER_NUM[crimsonQueen.GetMindStateInt()];
			data.Funnels = crimsonQueen.GetFunnels(num);
			data.EndPositions.Clear();
			float y = 360f / (float)num;
			Vector3 vector = crimsonQueen.GetTransform().forward;
			foreach (Funnel funnel in data.Funnels)
			{
				funnel.SetActive(true);
				funnel.SetPosition(crimsonQueen.GetPosition() + new Vector3(0f, 4f, 0f));
				vector = Quaternion.Euler(0f, y, 0f) * vector;
				funnel.GetTransform().LookAt(funnel.GetPosition() + vector);
			}
		}
		if (b != 2)
		{
			return;
		}
		data.InnerState = 2;
		data.AttackNum = b2;
		data.MoveSpeed = CrimsonQueenParameter.FUNNEL_LASER_MOVE_SPEED;
		if (b2 == 1)
		{
			foreach (Funnel funnel2 in data.Funnels)
			{
				funnel2.SpawnFinishPosition = funnel2.GetPosition();
				funnel2.SpawnFinishRotation = funnel2.GetTransform().eulerAngles;
				funnel2.ShowLaser();
			}
			crimsonQueen.PlaySoundLoop(AudioName.CRIMSONQUEEN_LASER_BEE);
		}
		data.EndPositions.Clear();
		for (int i = 0; i < b3; i++)
		{
			float x = (float)buffer.ReadShort() / 10f;
			float y2 = (float)buffer.ReadShort() / 10f;
			float z = (float)buffer.ReadShort() / 10f;
			data.EndPositions.Add(new Vector3(x, y2, z));
		}
	}
}
