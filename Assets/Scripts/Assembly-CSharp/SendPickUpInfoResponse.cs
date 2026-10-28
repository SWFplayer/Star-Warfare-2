using System.Collections.Generic;

public class SendPickUpInfoResponse : Response
{
	protected List<pickUpPointInfo> pickUpInfoList;

	protected byte listSize;

	protected List<byte> randomPickUpQueue;

	protected byte queueSize;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		listSize = bytesBuffer.ReadByte();
		pickUpInfoList = new List<pickUpPointInfo>();
		for (int i = 0; i < listSize; i++)
		{
			byte idInOnline = bytesBuffer.ReadByte();
			byte pickUpInfoId = bytesBuffer.ReadByte();
			short itemId = bytesBuffer.ReadShort();
			pickUpInfoList.Add(new pickUpPointInfo(idInOnline, pickUpInfoId, itemId));
		}
		queueSize = bytesBuffer.ReadByte();
		randomPickUpQueue = new List<byte>();
		for (int j = 0; j < queueSize; j++)
		{
			randomPickUpQueue.Add(bytesBuffer.ReadByte());
		}
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld != null && (gameWorld.State == GameState.Playing || !GameApp.GetInstance().GetGameMode().IsVSMode()))
		{
			RandomPickUpManagerScript.instance.SpawnItemFromOnlineForTheFirstTime(pickUpInfoList, randomPickUpQueue);
		}
	}
}
