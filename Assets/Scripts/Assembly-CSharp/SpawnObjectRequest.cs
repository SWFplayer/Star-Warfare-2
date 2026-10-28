using UnityEngine;

public class SpawnObjectRequest : Request
{
	private byte type;

	private short x;

	private short y;

	private short z;

	private short skillId;

	private short eX;

	private short eY;

	private int targetId;

	public SpawnObjectRequest(byte type, Vector3 position, short skillId = 0)
	{
		requestID = 148;
		this.type = type;
		x = (short)(position.x * 10f);
		y = (short)(position.y * 10f);
		z = (short)(position.z * 10f);
		this.skillId = skillId;
	}

	public SpawnObjectRequest(byte type, Vector3 position, float eX, float eY, int targetId = 0)
	{
		requestID = 148;
		this.type = type;
		x = (short)(position.x * 10f);
		y = (short)(position.y * 10f);
		z = (short)(position.z * 10f);
		this.eX = (short)(eX * 10f);
		this.eY = (short)(eY * 10f);
		this.targetId = targetId;
	}

	public override BytesBuffer GetBody()
	{
		int num = 7;
		if (type == 4 || type == 9)
		{
			num += 2;
		}
		else if (type == 8 || type == 10)
		{
			num += 4;
		}
		if (type == 10)
		{
			num += 4;
		}
		BytesBuffer bytesBuffer = new BytesBuffer(num);
		bytesBuffer.AddByte(type);
		bytesBuffer.AddShort(x);
		bytesBuffer.AddShort(y);
		bytesBuffer.AddShort(z);
		if (type == 4 || type == 9)
		{
			bytesBuffer.AddShort(skillId);
		}
		else if (type == 8 || type == 10)
		{
			bytesBuffer.AddShort(eX);
			bytesBuffer.AddShort(eY);
		}
		if (type == 10)
		{
			bytesBuffer.AddInt(targetId);
		}
		return bytesBuffer;
	}
}
