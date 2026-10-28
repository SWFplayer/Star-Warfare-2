using UnityEngine;

public class VehicleGetOffResponse : Response
{
	protected int vehicleOwerId;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		vehicleOwerId = bytesBuffer.ReadInt();
		Debug.Log("Send VGO Response");
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null || (gameWorld.State != GameState.Playing && GameApp.GetInstance().GetGameMode().IsVSMode()))
		{
			return;
		}
		Player playerByUserID = gameWorld.GetPlayerByUserID(vehicleOwerId);
		if (playerByUserID == gameWorld.GetLocalPlayer())
		{
			if (playerByUserID.mVehicle != null && playerByUserID.mVehicle.VehicleType == EVehicleType.LONG_SWORD_CANNON)
			{
				((VehicleLongSwordCannon)playerByUserID.mVehicle).ForceGetOff();
			}
		}
		else if (playerByUserID != null && playerByUserID.mVehicle != null)
		{
			playerByUserID.mVehicle.GetOff();
		}
	}
}
