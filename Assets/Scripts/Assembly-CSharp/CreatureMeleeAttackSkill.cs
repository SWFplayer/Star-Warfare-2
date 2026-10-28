using System;
using UnityEngine;

public class CreatureMeleeAttackSkill : CreatureSkill
{
	public string AnimationName;

	public Collider[] Colliders;

	public float KnockDownSpeed;

	public Vector2 KnockDownCheckTime = new Vector2(0f, 1f);

	private float mHitRadius;

	public float HitRadiusSqr;

	private float mRadius;

	public float RadiusSqr;

	public float AngleRad;

	public float Angle
	{
		set
		{
			AngleRad = (float)Math.PI / 180f * (180f - value) / 2f;
		}
	}

	public float Radius
	{
		get
		{
			return mRadius;
		}
		set
		{
			mRadius = value;
			RadiusSqr = value * value;
		}
	}

	public float HitRadius
	{
		get
		{
			return mHitRadius;
		}
		set
		{
			mHitRadius = value;
			HitRadiusSqr = value * value;
		}
	}

	public void SetDamageInfo(BossParameter param, int type)
	{
		DamageType = (byte)type;
		Damage = param.GetDamage(type);
		PlayerDamageType = param.DAMAGE_TYPES[type];
	}
}
