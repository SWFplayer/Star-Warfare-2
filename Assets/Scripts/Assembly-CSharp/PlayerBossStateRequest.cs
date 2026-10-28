public class PlayerBossStateRequest : Request
{
	protected byte stateID;

	public PlayerBossStateRequest(byte id)
	{
		requestID = 252;
		stateID = id;
	}

	public PlayerBossStateRequest(GameUnitState state)
	{
		requestID = 252;
		PlayerBossState playerBossState = (PlayerBossState)state;
		stateID = playerBossState.ID;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(1);
		bytesBuffer.AddByte(stateID);
		return bytesBuffer;
	}
}
