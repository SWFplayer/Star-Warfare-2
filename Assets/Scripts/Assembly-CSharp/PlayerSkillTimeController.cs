using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillTimeController : PlayerSpecialSkill
{
	public Player player;

	public int SkillId;

	private float controlRadius;

	public void Init()
	{
		controlRadius = 20f;
	}

	public void Stun()
	{
		Hashtable enemies = GameApp.GetInstance().GetGameWorld().GetEnemies();
		foreach (Enemy value in enemies.Values)
		{
			if (value != null && value.EnemyType != EnemyType.TANK && !value.IsBoss() && Vector3.Distance(value.GetPosition(), new Vector3(player.GetPosition().x, value.GetPosition().y, player.GetPosition().z)) <= controlRadius && value.InPlayingState() && value.GetState() != Enemy.STUN_STATE && value.GetState() != Enemy.BLACK_HOLE_STATE)
			{
				value.SetState(Enemy.STUN_STATE);
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && GameApp.GetInstance().GetGameMode().IsCoopMode())
				{
					EnemyStateRequest request = new EnemyStateRequest(value.EnemyID, value.GetID(EEnemyStateID.STUN), value.GetPosition());
					GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				}
			}
		}
		if (!GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			return;
		}
		List<RemotePlayer> remotePlayers = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
		List<int> list = new List<int>();
		foreach (RemotePlayer item in remotePlayers)
		{
			if (item != null && item.InPlayingState() && !item.IsSameTeam(player) && Vector3.Distance(item.GetPosition(), new Vector3(player.GetPosition().x, item.GetPosition().y, player.GetPosition().z)) <= controlRadius && item.UnitCanBeHitByRay(player.GetPosition() + Vector3.up, controlRadius))
			{
				list.Add(item.GetChannelID());
				Debug.Log("Control " + item.GetChannelID());
			}
		}
		PlayerBuffRequest request2 = new PlayerBuffRequest(list, SkillId);
		GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
	}
}
