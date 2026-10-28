public class SwVoidIapChestResponse : Response
{
	private SwVoidIapChestResponseData mData;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bb = new BytesBuffer(data);
		mData = new SwVoidIapChestResponseData();
		mData.ReadData(bb);
	}

	public override void ProcessLogic()
	{
		SwVoidOnlineProcess.SwVoidShowChestReward(mData);
	}
}
