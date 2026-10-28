public class SwVoidCheckServerResponseData : SwVoidBaseResponseData
{
	public int result;

	public override void ReadData(BytesBuffer bb)
	{
		result = bb.ReadInt();
	}
}
