using System.Collections.Generic;

public class DailyRewardInfoResponse : Response
{
	protected byte days;

	protected byte hasRecievedReward;

	protected byte loginRewardSize;

	protected List<SwDailyReward> loginRewardList;

	protected byte dailyQuestSize;

	protected List<DailyQuestData> dailyQuestDataList;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		days = bytesBuffer.ReadByte();
		hasRecievedReward = bytesBuffer.ReadByte();
		loginRewardSize = bytesBuffer.ReadByte();
		loginRewardList = new List<SwDailyReward>();
		for (int i = 0; i < loginRewardSize; i++)
		{
			SwDailyReward swDailyReward = new SwDailyReward();
			swDailyReward.ReadData(bytesBuffer);
			loginRewardList.Add(swDailyReward);
		}
		dailyQuestSize = bytesBuffer.ReadByte();
		dailyQuestDataList = new List<DailyQuestData>();
		for (int j = 0; j < dailyQuestSize; j++)
		{
			DailyQuestData dailyQuestData = new DailyQuestData();
			dailyQuestData.ReadData(bytesBuffer);
			dailyQuestDataList.Add(dailyQuestData);
		}
	}

	public override void ProcessLogic()
	{
		DailyBonusManager.GetInstance().SetLoginDays(days);
		DailyBonusManager.GetInstance().SetHasRecievedReward(hasRecievedReward);
		DailyBonusManager.GetInstance().SetLoginRewardList(loginRewardList);
		DailyBonusManager.GetInstance().SetQuestList(dailyQuestDataList);
		if (LobbyScript.mInstance != null)
		{
			if (hasRecievedReward == 1)
			{
				LobbyScript.mInstance.DailyBonusAlpha.enabled = false;
			}
			else
			{
				LobbyScript.mInstance.DailyBonusAlpha.enabled = true;
			}
		}
	}
}
