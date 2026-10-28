internal class SwVoidGetRewardResponse : Response
{
	private SwVoidGetRewardResponseData mData;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bb = new BytesBuffer(data);
		mData = new SwVoidGetRewardResponseData();
		mData.ReadData(bb);
	}

	public override void ProcessLogic()
	{
		SwVoidOnlineProcess.SwVoidGetRewardResponse(mData);
	}
}
