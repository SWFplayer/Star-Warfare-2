public class DailyQuestData
{
	public byte index;

	public byte type;

	public byte questId;

	public byte valueId;

	public byte weaponId;

	public int value;

	public int valueGot;

	public byte hasDone;

	public byte hasRewardReceived;

	public byte isHeroic;

	public short rewardPoint;

	public void ReadData(BytesBuffer buffer)
	{
		index = buffer.ReadByte();
		type = buffer.ReadByte();
		questId = buffer.ReadByte();
		valueId = buffer.ReadByte();
		weaponId = buffer.ReadByte();
		value = buffer.ReadInt();
		valueGot = buffer.ReadInt();
		hasDone = buffer.ReadByte();
		hasRewardReceived = buffer.ReadByte();
		isHeroic = buffer.ReadByte();
		rewardPoint = buffer.ReadShort();
	}
}
