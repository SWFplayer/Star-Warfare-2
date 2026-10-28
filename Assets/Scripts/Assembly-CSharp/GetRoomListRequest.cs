public class GetRoomListRequest : Request
{
	public byte roomType;

	public byte rankID;

	public GetRoomListRequest(byte roomType, byte rankID)
	{
		requestID = 7;
		this.roomType = roomType;
		this.rankID = rankID;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(2);
		bytesBuffer.AddByte(roomType);
		bytesBuffer.AddByte(rankID);
		return bytesBuffer;
	}
}
