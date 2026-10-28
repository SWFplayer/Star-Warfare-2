using UnityEngine;

public class SwVoidStateRequest : Request
{
	protected byte state;

	protected short x;

	protected short y;

	protected short z;

	protected int targetID;

	protected short enemyID;

	public SwVoidStateRequest(ESwVoidStateID state, short enemyID, int targetID, Vector3 pos)
	{
		requestID = 169;
		this.state = (byte)state;
		this.targetID = targetID;
		this.enemyID = enemyID;
		x = (short)(pos.x * 10f);
		y = (short)(pos.y * 10f);
		z = (short)(pos.z * 10f);
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = null;
		bytesBuffer = new BytesBuffer(13);
		bytesBuffer.AddByte(state);
		bytesBuffer.AddShort(x);
		bytesBuffer.AddShort(y);
		bytesBuffer.AddShort(z);
		bytesBuffer.AddInt(targetID);
		bytesBuffer.AddShort(enemyID);
		return bytesBuffer;
	}
}
