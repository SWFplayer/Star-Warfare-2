internal class PlayerHitVehicleResponse : Response
{
	private float reducedTime;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		reducedTime = (float)bytesBuffer.ReadShort() / 100f;
	}

	public override void ProcessLogic()
	{
		if (GameApp.GetInstance().GetGameWorld() != null)
		{
			Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			if (localPlayer.mVehicle != null)
			{
				localPlayer.mVehicle.ReduceDuringTime(reducedTime);
			}
		}
	}
}
