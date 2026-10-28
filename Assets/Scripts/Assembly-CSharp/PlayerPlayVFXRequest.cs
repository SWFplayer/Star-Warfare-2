public class PlayerPlayVFXRequest : Request
{
	protected short skillID;

	protected bool playOrDestroy;

	public PlayerPlayVFXRequest(short skillID, bool playerOrDestroy)
	{
		requestID = 258;
		this.skillID = skillID;
		playOrDestroy = playerOrDestroy;
	}

	public override BytesBuffer GetBody()
	{
		byte length = 3;
		BytesBuffer bytesBuffer = new BytesBuffer(length);
		bytesBuffer.AddShort(skillID);
		bytesBuffer.AddBool(playOrDestroy);
		return bytesBuffer;
	}
}
