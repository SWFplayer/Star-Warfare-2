public class PlayerHpRecoveryRequest : Request
{
	protected byte type;

	protected short point;

	public PlayerHpRecoveryRequest(byte type, short point)
	{
		requestID = 118;
		this.type = type;
		this.point = point;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(3);
		bytesBuffer.AddByte(type);
		bytesBuffer.AddShort(point);
		return bytesBuffer;
	}
}
