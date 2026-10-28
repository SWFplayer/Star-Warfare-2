internal class SwVoidGetRandomDropResponse : Response
{
	private SwVoidGetRandomDropResponseData mData;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bb = new BytesBuffer(data);
		mData = new SwVoidGetRandomDropResponseData();
		mData.ReadData(bb);
	}

	public override void ProcessLogic()
	{
		SwVoidOnlineProcess.SwVoidGetRandomDropResponse(mData);
	}
}
