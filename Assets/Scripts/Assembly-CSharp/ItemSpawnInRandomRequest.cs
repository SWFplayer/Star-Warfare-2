public class ItemSpawnInRandomRequest : Request
{
	protected byte pointID;

	protected byte pickupID;

	public ItemSpawnInRandomRequest(byte pointID, byte pickupID)
	{
		requestID = 150;
		this.pointID = pointID;
		this.pickupID = pickupID;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(2);
		bytesBuffer.AddByte(pointID);
		bytesBuffer.AddByte(pickupID);
		return bytesBuffer;
	}
}
