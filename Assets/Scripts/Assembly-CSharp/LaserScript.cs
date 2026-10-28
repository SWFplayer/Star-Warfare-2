using UnityEngine;

public class LaserScript : BaseLaserScript
{
	public float Interval = 0.2f;

	private HitCheck mHitCheck = new HitCheck();

	private void Start()
	{
		mHitCheck.Clear();
		mHitCheck.Interval = Interval;
		mHitCheck.mTransform = base.transform;
	}

	private void OnTriggerStay(Collider other)
	{
		mHitCheck.OnHit(Attacker, other.gameObject, DamageType, Damage, PlayerDamageType, KnockSpeed);
	}

	private void OnTriggerEnter(Collider other)
	{
		mHitCheck.OnHit(Attacker, other.gameObject, DamageType, Damage, PlayerDamageType, KnockSpeed);
	}
}
