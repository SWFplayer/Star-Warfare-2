public class FunnelRequest : Request
{
	public BytesBuffer Data = new BytesBuffer();

	public FunnelRequest(byte id, EFunnelActionType type)
	{
		requestID = 205;
		Data.AddByte(id);
		Data.AddByte((byte)type);
	}

	public override BytesBuffer GetBody()
	{
		return Data;
	}
}
