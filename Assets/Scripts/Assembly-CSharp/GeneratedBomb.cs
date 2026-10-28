using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratedBomb : MonoBehaviour
{
	public ESpawnObjectType type;

	private float radius;

	public Timer timer;

	public Player owner;

	public Player player;

	private List<RemotePlayer> playerList;

	private short SkillId;

	private int missleLaunched;

	private Timer nuclearMissleTimer;

	private int smallMissleLaunchNum;

	private void Start()
	{
		nuclearMissleTimer = new Timer(3f, false);
		if (type == ESpawnObjectType.BOMB)
		{
			radius = 7f;
		}
		else if (type == ESpawnObjectType.SLOW_BOMB)
		{
			radius = 8f;
		}
		else if (type == ESpawnObjectType.FLASH)
		{
			radius = 20f;
		}
		else if (type == ESpawnObjectType.MISSLE)
		{
			radius = 10f;
		}
		else if (type == ESpawnObjectType.SLOW_AREA)
		{
			radius = 20f;
		}
		if (type == ESpawnObjectType.BOMB || type == ESpawnObjectType.FLASH || type == ESpawnObjectType.SLOW_BOMB)
		{
			timer = new Timer(1f, false);
		}
		else if (type == ESpawnObjectType.MISSLE || type == ESpawnObjectType.SLOW_AREA || type == ESpawnObjectType.SMALL_MISSLE)
		{
			timer = new Timer(1.5f, false);
		}
		else if (type == ESpawnObjectType.NUCLEAR)
		{
			timer = new Timer(5f, false);
		}
		else if (type == ESpawnObjectType.TRIPPLE_MISSLE)
		{
			timer = new Timer(3f, false);
		}
		player = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		playerList = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
	}

	private void Update()
	{
		if (nuclearMissleTimer.Ready() && type == ESpawnObjectType.NUCLEAR && smallMissleLaunchNum < 5)
		{
			Object original = Resources.Load(EffectName.SMALL_MISSLE);
			Quaternion quaternion = Quaternion.Euler(0f, 72f * (float)smallMissleLaunchNum, 0f);
			Vector3 vector = quaternion * new Vector3(5f, 0f, 5f);
			GameObject gameObject = Object.Instantiate(original, player.GetPosition() + vector, Quaternion.identity) as GameObject;
			smallMissleLaunchNum++;
			nuclearMissleTimer = new Timer(0.4f, false);
		}
		if (!timer.Ready())
		{
			return;
		}
		if (type == ESpawnObjectType.FLASH && owner != player && !owner.IsSameTeam(player) && !GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			if (player.InPlayingState() && Vector3.Distance(base.transform.position, player.GetPosition()) < radius && player.UnitCanBeHitByRay(base.transform.position, radius))
			{
				float num = Vector3.Angle(player.GetTransform().forward, base.transform.position - player.GetPosition());
				if (num < 45f)
				{
					Camera.main.gameObject.GetComponentInChildren<ScreenFlashScript>().Flash(1f);
				}
				else if (num < 135f)
				{
					Camera.main.gameObject.GetComponentInChildren<ScreenFlashScript>().Flash(0.75f);
				}
				else
				{
					Camera.main.gameObject.GetComponentInChildren<ScreenFlashScript>().Flash(0.5f);
				}
			}
		}
		else if (type == ESpawnObjectType.FLASH && owner == player)
		{
			Hashtable enemies = GameApp.GetInstance().GetGameWorld().GetEnemies();
			foreach (Enemy value in enemies.Values)
			{
				Vector3 vector2 = player.GetPosition() - value.GetPosition();
				vector2.y = 0f;
				if (value != null && value.EnemyType != EnemyType.TANK && !value.IsBoss() && vector2.magnitude <= 20f && value.InPlayingState() && value.GetState() != Enemy.STUN_STATE && value.GetState() != Enemy.BLACK_HOLE_STATE)
				{
					value.SetState(Enemy.STUN_STATE);
					if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && GameApp.GetInstance().GetGameMode().IsCoopMode())
					{
						EnemyStateRequest request = new EnemyStateRequest(value.EnemyID, value.GetID(EEnemyStateID.STUN), value.GetPosition());
						GameApp.GetInstance().GetNetworkManager().SendRequest(request);
					}
				}
			}
		}
		else if (type == ESpawnObjectType.BOMB && owner == player)
		{
			foreach (RemotePlayer player in playerList)
			{
				if (player != null && player.InPlayingState() && !player.IsSameTeam(this.player) && Vector3.Distance(base.transform.position, player.GetPosition()) < radius && player.UnitCanBeHitByRay(base.transform.position, radius))
				{
					DamageProperty damageProperty = new DamageProperty(this.player, 0);
					damageProperty.mAttackerType = AttackerType.PLAYER_SPECIAL_SKILL;
					damageProperty.weaponid = 17;
					if (!player.IsInAnyShields(damageProperty))
					{
						PlayerHitPlayerRequest request2 = new PlayerHitPlayerRequest(damageProperty, player.GetChannelID());
						GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
					}
				}
			}
		}
		else if (type == ESpawnObjectType.SLOW_BOMB && owner == this.player)
		{
			List<int> list = new List<int>();
			foreach (RemotePlayer player2 in playerList)
			{
				if (player2 != null && player2.InPlayingState() && !player2.IsSameTeam(this.player) && Vector3.Distance(base.transform.position, player2.GetPosition()) < radius && player2.UnitCanBeHitByRay(base.transform.position, radius))
				{
					list.Add(player2.GetChannelID());
				}
			}
			PlayerBuffRequest request3 = new PlayerBuffRequest(list, SkillId);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request3);
		}
		else if (type == ESpawnObjectType.MISSLE)
		{
			if (owner == this.player)
			{
				foreach (RemotePlayer player3 in playerList)
				{
					if (player3 != null && player3.InPlayingState() && !player3.IsSameTeam(this.player) && Vector3.Distance(base.transform.position, player3.GetPosition()) < radius)
					{
						DamageProperty damageProperty2 = new DamageProperty(this.player, 0);
						damageProperty2.mAttackerType = AttackerType.PLAYER_SPECIAL_SKILL;
						damageProperty2.weaponid = 13;
						damageProperty2.isActiveSkill = true;
						if (!player3.IsInAnyShields(damageProperty2))
						{
							PlayerHitPlayerRequest request4 = new PlayerHitPlayerRequest(damageProperty2, player3.GetChannelID());
							GameApp.GetInstance().GetNetworkManager().SendRequest(request4);
						}
					}
				}
			}
			Object original2 = Resources.Load(EffectName.SKILL_MISSLE_BOOM);
			GameObject gameObject2 = Object.Instantiate(original2, base.transform.position, Quaternion.identity) as GameObject;
		}
		else if (type == ESpawnObjectType.SMALL_MISSLE)
		{
			Object original3 = Resources.Load(EffectName.SMALL_MISSLE_BOOM);
			GameObject gameObject3 = Object.Instantiate(original3, base.transform.position, Quaternion.identity) as GameObject;
		}
		else if (type == ESpawnObjectType.SLOW_AREA)
		{
			Object original4 = Resources.Load(EffectName.SKILL_SLOW_MISSLE_BOOM);
			GameObject gameObject4 = Object.Instantiate(original4, base.transform.position, Quaternion.identity) as GameObject;
			gameObject4.GetComponent<SlowAreaScript>().owner = owner;
			gameObject4.GetComponent<SlowAreaScript>().SkillId = SkillId;
			gameObject4.GetComponent<SlowAreaScript>().radius = radius;
			gameObject4.GetComponent<SlowAreaScript>().Init();
		}
		else if (type == ESpawnObjectType.NUCLEAR)
		{
			if (owner == this.player)
			{
				foreach (RemotePlayer player4 in playerList)
				{
					if (player4 != null && player4.InPlayingState())
					{
						PlayerHitPlayerRequest request5 = new PlayerHitPlayerRequest(player4.GetChannelID(), AttackerType.PLAYER_SPECIAL_SKILL, 16, 0, 0);
						GameApp.GetInstance().GetNetworkManager().SendRequest(request5);
					}
				}
				PlayerHitPlayerRequest request6 = new PlayerHitPlayerRequest(this.player.GetChannelID(), AttackerType.PLAYER_SPECIAL_SKILL, 16, 0, 0);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request6);
				PlayerPutNuclearRequest request7 = new PlayerPutNuclearRequest();
				GameApp.GetInstance().GetNetworkManager().SendRequest(request7);
			}
			Camera.main.gameObject.GetComponentInChildren<ScreenFlashScript>().Flash(1f);
		}
		else if (type == ESpawnObjectType.TRIPPLE_MISSLE && owner == this.player)
		{
			SpawnObjectRequest request8 = new SpawnObjectRequest(2, base.transform.position, SkillId);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request8);
			timer = new Timer(6f, false);
			missleLaunched++;
		}
		if (type != ESpawnObjectType.TRIPPLE_MISSLE || (type == ESpawnObjectType.TRIPPLE_MISSLE && missleLaunched == 3))
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void SetSkill(short skillID)
	{
		SkillId = skillID;
	}
}
