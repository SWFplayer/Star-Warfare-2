public class OnlineCustomizeUserStateInfo100
{
	private OnlineCustomizeData mData;

	public OnlineCustomizeUserStateInfo100(OnlineCustomizeData data)
	{
		mData = data;
	}

	public void ReadData(BytesBuffer bb)
	{
		mData.swVoidData = new SwVoidResponseData();
		if (bb.ReadIntNoOffset() == 0)
		{
			bb.ReadInt();
		}
		else
		{
			mData.swVoidData.ReadData(bb);
		}
	}
}
