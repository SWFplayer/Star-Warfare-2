using System.Collections.Generic;

public class ChangePointControllerRequest : Request
{
	protected byte pointID;

	protected byte teamID;

	protected byte hadInLen;

	protected List<int> hadInList;

	protected byte nowInLen;

	protected List<int> nowInList;

	protected int blueScore;

	protected int redScore;

	protected byte bluePointCount;

	protected byte redPointCount;

	public ChangePointControllerRequest(byte pointID, byte teamID, Dictionary<int, PointScript.EPlayerCacthPointState> menberList)
	{
		requestID = 140;
		this.pointID = pointID;
		this.teamID = teamID;
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null)
		{
			return;
		}
		hadInLen = 0;
		nowInLen = 0;
		hadInList = new List<int>();
		nowInList = new List<int>();
		foreach (KeyValuePair<int, PointScript.EPlayerCacthPointState> menber in menberList)
		{
			Player playerByUserID = gameWorld.GetPlayerByUserID(menber.Key);
			if (playerByUserID != null && playerByUserID.Team == (TeamName)teamID)
			{
				if (menber.Value == PointScript.EPlayerCacthPointState.HAD_IN)
				{
					hadInLen++;
					hadInList.Add(menber.Key);
				}
				else if (menber.Value == PointScript.EPlayerCacthPointState.NOW_IN)
				{
					nowInLen++;
					nowInList.Add(menber.Key);
				}
			}
		}
		blueScore = gameWorld.BattleInfo.TeamScores[0];
		redScore = gameWorld.BattleInfo.TeamScores[1];
		VsModeLogic vsBattleLogic = gameWorld.VsBattleLogic;
		if (vsBattleLogic != null)
		{
			bluePointCount = vsBattleLogic.GetPointCountForecast(TeamName.Blue, pointID, (TeamName)teamID);
			redPointCount = vsBattleLogic.GetPointCountForecast(TeamName.Red, pointID, (TeamName)teamID);
		}
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(3 + hadInLen * 4 + 1 + nowInLen * 4 + 8 + 2);
		bytesBuffer.AddByte(pointID);
		bytesBuffer.AddByte(teamID);
		bytesBuffer.AddByte(hadInLen);
		foreach (int hadIn in hadInList)
		{
			bytesBuffer.AddInt(hadIn);
		}
		bytesBuffer.AddByte(nowInLen);
		foreach (int nowIn in nowInList)
		{
			bytesBuffer.AddInt(nowIn);
		}
		bytesBuffer.AddInt(blueScore);
		bytesBuffer.AddInt(redScore);
		bytesBuffer.AddByte(bluePointCount);
		bytesBuffer.AddByte(redPointCount);
		return bytesBuffer;
	}
}
