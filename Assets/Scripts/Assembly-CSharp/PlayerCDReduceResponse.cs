internal class PlayerCDReduceResponse : Response
{
	private byte reduceTime;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		reduceTime = bytesBuffer.ReadByte();
	}

	public override void ProcessLogic()
	{
		if (GameApp.GetInstance().GetGameWorld() != null)
		{
			Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			if (localPlayer != null && localPlayer.InPlayingState() && HUDBase.instance != null)
			{
				HUDBase.instance.m_SkillHUDManager.ReduceCoolDown((int)reduceTime);
			}
		}
	}
}
