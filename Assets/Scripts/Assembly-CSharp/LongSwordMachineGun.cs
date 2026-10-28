using UnityEngine;

public class LongSwordMachineGun : UnccTurretMachineGun
{
	public override void CreateTrajectory()
	{
		mLastAttackTime = Time.time;
		if (mGunPointTransformList.Count > 0 && mGunPointTransformList[0] != null)
		{
			mGunFireObjectList[0].SetActive(true);
		}
		mGunPointTransformList[mCurrentGunPoint].rotation = mPlayer.mVehicle.GetRotationTransform().rotation;
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
}
