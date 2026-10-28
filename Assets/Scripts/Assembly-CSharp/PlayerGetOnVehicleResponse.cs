using UnityEngine;

public class PlayerGetOnVehicleResponse : Response
{
	protected int playerId;

	protected byte vehicleType;

	protected byte longswordSyncType;

	protected int mainRiderId;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		playerId = bytesBuffer.ReadInt();
		vehicleType = bytesBuffer.ReadByte();
		if (vehicleType == 6)
		{
			longswordSyncType = bytesBuffer.ReadByte();
			mainRiderId = bytesBuffer.ReadInt();
		}
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null || (gameWorld.State != GameState.Playing && GameApp.GetInstance().GetGameMode().IsVSMode()))
		{
			return;
		}
		if (vehicleType == 6)
		{
			Player playerByUserID = gameWorld.GetPlayerByUserID(mainRiderId);
			if (playerByUserID == null)
			{
				return;
			}
			Player playerByUserID2 = gameWorld.GetPlayerByUserID(playerId);
			if (playerByUserID2 != null && playerByUserID.mVehicle != null && playerByUserID.mVehicle.VehicleType == EVehicleType.LONG_SWORD)
			{
				((VehicleLongSword)playerByUserID.mVehicle).SetSubRider(playerByUserID2);
				EVehicleType vType = (EVehicleType)vehicleType;
				VehicleLongSwordCannon vehicleLongSwordCannon = gameWorld.GenerateVirtualVehicle(vType, playerByUserID.mVehicle.GetObject()) as VehicleLongSwordCannon;
				vehicleLongSwordCannon.SetMainRider(playerByUserID, playerByUserID2);
				vehicleLongSwordCannon.GetOn();
				if (longswordSyncType == 1)
				{
					playerByUserID2.SetPosition(vehicleLongSwordCannon.GetPosition());
					playerByUserID2.GetTransform().LookAt(vehicleLongSwordCannon.GetForward());
					playerByUserID2.SetState(Player.VEHICLE_IDLE_STATE);
					playerByUserID2.mVehicleState = EPlayerVehicleState.CANNON;
					vehicleLongSwordCannon.FinishGetOn();
				}
			}
			return;
		}
		RemotePlayer remotePlayerByUserID = gameWorld.GetRemotePlayerByUserID(playerId);
		if (remotePlayerByUserID != null && !remotePlayerByUserID.IsInVehicleState())
		{
			EVehicleType eVehicleType = (EVehicleType)vehicleType;
			Vehicle vehicle = gameWorld.SpawnVehicle(eVehicleType);
			vehicle.SetRemoteRider(remotePlayerByUserID);
			vehicle.GetOn();
			remotePlayerByUserID.SetPosition(vehicle.GetPosition());
			remotePlayerByUserID.GetTransform().LookAt(vehicle.GetForward());
			remotePlayerByUserID.SetState(Player.VEHICLE_IDLE_STATE);
			switch (eVehicleType)
			{
			case EVehicleType.ATV_MINI:
			case EVehicleType.ATV_LASER:
			case EVehicleType.LONG_SWORD:
				remotePlayerByUserID.mVehicleState = EPlayerVehicleState.CARRIER;
				break;
			case EVehicleType.UNCC_TURRET:
			case EVehicleType.RPG_TURRET:
			case EVehicleType.LONG_SWORD_CANNON:
				remotePlayerByUserID.mVehicleState = EPlayerVehicleState.CANNON;
				break;
			default:
				Debug.LogError("Type is wrong: " + eVehicleType);
				break;
			}
			vehicle.FinishGetOn();
		}
	}
}
