public class PlayerBossStateDataRequest : Request
{
	public BytesBuffer Data;

	public PlayerBossStateDataRequest(int playerId, byte stateId, int size)
	{
		requestID = 255;
		Data = new BytesBuffer(5 + size);
		Data.AddInt(playerId);
		Data.AddByte(stateId);
	}

	public override BytesBuffer GetBody()
	{
		return Data;
	}
}
