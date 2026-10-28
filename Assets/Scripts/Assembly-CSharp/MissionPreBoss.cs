using System.Collections.Generic;
using UnityEngine;

public class MissionPreBoss : Mission
{
	private int mTargetAmount;

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
		if (mProcess == EMissionProcess.NORMAL && GameApp.GetInstance().GetGameWorld().State != GameState.SwitchBossLevel)
		{
			if (GetKilledCount() >= mTargetAmount)
			{
				LocalPlayer localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
				GlobalData.GetInstance().mPlayer = localPlayer;
				Object.DontDestroyOnLoad(localPlayer.GetObject());
				List<RemotePlayer> remotePlayers = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
				GlobalData.GetInstance().RemotePlayerList = remotePlayers;
				foreach (RemotePlayer item in remotePlayers)
				{
					if (item != null)
					{
						if (item.InPlayingState())
						{
							item.OnSwitchToBoss();
						}
						Object.DontDestroyOnLoad(item.GetObject());
					}
				}
				if (HUDBase.instance != null && HUDBase.instance.m_StateItem != null)
				{
					GlobalData.GetInstance().ItemArray = HUDBase.instance.m_StateItem.GetRemainedItemArray();
				}
				string bossLevelName;
				if (!GameApp.GetInstance().GetGameMode().IsSingle())
				{
					bossLevelName = ((GameApp.GetInstance().GetUserState().Boss1Type == EnemyType.NA) ? GameApp.GetInstance().GetUserState().GetLevelConfig(GameApp.GetInstance().GetUserState().GetSelectedNetIndex(), Global.BOSS_FINAL_SUB_STAGE_ID)
						.levelName : "Campaign01_boss_test");
				}
				else
				{
					GameApp.GetInstance().GetUserState().SetSelectedSubStageIndex((byte)(GameApp.GetInstance().GetUserState().GetSelectedSubStageIndex() + 1));
					bossLevelName = GameApp.GetInstance().GetUserState().GetLevelConfig(GameApp.GetInstance().GetUserState().GetSelectedStageIndex(), GameApp.GetInstance().GetUserState().GetSelectedSubStageIndex())
						.levelName;
				}
				GlobalData.GetInstance().BossLevelName = bossLevelName;
				GameApp.GetInstance().GetGameMode().ModePlay = Mode.BOSS_FINAL;
				GameApp.GetInstance().GetGameWorld().SetGameState(GameState.SwitchBossLevel);
				localPlayer.OnSwitchToBoss();
				GameApp.GetInstance().Save();
			}
			else if (GetKilledCount() > mActionCounter * mTargetAmount / 8)
			{
				TriggerAction();
			}
		}
		return mProcess;
	}

	public override float GetProgressPercentage()
	{
		return (float)GetKilledCount() / (float)mTargetAmount;
	}

	public override string GetProgressText()
	{
		return base.SummaryText + " [e4ce00](" + GetKilledCount() + " / " + mTargetAmount + ")[-]";
	}

	private int GetKilledCount()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld != null)
		{
			if (!GameApp.GetInstance().GetGameMode().IsSingle())
			{
				return gameWorld.TotalEnemyKilled;
			}
			Player localPlayer = gameWorld.GetLocalPlayer();
			if (localPlayer != null)
			{
				return localPlayer.Kills;
			}
		}
		return 0;
	}
}
