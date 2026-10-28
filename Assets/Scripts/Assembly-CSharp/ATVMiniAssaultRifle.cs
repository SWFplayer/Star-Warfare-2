using System.Collections.Generic;
using UnityEngine;

public class ATVMiniAssaultRifle : VehicleWeapon
{
	public override void Init(Player player, float baseDamage, float damageRatio, float damageRadius, float attackFrequency, PlayerDamageType damageType, float bulletSpeed)
	{
		base.Init(player, baseDamage, damageRatio, damageRadius, attackFrequency, damageType, bulletSpeed);
		mWeaponType = EVehicleWeaponType.ATV_MINI_ASSAULT_RIFLE;
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
			mGunPointTransformList[i].rotation = mPlayer.GetTransform().rotation;
			mGunPointTransformList[i].rotation = Quaternion.AngleAxis(mPlayer.AngleV, -mGunPointTransformList[i].right) * mGunPointTransformList[i].rotation;
			Vector3 normalized = mGunPointTransformList[i].transform.forward.normalized;
			GameObject gameObject = GameApp.GetInstance().GetGameWorld().GetEffectPool(EffectPoolType.V1_TRAJECTORY)
				.CreateObject(mGunPointTransformList[i].transform.position, normalized, Quaternion.identity);
			if (gameObject != null)
			{
				FireLineScript component = gameObject.GetComponent<FireLineScript>();
				component.beginPos = mGunPointTransformList[i].position;
				component.endPos = mGunPointTransformList[i].position + normalized * 100f;
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
		RaycastHit raycastHit = new RaycastHit
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
			List<Ray> list = new List<Ray>();
			Vector3 vector2;
			if (flag)
			{
				Vector3 normalized = (vector - transform.position).normalized;
				vector2 = (raycastHit.point - mGunPointTransformList[i].position).normalized;
				ray = new Ray(transform.position + normalized * 1.8f, normalized);
				list.Add(ray);
			}
			else
			{
				vector2 = mGunPointTransformList[i].forward;
				Vector3 vector3 = mGunPointTransformList[i].position + vector2 * 80f;
				raycastHit.point = new Vector3(vector3.x, raycastHit.point.y, vector3.z);
				ray = new Ray(mGunPointTransformList[i].position, raycastHit.point - mGunPointTransformList[i].position);
				list.Add(ray);
			}
			BulletRay bulletRay = BulletScript.CreateBullet(BulletType.Ray, mPlayer) as BulletRay;
			if (bulletRay != null)
			{
				DamageProperty damageProperty = CreateDamageProperty();
				bulletRay.ShootDetect(damageProperty, list, 40, null, 1000f, 0f, 1f, false);
			}
			GameObject gameObject = GameApp.GetInstance().GetGameWorld().GetEffectPool(EffectPoolType.V1_TRAJECTORY)
				.CreateObject(mGunPointTransformList[i].transform.position, vector2, Quaternion.identity);
			if (gameObject != null)
			{
				FireLineScript component2 = gameObject.GetComponent<FireLineScript>();
				component2.beginPos = mGunPointTransformList[i].position;
				component2.endPos = mGunPointTransformList[i].position + vector2 * 100f;
			}
		}
	}
}
