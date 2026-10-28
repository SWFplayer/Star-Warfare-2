using System.Collections.Generic;
using UnityEngine;

public class RailGun : Weapon
{
	public enum EInnerState
	{
		EIdle = 0,
		ECharge = 1,
		EFire = 2
	}

	public const float BULLET_SPEED = 150f;

	public const float MaxEnergy = 100f;

	public float mCurrentEnergy;

	public float mChargeSpeed = 66f;

	public float mShotEnergy = 25f;

	private EInnerState mInnerState;

	private GameObject mChargeVFX;

	private float mDamageScale;

	private byte mOnlineFactor;

	private int mShootTimes;

	private bool mLastFire;

	private float mRobotFireStartTime;

	private float mRobotFireDuration;

	private bool mFirstShoot;

	public override void Init(Player player)
	{
		base.Init(player);
		mChargeVFX = Pool.sAllocate("SW2_Effect/W7_Charge01");
		mChargeVFX.transform.parent = gun.transform;
		mChargeVFX.transform.localPosition = Vector3.zero;
		mChargeVFX.transform.localEulerAngles = Vector3.zero;
		mChargeVFX.SetActive(false);
	}

	public override WeaponType GetWeaponType()
	{
		return WeaponType.RailGun;
	}

	public override string GetAnimationSuffix()
	{
		return "RailGun";
	}

	public override string GetAnimationSuffixAlter()
	{
		return "RailGun";
	}

	public override bool IsTypeOfLoopShootingWeapon()
	{
		return true;
	}

	public override EAimType GetIdleAimType()
	{
		return EAimType.NONE;
	}

