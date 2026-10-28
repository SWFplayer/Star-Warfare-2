public class ChangeToBossSceneRequest : Request
{
	public ChangeToBossSceneRequest()
	{
		requestID = 158;
	}

	public override BytesBuffer GetBody()
	{
		return new BytesBuffer(0);
	}
}
