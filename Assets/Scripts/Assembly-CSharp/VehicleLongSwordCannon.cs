using System.Collections.Generic;
using UnityEngine;

public class VehicleLongSwordCannon : Cannon
{
	private const float mHorizontalLimitAngle = 70f;

	private Player mMainRider;

	protected Shader mMainRiderOldShader;

	protected Transform cannonTransform;

	public override void Init(GameObject gObject)
	{
		vehicleType = EVehicleType.LONG_SWORD_CANNON;
		mOpenDistance = 3f;
		StartMoveTime = 0f;
		base.Init(gObject);
		AttachedLocalPosition = entityTransform.Find("root/Root/man02").localPosition;
		mRotationTransform = entityTransform.Find("root/man02_rotation");
		mHitObject = entityObject;
		mUpDownTransformList = new List<Transform>();
		mUpDownTransformList.Add(entityTransform.Find("root/Root/Bone02/Point06/Bone01/Point_Gun_B_L"));
		mUpDownTransformList.Add(entityTransform.Find("root/Root/Bone02/Point06/Bone01/Point_Gun_B_R"));
		mLeftHand = entityTransform.Find("root/Root/Bone02/Point06/Bone01/Point_Gun_B_L/PointHandleL02");
		mRightHand = entityTransform.Find("root/Root/Bone02/Point06/Bone01/Point_Gun_B_R/PointHandleR02");
		cannonTransform = entityTransform.Find("root/Root/Bone02/Point06/Bone01");
		animation[AnimationString.VEHICLE_SHOOT + "_idle02"].layer = 1;
		animation[AnimationString.VEHICLE_SHOOT + "_idle02"].AddMixingTransform(cannonTransform);
	}

	public override void GetOn()
	{
		mState = EInnerState.GetOn;
		if (mMainRider != null && mMainRider.mVehicle != null)
		{
			deadTime = mMainRider.mVehicle.GetDeadTime();
		}
		rider.GetObject().layer = PhysicsLayer.DEFAULT;
		rider.SetState(Player.GET_ON_CARRIER_STATE);
		rider.GetWeapon().GunOff();
		rider.StopWalkSound();
		rider.GetCharacterController().enabled = false;
		if (rider.GetNavMeshObstacle() != null)
		{
			rider.GetNavMeshObstacle().radius = 3f;
		}
		InitWeapons();
		rider.mVehicleState = EPlayerVehicleState.CANNON;
		mRotationTransform.localRotation = Quaternion.Euler(0f, 180f, 0f);
		if (rider.IsLocal())
		{
			GameApp.GetInstance().GetGameWorld().GetCamera()
				.ChangeTarget(mRotationTransform, 70f);
		}
		AttachRider();
	}

	protected override void InitWeapons()
	{
		mAggressiveWeaponList = new List<VehicleWeapon>();
		VehicleWeapon vehicleWeapon = new LongSwordMachineGun();
		vehicleWeapon.Init(rider, mBaseDamage1, mDamageRatio1, mDamageRadius1, mAttackFrequency1, mDamageType1, mBulletSpeed1);
		vehicleWeapon.AddGunPoint(entityTransform.Find("root/Root/Bone02/Point06/Bone01/Point_Gun_B_L/Bone14/Point_flame04"));
		vehicleWeapon.AddGunPoint(entityTransform.Find("root/Root/Bone02/Point06/Bone01/Point_Gun_B_R/Bone17/Point_flame03"));
		vehicleWeapon.CreateGunEffect();
		mAggressiveWeaponList.Add(vehicleWeapon);
	}

