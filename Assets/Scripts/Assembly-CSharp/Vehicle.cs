using System.Collections.Generic;
using UnityEngine;

public class Vehicle : GameUnit
{
	public enum EDataColumn
	{
		TYPE = 0,
		TIME = 1,
		SPEED = 2,
		SPEED_UPDATE = 3,
		SPEED_DOWN_UPDATE = 4,
		BASE_DAMAGE_1 = 5,
		DAMAGE_RATIO_1 = 6,
		DAMAGE_RADIUS1 = 7,
		FREQUENCY_1 = 8,
		DAMAGE_TYPE_1 = 9,
		BULLET_SPEED_1 = 10,
		BASE_DAMAGE_2 = 11,
		DAMAGE_RATIO_2 = 12,
		DAMAGE_RADIUS2 = 13,
		FREQUENCY_2 = 14,
		DAMAGE_TYPE_2 = 15,
		BULLET_SPEED_2 = 16,
		ID = 17,
		DEFENSE_ANGLE = 18,
		DEFENSE_RATE = 19
	}

	public enum EInnerState
	{
		Open = 0,
		GetOn = 1,
		FinishGetOn = 2,
		Close = 3
	}

	public float speedUpdate = 4f;

	public float speedDownUpdate = 4f;

	public float StartMoveTime;

	public Vector3 AttachedLocalPosition;

	protected float mOpenDistance;

	protected EVehicleType vehicleType;

	protected Player rider;

	protected float life = 15f;

	protected float deadTime;

	protected float mBaseDamage1;

	protected float mDamageRatio1;

	protected float mDamageRadius1;

	protected float mAttackFrequency1;

	protected PlayerDamageType mDamageType1;

	protected float mBulletSpeed1;

	protected float mBaseDamage2;

	protected float mDamageRatio2;

	protected float mDamageRadius2;

	protected float mAttackFrequency2;

	protected PlayerDamageType mDamageType2;

	protected float mBulletSpeed2;

	protected List<VehicleWeapon> mAggressiveWeaponList = new List<VehicleWeapon>();

	protected List<VehicleWeapon> mPassiveWeaponList = new List<VehicleWeapon>();

	protected EInnerState mState;

	protected Transform mRotationTransform;

	protected List<Transform> mUpDownTransformList;

	protected GameObject mHitObject;

	protected Transform mLeftHand;

	protected Transform mRightHand;

	protected float mFinishGetOnTime;

	protected int defenseAngle;

	protected float defenseRate;

	public EVehicleType VehicleType
	{
		get
		{
			return vehicleType;
		}
	}

	public Transform LeftHand
	{
		get
		{
			return mLeftHand;
		}
	}

	public Transform RightHand
	{
		get
		{
			return mRightHand;
		}
	}

	public virtual string GetNamePrefex()
	{
		return string.Empty;
	}

	public virtual string GetMoveSoundName()
	{
		return string.Empty;
	}

	public virtual string GetIdleSoundName()
	{
		return string.Empty;
	}

	protected virtual string GetOpenSoundName()
	{
		return string.Empty;
	}

	public virtual void StopAllSound()
	{
		EndLoopingSound();
	}

