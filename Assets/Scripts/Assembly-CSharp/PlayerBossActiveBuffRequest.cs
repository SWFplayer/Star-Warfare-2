public class PlayerBossActiveBuffRequest : Request
{
	private short skillId;

	public PlayerBossActiveBuffRequest(short skillId)
	{
		requestID = 257;
		this.skillId = skillId;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(2);
		bytesBuffer.AddShort(skillId);
		return bytesBuffer;
	}
}