	public override string GetNamePrefex()
	{
		return "UNCC_BUDDY02";
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

	public override void AdjustAnimationInLateUpdate(float deltaTime)
	{
		if (mState == EInnerState.Close)
		{
			return;
		}
		float num = 1f;
		if (rider.AngleV < 0f)
		{
			num = 0.45f;
		}
		foreach (Transform mUpDownTransform in mUpDownTransformList)
		{
			mUpDownTransform.rotation = mRotationTransform.rotation * Quaternion.Euler(-90f, -90f, -90f) * Quaternion.Euler(rider.AngleV * num, 0f, 0f);
		}
		if (mState == EInnerState.FinishGetOn)
		{
			rider.GetTransform().position = entityTransform.Find("root/Root/man02").position;
		}
	}

	public override Vector3 GetForward()
	{
		if (mMainRider != null && mMainRider.mVehicle != null)
		{
			return -mMainRider.mVehicle.GetTransform().forward;
		}
		return mRotationTransform.forward;
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
				GetTransform().position = hitInfo.point + new Vector3(0f, 1.1f, 0f);
			}
			else
			{
				GetTransform().position = rider.GetTransform().position + new Vector3(0f, 1.1f, 0f);
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

	public Player GetMainRider()
	{
		return mMainRider;
	}

	public void SetMainRider(Player mainRider, Player subRider)
	{
		mMainRider = mainRider;
		rider = subRider;
		subRider.mVehicle = this;
		if (rider.IsLocal())
		{
			rider.SetState(Player.FREEZE_STATE);
		}
	}

	public override void Loop()
	{
		if (mState == EInnerState.Open)
		{
			if (AnimationPlayed(AnimationString.VEHICLE_OPEN, 0.6f) && rider.InPlayingState())
			{
				GetOn();
			}
		}
		else if (mState == EInnerState.GetOn)
		{
			if (mMainRider == null || mMainRider.mVehicle == null)
			{
				if (rider.IsLocal())
				{
					GetOff();
					ThirdPersonStandardCameraScript component = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
					component.Enable = true;
				}
				entityObject.SetActive(false);
			}
		}
		else if (mState == EInnerState.FinishGetOn)
		{
			if (mMainRider == null || mMainRider.mVehicle == null)
			{
				if (rider.IsLocal())
				{
					GetOff();
					ThirdPersonStandardCameraScript component2 = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
					component2.Enable = true;
				}
				entityObject.SetActive(false);
				return;
			}
			if (Time.time - mFinishGetOnTime < 2f)
			{
				Vector3 vector = Vector3.RotateTowards(rider.GetTransform().forward, GetForward(), 3f * Time.deltaTime, 10000f);
				rider.GetTransform().LookAt(rider.GetPosition() + vector);
				Vector3 localPosition = Vector3.MoveTowards(rider.GetTransform().localPosition, AttachedLocalPosition, 5f * Time.deltaTime);
				rider.GetTransform().localPosition = localPosition;
				if (rider.IsLocal())
				{
					HideHead(Mathf.Lerp(1f, 0f, Time.time - mFinishGetOnTime));
				}
			}
			if (rider.IsLocal())
			{
				foreach (VehicleWeapon mPassiveWeapon in mPassiveWeaponList)
				{
					mPassiveWeapon.Attack();
				}
			}
			AutoDestroyGunEffect();
		}
		else if (mState == EInnerState.Close)
		{
			animation.Stop(AnimationString.VEHICLE_SHOOT + "_idle02");
			if (rider.IsLocal() && HUDBase.instance != null)
			{
				HUDBase.instance.m_Aim.ChangeWeapon(rider.GetWeapon().AimSpriteString);
			}
		}
	}

	public void ForceGetOff()
	{
		mState = EInnerState.Close;
		rider.mVehicleState = EPlayerVehicleState.NONE;
		StopAllSound();
		StopGunEffect();
		rider.SetState(Player.GET_OFF_CARRIER_STATE);
		rider.GetTransform().parent = null;
		rider.GetCharacterController().enabled = true;
		if (rider.GetNavMeshObstacle() != null)
		{
			rider.GetNavMeshObstacle().radius = 1f;
		}
		if (rider.IsLocal())
		{
			Ray ray = new Ray(rider.GetPosition() + new Vector3(0f, 1f, 0f), Vector3.down);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 2f, 1 << PhysicsLayer.FLOOR))
			{
				rider.GetTransform().position = hitInfo.point + new Vector3(0f, 0.1f, 0f);
			}
			rider.GetObject().layer = PhysicsLayer.PLAYER;
			HUDBase.instance.HideShowVehicleGetOff(false);
			GameApp.GetInstance().GetGameWorld().GetCamera()
				.ChangeTarget(rider.GetTransform());
			if (rider.IsLocal() && HUDBase.instance != null)
			{
				HUDBase.instance.m_Aim.ChangeWeapon(rider.GetWeapon().AimSpriteString);
			}
		}
		if (rider.IsLocal())
		{
			ShowHead();
		}
		if (mMainRider != null && mMainRider.mVehicle != null)
		{
			VehicleLongSword vehicleLongSword = mMainRider.mVehicle as VehicleLongSword;
			vehicleLongSword.SubRiderGetOff();
		}
	}

