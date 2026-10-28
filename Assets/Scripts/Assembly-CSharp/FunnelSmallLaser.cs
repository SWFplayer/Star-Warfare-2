using System;
using UnityEngine;

public class FunnelSmallLaser : BaseLaserScript
{
	[NonSerialized]
	public float Speed;

	private HitCheck mHitCheck = new HitCheck();

	private float mStartTime;

	public void Reset()
	{
		mHitCheck.Clear();
		mStartTime = Time.time;
	}

	private void Update()
	{
		base.transform.Translate(Speed * base.transform.forward * Time.deltaTime, Space.World);
		if (Time.time - mStartTime > 5f)
		{
			base.gameObject.SetActive(false);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == PhysicsLayer.FLOOR || other.gameObject.layer == PhysicsLayer.WALL_AGAINST_BULLET)
		{
			base.gameObject.SetActive(false);
		}
		mHitCheck.OnHit(Attacker, other.gameObject, DamageType, Damage, PlayerDamageType, 0f);
	}
}
