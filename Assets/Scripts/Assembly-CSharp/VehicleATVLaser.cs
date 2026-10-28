using System.Collections.Generic;
using UnityEngine;

public class VehicleATVLaser : Carrier
{
	private GameObject mTraceMove;

	private GameObject mTraceStay;

	private Transform mPanci;

	public override void Init(GameObject gObject)
	{
		vehicleType = EVehicleType.ATV_LASER;
		mOpenDistance = 3f;
		StartMoveTime = 11f / 63f;
		base.Init(gObject);
		mHitObject = entityObject;
		mUpDownTransformList = new List<Transform>();
		mUpDownTransformList.Add(entityTransform.Find("root/Point02/Point03"));
		mUpDownTransformList.Add(entityTransform.Find("root/Point02/Point04"));
		mLeftHand = entityTransform.Find("root/Point02/Point04/PointHandleL");
		mRightHand = entityTransform.Find("root/Point02/Point03/PointHandleR");
		mPanci = entityTransform.Find("root/Point02/Point01/Object04");
		GameObject gameObject = null;
		for (int i = 0; i < mPanci.childCount; i++)
		{
			GameObject gameObject2 = mPanci.GetChild(i).gameObject;
			if (gameObject2.name.Contains("V1_Spin"))
			{
				gameObject = gameObject2;
			}
		}
		if (gameObject == null)
		{
			GameObject original = Resources.Load(EffectName.V1_SPIN) as GameObject;
			gameObject = Object.Instantiate(original) as GameObject;
			gameObject.transform.parent = mPanci;
		}
		gameObject.transform.localPosition = new Vector3(0f, 0f, -0.7f);
		animation["shoot_back"].speed = 0.25f;
		animation["shoot_forward"].speed = 0.25f;
		animation["shoot_idle"].speed = 0.25f;
		animation["shoot_left"].speed = 0.25f;
		animation["shoot_right"].speed = 0.25f;
	}

	protected override void InitWeapons()
	{
		mAggressiveWeaponList = new List<VehicleWeapon>();
		mPassiveWeaponList = new List<VehicleWeapon>();
		VehicleWeapon vehicleWeapon = new ATVMiniRPG();
		vehicleWeapon.Init(rider, mBaseDamage1, mDamageRatio1, mDamageRadius1, mAttackFrequency1, mDamageType1, mBulletSpeed1);
		vehicleWeapon.AddGunPoint(entityTransform.Find("root/Point02/Point03/Point06"));
		vehicleWeapon.AddGunPoint(entityTransform.Find("root/Point02/Point04/Point05"));
		vehicleWeapon.CreateGunEffect();
		mAggressiveWeaponList.Add(vehicleWeapon);
		VehicleWeapon vehicleWeapon2 = new ATVMiniSaw();
		vehicleWeapon2.Init(rider, mBaseDamage2, mDamageRatio2, mDamageRadius2, mAttackFrequency2, mDamageType2, mBulletSpeed2);
		mPassiveWeaponList.Add(vehicleWeapon2);
	}

	public override string GetNamePrefex()
	{
		return "ATV_mini";
	}

	public override string GetMoveSoundName()
	{
		return AudioName.ATV_MINI_MOVE;
	}

	public override string GetIdleSoundName()
	{
		return AudioName.ATV_MINI_IDLE;
	}

	protected override string GetOpenSoundName()
	{
		return AudioName.ATV_MINI_OPEN;
	}

	public override void StopAllSound()
	{
		base.StopAllSound();
		AudioManager.GetInstance().StopSound(AudioName.ATV_MINI_IDLE, rider.GetObject());
		AudioManager.GetInstance().StopSound(AudioName.ATV_MINI_MOVE, rider.GetObject());
	}

	public override void Loop()
	{
		base.Loop();
		if (mState == EInnerState.GetOn && rider != null && rider.InputController.inputInfo.IsMoving() && rider.inputController.inputInfo.dir == MoveDirection.Backward)
		{
		}
	}

	public override void AdjustAnimationInLateUpdate(float deltaTime)
	{
		float num = 0.7f;
		foreach (Transform mUpDownTransform in mUpDownTransformList)
		{
			mUpDownTransform.localRotation = Quaternion.Euler(mUpDownTransform.localRotation.eulerAngles.x - rider.AngleV * num, 0f, 0f);
		}
	}
}
