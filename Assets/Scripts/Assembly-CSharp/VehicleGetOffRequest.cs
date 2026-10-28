using UnityEngine;

public class VehicleGetOffRequest : Request
{
	private byte mChangeState;

	public VehicleGetOffRequest(bool changeState)
	{
		Debug.Log("Send VGO Request");
		requestID = 153;
		mChangeState = (byte)(changeState ? 1 : 0);
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = null;
		bytesBuffer = new BytesBuffer(1);
		bytesBuffer.AddByte(mChangeState);
		return bytesBuffer;
	}
}
