public class SwVoidCreateRequest : Request
{
	private ESwVoidType mSwVoidType;

	public SwVoidCreateRequest(ESwVoidType type)
	{
		requestID = 62;
		mSwVoidType = type;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(1);
		bytesBuffer.AddByte((byte)mSwVoidType);
		return bytesBuffer;
	}
}
