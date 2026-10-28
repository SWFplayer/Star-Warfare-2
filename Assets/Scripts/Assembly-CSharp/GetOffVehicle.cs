using UnityEngine;

public class GetOffVehicle : MonoBehaviour
{
	private LocalPlayer player;

	private void Start()
	{
		player = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
	}

	private void Update()
	{
	}

	private void OnClick()
	{
		if (player.mVehicle != null)
		{
			player.mVehicle.GetOff();
		}
		else if (player.GetNeighborLongSwordRider() != null)
		{
			PlayerGetOnVehicleRequest request = new PlayerGetOnVehicleRequest(player.GetNeighborLongSwordRider().GetChannelID());
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
	}
}
