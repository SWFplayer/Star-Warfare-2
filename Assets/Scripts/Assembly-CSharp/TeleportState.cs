using System;
using UnityEngine;

public class TeleportState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public Vector3 Postition;
	}

	public override void Start(Enemy enemy)
	{
		enemy.PlayAnimation("attack_sp", WrapMode.ClampForever);
		GameObject gameObject = EffectPool.GetInstance().GetEffectPool(EffectPoolType.VD5_FLASH).CreateObject(enemy.GetTransform().position + Vector3.up * 1.2f, enemy.GetTransform().forward, enemy.GetTransform().rotation);
		gameObject.transform.localScale = Vector3.one;
		if (enemy.GetTransform().Find("Myth370") != null)
		{
			enemy.GetTransform().Find("Myth370").gameObject.GetComponent<Renderer>().enabled = false;
		}
		enemy.GetTransform().Find(EffectName.E5_Shield).gameObject.GetComponent<TweenScale>().PlayForward();
		if (!enemy.StateData.ContainsKey(this))
		{
			enemy.StateData.Add(this, new MyData());
		}
		MyData myData = enemy.StateData[this] as MyData;
		if (enemy.IsMasterPlayer)
		{
			myData.Postition = Vector3.zero;
			float num = UnityEngine.Random.Range(0f, 360f);
			float num2 = UnityEngine.Random.Range(Myth370Parameter.TeleportDistance.x, Myth370Parameter.TeleportDistance.y);
			for (int i = 0; i < 4; i++)
			{
				float f = (num + 90f * (float)i) * ((float)Math.PI / 180f);
				Vector3 vector = enemy.GetTargetPosition() + num2 * new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
				Ray ray = new Ray(vector + Vector3.up * 2f, Vector3.down);
				int layerMask = 1 << PhysicsLayer.FLOOR;
				RaycastHit hitInfo = default(RaycastHit);
				if (Physics.Raycast(ray, out hitInfo, 4f, layerMask))
				{
					Vector3 vector2 = hitInfo.point + new Vector3(0f, 0.2f, 0f);
					ray = new Ray(enemy.GetTargetPosition() + new Vector3(0f, 0.5f, 0f), vector2 - enemy.GetTargetPosition());
					layerMask = (1 << PhysicsLayer.WALL_AGAINST_PALYER) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.DESTRUCTIBLE);
					float magnitude = (vector2 - enemy.GetTargetPosition()).magnitude;
					if (!Physics.Raycast(ray, magnitude, layerMask))
					{
						myData.Postition = vector2;
						break;
					}
				}
			}
			if (myData.Postition != Vector3.zero && GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				CreatureTeleportRequest request = new CreatureTeleportRequest(enemy.EnemyID, myData.Postition);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
		else
		{
			enemy.SetRemoteEnemyMove(false);
		}
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		if (enemy.IsAnimationTriggered(0.75f))
		{
			MyData myData = enemy.StateData[this] as MyData;
			if (myData.Postition != Vector3.zero)
			{
				enemy.GetTransform().position = myData.Postition;
			}
			if (!enemy.IsMasterPlayer)
			{
				enemy.SetRemoteEnemyMove(true);
			}
			GameObject gameObject = EffectPool.GetInstance().GetEffectPool(EffectPoolType.VD5_FLASH).CreateObject(enemy.GetTransform().position + Vector3.up * 1.2f, enemy.GetTransform().forward, enemy.GetTransform().rotation);
			gameObject.transform.localScale = Vector3.one;
		}
	}

	public override void End(Enemy enemy)
	{
		if (!enemy.IsMasterPlayer)
		{
			enemy.SetRemoteEnemyMove(true);
		}
		if (enemy.GetTransform().Find("Myth370") != null)
		{
			enemy.GetTransform().Find("Myth370").gameObject.GetComponent<Renderer>().enabled = true;
		}
		enemy.GetTransform().Find(EffectName.E5_Shield).gameObject.GetComponent<TweenScale>().PlayReverse();
	}
}
