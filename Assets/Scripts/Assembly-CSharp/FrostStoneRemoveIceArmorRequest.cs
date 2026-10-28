public class FrostStoneRemoveIceArmorRequest : Request
{
	private int mEnemyId;

	public FrostStoneRemoveIceArmorRequest(int enemyId)
	{
		requestID = 203;
		mEnemyId = enemyId;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(4);
		bytesBuffer.AddInt(mEnemyId);
		return bytesBuffer;
	}
}
