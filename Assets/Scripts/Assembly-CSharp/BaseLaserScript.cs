using System;
using UnityEngine;

public class BaseLaserScript : MonoBehaviour
{
	[NonSerialized]
	public int Damage;

	[NonSerialized]
	public byte DamageType;

	[NonSerialized]
	public PlayerDamageType PlayerDamageType;

	[NonSerialized]
	public GameUnit Attacker;

	[NonSerialized]
	public float KnockSpeed;

	public void SetDamageInfo(BossParameter param, int type)
	{
		DamageType = (byte)type;
		Damage = param.GetDamage(type);
		PlayerDamageType = param.DAMAGE_TYPES[type];
	}

	public void SetRadius(float radius)
	{
		if (base.GetComponent<Collider>() != null)
		{
			BoxCollider boxCollider = base.GetComponent<Collider>() as BoxCollider;
			if (boxCollider != null)
			{
				Vector3 size = boxCollider.size;
				size.x = radius;
				size.y = radius;
				((BoxCollider)base.GetComponent<Collider>()).size = size;
			}
			SphereCollider sphereCollider = base.GetComponent<Collider>() as SphereCollider;
			if (sphereCollider != null)
			{
				sphereCollider.radius = radius;
			}
		}
	}
}
