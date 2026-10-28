public class UploadDataRequest : Request
{
	protected UserState userState;

	protected bool isRegist;

	public UploadDataRequest(UserState userState, bool isRegist)
	{
		requestID = 15;
		this.userState = userState;
		this.isRegist = isRegist;
	}

	public override BytesBuffer GetBody()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		IRecordset recordset = RecordFactory.CreateRecord("120", userState);
		BytesBuffer bytesBuffer = recordset.SaveBytesBuffer();
		bytesBuffer.AddBool(isRegist);
		return bytesBuffer;
	}
}
