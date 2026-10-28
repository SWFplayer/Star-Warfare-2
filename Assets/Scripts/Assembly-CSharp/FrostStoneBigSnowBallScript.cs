using System;
using UnityEngine;

public class FrostStoneBigSnowBallScript : BaseLaserScript
{
	[NonSerialized]
	public Vector3 Speed;

	[NonSerialized]
	public float Gravity;

	[NonSerialized]
	public float ExplodeRadius;

	[NonSerialized]
	public string Vfx;

	[NonSerialized]
	public string VfxSound;

	[NonSerialized]
	public float VfxDuration;

	private HitCheck mHitCheck = new HitCheck();

	private float mStartTime;

	public void Reset()
	{
		mHitCheck.Clear();
		mStartTime = Time.time;
	}

	private void Update()
	{
		Vector3 speed = Speed;
		Speed.y += Gravity * Time.deltaTime;
		speed = (speed + Speed) / 2f;
		base.transform.Translate(speed * Time.deltaTime, Space.World);
		if (Time.time - mStartTime > 10f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!(Time.time - mStartTime > 0.5f) || (other.gameObject.layer != PhysicsLayer.FLOOR && other.gameObject.layer != PhysicsLayer.WALL_AGAINST_BULLET))
		{
			return;
		}
		UnityEngine.Object.Destroy(base.gameObject);
		mHitCheck.CheckHit(Attacker, base.transform.position, ExplodeRadius * ExplodeRadius, DamageType, Damage, PlayerDamageType);
		if (Vfx != string.Empty)
		{
			Ray ray = new Ray(base.transform.position + Vector3.up, Vector3.down);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 10f, 1 << PhysicsLayer.FLOOR))
			{
				Pool.sAllocate(Vfx, hitInfo.point, VfxDuration);
			}
			else
			{
				Debug.Log("No floor.");
			}
		}
		if (VfxSound != null)
		{
			AudioManager.GetInstance().PlaySoundAt(VfxSound, base.transform.position);
		}
	}
}
