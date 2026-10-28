using UnityEngine;

public class PlayerSlasherWeaponAttackState02 : PlayerBossState
{
	public override void Start(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		player.PlaySound(AudioName.SLASHER_WEAPON_ATTACK02);
		player.PlayAnimation(AnimationString.SLASHER_WEAPON_ATTACK02, WrapMode.ClampForever);
		player.GetAnimationObject().GetComponent<Animation>()[AnimationString.SLASHER_WEAPON_ATTACK02].speed = SlasherParameter.SLASHER_PLAYER_BOSS_ANIMATION_SPEED[1];
		player.ResetAttack(player.SlasherSkill.WeaponMeleeAttackSkill2);
	}

	public override void Update(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		player.CheckHit2(player.SlasherSkill.WeaponMeleeAttackSkill2);
		StartIdleWhenAnimFinish(player);
	}

	public override void End(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		player.StopSound(AudioName.SLASHER_WEAPON_ATTACK02);
	}
}
