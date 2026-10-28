using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("TPS/GrenadeShotScript")]
public class GrenadeShotScript : MonoBehaviour
{
	protected GameObject resObject;

	protected GameObject explodeObject;

	protected GameObject smallExplodeObject;

	protected GameObject laserHitObject;

	public Transform targetTransform;

	public Vector3 targetPos;

	protected Transform proTransform;

	protected WeaponType gunType;

	public Vector3 dir;

	public float hitForce;

	public float explodeRadius;

	public float flySpeed;

	public Vector3 speed;

	public float life = 2f;

	public int damage;

	public bool isLocal = true;

	public byte bagIndex;

	protected float createdTime;

	protected float lastTriggerTime;

	protected float gravity = 16f;

	protected float downSpeed;

	protected float deltaTime;

	protected float initAngel = 50f;

	protected Timer explodeTimer;

	protected HashSet<Enemy> enemyhitSet = new HashSet<Enemy>();

	public WeaponType GunType
	{
		set
		{
			gunType = value;
		}
	}

	public void Start()
	{
		proTransform = base.transform;
		createdTime = Time.time;
	}

	public void Update()
	{
		if (explodeTimer != null && explodeTimer.Ready())
		{
			Explode();
			explodeTimer.Do();
		}
	}

	private void Explode()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		Player localPlayer = gameWorld.GetLocalPlayer();
		GameObject original = Resources.Load("_Old/_Effect/update_effect/effect_explosion_001") as GameObject;
		Object.Instantiate(original, proTransform.position, Quaternion.identity);
		Object.DestroyObject(base.gameObject.transform.parent.gameObject);
		AudioManager.GetInstance().PlaySoundAt("_Old/_Audio/gl/grenade_launcher_boom", base.transform.position);
		DamageProperty damageProperty = new DamageProperty(null, damage);
		damageProperty.isLocal = isLocal;
		damageProperty.criticalAttack = false;
		damageProperty.wType = gunType;
		damageProperty.damageType = PlayerDamageType.EXPLOSION;
		BulletProjectile bulletProjectile = BulletScript.CreateBullet(BulletType.Projectile) as BulletProjectile;
		if (bulletProjectile != null)
		{
			bulletProjectile.ExplosionDetect(damageProperty, null, false, 80, new HashSet<PenerateHitInfo>(), proTransform.position, explodeRadius, false);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (explodeTimer != null)
		{
			return;
		}
		if (other.gameObject.layer == PhysicsLayer.FLOOR || other.gameObject.layer == PhysicsLayer.WALL_AGAINST_BULLET || other.gameObject.layer == PhysicsLayer.TRANSPARENT_WALL)
		{
			explodeTimer = new Timer();
			float interval = 1.5f;
			if (GameApp.GetInstance().GetGameMode().IsVSMode())
			{
				interval = VSMath.GL_EXP_TIME;
			}
			explodeTimer.SetTimer(interval, false);
		}
		else
		{
			Explode();
		}
	}
}
