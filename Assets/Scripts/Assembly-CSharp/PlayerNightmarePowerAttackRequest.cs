public class PlayerNightmarePowerAttackRequest : Request
{
	protected byte type;

	protected bool isLeft;

	protected int beginID;

	protected int targetID;

	public PlayerNightmarePowerAttackRequest(byte type, bool isLeft, int beginID, int targetID)
	{
		requestID = 311;
		this.type = type;
		this.isLeft = isLeft;
		this.beginID = beginID;
		this.targetID = targetID;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = null;
		bytesBuffer = new BytesBuffer(10);
		bytesBuffer.AddByte(type);
		bytesBuffer.AddBool(isLeft);
		bytesBuffer.AddInt(beginID);
		bytesBuffer.AddInt(targetID);
		return bytesBuffer;
	}
}
