internal class PlayerOnKnockedResponse : Response
{
	private int playerID;

	private short speed;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		playerID = bytesBuffer.ReadInt();
		speed = bytesBuffer.ReadShort();
	}

	public override void ProcessLogic()
	{
		Player playerByUserID = GameApp.GetInstance().GetGameWorld().GetPlayerByUserID(playerID);
		if (playerByUserID != null)
		{
			playerByUserID.OnKnocked((float)speed / 100f);
		}
	}
}
