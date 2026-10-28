public class PlayerHealOthersRequest : Request
{
	private int otherID;

	private byte dpsFactor;

	public PlayerHealOthersRequest(int OtherID, byte DPSFactor)
	{
		requestID = 126;
		otherID = OtherID;
		dpsFactor = DPSFactor;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(5);
		bytesBuffer.AddInt(otherID);
		bytesBuffer.AddByte(dpsFactor);
		return bytesBuffer;
	}
}
