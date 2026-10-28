public class TryGetQuestGoldResponse : Response
{
	protected int result;

	protected int rewardGold;

	protected DailyQuestData questData;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		result = bytesBuffer.ReadInt();
		if (result == SwErrorCode.SUCCESS)
		{
			rewardGold = bytesBuffer.ReadInt();
			questData = new DailyQuestData();
			questData.ReadData(bytesBuffer);
		}
	}

	public override void ProcessLogic()
	{
		if (result == SwErrorCode.SUCCESS)
		{
			DailyBonusManager.GetInstance().SetQuestInfo(questData);
			DailyBonusManager.GetInstance().ReceivedReward(1, 0, rewardGold);
			if (UIDailyBonus.mInstance != null)
			{
				UIDailyBonus.mInstance.RefreshQuestInfo(questData.index);
			}
		}
		else
		{
			UIMsgBox.instance.ShowErrorCodeMessage(result);
		}
	}
}
