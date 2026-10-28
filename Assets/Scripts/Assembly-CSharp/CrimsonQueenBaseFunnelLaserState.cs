using System.Collections.Generic;
using UnityEngine;

public class CrimsonQueenBaseFunnelLaserState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public int InnerState2;

		public List<Funnel> Funnels;

		public List<Vector3> EndPositions = new List<Vector3>();

		public Vector3 StartPosition;

		public float MoveSpeed;

		public int AttackNum;

		public float BeeFlyBackStartTime;
	}

	public void CalculateDirection(Enemy enemy, MyData data, float laserRadius)
	{
		data.MoveSpeed = CrimsonQueenParameter.FUNNEL_LASER_MOVE_SPEED;
		Vector3 vector;
		if (enemy.GetTarget() != null)
		{
			vector = enemy.GetTarget().GetPosition();
			Vector3 vector2 = enemy.GetPosition() - vector;
			vector2.y = 0f;
			if (vector2.sqrMagnitude > laserRadius * laserRadius)
			{
				Vector3 onUnitSphere = Random.onUnitSphere;
				onUnitSphere.y = 0f;
				vector = enemy.GetPosition() + onUnitSphere * laserRadius * 0.8f;
			}
		}
		else
		{
			Vector3 onUnitSphere2 = Random.onUnitSphere;
			onUnitSphere2.y = 0f;
			vector = enemy.GetPosition() + onUnitSphere2 * laserRadius * 0.8f;
		}
		float num = float.MaxValue;
		int num2 = -1;
		for (int i = 0; i < data.Funnels.Count; i++)
		{
			float sqrMagnitude = (data.Funnels[i].GetPosition() - vector).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num2 = i;
				num = sqrMagnitude;
			}
		}
		Funnel funnel = data.Funnels[num2];
		Vector3 vector3 = vector - data.Funnels[num2].GetPosition();
		vector3.y = 0f;
		Vector3 rhs = enemy.GetPosition() - data.Funnels[num2].GetPosition();
		rhs.y = 0f;
		float magnitude = Vector3.Cross(vector3.normalized, rhs).magnitude;
		float num3 = Mathf.Sqrt(laserRadius * laserRadius - magnitude * magnitude);
		if (data.AttackNum > 0)
		{
			num3 *= 2f;
		}
		Vector3 position = enemy.GetPosition();
		position.y = data.Funnels[num2].GetPosition().y;
		Vector3 vector4 = funnel.GetPosition() + vector3.normalized * num3 - position;
		float num4 = 360f / (float)data.Funnels.Count;
		for (int j = 0; j < data.Funnels.Count; j++)
		{
			Vector3 vector5 = Quaternion.Euler(0f, num4 * (float)j, 0f) * vector4;
			int index = (num2 + j) % data.Funnels.Count;
			data.EndPositions[index] = position + vector5;
		}
		data.AttackNum++;
		if (!GameApp.GetInstance().GetGameMode().IsMultiPlayer() || !enemy.IsMasterPlayer)
		{
			return;
		}
		EnemyStateDataRequest enemyStateDataRequest = new EnemyStateDataRequest(enemy.EnemyID, enemy.GetConditionStateId(this));
		enemyStateDataRequest.Data.AddByte((byte)data.InnerState);
		enemyStateDataRequest.Data.AddByte((byte)data.AttackNum);
		enemyStateDataRequest.Data.AddByte((byte)data.EndPositions.Count);
		foreach (Vector3 endPosition in data.EndPositions)
		{
			enemyStateDataRequest.Data.AddShort((short)(endPosition.x * 10f));
			enemyStateDataRequest.Data.AddShort((short)(endPosition.y * 10f));
			enemyStateDataRequest.Data.AddShort((short)(endPosition.z * 10f));
		}
		GameApp.GetInstance().GetNetworkManager().SendRequest(enemyStateDataRequest);
	}

	public void CalculateDirectionSimple(Enemy enemy, MyData data, float radius)
	{
		Vector3 vector = enemy.GetTransform().forward * radius;
		float num = 360f / (float)data.Funnels.Count;
		for (int i = 0; i < data.Funnels.Count; i++)
		{
			Vector3 vector2 = Quaternion.Euler(0f, num * (float)i, 0f) * vector;
			vector2.y = 0f;
			data.EndPositions[i] = data.Funnels[i].GetPosition() + vector2;
		}
	}
}
