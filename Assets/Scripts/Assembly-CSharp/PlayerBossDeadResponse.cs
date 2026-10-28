using System.Collections.Generic;
using UnityEngine;

public class PlayerBossDeadResponse : Response
{
	protected int BossKiller;

	protected bool isPlayerBossWin;

	protected bool isPlayerBossLeave;

	protected byte playerCount;

	protected List<int> lastRank;

	protected List<int> lastRoundScores;

	protected List<int> totleScores;

	protected List<int> lastDamage;

	protected List<int> totleDamage;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		BossKiller = bytesBuffer.ReadInt();
		isPlayerBossWin = bytesBuffer.ReadBool();
		isPlayerBossLeave = bytesBuffer.ReadBool();
		playerCount = bytesBuffer.ReadByte();
		lastRank = new List<int>();
		lastRoundScores = new List<int>();
		totleScores = new List<int>();
		lastDamage = new List<int>();
		totleDamage = new List<int>();
		for (int i = 0; i < playerCount; i++)
		{
			lastRank.Add(bytesBuffer.ReadInt());
			lastRoundScores.Add(bytesBuffer.ReadInt());
			totleScores.Add(bytesBuffer.ReadInt());
			lastDamage.Add(bytesBuffer.ReadInt());
			totleDamage.Add(bytesBuffer.ReadInt());
			Debug.Log("mutant info : " + lastRank[i] + "; " + lastRoundScores[i] + "; " + totleScores[i] + "; " + lastDamage[i] + "; " + totleDamage[i] + ";");
		}
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null)
		{
			return;
		}
		Player localPlayer = gameWorld.GetLocalPlayer();
		if (localPlayer == null)
		{
			return;
		}
		int channelID = Lobby.GetInstance().GetChannelID();
		UserStateHUD.InfoBoxHUD infoBox = UserStateHUD.GetInstance().InfoBox;
		if (channelID == BossKiller)
		{
			localPlayer.VSStatistics.MakeExecuted();
			localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.TO_PLAYER_BOSS_EXECUTED);
		}
		else if (BossKiller == 0 && isPlayerBossLeave)
		{
			infoBox.SetMutantInfo(LocalizationManager.GetInstance().GetString("LOC_PVP_BOSS_ESCAPE"));
		}
		else if (!isPlayerBossWin)
		{
			Player remotePlayerByUserID = gameWorld.GetRemotePlayerByUserID(BossKiller);
			localPlayer.VSStatistics.ShowPublicInfo(infoBox, PlayerVSStatistics.EExpType.TO_PLAYER_BOSS_EXECUTED, remotePlayerByUserID, null, 0);
		}
		if (isPlayerBossWin)
		{
			if (localPlayer.IsBoss)
			{
				localPlayer.SetInBossWinState(true);
			}
			else
			{
				localPlayer.VSStatistics.ShowPublicInfo(infoBox, PlayerVSStatistics.EExpType.PLAYER_BOSS_WIN_SCORE, null, null, 0);
			}
		}
		bool flag = false;
		for (int i = 0; i < lastRank.Count; i++)
		{
			if (lastRank[i] == channelID)
			{
				int num = (int)((float)(totleScores[i] - localPlayer.VSStatistics.GetScore()) + (float)lastRoundScores[i] * 19f);
				if (localPlayer.IsBoss)
				{
					localPlayer.VSStatistics.MakePlayerBossWin(lastRoundScores[i]);
					if (isPlayerBossWin)
					{
						infoBox.SetMutantInfo(LocalizationManager.GetInstance().GetString("LOC_MUTANT_HUD3", lastDamage[i], num));
					}
					else
					{
						infoBox.SetMutantInfo(LocalizationManager.GetInstance().GetString("LOC_MUTANT_HUD4", lastDamage[i], num));
					}
				}
				else
				{
					localPlayer.VSStatistics.MakeKillPlayerBossAssist(lastRoundScores[i]);
					infoBox.SetMutantInfo(LocalizationManager.GetInstance().GetString("LOC_MUTANT_HUD5", lastDamage[i], num));
					Debug.Log("上次输出为 ： " + lastDamage[i]);
					localPlayer.VSStatistics.SetTotleDamageToPlayerBoss(totleDamage[i]);
				}
				localPlayer.VSStatistics.SetScore(totleScores[i]);
				Debug.Log("上次输出排名是" + i + " (反的)");
				Debug.Log("上局输出得分是" + lastRoundScores[i]);
				Debug.Log("总得分是" + totleScores[i]);
				flag = true;
			}
			else
			{
				RemotePlayer remotePlayerByUserID2 = gameWorld.GetRemotePlayerByUserID(lastRank[i]);
				Debug.Log("@@@@@@@@@@@" + lastRank[i]);
				if (remotePlayerByUserID2 != null)
				{
					Debug.Log(totleScores[i]);
					remotePlayerByUserID2.VSStatistics.SetScore(totleScores[i]);
					remotePlayerByUserID2.VSStatistics.SetTotleDamageToPlayerBoss(totleDamage[i]);
				}
			}
		}
		if (!flag)
		{
			Debug.Log("上局没有输出");
		}
		if (HUDBase.instance != null)
		{
			((HUDOnline)HUDBase.instance).OpenScorePage(true);
		}
		GiantBetrayalModeLogic giantBetrayalModeLogic = gameWorld.VsBattleLogic as GiantBetrayalModeLogic;
		if (giantBetrayalModeLogic != null)
		{
			giantBetrayalModeLogic.State = GiantBetrayalModeLogic.EGiantBetrayalState.WAIT_BOSS_CHOOSE_SKILL;
		}
	}
}
