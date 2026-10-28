using System.Collections.Generic;
using UnityEngine;

public class VehicleWeapon
{
	protected float mDamageRadius;

	protected float mAttackFrequency;

	protected PlayerDamageType mDamageType;

	protected float mBulletSpeed;

	protected Player mPlayer;

	protected float mDamage;

	protected float mLastAttackTime;

	protected EVehicleWeaponType mWeaponType;

	protected List<Transform> mGunPointTransformList = new List<Transform>();

	protected List<GameObject> mGunFireObjectList = new List<GameObject>();

	public string ExplodeSoundName { get; set; }

	public virtual void Attack()
	{
	}

	public virtual void StartLoopingSound()
	{
	}

	public virtual void EndLoopingSound()
	{
	}

	public Player GetPlayer()
	{
		return mPlayer;
	}

	public virtual void Init(Player player, float baseDamage, float damageRatio, float damageRadius, float attackFrequency, PlayerDamageType damageType, float bulletSpeed)
	{
		mPlayer = player;
		mDamage = baseDamage + damageRatio * mPlayer.GetWeapon().Damage * attackFrequency / mPlayer.GetWeapon().AttackFrequency;
		mDamageRadius = damageRadius;
		mAttackFrequency = attackFrequency;
		mDamageType = damageType;
		mBulletSpeed = bulletSpeed;
		mLastAttackTime = Time.time - mAttackFrequency;
	}

	public void AddGunPoint(Transform t)
	{
		if (mGunPointTransformList == null)
		{
			mGunPointTransformList = new List<Transform>();
		}
		mGunPointTransformList.Add(t);
	}

	public virtual void CreateGunEffect()
	{
	}

	public virtual void AutoDestroyGunEffect()
	{
	}

	public virtual void StopGunEffect()
	{
	}

	public virtual void CreateTrajectory()
	{
		mLastAttackTime = Time.time;
	}

	public bool CoolDown()
	{
		return Time.time - mLastAttackTime > mAttackFrequency;
	}

	public virtual bool IsTypeOfLoopShooting()
	{
		return false;
	}

	public virtual DamageProperty CreateDamageProperty()
	{
		DamageProperty damageProperty = new DamageProperty(mPlayer, (int)mDamage);
		damageProperty.criticalAttack = false;
		if (mPlayer.IsLocal())
		{
			damageProperty.isLocal = true;
		}
		else
		{
			damageProperty.isLocal = false;
		}
		damageProperty.wType = WeaponType.NoGun;
		damageProperty.vehicleWeaponType = mWeaponType;
		damageProperty.isVehicle = true;
		if (mPlayer.mVehicleState == EPlayerVehicleState.NONE)
		{
			damageProperty.vehicleType = EVehicleType.NA;
		}
		else
		{
			damageProperty.vehicleType = mPlayer.mVehicle.VehicleType;
		}
		damageProperty.damageType = mDamageType;
		damageProperty.weaponid = (int)mWeaponType;
		damageProperty.mAttackerType = AttackerType.VEHICLE;
		damageProperty.buff += mPlayer.GetSkills().GetSkill(SkillsType.ALL_DAMAGE);
		if (mDamageType != PlayerDamageType.NORMAL)
		{
			SkillsType sType = (SkillsType)(mDamageType - 1 + 53);
			damageProperty.buff += mPlayer.GetSkills().GetSkill(sType);
		}
		return damageProperty;
	}
}
