internal class PlayerSpawnVehicleResponse : Response
{
	private int mChannelId;

	private byte mVehicleType;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		mChannelId = bytesBuffer.ReadInt();
		mVehicleType = bytesBuffer.ReadByte();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null || (gameWorld.State != GameState.Playing && GameApp.GetInstance().GetGameMode().IsVSMode()))
		{
			return;
		}
		Player localPlayer = gameWorld.GetLocalPlayer();
		if (localPlayer != null && mChannelId == localPlayer.GetChannelID())
		{
			if (localPlayer.mVehicleState == EPlayerVehicleState.NONE && localPlayer.CanSpawnVehicle())
			{
				gameWorld.SpawnVehicle(localPlayer, (EVehicleType)mVehicleType);
			}
			else if (localPlayer.mVehicle != null)
			{
				localPlayer.mVehicle.ResetDeadTime();
			}
		}
		else
		{
			RemotePlayer remotePlayerByUserID = GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(mChannelId);
			if (remotePlayerByUserID != null && remotePlayerByUserID.mVehicleState == EPlayerVehicleState.NONE && remotePlayerByUserID.CanSpawnVehicle())
			{
				gameWorld.SpawnVehicle(remotePlayerByUserID, (EVehicleType)mVehicleType);
			}
		}
	}
}
