using UnityEngine;

public class FrostStoneIceSpikeState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public Vector3[] Positions;
	}

	public static int Index;

	public override void Start(Enemy enemy)
	{
		MyData data = GetData<MyData>(enemy);
		data.InnerState = 0;
		enemy.PlayAnimation("icespik", WrapMode.ClampForever);
		enemy.LookAtTargetHorizontal();
		Vector3 normalized = new Vector3(Random.value, 0f, Random.value).normalized;
		data.Positions = new Vector3[FrostStoneParameter.ICE_SPIKE_RANDOM_OFFSET.Length];
		for (int i = 0; i < FrostStoneParameter.ICE_SPIKE_RANDOM_OFFSET.Length; i++)
		{
			float num = Random.Range((i != 0) ? FrostStoneParameter.ICE_SPIKE_RANDOM_OFFSET[i - 1] : 0f, FrostStoneParameter.ICE_SPIKE_RANDOM_OFFSET[i]);
			Vector3 vector = Quaternion.Euler(0f, (float)i * 360f / (float)FrostStoneParameter.ICE_SPIKE_RANDOM_OFFSET.Length, 0f) * normalized * num;
			data.Positions[i] = enemy.GetTargetPosition() + vector;
			Ray ray = new Ray(data.Positions[i] + Vector3.up, Vector3.down);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 10f, 1 << PhysicsLayer.FLOOR))
			{
				data.Positions[i] = hitInfo.point;
			}
			else
			{
				Debug.Log("No floor: " + data.Positions[i]);
			}
		}
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			EnemyStateDataRequest enemyStateDataRequest = new EnemyStateDataRequest(enemy.EnemyID, enemy.GetConditionStateId(this), 12 * data.Positions.Length);
			for (int j = 0; j < data.Positions.Length; j++)
			{
				enemyStateDataRequest.Data.AddVector3(data.Positions[j]);
			}
			GameApp.GetInstance().GetNetworkManager().SendRequest(enemyStateDataRequest);
		}
		enemy.PlaySound(AudioName.FROSTSTONE_SPIKE);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		MyData myData = enemy.StateData[this] as MyData;
		if (enemy.IsAnimationTriggered(FrostStoneParameter.ICE_SPIKE_VFX_APPEAR_TIME))
		{
			for (int i = 0; i < myData.Positions.Length; i++)
			{
				GameObject go = Pool.sAllocate("SW2_Effect/B3_Icespike_show", myData.Positions[i]);
				GameWorld.GetComponent<AutoDestroyScript>(go).life = FrostStoneParameter.ICE_SPIKE_VFX_DURATION;
			}
		}
		if (enemy.IsAnimationTriggered(FrostStoneParameter.ICE_SPIKE_APPEAR_TIME))
		{
			for (int j = 0; j < myData.Positions.Length; j++)
			{
				GameObject gameObject = Pool.sAllocate("SW2_Effect/B3_spike", myData.Positions[j], new Vector3(Random.value, 0f, Random.value));
				DestructibleObjectScript component = gameObject.GetComponent<DestructibleObjectScript>();
				gameObject.name = "IceSpike" + Index++;
				GameApp.GetInstance().GetGameWorld().AddDestructible(gameObject);
				FrostStoneIceSpikeScript componentInChildren = gameObject.GetComponentInChildren<FrostStoneIceSpikeScript>();
				componentInChildren.Duration = FrostStoneParameter.ICE_SPIKE_DURATION;
				componentInChildren.SetDamageInfo(FrostStoneParameter.GetInstance(), 5);
				componentInChildren.SetDamageInfo2(FrostStoneParameter.GetInstance(), 6);
				componentInChildren.VFXSound = AudioName.FROSTSTONE_SPIKE_END;
				componentInChildren.Attacker = enemy;
				componentInChildren.Reset();
			}
		}
	}

	public override void End(Enemy enemy)
	{
	}

	public override void DoResponse(GameUnit gameUnit, BytesBuffer buffer)
	{
		MyData data = GetData<MyData>(gameUnit);
		data.Positions = new Vector3[FrostStoneParameter.ICE_SPIKE_RANDOM_OFFSET.Length];
		for (int i = 0; i < data.Positions.Length; i++)
		{
			data.Positions[i] = buffer.ReadVector3();
		}
	}
}
