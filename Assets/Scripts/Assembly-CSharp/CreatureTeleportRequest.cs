using UnityEngine;

public class CreatureTeleportRequest : Request
{
	private short mEnemyID;

	private short x;

	private short y;

	private short z;

	public CreatureTeleportRequest(short enemyID, Vector3 position)
	{
		requestID = 202;
		mEnemyID = enemyID;
		x = (short)(position.x * 10f);
		y = (short)(position.y * 10f);
		z = (short)(position.z * 10f);
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(8);
		bytesBuffer.AddShort(mEnemyID);
		bytesBuffer.AddShort(x);
		bytesBuffer.AddShort(y);
		bytesBuffer.AddShort(z);
		return bytesBuffer;
	}
}
