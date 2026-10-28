using System;
using UnityEngine;

public class MissionVehicleKillEnemy : Mission
{
	private int mTargetAmount;

	private EVehicleType mType;

	public override void LoadConfig(int i)
	{
		base.LoadConfig(i);
		GameDataTable dataTable = GetDataTable();
		mTargetAmount = 0;
		for (int j = 0; j < dataTable.Count; j++)
		{
			int num = dataTable.GetInt(j, 0);
			if (num == base.ID)
			{
				mTargetAmount = dataTable.GetInt(j, 1);
				string text = dataTable.GetString(j, 2);
				if (Enum.IsDefined(typeof(EVehicleType), text))
				{
					mType = (EVehicleType)(int)Enum.Parse(typeof(EVehicleType), text, true);
				}
				else
				{
					Debug.LogError("Invalid Mission Weapon Type: " + text);
				}
				break;
			}
		}
	}

	public override void LevelStart()
	{
		base.LevelStart();
		mProcess = EMissionProcess.NORMAL;
		MissionManager.GetInstance().VehicleKillEnemyType = mType;
		MissionManager.GetInstance().VehicleKillEnemyAmount = 0;
	}

	public override EMissionProcess CheckIfComplete()
	{
		if (mProcess == EMissionProcess.NORMAL && MissionManager.GetInstance().VehicleKillEnemyAmount >= mTargetAmount)
		{
			mProcess = EMissionProcess.SUCCESS;
		}
		return mProcess;
	}

	public override float GetProgressPercentage()
	{
		return (float)MissionManager.GetInstance().VehicleKillEnemyAmount / (float)mTargetAmount;
	}
}
