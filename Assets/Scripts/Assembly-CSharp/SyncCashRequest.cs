using UnityEngine;

public class SyncCashRequest : Request
{
	public SyncCashRequest()
	{
		requestID = 73;
		Debug.Log("Sync Cash");
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(4);
		bytesBuffer.AddInt(GameApp.GetInstance().GetUserState().GetCash());
		return bytesBuffer;
	}
}
