public class PlayerUseItemRequest : Request
{
	protected int mItemID;

	protected bool isCopySkill;

	public PlayerUseItemRequest(int itemID, bool isCopy)
	{
		requestID = 117;
		mItemID = itemID;
		isCopySkill = isCopy;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(5);
		bytesBuffer.AddInt(mItemID);
		bytesBuffer.AddByte((byte)(isCopySkill ? 1u : 0u));
		return bytesBuffer;
	}
}
