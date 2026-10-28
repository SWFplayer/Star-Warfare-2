using System.Collections.Generic;
using UnityEngine;

public class RPGTurret : Cannon
{
	public override void Init(GameObject gObject)
	{
		vehicleType = EVehicleType.RPG_TURRET;
		mOpenDistance = 1f;
		StartMoveTime = 0f;
		AttachedLocalPosition = new Vector3(0f, -0.784f, 0.296f);
		base.Init(gObject);
		mRotationTransform = entityTransform.Find("root/Point10/Point_Rot");
		mUpDownTransformList = new List<Transform>();
		mUpDownTransformList.Add(entityTransform.Find("root/Point10/Point_Rot/Bone08/Point_UD"));
		mLeftHand = entityTransform.Find("root/Point10/Point_Rot/Bone08/Point_UD/Bone09/Point14/Bone16/PointHandleL");
		mRightHand = entityTransform.Find("root/Point10/Point_Rot/Bone08/Point_UD/Bone09/Point13/Bone17/PointHandleR");
		mHitObject = mRotationTransform.gameObject;
	}

	public override void GetOn()
	{
		base.GetOn();
		if (rider.IsLocal())
		{
			GameApp.GetInstance().GetGameWorld().GetCamera()
				.ChangeTarget(mRotationTransform);
		}
	}

	public override void AttachRider()
	{
		if (rider != null)
		{
			rider.GetTransform().parent = mRotationTransform;
		}
	}

	protected override void InitWeapons()
	{
		mAggressiveWeaponList = new List<VehicleWeapon>();
		VehicleWeapon vehicleWeapon = new UnccTurretMachineGun();
		vehicleWeapon.Init(rider, mBaseDamage1, mDamageRatio1, mDamageRadius1, mAttackFrequency1, mDamageType1, mBulletSpeed1);
		vehicleWeapon.AddGunPoint(entityTransform.Find("root/Point10/Point_Rot/Bone08/Point_UD/Bone09/Point09/Bone21/Point06/Bone10/Point03/Bone11/Point_flame01"));
		vehicleWeapon.AddGunPoint(entityTransform.Find("root/Point10/Point_Rot/Bone08/Point_UD/Bone09/Point09/Bone21/Point06/Bone10/Point03/Bone11/Point_flame02"));
		vehicleWeapon.CreateGunEffect();
		mAggressiveWeaponList.Add(vehicleWeapon);
		VehicleWeapon vehicleWeapon2 = new UnccTurretMachineGun();
		vehicleWeapon2.Init(rider, mBaseDamage1, mDamageRatio1, mDamageRadius1, mAttackFrequency1, mDamageType1, mBulletSpeed1);
		vehicleWeapon2.AddGunPoint(entityTransform.Find("root/Point10/Point_Rot/Bone08/Point_UD/Bone09/Point08/Bone20/Point07/Bone13/Point04/Bone14/Point_flame07"));
		vehicleWeapon2.AddGunPoint(entityTransform.Find("root/Point10/Point_Rot/Bone08/Point_UD/Bone09/Point08/Bone20/Point07/Bone13/Point04/Bone14/Point_flame08"));
		vehicleWeapon2.CreateGunEffect();
		mAggressiveWeaponList.Add(vehicleWeapon2);
	}

	public override string GetNamePrefex()
	{
		return "UNCC_Turret";
	}

	protected override string GetOpenSoundName()
	{
		return AudioName.UNCC_TURRET_OPEN;
	}

	public override void AdjustAnimationInLateUpdate(float deltaTime)
	{
		float num = 1f;
		if (rider.AngleV < 0f)
		{
			num = 0.45f;
		}
		foreach (Transform mUpDownTransform in mUpDownTransformList)
		{
			mUpDownTransform.rotation = mRotationTransform.rotation * Quaternion.Euler((0f - rider.AngleV) * num, 0f, 0f);
		}
	}

	public override Vector3 GetForward()
	{
		return mRotationTransform.forward;
	}
}
