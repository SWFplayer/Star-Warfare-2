internal class SwVoidUpgradeConfirmResponse : Response
{
	private SwVoidUpgradeConfirmResponseData mData;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bb = new BytesBuffer(data);
		mData = new SwVoidUpgradeConfirmResponseData();
		mData.ReadData(bb);
	}

	public override void ProcessLogic()
	{
		SwVoidOnlineProcess.SwVoidUpgradeConfirmResopnse(mData);
	}
}
