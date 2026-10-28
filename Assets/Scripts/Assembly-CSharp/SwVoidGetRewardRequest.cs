public class SwVoidGetRewardRequest : Request
{
	private int mSwVoidId;

	private EDifficultMode mMode;

	private byte mStageId;

	private byte mSubStageId;

	private int mTotalKill;

	private bool mIsWin;

	public SwVoidGetRewardRequest(int swVoidId, EDifficultMode mode, int totalKill, byte stageId, byte subStageId, bool isWin)
	{
		requestID = 165;
		mSwVoidId = swVoidId;
		mMode = mode;
		mStageId = stageId;
		mSubStageId = subStageId;
		mTotalKill = totalKill;
		mIsWin = isWin;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(12);
		bytesBuffer.AddInt(mSwVoidId);
		bytesBuffer.AddByte((byte)mMode);
		bytesBuffer.AddByte(mStageId);
		bytesBuffer.AddByte(mSubStageId);
		bytesBuffer.AddInt(mTotalKill);
		bytesBuffer.AddBool(mIsWin);
		return bytesBuffer;
	}
}
