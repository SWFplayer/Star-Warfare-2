using UnityEngine;

public class EnemyStateRequest : Request
{
	protected short enemyID;

	protected byte state;

	protected short x;

	protected short y;

	protected short z;

	protected short sx;

	protected short sy;

	protected short sz;

	protected int targetID;

	protected byte targetPointID;

	public EnemyStateRequest(short enemyID, byte state, Vector3 pos)
	{
		requestID = 106;
		this.enemyID = enemyID;
		this.state = state;
		x = (short)(pos.x * 10f);
		y = (short)(pos.y * 10f);
		z = (short)(pos.z * 10f);
	}

	public EnemyStateRequest(short enemyID, byte state, Vector3 pos, Vector3 speed)
	{
		requestID = 106;
		this.enemyID = enemyID;
		this.state = state;
		x = (short)(pos.x * 10f);
		y = (short)(pos.y * 10f);
		z = (short)(pos.z * 10f);
		sx = (short)(speed.x * 10f);
		sy = (short)(speed.y * 10f);
		sz = (short)(speed.z * 10f);
	}

	public EnemyStateRequest(short enemyID, byte state, Vector3 pos, int targetID)
	{
		requestID = 106;
		this.enemyID = enemyID;
		this.state = state;
		x = (short)(pos.x * 10f);
		y = (short)(pos.y * 10f);
		z = (short)(pos.z * 10f);
		this.targetID = targetID;
	}

	public EnemyStateRequest(short enemyID, byte state, Vector3 pos, byte targetPointID)
	{
		requestID = 106;
		this.enemyID = enemyID;
		this.state = state;
		x = (short)(pos.x * 10f);
		y = (short)(pos.y * 10f);
		z = (short)(pos.z * 10f);
		this.targetPointID = targetPointID;
	}

	public EnemyStateRequest(short enemyID, EEnemyStateID stateID, Vector3 pos)
	{
		requestID = 106;
		this.enemyID = enemyID;
		state = (byte)stateID;
		x = (short)(pos.x * 10f);
		y = (short)(pos.y * 10f);
		z = (short)(pos.z * 10f);
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = null;
		bytesBuffer = new BytesBuffer(9);
		bytesBuffer.AddShort(enemyID);
		bytesBuffer.AddByte(state);
		bytesBuffer.AddShort(x);
		bytesBuffer.AddShort(y);
		bytesBuffer.AddShort(z);
		return bytesBuffer;
	}
}
