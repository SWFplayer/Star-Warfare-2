public class SwDailyReward
{
	private byte type;

	private short id;

	private int value;

	public void ReadData(BytesBuffer buffer)
	{
		type = buffer.ReadByte();
		id = buffer.ReadShort();
		value = buffer.ReadInt();
	}

	public byte GetRewardType()
	{
		return type;
	}

	public short GetRewardId()
	{
		return id;
	}

	public int GetRewardValue()
	{
		return value;
	}
}
