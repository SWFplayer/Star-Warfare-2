using System.Collections.Generic;
using UnityEngine;

public class SniperFirelineScript : MonoBehaviour
{
	public Vector3 beginPos;

	public Vector3 endPos;

	public float damageInterval;

	public Sniper gun;

	private float dis;

	protected bool growing = true;

	protected float grownTime;

	private Dictionary<Collider, Timer> TimerDictionary;

	private Dictionary<GameUnit, Timer> EnemyTimerDictionary;

	public void Init()
	{
		base.transform.position = beginPos;
		base.transform.rotation = Quaternion.LookRotation(endPos - beginPos);
		base.transform.localScale = new Vector3(base.transform.localScale.x, base.transform.localScale.y, 0.01f);
		dis = Vector3.Distance(beginPos, endPos);
		if (gun.GunID == 27)
		{
			Ray ray = new Ray(beginPos, endPos - beginPos);
			int layerMask = (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.TRANSPARENT_WALL) | (1 << PhysicsLayer.VOID_SHIELD);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, dis, layerMask))
			{
				dis = Vector3.Distance(hitInfo.point, beginPos);
			}
		}
		growing = true;
		TimerDictionary = new Dictionary<Collider, Timer>();
		EnemyTimerDictionary = new Dictionary<GameUnit, Timer>();
		if (base.gameObject.GetComponent<BoxCollider>() != null)
		{
			if (GameApp.GetInstance().GetGameMode().IsVSMode())
			{
				base.gameObject.GetComponent<BoxCollider>().size = new Vector3(0.5f, 0.5f, gun.GetFireLineLength());
			}
			else
			{
				base.gameObject.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, gun.GetFireLineLength());
			}
		}
	}

	public void Update()
	{
		float num = dis / gun.GetFireLineLength();
		if (growing && base.transform.localScale.z > num)
		{
			growing = false;
			grownTime = Time.time;
		}
		if (growing)
		{
			float z = Mathf.Min(num, base.transform.localScale.z + 400f / gun.GetFireLineLength() * Time.deltaTime);
			base.transform.localScale = new Vector3(base.transform.localScale.x, base.transform.localScale.y, z);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (gun == null || gun.player == null || !gun.player.IsLocal() || !gun.player.InPlayingState() || !(damageInterval > 0f))
		{
			return;
		}
		BulletRay bulletRay = BulletScript.CreateBullet(BulletType.Ray, gun.player) as BulletRay;
		List<Collider> list = new List<Collider>();
		if (other.gameObject.layer == PhysicsLayer.REMOTE_PLAYER || other.gameObject.layer == PhysicsLayer.DESTRUCTIBLE || other.gameObject.layer == PhysicsLayer.VOID_SHIELD)
		{
			if (!TimerDictionary.ContainsKey(other))
			{
				list.Add(other);
				TimerDictionary.Add(other, new Timer(damageInterval, false));
			}
			else if (TimerDictionary[other].Ready())
			{
				list.Add(other);
				TimerDictionary[other].Do();
			}
		}
		else if (other.gameObject.layer == PhysicsLayer.ENEMY)
		{
			GameUnit gameUnit = null;
			GameObject rootObjectByCollider = Enemy.GetRootObjectByCollider(other);
			if (rootObjectByCollider.tag == TagName.ENEMY_ROOT)
			{
				gameUnit = GameApp.GetInstance().GetGameWorld().GetEnemyByID(rootObjectByCollider.name);
			}
			else if (rootObjectByCollider.tag == TagName.PLAYER_ROOT)
			{
				int userID = int.Parse(rootObjectByCollider.name);
				gameUnit = GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(userID);
			}
			if (gameUnit != null)
			{
				if (!EnemyTimerDictionary.ContainsKey(gameUnit))
				{
					list.Add(other);
					EnemyTimerDictionary.Add(gameUnit, new Timer(damageInterval, false));
				}
				else if (EnemyTimerDictionary[gameUnit].Ready())
				{
					list.Add(other);
					EnemyTimerDictionary[gameUnit].Do();
				}
			}
		}
		if (list.Count != 0)
		{
			bulletRay.DamageDetect(gun.CreateDamageProperty(), list, new List<Vector3>(), 40, null, 0f, 0f, 1f);
		}
	}
}
