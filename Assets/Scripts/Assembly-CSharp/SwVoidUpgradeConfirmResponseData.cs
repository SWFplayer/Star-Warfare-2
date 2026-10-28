public class SwVoidUpgradeConfirmResponseData : SwVoidBaseResponseData
{
	public int result;

	public SwVoidResponseData swVoid;

	public override void ReadData(BytesBuffer bb)
	{
		result = bb.ReadInt();
		if (result == SwErrorCode.SUCCESS)
		{
			swVoid = new SwVoidResponseData();
			swVoid.ReadData(bb);
		}
	}
}
