public class KillStreakUsedRequest : Request
{
	public int mKillStreakId;

	public bool isPresent;

	public KillStreakUsedRequest(int killStreakId, bool isPresnet = false)
	{
		requestID = 145;
		isPresent = isPresnet;
		mKillStreakId = killStreakId;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(5);
		bytesBuffer.AddInt(mKillStreakId);
		bytesBuffer.AddByte((byte)(isPresent ? 1u : 0u));
		return bytesBuffer;
	}
}
