using UnityEngine;

public class PlayerGetOffCarrierState : PlayerState
{
	public const float MoveSpeed = 5f;

	private Vector3 targetPosition = Vector3.zero;

	public override void NextState(Player player, float deltaTime)
	{
		if (player.mVehicle != null && (player.mVehicle.VehicleType == EVehicleType.ATV_LASER || player.mVehicle.VehicleType == EVehicleType.ATV_MINI || player.mVehicle.VehicleType == EVehicleType.LONG_SWORD))
		{
			player.PlayAnimation(player.mVehicle.GetNamePrefex() + "_" + AnimationString.GetOffCarrier, WrapMode.ClampForever);
			if (player.mVehicle.VehicleType != EVehicleType.LONG_SWORD)
			{
				player.mVehicle.GetTransform().Translate(0f, 0f, 5f * Time.deltaTime, Space.Self);
			}
			if (player.IsAnimationFinish())
			{
				player.SetState(Player.IDLE_STATE);
			}
		}
		else if (player.mVehicle != null && player.mVehicle.VehicleType == EVehicleType.LONG_SWORD_CANNON)
		{
			if (targetPosition == Vector3.zero)
			{
				float deltaHeight = player.mVehicle.GetDeltaHeight();
				targetPosition = player.mVehicle.GetPosition() + 2.5f * player.GetTransform().forward;
				Ray ray = new Ray(targetPosition + 2.5f * Vector3.up, Vector3.down);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, 8f, 1 << PhysicsLayer.FLOOR))
				{
					targetPosition = hitInfo.point + new Vector3(0f, 0.1f, 0f);
				}
				else
				{
					targetPosition = player.mVehicle.GetPosition() + new Vector3(0f, 0.1f, 0f);
				}
			}
			player.PlayAnimationAllLayers(player.mVehicle.GetNamePrefex() + "_" + AnimationString.GetOffCarrier, WrapMode.ClampForever);
			player.GetCharacterController().enabled = false;
			GameApp.GetInstance().GetGameWorld().GetCamera()
				.ZoomToLongSwordMain(Time.deltaTime);
			Vector3 position = Vector3.MoveTowards(player.GetPosition(), targetPosition, 5f * Time.deltaTime);
			player.SetPosition(position);
			if (!player.IsLocal())
			{
				((RemotePlayer)player).GetInterpolation().SetTransform(player.GetTransform());
			}
			if (player.IsAnimationFinish())
			{
				player.GetCharacterController().enabled = true;
				player.mVehicle = null;
				player.SetState(Player.IDLE_STATE);
			}
		}
		else
		{
			player.SetState(Player.IDLE_STATE);
		}
		if (Time.time - player.StateStartTime > 5f)
		{
			player.SetState(Player.IDLE_STATE);
		}
	}

	public override void End(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		LeaveState(player);
	}

	private void LeaveState(Player player)
	{
		targetPosition = Vector3.zero;
		player.StopAnimation();
		player.GetWeapon().GunOn();
	}
}
