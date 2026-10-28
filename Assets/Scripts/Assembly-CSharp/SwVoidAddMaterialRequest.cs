public class SwVoidAddMaterialRequest : Request
{
	public SwVoidAddMaterialRequest()
	{
		requestID = 70;
	}

	public override BytesBuffer GetBody()
	{
		return new BytesBuffer(0);
	}
}
