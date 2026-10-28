public class SwVoidGetDataResponseData : SwVoidBaseResponseData
{
	public int result;

	public SwVoidResponseData[] swVoidArray;

	public SwVoidMaterialResponseData[] materialArray;

	public byte swVoidCountLimit;

	public override void ReadData(BytesBuffer bb)
	{
		result = bb.ReadInt();
		if (result == SwErrorCode.SUCCESS)
		{
			short num = bb.ReadShort();
			swVoidArray = new SwVoidResponseData[num];
			for (short num2 = 0; num2 < num; num2++)
			{
				swVoidArray[num2] = new SwVoidResponseData();
				swVoidArray[num2].ReadData(bb);
			}
			short num3 = bb.ReadShort();
			materialArray = new SwVoidMaterialResponseData[num3];
			for (short num4 = 0; num4 < num3; num4++)
			{
				materialArray[num4] = default(SwVoidMaterialResponseData);
				materialArray[num4].type = bb.ReadShort();
				materialArray[num4].number = bb.ReadShort();
			}
			swVoidCountLimit = bb.ReadByte();
		}
	}
}
