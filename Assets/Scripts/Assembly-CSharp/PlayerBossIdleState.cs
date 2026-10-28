using UnityEngine;

internal class PlayerBossIdleState : PlayerBossState
{
	public override void Start(GameUnit gameUnit)
	{
		Player player = (Player)gameUnit;
		InputController inputController = player.inputController;
		if (inputController.inputInfo.IsMoving())
		{
			player.PlayAnimation(AnimationString.ENEMY_RUN, WrapMode.Loop);
		}
		else if (player.mBossType == EnemyType.SLASHER)
		{
			player.PlayAnimation(AnimationString.SLASHER_WEAPON_IDLE, WrapMode.Loop);
		}
		else
		{
			player.PlayAnimation(AnimationString.ENEMY_IDLE, WrapMode.Loop);
		}
	}

	public override void Update(GameUnit gameUnit)
	{
		Player player = (Player)gameUnit;
		InputController inputController = player.inputController;
		if (player.IsLocal())
		{
			player.SimpleMove(inputController.inputInfo.moveDirection * (PlayerBossParameter.BossMoveSpeed[player.BossIndex] + player.GetSkills().GetSkill(SkillsType.SPEED_BOOTH)));
		}
		if (inputController.inputInfo.IsMoving())
		{
			player.PlayAnimation(AnimationString.ENEMY_RUN, WrapMode.Loop);
			AudioManager.GetInstance().PlaySoundSingleLoop(player.CurrentBossSkill.MoveAudioName, player.GetObject(), player.GetTransform());
		}
		else
		{
			AudioManager.GetInstance().StopSound(player.CurrentBossSkill.MoveAudioName);
			player.PlayAnimation(AnimationString.ENEMY_IDLE, WrapMode.Loop);
		}
		if (player.IsLocal() && Input.GetButtonUp("Jump"))
		{
			player.SetState(Player.BOSS_JUMP_STATE);
			if (GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
			{
				PlayerBossStateRequest request = new PlayerBossStateRequest(Player.BOSS_JUMP_STATE);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
	}

	public override void End(GameUnit gameUnit)
	{
		base.End(gameUnit);
		Player player = (Player)gameUnit;
		AudioManager.GetInstance().StopSound(player.CurrentBossSkill.MoveAudioName);
	}
}
