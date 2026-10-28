using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingRPG : Weapon
{
	protected const float shootLastingTime = 0.5f;

	protected static int sbulletCount;

	private GameObject trajectoryObject;

	protected float life = 5f;

	protected AlphaAnimationScript aas;

	public TrackingRPG()
	{
		base.IsSelectedForBattle = false;
		base.FireSoundName = AudioName.GUN_03_RPG;
		base.ExplodeSoundName = AudioName.GUN_03_RPG_BOOM;
	}

	public override WeaponType GetWeaponType()
	{
		return WeaponType.TrackingRPG;
	}

	public override string GetAnimationSuffix()
	{
		return "RPG";
	}

	public override string GetAnimationSuffixAlter()
	{
		return "RPG";
	}

	public override bool IsTypeOfLoopShootingWeapon()
	{
		return false;
	}

	public override EAimType GetIdleAimType()
	{
		return EAimType.ANGLE;
	}

	public override EAimType GetAttackAimType()
	{
		return EAimType.ANGLE;
	}

	public override void IdleZoom(float deltaTime)
	{
		GameApp.GetInstance().GetGameWorld().GetCamera()
			.ZoomToRPG(deltaTime);
	}

	public override void AttackZoom(float deltaTime)
	{
		GameApp.GetInstance().GetGameWorld().GetCamera()
			.ZoomToRPG(deltaTime);
	}

	public override void CreateTrajectory()
	{
		base.CreateTrajectory();
		AudioManager.GetInstance().PlaySoundAt(base.FireSoundName, player.GetTransform().position);
	}

	public override void Init(Player player)
	{
		base.Init(player);
		aas = gun.GetComponent<AlphaAnimationScript>();
	}

	public override void UIInit(GameObject weaponObj, GameObject weaponObjL)
	{
		base.UIInit(weaponObj, weaponObjL);
	}

	public override void CreateTrajectoryUI(GameObject avatar, GameObject gunObj, GameObject gunObjL)
	{
		AudioManager.GetInstance().PlaySoundAt(base.FireSoundName, avatar.transform.position);
		gunfire = GetGunFire(gunObj);
		Vector3 normalized = gunfire.transform.forward.normalized;
		trajectoryObject = EffectPool.GetInstance().GetEffectPool(EffectPoolType.W3_ROCKET).CreateObject(gunfire.position, Vector3.zero, gunfire.rotation);
		trajectoryObject.GetComponent<ProjectileScript>().enabled = false;
		FireLineScript component = trajectoryObject.GetComponent<FireLineScript>();
		component.enabled = true;
		component.beginPos = gunfire.position;
		component.endPos = gunfire.position + normalized * 100f;
		AutoDestroyScript component2 = trajectoryObject.GetComponent<AutoDestroyScript>();
		component2.enabled = true;
		component2.life = 1f;
		component2.type = AutoDestroyType.UNABLE;
		component2.destroySound = base.ExplodeSoundName;
	}

	public override void GunOn()
	{
		base.GunOn();
		PlayIdleAnimGunOn();
	}

	public Player GetNearestAimPlayer()
	{
		Player result = null;
		List<RemotePlayer> remotePlayers = gameWorld.GetRemotePlayers();
		Vector2 b = new Vector2((float)Screen.width / 2f, (float)Screen.height / 2f);
		float num = 1E+10f;
		foreach (RemotePlayer item in remotePlayers)
		{
			if (item == null || (GameApp.GetInstance().GetGameMode().IsTeamMode() && item.IsSameTeam(player)))
			{
				continue;
			}
			Vector3 vector = Camera.main.WorldToScreenPoint(item.GetTransform().position);
			Vector2 a = new Vector2(vector.x, vector.y);
			if (!(a.x > 0f) || !(a.x < (float)Screen.width) || !(a.y > 0f) || !(a.y < (float)Screen.height) || !(vector.z > 0f))
			{
				continue;
			}
			Camera main = Camera.main;
			Transform transform = main.transform;
			ThirdPersonStandardCameraScript component = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
			Ray ray = default(Ray);
			Vector3 normalized = (item.GetTransform().position + Vector3.up * 1f - transform.position).normalized;
			ray = new Ray(transform.position + normalized * 1.8f, normalized);
			RaycastHit hitInfo = default(RaycastHit);
			if (!Physics.Raycast(ray, out hitInfo, 1000f, (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.TRANSPARENT_WALL) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.REMOTE_PLAYER)) || hitInfo.collider.gameObject.layer == PhysicsLayer.REMOTE_PLAYER)
			{
				float num2 = Vector2.Distance(a, b);
				if (num2 < num)
				{
					num = num2;
					result = item;
				}
			}
		}
		return result;
	}

	public Enemy GetNearestAimEnemy()
	{
		if (player.GameUnitType == EGameUnitType.ROBOT)
		{
			return null;
		}
		Enemy result = null;
		if (GameApp.GetInstance().GetGameMode().IsSingle() || GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			Hashtable enemies = gameWorld.GetEnemies();
			object[] array = new object[enemies.Count];
			enemies.Keys.CopyTo(array, 0);
			Vector2 b = new Vector2((float)Screen.width / 2f, (float)Screen.height / 2f);
			float num = 1E+10f;
			for (int i = 0; i < array.Length; i++)
			{
				Enemy enemy = enemies[array[i]] as Enemy;
				Vector3 vector = Camera.main.WorldToScreenPoint(enemy.GetTransform().position);
				Vector2 a = new Vector2(vector.x, vector.y);
				if (!(a.x > 0f) || !(a.x < (float)Screen.width) || !(a.y > 0f) || !(a.y < (float)Screen.height) || !(vector.z > 0f))
				{
					continue;
				}
				Camera main = Camera.main;
				Transform transform = main.transform;
				ThirdPersonStandardCameraScript component = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
				Ray ray = default(Ray);
				Vector3 normalized = (enemy.GetColliderCenterPosition() - transform.position).normalized;
				ray = new Ray(transform.position + normalized * 1.8f, normalized);
				RaycastHit hitInfo = default(RaycastHit);
				if (!Physics.Raycast(ray, out hitInfo, 1000f, (1 << PhysicsLayer.ENEMY) | (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.TRANSPARENT_WALL) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.REMOTE_PLAYER)) || hitInfo.collider.gameObject.layer == PhysicsLayer.ENEMY)
				{
					float num2 = Vector2.Distance(a, b);
					if (num2 < num)
					{
						num = num2;
						result = enemy;
					}
				}
			}
		}
		return result;
	}

	public override void Loop(float deltaTime)
	{
		if (!player.IsLocal())
		{
			return;
		}
		string text = ((!(player.GetSkills().GetSkill(SkillsType.FLY) > 0f) || 1 == 0) ? (AnimationString.Stand + "_" + GetAnimationSuffix() + "_") : (AnimationString.Fly + "_" + GetAnimationSuffix() + "_"));
		if (attacked || (!player.AnimationPlayed(text + AnimationString.Attack, 0.1f) && !player.AnimationPlayed(text + AnimationString.RunAttack, 0.1f)))
		{
			return;
		}
		GameApp.GetInstance().GetUserState().UseEnegy(enegyConsume);
		if (aas != null)
		{
			aas.StartAnimation();
		}
		Vector3 direction;
		Ray weaponRay = GetWeaponRay(gunfire, out direction);
		GameObject gameObject = gameWorld.GetEffectPool(EffectPoolType.W3_ROCKET).CreateObject(gunfire.position, direction, Quaternion.identity);
		gameObject.GetComponent<AutoDestroyScript>().enabled = false;
		gameObject.GetComponent<FireLineScript>().enabled = false;
		ProjectileScript component = gameObject.GetComponent<ProjectileScript>();
		component.dir = direction;
		component.flySpeed = bulletSpeed;
		component.explodeRadius = GetBombRange();
		component.hitForce = hitForce;
		component.life = life;
		component.damage = (int)damage;
		component.GunType = WeaponType.TrackingRPG;
		component.targetPos = aimTarget;
		component.bagIndex = (byte)GameApp.GetInstance().GetUserState().GetWeaponBagIndex(this);
		component.weapon = this;
		int trackingID = -1;
		if (!GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			component.trackingEnemy = GetNearestAimEnemy();
			if (component.trackingEnemy != null)
			{
				trackingID = component.trackingEnemy.EnemyID;
			}
		}
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			PlayerFireRocketRequest request = new PlayerFireRocketRequest(gunID, gunfire.position, direction, trackingID);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
		AudioManager.GetInstance().PlaySound(base.FireSoundName);
		attacked = true;
	}

	public override void Attack(float deltaTime)
	{
		lastAttackTime = Time.time;
		attacked = false;
	}

	public override void StopFire()
	{
	}
}
