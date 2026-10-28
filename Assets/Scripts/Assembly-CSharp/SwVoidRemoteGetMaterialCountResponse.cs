public class SwVoidRemoteGetMaterialCountResponse : Response
{
	private int remotePlayerId;

	private short materialCount;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		remotePlayerId = bytesBuffer.ReadInt();
		materialCount = bytesBuffer.ReadShort();
	}

	public override void ProcessLogic()
	{
		if (GameApp.GetInstance().GetGameWorld() != null)
		{
			RemotePlayer remotePlayerByUserID = GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(remotePlayerId);
			if (remotePlayerByUserID != null)
			{
				remotePlayerByUserID.SetSwVoidMaterialCount(materialCount);
			}
		}
	}
}
