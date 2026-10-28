using System;
using UnityEngine;

public class MissionKillSpecialEnemy : Mission
{
	public EnemyType mType;

	public bool isElite;

	public int mSuccessNum;

	public int mCurrentNum;

	private float mStartTime;

	public int mIntervalNum;

	public override void LoadConfig(int i)
	{
		base.LoadConfig(i);
		GameDataTable dataTable = GetDataTable();
		for (int j = 0; j < dataTable.Count; j++)
		{
			int num = dataTable.GetInt(j, 0);
			if (num == base.ID)
			{
				mType = (EnemyType)(int)Enum.Parse(typeof(EnemyType), dataTable.GetString(j, 1));
				isElite = dataTable.GetBoolean(j, 2);
				mSuccessNum = dataTable.GetInt(j, 3);
				mIntervalNum = dataTable.GetInt(j, 4);
				break;
			}
		}
	}

	public override void LevelStart()
	{
		base.LevelStart();
		mCurrentNum = 0;
		mProcess = EMissionProcess.NORMAL;
		mStartTime = Time.time;
		MissionManager.GetInstance().SpecialEnemyKills = 0;
		MissionManager.GetInstance().SpecialEnemyType = mType;
		MissionManager.GetInstance().IsSpecialEnemyElite = isElite;
	}

	public override EMissionProcess CheckIfComplete()
	{
		if (mProcess == EMissionProcess.NORMAL)
		{
			mCurrentNum = MissionManager.GetInstance().SpecialEnemyKills;
			if (mCurrentNum >= mSuccessNum)
			{
				mProcess = EMissionProcess.SUCCESS;
			}
			else if (Time.time - mStartTime > (float)(mActionCounter * base.TimeLimitation / mIntervalNum))
			{
				TriggerAction();
			}
		}
		return mProcess;
	}

	public override float GetProgressPercentage()
	{
		return (float)mCurrentNum / (float)mSuccessNum;
	}
}
