public class SwVoidFirstBossStaticMaterialsRequest : Request
{
	public SwVoidFirstBossStaticMaterialsRequest()
	{
		requestID = 60;
	}

	public override BytesBuffer GetBody()
	{
		return new BytesBuffer(0);
	}
}
