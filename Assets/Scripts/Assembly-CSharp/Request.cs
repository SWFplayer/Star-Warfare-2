public abstract class Request
{
	protected short requestID;

	public abstract BytesBuffer GetBody();

	protected BytesBuffer CreateBytesBuffer()
	{
		return new BytesBuffer();
	}

	protected BytesBuffer CreateBytesBuffer(int length)
	{
		return new BytesBuffer(length);
	}

	public byte[] GetBytes()
	{
		BytesBuffer body = GetBody();
		return body.GetRequestBytes(requestID);
	}

	public short GetRequestId()
	{
		return requestID;
	}
}
