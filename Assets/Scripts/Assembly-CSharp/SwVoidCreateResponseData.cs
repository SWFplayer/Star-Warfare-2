public class SwVoidCreateResponseData : SwVoidBaseResponseData
{
	public int result;

	public SwVoidResponseData swVoid;

	public SwVoidMaterialResponseData[] materialArray;

	public override void ReadData(BytesBuffer bb)
	{
		result = bb.ReadInt();
		if (result == SwErrorCode.SUCCESS)
		{
			swVoid = new SwVoidResponseData();
			swVoid.ReadData(bb);
			byte b = bb.ReadByte();
			materialArray = new SwVoidMaterialResponseData[b];
			for (byte b2 = 0; b2 < b; b2++)
			{
				materialArray[b2] = default(SwVoidMaterialResponseData);
				materialArray[b2].type = bb.ReadShort();
				materialArray[b2].number = bb.ReadShort();
			}
		}
	}
}
