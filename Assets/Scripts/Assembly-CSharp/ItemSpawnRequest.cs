using UnityEngine;

public class ItemSpawnRequest : Request
{
	protected byte itemID;

	protected short x;

	protected short y;

	protected short z;

	protected short param1;

	protected short param2;

	public ItemSpawnRequest(byte itemID, Vector3 pos, short param1, short param2)
	{
		requestID = 119;
		this.itemID = itemID;
		x = (short)(pos.x * 10f);
		y = (short)(pos.y * 10f);
		z = (short)(pos.z * 10f);
		this.param1 = param1;
		this.param2 = param2;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(11);
		bytesBuffer.AddByte(itemID);
		bytesBuffer.AddShort(x);
		bytesBuffer.AddShort(y);
		bytesBuffer.AddShort(z);
		bytesBuffer.AddShort(param1);
		bytesBuffer.AddShort(param2);
		return bytesBuffer;
	}
}
