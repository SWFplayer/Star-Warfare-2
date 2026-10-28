using System;
using UnityEngine;

public class MissionPickup : Mission
{
	private int mTargetAmount;

	private LootType mType;

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
				mType = LootType.NONE;
				if (Enum.IsDefined(typeof(LootType), text))
				{
					mType = (LootType)(int)Enum.Parse(typeof(LootType), text, true);
				}
				else
				{
					Debug.LogError("Invalid Pickup Type: " + text);
				}
				break;
			}
		}
	}

	public override void LevelStart()
	{
		base.LevelStart();
		mProcess = EMissionProcess.NORMAL;
	}

	public override EMissionProcess CheckIfComplete()
	{
		if (mProcess == EMissionProcess.NORMAL)
		{
			GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
			if (gameWorld != null)
			{
				Player localPlayer = gameWorld.GetLocalPlayer();
				if (localPlayer != null)
				{
					if (mType == LootType.MONEY)
					{
						if ((int)((float)localPlayer.GetPickupCash() * (1f + localPlayer.GetSkills().GetSkill(SkillsType.MONEY_BOOTH))) >= mTargetAmount)
						{
							mProcess = EMissionProcess.SUCCESS;
						}
					}
					else if (localPlayer.GetPickNum(mType) >= mTargetAmount)
					{
						mProcess = EMissionProcess.SUCCESS;
					}
				}
			}
		}
		return mProcess;
	}

	public override float GetProgressPercentage()
	{
		int num = 0;
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld != null)
		{
			Player localPlayer = gameWorld.GetLocalPlayer();
			if (localPlayer != null)
			{
				num = ((mType != LootType.MONEY) ? localPlayer.GetPickNum(mType) : ((int)((float)localPlayer.GetPickupCash() * (1f + localPlayer.GetSkills().GetSkill(SkillsType.MONEY_BOOTH)))));
			}
		}
		return (float)num / (float)mTargetAmount;
	}
}
