using UnityEngine;

public class SwVoidSpawnRequest : Request
{
	protected byte swVoidType;

	protected byte swVoidLevel;

	protected byte[] swVoidAttributeId;

	protected short[] swVoidAttributeValue;

	protected short[] swVoidPassiveSkillId;

	public SwVoidSpawnRequest(SwVoidSavedata swVoidToSpawn)
	{
		requestID = 160;
		Debug.Log("No this request Anymore!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
		swVoidType = (byte)swVoidToSpawn.GetType();
		swVoidLevel = swVoidToSpawn.GetLevel();
		swVoidAttributeId = new byte[2]
		{
			swVoidToSpawn.GetAttributeId(0),
			swVoidToSpawn.GetAttributeId(1)
		};
		swVoidAttributeValue = new short[2]
		{
			swVoidToSpawn.GetAttributeValue(0),
			swVoidToSpawn.GetAttributeValue(1)
		};
		swVoidPassiveSkillId = new short[2]
		{
			(byte)swVoidToSpawn.GetPassiveSkillId(0),
			(byte)swVoidToSpawn.GetPassiveSkillId(1)
		};
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(12);
		bytesBuffer.AddByte(swVoidType);
		bytesBuffer.AddByte(swVoidLevel);
		bytesBuffer.AddBytes(swVoidAttributeId);
		for (int i = 0; i < 2; i++)
		{
			bytesBuffer.AddShort(swVoidAttributeValue[i]);
		}
		for (int j = 0; j < 2; j++)
		{
			bytesBuffer.AddShort(swVoidPassiveSkillId[j]);
		}
		return bytesBuffer;
	}
}
