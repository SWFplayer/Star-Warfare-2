using System.Collections.Generic;
using UnityEngine;

public class VehicleUnccTurret : Cannon
{
	public override void Init(GameObject gObject)
	{
		vehicleType = EVehicleType.UNCC_TURRET;
		mOpenDistance = 1f;
		StartMoveTime = 0f;
		AttachedLocalPosition = new Vector3(0f, -0.784f, 0.296f);
		base.Init(gObject);
		mRotationTransform = entityTransform.Find("root/Point11/body_contr/Point_Rot");
		mUpDownTransformList = new List<Transform>();
		mUpDownTransformList.Add(entityTransform.Find("root/Point11/body_contr/Point_Rot/Point_UD"));
		mLeftHand = entityTransform.Find("root/Point11/body_contr/Point_Rot/Point_UD/Bone01/Point05/Bone12/Point06/Bone14/PointHandleL");
		mRightHand = entityTransform.Find("root/Point11/body_contr/Point_Rot/Point_UD/Bone01/Point04/Bone11/Point07/Bone13/PointHandleR");
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
		vehicleWeapon.AddGunPoint(entityTransform.Find("root/Point11/body_contr/Point_Rot/Point_UD/Bone01/Point02/Bone02/Point03/Bone03/Point_flame01"));
		vehicleWeapon.AddGunPoint(entityTransform.Find("root/Point11/body_contr/Point_Rot/Point_UD/Bone01/Point02/Bone02/Point03/Bone03/Point_flame02"));
		vehicleWeapon.AddGunPoint(entityTransform.Find("root/Point11/body_contr/Point_Rot/Point_UD/Bone01/Point02/Bone02/Point03/Bone03/Point_flame03"));
		vehicleWeapon.CreateGunEffect();
		mAggressiveWeaponList.Add(vehicleWeapon);
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