	public virtual void Init(GameObject gObject)
	{
		SetObject(gObject);
		mRotationTransform = entityTransform;
		base.Name = entityObject.name;
		rigidbody = entityObject.GetComponent<Rigidbody>();
		animation = entityObject.transform.Find("root").GetComponent<Animation>();
		mState = EInnerState.Open;
		LoadConfig();
		Renderer[] componentsInChildren = entityObject.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			renderer.gameObject.layer = PhysicsLayer.VEHICLE;
		}
	}

	public virtual void Loop()
	{
		if (mState == EInnerState.Open)
		{
			if (AnimationPlayed(AnimationString.VEHICLE_OPEN, 0.6f) && rider.InPlayingState())
			{
				GetOn();
			}
		}
		else
		{
			if (mState == EInnerState.GetOn)
			{
				return;
			}
			if (mState == EInnerState.FinishGetOn)
			{
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
				if (Time.time > deadTime)
				{
					if (rider.IsLocal())
					{
						GetOff();
					}
					return;
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
			else if (mState == EInnerState.Close && IsAnimationFinish())
			{
				entityObject.SetActive(false);
				rider.mVehicle = null;
				if (rider.IsLocal() && HUDBase.instance != null)
				{
					HUDBase.instance.m_Aim.ChangeWeapon(rider.GetWeapon().AimSpriteString);
				}
			}
		}
	}

	public virtual void LoadConfig()
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.EQUIPMENT, DataSheetName.VEHICLE);
		life = gameDataTable.GetFloat((int)vehicleType, 1);
		Speed = gameDataTable.GetFloat((int)vehicleType, 2);
		speedUpdate = gameDataTable.GetFloat((int)vehicleType, 3);
		speedDownUpdate = gameDataTable.GetFloat((int)vehicleType, 4);
		mBaseDamage1 = gameDataTable.GetFloat((int)vehicleType, 5);
		mDamageRatio1 = gameDataTable.GetFloat((int)vehicleType, 6);
		mDamageRadius1 = gameDataTable.GetFloat((int)vehicleType, 7);
		mAttackFrequency1 = gameDataTable.GetFloat((int)vehicleType, 8);
		mDamageType1 = (PlayerDamageType)gameDataTable.GetFloat((int)vehicleType, 9);
		mBulletSpeed1 = gameDataTable.GetFloat((int)vehicleType, 10);
		mBaseDamage2 = gameDataTable.GetFloat((int)vehicleType, 11);
		mDamageRatio2 = gameDataTable.GetFloat((int)vehicleType, 12);
		mDamageRadius2 = gameDataTable.GetFloat((int)vehicleType, 13);
		mAttackFrequency2 = gameDataTable.GetFloat((int)vehicleType, 14);
		mDamageType2 = (PlayerDamageType)gameDataTable.GetFloat((int)vehicleType, 15);
		mBulletSpeed2 = gameDataTable.GetFloat((int)vehicleType, 16);
		defenseAngle = gameDataTable.GetInt((int)vehicleType, 18);
		defenseRate = gameDataTable.GetFloat((int)vehicleType, 19);
	}

	public void SetRemoteRider(RemotePlayer player)
	{
		rider = player;
		player.mVehicle = this;
		entityObject.transform.position = player.GetPosition();
		entityObject.transform.eulerAngles = player.GetTransform().eulerAngles;
		Speed += player.GetSkills().GetSkill(SkillsType.VEHICLE_SPEED);
		life *= 1f + player.GetSkills().GetSkill(SkillsType.VEHICLE_ENERGY);
		if (player.GetWeapon() != null)
		{
			player.GetWeapon().StopFire();
			player.GetWeapon().StopSound();
		}
	}

	public virtual void Open(Player player)
	{
		rider = player;
		player.mVehicle = this;
		if (rider != null)
		{
			Vector3 origin = rider.GetTransform().position + rider.GetTransform().forward * mOpenDistance + Vector3.up * 2f;
			Ray ray = new Ray(origin, Vector3.down);
			Ray ray2 = new Ray(rider.GetTransform().position + Vector3.up, rider.GetTransform().forward);
			RaycastHit hitInfo = default(RaycastHit);
			if (Physics.Raycast(ray, out hitInfo, 5f, 1 << PhysicsLayer.FLOOR) && !Physics.Raycast(ray2, mOpenDistance + 0.5f, (1 << PhysicsLayer.WALL_AGAINST_PALYER) | (1 << PhysicsLayer.TRANSPARENT_WALL)))
			{
				GetTransform().position = hitInfo.point + new Vector3(0f, 0.1f, 0f);
			}
			else
			{
				GetTransform().position = rider.GetTransform().position + new Vector3(0f, 0.1f, 0f);
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

	public void ResetDeadTime()
	{
		deadTime = Time.time + life;
	}

	public virtual void GetOn()
	{
		mState = EInnerState.GetOn;
		deadTime = Time.time + life;
		rider.GetObject().layer = PhysicsLayer.DEFAULT;
		rider.SetState(Player.GET_ON_CARRIER_STATE);
		rider.GetWeapon().GunOff();
		rider.StopWalkSound();
		rider.GetCharacterController().enabled = false;
		if (rider.GetNavMeshObstacle() != null)
		{
			rider.GetNavMeshObstacle().radius = 2f;
		}
		InitWeapons();
	}

	public virtual void FinishGetOn()
	{
		mFinishGetOnTime = Time.time;
		mState = EInnerState.FinishGetOn;
		if (rider.IsLocal())
		{
			HUDBase.instance.HideShowVehicleGetOff();
			mHitObject.layer = PhysicsLayer.PLAYER;
			mHitObject.tag = TagName.VEHICLE_ROOT;
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				PlayerGetOnVehicleRequest request = new PlayerGetOnVehicleRequest(vehicleType);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
		else
		{
			mHitObject.layer = PhysicsLayer.REMOTE_PLAYER;
			mHitObject.tag = TagName.VEHICLE_ROOT;
		}
		AttachRider();
		if (rider.IsLocal() && HUDBase.instance != null)
		{
			if (rider.mVehicleState == EPlayerVehicleState.CARRIER)
			{
				HUDBase.instance.m_Aim.ChangeWeapon("front star12");
			}
			else if (rider.mVehicleState == EPlayerVehicleState.CANNON)
			{
				HUDBase.instance.m_Aim.ChangeWeapon("front star13");
			}
		}
	}

	public virtual void AttachRider()
	{
		if (rider != null)
		{
			GetTransform().position = rider.GetTransform().position;
			GetTransform().rotation = rider.GetTransform().rotation;
			rider.GetTransform().parent = GetTransform();
		}
	}

	protected virtual void InitWeapons()
	{
	}

	public virtual void GetOff()
	{
		if (mState == EInnerState.Open)
		{
			float normalizedTime = animation[AnimationString.VEHICLE_OPEN].normalizedTime;
			PlayAnimation(AnimationString.VEHICLE_OPEN, WrapMode.ClampForever, 0.5f, -2f);
			animation[AnimationString.VEHICLE_OPEN].normalizedTime = normalizedTime;
		}
		else
		{
			PlayAnimation(AnimationString.VEHICLE_OPEN, WrapMode.ClampForever, 0.5f, -2f);
		}
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
		mHitObject.layer = PhysicsLayer.DEFAULT;
		if (rider.IsLocal())
		{
			Ray ray = new Ray(rider.GetPosition() + new Vector3(0f, 1f, 0f), Vector3.down);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 5f, 1 << PhysicsLayer.FLOOR))
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
			((RemotePlayer)rider).GetInterpolation().SetTransform(rider.GetTransform());
		}
	}

	public virtual void DeadOff()
	{
		GameObject original = Resources.Load("_Old/_Effect/RPG_EXP_87") as GameObject;
		Object.Instantiate(original, GetPosition(), Quaternion.identity);
		mState = EInnerState.Close;
		rider.mVehicleState = EPlayerVehicleState.NONE;
		entityObject.SetActive(false);
		StopAllSound();
		StopGunEffect();
		rider.GetTransform().parent = null;
		rider.GetCharacterController().enabled = true;
		if (rider.GetNavMeshObstacle() != null)
		{
			rider.GetNavMeshObstacle().radius = 1f;
		}
		mHitObject.layer = PhysicsLayer.DEFAULT;
		if (rider.IsLocal())
		{
			Ray ray = new Ray(rider.GetPosition() + new Vector3(0f, 1f, 0f), Vector3.down);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 5f, 1 << PhysicsLayer.FLOOR))
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
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				VehicleGetOffRequest request = new VehicleGetOffRequest(false);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
		else
		{
			rider.GetObject().layer = PhysicsLayer.REMOTE_PLAYER;
			((RemotePlayer)rider).GetInterpolation().SetTransform(rider.GetTransform());
		}
	}

	public new virtual void Move(Vector3 motion)
	{
		if (cc != null)
		{
			cc.Move(motion * Speed * Time.deltaTime);
		}
	}

	public new virtual bool IsPlayingAnimation(string name)
	{
		return animation.IsPlaying(name);
	}

	public void PlayAnimation(string name, WrapMode mode, float blendTime, float speed)
	{
		PlayAnimation(name, mode, blendTime);
		animation[name].speed = speed;
		if (speed < 0f)
		{
			animation[name].normalizedTime = 1f;
		}
		else
		{
			animation[name].normalizedTime = 0f;
		}
	}

	public new virtual void PlayAnimation(string name, WrapMode mode, float blendTime)
	{
		if (!animation.IsPlaying(name) || mode != WrapMode.ClampForever || AnimationPlayed(name, 1f))
		{
			animation[name].wrapMode = mode;
			animation.CrossFade(name, blendTime);
			base.CurrentAnimationName = name;
		}
	}

	public new virtual void PlayAnimation(string name, WrapMode mode)
	{
		PlayAnimation(name, mode, 0.2f);
	}

	public new virtual void PlayAnimationWithoutBlend(string name, WrapMode mode)
	{
		if (!animation.IsPlaying(name) || mode != WrapMode.ClampForever || AnimationPlayed(name, 1f))
		{
			animation[name].wrapMode = mode;
			animation.Play(name);
			base.CurrentAnimationName = name;
		}
	}

	public new virtual void PlayAnimationAllLayers(string name, WrapMode mode)
	{
		if (!animation.IsPlaying(name) || mode != WrapMode.ClampForever)
		{
			animation[name].wrapMode = mode;
			animation.CrossFade(name, 0.1f, PlayMode.StopAll);
			base.CurrentAnimationName = name;
		}
	}

	public new virtual void StopAnimation(string name)
	{
		animation.Stop(name);
	}

	public new bool AnimationPlayed(string name, float percent)
	{
		if (animation[name] == null)
		{
			return false;
		}
		if (animation[name].speed >= 0f)
		{
			if (animation[name].time >= animation[name].clip.length * percent)
			{
				return true;
			}
			return false;
		}
		if (animation[name].time <= animation[name].clip.length * (1f - percent))
		{
			return true;
		}
		return false;
	}

	public bool CoolDown()
	{
		if (mAggressiveWeaponList.Count > 0)
		{
			return mAggressiveWeaponList[0].CoolDown();
		}
		return false;
	}

	public bool IsTypeOfLoopShooting()
	{
		if (mAggressiveWeaponList.Count > 0)
		{
			return mAggressiveWeaponList[0].IsTypeOfLoopShooting();
		}
		return false;
	}

	public void Attack()
	{
		foreach (VehicleWeapon mAggressiveWeapon in mAggressiveWeaponList)
		{
			mAggressiveWeapon.Attack();
		}
	}

	public void StartLoopingSound()
	{
		foreach (VehicleWeapon mAggressiveWeapon in mAggressiveWeaponList)
		{
			mAggressiveWeapon.StartLoopingSound();
		}
	}

	public void EndLoopingSound()
	{
		foreach (VehicleWeapon mAggressiveWeapon in mAggressiveWeaponList)
		{
			mAggressiveWeapon.EndLoopingSound();
		}
	}

	public void CreateTrajectory()
	{
		foreach (VehicleWeapon mAggressiveWeapon in mAggressiveWeaponList)
		{
			mAggressiveWeapon.CreateTrajectory();
		}
	}

	public void StopGunEffect()
	{
		foreach (VehicleWeapon mAggressiveWeapon in mAggressiveWeaponList)
		{
			mAggressiveWeapon.StopGunEffect();
		}
		foreach (VehicleWeapon mPassiveWeapon in mPassiveWeaponList)
		{
			mPassiveWeapon.StopGunEffect();
		}
	}

	public void AutoDestroyGunEffect()
	{
		foreach (VehicleWeapon mAggressiveWeapon in mAggressiveWeaponList)
		{
			mAggressiveWeapon.AutoDestroyGunEffect();
		}
		foreach (VehicleWeapon mPassiveWeapon in mPassiveWeaponList)
		{
			mPassiveWeapon.AutoDestroyGunEffect();
		}
	}

	public Transform GetRotationTransform()
	{
		return mRotationTransform;
	}

	public virtual void AdjustAnimationInLateUpdate(float deltaTime)
	{
	}

	public virtual Vector3 GetForward()
	{
		return entityTransform.forward;
	}

	public virtual void HideHead(float alpha)
	{
	}

	public virtual float GetRemainTimePercentage()
	{
		return (deadTime - Time.time) / life;
	}

	public virtual bool AbsorbDamage(int damage, Vector3 attackerPosition)
	{
		if (defenseAngle == 360)
		{
			return true;
		}
		if (defenseAngle > 0 && Vector3.Angle(attackerPosition - rider.GetPosition(), rider.GetTransform().forward) <= (float)(defenseAngle / 2))
		{
			float num = (float)damage / (float)rider.MaxHp;
			deadTime -= num / (defenseRate / 100f);
			return true;
		}
		return false;
	}

	public virtual void ReduceDuringTime(float reducedTime)
	{
		deadTime -= reducedTime;
		rider.ShowDefent();
	}

	public float GetDeadTime()
	{
		return deadTime;
	}

	public virtual void ShowHead()
	{
	}

	public virtual float GetDeltaHeight()
	{
		return 0f;
	}
}
