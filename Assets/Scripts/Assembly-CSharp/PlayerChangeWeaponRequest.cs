public class PlayerChangeWeaponRequest : Request
{
	protected byte bagIndex;

	public PlayerChangeWeaponRequest(byte bagIndex)
	{
		requestID = 111;
		this.bagIndex = bagIndex;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(1);
		bytesBuffer.AddByte(bagIndex);
		return bytesBuffer;
	}
}
