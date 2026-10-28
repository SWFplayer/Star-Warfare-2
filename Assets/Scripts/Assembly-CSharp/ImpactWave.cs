using System.Collections.Generic;
using UnityEngine;

public class ImpactWave : MonoBehaviour
{
	public const float flySpeed = 13f;

	public const float life = 10f;

	private const int healDpsFacor = 10;

	public Player owner;

	private float startTime;

	private Player localPlayer;

	private GameWorld gameWorld;

	private Dictionary<GameUnit, int> hitCountDic = new Dictionary<GameUnit, int>();

	private int MaxDamageCount;

	private void Start()
	{
		startTime = Time.time;
		gameWorld = GameApp.GetInstance().GetGameWorld();
		localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		hitCountDic.Clear();
		if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.BOSS_FINAL || GameApp.GetInstance().GetGameMode().ModePlay == Mode.GIANT_BETRAYAL)
		{
			MaxDamageCount = 1;
		}
		else
		{
			MaxDamageCount = 2;
		}
	}

	private void Update()
	{
		base.transform.Translate(base.transform.forward * Time.deltaTime * 13f, Space.World);
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
			if (GameApp.GetInstance().GetGameMode().IsVSMode())
			{
				if (other.gameObject.layer == PhysicsLayer.REMOTE_PLAYER)
				{
					int result = 0;
					if (other.gameObject.tag == TagName.PLAYER_ROOT)
					{
						int.TryParse(other.gameObject.name, out result);
					}
					else if (other.gameObject.tag == TagName.VEHICLE_ROOT)
					{
						for (int i = 0; i < other.gameObject.transform.childCount; i++)
						{
							if (other.gameObject.transform.GetChild(i).tag == TagName.PLAYER_ROOT)
							{
								int.TryParse(other.gameObject.transform.GetChild(i).name, out result);
								if (result != 0)
								{
									break;
								}
							}
						}
					}
					RemotePlayer remotePlayerByUserID = gameWorld.GetRemotePlayerByUserID(result);
					if (remotePlayerByUserID != null)
					{
						Impact(remotePlayerByUserID);
					}
				}
				else if (other.gameObject.layer == PhysicsLayer.PLAYER)
				{
					Impact(localPlayer);
				}
				else if (other.gameObject.layer == PhysicsLayer.VOID_SHIELD)
				{
					ShieldScript component = other.transform.root.GetComponent<ShieldScript>();
					if (owner.IsLocal() && !component.ownedPlayer.IsFriendForce(owner) && component.ownedPlayer != owner)
					{
						DamageProperty damageProperty = new DamageProperty(owner, 0);
						damageProperty.mAttackerType = AttackerType.PLAYER_SPECIAL_SKILL;
						damageProperty.weaponid = 24;
						component.PlayerHitShield(damageProperty);
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
						if (remotePlayerByUserID2 != null)
						{
							Impact(remotePlayerByUserID2);
						}
					}
				}
			}
			else if (other.gameObject.layer == PhysicsLayer.REMOTE_PLAYER)
			{
				int result2 = 0;
				if (other.gameObject.tag == TagName.PLAYER_ROOT)
				{
					int.TryParse(other.gameObject.name, out result2);
				}
				else if (other.gameObject.tag == TagName.VEHICLE_ROOT)
				{
					for (int j = 0; j < other.gameObject.transform.childCount; j++)
					{
						if (other.gameObject.transform.GetChild(j).tag == TagName.PLAYER_ROOT)
						{
							int.TryParse(other.gameObject.transform.GetChild(j).name, out result2);
							if (result2 != 0)
							{
								break;
							}
						}
					}
				}
				RemotePlayer remotePlayerByUserID3 = gameWorld.GetRemotePlayerByUserID(result2);
				if (remotePlayerByUserID3 != null)
				{
					Impact(remotePlayerByUserID3);
				}
			}
			else if (other.gameObject.layer == PhysicsLayer.PLAYER)
			{
				Impact(localPlayer);
			}
			else if (other.gameObject.layer == PhysicsLayer.ENEMY)
			{
				GameObject rootObjectByCollider2 = Enemy.GetRootObjectByCollider(other);
				if (rootObjectByCollider2.tag == TagName.ENEMY_ROOT)
				{
					GameUnit enemyByID = GameApp.GetInstance().GetGameWorld().GetEnemyByID(rootObjectByCollider2.name);
					Impact(enemyByID);
				}
			}
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Impact(GameUnit target)
	{
		if (!owner.IsLocal())
		{
			return;
		}
		Player player = target as Player;
		if (player != null && player.InPlayingState())
		{
			if (!hitCountDic.ContainsKey(player))
			{
				hitCountDic.Add(player, 1);
			}
			else
			{
				Dictionary<GameUnit, int> dictionary2;
				Dictionary<GameUnit, int> dictionary = (dictionary2 = hitCountDic);
				Player key2;
				Player key = (key2 = player);
				int num = dictionary2[key2];
				dictionary[key] = num + 1;
			}
			if (hitCountDic[player] > MaxDamageCount)
			{
				return;
			}
			if (!player.IsFriendForce(localPlayer) && !player.IsLocal())
			{
				Debug.Log("Hit " + player.GetChannelID());
				DamageProperty damageProperty = new DamageProperty(owner, 0);
				damageProperty.mAttackerType = AttackerType.PLAYER_SPECIAL_SKILL;
				damageProperty.weaponid = 24;
				if (!player.IsInAnyShields(damageProperty))
				{
					PlayerHitPlayerRequest request = new PlayerHitPlayerRequest(damageProperty, player.GetChannelID());
					GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				}
			}
			else if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				Debug.Log("Heal " + player.GetChannelID());
				PlayerHealOthersRequest request2 = new PlayerHealOthersRequest(player.GetChannelID(), 10);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
			}
			else
			{
				localPlayer.RecoverHP(localPlayer.GetCurrentDps() * 10f);
			}
		}
		Enemy enemy = target as Enemy;
		if (enemy != null && enemy.InPlayingState())
		{
			if (!hitCountDic.ContainsKey(enemy))
			{
				hitCountDic.Add(enemy, 1);
			}
			else
			{
				Dictionary<GameUnit, int> dictionary4;
				Dictionary<GameUnit, int> dictionary3 = (dictionary4 = hitCountDic);
				Enemy key4;
				Enemy key3 = (key4 = enemy);
				int num = dictionary4[key4];
				dictionary3[key3] = num + 1;
			}
			if (hitCountDic[enemy] <= MaxDamageCount)
			{
				int specialSkillDamage = owner.GetSkills().GetSpecialSkillDamage(AdvancedSkill.ESpecialSkillType.IMPACT_WAVE);
				DamageProperty damageProperty2 = new DamageProperty(owner, specialSkillDamage);
				damageProperty2.mAttackerType = AttackerType.PLAYER_SPECIAL_SKILL;
				damageProperty2.weaponid = 24;
				damageProperty2.isLocal = true;
				enemy.HitEnemy(owner, damageProperty2);
			}
		}
	}
}
