public class RoomTimeSynchronizeRequest : Request
{
	private byte mId;

	public RoomTimeSynchronizeRequest(byte id)
	{
		requestID = 13;
		mId = id;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(1);
		bytesBuffer.AddByte(mId);
		return bytesBuffer;
	}
}
