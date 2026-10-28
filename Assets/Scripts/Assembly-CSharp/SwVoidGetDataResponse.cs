internal class SwVoidGetDataResponse : Response
{
	private SwVoidGetDataResponseData mData;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bb = new BytesBuffer(data);
		mData = new SwVoidGetDataResponseData();
		mData.ReadData(bb);
	}

	public override void ProcessLogic()
	{
		SwVoidOnlineProcess.SwVoidGetDataResponse(mData);
	}
}
