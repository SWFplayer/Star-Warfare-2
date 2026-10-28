public class LeaveRoomRequest : Request
{
	private byte mType;

	private int mChannelId;

	public LeaveRoomRequest()
	{
		requestID = 6;
		mType = 0;
	}

	public LeaveRoomRequest(int channelId)
	{
		requestID = 6;
		mChannelId = channelId;
		mType = 1;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = null;
		if (mType == 0)
		{
			bytesBuffer = new BytesBuffer(1);
			bytesBuffer.AddByte(mType);
		}
		else if (mType == 1)
		{
			bytesBuffer = new BytesBuffer(5);
			bytesBuffer.AddByte(mType);
			bytesBuffer.AddInt(mChannelId);
		}
		return bytesBuffer;
	}
}
