using UnityEngine;

public class AttackState : PlayerState
{
	public override void Start(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		if (player.GetWeapon() != null)
		{
			player.GetWeapon().StartAttackState();
		}
	}

	public override void NextState(Player player, float deltaTime)
	{
		InputController inputController = player.inputController;
		Weapon weapon = player.GetWeapon();
		bool flag = ((player.GetSkills().GetSkill(SkillsType.FLY) > 0f) ? true : false);
		if (player.IsLocal())
		{
			if (flag)
			{
				GameApp.GetInstance().GetGameWorld().GetCamera()
					.SetPivotOffset(Vector3.up * 0.25f);
			}
			player.GetWeapon().AttackZoom(deltaTime);
			player.Move(inputController.inputInfo.moveDirection);
			if (inputController.inputInfo.IsMoving())
			{
				if (flag)
				{
					AudioManager.GetInstance().StopSound("float_idle");
					AudioManager.GetInstance().PlaySoundSingleAt("_Old/_Audio/float_move", player.GetTransform().position);
				}
				else
				{
					AudioManager.GetInstance().PlaySoundSingleLoop(AudioName.PLAYER_FOOTSTEP, player.GetObject(), player.GetTransform());
				}
			}
			else if (flag)
			{
				AudioManager.GetInstance().StopSound("float_move");
				AudioManager.GetInstance().PlaySoundSingleAt("_Old/_Audio/float_idle", player.GetTransform().position);
			}
			else
			{
				AudioManager.GetInstance().StopSound(AudioName.PLAYER_FOOTSTEP, player.GetObject());
			}
		}
		player.GetWeapon().PlaySound();
		player.GetWeapon().Loop(deltaTime);
		player.GetWeapon().AutoDestructEffect();
		player.GetWeapon().UpdateAttackState(deltaTime);
		player.GetWeapon().PlayFireAnim();
	}
}
