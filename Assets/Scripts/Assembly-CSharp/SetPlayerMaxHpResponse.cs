public class SetPlayerMaxHpResponse : Response
{
	protected int m_PlayerID;

	protected int m_maxHp;

	protected bool modifyHp;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		m_PlayerID = bytesBuffer.ReadInt();
		m_maxHp = bytesBuffer.ReadInt();
		modifyHp = bytesBuffer.ReadBool();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null)
		{
			return;
		}
		Player playerByUserID = gameWorld.GetPlayerByUserID(m_PlayerID);
		if (playerByUserID != null)
		{
			if (modifyHp)
			{
				playerByUserID.MaxHp = m_maxHp;
				playerByUserID.Hp = m_maxHp;
			}
			else
			{
				float num = (float)playerByUserID.Hp / ((float)playerByUserID.MaxHp * 1f);
				playerByUserID.MaxHp = m_maxHp;
				playerByUserID.Hp = (int)((float)m_maxHp * num);
			}
		}
	}
}
