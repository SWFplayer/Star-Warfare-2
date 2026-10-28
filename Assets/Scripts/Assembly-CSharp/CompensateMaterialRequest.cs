public class CompensateMaterialRequest : Request
{
	public CompensateMaterialRequest()
	{
		requestID = 77;
	}

	public override BytesBuffer GetBody()
	{
		return new BytesBuffer(0);
	}
}
