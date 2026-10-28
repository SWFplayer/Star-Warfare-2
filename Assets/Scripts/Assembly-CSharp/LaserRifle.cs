using System.Collections.Generic;
using UnityEngine;

public class LaserRifle : Weapon
{
	public override WeaponType GetWeaponType()
	{
		return WeaponType.LaserRifle;
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
		AudioManager.GetInstance().PlaySoundAt(AudioName.GUN_05_LaserRifle, avatar.transform.position);
		gunfire = GetGunFire(gunObj);
		Vector3 normalized = gunfire.transform.forward.normalized;
		GameObject gameObject = EffectPool.GetInstance().GetEffectPool(GetTrajectoryEffect()).CreateObject(gunfire.transform.position, normalized, Quaternion.identity);
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
		Object obj = ((gunID != 8) ? Resources.Load(EffectName.W4_SPARK) : Resources.Load(EffectName.W17_SPARK));
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
		Object original = ((gunID != 8) ? Resources.Load(EffectName.W4_SPARK) : Resources.Load(EffectName.W17_SPARK));
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

	public override void PlaySound()
	{
		if (playSoundTimer.Ready())
		{
			AudioManager.GetInstance().PlaySoundAt(AudioName.GUN_05_LaserRifle, player.GetTransform().position);
			playSoundTimer.Do();
		}
	}

	public override void CreateTrajectory()
	{
		base.CreateTrajectory();
		Vector3 normalized = player.GetTransform().forward.normalized;
		gunfire.transform.rotation = player.GetTransform().rotation;
		gunfire.transform.rotation = Quaternion.AngleAxis(player.AngleV, -gunfire.transform.right) * gunfire.transform.rotation;
		normalized = gunfire.transform.forward.normalized;
		GameObject gameObject = gameWorld.GetEffectPool(GetTrajectoryEffect()).CreateObject(gunfire.transform.position, normalized, Quaternion.identity);
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
			bulletRay.ShootDetect(damageProperty, list, 40, GetShootMark(), range, 0f, 1f, true);
		}
		GameObject gameObject = gameWorld.GetEffectPool(GetTrajectoryEffect()).CreateObject(gunfire.transform.position, direction, Quaternion.identity);
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

	protected virtual EffectPoolType GetTrajectoryEffect()
	{
		if (base.GunID == 8)
		{
			return EffectPoolType.W17_TRAJECTORY;
		}
		return EffectPoolType.W4_TRAJECTORY;
	}

	protected virtual ObjectPool GetShootMark()
	{
		if (gunID == 8)
		{
			return gameWorld.GetEffectPool(EffectPoolType.W17_SHOOTMARK);
		}
		return gameWorld.GetEffectPool(EffectPoolType.W4_SHOOT_MARK);
	}

	private float GetBulletSpeed()
	{
		float num = ((!GameApp.GetInstance().GetGameMode().IsVSMode()) ? 1 : 2);
		return 75f * num;
	}
}
