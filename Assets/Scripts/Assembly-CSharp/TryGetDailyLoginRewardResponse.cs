using System.Collections.Generic;
using UnityEngine;

public class TryGetDailyLoginRewardResponse : Response
{
	protected int result;

	protected byte days;

	protected byte hasRecievedReward;

	protected byte loginRewardSize;

	protected List<SwDailyReward> loginRewardList;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		result = bytesBuffer.ReadInt();
		if (result == SwErrorCode.SUCCESS)
		{
			days = bytesBuffer.ReadByte();
			hasRecievedReward = bytesBuffer.ReadByte();
			loginRewardSize = bytesBuffer.ReadByte();
			loginRewardList = new List<SwDailyReward>();
			for (int i = 0; i < loginRewardSize; i++)
			{
				SwDailyReward swDailyReward = new SwDailyReward();
				swDailyReward.ReadData(bytesBuffer);
				loginRewardList.Add(swDailyReward);
				Debug.Log(swDailyReward.GetRewardType() + " " + swDailyReward.GetRewardId());
			}
		}
	}

	public override void ProcessLogic()
	{
		if (result == SwErrorCode.SUCCESS)
		{
			DailyBonusManager.GetInstance().ReceivedLoginReward(loginRewardList);
		}
		else
		{
			UIMsgBox.instance.ShowErrorCodeMessage(result);
		}
	}
}
