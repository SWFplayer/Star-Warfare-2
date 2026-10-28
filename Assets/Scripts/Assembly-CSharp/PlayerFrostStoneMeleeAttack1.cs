using UnityEngine;

public class PlayerFrostStoneMeleeAttack1 : PlayerBossState
{
	public override void Start(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		player.PlayAnimation(AnimationString.BOSS_ATTACK03, WrapMode.ClampForever);
		player.GetAnimationObject().GetComponent<Animation>()[AnimationString.BOSS_ATTACK03].speed = FrostStoneParameter.FROST_STONE_PLAYER_BOSS_ANIMATION_SPEED[0];
		player.PlaySecondAnimation("attack03point");
		player.ResetAttack(player.FrostStoneSkill.MeleeAttackSkill1);
		player.PlaySound(AudioName.FROSTSTONE_ATTACK01);
	}

	public override void Update(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		player.CheckColliderHit(player.FrostStoneSkill.MeleeAttackSkill1);
		StartIdleWhenAnimFinish(player);
	}

	public override void End(GameUnit gameUnit)
	{
		gameUnit.StopSecondAnimation();
	}

	public override void ChangeInnerState(Player player, int innerState)
	{
	}
}
