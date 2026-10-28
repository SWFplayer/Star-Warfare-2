using UnityEngine;

public class SendPointInfoRequest : Request
{
	protected int channelID;

	protected byte pointCount;

	protected bool[] isNeutral;

	protected byte[] mControllerID;

	protected float[] mBalance;

	protected float mTeamBlueScore;

	protected float mTeamRedScore;

	protected short len;

	public SendPointInfoRequest(int channelID, bool neutral, TeamName controllerID, float blueScore, float redScore, float balance)
	{
		requestID = 141;
		this.channelID = channelID;
		pointCount = 1;
		isNeutral = new bool[pointCount];
		isNeutral[0] = neutral;
		mControllerID = new byte[pointCount];
		mControllerID[0] = (byte)controllerID;
		mTeamBlueScore = blueScore;
		mTeamRedScore = redScore;
		mBalance = new float[pointCount];
		mBalance[0] = balance;
		len = 19;
	}

	public SendPointInfoRequest(byte pointCount, int channelID, bool[] neutral, TeamName[] controllerID, float blueScore, float redScore, PointScript[] point)
	{
		requestID = 141;
		this.channelID = channelID;
		this.pointCount = pointCount;
		isNeutral = new bool[pointCount];
		mControllerID = new byte[pointCount];
		mBalance = new float[pointCount];
		mTeamBlueScore = blueScore;
		mTeamRedScore = redScore;
		len = 13;
		for (int i = 0; i < pointCount; i++)
		{
			isNeutral[i] = neutral[i];
			mControllerID[i] = (byte)controllerID[i];
			mBalance[i] = (int)(byte)point[i].mBalance;
			len += 6;
		}
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(len);
		bytesBuffer.AddInt(channelID);
		bytesBuffer.AddByte(pointCount);
		for (int i = 0; i < pointCount; i++)
		{
			bytesBuffer.AddBool(isNeutral[i]);
			bytesBuffer.AddByte(mControllerID[i]);
			bytesBuffer.AddFloat(mBalance[i]);
		}
		bytesBuffer.AddFloat(mTeamBlueScore);
		bytesBuffer.AddFloat(mTeamRedScore);
		Debug.Log("request :  controller : " + mControllerID[0] + " neutral : " + isNeutral[0] + " balance : " + mBalance[0] + " blue : " + mTeamBlueScore + " red : " + mTeamRedScore);
		return bytesBuffer;
	}
}
