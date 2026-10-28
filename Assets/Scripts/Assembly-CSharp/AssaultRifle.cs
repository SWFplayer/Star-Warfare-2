using System.Collections.Generic;
using UnityEngine;

public class AssaultRifle : Weapon
{
	public override WeaponType GetWeaponType()
	{
		return WeaponType.AssaultRifle;
	}

	public override string GetAnimationSuffix()
	{
		return "assault";
	}

	public override string GetAnimationSuffixAlter()
	{
		return "assault";
	}

	public override EAimType GetAttackAimType()
	{
		return EAimType.LOOK_AT;
	}

	public override void CreateTrajectoryUI(GameObject avatar, GameObject gunObj, GameObject gunObjL)
	{
		AudioManager.GetInstance().PlaySoundAt(AudioName.GUN_01_SMG, avatar, avatar.transform.position);
		gunfire = GetGunFire(gunObj);
		Vector3 normalized = gunfire.transform.forward.normalized;
		GameObject gameObject = EffectPool.GetInstance().GetEffectPool(GetBulletEffectPoolType()).CreateObject(gunfire.transform.position, normalized, Quaternion.identity);
		if (gameObject != null)
		{
			FireLineScript component = gameObject.GetComponent<FireLineScript>();
			component.beginPos = gunfire.position;
			component.endPos = gunfire.position + normalized * 100f;
			component.speed = GetBulletSpeed();
		}
		if (gunfireObj != null)
		{
			gunfireObj.SetActive(true);
		}
	}

	public override void UIInit(GameObject weaponObj, GameObject weaponObjL)
	{
		gunfire = GetGunFire(weaponObj);
		Object obj = Resources.Load(GetGunFireString());
		Transform transform = gunfire.Find(obj.name + "(Clone)");
		if (transform == null)
		{
			gunfireObj = (GameObject)Object.Instantiate(obj, gunfire.position, Quaternion.identity);
			gunfireObj.transform.parent = gunfire;
		}
		else
		{
			gunfireObj = transform.gameObject;
		}
		StopFire();
	}

	public override void Init(Player player)
	{
		base.Init(player);
		Object original = Resources.Load(GetGunFireString());
		gunfireObj = (GameObject)Object.Instantiate(original, gunfire.position, Quaternion.identity);
		gunfireObj.transform.parent = gunfire;
		playSoundTimer.SetTimer(attackFrenquency / 2f, true);
		StopFire();
	}

	public override void Loop(float deltaTime)
	{
	}

	public override void StopFire()
	{
		if (gunfireObj != null)
		{
			gunfireObj.SetActive(false);
		}
	}

	public override void PlayIdleAnim()
	{
		StopFire();
	}

	public override void GunOff()
	{
		base.GunOff();
	}

	public override void PlaySound()
	{
		AudioManager.GetInstance().PlaySoundSingleLoop(AudioName.GUN_01_SMG, player.GetObject(), player.GetTransform());
	}

	public override void StopSound()
	{
		AudioManager.GetInstance().StopSound(AudioName.GUN_01_SMG, player.GetObject());
	}

	public override void StopSoundForUI(GameObject avatar)
	{
		AudioManager.GetInstance().StopSound(AudioName.GUN_01_SMG, avatar);
	}

	public override void CreateTrajectory()
	{
		base.CreateTrajectory();
		Vector3 normalized = player.GetTransform().forward.normalized;
		gunfire.transform.rotation = player.GetTransform().rotation;
		gunfire.transform.rotation = Quaternion.AngleAxis(player.AngleV, -gunfire.transform.right) * gunfire.transform.rotation;
		normalized = gunfire.transform.forward.normalized;
		GameObject gameObject = gameWorld.GetEffectPool(GetBulletEffectPoolType()).CreateObject(gunfire.transform.position, normalized, Quaternion.identity);
		if (gameObject != null)
		{
			FireLineScript component = gameObject.GetComponent<FireLineScript>();
			component.beginPos = gunfire.position;
			component.endPos = gunfire.position + normalized * 100f;
			component.speed = GetBulletSpeed();
		}
		if (gunfireObj != null)
		{
			gunfireObj.SetActive(true);
		}
	}

	public override void Attack(float deltaTime)
	{
		GameApp.GetInstance().GetUserState().UseEnegy(enegyConsume);
		if (gunfireObj != null)
		{
			gunfireObj.SetActive(true);
		}
		Vector3 direction;
		Ray weaponRay = GetWeaponRay(gunfire, out direction);
		List<Ray> list = new List<Ray>();
		list.Add(weaponRay);
		BulletRay bulletRay = BulletScript.CreateBullet(BulletType.Ray, player) as BulletRay;
		if (bulletRay != null)
		{
			DamageProperty damageProperty = CreateDamageProperty();
			damageProperty.hitForce = weaponRay.direction * 2f;
			damageProperty.hitpoint = aimTarget;
			bulletRay.ShootDetect(damageProperty, list, 40, gameWorld.GetEffectPool(GetShootMark()), range, 0f, 1f, false);
		}
		GameObject gameObject = gameWorld.GetEffectPool(GetBulletEffectPoolType()).CreateObject(gunfire.transform.position, direction, Quaternion.identity);
		if (gameObject != null)
		{
			FireLineScript component = gameObject.GetComponent<FireLineScript>();
			component.beginPos = gunfire.position;
			component.endPos = gunfire.position + direction * 100f;
			component.speed = GetBulletSpeed();
		}
		attacked = false;
		lastAttackTime = Time.time;
	}

	protected virtual EffectPoolType GetBulletEffectPoolType()
	{
		if (base.GunID == 25)
		{
			return EffectPoolType.W25_BULLET;
		}
		return EffectPoolType.W1_TRAJECTORY;
	}

	private float GetBulletSpeed()
	{
		float num = ((!GameApp.GetInstance().GetGameMode().IsVSMode()) ? 1 : 2);
		if (gunID == 25)
		{
			return 100f * num;
		}
		return 75f * num;
	}

	private string GetGunFireString()
	{
		if (gunID == 25)
		{
			return EffectName.W25_SPARK;
		}
		return EffectName.W1_SPARK;
	}

	private EffectPoolType GetShootMark()
	{
		if (gunID == 25)
		{
			return EffectPoolType.W25_SHOOTMARK;
		}
		return EffectPoolType.W1_SHOOT_MARK;
	}
}
