using UnityEngine;

public class BossStepBackState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public float Speed;

		public float MinSpeed;

		public float Deceleration;
	}

	public override void Start(Enemy enemy)
	{
		enemy.LookAtTargetHorizontal();
		enemy.PlayAnimation(AnimationString.BOSS_STEP_BACK, WrapMode.ClampForever);
		if (!enemy.StateData.ContainsKey(this))
		{
			enemy.StateData.Add(this, new MyData());
		}
		MyData myData = enemy.StateData[this] as MyData;
		myData.InnerState = 0;
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		EnemyBoss enemyBoss = enemy as EnemyBoss;
		enemy.LookAtTargetHorizontal();
		MyData myData = enemy.StateData[this] as MyData;
		if (myData.InnerState == 0)
		{
			if (enemy.CurrentAnimTime >= 0.37f)
			{
				myData.InnerState++;
				Vector3 vector = enemy.GetPosition() - enemy.GetTransform().forward * enemyBoss.StepBack.BACK_DISTANCE;
				Vector3 vector2 = -enemy.GetTransform().forward;
				float num = enemyBoss.StepBack.BACK_DISTANCE;
				Ray ray = new Ray(enemy.GetPosition() + new Vector3(0f, 0.5f, 0f), vector2);
				int layerMask = 1 << PhysicsLayer.WALL_AGAINST_PALYER;
				RaycastHit hitInfo = default(RaycastHit);
				if (Physics.Raycast(ray, out hitInfo, num + enemyBoss.StepBack.MINIMUM_DISTANCE_BEFORE_WALL, layerMask))
				{
					num = (enemy.GetPosition() + new Vector3(0f, 0.5f, 0f) - hitInfo.point).magnitude - enemyBoss.StepBack.MINIMUM_DISTANCE_BEFORE_WALL;
				}
				num = Mathf.Max(0f, num);
				vector = enemy.GetPosition() + vector2 * num;
				myData.Speed = enemyBoss.StepBack.BACK_MAXIMUM_SPEED * num;
				enemy.MoveToward(vector, myData.Speed);
				enemy.SetCanTurn(true);
				myData.MinSpeed = enemyBoss.StepBack.BACK_MINIMUM_SPEED * num;
				myData.Deceleration = (enemyBoss.StepBack.BACK_MAXIMUM_SPEED - enemyBoss.StepBack.BACK_MINIMUM_SPEED) / enemyBoss.StepBack.BACK_TIME * num;
			}
		}
		else if (myData.InnerState == 1)
		{
			myData.Speed = Mathf.Max(myData.MinSpeed, myData.Speed - myData.Deceleration * Time.deltaTime);
			enemy.ChangeNavigationSpeed(myData.Speed);
		}
	}

	public override void End(Enemy enemy)
	{
		enemy.StopNavMesh();
	}
}
