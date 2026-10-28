using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShotGun : Weapon
{
	protected static int sbulletCount;

	protected Timer shotgunFireTimer;

	protected bool readyforCock;

	private float damageDeductRate = 0.08f;

	private GameObject LightShoot;

	private GameObject LightIdle;

	private Timer mAnimationTimer = new Timer();

	public override int BulletCount
	{
		get
		{
			return sbulletCount;
		}
		set
		{
			sbulletCount = value;
		}
	}

	public LaserShotGun()
	{
		maxCapacity = 9999;
		base.IsSelectedForBattle = false;
		shotgunFireTimer = new Timer();
	}

	public override WeaponType GetWeaponType()
	{
		return WeaponType.LaserShotGun;
	}

	public override string GetAnimationSuffix()
	{
		return "shotgun";
	}

	public override string GetAnimationSuffixAlter()
	{
		return "shotgun";
	}

	public override bool IsTypeOfLoopShootingWeapon()
	{
		return false;
	}

	public override void CreateTrajectory()
	{
		base.CreateTrajectory();
		AudioManager.GetInstance().PlaySoundAt(AudioName.GUN_04_LaserShotgun, player.GetTransform().position);
		GameObject gameObject = EffectPool.GetInstance().GetEffectPool(EffectPoolType.W5_SPARK).CreateObject(gunfire.position, Vector3.zero, gunfire.rotation);
		GameObject gameObject2 = EffectPool.GetInstance().GetEffectPool(GetTrajectory()).CreateObject(gunfire.position, Vector3.zero, gunfire.rotation);
	}

	public override void CreateTrajectoryUI(GameObject avatar, GameObject gunObj, GameObject gunObjL)
	{
		AudioManager.GetInstance().PlaySoundAt(AudioName.GUN_04_LaserShotgun, avatar.transform.position);
		gunfire = GetGunFire(gunObj);
		GameObject gameObject = EffectPool.GetInstance().GetEffectPool(EffectPoolType.W5_SPARK).CreateObject(gunfire.position, Vector3.zero, gunfire.rotation);
		GameObject gameObject2 = EffectPool.GetInstance().GetEffectPool(GetTrajectory()).CreateObject(gunfire.position, Vector3.zero, gunfire.rotation);
	}

	public override void GunOn()
	{
		base.GunOn();
		PlayIdleAnim();
	}

	public override EAimType GetIdleAimType()
	{
		return EAimType.ANGLE;
	}

	public override EAimType GetAttackAimType()
	{
		return EAimType.ANGLE;
	}

	public override void UIInit(GameObject weaponObj, GameObject weaponObjL)
	{
		if (gunID == 4)
		{
			Transform transform = weaponObj.gameObject.transform.Find("diandian");
			Object obj = Resources.Load(EffectName.W5_LIGHT_IDLE);
			Transform transform2 = transform.Find(obj.name + "(Clone)");
			if (transform2 == null)
			{
				LightIdle = Object.Instantiate(obj) as GameObject;
				LightIdle.transform.parent = transform;
				LightIdle.transform.localPosition = Vector3.zero;
				LightIdle.transform.localRotation = Quaternion.identity;
			}
			else
			{
				LightIdle = transform2.gameObject;
			}
			Object obj2 = Resources.Load(EffectName.W5_LIGHT_FIRE);
			Transform transform3 = transform.Find(obj2.name + "(Clone)");
			if (transform3 == null)
			{
				LightShoot = Object.Instantiate(obj2) as GameObject;
				LightShoot.transform.parent = transform;
				LightShoot.transform.localPosition = Vector3.zero;
				LightShoot.transform.localRotation = Quaternion.identity;
			}
			else
			{
				LightShoot = transform3.gameObject;
			}
			mAnimationTimer.SetTimer(attackFrenquency, true);
			PlayIdleAnim();
		}
	}

	public override void Init(Player player)
	{
		base.Init(player);
		hitForce = 20f;
		damageDeductRate = 50f / range * 0.01f;
		if (gunID == 4)
		{
			Object original = Resources.Load(EffectName.W5_LIGHT_IDLE);
			LightIdle = (GameObject)Object.Instantiate(original);
			LightIdle.transform.parent = gun.gameObject.transform.Find("diandian");
			LightIdle.transform.localPosition = Vector3.zero;
			LightIdle.transform.localRotation = Quaternion.identity;
			Object original2 = Resources.Load(EffectName.W5_LIGHT_FIRE);
			LightShoot = (GameObject)Object.Instantiate(original2);
			LightShoot.transform.parent = gun.gameObject.transform.Find("diandian");
			LightShoot.transform.localPosition = Vector3.zero;
			LightShoot.transform.localRotation = Quaternion.identity;
			mAnimationTimer.SetTimer(attackFrenquency, true);
			PlayIdleAnim();
		}
		else if (gunID != 2)
		{
			Debug.Log("Can not find reticle size for this shotgun");
		}
		GunOff();
	}

	public override void PlayIdleAnim()
	{
		base.PlayIdleAnim();
		if (LightIdle != null && LightShoot != null)
		{
			LightShoot.SetActive(false);
			LightIdle.SetActive(true);
		}
	}

	public override void PlayFireAnim()
	{
		base.PlayFireAnim();
		if (LightIdle != null && LightShoot != null && mAnimationTimer.Ready())
		{
			mAnimationTimer.Do();
			LightShoot.SetActive(false);
			LightShoot.SetActive(true);
		}
	}

	public override void StopGunAnim()
	{
		base.StopGunAnim();
		if (LightIdle != null && LightShoot != null)
		{
			LightIdle.SetActive(false);
			LightShoot.SetActive(false);
		}
	}

	public void PlayPumpAnimation()
	{
	}

	public override void PlayBlankShootSound()
	{
		AudioManager.GetInstance().PlaySoundSingle("_Old/_Audio/blank/blank_shot02");
	}

	public override void Loop(float deltaTime)
	{
		base.Loop(deltaTime);
	}

	public override void Attack(float deltaTime)
	{
		AudioManager.GetInstance().PlaySoundAt(AudioName.GUN_04_LaserShotgun, player.GetTransform().position);
		GameApp.GetInstance().GetUserState().UseEnegy(enegyConsume);
		GameObject gameObject = gameWorld.GetEffectPool(EffectPoolType.W5_SPARK).CreateObject(gunfire.position, Vector3.zero, Quaternion.identity);
		float x = 0f - GameApp.GetInstance().GetGameWorld().GetCamera()
			.AngelV;
		float angelH = GameApp.GetInstance().GetGameWorld().GetCamera()
			.AngelH;
		GameObject gameObject2 = gameWorld.GetEffectPool(GetTrajectory()).CreateObject(gunfire.position, Vector3.zero, Quaternion.Euler(x, angelH, 0f));
		lastAttackTime = Time.time;
		readyforCock = true;
		shotgunFireTimer = new Timer();
		shotgunFireTimer.SetTimer(attackFrenquency * 0.7f, false);
		Hashtable hashtable = new Hashtable();
		Hashtable hashtable2 = new Hashtable();
		List<Ray> list = new List<Ray>();
		for (int i = 0; i < bulletCount; i++)
		{
			Ray shotGunRay = GetShotGunRay(gunfire);
			list.Add(shotGunRay);
		}
		BulletRay bulletRay = BulletScript.CreateBullet(BulletType.Ray, player) as BulletRay;
		if (bulletRay != null)
		{
			DamageProperty damageProperty = CreateDamageProperty();
			damageProperty.hitForce = list[0].direction * 2f;
			bulletRay.ShootDetect(damageProperty, list, 60, gameWorld.GetEffectPool(EffectPoolType.W5_SHOOT_MARK), range, 0f, 0.35f, true);
		}
	}

	public override void GunOff()
	{
		base.GunOff();
		readyforCock = false;
		StopGunAnim();
	}

	public override float SimpleDamage()
	{
		return 2f * base.SimpleDamage();
	}

	public EffectPoolType GetTrajectory()
	{
		if (gunID == 29)
		{
			return EffectPoolType.W29_TRAJECTORY;
		}
		return EffectPoolType.W5_TRAJECTORY;
	}
}
