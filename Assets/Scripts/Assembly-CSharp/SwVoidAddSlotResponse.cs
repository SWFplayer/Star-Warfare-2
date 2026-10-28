public class SwVoidAddSlotResponse : Response
{
	private SwVoidAddSlotResponseData mData;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bb = new BytesBuffer(data);
		mData = new SwVoidAddSlotResponseData();
		mData.ReadData(bb);
	}

	public override void ProcessLogic()
	{
		SwVoidOnlineProcess.SwVoidAddSlotResponse(mData);
	}
}
