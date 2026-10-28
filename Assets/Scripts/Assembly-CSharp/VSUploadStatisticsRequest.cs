public class VSUploadStatisticsRequest : Request
{
	protected short kills;

	protected short death;

	protected short assist;

	protected short goodAssist;

	protected short capture;

	protected short assassinate;

	protected short controlPoint;

	protected short controlPointAssist;

	protected short stopHim;

	public VSUploadStatisticsRequest(short k, short d, short a, short g, short cap, short assa, short ctrl, short ctrlAssist, short stop)
	{
		requestID = 130;
		kills = k;
		death = d;
		assist = a;
		goodAssist = g;
		capture = cap;
		assassinate = assa;
		controlPoint = ctrl;
		controlPointAssist = ctrlAssist;
		stopHim = stop;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(18);
		bytesBuffer.AddShort(kills);
		bytesBuffer.AddShort(death);
		bytesBuffer.AddShort(assist);
		bytesBuffer.AddShort(capture);
		bytesBuffer.AddShort(goodAssist);
		bytesBuffer.AddShort(assassinate);
		bytesBuffer.AddShort(controlPoint);
		bytesBuffer.AddShort(controlPointAssist);
		bytesBuffer.AddShort(stopHim);
		return bytesBuffer;
	}
}
