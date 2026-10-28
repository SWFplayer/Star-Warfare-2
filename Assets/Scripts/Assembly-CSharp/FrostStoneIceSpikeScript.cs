using System;
using UnityEngine;

public class FrostStoneIceSpikeScript : BaseLaserScript
{
	[NonSerialized]
	public int Damage2;

	[NonSerialized]
	public byte DamageType2;

	[NonSerialized]
	public PlayerDamageType PlayerDamageType2;

	[NonSerialized]
	public float Interval = 1f;

	[NonSerialized]
	public float Duration;

	[NonSerialized]
	public string VFXSound;

	private HitCheck mHitCheck = new HitCheck();

	private Animation mAnimation;

	private float mStartTime;

	private DestructibleObjectScript mScript;

	public void Reset()
	{
		mHitCheck.Clear();
		mHitCheck.Interval = Interval;
		mAnimation = base.gameObject.GetComponentInChildren<Animation>();
		mHitCheck.mTransform = base.transform;
		base.transform.Translate(0f, 0.1f, 0f);
		mStartTime = Time.time;
		mScript = base.gameObject.GetComponent<DestructibleObjectScript>();
	}

	private void Update()
	{
		if (Time.time - mStartTime > Duration)
		{
			mScript.OnHit(99999999, false);
		}
		if (mScript.mDestroyed)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)mAnimation && mAnimation["b3_spike_up"].normalizedTime < 1f)
		{
			mHitCheck.OnPassiveHit(Attacker, other.gameObject, DamageType, Damage, PlayerDamageType, FrostStoneParameter.ICE_SPIKE_KNOCK_SPEED);
		}
		else
		{
			mHitCheck.OnPassiveHit(Attacker, other.gameObject, DamageType2, Damage2, PlayerDamageType2, 0f);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if ((bool)mAnimation && mAnimation["b3_spike_up"].normalizedTime < 1f)
		{
			mHitCheck.OnPassiveHit(Attacker, other.gameObject, DamageType, Damage, PlayerDamageType, FrostStoneParameter.ICE_SPIKE_KNOCK_SPEED);
		}
		else
		{
			mHitCheck.OnPassiveHit(Attacker, other.gameObject, DamageType2, Damage2, PlayerDamageType2, 0f);
		}
	}

	private void OnDestroy()
	{
		if (VFXSound != null && AudioManager.GetInstance() != null)
		{
			AudioManager.GetInstance().PlaySoundAt(VFXSound, base.transform.position);
		}
	}

	public void SetDamageInfo2(BossParameter param, int type)
	{
		DamageType2 = (byte)type;
		Damage2 = param.GetDamage(type);
		PlayerDamageType2 = param.DAMAGE_TYPES[type];
	}
}
