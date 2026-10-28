public class ChangePointControllerResponse : Response
{
	protected byte pointID;

	protected byte teamID;

	protected byte hadInLen;

	protected int[] hadInList;

	protected byte nowInLen;

	protected int[] nowInList;

	protected int blueScore;

	protected int redScore;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		pointID = bytesBuffer.ReadByte();
		teamID = bytesBuffer.ReadByte();
		hadInLen = bytesBuffer.ReadByte();
		hadInList = new int[hadInLen];
		for (int i = 0; i < hadInLen; i++)
		{
			hadInList[i] = bytesBuffer.ReadInt();
		}
		nowInLen = bytesBuffer.ReadByte();
		nowInList = new int[nowInLen];
		for (int j = 0; j < nowInLen; j++)
		{
			nowInList[j] = bytesBuffer.ReadInt();
		}
		blueScore = bytesBuffer.ReadInt();
		redScore = bytesBuffer.ReadInt();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		Player localPlayer = gameWorld.GetLocalPlayer();
		if (gameWorld != null)
		{
			if (gameWorld.VsBattleLogic != null)
			{
				gameWorld.VsBattleLogic.SetPointController(pointID, (TeamName)teamID);
				gameWorld.VsBattleLogic.SetScore(blueScore, redScore);
			}
			gameWorld.BattleInfo.TeamScores[0] = blueScore;
			gameWorld.BattleInfo.TeamScores[1] = redScore;
		}
		if (localPlayer == null)
		{
			return;
		}
		UserStateHUD.InfoBoxHUD infoBox = UserStateHUD.GetInstance().InfoBox;
		for (int i = 0; i < hadInLen; i++)
		{
			if (hadInList[i] == localPlayer.GetChannelID())
			{
				localPlayer.VSStatistics.MakeOccupationAssist();
				localPlayer.VSStatistics.UpdataScore();
				localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.OCCUPATION_ASSIST);
				continue;
			}
			RemotePlayer remotePlayerByUserID = GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(hadInList[i]);
			if (remotePlayerByUserID != null)
			{
				remotePlayerByUserID.VSStatistics.MakeOccupationAssist();
				remotePlayerByUserID.VSStatistics.UpdataScore();
			}
		}
		for (int j = 0; j < nowInLen; j++)
		{
			if (nowInList[j] == localPlayer.GetChannelID())
			{
				localPlayer.VSStatistics.MakeOccupation();
				localPlayer.VSStatistics.UpdataScore();
				localPlayer.VSStatistics.ShowPrivateInfo(infoBox, PlayerVSStatistics.EExpType.OCCUPATION);
				continue;
			}
			RemotePlayer remotePlayerByUserID2 = GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(nowInList[j]);
			if (remotePlayerByUserID2 != null)
			{
				remotePlayerByUserID2.VSStatistics.MakeOccupationAssist();
				remotePlayerByUserID2.VSStatistics.UpdataScore();
			}
		}
	}
}
