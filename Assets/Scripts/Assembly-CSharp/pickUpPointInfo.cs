public class pickUpPointInfo
{
	public byte idInOnline;

	public byte pickUpInfoId;

	public short itemId;

	public pickUpPointInfo()
	{
	}

	public pickUpPointInfo(byte idInOnline, byte pickUpInfoId, short itemId)
	{
		this.idInOnline = idInOnline;
		this.pickUpInfoId = pickUpInfoId;
		this.itemId = itemId;
	}
}
