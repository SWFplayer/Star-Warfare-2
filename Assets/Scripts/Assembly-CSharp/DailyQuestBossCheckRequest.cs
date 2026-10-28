public class DailyQuestBossCheckRequest : Request
{
	protected byte index;

	protected byte bossId;

	protected byte isHeroic;

	public DailyQuestBossCheckRequest(byte index, byte bossId, byte isHeroic)
	{
		requestID = 210;
		this.index = index;
		this.bossId = bossId;
		this.isHeroic = isHeroic;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(3);
		bytesBuffer.AddByte(index);
		bytesBuffer.AddByte(bossId);
		bytesBuffer.AddByte(isHeroic);
		return bytesBuffer;
	}
}
