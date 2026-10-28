using System.Collections.Generic;

public class NeighborPlayerRequest : Request
{
	private List<int> playerList = new List<int>();

	public NeighborPlayerRequest(List<int> list)
	{
		requestID = 171;
		playerList = list;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(21);
		bytesBuffer.AddByte((byte)playerList.Count);
		foreach (int player in playerList)
		{
			bytesBuffer.AddInt(player);
		}
		return bytesBuffer;
	}
}
