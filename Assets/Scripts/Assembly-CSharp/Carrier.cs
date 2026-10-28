using UnityEngine;

public class Carrier : Vehicle
{
	public float maxForwardSpeed = 10f;

	public float maxBackwardSpeed = 3f;

	public float maxSidewardSpeed = 10f;

	public Vector3 dirSpeed;

	private Vector3 lastMotionDir = Vector3.zero;

	public override void GetOn()
	{
		base.GetOn();
		rider.mVehicleState = EPlayerVehicleState.CARRIER;
		if (rider.IsLocal())
		{
			GameApp.GetInstance().GetGameWorld().GetCamera()
				.ChangeTarget(GetTransform());
		}
	}

	public override void Move(Vector3 motion)
	{
		CalculateSpeed(motion);
		base.Move(dirSpeed * Speed);
	}

	private void CalculateSpeed(Vector3 motion)
	{
		float num = ((!(motion.magnitude <= dirSpeed.magnitude)) ? (speedUpdate * Time.deltaTime) : (speedDownUpdate * Time.deltaTime));
		dirSpeed = dirSpeed * (1f - num) + motion * num;
	}
}
