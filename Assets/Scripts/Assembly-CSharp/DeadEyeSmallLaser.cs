using System;
using UnityEngine;

public class DeadEyeSmallLaser : BaseLaserScript
{
	[NonSerialized]
	public Vector3 Speed;

	private HitCheck mHitCheck = new HitCheck();

	public void Reset()
	{
		mHitCheck.Clear();
	}

	private void Update()
	{
		base.transform.Translate(Speed * Time.deltaTime, Space.World);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == PhysicsLayer.FLOOR || other.gameObject.layer == PhysicsLayer.WALL_AGAINST_BULLET)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		mHitCheck.OnHit(Attacker, other.gameObject, DamageType, Damage, PlayerDamageType, 0f);
	}
}
