using UnityEngine;

public class PlayerCerberliskMeleeAttack1 : PlayerBossState
{
	public override void Start(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		player.PlayAnimation(AnimationString.BOSS_ATTACK01, WrapMode.ClampForever);
		player.GetAnimationObject().GetComponent<Animation>()[AnimationString.BOSS_ATTACK01].speed = CerberliskParameter.CERBERLISK_PLAYER_BOSS_ANIMATION_SPEED[0];
		player.ResetAttack(player.CerberliskSkill.MeleeAttackSkill1);
	}

	public override void Update(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		player.CheckColliderHit(player.CerberliskSkill.MeleeAttackSkill1);
		StartIdleWhenAnimFinish(player);
		if (player.IsAnimationTriggered(0.2f))
		{
			player.PlaySound(AudioName.CERBERLISK_ATTACK01);
		}
	}

	public override void End(GameUnit gameUnit)
	{
	}

	public override void ChangeInnerState(Player player, int innerState)
	{
	}
}
