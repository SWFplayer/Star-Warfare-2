using System.Collections.Generic;

public class SendPickUpInfoRequest : Request
{
	protected int newPlayerId;

	protected List<pickUpPointInfo> pickUpInfoList;

	protected byte listSize;

	protected List<byte> randomPickUpQueue;

	protected byte queueSize;

	public SendPickUpInfoRequest(List<pickUpPointInfo> pickUpInfoList, List<byte> randomPickUpQueue, int newPlayerId)
	{
		requestID = 151;
		this.newPlayerId = newPlayerId;
		this.pickUpInfoList = pickUpInfoList;
		listSize = (byte)pickUpInfoList.Count;
		this.randomPickUpQueue = randomPickUpQueue;
		queueSize = (byte)randomPickUpQueue.Count;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = null;
		bytesBuffer = new BytesBuffer(listSize * 4 + queueSize + 2 + 4);
		bytesBuffer.AddInt(newPlayerId);
		bytesBuffer.AddByte(listSize);
		foreach (pickUpPointInfo pickUpInfo in pickUpInfoList)
		{
			bytesBuffer.AddByte(pickUpInfo.idInOnline);
			bytesBuffer.AddByte(pickUpInfo.pickUpInfoId);
			bytesBuffer.AddShort(pickUpInfo.itemId);
		}
		bytesBuffer.AddByte(queueSize);
		foreach (byte item in randomPickUpQueue)
		{
			bytesBuffer.AddByte(item);
		}
		return bytesBuffer;
	}
}
