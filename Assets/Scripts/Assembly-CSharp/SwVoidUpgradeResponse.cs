internal class SwVoidUpgradeResponse : Response
{
	private SwVoidUpgradeResponseData mData;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bb = new BytesBuffer(data);
		mData = new SwVoidUpgradeResponseData();
		mData.ReadData(bb);
	}

	public override void ProcessLogic()
	{
		SwVoidOnlineProcess.SwVoidUpgradeResponse(mData);
	}
}
