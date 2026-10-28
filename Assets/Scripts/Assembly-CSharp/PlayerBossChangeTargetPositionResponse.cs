using UnityEngine;

internal class PlayerBossChangeTargetPositionResponse : Response
{
	protected int playerID;

	protected short target_x;

	protected short target_y;

	protected short target_z;

	protected short m_x;

	protected short m_y;

	protected short m_z;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		playerID = bytesBuffer.ReadInt();
		target_x = bytesBuffer.ReadShort();
		target_y = bytesBuffer.ReadShort();
		target_z = bytesBuffer.ReadShort();
		m_x = bytesBuffer.ReadShort();
		m_y = bytesBuffer.ReadShort();
		m_z = bytesBuffer.ReadShort();
	}

	public override void ProcessLogic()
	{
		int channelID = Lobby.GetInstance().GetChannelID();
		if (playerID != channelID)
		{
			GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
			RemotePlayer remotePlayerByUserID = gameWorld.GetRemotePlayerByUserID(playerID);
			if (remotePlayerByUserID != null)
			{
				remotePlayerByUserID.SetTargetPosition(new Vector3((float)target_x / 10f, (float)target_y / 10f, (float)target_z / 10f));
			}
			else
			{
				Debug.Log("player is null");
			}
		}
	}
}
