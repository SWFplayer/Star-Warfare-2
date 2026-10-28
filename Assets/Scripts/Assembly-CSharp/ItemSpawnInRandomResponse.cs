public class ItemSpawnInRandomResponse : Response
{
	protected short m_sequenceID;

	protected byte pointID;

	protected byte pickupID;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		m_sequenceID = bytesBuffer.ReadShort();
		pointID = bytesBuffer.ReadByte();
		pickupID = bytesBuffer.ReadByte();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld != null && gameWorld.State == GameState.Playing && RandomPickUpManagerScript.instance != null)
		{
			RandomPickUpManagerScript.instance.SpawnItemWithPointPickUpID(m_sequenceID, pointID, pickupID, false);
		}
	}
}
