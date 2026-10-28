using System.Collections.Generic;

public class GameAnalysisRec100
{
	private GameAnalysisManager mManager;

	public GameAnalysisRec100(GameAnalysisManager manager)
	{
		mManager = manager;
	}

	public void SaveData(BytesBuffer bb)
	{
		bb.AddString("120");
		bb.AddByte((byte)Global.TOTAL_STAGE);
		bb.AddByte((byte)Global.TOTAL_SUB_STAGE);
		PVEStageAnalysisInfo[,] normalSingleStageInfo = mManager.GetNormalSingleStageInfo();
		for (int i = 0; i < Global.TOTAL_STAGE; i++)
		{
			for (int j = 0; j < Global.TOTAL_SUB_STAGE; j++)
			{
				PVEStageAnalysisInfo pVEStageAnalysisInfo = normalSingleStageInfo[i, j];
				bb.AddInt(pVEStageAnalysisInfo.playTime);
				bb.AddInt(pVEStageAnalysisInfo.passTime);
				bb.AddInt(pVEStageAnalysisInfo.goldCollect);
				bb.AddInt(pVEStageAnalysisInfo.mithrilCollect);
				bb.AddInt(pVEStageAnalysisInfo.normalVoidMatCollect);
				bb.AddInt(pVEStageAnalysisInfo.uncommonVoidMatCollect);
				bb.AddInt(pVEStageAnalysisInfo.rareVoidMatCollect);
				bb.AddInt(pVEStageAnalysisInfo.RebirthTime);
				bb.AddInt(pVEStageAnalysisInfo.MithrilUsed);
			}
		}
		PVEStageAnalysisInfo[,] heroSingleStageInfo = mManager.GetHeroSingleStageInfo();
		for (int k = 0; k < Global.TOTAL_STAGE; k++)
		{
			for (int l = 0; l < Global.TOTAL_SUB_STAGE; l++)
			{
				PVEStageAnalysisInfo pVEStageAnalysisInfo2 = heroSingleStageInfo[k, l];
				bb.AddInt(pVEStageAnalysisInfo2.playTime);
				bb.AddInt(pVEStageAnalysisInfo2.passTime);
				bb.AddInt(pVEStageAnalysisInfo2.goldCollect);
				bb.AddInt(pVEStageAnalysisInfo2.mithrilCollect);
				bb.AddInt(pVEStageAnalysisInfo2.normalVoidMatCollect);
				bb.AddInt(pVEStageAnalysisInfo2.uncommonVoidMatCollect);
				bb.AddInt(pVEStageAnalysisInfo2.rareVoidMatCollect);
				bb.AddInt(pVEStageAnalysisInfo2.RebirthTime);
				bb.AddInt(pVEStageAnalysisInfo2.MithrilUsed);
			}
		}
		bb.AddByte(2);
		bb.AddByte((byte)Global.TOTAL_STAGE);
		PVEStageAnalysisInfo[,] onlineSurvivalInfo = mManager.GetOnlineSurvivalInfo();
		for (int m = 0; m < 2; m++)
		{
			for (int n = 0; n < Global.TOTAL_STAGE; n++)
			{
				PVEStageAnalysisInfo pVEStageAnalysisInfo3 = onlineSurvivalInfo[m, n];
				bb.AddInt(pVEStageAnalysisInfo3.playTime);
				bb.AddInt(pVEStageAnalysisInfo3.passTime);
				bb.AddInt(pVEStageAnalysisInfo3.goldCollect);
				bb.AddInt(pVEStageAnalysisInfo3.mithrilCollect);
				bb.AddInt(pVEStageAnalysisInfo3.normalVoidMatCollect);
				bb.AddInt(pVEStageAnalysisInfo3.uncommonVoidMatCollect);
				bb.AddInt(pVEStageAnalysisInfo3.rareVoidMatCollect);
				bb.AddInt(pVEStageAnalysisInfo3.RebirthTime);
				bb.AddInt(pVEStageAnalysisInfo3.MithrilUsed);
			}
		}
		bb.AddByte(2);
		bb.AddByte((byte)Global.TOTAL_STAGE);
		PVEStageAnalysisInfo[,] onlineBossInfo = mManager.GetOnlineBossInfo();
		for (int num = 0; num < 2; num++)
		{
			for (int num2 = 0; num2 < Global.TOTAL_STAGE; num2++)
			{
				PVEStageAnalysisInfo pVEStageAnalysisInfo4 = onlineBossInfo[num, num2];
				bb.AddInt(pVEStageAnalysisInfo4.playTime);
				bb.AddInt(pVEStageAnalysisInfo4.passTime);
				bb.AddInt(pVEStageAnalysisInfo4.goldCollect);
				bb.AddInt(pVEStageAnalysisInfo4.mithrilCollect);
				bb.AddInt(pVEStageAnalysisInfo4.normalVoidMatCollect);
				bb.AddInt(pVEStageAnalysisInfo4.uncommonVoidMatCollect);
				bb.AddInt(pVEStageAnalysisInfo4.rareVoidMatCollect);
				bb.AddInt(pVEStageAnalysisInfo4.RebirthTime);
				bb.AddInt(pVEStageAnalysisInfo4.MithrilUsed);
			}
		}
		bb.AddByte(2);
		bb.AddByte(2);
		PVEStageAnalysisInfo[,] twinBossInfo = mManager.GetTwinBossInfo();
		for (int num3 = 0; num3 < 2; num3++)
		{
			for (int num4 = 0; num4 < 2; num4++)
			{
				PVEStageAnalysisInfo pVEStageAnalysisInfo5 = twinBossInfo[num3, num4];
				bb.AddInt(pVEStageAnalysisInfo5.playTime);
				bb.AddInt(pVEStageAnalysisInfo5.passTime);
				bb.AddInt(pVEStageAnalysisInfo5.goldCollect);
				bb.AddInt(pVEStageAnalysisInfo5.mithrilCollect);
				bb.AddInt(pVEStageAnalysisInfo5.normalVoidMatCollect);
				bb.AddInt(pVEStageAnalysisInfo5.uncommonVoidMatCollect);
				bb.AddInt(pVEStageAnalysisInfo5.rareVoidMatCollect);
				bb.AddInt(pVEStageAnalysisInfo5.RebirthTime);
				bb.AddInt(pVEStageAnalysisInfo5.MithrilUsed);
			}
		}
		bb.AddByte(6);
		PVPAnalysisInfo[] pvpInfo = mManager.GetPvpInfo();
		for (int num5 = 0; num5 < 6; num5++)
		{
			PVPAnalysisInfo pVPAnalysisInfo = pvpInfo[num5];
			bb.AddInt(pVPAnalysisInfo.playTime);
			bb.AddInt(pVPAnalysisInfo.rebirthTime);
			bb.AddInt(pVPAnalysisInfo.mithrilUsed);
		}
		bb.AddByte(6);
		bb.AddByte(6);
		SwVoidAnalysisInfo[] voidInfo = mManager.GetVoidInfo();
		for (int num6 = 0; num6 < 6; num6++)
		{
			SwVoidAnalysisInfo swVoidAnalysisInfo = voidInfo[num6];
			for (int num7 = 0; num7 < 6; num7++)
			{
				bb.AddInt(swVoidAnalysisInfo.UpgradeNum[num7]);
			}
			for (int num8 = 0; num8 < 6; num8++)
			{
				bb.AddInt(swVoidAnalysisInfo.CancelNum[num8]);
			}
			for (int num9 = 0; num9 < 6; num9++)
			{
				bb.AddInt(swVoidAnalysisInfo.ResolveNum[num9]);
			}
		}
		bb.AddByte(20);
		int[] pveRecord = mManager.GetPveRecord();
		for (int num10 = 0; num10 < 20; num10++)
		{
			bb.AddInt(pveRecord[num10]);
		}
		bb.AddByte(20);
		int[] pvpRecord = mManager.GetPvpRecord();
		for (int num11 = 0; num11 < 20; num11++)
		{
			bb.AddInt(pvpRecord[num11]);
		}
		bb.AddByte(34);
		int[] itemBuyNum = mManager.GetItemBuyNum();
		for (int num12 = 0; num12 < 34; num12++)
		{
			bb.AddInt(itemBuyNum[num12]);
		}
		bb.AddByte(7);
		int[] goldBuyNum = mManager.GetGoldBuyNum();
		for (int num13 = 0; num13 < 7; num13++)
		{
			bb.AddInt(goldBuyNum[num13]);
		}
		bb.AddByte(7);
		int[] mithrilBuyNum = mManager.GetMithrilBuyNum();
		for (int num14 = 0; num14 < 7; num14++)
		{
			bb.AddInt(mithrilBuyNum[num14]);
		}
		List<int> packageBuyNum = mManager.GetPackageBuyNum();
		bb.AddByte((byte)packageBuyNum.Count);
		foreach (int item in packageBuyNum)
		{
			bb.AddInt(item);
		}
		bb.AddInt(mManager.mithrilInfo.DailyBonusMithril);
		bb.AddInt(mManager.mithrilInfo.DailyTaskMithril);
		bb.AddInt(mManager.mithrilInfo.ADMithril);
		bb.AddInt(mManager.mithrilInfo.OfficialMithril);
		bb.AddInt(mManager.goldInfo.DailyBonusGold);
		bb.AddInt(mManager.goldInfo.DailyTaskGold);
		bb.AddInt(mManager.goldInfo.ADGold);
		bb.AddInt(mManager.goldInfo.OfficialGold);
	}

