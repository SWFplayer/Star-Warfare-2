using UnityEngine;

public class Fist : Weapon
{
	protected const float shootLastingTime = 0.5f;

	protected GameObject trajectoryObject;

	protected AlphaAnimationScript aas;

	protected Animation mAnimation;

	public Fist()
	{
		maxCapacity = 9999;
		base.IsSelectedForBattle = false;
	}

	public override WeaponType GetWeaponType()
	{
		return WeaponType.Fist;
	}

	public override string GetAnimationSuffix()
	{
		return "fist";
	}

	public override string GetAnimationSuffixAlter()
	{
		return "fist";
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

	public override void GunOn()
	{
		base.GunOn();
	}

	public override void Init(Player player)
	{
		base.Init(player);
		aas = gun.GetComponent<AlphaAnimationScript>();
		base.FireSoundName = AudioName.GUN_03_RPG;
		base.ExplodeSoundName = AudioName.GUN_03_RPG_BOOM;
		mAnimation = gun.GetComponent<Animation>();
	}

	public override void UIInit(GameObject weaponObj, GameObject weaponObjL)
	{
		base.UIInit(weaponObj, weaponObjL);
		base.FireSoundName = AudioName.GUN_03_RPG;
		base.ExplodeSoundName = AudioName.GUN_03_RPG_BOOM;
		mAnimation = weaponObj.GetComponent<Animation>();
		if (mAnimation != null)
		{
			mAnimation.Play("idle");
		}
	}

	public override void CreateTrajectoryUI(GameObject avatar, GameObject gunObj, GameObject gunObjL)
	{
		AudioManager.GetInstance().PlaySoundAt(base.FireSoundName, avatar.transform.position);
		gunfire = GetGunFire(gunObj);
		Vector3 normalized = avatar.transform.forward.normalized;
		trajectoryObject = EffectPool.GetInstance().GetEffectPool(GetFistVFX()).CreateObject(gunfire.position, Vector3.zero, avatar.transform.rotation);
		trajectoryObject.transform.localScale = Vector3.one * 0.5f;
		trajectoryObject.GetComponent<ProjectileScript>().enabled = false;
		FireLineScript component = trajectoryObject.GetComponent<FireLineScript>();
		component.enabled = true;
		component.beginPos = gunfire.position;
		component.endPos = gunfire.position + normalized * 100f;
		AutoDestroyScript component2 = trajectoryObject.GetComponent<AutoDestroyScript>();
		component2.enabled = true;
		component2.type = AutoDestroyType.UNABLE;
		component2.destroySound = base.ExplodeSoundName;
		if ((bool)mAnimation)
		{
			mAnimation.Play("attack");
		}
	}

	public override void Attack(float deltaTime)
	{
		lastAttackTime = Time.time;
		AudioManager.GetInstance().PlaySound(base.FireSoundName);
		GameApp.GetInstance().GetUserState().UseEnegy(enegyConsume);
		if (aas != null)
		{
			aas.StartAnimation();
		}
		Vector3 direction;
		Ray weaponRay = GetWeaponRay(gunfire, out direction);
		GameObject gameObject = gameWorld.GetEffectPool(GetFistVFX()).CreateObject(gunfire.position, direction, Quaternion.identity);
		gameObject.transform.localScale = Vector3.one;
		gameObject.GetComponent<AutoDestroyScript>().enabled = false;
		gameObject.GetComponent<FireLineScript>().enabled = false;
		ProjectileScript component = gameObject.GetComponent<ProjectileScript>();
		component.enabled = true;
		component.isLocal = true;
		component.dir = direction;
		component.flySpeed = bulletSpeed;
		component.explodeRadius = GetBombRange();
		component.hitForce = hitForce;
		component.life = 10f;
		component.damage = (int)damage;
		component.GunType = WeaponType.Fist;
		component.targetPos = aimTarget;
		component.weapon = this;
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			PlayerFireRocketRequest request = new PlayerFireRocketRequest(gunID, gunfire.position, direction);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
		if ((bool)mAnimation)
		{
			mAnimation.CrossFade("attack", 0.1f);
		}
	}

	public override void StopFire()
	{
	}

	public override void UpdateIdleState(float deltaTime)
	{
		base.UpdateIdleState(deltaTime);
		if ((bool)mAnimation)
		{
			mAnimation.CrossFade("idle", 0.3f);
		}
	}

	public override void PlayIdleAnim()
	{
		base.PlayIdleAnim();
		if ((bool)mAnimation)
		{
			mAnimation.CrossFade("idle", 0.3f);
		}
	}

	public virtual EffectPoolType GetFistVFX()
	{
		return EffectPoolType.W8_FIST;
	}
}
