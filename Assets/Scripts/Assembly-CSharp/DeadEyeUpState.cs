using UnityEngine;

public class DeadEyeUpState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public Vector3 Speed;

		public Vector3 SpeedRotate;

		public Vector3 Position;

		public int InnerState;
	}

	public override void Start(Enemy enemy)
	{
		enemy.PlayAnimation(AnimationString.DEAD_EYE_ROOF_UP, WrapMode.ClampForever);
		enemy.StopNavMesh();
		DeadEye deadEye = (DeadEye)enemy;
		if (!enemy.StateData.ContainsKey(this))
		{
			enemy.StateData.Add(this, new MyData());
		}
		MyData myData = enemy.StateData[this] as MyData;
		myData.InnerState = 0;
		myData.Speed = new Vector3(0f, DeadEyeParameter.ROOF_UP_SPEED, 0f);
		myData.SpeedRotate = new Vector3(0f, DeadEyeParameter.ROOF_UP_SPEED + DeadEyeParameter.ROOF_UP_EXTRA_SPEED, 0f);
		Ray ray = new Ray(enemy.GetPosition() + new Vector3(0f, 0.5f, 0f), Vector3.up);
		int layerMask = 1 << PhysicsLayer.WALL_AGAINST_PALYER;
		RaycastHit hitInfo = default(RaycastHit);
		if (Physics.Raycast(ray, out hitInfo, 50f, layerMask))
		{
			myData.Position = hitInfo.point;
		}
		else
		{
			Debug.LogError("No roof");
		}
		deadEye.GroundRoofStartTime = Time.time;
		deadEye.Shadow.transform.parent = null;
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		MyData myData = enemy.StateData[this] as MyData;
		if (enemy.CurrentAnimTime >= DeadEyeParameter.ROOF_UP_TIME && enemy.GetTransform().position.y < myData.Position.y)
		{
			DeadEye deadEye = (DeadEye)enemy;
			if (enemy.CurrentAnimTime <= 0.6f)
			{
				enemy.GetTransform().position += myData.SpeedRotate * Time.deltaTime;
			}
			else
			{
				enemy.GetTransform().position += myData.Speed * Time.deltaTime;
			}
			myData.Speed.y -= DeadEyeParameter.ROOF_UP_DECELERATE * Time.deltaTime;
			deadEye.Shadow.transform.localScale -= Vector3.one * Time.deltaTime * 2f;
			if (deadEye.Shadow.transform.localScale.x < 0f)
			{
				deadEye.Shadow.transform.localScale = Vector3.zero;
				deadEye.SetShadow(false);
			}
		}
	}

	public override void End(Enemy enemy)
	{
		DeadEye deadEye = (DeadEye)enemy;
		deadEye.SetShadow(false);
		enemy.PlaySound(AudioName.DEADEYE_ROOF_DOWN);
	}

	public override bool IsFinish(GameUnit gameUnit)
	{
		MyData myData = gameUnit.StateData[this] as MyData;
		return gameUnit.GetTransform().position.y >= myData.Position.y;
	}
}
