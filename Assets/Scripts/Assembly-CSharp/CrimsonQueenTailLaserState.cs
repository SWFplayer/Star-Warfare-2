using UnityEngine;

public class CrimsonQueenTailLaserState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public Transform Point;

		public GameObject Laser;

		public float StartTime;

		public float SideValue;

		public GameObject VFX;
	}

	public override void Start(Enemy enemy)
	{
		CrimsonQueen crimsonQueen = enemy as CrimsonQueen;
		MyData data = GetData<MyData>(enemy);
		data.InnerState = 0;
		data.Point = crimsonQueen.LaserPoint;
		data.VFX = Pool.sGetInstance().Allocate("SW2_Effect/B5_Laser_Charge_small");
		data.VFX.transform.parent = data.Point;
		data.VFX.transform.localPosition = Vector3.zero;
		enemy.PlayAnimation("laser01", WrapMode.ClampForever);
		enemy.LookAtTargetHorizontal();
		enemy.PlaySound(AudioName.DEADEYE_LASER_LONG0);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		MyData data = GetData<MyData>(enemy);
		CrimsonQueen crimsonQueen = enemy as CrimsonQueen;
		if (data.InnerState == 0)
		{
			if (enemy.IsAnimationFinish())
			{
				data.VFX.SetActive(false);
				data.VFX = null;
				data.InnerState = 1;
				enemy.PlayAnimation("laser02", WrapMode.Loop);
				enemy.PlaySoundLoopSingle(AudioName.CRIMSONQUEEN_LASER02);
				data.StartTime = Time.time;
				data.Laser = Pool.sGetInstance().Allocate("SW2_Effect/B5_laser");
				LaserScript component = GameWorld.GetComponent<LaserScript>(data.Laser);
				component.SetDamageInfo(CrimsonQueenParameter.GetInstance(), 3);
				component.Attacker = enemy;
				component.KnockSpeed = CrimsonQueenParameter.TAIL_LASER_KNOCK_SPEED;
				data.Laser.transform.eulerAngles = enemy.GetTransform().eulerAngles;
				data.Laser.transform.position = data.Point.TransformPoint(1f, 0f, 0f);
				if (enemy.GetTarget() != null)
				{
					float y = Vector3.Cross(enemy.GetTransform().forward, enemy.GetTarget().GetPosition() - enemy.GetPosition()).y;
					data.SideValue = ((!(y <= 0f)) ? 1 : (-1));
				}
				else
				{
					data.SideValue = -1f;
				}
				enemy.SetCanTurn(false);
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && enemy.IsMasterPlayer)
				{
					EnemyStateDataRequest enemyStateDataRequest = new EnemyStateDataRequest(enemy.EnemyID, enemy.GetConditionStateId(this));
					enemyStateDataRequest.Data.AddBool(data.SideValue > 0f);
					GameApp.GetInstance().GetNetworkManager().SendRequest(enemyStateDataRequest);
				}
			}
		}
		else if (data.InnerState == 1)
		{
			enemy.GetTransform().Rotate(new Vector3(0f, CrimsonQueenParameter.TAIL_LASER_ROTATE_SPEED * Time.deltaTime * data.SideValue, 0f), Space.World);
			data.Laser.transform.position = data.Point.TransformPoint(1f, 0f, 0f);
			data.Laser.transform.LookAt(data.Laser.transform.position + enemy.GetTransform().forward);
			if (Time.time - data.StartTime > CrimsonQueenParameter.TAIL_LASER_DURATION)
			{
				data.InnerState = 2;
				enemy.PlayAnimation("laser03", WrapMode.ClampForever);
				if (data.Laser != null)
				{
					Pool.sGetInstance().Deallocate(data.Laser);
					data.Laser = null;
					enemy.StopSound(AudioName.CRIMSONQUEEN_LASER02);
				}
			}
		}
		else if (data.InnerState != 2)
		{
		}
	}

	public override void End(Enemy enemy)
	{
		MyData data = GetData<MyData>(enemy);
		if (data.Laser != null)
		{
			Pool.sGetInstance().Deallocate(data.Laser);
			data.Laser = null;
		}
		if (data.VFX != null)
		{
			data.VFX.SetActive(false);
		}
		enemy.StopSound(AudioName.CRIMSONQUEEN_LASER02);
	}

	public override bool IsFinish(GameUnit gameUnit)
	{
		MyData data = GetData<MyData>(gameUnit);
		return data.InnerState == 2 && gameUnit.IsAnimationFinish();
	}

	public override void DoResponse(GameUnit gameUnit, BytesBuffer buffer)
	{
		MyData data = GetData<MyData>(gameUnit);
		bool flag = buffer.ReadBool();
		data.SideValue = ((!flag) ? (-1f) : 1f);
	}
}
