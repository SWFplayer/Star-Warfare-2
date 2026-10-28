public class UploadGlobalDataRequest : Request
{
	protected UserState userState;

	public UploadGlobalDataRequest(UserState userState)
	{
		requestID = 30;
		this.userState = userState;
	}

	public override BytesBuffer GetBody()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		IRecordset recordset = RecordFactory.CreateRecord("120", userState);
		BytesBuffer bytesBuffer = new BytesBuffer();
		recordset.SaveGlobalBuffer(bytesBuffer);
		return bytesBuffer;
	}
}
