public class SetRoomPingRequest : Request
{
	private short mRoomId;

	private short mPing;

	public SetRoomPingRequest(short roomId, short ping)
	{
		requestID = 14;
		mRoomId = roomId;
		mPing = ping;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(4);
		bytesBuffer.AddShort(mRoomId);
		bytesBuffer.AddShort(mPing);
		return bytesBuffer;
	}
}
