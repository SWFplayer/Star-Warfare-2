public class PlayerBossActiveBufferResponse : Response
{
	protected short skillId;

	protected int playerId;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		playerId = bytesBuffer.ReadInt();
		skillId = bytesBuffer.ReadShort();
	}

	public override void ProcessLogic()
	{
		if (playerId == Lobby.GetInstance().GetChannelID())
		{
			LocalPlayer localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			if (localPlayer != null)
			{
				localPlayer.OnCastBossActiveBuffer(skillId);
			}
		}
	}
}
