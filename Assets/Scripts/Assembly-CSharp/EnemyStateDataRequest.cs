public class EnemyStateDataRequest : Request
{
	public BytesBuffer Data;

	public EnemyStateDataRequest(int enemyId, byte stateId, int size)
	{
		requestID = 199;
		Data = new BytesBuffer(5 + size);
		Data.AddInt(enemyId);
		Data.AddByte(stateId);
	}

	public EnemyStateDataRequest(int enemyId, byte stateId)
	{
		requestID = 199;
		Data = new BytesBuffer();
		Data.AddInt(enemyId);
		Data.AddByte(stateId);
	}

	public override BytesBuffer GetBody()
	{
		return Data;
	}
}