	public override EAimType GetAttackAimType()
	{
		return EAimType.NONE;
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

	public override bool CoolDown()
	{
		if (mFirstShoot)
		{
			return base.CoolDown();
		}
		if (mInnerState == EInnerState.EFire)
		{
			return Time.time - lastAttackTime > attackFrenquency * 3f / 7f;
		}
		return base.CoolDown();
	}

	public override void UpdateAttackState(float deltaTime)
	{
		bool flag = ((player.GetSkills().GetSkill(SkillsType.FLY) > 0f) ? true : false);
		InputController inputController = player.inputController;
		WrapMode mode = WrapMode.Loop;
		if (inputController.inputInfo.fire)
		{
			mCurrentEnergy += mChargeSpeed * Time.deltaTime;
			if (mCurrentEnergy > 100f)
			{
				mCurrentEnergy = 100f;
			}
			AudioManager.GetInstance().PlaySoundSingleLoop(AudioName.GUN_07_RailgunCharge, player.GetObject());
		}
		else
		{
			AudioManager.GetInstance().StopSound(AudioName.GUN_07_RailgunCharge, player.GetObject());
		}
		if (inputController.inputInfo.fire && mInnerState == EInnerState.EIdle)
		{
			mInnerState = EInnerState.ECharge;
			SetChargeActive(true);
		}
		else if (mLastFire && !inputController.inputInfo.fire)
		{
			if (mCurrentEnergy.Equals(100f))
			{
				mDamageScale = 1.8f;
				mOnlineFactor = 4;
				mShootTimes = 4;
			}
			else if (mCurrentEnergy > 3f * mShotEnergy)
			{
				mDamageScale = 1.5f;
				mOnlineFactor = 3;
				mShootTimes = 4;
			}
			else if (mCurrentEnergy > 2f * mShotEnergy)
			{
				mDamageScale = 1.3f;
				mOnlineFactor = 2;
				mShootTimes = 3;
			}
			else if (mCurrentEnergy > 1f * mShotEnergy)
			{
				mDamageScale = 1.1f;
				mOnlineFactor = 1;
				mShootTimes = 2;
			}
			else
			{
				mDamageScale = 1f;
				mOnlineFactor = 0;
				mShootTimes = 1;
			}
			mCurrentEnergy = 0f;
			mFirstShoot = true;
		}
		if (CoolDown())
		{
			if (player.IsLocal())
			{
				if (mShootTimes > 0)
				{
					player.Attack();
				}
			}
			else if (mShootTimes > 0)
			{
				CreateTrajectory();
			}
			if (inputController.inputInfo.IsMoving())
			{
				if (flag)
				{
					player.PlayAnimation(AnimationString.GetFlyAniString(inputController.inputInfo.dir), WrapMode.Loop);
					if (mInnerState == EInnerState.ECharge)
					{
						player.PlayAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttackIdle, WrapMode.Loop);
					}
					else if (mInnerState == EInnerState.EFire)
					{
						player.PlayAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttack, mode);
					}
				}
				else
				{
					player.PlayAnimation(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttackDown, WrapMode.Loop);
					if (mInnerState == EInnerState.ECharge)
					{
						player.PlayAnimation(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttackIdle, WrapMode.Loop);
					}
					else if (mInnerState == EInnerState.EFire)
					{
						player.PlayAnimation(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttackUp, mode);
					}
				}
			}
			else if (flag)
			{
				player.PlayAnimation(AnimationString.FlyIdle, WrapMode.Loop);
				if (mInnerState == EInnerState.ECharge)
				{
					player.PlayAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
				}
				else if (mInnerState == EInnerState.EFire)
				{
					player.PlayAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack, mode);
				}
			}
			else if (mInnerState == EInnerState.ECharge)
			{
				player.PlayAnimation(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
				player.PlayAnimation(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttackIdle, WrapMode.Loop);
			}
			else if (mInnerState == EInnerState.EFire)
			{
				player.PlayAnimationAllLayers(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack, mode);
			}
		}
		if (GetWaitForAttackAnimationStop() && Time.time - lastAttackTime > 0.2f && (player.AnimationPlayed(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack, 0.5f) || player.AnimationPlayed(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttackUp, 0.5f) || player.AnimationPlayed(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack, 0.5f) || player.AnimationPlayed(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttack, 0.5f)) && mShootTimes == 0)
		{
			if (inputController.inputInfo.fire)
			{
				mInnerState = EInnerState.ECharge;
				SetChargeActive(true);
				if (inputController.inputInfo.IsMoving())
				{
					if (flag)
					{
						player.PlayAnimation(AnimationString.GetFlyAniString(inputController.inputInfo.dir), WrapMode.Loop);
						if (mInnerState == EInnerState.ECharge)
						{
							player.PlayAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
						}
					}
					else
					{
						player.PlayAnimation(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttackDown, WrapMode.Loop);
						if (mInnerState == EInnerState.ECharge)
						{
							player.PlayAnimation(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttackIdle, WrapMode.Loop);
						}
					}
				}
				else if (flag)
				{
					player.PlayAnimation(AnimationString.FlyIdle, WrapMode.Loop);
					if (mInnerState == EInnerState.ECharge)
					{
						player.PlayAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
					}
				}
				else if (mInnerState == EInnerState.ECharge)
				{
					player.PlayAnimation(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
					player.PlayAnimation(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttackIdle, WrapMode.Loop);
				}
			}
			else
			{
				mInnerState = EInnerState.EIdle;
				player.ChangeToIdleState();
				if (inputController.inputInfo.IsMoving())
				{
					if (flag)
					{
						player.PlayAnimation(AnimationString.GetFlyAniString(inputController.inputInfo.dir), WrapMode.Loop);
						player.PlayAnimation(AnimationString.Fly + "_" + player.GetWeaponAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
					}
					else
					{
						player.PlayAnimationAllLayers(AnimationString.Stand + "_" + player.GetWeaponAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
					}
				}
				else if (flag)
				{
					player.PlayAnimation(AnimationString.FlyIdle, WrapMode.Loop);
					player.PlayAnimation(AnimationString.Fly + "_" + player.GetWeaponAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
				}
				else
				{
					player.PlayAnimationAllLayers(AnimationString.Stand + "_" + player.GetWeaponAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
				}
			}
		}
		mLastFire = inputController.inputInfo.fire;
	}

	public override void UpdateIdleState(float deltaTime)
	{
		base.UpdateIdleState(deltaTime);
		mInnerState = EInnerState.EIdle;
		AudioManager.GetInstance().StopSound(AudioName.GUN_07_RailgunCharge, player.GetObject());
	}

	public override void Attack(float deltaTime)
	{
		AudioManager.GetInstance().PlaySoundAt(AudioName.GUN_07_RailgunShot, player.GetTransform().position);
		mFirstShoot = false;
		lastAttackTime = Time.time;
		mShootTimes--;
		mInnerState = EInnerState.EFire;
		SetChargeActive(false);
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
			damageProperty.damage = (int)(damage * mDamageScale);
			damageProperty.hitpoint = aimTarget;
			damageProperty.damageFactor = mOnlineFactor;
			bulletRay.ShootDetect(damageProperty, list, 40, gameWorld.GetEffectPool(EffectPoolType.W7_SHOOT_MARK), range, 0f, 1f, true);
		}
		GameObject gameObject = EffectPool.GetInstance().GetEffectPool(EffectPoolType.W7_TRAJECTORY).CreateObject(gunfire.transform.position, direction, Quaternion.identity);
		gameObject.transform.localScale = Vector3.one;
		if (gameObject != null)
		{
			FireLineScript fireLineScript = gameObject.GetComponent<FireLineScript>();
			if (fireLineScript == null)
			{
				fireLineScript = gameObject.AddComponent<FireLineScript>();
			}
			fireLineScript.beginPos = gunfire.position;
			fireLineScript.endPos = gunfire.position + direction * 100f;
			fireLineScript.speed = 150f;
		}
	}

	private void SetChargeActive(bool enable)
	{
		if (mChargeVFX != null)
		{
			mChargeVFX.SetActive(enable);
		}
	}

	public override void StartAttackState()
	{
		mRobotFireDuration = Random.Range(Robot.RailGunChargeDuration.x, Robot.RailGunChargeDuration.y);
		mRobotFireStartTime = Time.time;
		mCurrentEnergy = 0f;
		mLastFire = false;
		mInnerState = EInnerState.EIdle;
		mFirstShoot = true;
	}

	public override void EndAttackState()
	{
		SetChargeActive(false);
	}

	public override void StartIdleState()
	{
		SetChargeActive(false);
	}

	public override bool GetInputFireStatus()
	{
		if (Time.time - mRobotFireStartTime < mRobotFireDuration)
		{
			return true;
		}
		if (mShootTimes == 0)
		{
			mRobotFireStartTime = Time.time;
		}
		return false;
	}

	public float GetEnergyPercentage()
	{
		return mCurrentEnergy / 100f;
	}

	public override void GunOff()
	{
		base.GunOff();
		if (mChargeVFX != null)
		{
			mChargeVFX.SetActive(false);
		}
		AudioManager.GetInstance().StopSound(AudioName.GUN_07_RailgunCharge, player.GetObject());
	}

	public override void UIInit(GameObject weaponObj, GameObject weaponObjL)
	{
		base.UIInit(weaponObj, weaponObjL);
		gunfire = GetGunFire(weaponObj);
	}

	public override void CreateTrajectoryUI(GameObject avatar, GameObject gunObj, GameObject gunObjL)
	{
		Vector3 forward = gunfire.transform.forward;
		AudioManager.GetInstance().PlaySoundAt(AudioName.GUN_07_RailgunShot, avatar.transform.position);
		GameObject gameObject = EffectPool.GetInstance().GetEffectPool(EffectPoolType.W7_TRAJECTORY).CreateObject(gunfire.transform.position, forward, Quaternion.identity);
		gameObject.transform.localScale = Vector3.one * 0.5f;
		if (gameObject != null)
		{
			FireLineScript fireLineScript = gameObject.GetComponent<FireLineScript>();
			if (fireLineScript == null)
			{
				fireLineScript = gameObject.AddComponent<FireLineScript>();
			}
			fireLineScript.beginPos = gunfire.position;
			fireLineScript.endPos = gunfire.position + forward * 100f;
			fireLineScript.speed = 10f;
		}
	}

	public override void GunOn()
	{
		base.GunOn();
		PlayIdleAnimGunOn();
	}

	public override void CreateTrajectory()
	{
		AudioManager.GetInstance().PlaySoundAt(AudioName.GUN_07_RailgunShot, player.GetTransform().position);
		mFirstShoot = false;
		lastAttackTime = Time.time;
		mShootTimes--;
		mInnerState = EInnerState.EFire;
		SetChargeActive(false);
		if (gunfireObj != null)
		{
			gunfireObj.SetActive(true);
		}
		gunfire.transform.rotation = player.GetTransform().rotation;
		gunfire.transform.rotation = Quaternion.AngleAxis(player.AngleV, -gunfire.transform.right) * gunfire.transform.rotation;
		Vector3 normalized = gunfire.transform.forward.normalized;
		GameObject gameObject = EffectPool.GetInstance().GetEffectPool(EffectPoolType.W7_TRAJECTORY).CreateObject(gunfire.transform.position, normalized, Quaternion.identity);
		gameObject.transform.localScale = Vector3.one;
		if (gameObject != null)
		{
			FireLineScript fireLineScript = gameObject.GetComponent<FireLineScript>();
			if (fireLineScript == null)
			{
				fireLineScript = gameObject.AddComponent<FireLineScript>();
			}
			fireLineScript.beginPos = gunfire.position;
			fireLineScript.endPos = gunfire.position + normalized * 100f;
			fireLineScript.speed = 150f;
		}
	}

	public override void StopFire()
	{
		base.StopFire();
		if (player != null)
		{
			AudioManager.GetInstance().StopSound(AudioName.GUN_07_RailgunCharge, player.GetObject());
		}
	}
}
