using UnityEngine;

public class HookHitPlayerRequest : Request
{
	protected int playerId;

	protected Vector3 beginPos;

	public HookHitPlayerRequest(int playerId, Vector3 beginPos)
	{
		requestID = 315;
		this.playerId = playerId;
		this.beginPos = beginPos;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(10);
		bytesBuffer.AddInt(playerId);
		bytesBuffer.AddShort((short)(beginPos.x * 10f));
		bytesBuffer.AddShort((short)(beginPos.y * 10f));
		bytesBuffer.AddShort((short)(beginPos.z * 10f));
		Debug.Log(beginPos);
		return bytesBuffer;
	}
}
