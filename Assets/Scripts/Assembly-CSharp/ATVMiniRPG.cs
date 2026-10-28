using System.Collections.Generic;
using UnityEngine;

public class ATVMiniRPG : VehicleWeapon
{
	public override void Init(Player player, float baseDamage, float damageRatio, float damageRadius, float attackFrequency, PlayerDamageType damageType, float bulletSpeed)
	{
		base.Init(player, baseDamage, damageRatio, damageRadius, attackFrequency, damageType, bulletSpeed);
		mWeaponType = EVehicleWeaponType.ATV_MINI_RPG;
		base.ExplodeSoundName = AudioName.GUN_03_RPG_BOOM;
	}

	public override void CreateGunEffect()
	{
		GameObject original = Resources.Load(EffectName.V1_SPARK) as GameObject;
		mGunFireObjectList = new List<GameObject>();
		foreach (Transform mGunPointTransform in mGunPointTransformList)
		{
			GameObject gameObject = Object.Instantiate(original, mGunPointTransform.position, Quaternion.identity) as GameObject;
			gameObject.transform.parent = mGunPointTransform;
			mGunFireObjectList.Add(gameObject);
		}
		StopGunEffect();
	}

	public override bool IsTypeOfLoopShooting()
	{
		return true;
	}

	public override void StartLoopingSound()
	{
		AudioManager.GetInstance().PlaySoundSingleLoop(AudioName.ATV_MINI_SHOOT, mPlayer.GetObject(), mPlayer.GetTransform());
	}

	public override void EndLoopingSound()
	{
		AudioManager.GetInstance().StopSound(AudioName.ATV_MINI_SHOOT, mPlayer.GetObject());
	}

	public override void StopGunEffect()
	{
		foreach (GameObject mGunFireObject in mGunFireObjectList)
		{
			if (mGunFireObject != null)
			{
				mGunFireObject.SetActive(false);
			}
		}
	}

	public override void CreateTrajectory()
	{
		base.CreateTrajectory();
		for (int i = 0; i < mGunPointTransformList.Count; i++)
		{
			if (mGunPointTransformList[i] != null)
			{
				mGunFireObjectList[i].SetActive(true);
			}
		}
	}

	public override void Attack()
	{
		if (!CoolDown())
		{
			return;
		}
		mLastAttackTime = Time.time;
		Camera main = Camera.main;
		Transform transform = main.transform;
		ThirdPersonStandardCameraScript component = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
		Ray ray = default(Ray);
		Vector3 vector = main.ScreenToWorldPoint(new Vector3(component.ReticlePosition.x, (float)Screen.height - component.ReticlePosition.y, 0.1f));
		RaycastHit hitInfo = new RaycastHit
		{
			point = transform.position + (vector - transform.position).normalized * 80f
		};
		bool flag = true;
		Vector3 forward = transform.forward;
		forward.y = 0f;
		Vector3 forward2 = mPlayer.mVehicle.GetTransform().forward;
		forward2.y = 0f;
		if (Vector3.Angle(forward, forward2) > 30f)
		{
			flag = false;
		}
		for (int i = 0; i < mGunPointTransformList.Count; i++)
		{
			if (mGunPointTransformList[i] != null)
			{
				mGunFireObjectList[i].SetActive(true);
			}
			Vector3 normalized2;
			if (flag)
			{
				Vector3 normalized = (vector - transform.position).normalized;
				normalized2 = (hitInfo.point - mGunPointTransformList[i].position).normalized;
				ray = new Ray(transform.position + normalized * 1.8f, normalized);
			}
			else
			{
				normalized2 = mGunPointTransformList[i].forward;
				Vector3 vector2 = mGunPointTransformList[i].position + normalized2 * 80f;
				hitInfo.point = new Vector3(vector2.x, hitInfo.point.y, vector2.z);
				ray = new Ray(mGunPointTransformList[i].position, hitInfo.point - mGunPointTransformList[i].position);
			}
			Vector3 vector3 = ((!Physics.Raycast(ray, out hitInfo, 1000f, (1 << PhysicsLayer.ENEMY) | (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.TRANSPARENT_WALL) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.REMOTE_PLAYER))) ? transform.TransformPoint(0f, 0f, 1000f) : hitInfo.point);
			normalized2 = (vector3 - mGunPointTransformList[i].position).normalized;
			GameObject gameObject = GameApp.GetInstance().GetGameWorld().GetEffectPool(EffectPoolType.W8_FIST)
				.CreateObject(mGunPointTransformList[i].position, normalized2, Quaternion.identity);
			ProjectileScript component2 = gameObject.GetComponent<ProjectileScript>();
			component2.enabled = true;
			component2.isLocal = true;
			component2.dir = normalized2;
			component2.flySpeed = mBulletSpeed;
			component2.explodeRadius = mDamageRadius;
			component2.life = 10f;
			component2.damage = (int)mDamage;
			component2.GunType = WeaponType.NoGun;
			component2.VehicleWeaponType = EVehicleWeaponType.ATV_MINI_RPG;
			component2.targetPos = vector3;
			component2.vehicleWeapon = this;
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				PlayerFireRocketRequest request = new PlayerFireRocketRequest(EVehicleWeaponType.ATV_MINI_RPG, mGunPointTransformList[i].position, normalized2, mBulletSpeed);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
	}
}
