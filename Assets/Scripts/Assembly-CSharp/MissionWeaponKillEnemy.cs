using System;
using UnityEngine;

public class MissionWeaponKillEnemy : Mission
{
	private int mTargetAmount;

	private MissionWeaponType mType;

	private int GunID;

	public override void LoadConfig(int i)
	{
		base.LoadConfig(i);
		GameDataTable dataTable = GetDataTable();
		mTargetAmount = 0;
		for (int j = 0; j < dataTable.Count; j++)
		{
			int num = dataTable.GetInt(j, 0);
			if (num != base.ID)
			{
				continue;
			}
			mTargetAmount = dataTable.GetInt(j, 1);
			string text = dataTable.GetString(j, 2);
			mType = MissionWeaponType.NO_GUN;
			if (Enum.IsDefined(typeof(MissionWeaponType), text))
			{
				mType = (MissionWeaponType)(int)Enum.Parse(typeof(MissionWeaponType), text, true);
				if (mType == MissionWeaponType.SPECIAL_GUN)
				{
					GunID = dataTable.GetInt(j, 3);
				}
			}
			else
			{
				Debug.LogError("Invalid Mission Weapon Type: " + text);
			}
			break;
		}
	}

	public override void LevelStart()
	{
		base.LevelStart();
		mProcess = EMissionProcess.NORMAL;
		MissionManager.GetInstance().WeaponKillEnemyType = mType;
		MissionManager.GetInstance().WeaponKillEnemyID = GunID;
		MissionManager.GetInstance().WeaponKillEnemyAmount = 0;
	}

	public override EMissionProcess CheckIfComplete()
	{
		if (mProcess == EMissionProcess.NORMAL && MissionManager.GetInstance().WeaponKillEnemyAmount >= mTargetAmount)
		{
			mProcess = EMissionProcess.SUCCESS;
		}
		return mProcess;
	}

	public override float GetProgressPercentage()
	{
		return (float)MissionManager.GetInstance().WeaponKillEnemyAmount / (float)mTargetAmount;
	}

	public MissionWeaponType GetMissionWeaponType()
	{
		return mType;
	}

	public int GetSpecialGunID()
	{
		return GunID;
	}
}
