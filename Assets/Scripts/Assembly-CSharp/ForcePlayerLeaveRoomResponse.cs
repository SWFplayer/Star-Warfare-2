public class ForcePlayerLeaveRoomResponse : Response
{
	protected byte forceLeaveId;

	protected bool isMutantEnable;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		forceLeaveId = bytesBuffer.ReadByte();
		isMutantEnable = bytesBuffer.ReadBool();
	}

	public override void ProcessLogic()
	{
		if (forceLeaveId == 0)
		{
			Lobby.GetInstance().EnableMutantMode = isMutantEnable;
			if (HUDBase.instance != null)
			{
				((HUDOnline)HUDBase.instance).ForceExit();
			}
		}
	}
}
