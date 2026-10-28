public class AddCompensateExpResponse : Response
{
	private byte pretige;

	private int exp;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		pretige = bytesBuffer.ReadByte();
		exp = bytesBuffer.ReadInt();
	}

	public override void ProcessLogic()
	{
		if (GameApp.GetInstance().GetUserState().GetOnlinePlayer() != null)
		{
			GameApp.GetInstance().GetUserState().GetOnlinePlayer()
				.SetPrestige(pretige);
			GameApp.GetInstance().GetUserState().GetOnlinePlayer()
				.SetExp(exp);
		}
	}
}
