using System.Collections.Generic;
using UnityEngine;

public class VehicleLongSword : Carrier
{
	private Player mSubRider;

	public override void Init(GameObject gObject)
	{
		vehicleType = EVehicleType.LONG_SWORD;
		mOpenDistance = 3f;
		StartMoveTime = 11f / 63f;
		base.Init(gObject);
		AttachedLocalPosition = entityTransform.Find("root/Root/man01").localPosition;
		mHitObject = entityObject;
		mUpDownTransformList = new List<Transform>();
		mUpDownTransformList.Add(entityTransform.Find("root/Root/Bone02/Bone03/Point04/Bone04/Point15/Bone06/Point_Gun_F_R"));
		mUpDownTransformList.Add(entityTransform.Find("root/Root/Bone02/Bone03/Point04/Bone04/Point15/Bone06/Point_Gun_F_L"));
		mLeftHand = entityTransform.Find("root/Root/Bone02/Bone03/Point04/Bone04/Point15/Bone06/PointHandleL01");
		mRightHand = entityTransform.Find("root/Root/Bone02/Bone03/Point04/Bone04/Point15/Bone06/PointHandleR01");
	}

	protected override void InitWeapons()
	{
		VehicleWeapon vehicleWeapon = new ATVMiniAssaultRifle();
		vehicleWeapon.Init(rider, mBaseDamage1, mDamageRatio1, mDamageRadius1, mAttackFrequency1, mDamageType1, mBulletSpeed1);
		vehicleWeapon.AddGunPoint(entityTransform.Find("root/Root/Bone02/Bone03/Point04/Bone04/Point15/Bone06/Point_Gun_F_R/Bone19/Point_flame02"));
		vehicleWeapon.AddGunPoint(entityTransform.Find("root/Root/Bone02/Bone03/Point04/Bone04/Point15/Bone06/Point_Gun_F_L/Bone20/Point_flame01"));
		vehicleWeapon.CreateGunEffect();
		mAggressiveWeaponList.Add(vehicleWeapon);
	}

	public override void GetOn()
	{
		base.GetOn();
		if (rider.GetNavMeshObstacle() != null)
		{
			rider.GetNavMeshObstacle().radius = 3f;
		}
	}

	public override string GetNamePrefex()
	{
		return "UNCC_BUDDY01";
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
	}

	public override void AdjustAnimationInLateUpdate(float deltaTime)
	{
		if (mState == EInnerState.Close)
		{
			return;
		}
		float num = 1f;
		float num2 = ((!rider.IsLocal()) ? 0f : (-20f));
		foreach (Transform mUpDownTransform in mUpDownTransformList)
		{
			mUpDownTransform.localRotation = Quaternion.Euler(-90f, 0f, 90f) * Quaternion.Euler(num2 - rider.AngleV * num, 0f, 0f);
		}
		if (mState == EInnerState.FinishGetOn)
		{
			rider.GetTransform().position = entityTransform.Find("root/Root/man01").position;
		}
	}

	public Player GetSubRider()
	{
		return mSubRider;
	}

	public void SetSubRider(Player player)
	{
		if (player.GetWeapon() != null)
		{
			player.GetWeapon().StopFire();
			player.GetWeapon().StopSound();
		}
		mSubRider = player;
	}

	public override void Open(Player player)
	{
		rider = player;
		player.mVehicle = this;
		if (rider != null)
		{
			Vector3 origin = rider.GetTransform().position + rider.GetTransform().forward * mOpenDistance + Vector3.up * 3f;
			Ray ray = new Ray(origin, Vector3.down);
			Ray ray2 = new Ray(rider.GetTransform().position + Vector3.up, rider.GetTransform().forward);
			RaycastHit hitInfo = default(RaycastHit);
			if (Physics.Raycast(ray, out hitInfo, 5f, 1 << PhysicsLayer.FLOOR) && !Physics.Raycast(ray2, mOpenDistance + 0.5f, (1 << PhysicsLayer.WALL_AGAINST_PALYER) | (1 << PhysicsLayer.TRANSPARENT_WALL)))
			{
				GetTransform().position = hitInfo.point + new Vector3(0f, 0.5f, 0f);
			}
			else
			{
				GetTransform().position = rider.GetTransform().position + new Vector3(0f, 0.5f, 0f);
				Debug.Log("载具下面没有地面,或者中间有墙。");
			}
			GetTransform().localRotation = rider.GetTransform().localRotation;
			Speed += player.GetSkills().GetSkill(SkillsType.VEHICLE_SPEED);
			life *= 1f + player.GetSkills().GetSkill(SkillsType.VEHICLE_ENERGY);
		}
		PlayAnimation(AnimationString.VEHICLE_OPEN, WrapMode.ClampForever, 0.5f, 1f);
		AudioManager.GetInstance().PlaySoundSingleAt(GetOpenSoundName(), GetTransform().position);
		rider.SetState(Player.FREEZE_STATE);
	}

	public void SubRiderGetOff()
	{
		mSubRider = null;
	}

	public override void DeadOff()
	{
		base.DeadOff();
		if (mSubRider != null && mSubRider.mVehicle != null)
		{
			((VehicleLongSwordCannon)mSubRider.mVehicle).ForceGetOff();
		}
	}

	public override float GetDeltaHeight()
	{
		return 0.6f;
	}

	public bool CanGetOn()
	{
		return mState == EInnerState.FinishGetOn;
	}
}
