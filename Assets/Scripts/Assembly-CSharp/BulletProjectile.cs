using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : BulletScript
{
	public void ExplosionDetect(DamageProperty damageProperty, Collider touched, bool isPenerating, int criticalRate, HashSet<PenerateHitInfo> penetratingTargets, Vector3 explosionPosition, float explosionRadius, bool penerateExplode, bool ignoreShield = false)
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		Player localPlayer = gameWorld.GetLocalPlayer();
		List<Collider> list = new List<Collider>();
		List<Vector3> list2 = new List<Vector3>();
		HashSet<object> hashSet = new HashSet<object>();
		bool flag = true;
		if (isPenerating && touched != null)
		{
			int num = -1;
			Vector3 vector = default(Vector3);
			if (touched.gameObject.layer == PhysicsLayer.ENEMY)
			{
				GameObject rootObjectByCollider = Enemy.GetRootObjectByCollider(touched);
				if (rootObjectByCollider.tag == TagName.ENEMY_ROOT)
				{
					Enemy enemyByID = GameApp.GetInstance().GetGameWorld().GetEnemyByID(rootObjectByCollider.name);
					if (enemyByID != null)
					{
						num = enemyByID.EnemyID;
						vector = enemyByID.GetTransform().position;
						hashSet.Add(enemyByID);
					}
				}
				else if (rootObjectByCollider.tag == TagName.PLAYER_ROOT)
				{
					RemotePlayer remotePlayerByUserID = gameWorld.GetRemotePlayerByUserID(int.Parse(rootObjectByCollider.name));
					if (remotePlayerByUserID != null && GameApp.GetInstance().GetGameMode().IsVSMode())
					{
						num = remotePlayerByUserID.GetChannelID();
						vector = remotePlayerByUserID.GetTransform().position;
						hashSet.Add(remotePlayerByUserID);
					}
				}
			}
			else if (touched.gameObject.layer == PhysicsLayer.REMOTE_PLAYER)
			{
				RemotePlayer remotePlayerByUserID2 = gameWorld.GetRemotePlayerByUserID(int.Parse(touched.gameObject.name));
				if (remotePlayerByUserID2 != null && GameApp.GetInstance().GetGameMode().IsVSMode())
				{
					num = remotePlayerByUserID2.GetChannelID();
					vector = remotePlayerByUserID2.GetTransform().position;
					hashSet.Add(remotePlayerByUserID2);
				}
			}
			else if (touched.gameObject.layer == PhysicsLayer.DESTRUCTIBLE)
			{
				GameObject gameObject = touched.gameObject;
				while (gameObject.tag != TagName.DESTRUCTIBLE && gameObject.tag != TagName.FUNNEL)
				{
					if (gameObject.transform.parent == null)
					{
						gameObject = null;
						break;
					}
					gameObject = gameObject.transform.parent.gameObject;
				}
				if (gameObject != null)
				{
					if (gameObject.tag == TagName.FUNNEL)
					{
						num = 0;
						vector = gameObject.transform.position;
						hashSet.Add(gameObject);
					}
					else
					{
						DestructibleObjectScript destructibleByName = GameApp.GetInstance().GetGameWorld().GetDestructibleByName(gameObject);
						if (null != destructibleByName)
						{
							num = destructibleByName.ID;
							vector = gameObject.transform.position;
							hashSet.Add(gameObject);
						}
					}
				}
			}
			else if (touched.gameObject.layer == PhysicsLayer.VOID_SHIELD)
			{
				ShieldScript component = touched.transform.root.GetComponent<ShieldScript>();
				if (!localPlayer.IsFriendForce(component.ownedPlayer) && localPlayer != component.ownedPlayer)
				{
					component.PlayerHitShield(damageProperty);
				}
			}
			if (num != -1)
			{
				PenerateHitInfo penerateHitInfo = null;
				foreach (PenerateHitInfo penetratingTarget in penetratingTargets)
				{
					if (num == penetratingTarget.enemyID)
					{
						penerateHitInfo = penetratingTarget;
						break;
					}
				}
				bool flag2 = false;
				if (penerateHitInfo == null)
				{
					flag2 = true;
					penetratingTargets.Add(new PenerateHitInfo(num, Time.time));
				}
				else if (penerateHitInfo.CouldHit())
				{
					flag2 = true;
				}
				if (flag2)
				{
					list.Add(touched);
					list2.Add(vector + Vector3.up * 2f);
				}
			}
			if (!penerateExplode)
			{
				flag = false;
			}
		}
		if (flag && explosionRadius > 0f)
		{
			Collider[] array = Physics.OverlapSphere(explosionPosition, explosionRadius, (1 << PhysicsLayer.ENEMY) | (1 << PhysicsLayer.DESTRUCTIBLE));
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				Ray ray = new Ray(explosionPosition, collider.transform.position - explosionPosition);
				float distance = Mathf.Sqrt((explosionPosition - collider.transform.position).sqrMagnitude);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, distance, (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.TRANSPARENT_WALL) | (1 << PhysicsLayer.ENEMY) | (1 << PhysicsLayer.DESTRUCTIBLE)) && hitInfo.collider.gameObject.layer != PhysicsLayer.ENEMY && hitInfo.collider.gameObject.layer != PhysicsLayer.DESTRUCTIBLE)
				{
					continue;
				}
				if (collider.gameObject.layer == PhysicsLayer.ENEMY)
				{
					GameObject rootObjectByCollider2 = Enemy.GetRootObjectByCollider(collider);
					if (rootObjectByCollider2.tag == TagName.ENEMY_ROOT)
					{
						Enemy enemyByID2 = GameApp.GetInstance().GetGameWorld().GetEnemyByID(rootObjectByCollider2.name);
						if (enemyByID2 != null && !hashSet.Contains(enemyByID2))
						{
							hashSet.Add(enemyByID2);
							list.Add(collider);
							list2.Add(enemyByID2.GetTransform().position + Vector3.up * 2f);
						}
					}
					else if (rootObjectByCollider2.tag == TagName.PLAYER_ROOT)
					{
						RemotePlayer remotePlayerByUserID3 = gameWorld.GetRemotePlayerByUserID(int.Parse(rootObjectByCollider2.name));
						if (remotePlayerByUserID3 != null && GameApp.GetInstance().GetGameMode().IsVSMode())
						{
							hashSet.Add(remotePlayerByUserID3);
							list.Add(collider);
							list2.Add(remotePlayerByUserID3.GetTransform().position + Vector3.up * 2f);
						}
					}
				}
				else
				{
					if (collider.gameObject.layer != PhysicsLayer.DESTRUCTIBLE)
					{
						continue;
					}
					GameObject gameObject2 = collider.gameObject;
					while (gameObject2.tag != TagName.DESTRUCTIBLE && gameObject2.tag != TagName.FUNNEL)
					{
						if (gameObject2.transform.parent == null)
						{
							gameObject2 = null;
							break;
						}
						gameObject2 = gameObject2.transform.parent.gameObject;
					}
					if (gameObject2 != null)
					{
						DestructibleObjectScript destructibleByName2 = GameApp.GetInstance().GetGameWorld().GetDestructibleByName(gameObject2);
						if ((null != destructibleByName2 || gameObject2.tag == TagName.FUNNEL) && !hashSet.Contains(gameObject2))
						{
							hashSet.Add(gameObject2);
							list.Add(collider);
							list2.Add(gameObject2.transform.position + Vector3.up * 2f);
						}
					}
				}
			}
			if (GameApp.GetInstance().GetGameMode().IsVSMode())
			{
				List<RemotePlayer> remotePlayers = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
				foreach (RemotePlayer item in remotePlayers)
				{
					if (item.IsBoss || !item.InPlayingState())
					{
						continue;
					}
					PenerateHitInfo penerateHitInfo2 = null;
					foreach (PenerateHitInfo penetratingTarget2 in penetratingTargets)
					{
						if (item.GetChannelID() == penetratingTarget2.enemyID)
						{
							penerateHitInfo2 = penetratingTarget2;
							break;
						}
					}
					bool flag3 = false;
					if (penerateHitInfo2 == null)
					{
						flag3 = true;
						penetratingTargets.Add(new PenerateHitInfo(item.GetChannelID(), Time.time));
					}
					else if (penerateHitInfo2.CouldHit())
					{
						flag3 = true;
					}
					if (!flag3)
					{
						continue;
					}
					if (GameApp.GetInstance().GetGameMode().IsTeamMode())
					{
						Player remotePlayerByUserID4 = gameWorld.GetRemotePlayerByUserID(item.GetChannelID());
						if (remotePlayerByUserID4 == null || remotePlayerByUserID4.IsSameTeam(localPlayer))
						{
							continue;
						}
					}
					Vector3 vector2 = item.GetTransform().position + new Vector3(0f, 1f, 0f);
					float sqrMagnitude = (vector2 - explosionPosition).sqrMagnitude;
					float num2 = explosionRadius * explosionRadius;
					if (!(sqrMagnitude < num2))
					{
						continue;
					}
					Ray ray2 = new Ray(explosionPosition, vector2 - explosionPosition);
					RaycastHit hitInfo2;
					if (!ignoreShield)
					{
						if (Physics.Raycast(ray2, out hitInfo2, Mathf.Sqrt(sqrMagnitude), (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.TRANSPARENT_WALL) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.REMOTE_PLAYER) | (1 << PhysicsLayer.VOID_SHIELD)))
						{
							if (hitInfo2.collider.gameObject.layer == PhysicsLayer.REMOTE_PLAYER)
							{
								list.Add(hitInfo2.collider);
								list2.Add(hitInfo2.point);
							}
							else if (hitInfo2.collider.gameObject.layer == PhysicsLayer.VOID_SHIELD)
							{
								list.Add(hitInfo2.collider);
								list2.Add(hitInfo2.point);
							}
						}
						else
						{
							list.Add(item.GetCollider());
							list2.Add(explosionPosition);
						}
					}
					else if (Physics.Raycast(ray2, out hitInfo2, Mathf.Sqrt(sqrMagnitude), (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.TRANSPARENT_WALL) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.REMOTE_PLAYER)))
					{
						if (hitInfo2.collider.gameObject.layer == PhysicsLayer.REMOTE_PLAYER)
						{
							list.Add(hitInfo2.collider);
							list2.Add(hitInfo2.point);
						}
					}
					else
					{
						list.Add(item.GetCollider());
						list2.Add(explosionPosition);
					}
				}
			}
		}
		DamageDetect(damageProperty, list, list2, criticalRate, null, 1000f, 0f, 1f);
	}
}
