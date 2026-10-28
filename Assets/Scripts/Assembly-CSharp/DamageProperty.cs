using UnityEngine;

public class DamageProperty
{
	public Vector3 hitForce;

	public int damage;

	public WeaponType wType;

	public Vector3 hitpoint;

	public bool criticalAttack;

	public bool isLocal;

	public int weaponid;

	public PlayerDamageType damageType;

	public bool isVehicle;

	public EVehicleType vehicleType;

	public EVehicleWeaponType vehicleWeaponType;

	public AttackerType mAttackerType;

	public byte damageFactor;

	public float buff;

	public float debuff;

	public GameUnit attackerGameUnit;

	public static int PLAYER_LONG_HIT_PLAYER_DISTANCE = 35;

	public bool isActiveSkill;

	public byte EnemySkillType;

	public RemotePlayer directHitUnit;

	public DamageProperty(GameUnit attacker, int damage)
	{
		attackerGameUnit = attacker;
		this.damage = damage;
	}

	public DamageProperty(GameUnit attacker, int damage, PlayerDamageType type)
	{
		attackerGameUnit = attacker;
		this.damage = damage;
		damageType = type;
	}
}