	public override void GetOff()
	{
		mState = EInnerState.Close;
		rider.mVehicleState = EPlayerVehicleState.NONE;
		StopAllSound();
		StopGunEffect();
		rider.SetState(Player.GET_OFF_CARRIER_STATE);
		rider.GetTransform().parent = null;
		rider.GetCharacterController().enabled = true;
		if (rider.GetNavMeshObstacle() != null)
		{
			rider.GetNavMeshObstacle().radius = 1f;
		}
		rider.GetObject().tag = TagName.PLAYER_ROOT;
		if (rider.IsLocal())
		{
			Ray ray = new Ray(rider.GetPosition() + new Vector3(0f, 1f, 0f), Vector3.down);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 2f, 1 << PhysicsLayer.FLOOR))
			{
				rider.GetTransform().position = hitInfo.point + new Vector3(0f, 0.1f, 0f);
			}
			rider.GetObject().layer = PhysicsLayer.PLAYER;
			HUDBase.instance.HideShowVehicleGetOff(false);
			GameApp.GetInstance().GetGameWorld().GetCamera()
				.ChangeTarget(rider.GetTransform());
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				VehicleGetOffRequest request = new VehicleGetOffRequest(true);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			if (rider.IsLocal() && HUDBase.instance != null)
			{
				HUDBase.instance.m_Aim.ChangeWeapon(rider.GetWeapon().AimSpriteString);
			}
		}
		else
		{
			rider.GetObject().layer = PhysicsLayer.REMOTE_PLAYER;
		}
		if (rider.IsLocal())
		{
			ShowHead();
		}
		if (mMainRider != null && mMainRider.mVehicle != null)
		{
			VehicleLongSword vehicleLongSword = mMainRider.mVehicle as VehicleLongSword;
			vehicleLongSword.SubRiderGetOff();
		}
	}

	public override void DeadOff()
	{
		mState = EInnerState.Close;
		rider.mVehicleState = EPlayerVehicleState.NONE;
		StopAllSound();
		StopGunEffect();
		rider.GetTransform().parent = null;
		rider.GetCharacterController().enabled = true;
		if (rider.GetNavMeshObstacle() != null)
		{
			rider.GetNavMeshObstacle().radius = 1f;
		}
		rider.GetObject().tag = TagName.PLAYER_ROOT;
		if (rider.IsLocal())
		{
			rider.GetObject().layer = PhysicsLayer.PLAYER;
			HUDBase.instance.HideShowVehicleGetOff(false);
			GameApp.GetInstance().GetGameWorld().GetCamera()
				.ChangeTarget(rider.GetTransform());
			if (rider.IsLocal() && HUDBase.instance != null)
			{
				HUDBase.instance.m_Aim.ChangeWeapon(rider.GetWeapon().AimSpriteString);
			}
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				VehicleGetOffRequest request = new VehicleGetOffRequest(false);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
		else
		{
			rider.GetObject().layer = PhysicsLayer.REMOTE_PLAYER;
		}
		if (rider.IsLocal())
		{
			ShowHead();
		}
		if (mMainRider != null && mMainRider.mVehicle != null)
		{
			VehicleLongSword vehicleLongSword = mMainRider.mVehicle as VehicleLongSword;
			vehicleLongSword.SubRiderGetOff();
		}
	}

	public override void FinishGetOn()
	{
		mFinishGetOnTime = Time.time;
		mState = EInnerState.FinishGetOn;
		if (rider.IsLocal())
		{
			HUDBase.instance.HideShowVehicleGetOff();
		}
		rider.GetObject().tag = TagName.VEHICLE_ROOT;
		if (rider.IsLocal() && HUDBase.instance != null)
		{
			HUDBase.instance.m_Aim.ChangeWeapon("front star13");
		}
	}

	public override void HideHead(float alpha)
	{
		if (alpha == 0f)
		{
			rider.GetHeadObj().GetComponent<Renderer>().enabled = false;
			rider.GetBodyObj().GetComponent<Renderer>().enabled = false;
			rider.GetHandObj().GetComponent<Renderer>().enabled = false;
			rider.GetFootObj().GetComponent<Renderer>().enabled = false;
			rider.GetBagObj().SetActive(false);
		}
		if (rider.GetHeadObj().GetComponent<Renderer>().material.shader != Shader.Find("StarWarfare/SNDM_new_Alpha"))
		{
			oldShader = rider.GetHeadObj().GetComponent<Renderer>().material.shader;
		}
		rider.GetHeadObj().GetComponent<Renderer>().material.shader = Shader.Find("StarWarfare/SNDM_new_Alpha");
		rider.GetHeadObj().GetComponent<Renderer>().material.SetFloat("_Alpha", alpha);
		Material[] materials = rider.GetBodyObj().GetComponent<Renderer>().materials;
		Material[] array = materials;
		foreach (Material material in array)
		{
			material.shader = Shader.Find("StarWarfare/SNDM_new_Alpha");
			material.SetFloat("_Alpha", alpha);
		}
		rider.GetHandObj().GetComponent<Renderer>().material.shader = Shader.Find("StarWarfare/SNDM_new_Alpha");
		rider.GetHandObj().GetComponent<Renderer>().material.SetFloat("_Alpha", alpha);
		rider.GetFootObj().GetComponent<Renderer>().material.shader = Shader.Find("StarWarfare/SNDM_new_Alpha");
		rider.GetFootObj().GetComponent<Renderer>().material.SetFloat("_Alpha", alpha);
		rider.GetBagObj().SetActive(false);
		if (mMainRider != null)
		{
			if (alpha == 0f)
			{
				mMainRider.GetHeadObj().GetComponent<Renderer>().enabled = false;
				mMainRider.GetBodyObj().GetComponent<Renderer>().enabled = false;
				mMainRider.GetHandObj().GetComponent<Renderer>().enabled = false;
				mMainRider.GetFootObj().GetComponent<Renderer>().enabled = false;
				mMainRider.GetBagObj().SetActive(false);
			}
			if (mMainRider.GetHeadObj().GetComponent<Renderer>().material.shader != Shader.Find("StarWarfare/SNDM_new_Alpha"))
			{
				mMainRiderOldShader = mMainRider.GetHeadObj().GetComponent<Renderer>().material.shader;
			}
			mMainRider.GetHeadObj().GetComponent<Renderer>().material.shader = Shader.Find("StarWarfare/SNDM_new_Alpha");
			mMainRider.GetHeadObj().GetComponent<Renderer>().material.SetFloat("_Alpha", alpha);
			materials = mMainRider.GetBodyObj().GetComponent<Renderer>().materials;
			Material[] array2 = materials;
			foreach (Material material2 in array2)
			{
				material2.shader = Shader.Find("StarWarfare/SNDM_new_Alpha");
				material2.SetFloat("_Alpha", alpha);
			}
			mMainRider.GetHandObj().GetComponent<Renderer>().material.shader = Shader.Find("StarWarfare/SNDM_new_Alpha");
			mMainRider.GetHandObj().GetComponent<Renderer>().material.SetFloat("_Alpha", alpha);
			mMainRider.GetFootObj().GetComponent<Renderer>().material.shader = Shader.Find("StarWarfare/SNDM_new_Alpha");
			mMainRider.GetFootObj().GetComponent<Renderer>().material.SetFloat("_Alpha", alpha);
			mMainRider.GetBagObj().SetActive(false);
		}
	}

	public override void ShowHead()
	{
		if (oldShader == null)
		{
			oldShader = Shader.Find("StarWarfare/SNDM_new");
		}
		rider.GetHeadObj().GetComponent<Renderer>().material.shader = oldShader;
		Material[] materials = rider.GetBodyObj().GetComponent<Renderer>().materials;
		Material[] array = materials;
		foreach (Material material in array)
		{
			material.shader = oldShader;
		}
		rider.GetHandObj().GetComponent<Renderer>().material.shader = oldShader;
		rider.GetFootObj().GetComponent<Renderer>().material.shader = oldShader;
		rider.GetBagObj().SetActive(true);
		rider.GetHeadObj().GetComponent<Renderer>().enabled = true;
		rider.GetBodyObj().GetComponent<Renderer>().enabled = true;
		rider.GetHandObj().GetComponent<Renderer>().enabled = true;
		rider.GetFootObj().GetComponent<Renderer>().enabled = true;
		oldShader = null;
		if (mMainRider != null)
		{
			if (mMainRiderOldShader == null)
			{
				mMainRiderOldShader = Shader.Find("StarWarfare/SNDM_new");
			}
			mMainRider.GetHeadObj().GetComponent<Renderer>().material.shader = mMainRiderOldShader;
			materials = mMainRider.GetBodyObj().GetComponent<Renderer>().materials;
			Material[] array2 = materials;
			foreach (Material material2 in array2)
			{
				material2.shader = mMainRiderOldShader;
			}
			mMainRider.GetHandObj().GetComponent<Renderer>().material.shader = mMainRiderOldShader;
			mMainRider.GetFootObj().GetComponent<Renderer>().material.shader = mMainRiderOldShader;
			mMainRider.GetBagObj().SetActive(true);
			mMainRider.GetHeadObj().GetComponent<Renderer>().enabled = true;
			mMainRider.GetBodyObj().GetComponent<Renderer>().enabled = true;
			mMainRider.GetHandObj().GetComponent<Renderer>().enabled = true;
			mMainRider.GetFootObj().GetComponent<Renderer>().enabled = true;
			mMainRiderOldShader = null;
		}
	}

	public override void AttachRider()
	{
		if (rider != null)
		{
			rider.GetTransform().parent = GetTransform();
		}
	}

	public override void PlayAnimation(string name, WrapMode mode, float blendTime)
	{
		if (name.Contains("shoot"))
		{
			name = AnimationString.VEHICLE_SHOOT + "_idle02";
			base.PlayAnimation(name, mode, blendTime);
		}
		else
		{
			animation.Stop(AnimationString.VEHICLE_SHOOT + "_idle02");
		}
	}

	public override float GetDeltaHeight()
	{
		return 0.6f;
	}
}
