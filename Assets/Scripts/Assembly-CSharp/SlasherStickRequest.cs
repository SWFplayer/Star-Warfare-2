public class SlasherStickRequest : Request
{
	private short mEnemyID;

	private byte mSickID;

	public SlasherStickRequest(short enemyID, byte stickID)
	{
		requestID = 200;
		mEnemyID = enemyID;
		mSickID = stickID;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(3);
		bytesBuffer.AddShort(mEnemyID);
		bytesBuffer.AddByte(mSickID);
		return bytesBuffer;
	}
}
