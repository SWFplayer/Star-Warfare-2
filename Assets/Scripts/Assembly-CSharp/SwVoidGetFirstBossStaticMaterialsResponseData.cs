public class SwVoidGetFirstBossStaticMaterialsResponseData : SwVoidBaseResponseData
{
	public int result;

	public override void ReadData(BytesBuffer bb)
	{
		result = bb.ReadInt();
	}
}
