public class SwVoidAddSlotRequest : Request
{
	protected byte nextSlotCount;

	public SwVoidAddSlotRequest(byte nextSlotCount)
	{
		requestID = 69;
		this.nextSlotCount = nextSlotCount;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(1);
		bytesBuffer.AddByte(nextSlotCount);
		return bytesBuffer;
	}
}
