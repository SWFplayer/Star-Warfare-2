public class DailyQuestInfoResponse : Response
{
	protected DailyQuestData questData;

	public override void ReadData(byte[] data)
	{
		BytesBuffer buffer = new BytesBuffer(data);
		questData = new DailyQuestData();
		questData.ReadData(buffer);
	}

	public override void ProcessLogic()
	{
		DailyBonusManager.GetInstance().SetQuestInfo(questData);
	}
}
