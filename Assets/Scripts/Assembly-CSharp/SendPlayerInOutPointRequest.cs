public class SendPlayerInOutPointRequest : Request
{
	protected bool isIn;

	protected byte pointId;

	public SendPlayerInOutPointRequest(bool isIn, byte pointId)
	{
		requestID = 143;
		this.isIn = isIn;
		this.pointId = pointId;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = null;
		bytesBuffer = new BytesBuffer(2);
		bytesBuffer.AddBool(isIn);
		bytesBuffer.AddByte(pointId);
		return bytesBuffer;
	}
}
