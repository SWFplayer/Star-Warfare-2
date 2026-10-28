using UnityEngine;

public class DeadEyeDownState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public Vector3 Speed;

		public Vector3 SpeedRotate;

		public Vector3 Position;

		public bool SpawnVFX;

		public int InnerState;
	}

	public override void Start(Enemy enemy)
	{
		enemy.PlayAnimation(AnimationString.DEAD_EYE_ROOF_DOWN, WrapMode.ClampForever);
		enemy.StopNavMesh();
		DeadEye deadEye = (DeadEye)enemy;
		if (!enemy.StateData.ContainsKey(this))
		{
			enemy.StateData.Add(this, new MyData());
		}
		MyData myData = enemy.StateData[this] as MyData;
		myData.InnerState = 0;
		myData.Speed = new Vector3(0f, DeadEyeParameter.ROOF_DOWN_SPEED, 0f);
		myData.SpawnVFX = false;
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
		deadEye.GroundRoofStartTime = Time.time;
		deadEye.ResetAttack(deadEye.mSkill.DownAttackSkill);
		deadEye.SetShadow(true);
		deadEye.Shadow.transform.position = myData.Position + new Vector3(0f, 0.2f, 0f);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		MyData myData = enemy.StateData[this] as MyData;
		DeadEye deadEye = (DeadEye)enemy;
		if (myData.InnerState == 0 && enemy.CurrentAnimTime >= DeadEyeParameter.ROOF_DOWN_TIME)
		{
			enemy.GetTransform().position -= myData.Speed * Time.deltaTime;
			deadEye.Shadow.transform.localScale += Vector3.one * Time.deltaTime * 2f;
			if (deadEye.Shadow.transform.localScale.x > deadEye.ShadowScale)
			{
				deadEye.Shadow.transform.localScale = Vector3.one;
			}
			if (enemy.GetTransform().position.y <= myData.Position.y)
			{
				myData.InnerState++;
				Vector3 position = enemy.GetTransform().position;
				position.y = myData.Position.y;
				enemy.GetTransform().position = position;
				deadEye.PlaySound(AudioName.DEADEYE_ROOF_DOWN);
			}
		}
		if (!myData.SpawnVFX && deadEye.GetPosition().y - myData.Position.y <= 16f)
		{
			myData.SpawnVFX = true;
			GameObject original = Resources.Load("SW2_Effect/B2_Dash01_smoke") as GameObject;
			Object.Instantiate(original, myData.Position, Quaternion.identity);
		}
		enemy.CheckHit2(((DeadEye)enemy).mSkill.DownAttackSkill);
	}

	public override void End(Enemy enemy)
	{
		DeadEye deadEye = (DeadEye)enemy;
		deadEye.Shadow.transform.parent = deadEye.GetTransform();
		deadEye.Shadow.transform.localPosition = new Vector3(0f, deadEye.ShadowLocalY, 0f);
	}

	public override bool IsFinish(GameUnit gameUnit)
	{
		MyData myData = gameUnit.StateData[this] as MyData;
		return myData.InnerState == 1 && gameUnit.CurrentAnimTime >= 1f;
	}
}
