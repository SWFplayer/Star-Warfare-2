using System.Collections.Generic;
using UnityEngine;

public class HitCheck
{
	public float Interval;

	private Dictionary<GameObject, Timer> mHitDictionary = new Dictionary<GameObject, Timer>();

	public Transform mTransform;

	private bool IsLocalBoss
	{
		get
		{
			return GameApp.GetInstance().GetGameMode().IsBossVSPlayer() && GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.IsBoss;
		}
	}

	public HitCheck()
	{
		Interval = 999f;
	}

	public HitCheck(float interval)
	{
		Interval = interval;
	}

	public void Clear()
	{
		mHitDictionary.Clear();
	}

	public bool CanHit(GameObject go)
	{
		if (!mHitDictionary.ContainsKey(go))
		{
			mHitDictionary[go] = new Timer(Interval);
			return true;
		}
		if (mHitDictionary[go].Ready2())
		{
			return true;
		}
		return false;
	}

	public bool OnHit(GameUnit attacker, GameObject target, byte DamageType, int Damage, PlayerDamageType PlayerDamageType, float knockSpeed = 0f)
	{
		Player player = null;
		if (GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
		{
			if (IsLocalBoss && target.layer == PhysicsLayer.REMOTE_PLAYER && CanHit(target))
			{
				int num = int.Parse(target.name);
				player = GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(num);
				DamageProperty damageProperty = new DamageProperty(attacker, Damage, PlayerDamageType);
				damageProperty.weaponid = DamageType;
				damageProperty.mAttackerType = AttackerType.PLAYER_BOSS;
				if (!player.IsInAnyShields(damageProperty))
				{
					PlayerHitPlayerRequest request = new PlayerHitPlayerRequest(damageProperty, num);
					GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				}
				return true;
			}
		}
		else if (target.layer == PhysicsLayer.PLAYER && CanHit(target))
		{
			player = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			DamageProperty damageProperty2 = new DamageProperty(attacker, Damage, PlayerDamageType);
			damageProperty2.EnemySkillType = DamageType;
			player.OnHit(damageProperty2);
			if (knockSpeed > 0f)
			{
				Vector3 vector = new Vector3(player.GetTransform().forward.x, 0f, player.GetTransform().forward.z);
				Vector3 to = player.GetPosition() - mTransform.position;
				float f = Vector3.Angle(vector, to);
				float num2 = knockSpeed;
				if (Mathf.Abs(f) > 90f)
				{
					num2 = 0f - num2;
				}
				player.OnKnocked(num2);
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
				{
					PlayerOnKnockedRequest request2 = new PlayerOnKnockedRequest(num2, player.GetChannelID());
					GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
				}
			}
			return true;
		}
		return false;
	}

	public void CheckHit(GameUnit attacker, Vector3 position, float radiusSqr, byte DamageType, int Damage, PlayerDamageType playerDamageType)
	{
		if (GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
		{
			List<RemotePlayer> remotePlayers = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
			{
				foreach (RemotePlayer item in remotePlayers)
				{
					if (CanHit(item.GetObject()) && Vector3.SqrMagnitude(position - item.GetPosition()) < radiusSqr)
					{
						DamageProperty damageProperty = new DamageProperty(attacker, 0, playerDamageType);
						damageProperty.weaponid = DamageType;
						damageProperty.mAttackerType = AttackerType.PLAYER_BOSS;
						if (!item.IsInAnyShields(damageProperty))
						{
							PlayerHitPlayerRequest request = new PlayerHitPlayerRequest(damageProperty, item.GetChannelID());
							GameApp.GetInstance().GetNetworkManager().SendRequest(request);
						}
					}
				}
				return;
			}
		}
		Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		if (CanHit(localPlayer.GetObject()) && Vector3.SqrMagnitude(position - localPlayer.GetPosition()) < radiusSqr)
		{
			DamageProperty damageProperty2 = new DamageProperty(attacker, Damage, playerDamageType);
			damageProperty2.EnemySkillType = DamageType;
			localPlayer.OnHit(damageProperty2);
		}
	}

	public bool OnPassiveHit(GameUnit attacker, GameObject target, byte DamageType, int Damage, PlayerDamageType playerDamageType, float knockSpeed = 0f)
	{
		if (target.layer == PhysicsLayer.PLAYER && CanHit(target) && !GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.IsBoss)
		{
			Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			if (GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
			{
				DamageProperty damageProperty = new DamageProperty(attacker, Damage, playerDamageType);
				damageProperty.weaponid = DamageType;
				damageProperty.mAttackerType = AttackerType.PLAYER_BOSS;
				if (!localPlayer.IsInAnyShields(damageProperty))
				{
					PlayerHitPlayerRequest request = new PlayerHitPlayerRequest(damageProperty, localPlayer.GetChannelID());
					GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				}
			}
			else
			{
				DamageProperty damageProperty2 = new DamageProperty(attacker, Damage, playerDamageType);
				damageProperty2.EnemySkillType = DamageType;
				localPlayer.OnHit(damageProperty2);
			}
			if (knockSpeed > 0f)
			{
				Vector3 vector = new Vector3(localPlayer.GetTransform().forward.x, 0f, localPlayer.GetTransform().forward.z);
				Vector3 to = localPlayer.GetPosition() - mTransform.position;
				float f = Vector3.Angle(vector, to);
				float num = knockSpeed;
				if (Mathf.Abs(f) > 90f)
				{
					num = 0f - num;
				}
				localPlayer.OnKnocked(num);
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
				{
					PlayerOnKnockedRequest request2 = new PlayerOnKnockedRequest(num, localPlayer.GetChannelID());
					GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
				}
			}
			return true;
		}
		return false;
	}
}
