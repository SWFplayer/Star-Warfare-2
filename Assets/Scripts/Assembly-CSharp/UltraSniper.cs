using System.Collections.Generic;
using UnityEngine;

public class UltraSniper : Sniper
{
	private const float EXPLODE_RADIUS = 4f;

	private const float EXPLODE_DAMAGE_SCALE = 3f;

	public override WeaponType GetWeaponType()
	{
		return WeaponType.UltraSniper;
	}

	public override void Init(Player player)
	{
		base.Init(player);
		base.ExplodeSoundName = AudioName.GUN_03_RPG_BOOM;
	}

	public void ExplodeAt(Vector3 pos)
	{
		gameWorld.GetEffectPool(EffectPoolType.W9_EXPLOSION).CreateObject(pos, Vector3.zero, Quaternion.identity);
		AudioManager.GetInstance().PlaySoundAt(base.ExplodeSoundName, pos);
		if (player.IsLocal())
		{
			DamageProperty damageProperty = new DamageProperty(player, (int)(damage * 3f));
			damageProperty.criticalAttack = false;
			if (player.IsLocal())
			{
				damageProperty.isLocal = true;
			}
			else
			{
				damageProperty.isLocal = false;
			}
			damageProperty.wType = WeaponType.UltraSniper_Exp;
			damageProperty.damageType = PlayerDamageType.EXPLOSION;
			damageProperty.damageFactor = 1;
			damageProperty.buff += player.GetSkills().GetSkill(SkillsType.ALL_DAMAGE);
			damageProperty.weaponid = gunID;
			if (damageType != PlayerDamageType.NORMAL)
			{
				SkillsType sType = (SkillsType)(damageType - 1 + 53);
				damageProperty.buff += player.GetSkills().GetSkill(sType);
			}
			BulletProjectile bulletProjectile = BulletScript.CreateBullet(BulletType.Projectile, player) as BulletProjectile;
			if (bulletProjectile != null)
			{
				bulletProjectile.ExplosionDetect(damageProperty, null, false, 100, new HashSet<PenerateHitInfo>(), pos, 4f, false);
			}
		}
	}
}
