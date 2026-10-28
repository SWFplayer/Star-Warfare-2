using UnityEngine;

public class InfiniteTurrentScript : MonoBehaviour
{
	public float Radius;

	public EVehicleType Type = EVehicleType.NA;

	public float VehicleCD;

	public GameObject effectObject;

	private Player mPlayer;

	private float playerGetOffTime = float.MinValue;

	private bool isOnVehicle;

	private void Start()
	{
		base.transform.Find("M1_Target").localScale = Vector3.one * Radius / 5f;
	}

	private void Update()
	{
		if (GameApp.GetInstance().GetGameWorld() != null)
		{
			mPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			bool flag = Time.time - playerGetOffTime >= VehicleCD;
			if (flag && !effectObject.activeSelf)
			{
				effectObject.SetActive(true);
			}
			else if (!flag && effectObject.activeSelf)
			{
				effectObject.SetActive(false);
			}
			if (Vector3.Distance(mPlayer.GetPosition(), base.transform.position) < Radius && !mPlayer.IsInVehicleState() && flag && !isOnVehicle)
			{
				isOnVehicle = true;
				GameApp.GetInstance().GetGameWorld().SpawnVehicle(mPlayer, Type);
			}
			if (isOnVehicle && mPlayer.mVehicle == null)
			{
				isOnVehicle = false;
				playerGetOffTime = Time.time;
			}
		}
	}
}
