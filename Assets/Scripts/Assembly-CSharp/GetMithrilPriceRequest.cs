public class GetMithrilPriceRequest : Request
{
	public GetMithrilPriceRequest()
	{
		requestID = 27;
	}

	public override BytesBuffer GetBody()
	{
		byte length = 0;
		return new BytesBuffer(length);
	}
}
