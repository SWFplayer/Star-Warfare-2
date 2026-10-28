public class SwVoidResponseData
{
	public int id;

	public byte type;

	public byte level;

	public short[] skillIdArray;

	public SwVoidAttribute[] attributeArray;

	public void ReadData(BytesBuffer bb)
	{
		id = bb.ReadInt();
		type = bb.ReadByte();
		level = bb.ReadByte();
		byte b = bb.ReadByte();
		skillIdArray = new short[b];
		for (byte b2 = 0; b2 < b; b2++)
		{
			skillIdArray[b2] = bb.ReadShort();
		}
		byte b3 = bb.ReadByte();
		attributeArray = new SwVoidAttribute[b3];
		for (byte b4 = 0; b4 < b3; b4++)
		{
			attributeArray[b4] = new SwVoidAttribute();
			attributeArray[b4].id = bb.ReadByte();
			attributeArray[b4].value = bb.ReadShort();
		}
	}
}
