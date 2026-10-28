public class SwVoidGetRandomDropResponseData : SwVoidBaseResponseData
{
	public int result;

	public short[] dropRoundArray;

	public short[] dropTypeArray;

	public short[] dropBossArray;

	public override void ReadData(BytesBuffer bb)
	{
		result = bb.ReadInt();
		if (result == SwErrorCode.SUCCESS)
		{
			byte b = bb.ReadByte();
			dropRoundArray = new short[b];
			for (byte b2 = 0; b2 < b; b2++)
			{
				dropRoundArray[b2] = bb.ReadShort();
			}
			dropTypeArray = new short[b];
			for (byte b3 = 0; b3 < b; b3++)
			{
				dropTypeArray[b3] = bb.ReadShort();
			}
			byte b4 = bb.ReadByte();
			dropBossArray = new short[b4];
			for (byte b5 = 0; b5 < b4; b5++)
			{
				dropBossArray[b5] = bb.ReadShort();
			}
		}
	}
}
