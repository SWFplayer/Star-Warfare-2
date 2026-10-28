using System.Collections.Generic;

public class PlayerBuffRequest : Request
{
	private List<int> playerList = new List<int>();

	private int SkillId;

	public PlayerBuffRequest(List<int> list, int SkillId)
	{
		requestID = 123;
		playerList = list;
		this.SkillId = SkillId;
	}

	public override BytesBuffer GetBody()
	{
		int count = playerList.Count;
		BytesBuffer bytesBuffer = new BytesBuffer(count * 4 + 5);
		bytesBuffer.AddByte((byte)playerList.Count);
		bytesBuffer.AddInt(SkillId);
		foreach (int player in playerList)
		{
			bytesBuffer.AddInt(player);
		}
		return bytesBuffer;
	}
}
