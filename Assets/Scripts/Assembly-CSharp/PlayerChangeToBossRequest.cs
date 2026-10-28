public class PlayerChangeToBossRequest : Request
{
	protected int playerID;

	protected byte type;

	protected bool newSpawn;

	public PlayerChangeToBossRequest(int playerID, EnemyType type)
	{
		requestID = 251;
		this.playerID = playerID;
		this.type = (byte)type;
		newSpawn = true;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(6);
		bytesBuffer.AddInt(playerID);
		bytesBuffer.AddByte(type);
		bytesBuffer.AddBool(newSpawn);
		return bytesBuffer;
	}
}
