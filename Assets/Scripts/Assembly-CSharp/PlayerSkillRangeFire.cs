using System.Collections;
using System.Collections.Generic;

public class PlayerSkillRangeFire : PlayerSpecialSkill
{
	public Player player;

	public float rangeSqr;

	public float damagePerSecond;

	private float timeInterval = 1f;

	private Timer checkNewEnemyTimer = new Timer(0.5f);

	private Dictionary<GameUnit, Timer> timerDictionary = new Dictionary<GameUnit, Timer>();

	private void MakeDamage(GameUnit gameUnit)
	{
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			if (gameUnit.GameUnitType == EGameUnitType.ENEMY)
			{
				Enemy enemy = gameUnit as Enemy;
				EnemyOnHitRequest request = new EnemyOnHitRequest(enemy.EnemyID, (int)damagePerSecond, 0, false, 0);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			else if (gameUnit.GameUnitType == EGameUnitType.REMOTE_PLAYER && ((Player)gameUnit).InPlayingState())
			{
				DamageProperty damageProperty = new DamageProperty(player, 0);
				damageProperty.mAttackerType = AttackerType.PLAYER_SPECIAL_SKILL;
				damageProperty.weaponid = 4;
				if (!((Player)gameUnit).IsInAnyShields(damageProperty))
				{
					player.HitTargetShowCrossHit();
					PlayerHitPlayerRequest request2 = new PlayerHitPlayerRequest(damageProperty, ((Player)gameUnit).GetChannelID());
					GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
				}
			}
		}
		else
		{
			gameUnit.OnHit(new DamageProperty(player, (int)damagePerSecond));
		}
	}

	public override void Loop()
	{
		if (checkNewEnemyTimer.Ready())
		{
			checkNewEnemyTimer.Do();
			CheckNewEnemy();
		}
		foreach (KeyValuePair<GameUnit, Timer> item in timerDictionary)
		{
			if (item.Key != null && item.Key.InPlayingState())
			{
				if (item.Value.Ready())
				{
					if (!(player.GetTransform() != null) || !(item.Key.GetTransform() != null))
					{
						timerDictionary.Remove(item.Key);
						break;
					}
					float sqrMagnitude = (player.GetTransform().position - item.Key.GetTransform().position).sqrMagnitude;
					if (sqrMagnitude < rangeSqr)
					{
						MakeDamage(item.Key);
					}
					item.Value.Do();
				}
				continue;
			}
			timerDictionary.Remove(item.Key);
			break;
		}
	}

	private void CheckNewEnemy()
	{
		Hashtable enemies = GameApp.GetInstance().GetGameWorld().GetEnemies();
		foreach (Enemy value in enemies.Values)
		{
			if (!timerDictionary.ContainsKey(value) && player.GetTransform() != null && value.GetTransform() != null)
			{
				float sqrMagnitude = (player.GetTransform().position - value.GetTransform().position).sqrMagnitude;
				if (sqrMagnitude < rangeSqr)
				{
					Timer timer = new Timer();
					timer.SetTimer(timeInterval, true);
					timerDictionary.Add(value, timer);
				}
			}
		}
		if (!GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			return;
		}
		foreach (RemotePlayer remotePlayer in GameApp.GetInstance().GetGameWorld().GetRemotePlayers())
		{
			if (!remotePlayer.IsSameTeam(player) && !timerDictionary.ContainsKey(remotePlayer) && remotePlayer != null && remotePlayer.InPlayingState() && player.GetTransform() != null && remotePlayer.GetTransform() != null)
			{
				float sqrMagnitude2 = (player.GetTransform().position - remotePlayer.GetTransform().position).sqrMagnitude;
				if (sqrMagnitude2 < rangeSqr)
				{
					Timer timer2 = new Timer();
					timer2.SetTimer(timeInterval, true);
					timerDictionary.Add(remotePlayer, timer2);
				}
			}
		}
	}
}
