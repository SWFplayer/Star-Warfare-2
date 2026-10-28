public class SwVoidFirstBossStaticMaterialsResponse : Response
{
	private SwVoidGetFirstBossStaticMaterialsResponseData mData;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bb = new BytesBuffer(data);
		mData = new SwVoidGetFirstBossStaticMaterialsResponseData();
		mData.ReadData(bb);
	}

	public override void ProcessLogic()
	{
		SwVoidOnlineProcess.GetFirstBossStaticMaterialsResponse(mData);
	}
}