	public void LoadData(BytesBuffer bb)
	{
		byte b = bb.ReadByte();
		byte b2 = bb.ReadByte();
		PVEStageAnalysisInfo[,] normalSingleStageInfo = mManager.GetNormalSingleStageInfo();
		for (int i = 0; i < b; i++)
		{
			for (int j = 0; j < b2; j++)
			{
				PVEStageAnalysisInfo pVEStageAnalysisInfo = normalSingleStageInfo[i, j];
				pVEStageAnalysisInfo.playTime = bb.ReadInt();
				pVEStageAnalysisInfo.passTime = bb.ReadInt();
				pVEStageAnalysisInfo.goldCollect = bb.ReadInt();
				pVEStageAnalysisInfo.mithrilCollect = bb.ReadInt();
				pVEStageAnalysisInfo.normalVoidMatCollect = bb.ReadInt();
				pVEStageAnalysisInfo.uncommonVoidMatCollect = bb.ReadInt();
				pVEStageAnalysisInfo.rareVoidMatCollect = bb.ReadInt();
				pVEStageAnalysisInfo.RebirthTime = bb.ReadInt();
				pVEStageAnalysisInfo.MithrilUsed = bb.ReadInt();
			}
		}
		PVEStageAnalysisInfo[,] heroSingleStageInfo = mManager.GetHeroSingleStageInfo();
		for (int k = 0; k < b; k++)
		{
			for (int l = 0; l < b2; l++)
			{
				PVEStageAnalysisInfo pVEStageAnalysisInfo2 = heroSingleStageInfo[k, l];
				pVEStageAnalysisInfo2.playTime = bb.ReadInt();
				pVEStageAnalysisInfo2.passTime = bb.ReadInt();
				pVEStageAnalysisInfo2.goldCollect = bb.ReadInt();
				pVEStageAnalysisInfo2.mithrilCollect = bb.ReadInt();
				pVEStageAnalysisInfo2.normalVoidMatCollect = bb.ReadInt();
				pVEStageAnalysisInfo2.uncommonVoidMatCollect = bb.ReadInt();
				pVEStageAnalysisInfo2.rareVoidMatCollect = bb.ReadInt();
				pVEStageAnalysisInfo2.RebirthTime = bb.ReadInt();
				pVEStageAnalysisInfo2.MithrilUsed = bb.ReadInt();
			}
		}
		byte b3 = bb.ReadByte();
		b = bb.ReadByte();
		PVEStageAnalysisInfo[,] onlineSurvivalInfo = mManager.GetOnlineSurvivalInfo();
		for (int m = 0; m < b3; m++)
		{
			for (int n = 0; n < b; n++)
			{
				PVEStageAnalysisInfo pVEStageAnalysisInfo3 = onlineSurvivalInfo[m, n];
				pVEStageAnalysisInfo3.playTime = bb.ReadInt();
				pVEStageAnalysisInfo3.passTime = bb.ReadInt();
				pVEStageAnalysisInfo3.goldCollect = bb.ReadInt();
				pVEStageAnalysisInfo3.mithrilCollect = bb.ReadInt();
				pVEStageAnalysisInfo3.normalVoidMatCollect = bb.ReadInt();
				pVEStageAnalysisInfo3.uncommonVoidMatCollect = bb.ReadInt();
				pVEStageAnalysisInfo3.rareVoidMatCollect = bb.ReadInt();
				pVEStageAnalysisInfo3.RebirthTime = bb.ReadInt();
				pVEStageAnalysisInfo3.MithrilUsed = bb.ReadInt();
			}
		}
		b3 = bb.ReadByte();
		b = bb.ReadByte();
		PVEStageAnalysisInfo[,] onlineBossInfo = mManager.GetOnlineBossInfo();
		for (int num = 0; num < b3; num++)
		{
			for (int num2 = 0; num2 < b; num2++)
			{
				PVEStageAnalysisInfo pVEStageAnalysisInfo4 = onlineBossInfo[num, num2];
				pVEStageAnalysisInfo4.playTime = bb.ReadInt();
				pVEStageAnalysisInfo4.passTime = bb.ReadInt();
				pVEStageAnalysisInfo4.goldCollect = bb.ReadInt();
				pVEStageAnalysisInfo4.mithrilCollect = bb.ReadInt();
				pVEStageAnalysisInfo4.normalVoidMatCollect = bb.ReadInt();
				pVEStageAnalysisInfo4.uncommonVoidMatCollect = bb.ReadInt();
				pVEStageAnalysisInfo4.rareVoidMatCollect = bb.ReadInt();
				pVEStageAnalysisInfo4.RebirthTime = bb.ReadInt();
				pVEStageAnalysisInfo4.MithrilUsed = bb.ReadInt();
			}
		}
		b3 = bb.ReadByte();
		byte b4 = bb.ReadByte();
		PVEStageAnalysisInfo[,] twinBossInfo = mManager.GetTwinBossInfo();
		for (int num3 = 0; num3 < b3; num3++)
		{
			for (int num4 = 0; num4 < b4; num4++)
			{
				PVEStageAnalysisInfo pVEStageAnalysisInfo5 = twinBossInfo[num3, num4];
				pVEStageAnalysisInfo5.playTime = bb.ReadInt();
				pVEStageAnalysisInfo5.passTime = bb.ReadInt();
				pVEStageAnalysisInfo5.goldCollect = bb.ReadInt();
				pVEStageAnalysisInfo5.mithrilCollect = bb.ReadInt();
				pVEStageAnalysisInfo5.normalVoidMatCollect = bb.ReadInt();
				pVEStageAnalysisInfo5.uncommonVoidMatCollect = bb.ReadInt();
				pVEStageAnalysisInfo5.rareVoidMatCollect = bb.ReadInt();
				pVEStageAnalysisInfo5.RebirthTime = bb.ReadInt();
				pVEStageAnalysisInfo5.MithrilUsed = bb.ReadInt();
			}
		}
		byte b5 = bb.ReadByte();
		PVPAnalysisInfo[] pvpInfo = mManager.GetPvpInfo();
		for (int num5 = 0; num5 < b5; num5++)
		{
			PVPAnalysisInfo pVPAnalysisInfo = pvpInfo[num5];
			pVPAnalysisInfo.playTime = bb.ReadInt();
			pVPAnalysisInfo.rebirthTime = bb.ReadInt();
			pVPAnalysisInfo.mithrilUsed = bb.ReadInt();
		}
		byte b6 = bb.ReadByte();
		byte b7 = bb.ReadByte();
		SwVoidAnalysisInfo[] voidInfo = mManager.GetVoidInfo();
		for (int num6 = 0; num6 < b6; num6++)
		{
			SwVoidAnalysisInfo swVoidAnalysisInfo = voidInfo[num6];
			for (int num7 = 0; num7 < b7; num7++)
			{
				swVoidAnalysisInfo.UpgradeNum[num7] = bb.ReadInt();
			}
			for (int num8 = 0; num8 < b7; num8++)
			{
				swVoidAnalysisInfo.CancelNum[num8] = bb.ReadInt();
			}
			for (int num9 = 0; num9 < b7; num9++)
			{
				swVoidAnalysisInfo.ResolveNum[num9] = bb.ReadInt();
			}
		}
		byte b8 = bb.ReadByte();
		int[] pveRecord = mManager.GetPveRecord();
		for (int num10 = 0; num10 < b8; num10++)
		{
			pveRecord[num10] = bb.ReadInt();
		}
		b8 = bb.ReadByte();
		int[] pvpRecord = mManager.GetPvpRecord();
		for (int num11 = 0; num11 < b8; num11++)
		{
			pvpRecord[num11] = bb.ReadInt();
		}
		byte b9 = bb.ReadByte();
		int[] itemBuyNum = mManager.GetItemBuyNum();
		for (int num12 = 0; num12 < b9; num12++)
		{
			itemBuyNum[num12] = bb.ReadInt();
		}
		byte b10 = bb.ReadByte();
		int[] goldBuyNum = mManager.GetGoldBuyNum();
		for (int num13 = 0; num13 < b10; num13++)
		{
			goldBuyNum[num13] = bb.ReadInt();
		}
		byte b11 = bb.ReadByte();
		int[] mithrilBuyNum = mManager.GetMithrilBuyNum();
		for (int num14 = 0; num14 < b11; num14++)
		{
			mithrilBuyNum[num14] = bb.ReadInt();
		}
		List<int> packageBuyNum = mManager.GetPackageBuyNum();
		byte b12 = bb.ReadByte();
		for (int num15 = 0; num15 < b12; num15++)
		{
			int item = bb.ReadInt();
			packageBuyNum.Add(item);
		}
		mManager.mithrilInfo.DailyBonusMithril = bb.ReadInt();
		mManager.mithrilInfo.DailyTaskMithril = bb.ReadInt();
		mManager.mithrilInfo.ADMithril = bb.ReadInt();
		mManager.mithrilInfo.OfficialMithril = bb.ReadInt();
		mManager.goldInfo.DailyBonusGold = bb.ReadInt();
		mManager.goldInfo.DailyTaskGold = bb.ReadInt();
		mManager.goldInfo.ADGold = bb.ReadInt();
		mManager.goldInfo.OfficialGold = bb.ReadInt();
	}
}
