using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
	private static BulletScript mInstance;

	private static BulletRay mBulletRay;

	private static BulletProjectile mBulletProjectile;

	private static GameUnit mOwner;

	public static BulletScript CreateBullet(BulletType bulletType, GameUnit owner = null)
	{
		BulletScript result = null;
		mOwner = owner;
		switch (bulletType)
		{
		case BulletType.Ray:
			if (mBulletRay == null)
			{
				GameObject gameObject2 = null;
				Object original2 = Resources.Load("Bullet/BulletRay");
				gameObject2 = Object.Instantiate(original2) as GameObject;
				if (gameObject2 != null)
				{
					Object.DontDestroyOnLoad(gameObject2);
					mBulletRay = gameObject2.GetComponent<BulletRay>();
				}
			}
			result = mBulletRay;
			break;
		case BulletType.Projectile:
			if (mBulletProjectile == null)
			{
				GameObject gameObject = null;
				Object original = Resources.Load("Bullet/BulletProjectile");
				gameObject = Object.Instantiate(original) as GameObject;
				if (gameObject != null)
				{
					Object.DontDestroyOnLoad(gameObject);
					mBulletProjectile = gameObject.GetComponent<BulletProjectile>();
				}
			}
			result = mBulletProjectile;
			break;
		}
		return result;
	}

	public void DamageDetect(DamageProperty damageProperty, List<Collider> colliders, List<Vector3> hitPoints, int criticalRate, ObjectPool sparksObjectPool, float range, float damageDeductRate, float pvpDamageRate)
	{
		if (damageProperty.wType == WeaponType.AssaultRifle && damageProperty.isLocal)
		{
			GameApp.GetInstance().GetUserState().OperInfo.AddAssualtShootCount(1);
		}
		Hashtable hashtable = new Hashtable();
		Hashtable hashtable2 = new Hashtable();
		Hashtable hashtable3 = new Hashtable();
		Hashtable hashtable4 = new Hashtable();
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		LocalPlayer localPlayer = gameWorld.GetLocalPlayer();
		for (int i = 0; i < colliders.Count; i++)
		{
			if (sparksObjectPool != null && i == 0)
			{
				sparksObjectPool.CreateObject(hitPoints[i], Vector3.one, Quaternion.identity);
			}
			int num = 0;
			if (GameApp.GetInstance().GetGameMode().IsVSMode() && colliders[i].gameObject.layer == PhysicsLayer.REMOTE_PLAYER)
			{
				if (colliders[i].gameObject.tag == TagName.PLAYER_ROOT)
				{
					num = int.Parse(colliders[i].gameObject.name);
				}
				else if (colliders[i].gameObject.tag == TagName.VEHICLE_ROOT)
				{
					num = 0;
					for (int j = 0; j < colliders[i].gameObject.transform.childCount; j++)
					{
						if (colliders[i].gameObject.transform.GetChild(j).tag == TagName.PLAYER_ROOT)
						{
							num = int.Parse(colliders[i].gameObject.transform.GetChild(j).name);
							break;
						}
					}
				}
				if (GameApp.GetInstance().GetGameMode().IsTeamMode())
				{
					Player remotePlayerByUserID = gameWorld.GetRemotePlayerByUserID(num);
					if (remotePlayerByUserID == null || remotePlayerByUserID.IsSameTeam(localPlayer))
					{
						continue;
					}
				}
			}
			if (!(colliders[i] != null))
			{
				continue;
			}
			if (colliders[i].gameObject.layer == PhysicsLayer.ENEMY)
			{
				GameObject rootObjectByCollider = Enemy.GetRootObjectByCollider(colliders[i]);
				if (rootObjectByCollider.tag == TagName.ENEMY_ROOT)
				{
					Enemy enemyByID = GameApp.GetInstance().GetGameWorld().GetEnemyByID(rootObjectByCollider.name);
					if (!enemyByID.InPlayingState())
					{
						continue;
					}
					float num2 = Vector3.Distance(enemyByID.GetTransform().position, localPlayer.GetTransform().position);
					if (num2 > range)
					{
						damageProperty.damage = (int)((float)damageProperty.damage * (1f - (num2 - range) * damageDeductRate));
					}
					if (damageProperty.damage <= 0)
					{
						continue;
					}
					if (hashtable.Contains(enemyByID.EnemyID))
					{
						if (enemyByID.IsBoss())
						{
							if (damageProperty.wType == WeaponType.ShotGun || damageProperty.wType == WeaponType.LaserShotGun)
							{
								if (hashtable4.Contains(enemyByID.EnemyID))
								{
									hashtable4[enemyByID.EnemyID] = (int)hashtable4[enemyByID.EnemyID] + 1;
								}
								else
								{
									hashtable4.Add(enemyByID.EnemyID, 1);
								}
								if ((int)hashtable4[enemyByID.EnemyID] <= 3)
								{
									hashtable[enemyByID.EnemyID] = (int)hashtable[enemyByID.EnemyID] + damageProperty.damage;
								}
							}
							else
							{
								hashtable[enemyByID.EnemyID] = (int)hashtable[enemyByID.EnemyID] + damageProperty.damage;
							}
						}
						else
						{
							hashtable[enemyByID.EnemyID] = (int)hashtable[enemyByID.EnemyID] + damageProperty.damage;
						}
					}
					else
					{
						hashtable.Add(enemyByID.EnemyID, damageProperty.damage);
						if (damageProperty.wType == WeaponType.AssaultRifle && damageProperty.isLocal)
						{
							GameApp.GetInstance().GetUserState().OperInfo.AddAssualtHitCount(1);
						}
					}
				}
				else
				{
					if (!(rootObjectByCollider.tag == TagName.PLAYER_ROOT))
					{
						continue;
					}
					num = int.Parse(rootObjectByCollider.name);
					if (!GameApp.GetInstance().GetGameMode().IsVSMode())
					{
						continue;
					}
					if (hashtable2.Contains(num))
					{
						if (damageProperty.wType == WeaponType.ShotGun || damageProperty.wType == WeaponType.LaserShotGun)
						{
							damageProperty.damageFactor++;
						}
						continue;
					}
					hashtable2.Add(num, damageProperty.damage);
					if (damageProperty.wType == WeaponType.AssaultRifle && damageProperty.isLocal)
					{
						GameApp.GetInstance().GetUserState().OperInfo.AddAssualtHitCount(1);
					}
				}
			}
			else if (colliders[i].gameObject.layer == PhysicsLayer.REMOTE_PLAYER)
			{
				if (!GameApp.GetInstance().GetGameMode().IsVSMode())
				{
					continue;
				}
				if (hashtable2.Contains(num))
				{
					if (damageProperty.wType == WeaponType.ShotGun || damageProperty.wType == WeaponType.LaserShotGun)
					{
						damageProperty.damageFactor++;
					}
					continue;
				}
				hashtable2.Add(num, damageProperty.damage);
				if (damageProperty.wType == WeaponType.AssaultRifle && damageProperty.isLocal)
				{
					GameApp.GetInstance().GetUserState().OperInfo.AddAssualtHitCount(1);
				}
			}
			else if (colliders[i].gameObject.layer == PhysicsLayer.DESTRUCTIBLE)
			{
				GameObject gameObject = colliders[i].gameObject;
				while (gameObject.tag != TagName.DESTRUCTIBLE && gameObject.tag != TagName.FUNNEL)
				{
					if (gameObject.transform.parent == null)
					{
						gameObject = null;
						break;
					}
					gameObject = gameObject.transform.parent.gameObject;
				}
				if (!(gameObject != null))
				{
					continue;
				}
				if (hashtable3.Contains(gameObject))
				{
					hashtable3[gameObject] = (int)hashtable3[gameObject] + damageProperty.damage;
					continue;
				}
				hashtable3.Add(gameObject, damageProperty.damage);
				if (damageProperty.wType == WeaponType.AssaultRifle && damageProperty.isLocal)
				{
					GameApp.GetInstance().GetUserState().OperInfo.AddAssualtHitCount(1);
				}
			}
			else if (colliders[i].gameObject.layer == PhysicsLayer.VOID_SHIELD)
			{
				ShieldScript component = colliders[i].transform.root.GetComponent<ShieldScript>();
				if (!localPlayer.IsFriendForce(component.ownedPlayer) && localPlayer != component.ownedPlayer)
				{
					component.PlayerHitShield(damageProperty);
				}
			}
		}
		foreach (short key in hashtable.Keys)
		{
			Enemy enemyByID2 = GameApp.GetInstance().GetGameWorld().GetEnemyByID("E_" + key);
			damageProperty.damage = (int)hashtable[key];
			bool criticalAttack = false;
			int num4 = Random.Range(0, 100);
			if (num4 < 60)
			{
				criticalAttack = true;
			}
			damageProperty.hitpoint = enemyByID2.GetTransform().position + Vector3.up;
			damageProperty.criticalAttack = criticalAttack;
			if (localPlayer.GetWeapon().GetWeaponType() == WeaponType.Myth370Fist)
			{
				float num5 = Vector3.Distance(enemyByID2.GetPosition(), localPlayer.GetPosition());
				damageProperty.buff += Mathf.Clamp(num5 - Global.MYTH_FIST_INIT_DIS, 0f, 255f) * Global.MYTH_FIST_DIS_FACTOR;
			}
			enemyByID2.HitEnemy(localPlayer, damageProperty);
			if (mOwner == localPlayer)
			{
				localPlayer.SetLastHitTarget(enemyByID2);
				if (damageProperty.wType == WeaponType.MachineGun)
				{
					MachineGun machineGun = localPlayer.GetWeapon() as MachineGun;
					machineGun.SetHitTarget(enemyByID2);
				}
				else if (damageProperty.wType == WeaponType.ChargeDoublePistol)
				{
					ChargeDoublePistol chargeDoublePistol = localPlayer.GetWeapon() as ChargeDoublePistol;
					chargeDoublePistol.AddStack();
					chargeDoublePistol.AttackTarget(enemyByID2, damageProperty.hitpoint);
				}
			}
		}
		foreach (int key2 in hashtable2.Keys)
		{
			RemotePlayer remotePlayerByUserID2 = GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(key2);
			int num7 = (int)((float)(int)hashtable2[key2] * pvpDamageRate);
			byte playerHitPlayerDistanceId = 0;
			if (remotePlayerByUserID2 != null && localPlayer != null)
			{
				if (!remotePlayerByUserID2.InPlayingState())
				{
					continue;
				}
				float magnitude = (remotePlayerByUserID2.GetTransform().position - localPlayer.GetTransform().position).magnitude;
				if (magnitude > (float)DamageProperty.PLAYER_LONG_HIT_PLAYER_DISTANCE)
				{
					playerHitPlayerDistanceId = 1;
				}
				if (damageProperty.wType == WeaponType.Myth370Fist)
				{
					damageProperty.damageFactor = (byte)Mathf.Clamp((magnitude - Global.MYTH_FIST_INIT_DIS) / 2f, 0f, 49f);
				}
				if (remotePlayerByUserID2 == damageProperty.directHitUnit)
				{
					if (damageProperty.wType == WeaponType.Myth370Fist)
					{
						damageProperty.damageFactor += 50;
					}
					else if (damageProperty.wType == WeaponType.SplitRPG)
					{
						damageProperty.damageFactor += 2;
					}
					else
					{
						damageProperty.damageFactor++;
					}
				}
			}
			if (damageProperty.mAttackerType == AttackerType.PLAYER || damageProperty.mAttackerType == AttackerType.VEHICLE)
			{
				localPlayer.HitTargetShowCrossHit();
			}
			else if (damageProperty.mAttackerType == AttackerType.SW_VOID)
			{
				localPlayer.HitTargetShowCrossHit(true);
			}
			PlayerHitPlayerRequest request = new PlayerHitPlayerRequest(damageProperty, key2, playerHitPlayerDistanceId);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			if (mOwner == localPlayer)
			{
				if (damageProperty.wType != WeaponType.NoGun)
				{
					localPlayer.HitEnemy();
				}
				localPlayer.SetLastHitTarget(remotePlayerByUserID2);
				if (damageProperty.wType == WeaponType.MachineGun)
				{
					MachineGun machineGun2 = localPlayer.GetWeapon() as MachineGun;
					machineGun2.SetHitTarget(remotePlayerByUserID2);
				}
				else if (damageProperty.wType == WeaponType.ChargeDoublePistol)
				{
					ChargeDoublePistol chargeDoublePistol2 = localPlayer.GetWeapon() as ChargeDoublePistol;
					chargeDoublePistol2.AddStack();
					chargeDoublePistol2.AttackTarget(remotePlayerByUserID2, damageProperty.hitpoint);
				}
			}
		}
		foreach (GameObject key3 in hashtable3.Keys)
		{
			if (key3.tag == TagName.FUNNEL)
			{
				CrimsonQueen instance = CrimsonQueen.Instance;
				if (instance != null)
				{
					damageProperty.damage = (int)hashtable3[key3];
					Funnel funnel = instance.GetFunnel(key3.name);
					funnel.OnGeneralHit(localPlayer, damageProperty);
				}
			}
			else
			{
				DestructibleObjectScript destructibleByName = GameApp.GetInstance().GetGameWorld().GetDestructibleByName(key3);
				if (null != destructibleByName)
				{
					destructibleByName.OnHit((int)hashtable3[key3]);
				}
			}
		}
	}
}
