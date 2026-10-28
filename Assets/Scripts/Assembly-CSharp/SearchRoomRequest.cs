public class SearchRoomRequest : Request
{
	public string mSearchText;

	public SearchRoomRequest(string text)
	{
		requestID = 10;
		mSearchText = text;
	}

	public override BytesBuffer GetBody()
	{
		byte stringByteLength = BytesBuffer.GetStringByteLength(mSearchText);
		BytesBuffer bytesBuffer = new BytesBuffer(stringByteLength);
		bytesBuffer.AddString(mSearchText);
		return bytesBuffer;
	}
}
