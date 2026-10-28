using UnityEngine;

public class PlayerCerberliskMeleeAttack2 : PlayerBossState
{
	public override void Start(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		player.PlayAnimation(AnimationString.BOSS_ATTACK02, WrapMode.ClampForever);
		player.GetAnimationObject().GetComponent<Animation>()[AnimationString.BOSS_ATTACK02].speed = CerberliskParameter.CERBERLISK_PLAYER_BOSS_ANIMATION_SPEED[1];
		player.ResetAttack(player.CerberliskSkill.MeleeAttackSkill2);
		player.PlaySound(AudioName.CERBERLISK_ATTACK02);
	}

	public override void Update(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		player.CheckColliderHit(player.CerberliskSkill.MeleeAttackSkill2);
		StartIdleWhenAnimFinish(player);
	}

	public override void End(GameUnit gameUnit)
	{
	}

	public override void ChangeInnerState(Player player, int innerState)
	{
	}
}
