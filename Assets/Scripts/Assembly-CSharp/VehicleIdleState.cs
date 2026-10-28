using UnityEngine;

public class VehicleIdleState : PlayerState
{
	public override void NextState(Player player, float deltaTime)
	{
		player.GetWeapon().StopGunAnim();
		InputController inputController = player.inputController;
		player.mVehicle.StopGunEffect();
		bool flag = ((player.GetSkills().GetSkill(SkillsType.FLY) > 0f) ? true : false);
		if (player.IsLocal())
		{
			if (player.mVehicleState == EPlayerVehicleState.CARRIER)
			{
				if (player.mVehicle != null && player.mVehicle.VehicleType == EVehicleType.LONG_SWORD)
				{
					GameApp.GetInstance().GetGameWorld().GetCamera()
						.ZoomToLongSwordMain(deltaTime);
				}
				else
				{
					GameApp.GetInstance().GetGameWorld().GetCamera()
						.ZoomToVehicle(deltaTime);
				}
				player.mVehicle.Move(inputController.inputInfo.moveDirection);
			}
			else if (player.mVehicleState == EPlayerVehicleState.CANNON)
			{
				if (player.mVehicle != null && player.mVehicle.VehicleType == EVehicleType.LONG_SWORD_CANNON)
				{
					GameApp.GetInstance().GetGameWorld().GetCamera()
						.ZoomToLongSwordSub(deltaTime);
				}
				else
				{
					GameApp.GetInstance().GetGameWorld().GetCamera()
						.ZoomToCannonAttack(deltaTime);
				}
			}
		}
		if (player.mVehicleState == EPlayerVehicleState.CARRIER)
		{
			if (inputController.inputInfo.IsMoving())
			{
				if (player.IsLocal())
				{
					player.PlayAnimation(player.mVehicle.GetNamePrefex() + "_" + AnimationString.GetDirectionString(inputController.inputInfo.dir), WrapMode.Loop, 0.5f);
					player.mVehicle.PlayAnimation(AnimationString.GetDirectionString(inputController.inputInfo.dir), WrapMode.Loop, 0.5f);
				}
				else
				{
					player.PlayAnimation(player.mVehicle.GetNamePrefex() + "_" + AnimationString.GetDirectionString(((RemotePlayer)player).GetInterpolation().moveDirection), WrapMode.Loop, 0.5f);
					player.mVehicle.PlayAnimation(AnimationString.GetDirectionString(((RemotePlayer)player).GetInterpolation().moveDirection), WrapMode.Loop, 0.5f);
				}
				AudioManager.GetInstance().StopSound(player.mVehicle.GetIdleSoundName(), player.GetObject());
				AudioManager.GetInstance().PlaySoundSingleLoop(player.mVehicle.GetMoveSoundName(), player.GetObject(), player.GetTransform());
			}
			else
			{
				player.PlayAnimation(player.mVehicle.GetNamePrefex() + "_" + AnimationString.Idle, WrapMode.Loop, 0.5f);
				player.mVehicle.PlayAnimation(AnimationString.Idle, WrapMode.Loop, 0.5f);
				AudioManager.GetInstance().StopSound(player.mVehicle.GetMoveSoundName(), player.GetObject());
				AudioManager.GetInstance().PlaySoundSingleLoop(player.mVehicle.GetIdleSoundName(), player.GetObject(), player.GetTransform());
			}
		}
		else if (player.mVehicleState == EPlayerVehicleState.CANNON)
		{
			if (player.mVehicle != null && player.mVehicle.VehicleType != EVehicleType.LONG_SWORD_CANNON)
			{
				float num = ((!player.IsLocal()) ? ((RemotePlayer)player).GetInterpolation().cannonTurn : GameApp.GetInstance().GetGameWorld().GetCamera()
					.cannonTurn);
				if (num == 0f)
				{
					player.PlayAnimation(player.mVehicle.GetNamePrefex() + "_" + AnimationString.Idle, WrapMode.Loop, 0.5f);
					player.mVehicle.PlayAnimation(AnimationString.Idle, WrapMode.Loop, 0.5f);
				}
				else if (num > 0f)
				{
					player.PlayAnimation(player.mVehicle.GetNamePrefex() + "_" + AnimationString.DIRECTION_RIGHT, WrapMode.Loop, 0.5f);
					player.mVehicle.PlayAnimation(AnimationString.Idle, WrapMode.Loop, 0.5f);
				}
				else
				{
					player.PlayAnimation(player.mVehicle.GetNamePrefex() + "_" + AnimationString.DIRECTION_LEFT, WrapMode.Loop, 0.5f);
					player.mVehicle.PlayAnimation(AnimationString.Idle, WrapMode.Loop, 0.5f);
				}
			}
			else if (player.mVehicle != null)
			{
				string text = "idle";
				if (((VehicleLongSwordCannon)player.mVehicle).GetMainRider() != null)
				{
					Player mainRider = ((VehicleLongSwordCannon)player.mVehicle).GetMainRider();
					if (mainRider.inputController.inputInfo.IsMoving())
					{
						text = ((!mainRider.IsLocal()) ? AnimationString.GetDirectionString(((RemotePlayer)mainRider).GetInterpolation().moveDirection) : AnimationString.GetDirectionString(mainRider.inputController.inputInfo.dir));
					}
				}
				player.mVehicle.PlayAnimation(AnimationString.Idle, WrapMode.Loop, 0.5f);
				player.PlayAnimation(player.mVehicle.GetNamePrefex() + "_" + text, WrapMode.Loop, 0.5f);
				AnimationState animationState = player.GetObject().GetComponent<Animation>()[player.mVehicle.GetNamePrefex() + "_" + AnimationString.VEHICLE_SHOOT + "_" + AnimationString.DIRECTION_LEFT];
				AnimationState animationState2 = player.GetObject().GetComponent<Animation>()[player.mVehicle.GetNamePrefex() + "_" + AnimationString.VEHICLE_SHOOT + "_" + AnimationString.DIRECTION_RIGHT];
				float num2 = (player.mVehicle.GetRotationTransform().localRotation.eulerAngles.y - 180f) / 70f;
				animationState.weight = 0f - num2;
				animationState2.weight = num2;
				animationState.layer = 10;
				animationState2.layer = 10;
				if (num2 < 0f)
				{
					player.PlayAnimation(animationState.name, WrapMode.Loop, 0.5f);
				}
				else if (num2 > 0f)
				{
					player.PlayAnimation(animationState2.name, WrapMode.Loop, 0.5f);
				}
			}
		}
		if (inputController.inputInfo.fire)
		{
			player.SetState(Player.VEHICLE_ATTACK_STATE);
			player.mVehicle.StartLoopingSound();
		}
	}
}
