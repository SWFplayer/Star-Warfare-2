public class SendGameStatistics : Response
{
	private int maxDamagePlayerId;

	private int maxBeHitDamagePlayerId;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		maxDamagePlayerId = bytesBuffer.ReadInt();
		maxBeHitDamagePlayerId = bytesBuffer.ReadInt();
	}

	public override void ProcessLogic()
	{
	}
}
