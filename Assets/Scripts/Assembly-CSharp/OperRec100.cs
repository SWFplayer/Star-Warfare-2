public class OperRec100
{
	private OperatingInfo oper;

	public OperRec100(OperatingInfo oper)
	{
		this.oper = oper;
	}

	public void SaveData(BytesBuffer bb)
	{
		bb.AddString("120");
		bb.AddByte((byte)Global.TOTAL_STAGE);
		bb.AddByte((byte)Global.TOTAL_SUB_STAGE);
		for (int i = 0; i < Global.TOTAL_STAGE; i++)
		{
			for (int j = 0; j < Global.TOTAL_SUB_STAGE; j++)
			{
				bb.AddInt(oper.clearStageTime[i, j]);
			}
		}
		bb.AddInt(oper.soloDeathTimes);
		bb.AddByte((byte)oper.firstDeathLevelID.Count);
		for (int k = 0; k < oper.firstDeathLevelID.Count; k++)
		{
			bb.AddByte((byte)oper.firstDeathLevelID[k].Difficult);
			bb.AddByte(oper.firstDeathLevelID[k].StageID);
			bb.AddByte(oper.firstDeathLevelID[k].SubStageID);
		}
		bb.AddByte((byte)oper.recentlyDeathLevelID.Count);
		for (int l = 0; l < oper.recentlyDeathLevelID.Count; l++)
		{
			bb.AddByte((byte)oper.recentlyDeathLevelID[l].Difficult);
			bb.AddByte(oper.recentlyDeathLevelID[l].StageID);
			bb.AddByte(oper.recentlyDeathLevelID[l].SubStageID);
		}
		bb.AddInt(oper.assualtShootCount);
		bb.AddInt(oper.assualtHitCount);
		bb.AddByte((byte)oper.recentlyNormalBattleGetPieceTime.Count);
		for (int m = 0; m < oper.recentlyNormalBattleGetPieceTime.Count; m++)
		{
			bb.AddInt(oper.recentlyNormalBattleGetPieceTime[m]);
		}
		bb.AddByte((byte)oper.recentlyNormalBattleGetPieceCount.Count);
		for (int n = 0; n < oper.recentlyNormalBattleGetPieceCount.Count; n++)
		{
			bb.AddInt(oper.recentlyNormalBattleGetPieceCount[n]);
		}
		bb.AddByte((byte)oper.recentlyInfinityBattleGetPieceTime.Count);
		for (int num = 0; num < oper.recentlyInfinityBattleGetPieceTime.Count; num++)
		{
			bb.AddInt(oper.recentlyInfinityBattleGetPieceTime[num]);
		}
		bb.AddByte((byte)oper.recentlyInfinityBattleGetPieceCount.Count);
		for (int num2 = 0; num2 < oper.recentlyInfinityBattleGetPieceCount.Count; num2++)
		{
			bb.AddInt(oper.recentlyInfinityBattleGetPieceCount[num2]);
		}
	}

	public void LoadData(BytesBuffer bb)
	{
		byte b = bb.ReadByte();
		byte b2 = bb.ReadByte();
		for (int i = 0; i < b; i++)
		{
			for (int j = 0; j < b2; j++)
			{
				oper.clearStageTime[i, j] = bb.ReadInt();
			}
		}
		oper.soloDeathTimes = bb.ReadInt();
		byte b3 = bb.ReadByte();
		for (int k = 0; k < b3; k++)
		{
			LevelID item = new LevelID
			{
				Difficult = (EDifficultMode)bb.ReadByte(),
				StageID = bb.ReadByte(),
				SubStageID = bb.ReadByte()
			};
			oper.firstDeathLevelID.Add(item);
		}
		b3 = bb.ReadByte();
		for (int l = 0; l < b3; l++)
		{
			LevelID item2 = new LevelID
			{
				Difficult = (EDifficultMode)bb.ReadByte(),
				StageID = bb.ReadByte(),
				SubStageID = bb.ReadByte()
			};
			oper.recentlyDeathLevelID.Add(item2);
		}
		oper.assualtShootCount = bb.ReadInt();
		oper.assualtHitCount = bb.ReadInt();
		b3 = bb.ReadByte();
		for (int m = 0; m < b3; m++)
		{
			oper.recentlyNormalBattleGetPieceTime.Add(bb.ReadInt());
		}
		b3 = bb.ReadByte();
		for (int n = 0; n < b3; n++)
		{
			oper.recentlyNormalBattleGetPieceCount.Add(bb.ReadInt());
		}
		b3 = bb.ReadByte();
		for (int num = 0; num < b3; num++)
		{
			oper.recentlyInfinityBattleGetPieceTime.Add(bb.ReadInt());
		}
		b3 = bb.ReadByte();
		for (int num2 = 0; num2 < b3; num2++)
		{
			oper.recentlyInfinityBattleGetPieceCount.Add(bb.ReadInt());
		}
	}
}
