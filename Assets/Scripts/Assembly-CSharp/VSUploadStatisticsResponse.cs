internal class VSUploadStatisticsResponse : Response
{
	protected int m_playerID;

	protected short m_kills;

	protected short m_death;

	protected short m_assist;

	protected short m_goodAssist;

	protected short m_capture;

	protected short m_assassinate;

	protected short m_controlPoint;

	protected short m_controlPointAssist;

	protected short m_stopHim;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		m_playerID = bytesBuffer.ReadInt();
		m_kills = bytesBuffer.ReadShort();
		m_death = bytesBuffer.ReadShort();
		m_assist = bytesBuffer.ReadShort();
		m_goodAssist = bytesBuffer.ReadShort();
		m_capture = bytesBuffer.ReadShort();
		m_assassinate = bytesBuffer.ReadShort();
		m_controlPoint = bytesBuffer.ReadShort();
		m_controlPointAssist = bytesBuffer.ReadShort();
		m_stopHim = bytesBuffer.ReadShort();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null)
		{
			return;
		}
		RemotePlayer remotePlayerByUserID = gameWorld.GetRemotePlayerByUserID(m_playerID);
		if (remotePlayerByUserID != null)
		{
			if (GameApp.GetInstance().GetGameMode().ModePlay == Mode.GIANT_BETRAYAL)
			{
				remotePlayerByUserID.VSStatistics.PlayerBossKills = m_kills;
			}
			else
			{
				remotePlayerByUserID.VSStatistics.Kills = m_kills;
			}
			remotePlayerByUserID.VSStatistics.Death = m_death;
			remotePlayerByUserID.VSStatistics.Assist = m_assist;
			remotePlayerByUserID.VSStatistics.GoodAssist = m_goodAssist;
			remotePlayerByUserID.VSStatistics.Bonus = m_capture;
			remotePlayerByUserID.VSStatistics.CashReward = m_assassinate;
			remotePlayerByUserID.VSStatistics.SecureFlags = m_controlPoint;
			remotePlayerByUserID.VSStatistics.AssistFlags = m_controlPointAssist;
			remotePlayerByUserID.VSStatistics.StopHim = m_stopHim;
			remotePlayerByUserID.VSStatistics.UpdataScore();
		}
	}
}
