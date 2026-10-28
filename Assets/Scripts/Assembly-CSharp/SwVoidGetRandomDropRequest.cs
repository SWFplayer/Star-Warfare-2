public class SwVoidGetRandomDropRequest : Request
{
	private int mSwVoidId;

	private EDifficultMode mMode;

	private byte mStageId;

	private byte mSubStageId;

	public SwVoidGetRandomDropRequest(int swVoidId, EDifficultMode mode, byte stageId, byte subStageId)
	{
		requestID = 164;
		mSwVoidId = swVoidId;
		mMode = mode;
		mStageId = stageId;
		mSubStageId = subStageId;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer();
		bytesBuffer.AddInt(mSwVoidId);
		bytesBuffer.AddByte((byte)mMode);
		bytesBuffer.AddByte(mStageId);
		bytesBuffer.AddByte(mSubStageId);
		return bytesBuffer;
	}
}
