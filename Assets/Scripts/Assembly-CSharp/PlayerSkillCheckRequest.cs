public class PlayerSkillCheckRequest : Request
{
	protected bool isOnCheck;

	protected byte triggerType;

	protected int skillId;

	public PlayerSkillCheckRequest(bool isOnCheck, byte triggerType, int skillId)
	{
		requestID = 155;
		this.isOnCheck = isOnCheck;
		this.triggerType = triggerType;
		this.skillId = skillId;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(6);
		bytesBuffer.AddBool(isOnCheck);
		bytesBuffer.AddByte(triggerType);
		bytesBuffer.AddInt(skillId);
		return bytesBuffer;
	}
}
