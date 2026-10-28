using System;
using UnityEngine;

public class ParaHitCheckScript : BaseLaserScript
{
	[NonSerialized]
	public Vector3 Speed;

	[NonSerialized]
	public float Gravity;

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
		if (!(Time.time - mStartTime > 0.2f))
		{
			return;
		}
		if (other.gameObject.layer == PhysicsLayer.FLOOR || other.gameObject.layer == PhysicsLayer.WALL_AGAINST_BULLET)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			if (Vfx != null)
			{
				Pool.sAllocate(Vfx, base.transform.position, VfxDuration);
			}
			if (VfxSound != null)
			{
				AudioManager.GetInstance().PlaySoundAt(AudioName.FROSTSTONE_SNOWBALL_END, null, base.transform.position);
			}
		}
		else
		{
			mHitCheck.OnHit(Attacker, other.gameObject, DamageType, Damage, PlayerDamageType, 0f);
		}
	}
}
