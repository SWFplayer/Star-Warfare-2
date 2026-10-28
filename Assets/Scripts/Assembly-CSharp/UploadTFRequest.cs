public class UploadTFRequest : Request
{
	private bool bTwitter;

	private bool bFacebook;

	public UploadTFRequest(bool twitter, bool facebook)
	{
		requestID = 26;
		bTwitter = twitter;
		bFacebook = facebook;
	}

	public override BytesBuffer GetBody()
	{
		byte length = 2;
		BytesBuffer bytesBuffer = new BytesBuffer(length);
		bytesBuffer.AddBool(bTwitter);
		bytesBuffer.AddBool(bFacebook);
		return bytesBuffer;
	}
}
