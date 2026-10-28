public class SwVoidAddSlotResponseData : SwVoidBaseResponseData
{
	public int result;

	public byte newSlotCount;

	public override void ReadData(BytesBuffer bb)
	{
		result = bb.ReadInt();
		if (result == SwErrorCode.SUCCESS)
		{
			newSlotCount = bb.ReadByte();
		}
	}
}
