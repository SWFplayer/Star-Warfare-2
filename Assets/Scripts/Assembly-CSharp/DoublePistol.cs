using System.Collections.Generic;
using UnityEngine;

public class DoublePistol : Weapon
{
	protected Transform gunfireL;

	protected GameObject gunL;

	protected GameObject gunfireObjL;

	protected Transform weaponBoneTransL;

	protected bool mIsLeftFire;

	public override WeaponType GetWeaponType()
	{
		return WeaponType.DoublePistol;
	}

	public override string GetAnimationSuffix()
	{
		return "DoubleP";
	}

	public override string GetAnimationSuffixAlter()
	{
		return "DoubleP";
	}

	public override EAimType GetAttackAimType()
	{
		return EAimType.LOOK_AT;
	}

	public override void CreateTrajectoryUI(GameObject avatar, GameObject gunObj, GameObject gunObjL)
	{
		AudioManager.GetInstance().PlaySoundAt(AudioName.GUN_06_DoublePistol, avatar.transform.position);
		gunfire = GetGunFire(gunObj);
		Vector3 normalized = gunfire.transform.forward.normalized;
		GameObject gameObject = EffectPool.GetInstance().GetEffectPool(GetTrajectoryEffect()).CreateObject(gunfire.transform.position, normalized, Quaternion.identity);
		if (gameObject != null)
		{
			FireLineScript component = gameObject.GetComponent<FireLineScript>();
			component.beginPos = gunfire.position;
			component.endPos = gunfire.position + normalized * 100f;
		}
		if (gunfireObj != null)
		{
			gunfireObj.SetActive(true);
		}
		gunfireL = GetGunFire(gunObjL);
		Vector3 normalized2 = gunfireL.transform.forward.normalized;
		GameObject gameObject2 = EffectPool.GetInstance().GetEffectPool(GetTrajectoryEffect()).CreateObject(gunfireL.transform.position, normalized2, Quaternion.identity);
		if (gameObject2 != null)
		{
			FireLineScript component2 = gameObject2.GetComponent<FireLineScript>();
			component2.beginPos = gunfireL.position;
			component2.endPos = gunfireL.position + normalized * 100f;
		}
		if (gunfireObjL != null)
		{
			gunfireObjL.SetActive(true);
		}
	}

	public override void UIInit(GameObject weaponObj, GameObject weaponObjL)
	{
		gunfire = GetGunFire(weaponObj);
		gunfireL = GetGunFire(weaponObjL);
		Object obj = Resources.Load(EffectName.W6_SPARK);
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
		transform = gunfireL.Find(obj.name + "(Clone)");
		if (transform == null)
		{
			gunfireObjL = (GameObject)Object.Instantiate(obj, gunfireL.position, Quaternion.identity);
			gunfireObjL.transform.parent = gunfireL;
		}
		else
		{
			gunfireObjL = transform.gameObject;
		}
		StopFire();
	}

	protected override void GetRenderers()
	{
		Renderer[] componentsInChildren = gun.GetComponentsInChildren<Renderer>();
		Renderer[] componentsInChildren2 = gunL.GetComponentsInChildren<Renderer>();
		List<Renderer> list = new List<Renderer>();
		list.AddRange(componentsInChildren);
		list.AddRange(componentsInChildren2);
		gunRenderers = list.ToArray();
	}

	protected override void CreateGun()
	{
		string path = Global.WEAPON_PATH + "/" + GetPrefabLevel() + "/" + Global.WEAPON_PREFIX + string.Format("{0:D2}", gunID);
		Object original = Resources.Load(path);
		gun = (GameObject)Object.Instantiate(original);
		gunL = (GameObject)Object.Instantiate(original);
		GetRenderers();
	}

	public override void Init(Player player)
	{
		gameWorld = GameApp.GetInstance().GetGameWorld();
		gameCamera = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
		cameraComponent = gameCamera.GetComponent<Camera>();
		cameraTransform = gameCamera.CameraTransform;
		base.player = player;
		hitForce = 0f;
		weaponBoneTrans = player.GetTransform().Find(BoneName.Weapon);
		weaponBoneTransL = player.GetTransform().Find(BoneName.WeaponL);
		CreateGun();
		gun.transform.parent = weaponBoneTrans;
		gun.transform.localPosition = Vector3.zero;
		gun.transform.localRotation = Quaternion.identity;
		gunL.transform.parent = weaponBoneTransL;
		gunL.transform.localPosition = Vector3.zero;
		gunL.transform.localRotation = Quaternion.identity;
		shootAudio = gun.GetComponent<AudioSource>();
		gunfire = GetGunFire();
		gunfireL = GetGunFire(gunL);
		gunfire.transform.localRotation = Quaternion.Euler(0f, 0f, -70f);
		gunfireL.transform.localRotation = Quaternion.Euler(0f, 0f, 70f);
		GameObject gameObject = new GameObject("RotatePivot");
		mRotatePivot = gameObject.transform;
		mRotatePivot.parent = gunfire;
		mRotatePivot.localPosition = new Vector3(0f, 0f, 0f - base.Adjuster.mOffsetStand);
		mRotatePivot.localRotation = Quaternion.identity;
		GameObject gameObject2 = new GameObject("RotatePivot");
		mRotatePivotL = gameObject2.transform;
		mRotatePivotL.parent = gunfireL;
		mRotatePivotL.localPosition = new Vector3(0f, 0f, 0f - base.Adjuster.mOffsetStand);
		mRotatePivotL.localRotation = Quaternion.identity;
		weaponBoneTransL = player.GetTransform().Find(BoneName.WeaponL);
		Object original = Resources.Load(EffectName.W6_SPARK);
		gunfireObj = (GameObject)Object.Instantiate(original, gunfire.position, Quaternion.identity);
		gunfireObj.transform.parent = gunfire;
		gunfireObjL = (GameObject)Object.Instantiate(original, gunfireL.position, Quaternion.identity);
		gunfireObjL.transform.parent = gunfireL;
		playSoundTimer.SetTimer(attackFrenquency, true);
		GunOff();
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
		if (gunfireObjL != null)
		{
			gunfireObjL.SetActive(false);
		}
	}

