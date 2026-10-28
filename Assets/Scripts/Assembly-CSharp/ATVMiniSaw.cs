using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATVMiniSaw : VehicleWeapon
{
	private List<GameUnit> mTargetList = new List<GameUnit>();

	public override void Init(Player player, float baseDamage, float damageRatio, float damageRadius, float attackFrequency, PlayerDamageType damageType, float bulletSpeed)
	{
		base.Init(player, baseDamage, damageRatio, damageRadius, attackFrequency, damageType, bulletSpeed);
		mWeaponType = EVehicleWeaponType.ATV_MINI_SAW;
	}

	public override void Attack()
	{
		if (!CoolDown())
		{
			return;
		}
		mLastAttackTime = Time.time;
		mTargetList.Clear();
		Hashtable enemies = GameApp.GetInstance().GetGameWorld().GetEnemies();
		foreach (Enemy value in enemies.Values)
		{
			mTargetList.Add(value);
		}
		List<RemotePlayer> remotePlayers = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
		foreach (RemotePlayer item in remotePlayers)
		{
			mTargetList.Add(item);
		}
		foreach (GameUnit mTarget in mTargetList)
		{
			if (mTarget.InPlayingState() && Vector3.SqrMagnitude(mPlayer.GetTransform().position - mTarget.GetTransform().position) < mDamageRadius * mDamageRadius)
			{
				DamageProperty dp = CreateDamageProperty();
				if (mTarget.GameUnitType == EGameUnitType.ENEMY)
				{
					((Enemy)mTarget).HitEnemy(mPlayer, dp);
				}
				else if (mTarget.GameUnitType == EGameUnitType.REMOTE_PLAYER && !((Player)mTarget).IsInAnyShields(dp))
				{
					mPlayer.HitTargetShowCrossHit();
					PlayerHitPlayerRequest request = new PlayerHitPlayerRequest(dp, ((Player)mTarget).GetChannelID());
					GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				}
			}
		}
	}

	public override DamageProperty CreateDamageProperty()
	{
		DamageProperty damageProperty = base.CreateDamageProperty();
		damageProperty.criticalAttack = true;
		damageProperty.damageFactor = 1;
		return damageProperty;
	}
}
