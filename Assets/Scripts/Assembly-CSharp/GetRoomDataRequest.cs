public class GetRoomDataRequest : Request
{
	protected short roomID;

	public GetRoomDataRequest(short roomID)
	{
		requestID = 8;
		this.roomID = roomID;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(2);
		bytesBuffer.AddShort(roomID);
		return bytesBuffer;
	}
}
