using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGun : Weapon
{
	protected static int sbulletCount;

	protected Timer shotgunFireTimer;

	protected bool readyforCock;

	private float damageDeductRate = 0.08f;

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

	public ShotGun()
	{
		maxCapacity = 9999;
		base.IsSelectedForBattle = false;
		shotgunFireTimer = new Timer();
	}

	public override WeaponType GetWeaponType()
	{
		return WeaponType.ShotGun;
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

	public override EAimType GetIdleAimType()
	{
		return EAimType.ANGLE;
	}

	public override EAimType GetAttackAimType()
	{
		return EAimType.ANGLE;
	}

	public override void CreateTrajectory()
	{
		base.CreateTrajectory();
		AudioManager.GetInstance().PlaySoundAt(AudioName.GUN_02_SHOTGUN, player.GetTransform().position);
		GameObject gameObject = gameWorld.GetEffectPool(EffectPoolType.W2_SPARK).CreateObject(gunfire.position, Vector3.zero, Quaternion.identity);
		GameObject gameObject2 = gameWorld.GetEffectPool(EffectPoolType.W2_TRAJECTORY).CreateObject(gunfire.position, Vector3.zero, gunfire.rotation);
	}

	public override void CreateTrajectoryUI(GameObject avatar, GameObject gunObj, GameObject gunObjL)
	{
		AudioManager.GetInstance().PlaySoundAt(AudioName.GUN_02_SHOTGUN, avatar.transform.position);
		gunfire = GetGunFire(gunObj);
		GameObject gameObject = EffectPool.GetInstance().GetEffectPool(EffectPoolType.W2_SPARK).CreateObject(gunfire.position, Vector3.zero, gunfire.rotation);
		GameObject gameObject2 = EffectPool.GetInstance().GetEffectPool(EffectPoolType.W2_TRAJECTORY).CreateObject(gunfire.position, Vector3.zero, gunfire.rotation);
	}

	public override void GunOn()
	{
		base.GunOn();
	}

	public override void Init(Player player)
	{
		base.Init(player);
		hitForce = 20f;
		damageDeductRate = 50f / range * 0.01f;
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
		AudioManager.GetInstance().PlaySoundAt(AudioName.GUN_02_SHOTGUN, player.GetTransform().position);
		GameApp.GetInstance().GetUserState().UseEnegy(enegyConsume);
		GameObject gameObject = gameWorld.GetEffectPool(EffectPoolType.W2_SPARK).CreateObject(gunfire.position, Vector3.zero, Quaternion.identity);
		GameObject gameObject2 = gameWorld.GetEffectPool(EffectPoolType.W2_TRAJECTORY).CreateObject(gunfire.position, Vector3.zero, gunfire.rotation);
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
			damageProperty.hitpoint = Vector3.zero;
			bulletRay.ShootDetect(damageProperty, list, 60, gameWorld.GetEffectPool(EffectPoolType.W2_SHOOT_MARK), range, 0f, 0.35f, false);
		}
	}

	public override void GunOff()
	{
		base.GunOff();
		readyforCock = false;
	}

	public override float SimpleDamage()
	{
		return 2f * base.SimpleDamage();
	}
}
