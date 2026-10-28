public class PlayerGetOnVehicleRequest : Request
{
	protected byte vehicleType;

	protected int mainRiderId;

	public PlayerGetOnVehicleRequest(EVehicleType type)
	{
		requestID = 157;
		vehicleType = (byte)type;
	}

	public PlayerGetOnVehicleRequest(int MainRiderId)
	{
		requestID = 157;
		vehicleType = 6;
		mainRiderId = MainRiderId;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(1);
		if (vehicleType != 6)
		{
			bytesBuffer.AddByte(vehicleType);
		}
		else
		{
			bytesBuffer = new BytesBuffer(5);
			bytesBuffer.AddByte(vehicleType);
			bytesBuffer.AddInt(mainRiderId);
		}
		return bytesBuffer;
	}
}
