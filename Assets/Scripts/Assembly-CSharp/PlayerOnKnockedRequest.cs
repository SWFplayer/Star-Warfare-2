public class PlayerOnKnockedRequest : Request
{
	public int KnockPlayerID;

	protected short speed;

	public PlayerOnKnockedRequest(float speed, int player)
	{
		requestID = 121;
		this.speed = (short)(speed * 100f);
		KnockPlayerID = player;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(6);
		bytesBuffer.AddShort(speed);
		bytesBuffer.AddInt(KnockPlayerID);
		return bytesBuffer;
	}
}
