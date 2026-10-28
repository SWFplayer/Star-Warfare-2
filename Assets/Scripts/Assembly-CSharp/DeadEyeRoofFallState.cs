using UnityEngine;

internal class DeadEyeRoofFallState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public Vector3 Speed;

		public Vector3 Position;
	}

	public override void Start(Enemy enemy)
	{
		enemy.PlayAnimation(AnimationString.DEAD_EYE_ROOF_FALL, WrapMode.ClampForever);
		enemy.StopNavMesh();
		DeadEye deadEye = (DeadEye)enemy;
		if (!enemy.StateData.ContainsKey(this))
		{
			enemy.StateData.Add(this, new MyData());
		}
		MyData myData = enemy.StateData[this] as MyData;
		myData.Speed = new Vector3(0f, DeadEyeParameter.ROOF_DOWN_SPEED, 0f);
		Ray ray = new Ray(enemy.GetPosition() + new Vector3(0f, -0.5f, 0f), Vector3.down);
		int layerMask = 1 << PhysicsLayer.FLOOR;
		RaycastHit hitInfo = default(RaycastHit);
		if (Physics.Raycast(ray, out hitInfo, 50f, layerMask))
		{
			myData.Position = hitInfo.point;
		}
		else
		{
			Debug.LogError("No ground");
		}
		deadEye.SetShadow(true);
		deadEye.Shadow.transform.position = myData.Position + new Vector3(0f, 0.2f, 0f);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		MyData myData = enemy.StateData[this] as MyData;
		if (enemy.GetTransform().position.y <= myData.Position.y)
		{
			if (enemy.Hp <= 0)
			{
				enemy.SetState(DeadEye.DEAD_EYE_ROOF_DEAD_STATE);
			}
		}
		else if (enemy.CurrentAnimTime >= 0.2f)
		{
			DeadEye deadEye = (DeadEye)enemy;
			enemy.GetTransform().position -= myData.Speed * Time.deltaTime;
			deadEye.Shadow.transform.localScale += Vector3.one * Time.deltaTime * 2f;
			if (deadEye.Shadow.transform.localScale.x > deadEye.ShadowScale)
			{
				deadEye.Shadow.transform.localScale = Vector3.one;
			}
		}
	}

	public override void End(Enemy enemy)
	{
		DeadEye deadEye = (DeadEye)enemy;
		MyData myData = enemy.StateData[this] as MyData;
		deadEye.Shadow.transform.parent = deadEye.GetTransform();
		deadEye.Shadow.transform.localPosition = new Vector3(0f, deadEye.ShadowLocalY, 0f);
		Vector3 position = enemy.GetTransform().position;
		position.y = myData.Position.y;
		enemy.GetTransform().position = position;
		deadEye.PlaySound(AudioName.DEADEYE_ROOF_DOWN);
	}

	public override bool IsFinish(GameUnit gameUnit)
	{
		MyData myData = gameUnit.StateData[this] as MyData;
		return gameUnit.GetTransform().position.y <= myData.Position.y;
	}
}
