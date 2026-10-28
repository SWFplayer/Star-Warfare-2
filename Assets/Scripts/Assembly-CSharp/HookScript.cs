using System.Collections.Generic;
using UnityEngine;

public class HookScript : MonoBehaviour
{
	public const float life = 10f;

	public Player owner;

	public float flySpeed = 13f;

	private Vector3 beginPos;

	private GameUnit targetUnit;

	private float startTime;

	private GameWorld gameWorld;

	protected HashSet<PenerateHitInfo> penetratingTargets = new HashSet<PenerateHitInfo>();

	private void Start()
	{
		startTime = Time.time;
		gameWorld = GameApp.GetInstance().GetGameWorld();
	}

	private void Update()
	{
		if (targetUnit != null && targetUnit.InPlayingState())
		{
			base.transform.LookAt(targetUnit.GetPosition() + new Vector3(0f, 1f, 0f));
		}
		base.transform.Translate(base.transform.forward * Time.deltaTime * flySpeed, Space.World);
	}

	private void LateUpdate()
	{
		if (Time.time - startTime >= 10f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (owner != null)
		{
			if (owner.IsLocal())
			{
				if (GameApp.GetInstance().GetGameMode().IsVSMode())
				{
					if (other.gameObject.layer == PhysicsLayer.REMOTE_PLAYER)
					{
						int num = 0;
						bool flag = false;
						if (other.gameObject.tag == TagName.PLAYER_ROOT)
						{
							num = int.Parse(other.gameObject.name);
						}
						else if (other.gameObject.tag == TagName.VEHICLE_ROOT)
						{
							num = 0;
							for (int i = 0; i < other.gameObject.transform.childCount; i++)
							{
								if (other.gameObject.transform.GetChild(i).tag == TagName.PLAYER_ROOT)
								{
									num = int.Parse(other.gameObject.transform.GetChild(i).name);
									break;
								}
							}
							flag = true;
						}
						if (num == 0)
						{
							return;
						}
						RemotePlayer remotePlayerByUserID = gameWorld.GetRemotePlayerByUserID(num);
						if (remotePlayerByUserID != null && !remotePlayerByUserID.IsSameTeam(owner))
						{
							if (!flag)
							{
								HookPlayer(remotePlayerByUserID);
							}
							LocalExplode();
						}
					}
					else
					{
						if (other.gameObject.layer != PhysicsLayer.ENEMY)
						{
							return;
						}
						GameObject rootObjectByCollider = Enemy.GetRootObjectByCollider(other);
						if (rootObjectByCollider.tag == TagName.PLAYER_ROOT)
						{
							int userID = int.Parse(rootObjectByCollider.name);
							RemotePlayer remotePlayerByUserID2 = gameWorld.GetRemotePlayerByUserID(userID);
							if (remotePlayerByUserID2 != null && remotePlayerByUserID2.IsBoss)
							{
								LocalExplode();
							}
						}
					}
				}
				else
				{
					if (owner.IsLocal() && other.gameObject.layer == PhysicsLayer.PLAYER)
					{
						return;
					}
					if (!owner.IsLocal() && other.gameObject.layer == PhysicsLayer.REMOTE_PLAYER)
					{
						if (!(other.gameObject == owner.GetObject()))
						{
						}
					}
					else
					{
						LocalExplode();
					}
				}
			}
			else if ((other.gameObject.layer == PhysicsLayer.REMOTE_PLAYER || other.gameObject.layer == PhysicsLayer.ENEMY || other.gameObject.layer == PhysicsLayer.PLAYER) && !(other.gameObject == owner.GetObject()))
			{
				PlayExplodeAnimation();
				Object.Destroy(base.gameObject);
			}
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void LocalExplode()
	{
		if (owner.IsLocal())
		{
			int specialSkillDamage = owner.GetSkills().GetSpecialSkillDamage(AdvancedSkill.ESpecialSkillType.HOOK);
			DamageProperty damageProperty = new DamageProperty(owner, specialSkillDamage);
			damageProperty.criticalAttack = true;
			damageProperty.mAttackerType = AttackerType.PLAYER_SPECIAL_SKILL;
			damageProperty.isLocal = true;
			damageProperty.wType = WeaponType.NoGun;
			damageProperty.damageType = PlayerDamageType.EXPLOSION;
			damageProperty.isActiveSkill = true;
			damageProperty.weaponid = 27;
			damageProperty.buff += owner.GetSkills().GetSkill(SkillsType.ALL_DAMAGE);
			if (damageProperty.damageType != PlayerDamageType.NORMAL)
			{
				SkillsType sType = (SkillsType)(damageProperty.damageType - 1 + 53);
				damageProperty.buff += owner.GetSkills().GetSkill(sType);
			}
			BulletProjectile bulletProjectile = BulletScript.CreateBullet(BulletType.Projectile, owner) as BulletProjectile;
			if (bulletProjectile != null)
			{
				bulletProjectile.ExplosionDetect(damageProperty, null, false, 80, penetratingTargets, base.transform.position, 10f, false, true);
			}
			PlayExplodeAnimation();
		}
		Object.Destroy(base.gameObject);
	}

	public void PlayExplodeAnimation()
	{
		GameObject gameObject = gameWorld.GetEffectPool(EffectPoolType.W3_ROCKET_BOOM_9).CreateObject(base.transform.position, Vector3.zero, Quaternion.identity);
	}

	public Vector3 GetWeaponDir(Vector3 pos)
	{
		if (owner.GameUnitType == EGameUnitType.LOCAL_PLAYER)
		{
			Camera main = Camera.main;
			Transform transform = main.transform;
			ThirdPersonStandardCameraScript component = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
			Vector3 vector = default(Vector3);
			if (0 == 0)
			{
				vector = main.ScreenToWorldPoint(new Vector3(component.ReticlePosition.x, (float)Screen.height - component.ReticlePosition.y, 0.1f));
			}
			Vector3 normalized = (vector - transform.position).normalized;
			Ray ray = new Ray(transform.position + normalized * 1.8f, normalized);
			Vector3 vector2 = transform.position + (vector - transform.position).normalized * 80f;
			return (vector2 - pos).normalized;
		}
		if (owner.GameUnitType == EGameUnitType.ROBOT)
		{
			Robot robot = owner as Robot;
			Vector3 result = robot.AimDirection;
			if (result.Equals(Vector3.zero))
			{
				result = owner.GetTransform().forward;
			}
			return result;
		}
		Debug.LogError("game unit type is wrong: " + owner.GameUnitType);
		return Vector3.zero;
	}

	public void SetBeginPos(Vector3 pos)
	{
		beginPos = pos;
	}

	public void SetTargetUnit(int playerId)
	{
		gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld != null)
		{
			RemotePlayer remotePlayerByUserID = gameWorld.GetRemotePlayerByUserID(playerId);
			if (remotePlayerByUserID != null)
			{
				targetUnit = remotePlayerByUserID;
			}
			LocalPlayer localPlayer = gameWorld.GetLocalPlayer();
			if (localPlayer == null && localPlayer.GetChannelID() == playerId)
			{
				targetUnit = localPlayer;
			}
		}
	}

	public void HookPlayer(Player player)
	{
		HookHitPlayerRequest request = new HookHitPlayerRequest(player.GetChannelID(), beginPos);
		GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		HookForceScript hookForceScript = player.GetObject().AddComponent<HookForceScript>();
		hookForceScript.Init(beginPos, player);
	}
}
