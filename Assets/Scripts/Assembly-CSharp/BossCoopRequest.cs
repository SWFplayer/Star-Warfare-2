public class BossCoopRequest : Request
{
	public BytesBuffer Data;

	public BossCoopRequest(byte type, int size)
	{
		requestID = 204;
		Data = new BytesBuffer(size + 1);
		Data.AddByte(type);
	}

	public override BytesBuffer GetBody()
	{
		return Data;
	}
}
