using UnityEngine;

public class VehicleAttackState : PlayerState
{
	public override void Start(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		if (player != null && player.mVehicleState == EPlayerVehicleState.CARRIER && player.mVehicle != null)
		{
			float speed = 1f;
			if (player.mVehicle.VehicleType == EVehicleType.ATV_LASER)
			{
				speed = 0.25f;
			}
			player.GetObject().GetComponent<Animation>()["ATV_mini_shoot_back"].speed = speed;
			player.GetObject().GetComponent<Animation>()["ATV_mini_shoot_forward"].speed = speed;
			player.GetObject().GetComponent<Animation>()["ATV_mini_shoot_idle"].speed = speed;
			player.GetObject().GetComponent<Animation>()["ATV_mini_shoot_left"].speed = speed;
			player.GetObject().GetComponent<Animation>()["ATV_mini_shoot_right"].speed = speed;
		}
	}

	public override void NextState(Player player, float deltaTime)
	{
		InputController inputController = player.inputController;
		player.GetWeapon().StopGunAnim();
		if (player.IsLocal())
		{
			if (player.mVehicleState == EPlayerVehicleState.CANNON)
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
			else if (player.mVehicleState == EPlayerVehicleState.CARRIER)
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
		}
		WrapMode mode = WrapMode.Loop;
		if (!player.mVehicle.IsTypeOfLoopShooting())
		{
			mode = WrapMode.ClampForever;
		}
		if (player.mVehicle.CoolDown())
		{
			if (player.IsLocal())
			{
				player.mVehicle.Attack();
			}
			else
			{
				player.mVehicle.CreateTrajectory();
			}
			if (player.mVehicleState == EPlayerVehicleState.CARRIER)
			{
				if (inputController.inputInfo.IsMoving())
				{
					if (player.IsLocal())
					{
						player.PlayAnimation(player.mVehicle.GetNamePrefex() + "_" + AnimationString.VEHICLE_SHOOT + "_" + AnimationString.GetDirectionString(inputController.inputInfo.dir), mode, 0.5f);
						player.mVehicle.PlayAnimation(AnimationString.VEHICLE_SHOOT + "_" + AnimationString.GetDirectionString(inputController.inputInfo.dir), mode, 0.5f);
					}
					else
					{
						player.PlayAnimation(player.mVehicle.GetNamePrefex() + "_" + AnimationString.VEHICLE_SHOOT + "_" + AnimationString.GetDirectionString(((RemotePlayer)player).GetInterpolation().moveDirection), mode, 0.5f);
						player.mVehicle.PlayAnimation(AnimationString.VEHICLE_SHOOT + "_" + AnimationString.GetDirectionString(((RemotePlayer)player).GetInterpolation().moveDirection), mode, 0.5f);
					}
					AudioManager.GetInstance().StopSound(player.mVehicle.GetIdleSoundName(), player.GetObject());
					AudioManager.GetInstance().PlaySoundSingleLoop(player.mVehicle.GetMoveSoundName(), player.GetObject(), player.GetTransform());
				}
				else
				{
					player.PlayAnimation(player.mVehicle.GetNamePrefex() + "_" + AnimationString.VEHICLE_SHOOT + "_" + AnimationString.Idle, mode, 0.5f);
					player.mVehicle.PlayAnimation(AnimationString.VEHICLE_SHOOT + "_" + AnimationString.Idle, mode, 0.5f);
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
						player.PlayAnimation(player.mVehicle.GetNamePrefex() + "_" + AnimationString.VEHICLE_SHOOT + "_" + AnimationString.Idle, mode, 0.5f);
						player.mVehicle.PlayAnimation(AnimationString.VEHICLE_SHOOT + "_" + AnimationString.Idle, mode, 0.5f);
					}
					else if (num > 0f)
					{
						player.PlayAnimation(player.mVehicle.GetNamePrefex() + "_" + AnimationString.VEHICLE_SHOOT + "_" + AnimationString.DIRECTION_RIGHT, mode, 0.5f);
						player.mVehicle.PlayAnimation(AnimationString.VEHICLE_SHOOT + "_" + AnimationString.Idle, mode, 0.5f);
					}
					else
					{
						player.PlayAnimation(player.mVehicle.GetNamePrefex() + "_" + AnimationString.VEHICLE_SHOOT + "_" + AnimationString.DIRECTION_LEFT, mode, 0.5f);
						player.mVehicle.PlayAnimation(AnimationString.VEHICLE_SHOOT + "_" + AnimationString.Idle, mode, 0.5f);
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
					player.mVehicle.PlayAnimation(AnimationString.VEHICLE_SHOOT + "_" + AnimationString.Idle, mode, 0.5f);
					player.PlayAnimation(player.mVehicle.GetNamePrefex() + "_" + text, mode, 0.5f);
					AnimationState animationState = player.GetObject().GetComponent<Animation>()[player.mVehicle.GetNamePrefex() + "_" + AnimationString.VEHICLE_SHOOT + "_" + AnimationString.DIRECTION_LEFT];
					AnimationState animationState2 = player.GetObject().GetComponent<Animation>()[player.mVehicle.GetNamePrefex() + "_" + AnimationString.VEHICLE_SHOOT + "_" + AnimationString.DIRECTION_RIGHT];
					float num2 = (player.mVehicle.GetRotationTransform().localRotation.eulerAngles.y - 180f) / 70f;
					animationState.weight = 0f - num2;
					animationState2.weight = num2;
					animationState.layer = 10;
					animationState2.layer = 10;
					if (num2 < 0f)
					{
						player.PlayAnimation(animationState.name, mode, 0.5f);
					}
					else if (num2 > 0f)
					{
						player.PlayAnimation(animationState2.name, mode, 0.5f);
					}
				}
			}
		}
		else
		{
			if (!player.mVehicle.IsTypeOfLoopShooting())
			{
				if (inputController.inputInfo.IsMoving())
				{
					if (player.mVehicleState == EPlayerVehicleState.CARRIER)
					{
						player.PlayAnimation(player.mVehicle.GetNamePrefex() + "_" + AnimationString.GetDirectionString(inputController.inputInfo.dir), WrapMode.Loop, 0.5f);
						player.mVehicle.PlayAnimation(AnimationString.GetDirectionString(inputController.inputInfo.dir), WrapMode.Loop, 0.5f);
					}
				}
				else if (player.mVehicleState == EPlayerVehicleState.CARRIER)
				{
					player.PlayAnimation(player.mVehicle.GetNamePrefex() + "_" + AnimationString.Idle, WrapMode.Loop, 0.5f);
					player.mVehicle.PlayAnimation(AnimationString.Idle, WrapMode.Loop, 0.5f);
				}
			}
			if (player.mVehicle != null && player.mVehicle.VehicleType == EVehicleType.LONG_SWORD_CANNON)
			{
				string text2 = "idle";
				if (((VehicleLongSwordCannon)player.mVehicle).GetMainRider() != null)
				{
					Player mainRider2 = ((VehicleLongSwordCannon)player.mVehicle).GetMainRider();
					if (mainRider2.inputController.inputInfo.IsMoving())
					{
						text2 = ((!mainRider2.IsLocal()) ? AnimationString.GetDirectionString(((RemotePlayer)mainRider2).GetInterpolation().moveDirection) : AnimationString.GetDirectionString(mainRider2.inputController.inputInfo.dir));
					}
				}
				player.mVehicle.PlayAnimation(AnimationString.VEHICLE_SHOOT + "_" + AnimationString.Idle, mode, 0.5f);
				player.PlayAnimation(player.mVehicle.GetNamePrefex() + "_" + text2, mode, 0.5f);
				AnimationState animationState3 = player.GetObject().GetComponent<Animation>()[player.mVehicle.GetNamePrefex() + "_" + AnimationString.VEHICLE_SHOOT + "_" + AnimationString.DIRECTION_LEFT];
				AnimationState animationState4 = player.GetObject().GetComponent<Animation>()[player.mVehicle.GetNamePrefex() + "_" + AnimationString.VEHICLE_SHOOT + "_" + AnimationString.DIRECTION_RIGHT];
				float num3 = (player.mVehicle.GetRotationTransform().localRotation.eulerAngles.y - 180f) / 70f;
				animationState3.weight = 0f - num3;
				animationState4.weight = num3;
				animationState3.layer = 10;
				animationState4.layer = 10;
				if (num3 < 0f)
				{
					player.PlayAnimation(animationState3.name, mode, 0.5f);
				}
				else if (num3 > 0f)
				{
					player.PlayAnimation(animationState4.name, mode, 0.5f);
				}
			}
		}
		if (!inputController.inputInfo.fire && (player.mVehicle.IsTypeOfLoopShooting() || ((!player.IsPlayingAnimation(player.mVehicle.GetNamePrefex() + "_" + AnimationString.VEHICLE_SHOOT + "_" + AnimationString.Idle) || player.AnimationPlayed(player.mVehicle.GetNamePrefex() + "_" + AnimationString.VEHICLE_SHOOT + "_" + AnimationString.Idle, 1f)) && (!inputController.inputInfo.IsMoving() || !player.IsPlayingAnimation(player.mVehicle.GetNamePrefex() + "_" + AnimationString.VEHICLE_SHOOT + "_" + AnimationString.GetDirectionString(inputController.inputInfo.dir)) || player.AnimationPlayed(player.mVehicle.GetNamePrefex() + "_" + AnimationString.VEHICLE_SHOOT + "_" + AnimationString.GetDirectionString(inputController.inputInfo.dir), 1f)))))
		{
			player.SetState(Player.VEHICLE_IDLE_STATE);
			player.mVehicle.EndLoopingSound();
		}
	}
}
