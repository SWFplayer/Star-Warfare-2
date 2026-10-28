using UnityEngine;

public class PlayerBossChangeTargetPositionRequest : Request
{
	protected int playerID;

	protected short target_x;

	protected short target_y;

	protected short target_z;

	protected short x;

	protected short y;

	protected short z;

	public PlayerBossChangeTargetPositionRequest(int playerID, Vector3 targetPos, Vector3 pos)
	{
		requestID = 250;
		this.playerID = playerID;
		target_x = (short)(targetPos.x * 10f);
		target_y = (short)(targetPos.y * 10f);
		target_z = (short)(targetPos.z * 10f);
		x = (short)(pos.x * 10f);
		y = (short)(pos.y * 10f);
		z = (short)(pos.z * 10f);
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(16);
		bytesBuffer.AddInt(playerID);
		bytesBuffer.AddShort(target_x);
		bytesBuffer.AddShort(target_y);
		bytesBuffer.AddShort(target_z);
		bytesBuffer.AddShort(x);
		bytesBuffer.AddShort(y);
		bytesBuffer.AddShort(z);
		return bytesBuffer;
	}
}
