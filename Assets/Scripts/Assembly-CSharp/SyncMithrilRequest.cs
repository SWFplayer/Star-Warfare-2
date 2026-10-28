using UnityEngine;

public class SyncMithrilRequest : Request
{
	public SyncMithrilRequest()
	{
		requestID = 72;
		Debug.Log("Sync Mithril");
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(4);
		bytesBuffer.AddInt(GameApp.GetInstance().GetUserState().GetMithril());
		return bytesBuffer;
	}
}
