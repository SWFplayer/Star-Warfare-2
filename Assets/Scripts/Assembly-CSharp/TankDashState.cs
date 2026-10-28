using UnityEngine;

public class TankDashState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public GameObject Vfx;

		public Vector3 TargetPosition;
	}

	public override void Start(Enemy enemy)
	{
		Tank tank = enemy as Tank;
		if (!enemy.StateData.ContainsKey(this))
		{
			enemy.StateData.Add(this, new MyData());
		}
		MyData myData = enemy.StateData[this] as MyData;
		myData.InnerState = 0;
		enemy.PlayAnimation("dash", WrapMode.Loop);
		enemy.MoveToward(enemy.GetTargetPosition(), TankParameter.DashSpeed);
		GameObject original = Resources.Load("SW2_Effect/B2_Dash02_smoke") as GameObject;
		myData.Vfx = Object.Instantiate(original, enemy.GetPosition(), Quaternion.identity) as GameObject;
		myData.Vfx.transform.parent = enemy.GetTransform();
		tank.ResetAttack(tank.DashAttackSkill);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		MyData data = GetData<MyData>(enemy);
		Tank tank = enemy as Tank;
		if (data.InnerState == 0)
		{
			enemy.UpdateNavMeshInDash(false, tank.DashSpeed);
			if (enemy.GetSqrDistanceFromTarget() <= TankParameter.DashStopRotateDistance * TankParameter.DashStopRotateDistance)
			{
				data.TargetPosition = enemy.GetTargetPosition();
				enemy.MoveToward(data.TargetPosition, tank.DashSpeed);
				data.InnerState++;
			}
		}
		enemy.CheckHit2(((Tank)enemy).DashAttackSkill);
	}

	public override void End(Enemy enemy)
	{
		enemy.StopNavMesh();
		MyData myData = enemy.StateData[this] as MyData;
		Object.Destroy(myData.Vfx);
	}

	public override bool IsFinish(GameUnit gameUnit)
	{
		MyData data = GetData<MyData>(gameUnit);
		return data.InnerState == 1 && Vector3.SqrMagnitude(gameUnit.GetPosition() - data.TargetPosition) <= TankParameter.DashStartAttackRadiusSqr;
	}
}
