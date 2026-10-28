using UnityEngine;

public class FlagScript : MonoBehaviour
{
	private bool moveUp;

	public Vector3 rotationSpeed = new Vector3(0f, 45f, 0f);

	public bool enableUpandDown = true;

	protected float deltaTime;

	public float moveSpeed = 0.2f;

	public float HighPos = 1.2f;

	public float LowPos = 1f;

	public short sequenceID;

	protected Timer lastPickUpRequestTimer = new Timer();

	private void Start()
	{
		lastPickUpRequestTimer.SetTimer(3f, true);
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (deltaTime < 0.03f)
		{
			return;
		}
		base.transform.Rotate(rotationSpeed * deltaTime);
		deltaTime = 0f;
		if (!GameApp.GetInstance().GetGameMode().IsCatchTheFlagMode())
		{
			return;
		}
		Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		if (!localPlayer.InPlayingState())
		{
			return;
		}
		if (localPlayer.mVehicleState == EPlayerVehicleState.NONE)
		{
			if (base.GetComponent<Collider>().bounds.Intersects(localPlayer.GetCollider().bounds) && lastPickUpRequestTimer.Ready())
			{
				CatchTheFlagRequest request = new CatchTheFlagRequest();
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
					.GetSkills()
					.PlayerCaptureFlag();
				lastPickUpRequestTimer.Do();
			}
		}
		else if (localPlayer.mVehicleState == EPlayerVehicleState.CARRIER && base.GetComponent<Collider>().bounds.Intersects(localPlayer.mVehicle.GetCollider().bounds) && lastPickUpRequestTimer.Ready())
		{
			CatchTheFlagRequest request2 = new CatchTheFlagRequest();
			GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
			GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.GetSkills()
				.PlayerCaptureFlag();
			lastPickUpRequestTimer.Do();
		}
	}
}
