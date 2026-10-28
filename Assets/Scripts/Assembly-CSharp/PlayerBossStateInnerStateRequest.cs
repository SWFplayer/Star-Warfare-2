public class PlayerBossStateInnerStateRequest : Request
{
	protected byte stateID;

	protected byte InnerState;

	public PlayerBossStateInnerStateRequest(byte id, int innerState)
	{
		requestID = 253;
		stateID = id;
		InnerState = (byte)innerState;
	}

	public PlayerBossStateInnerStateRequest(GameUnitState state, int innerState)
	{
		requestID = 253;
		PlayerBossState playerBossState = (PlayerBossState)state;
		stateID = playerBossState.ID;
		InnerState = (byte)innerState;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(2);
		bytesBuffer.AddByte(stateID);
		bytesBuffer.AddByte(InnerState);
		return bytesBuffer;
	}
}
