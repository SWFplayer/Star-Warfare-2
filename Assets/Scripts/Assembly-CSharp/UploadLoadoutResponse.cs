public class UploadLoadoutResponse : Response
{
	private byte mCustomizeSize;

	private OnlineCustomizeData[] mCustomizeDataArray;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		mCustomizeSize = bytesBuffer.ReadByte();
		mCustomizeDataArray = new OnlineCustomizeData[mCustomizeSize];
		for (int i = 0; i < mCustomizeSize; i++)
		{
			mCustomizeDataArray[i] = new OnlineCustomizeData();
			OnlineCustomizeRec100 onlineCustomizeRec = new OnlineCustomizeRec100(mCustomizeDataArray[i]);
			onlineCustomizeRec.ReadData(bytesBuffer);
		}
	}

	public override void ProcessLogic()
	{
		OnlineCustomizeData[] array = new OnlineCustomizeData[mCustomizeDataArray.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new OnlineCustomizeData(mCustomizeDataArray[i]);
		}
		GameApp.GetInstance().GetUserState().GetOnlinePlayer()
			.SetOnlineCustomizeArray(array);
	}
}
