public class BattlePickUpRequest : Request
{
	protected byte battlePickupId;

	public BattlePickUpRequest(byte battlePickupId)
	{
		requestID = 154;
		this.battlePickupId = battlePickupId;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(1);
		bytesBuffer.AddByte(battlePickupId);
		return bytesBuffer;
	}
}
