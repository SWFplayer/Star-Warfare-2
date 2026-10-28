using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitRPG : TrackingRPG
{
	private int trackingID;

	private float PVETrackingRadius = 30f;

	private float PVPTrackingRadius = 8f;

	private float ForwardAngle = 60f;

	public override WeaponType GetWeaponType()
	{
		return WeaponType.SplitRPG;
	}

	public override void Init(Player player)
	{
		base.Init(player);
		if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.BOSS_FINAL || GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
		{
			PVETrackingRadius *= 2f;
		}
	}

	public override void CreateTrajectoryUI(GameObject avatar, GameObject gunObj, GameObject gunObjL)
	{
		AudioManager.GetInstance().PlaySoundAt(base.FireSoundName, avatar.transform.position);
		if (gunObj.GetComponent<Animation>() != null && gunObj.GetComponent<Animation>()["open"] != null)
		{
			gunObj.GetComponent<Animation>().Play("open");
		}
		gunfire = GetGunFire(gunObj);
		Vector3 normalized = gunfire.transform.forward.normalized;
		GameObject gameObject = EffectPool.GetInstance().GetEffectPool(EffectPoolType.W28_ROCKET).CreateObject(gunfire.position, Vector3.zero, gunfire.rotation);
		gameObject.GetComponent<ProjectileScript>().enabled = false;
		FireLineScript component = gameObject.GetComponent<FireLineScript>();
		component.enabled = true;
		component.beginPos = gunfire.position;
		component.endPos = gunfire.position + normalized * 100f;
		AutoDestroyScript component2 = gameObject.GetComponent<AutoDestroyScript>();
		component2.enabled = true;
		component2.life = 1f;
		component2.type = AutoDestroyType.UNABLE;
		component2.destroySound = base.ExplodeSoundName;
	}

	public override void Attack(float deltaTime)
	{
		lastAttackTime = Time.time;
		AudioManager.GetInstance().PlaySound(base.FireSoundName);
		PlayAttackAnimation();
		GameApp.GetInstance().GetUserState().UseEnegy(enegyConsume);
		if (aas != null)
		{
			aas.StartAnimation();
		}
		Vector3 direction;
		Ray weaponRay = GetWeaponRay(gunfire, out direction);
		GameObject gameObject = gameWorld.GetEffectPool(EffectPoolType.W28_ROCKET).CreateObject(gunfire.position, direction, Quaternion.identity);
		AutoDestroyScript component = gameObject.GetComponent<AutoDestroyScript>();
		if (component != null)
		{
			component.enabled = false;
		}
		FireLineScript component2 = gameObject.GetComponent<FireLineScript>();
		if (component2 != null)
		{
			component2.enabled = false;
		}
		ProjectileScript component3 = gameObject.GetComponent<ProjectileScript>();
		component3.enabled = true;
		component3.isLocal = true;
		component3.dir = direction;
		component3.flySpeed = bulletSpeed;
		component3.explodeRadius = GetBombRange();
		component3.hitForce = hitForce;
		component3.life = 10f;
		component3.damage = (int)damage;
		component3.GunType = WeaponType.SplitRPG;
		component3.targetPos = aimTarget;
		component3.weapon = this;
		component3.isSpliting = true;
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			PlayerFireRocketRequest request = new PlayerFireRocketRequest(gunID, gunfire.position, direction);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
	}

	public override void Loop(float deltaTime)
	{
	}

	public void Split(Vector3 position, Vector3 projectileDir)
	{
		if (player.GetSkills().HasSkill(SkillsType.ROCKET_SPLIT))
		{
			AudioManager.GetInstance().PlaySoundAt(base.FireSoundName, position);
			Vector3 vector = projectileDir + Vector3.up;
			Vector3 normalized = (Quaternion.Euler(0f, 90f, 0f) * vector).normalized;
			Vector3 normalized2 = (Quaternion.Euler(0f, -90f, 0f) * vector).normalized;
			GenerateSmallRPG(position, normalized, 1);
			GenerateSmallRPG(position, normalized2, 2);
		}
		else
		{
			AudioManager.GetInstance().PlaySoundAt(base.FireSoundName, position);
			Vector3 up = Vector3.up;
			GenerateSmallRPG(position, up, 1);
		}
	}

	public void GenerateSmallRPG(Vector3 position, Vector3 projectileDir, int index)
	{
		GameObject gameObject = null;
		switch (index)
		{
		case 1:
			gameObject = gameWorld.GetEffectPool(EffectPoolType.W28_ROCKET1).CreateObject(position, projectileDir, Quaternion.identity);
			break;
		case 2:
			gameObject = gameWorld.GetEffectPool(EffectPoolType.W28_ROCKET2).CreateObject(position, projectileDir, Quaternion.identity);
			break;
		}
		AutoDestroyScript component = gameObject.GetComponent<AutoDestroyScript>();
		if (component != null)
		{
			component.enabled = false;
		}
		FireLineScript component2 = gameObject.GetComponent<FireLineScript>();
		if (component2 != null)
		{
			component2.enabled = false;
		}
		ProjectileScript component3 = gameObject.GetComponent<ProjectileScript>();
		component3.enabled = true;
		component3.isLocal = true;
		component3.dir = projectileDir;
		component3.flySpeed = bulletSpeed;
		component3.explodeRadius = GetBombRange();
		component3.hitForce = hitForce;
		component3.life = 10f;
		component3.damage = (int)damage / 2;
		component3.GunType = WeaponType.SplitRPG;
		component3.targetPos = position + projectileDir * 2f;
		component3.weapon = this;
		component3.isSplited = true;
		trackingID = -1;
		if (GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			component3.trackingPlayer = GetNearestAimPlayer(position);
			if (component3.trackingPlayer != null)
			{
				trackingID = component3.trackingPlayer.GetChannelID();
			}
		}
		else
		{
			component3.trackingEnemy = GetNearestAimEnemy(position);
			if (component3.trackingEnemy != null)
			{
				trackingID = component3.trackingEnemy.EnemyID;
			}
		}
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			PlayerFireRocketRequest request = new PlayerFireRocketRequest(gunID, position, projectileDir, trackingID);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
	}

	private void PlayAttackAnimation()
	{
		if (gun.GetComponent<Animation>() != null && gun.GetComponent<Animation>()["open"] != null)
		{
			gun.GetComponent<Animation>().Play("open");
		}
	}

	public Enemy GetNearestAimEnemy(Vector3 position)
	{
		if (player.GameUnitType == EGameUnitType.ROBOT)
		{
			return null;
		}
		Enemy result = null;
		if (GameApp.GetInstance().GetGameMode().IsSingle() || GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			Hashtable enemies = gameWorld.GetEnemies();
			foreach (Enemy value in enemies.Values)
			{
				float pVETrackingRadius = PVETrackingRadius;
				Ray ray = default(Ray);
				Vector3 normalized = (value.GetColliderCenterPosition() - position).normalized;
				ray = new Ray(position, normalized);
				RaycastHit hitInfo = default(RaycastHit);
				if (Physics.Raycast(ray, out hitInfo, PVETrackingRadius, (1 << PhysicsLayer.ENEMY) | (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.FLOOR)) && hitInfo.collider.gameObject.layer == PhysicsLayer.ENEMY && Vector3.Angle(player.GetTransform().forward, value.GetPosition() - player.GetPosition()) < ForwardAngle)
				{
					float num = Vector3.Distance(position, value.GetPosition());
					if (num < pVETrackingRadius)
					{
						result = value;
						pVETrackingRadius = num;
					}
				}
			}
		}
		return result;
	}

	public Player GetNearestAimPlayer(Vector3 position)
	{
		if (player.GameUnitType == EGameUnitType.ROBOT)
		{
			return null;
		}
		Player result = null;
		if (GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			List<RemotePlayer> remotePlayers = gameWorld.GetRemotePlayers();
			foreach (RemotePlayer item in remotePlayers)
			{
				float pVPTrackingRadius = PVPTrackingRadius;
				Ray ray = default(Ray);
				Vector3 normalized = (item.GetPosition() + Vector3.up - position).normalized;
				ray = new Ray(position, normalized);
				RaycastHit hitInfo = default(RaycastHit);
				if (!Physics.Raycast(ray, out hitInfo, PVPTrackingRadius, (1 << PhysicsLayer.REMOTE_PLAYER) | (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.ENEMY)))
				{
					continue;
				}
				if (hitInfo.collider.gameObject.layer == PhysicsLayer.REMOTE_PLAYER)
				{
					if (Vector3.Angle(player.GetTransform().forward, item.GetPosition() - player.GetPosition()) < ForwardAngle && !item.IsFriendForce(player))
					{
						float num = Vector3.Distance(position, item.GetPosition());
						if (num < pVPTrackingRadius)
						{
							result = item;
							pVPTrackingRadius = num;
						}
					}
				}
				else
				{
					if (hitInfo.collider.gameObject.layer != PhysicsLayer.ENEMY)
					{
						continue;
					}
					GameObject rootObjectByCollider = Enemy.GetRootObjectByCollider(hitInfo.collider);
					if (rootObjectByCollider.tag == TagName.PLAYER_ROOT && Vector3.Angle(player.GetTransform().forward, item.GetPosition() - player.GetPosition()) < ForwardAngle)
					{
						float num2 = Vector3.Distance(position, item.GetPosition());
						if (num2 < pVPTrackingRadius)
						{
							result = item;
							pVPTrackingRadius = num2;
						}
					}
				}
			}
		}
		return result;
	}
}
