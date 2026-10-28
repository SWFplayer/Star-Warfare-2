using System.Collections.Generic;
using UnityEngine;

public class CrimsonQueenExplosionSpawnState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public Vector3 StartPosition;

		public Vector3 EndPosition;

		public float MoveUpSpeed;

		public float FlySpeed;

		public float BombDistance;

		public float OutsideMoveStartTime;

		public List<CrimsonQueenBomb> BombList = new List<CrimsonQueenBomb>();
	}

	public override void Start(Enemy enemy)
	{
		MyData data = GetData<MyData>(enemy);
		data.InnerState = 0;
		GameObject gameObject = GameObject.Find("__BossStartPosition");
		if (gameObject == null)
		{
			gameObject = new GameObject("__BossStartPosition");
			gameObject.transform.position = enemy.GetTarget().GetPosition();
		}
		data.StartPosition = gameObject.transform.position;
		data.MoveUpSpeed = CrimsonQueenParameter.EXPLOSION_SPAWN_MOVE_UP_SPEED;
		data.FlySpeed = CrimsonQueenParameter.EXPLOSION_SPAWN_FLY_SPEED;
		enemy.LookAtPointHorizontalAtOnce(data.StartPosition);
		enemy.PlayAnimation("run", WrapMode.Loop);
		enemy.MoveToward(data.StartPosition, CrimsonQueenParameter.EXPLOSION_SPAWN_MOVE_TO_START_SPEED);
		foreach (CrimsonQueenBomb bomb in data.BombList)
		{
			if (bomb != null && bomb.gameObject.activeSelf)
			{
				bomb.Explode();
			}
		}
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		MyData data = GetData<MyData>(enemy);
		CrimsonQueen enemy2 = enemy as CrimsonQueen;
		if (data.InnerState == 0)
		{
			if (Vector3.SqrMagnitude(enemy.GetPosition() - data.StartPosition) < 1f)
			{
				data.InnerState++;
				enemy.PlayAnimation("fly01", WrapMode.ClampForever);
				enemy.StopNavMesh();
				if (enemy.GetTarget() != null)
				{
					enemy.GetTransform().LookAt(enemy.GetTarget().GetPosition());
				}
			}
		}
		else if (data.InnerState == 1)
		{
			if (enemy.CurrentAnimTime >= CrimsonQueenParameter.EXPLOSION_SPAWN_MOVE_UP_TIME)
			{
				enemy.GetTransform().Translate(0f, data.MoveUpSpeed * Time.deltaTime, 0f, Space.World);
				data.MoveUpSpeed += CrimsonQueenParameter.EXPLOSION_SPAWN_MOVE_UP_ACC * Time.deltaTime;
				if (enemy.GetPosition().y - data.StartPosition.y > CrimsonQueenParameter.EXPLOSION_SPAWN_MOVE_UP_HEIGHT)
				{
					data.InnerState++;
					enemy.PlayAnimation("fly02", WrapMode.Loop);
					data.StartPosition -= enemy.GetTransform().right;
					data.BombDistance = 0f;
				}
			}
		}
		else if (data.InnerState == 2)
		{
			float num = FlyRound(enemy2, data, true);
			if (num > CrimsonQueenParameter.EXPLOSION_SPAWN_FLY_MAX_RADIUS * CrimsonQueenParameter.EXPLOSION_SPAWN_FLY_MAX_RADIUS)
			{
				data.InnerState++;
				data.OutsideMoveStartTime = Time.time;
			}
		}
		else if (data.InnerState == 3)
		{
			FlyRound(enemy2, data, false);
			if (Time.time - data.OutsideMoveStartTime > CrimsonQueenParameter.EXPLOSION_SPAWN_OUTSIDE_MOVE_TIME)
			{
				data.InnerState++;
				Vector3 vector = enemy.GetPosition() - data.StartPosition;
				vector.y = 0f;
				Vector3 endPosition = data.StartPosition + vector.normalized * CrimsonQueenParameter.EXPLOSION_SPAWN_FLY_BACK_RADIUS;
				endPosition.y = enemy.GetPosition().y;
				data.EndPosition = endPosition;
				enemy.GetTransform().LookAt(data.EndPosition);
			}
		}
		else if (data.InnerState == 4)
		{
			enemy.GetTransform().position = Vector3.MoveTowards(enemy.GetPosition(), data.EndPosition, data.FlySpeed * Time.deltaTime);
			if ((enemy.GetPosition() - data.EndPosition).sqrMagnitude < 1f)
			{
				data.InnerState++;
				enemy.PlayAnimation("fly03", WrapMode.ClampForever);
				Ray ray = new Ray(enemy.GetPosition(), Vector3.down);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, CrimsonQueenParameter.EXPLOSION_SPAWN_MOVE_UP_HEIGHT, 1 << PhysicsLayer.FLOOR))
				{
					data.EndPosition = hitInfo.point + new Vector3(0f, 0.1f, 0f);
					return;
				}
				data.EndPosition = enemy.GetPosition();
				data.EndPosition.y = data.StartPosition.y + 0.1f;
			}
		}
		else if (data.InnerState == 5)
		{
			enemy.GetTransform().Translate(Vector3.down * CrimsonQueenParameter.EXPLOSION_SPAWN_MOVE_DOWN_SPEED * Time.deltaTime);
			if (enemy.GetPosition().y < data.EndPosition.y)
			{
				enemy.GetTransform().position = data.EndPosition;
				data.InnerState++;
			}
		}
	}

	public override void End(Enemy enemy)
	{
		enemy.StopNavMesh();
	}

	public override bool IsFinish(GameUnit gameUnit)
	{
		MyData data = GetData<MyData>(gameUnit);
		return data.InnerState == 6;
	}

	private float FlyRound(EnemyBoss enemy, MyData data, bool rotate)
	{
		if (data.BombDistance > CrimsonQueenParameter.EXPLOSION_SPAWN_BOMB_INTERVAL_DISTANCE[enemy.GetMindStateInt()])
		{
			data.BombDistance = 0f;
			GameObject gameObject = Pool.sGetInstance().Allocate("Enemy/L1/Boss/Weapon/CqBomb", enemy.GetPosition());
			CrimsonQueenBomb component = gameObject.GetComponent<CrimsonQueenBomb>();
			data.BombList.Add(component);
			component.Reset();
			component.SetDamageInfo(CrimsonQueenParameter.GetInstance(), 6);
			component.Attacker = enemy;
			component.KnockSpeed = CrimsonQueenParameter.EXPLOSION_SPAWN_BOMB_KNOCK_SPEED;
			DestructibleObjectScript component2 = gameObject.GetComponent<DestructibleObjectScript>();
			component2.MaxHp = ((GameApp.GetInstance().GetUserState().GetSelectedDifficultMode() != EDifficultMode.HEROIC) ? CrimsonQueenParameter.EXPLOSION_SPAWN_BOMB_HP : CrimsonQueenParameter.EXPLOSION_SPAWN_BOMB_HP_HERO);
			component2.DoNotDestroy = true;
			component2.onDead = component.Explode;
			component2.Activate();
			AudioManager.GetInstance().PlaySoundAt(AudioName.CRIMSONQUEEN_BOMB_DROP, gameObject.transform.position);
		}
		Vector3 lhs = enemy.GetPosition() - data.StartPosition;
		lhs.y = 0f;
		Vector3 vector = Vector3.Cross(lhs, Vector3.up).normalized;
		if (rotate)
		{
			vector = Quaternion.Euler(0f, CrimsonQueenParameter.EXPLOSION_SPAWN_FLY_ROTATE_ANGLE, 0f) * vector;
			enemy.GetTransform().Translate(vector * data.FlySpeed * Time.deltaTime, Space.World);
		}
		else
		{
			Vector3 vector2 = enemy.GetPosition() + vector * data.FlySpeed * Time.deltaTime;
			Vector3 vector3 = vector2 - data.StartPosition;
			vector3.y = 0f;
			vector2 = data.StartPosition + vector3.normalized * CrimsonQueenParameter.EXPLOSION_SPAWN_FLY_MAX_RADIUS;
			vector2.y = enemy.GetPosition().y;
			enemy.GetTransform().position = vector2;
		}
		enemy.GetTransform().LookAt(enemy.GetPosition() + vector);
		data.BombDistance += data.FlySpeed * Time.deltaTime;
		data.FlySpeed += CrimsonQueenParameter.EXPLOSION_SPAWN_FLY_ACC * Time.deltaTime;
		return lhs.sqrMagnitude;
	}
}
