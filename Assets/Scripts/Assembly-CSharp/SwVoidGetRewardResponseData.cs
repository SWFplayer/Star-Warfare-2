public class SwVoidGetRewardResponseData : SwVoidBaseResponseData
{
	public int result;

	public short[] materialTypeArray;

	public override void ReadData(BytesBuffer bb)
	{
		result = bb.ReadInt();
		if (result == SwErrorCode.SUCCESS)
		{
			short num = bb.ReadShort();
			materialTypeArray = new short[num];
			for (short num2 = 0; num2 < num; num2++)
			{
				materialTypeArray[num2] = bb.ReadShort();
			}
		}
	}
}