	public override void PlayIdleAnim()
	{
		StopFire();
	}

	public override void GunOn()
	{
		base.GunOn();
		if (Random.Range(0, 100) < 50)
		{
			mIsLeftFire = true;
		}
		else
		{
			mIsLeftFire = false;
		}
	}

	public override void GunOff()
	{
		base.GunOff();
	}

	public override void PlaySound()
	{
		if (playSoundTimer.Ready())
		{
			AudioManager.GetInstance().PlaySoundAt(AudioName.GUN_06_DoublePistol, player.GetTransform().position);
			playSoundTimer.Do();
		}
	}

	public override void CreateTrajectory()
	{
		base.CreateTrajectory();
		Transform transform = gunfire;
		if (mIsLeftFire)
		{
			transform = gunfireL;
		}
		transform.transform.rotation = player.GetTransform().rotation;
		transform.transform.rotation = Quaternion.AngleAxis(player.AngleV, -transform.transform.right) * transform.transform.rotation;
		Vector3 normalized = transform.transform.forward.normalized;
		GameObject gameObject = gameWorld.GetEffectPool(GetTrajectoryEffect()).CreateObject(transform.transform.position, normalized, Quaternion.identity);
		if (gameObject != null)
		{
			FireLineScript component = gameObject.GetComponent<FireLineScript>();
			component.beginPos = transform.position;
			component.endPos = transform.position + normalized * 100f;
		}
		if (gunfireObj != null)
		{
			gunfireObj.SetActive(true);
		}
		if (gunfireObjL != null)
		{
			gunfireObjL.SetActive(true);
		}
		mIsLeftFire = !mIsLeftFire;
	}

	public override void Attack(float deltaTime)
	{
		GameApp.GetInstance().GetUserState().UseEnegy(enegyConsume);
		if (gunfireObj != null)
		{
			gunfireObj.SetActive(true);
		}
		if (gunfireObjL != null)
		{
			gunfireObjL.SetActive(true);
		}
		Transform transform = gunfire;
		if (mIsLeftFire)
		{
			transform = gunfireL;
		}
		Vector3 direction;
		Ray weaponRay = GetWeaponRay(transform, out direction);
		List<Ray> list = new List<Ray>();
		list.Add(weaponRay);
		BulletRay bulletRay = BulletScript.CreateBullet(BulletType.Ray, player) as BulletRay;
		if (bulletRay != null)
		{
			DamageProperty damageProperty = CreateDamageProperty();
			damageProperty.hitForce = weaponRay.direction * 2f;
			damageProperty.hitpoint = aimTarget;
			bulletRay.ShootDetect(damageProperty, list, 40, GetShootMark(), range, 0f, 1f, false);
		}
		GameObject gameObject = gameWorld.GetEffectPool(GetTrajectoryEffect()).CreateObject(transform.transform.position, direction, Quaternion.identity);
		if (gameObject != null)
		{
			FireLineScript component = gameObject.GetComponent<FireLineScript>();
			component.beginPos = transform.position;
			component.endPos = transform.position + direction * 100f;
		}
		attacked = false;
		lastAttackTime = Time.time;
		mIsLeftFire = !mIsLeftFire;
	}

	public override void EnableGunObject(bool bEnable)
	{
		if (!bEnable)
		{
			gun.SetActive(false);
			gunL.SetActive(false);
		}
		else
		{
			gun.SetActive(true);
			gunL.SetActive(true);
		}
	}

	protected virtual EffectPoolType GetTrajectoryEffect()
	{
		return EffectPoolType.W6_TRAJECTORY;
	}

	protected virtual ObjectPool GetShootMark()
	{
		return gameWorld.GetEffectPool(EffectPoolType.W4_SHOOT_MARK);
	}
}
