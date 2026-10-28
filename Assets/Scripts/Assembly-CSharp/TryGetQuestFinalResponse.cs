using System.Collections.Generic;

public class TryGetQuestFinalResponse : Response
{
	protected int result;

	protected byte size;

	protected List<SwDailyReward> rewardList;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		result = bytesBuffer.ReadInt();
		if (result == SwErrorCode.SUCCESS)
		{
			size = bytesBuffer.ReadByte();
			rewardList = new List<SwDailyReward>();
			for (int i = 0; i < size; i++)
			{
				SwDailyReward swDailyReward = new SwDailyReward();
				swDailyReward.ReadData(bytesBuffer);
				rewardList.Add(swDailyReward);
			}
		}
	}

	public override void ProcessLogic()
	{
		if (result == SwErrorCode.SUCCESS)
		{
			DailyBonusManager.GetInstance().RecievedQuestFinalRewards(rewardList);
		}
		else
		{
			UIMsgBox.instance.ShowErrorCodeMessage(result);
		}
	}
}
