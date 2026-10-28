using System;
using UnityEngine;

public class CerberliskBigFireBallScript : BaseLaserScript
{
	[NonSerialized]
	public Vector3 Speed;

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
		base.transform.Translate(Speed * Time.deltaTime, Space.World);
		if (Time.time - mStartTime > 10f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!(Time.time - mStartTime > 0.1f) || (other.gameObject.layer != PhysicsLayer.FLOOR && other.gameObject.layer != PhysicsLayer.WALL_AGAINST_BULLET))
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
				GameObject go = Pool.sAllocate(Vfx, hitInfo.point, VfxDuration);
				LaserScript component = GameWorld.GetComponent<LaserScript>(go);
				component.SetDamageInfo(CerberliskParameter.GetInstance(), 8);
				component.Attacker = Attacker;
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
