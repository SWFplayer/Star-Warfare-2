using System.Collections.Generic;
using UnityEngine;

public class UnccTurretMachineGun : VehicleWeapon
{
	protected int mCurrentGunPoint;

	public override void Init(Player player, float baseDamage, float damageRatio, float damageRadius, float attackFrequency, PlayerDamageType damageType, float bulletSpeed)
	{
		base.Init(player, baseDamage, damageRatio, damageRadius, attackFrequency, damageType, bulletSpeed);
		mWeaponType = EVehicleWeaponType.UNCC_TURRET_MACHINE_GUN;
		mCurrentGunPoint = 0;
	}

	public override void CreateGunEffect()
	{
		GameObject original = Resources.Load(EffectName.V2_SPARK) as GameObject;
		mGunFireObjectList = new List<GameObject>();
		if (mGunPointTransformList.Count > 0)
		{
			Vector3 zero = Vector3.zero;
			foreach (Transform mGunPointTransform in mGunPointTransformList)
			{
				zero += mGunPointTransform.position;
			}
			GameObject gameObject = Object.Instantiate(original, zero / mGunPointTransformList.Count, Quaternion.identity) as GameObject;
			gameObject.transform.parent = mGunPointTransformList[0];
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
		AudioManager.GetInstance().PlaySoundSingleLoop(AudioName.UNCC_TURRET_SHOOT, mPlayer.GetObject(), mPlayer.GetTransform());
	}

	public override void EndLoopingSound()
	{
		AudioManager.GetInstance().StopSound(AudioName.UNCC_TURRET_SHOOT, mPlayer.GetObject());
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
		if (mGunPointTransformList.Count > 0 && mGunPointTransformList[0] != null)
		{
			mGunFireObjectList[0].SetActive(true);
		}
		mGunPointTransformList[mCurrentGunPoint].rotation = mPlayer.GetTransform().rotation;
		mGunPointTransformList[mCurrentGunPoint].rotation = Quaternion.AngleAxis(mPlayer.AngleV, -mGunPointTransformList[mCurrentGunPoint].right) * mGunPointTransformList[mCurrentGunPoint].rotation;
		Vector3 normalized = mGunPointTransformList[mCurrentGunPoint].transform.forward.normalized;
		GameObject gameObject = GameApp.GetInstance().GetGameWorld().GetEffectPool(EffectPoolType.V2_TRAJECTORY)
			.CreateObject(mGunPointTransformList[mCurrentGunPoint].transform.position, normalized, Quaternion.identity);
		if (gameObject != null)
		{
			FireLineScript component = gameObject.GetComponent<FireLineScript>();
			component.beginPos = mGunPointTransformList[mCurrentGunPoint].position;
			component.endPos = mGunPointTransformList[mCurrentGunPoint].position + normalized * 100f;
		}
		mCurrentGunPoint = (mCurrentGunPoint + 1) % mGunPointTransformList.Count;
	}

	public override void Attack()
	{
		if (CoolDown())
		{
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
			if (mGunPointTransformList.Count > 0 && mGunPointTransformList[0] != null)
			{
				mGunFireObjectList[0].SetActive(true);
			}
			Vector3 normalized = (vector - transform.position).normalized;
			Vector3 normalized2 = (raycastHit.point - mGunPointTransformList[mCurrentGunPoint].position).normalized;
			List<Ray> list = new List<Ray>();
			ray = new Ray(transform.position + normalized * 1.8f, normalized);
			list.Add(ray);
			BulletRay bulletRay = BulletScript.CreateBullet(BulletType.Ray, mPlayer) as BulletRay;
			if (bulletRay != null)
			{
				DamageProperty damageProperty = CreateDamageProperty();
				bulletRay.ShootDetect(damageProperty, list, 40, GameApp.GetInstance().GetGameWorld().GetEffectPool(EffectPoolType.V2_SHOOT_MARK), 1000f, 0f, 1f, false);
			}
			GameObject gameObject = GameApp.GetInstance().GetGameWorld().GetEffectPool(EffectPoolType.V2_TRAJECTORY)
				.CreateObject(mGunPointTransformList[mCurrentGunPoint].transform.position, normalized2, Quaternion.identity);
			if (gameObject != null)
			{
				FireLineScript component2 = gameObject.GetComponent<FireLineScript>();
				component2.beginPos = mGunPointTransformList[mCurrentGunPoint].position;
				component2.endPos = mGunPointTransformList[mCurrentGunPoint].position + normalized2 * 100f;
			}
			mCurrentGunPoint = (mCurrentGunPoint + 1) % mGunPointTransformList.Count;
		}
	}
}
