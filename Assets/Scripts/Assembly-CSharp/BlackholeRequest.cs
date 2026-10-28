public class BlackholeRequest : Request
{
	private BlackHole blackhole;

	public BlackholeRequest(BlackHole b)
	{
		requestID = 170;
		blackhole = b;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(12);
		bytesBuffer.AddShort((short)(blackhole.x * 10f));
		bytesBuffer.AddShort((short)(blackhole.z * 10f));
		bytesBuffer.AddByte((byte)blackhole.radius);
		bytesBuffer.AddByte((byte)blackhole.speed);
		bytesBuffer.AddInt(blackhole.owner.GetChannelID());
		bytesBuffer.AddByte((byte)blackhole.BlackHoleDuringTime);
		bytesBuffer.AddByte((byte)(blackhole.isAttract ? 1u : 0u));
		return bytesBuffer;
	}
}
