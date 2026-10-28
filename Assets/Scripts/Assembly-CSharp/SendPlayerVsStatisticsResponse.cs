using System.Collections.Generic;

public class SendPlayerVsStatisticsResponse : Response
{
	private Dictionary<byte, int> expInfoList = new Dictionary<byte, int>();

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		int num = bytesBuffer.ReadByte();
		for (int i = 0; i < num; i++)
		{
			byte key = bytesBuffer.ReadByte();
			int value = bytesBuffer.ReadInt();
			expInfoList.Add(key, value);
		}
	}

	public override void ProcessLogic()
	{
	}
}
