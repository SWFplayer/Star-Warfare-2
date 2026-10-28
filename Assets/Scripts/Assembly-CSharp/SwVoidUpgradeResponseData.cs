public class SwVoidUpgradeResponseData : SwVoidBaseResponseData
{
	public int result;

	public int swVoidId;

	public short[] newAttributeArray;

	public SwVoidMaterialResponseData[] materialArray;

	public override void ReadData(BytesBuffer bb)
	{
		result = bb.ReadInt();
		if (result == SwErrorCode.SUCCESS)
		{
			swVoidId = bb.ReadInt();
			byte b = bb.ReadByte();
			newAttributeArray = new short[b];
			for (byte b2 = 0; b2 < b; b2++)
			{
				newAttributeArray[b2] = bb.ReadShort();
			}
			byte b3 = bb.ReadByte();
			materialArray = new SwVoidMaterialResponseData[b3];
			for (byte b4 = 0; b4 < b3; b4++)
			{
				materialArray[b4] = default(SwVoidMaterialResponseData);
				materialArray[b4].type = bb.ReadShort();
				materialArray[b4].number = bb.ReadShort();
			}
		}
	}
}
