public class SlasherBladeWindRequest : Request
{
	private int mEnemyID;

	private byte mBladeWindNumber;

	public SlasherBladeWindRequest(int enemyID, byte bladeWindNumber)
	{
		requestID = 201;
		mEnemyID = enemyID;
		mBladeWindNumber = bladeWindNumber;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(5);
		bytesBuffer.AddInt(mEnemyID);
		bytesBuffer.AddByte(mBladeWindNumber);
		return bytesBuffer;
	}
}
