using System.Collections.Generic;
using UnityEngine;

public class EnemyShotScript : MonoBehaviour
{
	public delegate void VoidDelegate();

	public Vector3 speed;

	public float explodeRadius = 8f;

	public int attackDamage = 5;

	public int areaDamage = 5;

	public TrajectoryType trType;

	public EnemyDamageType damageType;

	public EnemyType enemyType;

	public string explodeEffect;

	public Enemy attacker;

	public bool DoNotDestroy;

	public PlayerDamageType PlayerDamageType;

	public bool CheckFloor = true;

	private bool mEnable;

	public VoidDelegate onDead;

	private float startTime;

	private void OnEnable()
	{
		mEnable = true;
	}

	private void Start()
	{
		startTime = Time.time;
	}

	private void Update()
	{
		if (trType == TrajectoryType.Parabola)
		{
			speed += Physics.gravity.y * Vector3.up * Time.deltaTime;
			base.transform.Translate(speed * Time.deltaTime, Space.World);
			base.transform.LookAt(base.transform.position + speed * 10f);
		}
		else if (trType == TrajectoryType.Straight)
		{
			base.transform.Translate(speed * Time.deltaTime, Space.World);
			base.transform.LookAt(base.transform.position + speed * 10f);
		}
		if (!DoNotDestroy && Time.time - startTime > 20f)
		{
			Object.DestroyObject(base.gameObject);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer != PhysicsLayer.PLAYER && other.gameObject.layer != PhysicsLayer.REMOTE_PLAYER && (!CheckFloor || other.gameObject.layer != PhysicsLayer.FLOOR) && other.gameObject.layer != PhysicsLayer.WALL_AGAINST_BULLET && other.gameObject.layer != PhysicsLayer.TRANSPARENT_WALL && other.gameObject.layer != PhysicsLayer.PROTECTED_ITEM && other.gameObject.layer != PhysicsLayer.ROBOT && other.gameObject.layer != PhysicsLayer.VOID_SHIELD && other.gameObject.layer != PhysicsLayer.DESTRUCTIBLE)
		{
			return;
		}
		if (onDead != null)
		{
			onDead();
		}
		if (DoNotDestroy)
		{
			if (!mEnable)
			{
				return;
			}
			mEnable = false;
			base.gameObject.SetActive(false);
		}
		else
		{
			Object.DestroyObject(base.gameObject);
		}
		if (enemyType == EnemyType.CRIMSON_QUEEN)
		{
			GameApp.GetInstance().GetGameWorld().GetEffectPool(EffectPoolType.W3_ROCKET_BOOM_5)
				.CreateObject(base.transform.position, Vector3.zero, Quaternion.identity);
		}
		else
		{
			string path = "_Old/_Effect/bug_3_bom";
			Vector3 position = base.gameObject.transform.position;
			if (enemyType == EnemyType.MYTH370)
			{
				path = EffectName.E5_BigBallExp;
				if (other.gameObject.layer == PhysicsLayer.FLOOR)
				{
					position += Vector3.down;
				}
			}
			GameObject original = Resources.Load(path) as GameObject;
			Object.Instantiate(original, position, Quaternion.identity);
		}
		Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		if (other.gameObject.layer == PhysicsLayer.PLAYER)
		{
			if (damageType == EnemyDamageType.Normal || damageType == EnemyDamageType.Sputtering)
			{
				localPlayer.OnHit(new DamageProperty(attacker, attackDamage, PlayerDamageType));
			}
		}
		else if (other.gameObject.layer == PhysicsLayer.PROTECTED_ITEM)
		{
			ProtectedItem protectedItemByName = GameApp.GetInstance().GetGameWorld().GetProtectedItemByName(other.gameObject.name);
			if (protectedItemByName != null)
			{
				protectedItemByName.OnHit(new DamageProperty(attacker, attackDamage, PlayerDamageType));
			}
		}
		else if (other.gameObject.layer == PhysicsLayer.ROBOT)
		{
			Robot robotByName = GameApp.GetInstance().GetGameWorld().GetRobotByName(other.gameObject.name);
			if (robotByName != null)
			{
				robotByName.OnHit(new DamageProperty(attacker, attackDamage, PlayerDamageType));
			}
		}
		else if (other.gameObject.layer == PhysicsLayer.VOID_SHIELD)
		{
			ShieldScript component = other.transform.root.GetComponent<ShieldScript>();
			if (component.ownedPlayer == localPlayer)
			{
				DamageProperty dp = new DamageProperty(attacker, attackDamage, PlayerDamageType);
				int damage = localPlayer.CalculateDamage(dp);
				other.transform.root.GetComponent<ShieldScript>().AbsorbDamage(damage);
			}
		}
		if (damageType != EnemyDamageType.Sputtering && damageType != EnemyDamageType.Explosion)
		{
			return;
		}
		if (damageType == EnemyDamageType.Explosion)
		{
			if (explodeEffect != null && string.Empty != explodeEffect)
			{
				GameObject original2 = Resources.Load(explodeEffect) as GameObject;
				Object.Instantiate(original2, base.transform.position, Quaternion.identity);
			}
			AudioManager.GetInstance().PlaySoundAt("_Old/_Audio/rpg/rpg-21_boom", base.transform.position);
		}
		if ((localPlayer.GetTransform().position - base.transform.position).sqrMagnitude < explodeRadius * explodeRadius)
		{
			if (attacker != null && attacker.EnemyType == EnemyType.CRIMSON_QUEEN)
			{
				localPlayer.OnHit(new DamageProperty(attacker, areaDamage, PlayerDamageType));
			}
			else
			{
				Ray ray = new Ray(base.transform.position, localPlayer.GetTransform().position - base.transform.position);
				float distance = Mathf.Sqrt((base.transform.position - localPlayer.GetTransform().position).sqrMagnitude);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, distance, (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.TRANSPARENT_WALL) | (1 << PhysicsLayer.PLAYER) | (1 << PhysicsLayer.VOID_SHIELD) | (1 << PhysicsLayer.DESTRUCTIBLE)) && hitInfo.collider.gameObject.layer == PhysicsLayer.PLAYER)
				{
					localPlayer.OnHit(new DamageProperty(attacker, areaDamage, PlayerDamageType));
				}
			}
		}
		List<ProtectedItem> protectedItemList = GameApp.GetInstance().GetGameWorld().GetProtectedItemList();
		foreach (ProtectedItem item in protectedItemList)
		{
			if ((item.GetTransform().position - base.transform.position).sqrMagnitude < explodeRadius * explodeRadius)
			{
				Ray ray2 = new Ray(base.transform.position, item.GetTransform().position - base.transform.position);
				float distance2 = Mathf.Sqrt((base.transform.position - item.GetTransform().position).sqrMagnitude);
				RaycastHit hitInfo2;
				if (Physics.Raycast(ray2, out hitInfo2, distance2, (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.TRANSPARENT_WALL) | (1 << PhysicsLayer.PROTECTED_ITEM) | (1 << PhysicsLayer.DESTRUCTIBLE)) && hitInfo2.collider.gameObject.layer == PhysicsLayer.PROTECTED_ITEM)
				{
					item.OnHit(new DamageProperty(attacker, areaDamage, PlayerDamageType));
				}
			}
		}
		List<ShieldScript> currentShields = GameApp.GetInstance().GetGameWorld().GetCurrentShields();
		foreach (ShieldScript item2 in currentShields)
		{
			if (!(item2 != null) || !(Vector3.Distance(item2.transform.position, base.transform.position) < explodeRadius))
			{
				continue;
			}
			Ray ray3 = new Ray(base.transform.position, item2.transform.position - base.transform.position);
			float distance3 = Mathf.Sqrt((base.transform.position - item2.transform.position).sqrMagnitude);
			RaycastHit hitInfo3;
			if (Physics.Raycast(ray3, out hitInfo3, distance3, (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.TRANSPARENT_WALL) | (1 << PhysicsLayer.VOID_SHIELD)) && hitInfo3.collider.gameObject.layer == PhysicsLayer.VOID_SHIELD)
			{
				ShieldScript component2 = other.transform.root.GetComponent<ShieldScript>();
				if (component2.ownedPlayer == localPlayer)
				{
					DamageProperty dp2 = new DamageProperty(attacker, areaDamage, PlayerDamageType);
					int damage2 = localPlayer.CalculateDamage(dp2);
					other.transform.root.GetComponent<ShieldScript>().AbsorbDamage(damage2);
				}
			}
		}
	}
}
