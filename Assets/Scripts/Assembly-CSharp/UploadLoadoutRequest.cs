public class UploadLoadoutRequest : Request
{
	private byte mCustomizeSize;

	private OnlineCustomizeData[] mCustomizeDataArray;

	public UploadLoadoutRequest()
	{
		requestID = 29;
		OnlinePlayer onlinePlayer = GameApp.GetInstance().GetUserState().GetOnlinePlayer();
		mCustomizeSize = (byte)onlinePlayer.GetOnlineCustomizeSize();
		mCustomizeDataArray = new OnlineCustomizeData[mCustomizeSize];
		for (int i = 0; i < mCustomizeSize; i++)
		{
			OnlineCustomizeData onlineCustomizeById = onlinePlayer.GetOnlineCustomizeById(i);
			mCustomizeDataArray[i] = new OnlineCustomizeData(onlineCustomizeById);
		}
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(512);
		bytesBuffer.AddByte(mCustomizeSize);
		for (int i = 0; i < mCustomizeSize; i++)
		{
			OnlineCustomizeRec100 onlineCustomizeRec = new OnlineCustomizeRec100(mCustomizeDataArray[i]);
			onlineCustomizeRec.WriteData(bytesBuffer);
		}
		return bytesBuffer;
	}
}
