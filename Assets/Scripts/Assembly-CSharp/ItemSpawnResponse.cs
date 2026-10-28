using UnityEngine;

internal class ItemSpawnResponse : Response
{
	protected short m_sequenceID;

	protected short m_x;

	protected short m_y;

	protected short m_z;

	protected byte m_itemID;

	protected short m_param1;

	protected short m_param2;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		m_sequenceID = bytesBuffer.ReadShort();
		m_x = bytesBuffer.ReadShort();
		m_y = bytesBuffer.ReadShort();
		m_z = bytesBuffer.ReadShort();
		m_itemID = bytesBuffer.ReadByte();
		m_param1 = bytesBuffer.ReadShort();
		m_param2 = bytesBuffer.ReadShort();
	}

	public override void ProcessLogic()
	{
		Vector3 position = new Vector3((float)m_x / 10f, (float)m_y / 10f, (float)m_z / 10f);
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld != null && gameWorld.State == GameState.Playing && gameWorld != null)
		{
			gameWorld.CreatePickUp((LootType)m_itemID, position, m_param1, m_param2, m_sequenceID);
		}
	}
}
