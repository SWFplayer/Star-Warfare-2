public class PlayerSetShieldResponse : Response
{
	private int ID;

	private int itemShield;

	private int maxItemShield;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		ID = bytesBuffer.ReadInt();
		itemShield = bytesBuffer.ReadInt();
		maxItemShield = bytesBuffer.ReadInt();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld != null && gameWorld.State == GameState.Playing)
		{
			Player playerByUserID = gameWorld.GetPlayerByUserID(ID);
			if (playerByUserID != null)
			{
				playerByUserID.SetShield(itemShield, maxItemShield);
			}
		}
	}
}
