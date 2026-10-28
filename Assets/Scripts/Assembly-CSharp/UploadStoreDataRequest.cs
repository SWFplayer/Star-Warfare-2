public class UploadStoreDataRequest : Request
{
	protected UserState userState;

	public UploadStoreDataRequest(UserState userState)
	{
		requestID = 31;
		this.userState = userState;
	}

	public override BytesBuffer GetBody()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		IRecordset recordset = RecordFactory.CreateRecord("120", userState);
		BytesBuffer bytesBuffer = new BytesBuffer(512);
		recordset.SaveGlobalBuffer(bytesBuffer);
		recordset.SaveEquipBuffer(bytesBuffer);
		return bytesBuffer;
	}
}
