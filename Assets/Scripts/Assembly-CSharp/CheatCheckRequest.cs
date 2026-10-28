public class CheatCheckRequest : Request
{
	private CheatType cheatType;

	private float cheatValue;

	private byte cheatInfo;

	private byte gunId;

	public CheatCheckRequest(CheatType type, float value, byte info, byte gunId)
	{
		requestID = 300;
		cheatType = type;
		cheatValue = value;
		cheatInfo = info;
		this.gunId = gunId;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(7);
		bytesBuffer.AddByte((byte)cheatType);
		bytesBuffer.AddFloat(cheatValue);
		bytesBuffer.AddByte(cheatInfo);
		bytesBuffer.AddByte(gunId);
		return bytesBuffer;
	}
}
