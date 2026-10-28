public class ChangeExpRateResponse : Response
{
	protected float expRate;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		expRate = bytesBuffer.ReadFloat();
	}

	public override void ProcessLogic()
	{
		PlayerVSStatistics.EXP_RATE = expRate;
	}
}
