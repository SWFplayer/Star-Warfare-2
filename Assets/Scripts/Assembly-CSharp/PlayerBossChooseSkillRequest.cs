public class PlayerBossChooseSkillRequest : Request
{
	protected byte[] bossSkillIds = new byte[4];

	public PlayerBossChooseSkillRequest(byte[] chooseBossSkillIds)
	{
		requestID = 246;
		for (int i = 0; i < chooseBossSkillIds.Length; i++)
		{
			bossSkillIds[i] = chooseBossSkillIds[i];
		}
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(5);
		bytesBuffer.AddByte(4);
		bytesBuffer.AddBytes(bossSkillIds);
		return bytesBuffer;
	}
}
