public class SwVoidCheckServerResponse : Response
{
	private SwVoidCheckServerResponseData mData;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bb = new BytesBuffer(data);
		mData = new SwVoidCheckServerResponseData();
		mData.ReadData(bb);
	}

	public override void ProcessLogic()
	{
		SwVoidOnlineProcess.SwVoidCheckServerResponse(mData);
	}
}
