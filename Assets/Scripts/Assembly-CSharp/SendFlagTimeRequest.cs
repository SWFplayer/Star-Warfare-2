using UnityEngine;

public class SendFlagTimeRequest : Request
{
	protected int channelID;

	protected float time;

	public SendFlagTimeRequest(int channelID, float time)
	{
		requestID = 137;
		this.channelID = channelID;
		this.time = time;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(8);
		bytesBuffer.AddInt(channelID);
		bytesBuffer.AddFloat(time);
		Debug.Log(time);
		return bytesBuffer;
	}
}
