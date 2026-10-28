public class PlayerRebirthRequest : Request
{
	protected byte spawnPointIndex;

	protected byte spawnLoadoutID;

	public PlayerRebirthRequest(byte spawnPointIndex, byte spawnLoadoutID)
	{
		requestID = 125;
		this.spawnPointIndex = spawnPointIndex;
		this.spawnLoadoutID = spawnLoadoutID;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(2);
		bytesBuffer.AddByte(spawnPointIndex);
		bytesBuffer.AddByte(spawnLoadoutID);
		return bytesBuffer;
	}
}
