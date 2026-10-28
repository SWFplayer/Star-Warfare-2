using UnityEngine;

public class MissionSurvival : Mission
{
	private int mSurvivalTime;

	private float mStartTime;

	public override void LoadConfig(int i)
	{
		base.LoadConfig(i);
		GameDataTable dataTable = GetDataTable();
		for (int j = 0; j < dataTable.Count; j++)
		{
			int num = dataTable.GetInt(j, 0);
			if (num == base.ID)
			{
				mSurvivalTime = dataTable.GetInt(j, 1);
				base.TimeLimitation = mSurvivalTime;
				break;
			}
		}
	}

	public override void LevelStart()
	{
		base.LevelStart();
		mProcess = EMissionProcess.NORMAL;
		mStartTime = Time.time;
	}

	public override EMissionProcess CheckIfComplete()
	{
		if (mProcess == EMissionProcess.NORMAL)
		{
			if (Time.time - mStartTime > (float)mSurvivalTime)
			{
				mProcess = EMissionProcess.SUCCESS;
			}
			else if (Time.time - mStartTime > (float)(mActionCounter * mSurvivalTime / 8))
			{
				TriggerAction();
			}
		}
		return mProcess;
	}

	public override float GetProgressPercentage()
	{
		return (Time.time - mStartTime) / (float)mSurvivalTime;
	}
}
