internal class PickUpItemResponse : Response
{
	protected int m_playerID;

	protected short m_sequenceID;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		m_playerID = bytesBuffer.ReadInt();
		m_sequenceID = bytesBuffer.ReadShort();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld != null && (gameWorld.State == GameState.Playing || !GameApp.GetInstance().GetGameMode().IsVSMode()))
		{
			GameApp.GetInstance().GetGameWorld().PlayerPickUpLoot(m_sequenceID, m_playerID);
		}
	}
}
