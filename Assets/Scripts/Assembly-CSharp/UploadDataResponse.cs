public class UploadDataResponse : Response
{
	private bool mIsRegist;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		mIsRegist = bytesBuffer.ReadBool();
	}

	public override void ProcessLogic()
	{
		if (MainMenuScript.mInstance != null)
		{
			MainMenuScript.mInstance.OnUploadDataComplete(mIsRegist);
		}
		else if (MainMenu2DScript.mInstance != null)
		{
			MainMenu2DScript.mInstance.OnUploadDataComplete(mIsRegist);
		}
	}
}
