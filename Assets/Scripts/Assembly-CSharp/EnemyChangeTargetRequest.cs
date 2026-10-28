using UnityEngine;

public class EnemyChangeTargetRequest : Request
{
	protected short enemyID;

	protected int targetID;

	protected short x;

	protected short y;

	protected short z;

	public EnemyChangeTargetRequest(short enemyID, int targetID, Vector3 pos)
	{
		requestID = 105;
		this.enemyID = enemyID;
		this.targetID = targetID;
		x = (short)(pos.x * 10f);
		y = (short)(pos.y * 10f);
		z = (short)(pos.z * 10f);
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(12);
		bytesBuffer.AddShort(enemyID);
		bytesBuffer.AddInt(targetID);
		bytesBuffer.AddShort(x);
		bytesBuffer.AddShort(y);
		bytesBuffer.AddShort(z);
		return bytesBuffer;
	}
}
