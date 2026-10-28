using UnityEngine;

public class PlayerFreezeState : PlayerState
{
	public override void Start(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		player.GetWeapon().StopFire();
		player.GetWeapon().StopSound();
	}

	public override void NextState(Player player, float deltaTime)
	{
		if ((player.GetSkills().GetSkill(SkillsType.FLY) > 0f) ? true : false)
		{
			player.PlayAnimation(AnimationString.FlyIdle, WrapMode.Loop);
			player.PlayAnimation(AnimationString.Fly + "_" + player.GetWeaponAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
			AudioManager.GetInstance().StopSound("float_move");
			AudioManager.GetInstance().PlaySoundSingleAt("_Old/_Audio/float_idle", player.GetTransform().position);
		}
		else
		{
			player.PlayAnimationAllLayers(AnimationString.Stand + "_" + player.GetWeaponAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
			AudioManager.GetInstance().StopSound(AudioName.PLAYER_FOOTSTEP, player.GetObject());
		}
		if (player.IsLocal())
		{
			InputController inputController = player.inputController;
			inputController.CameraRotation = Vector2.zero;
		}
	}
}
