using UnityEngine;

public class Rank
{
	private enum EPvpRankColumn
	{
		ID = 0,
		EXP = 1,
		RankName = 2
	}

	private int rowId;

	public int exp;

	public byte rankID;

	public string rankName;

	public Rank(int rowId)
	{
		this.rowId = (byte)rowId;
	}

	public void LoadConfig()
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.EQUIPMENT, DataSheetName.PVP_RANK);
		if (gameDataTable != null)
		{
			rankID = (byte)gameDataTable.GetInt(rowId, 0);
			exp = gameDataTable.GetInt(rowId, 1);
			rankName = gameDataTable.GetString(rowId, 2);
		}
	}

	public static string GetRankIconName(int rank)
	{
		return "rank_" + (Mathf.FloorToInt((rank - 1) / 5) + 1);
	}

	public static string GetRankLevelString(int rank)
	{
		if (rank >= 46)
		{
			return UIConstant.RANK_TOP_COLOR_PREFIX + rank + UIConstant.COLOR_POSTFIX;
		}
		return UIConstant.RANK_NORMAL_COLOR_PREFIX + rank + UIConstant.COLOR_POSTFIX;
	}
}
