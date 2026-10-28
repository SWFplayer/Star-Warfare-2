using UnityEngine;

public class PlayerGetOnCarrierState : PlayerState
{
	private const float RotateSpeed = 10000f;

	public override void NextState(Player player, float deltaTime)
	{
		player.PlayAnimationAllLayers(player.mVehicle.GetNamePrefex() + "_" + AnimationString.GetOnCarrier, WrapMode.ClampForever);
		float deltaHeight = player.mVehicle.GetDeltaHeight();
		Vector3 vector = player.mVehicle.GetPosition() + Vector3.up * deltaHeight;
		if (player.mVehicle.VehicleType == EVehicleType.LONG_SWORD_CANNON)
		{
			vector -= player.mVehicle.GetTransform().forward * 0.5f;
		}
		if (player.IsAnimationPlayed(player.mVehicle.StartMoveTime))
		{
			float f = Vector3.Distance(player.GetPosition(), vector);
			f = Mathf.Clamp(Mathf.Pow(f, 2f), 5f, 100f);
			Vector3 position = Vector3.MoveTowards(player.GetPosition(), vector, f * Time.deltaTime);
			player.SetPosition(position);
			Vector3 vector2 = Vector3.RotateTowards(player.GetTransform().forward, player.mVehicle.GetForward(), 10000f * Time.deltaTime, 10000f);
			player.GetTransform().LookAt(player.GetPosition() + vector2);
		}
		if (player.IsLocal())
		{
			InputController inputController = player.inputController;
			inputController.CameraRotation = Vector2.zero;
			ThirdPersonStandardCameraScript component = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
			component.Enable = false;
		}
		if (player.IsAnimationFinish())
		{
			player.GetTransform().position = vector;
			player.mVehicle.FinishGetOn();
			player.SetState(Player.VEHICLE_IDLE_STATE);
		}
		if (player.mVehicle.IsAnimationFinish())
		{
			player.mVehicle.PlayAnimation(AnimationString.Idle, WrapMode.Loop, 0.5f);
		}
	}

	public override void End(GameUnit gameUnit)
	{
		Player player = gameUnit as Player;
		if (player.IsLocal())
		{
			ThirdPersonStandardCameraScript component = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
			component.Enable = true;
		}
	}
}
