using UnityEngine;

public class RPG : Weapon
{
	protected const float shootLastingTime = 0.5f;

	protected static int sbulletCount;

	private GameObject trajectoryObject;

	protected AlphaAnimationScript aas;

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

	public RPG()
	{
		maxCapacity = 9999;
		base.IsSelectedForBattle = false;
	}

	public override WeaponType GetWeaponType()
	{
		return WeaponType.RPG;
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

	protected virtual EffectPoolType GetRocketEffect()
	{
		return EffectPoolType.W3_ROCKET;
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
		PlayIdleAnimGunOn();
	}

	public override void Init(Player player)
	{
		base.Init(player);
		aas = gun.GetComponent<AlphaAnimationScript>();
		base.FireSoundName = AudioName.GUN_03_RPG;
		base.ExplodeSoundName = AudioName.GUN_03_RPG_BOOM;
	}

	public override void UIInit(GameObject weaponObj, GameObject weaponObjL)
	{
		base.UIInit(weaponObj, weaponObjL);
		base.FireSoundName = AudioName.GUN_03_RPG;
		base.ExplodeSoundName = AudioName.GUN_03_RPG_BOOM;
	}

	public override void CreateTrajectoryUI(GameObject avatar, GameObject gunObj, GameObject gunObjL)
	{
		AudioManager.GetInstance().PlaySoundAt(base.FireSoundName, avatar.transform.position);
		gunfire = GetGunFire(gunObj);
		Vector3 normalized = gunfire.transform.forward.normalized;
		trajectoryObject = EffectPool.GetInstance().GetEffectPool(GetRocketEffect()).CreateObject(gunfire.position, Vector3.zero, gunfire.rotation);
		trajectoryObject.GetComponent<ProjectileScript>().enabled = false;
		trajectoryObject.transform.localScale = Vector3.one * 0.5f;
		FireLineScript component = GameWorld.GetComponent<FireLineScript>(trajectoryObject);
		component.enabled = true;
		component.beginPos = gunfire.position;
		component.endPos = gunfire.position + normalized * 100f;
		AutoDestroyScript component2 = GameWorld.GetComponent<AutoDestroyScript>(trajectoryObject);
		component2.life = 1f;
		component2.enabled = true;
		component2.type = AutoDestroyType.UNABLE;
		component2.destroySound = base.ExplodeSoundName;
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
		GameObject gameObject = gameWorld.GetEffectPool(GetRocketEffect()).CreateObject(gunfire.position, direction, Quaternion.identity);
		gameObject.transform.localScale = Vector3.one;
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
		component3.GunType = WeaponType.RPG;
		component3.targetPos = aimTarget;
		component3.weapon = this;
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			PlayerFireRocketRequest request = new PlayerFireRocketRequest(gunID, gunfire.position, direction);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
	}

	public override void StopFire()
	{
	}
}
