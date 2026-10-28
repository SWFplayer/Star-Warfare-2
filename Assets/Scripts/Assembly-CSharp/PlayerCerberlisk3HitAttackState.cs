using UnityEngine;

public class PlayerCerberlisk3HitAttackState : PlayerBossState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;
	}

	public override void Start(GameUnit gameUnit)
	{
		MyData data = GetData<MyData>(gameUnit);
		data.InnerState = 0;
		Player player = gameUnit as Player;
		player.PlayAnimation("3hit01", WrapMode.ClampForever);
		player.GetAnimationObject().GetComponent<Animation>()["3hit01"].speed = CerberliskParameter.CERBERLISK_PLAYER_BOSS_ANIMATION_SPEED[2];
		player.PlaySecondAnimation("3hit01_point");
		player.GetAnimationObject().GetComponent<Animation>()["3hit01_point"].speed = CerberliskParameter.CERBERLISK_PLAYER_BOSS_ANIMATION_SPEED[2];
		player.ResetAttack(player.CerberliskSkill.ThreeHitAttack1);
	}

	public override void Update(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		MyData data = GetData<MyData>(gameUnit);
		if (data.InnerState == 0)
		{
			RestrictRotationSpeed(player, CerberliskParameter.THREE_ATTACK_ROTATE_SPEED1);
			player.CheckHit2(player.CerberliskSkill.ThreeHitAttack1);
			if (player.IsAnimationFinish())
			{
				data.InnerState++;
				player.PlayAnimation("3hit02", WrapMode.ClampForever);
				player.PlaySecondAnimation("3hit02_point");
				player.ResetAttack(player.CerberliskSkill.ThreeHitAttack2);
			}
			if (player.IsAnimationTriggered(0.21f))
			{
				player.PlaySound(AudioName.CERBERLISK_3HIT);
			}
		}
		else if (data.InnerState == 1)
		{
			RestrictRotationSpeed(player, CerberliskParameter.THREE_ATTACK_ROTATE_SPEED2);
			player.CheckHit2(player.CerberliskSkill.ThreeHitAttack2);
			if (player.IsAnimationFinish())
			{
				data.InnerState++;
				player.PlayAnimation("3hit03", WrapMode.ClampForever);
				player.PlaySecondAnimation("3hit03_point");
				player.ResetAttack(player.CerberliskSkill.ThreeHitAttack3);
			}
			if (player.IsAnimationTriggered(0.54f))
			{
				player.PlaySound(AudioName.CERBERLISK_3HIT);
			}
		}
		else if (data.InnerState == 2)
		{
			RestrictRotationSpeed(player, CerberliskParameter.THREE_ATTACK_ROTATE_SPEED3);
			player.CheckHit2(player.CerberliskSkill.ThreeHitAttack3);
			StartIdleWhenAnimFinish(player);
			if (player.IsAnimationTriggered(0.51f))
			{
				player.PlaySound(AudioName.CERBERLISK_3HIT);
			}
		}
	}

	public override void End(GameUnit gameUnit)
	{
		gameUnit.StopSecondAnimation();
	}

	public override void DoResponse(GameUnit gameUnit, BytesBuffer buffer)
	{
	}
}
