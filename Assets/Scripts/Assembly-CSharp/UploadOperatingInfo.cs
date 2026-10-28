public class UploadOperatingInfo : Request
{
	private int mithrilRebirthTime;

	private int expendDollar;

	private string udid;

	public UploadOperatingInfo(int rebirthTime, int expendDols, string udid)
	{
		requestID = 23;
		mithrilRebirthTime = rebirthTime;
		expendDollar = expendDols;
		this.udid = udid;
	}

	public override BytesBuffer GetBody()
	{
		byte length = (byte)(8 + udid.Length + 1);
		BytesBuffer bytesBuffer = new BytesBuffer(length);
		bytesBuffer.AddInt(mithrilRebirthTime);
		bytesBuffer.AddInt(expendDollar);
		bytesBuffer.AddString(udid);
		return bytesBuffer;
	}
}
