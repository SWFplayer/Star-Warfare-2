public class PlayerBossChooseSkillResponse : Response
{
	protected byte[] bossSkillIdsOnline = new byte[4];

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		int num = bytesBuffer.ReadByte();
		for (int i = 0; i < num; i++)
		{
			bossSkillIdsOnline[i] = bytesBuffer.ReadByte();
		}
	}

	public override void ProcessLogic()
	{
		if (HUDBase.instance != null)
		{
			HUDBase.instance.ChangeBossControlHUD(bossSkillIdsOnline);
		}
	}
}
